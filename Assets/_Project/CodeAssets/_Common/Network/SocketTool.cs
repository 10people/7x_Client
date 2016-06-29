//#define DEBUG_SOCKET_STATE

//#define DEBUG_SOCKET_CONNECT

//#define PROTO_TOOL

//#define DEBUG_WAITING

//#define DEBUG_CLEAR

//#define DEBUG_SEND

//#define DEBUG_RECEIVE

//#define DEBUG_PROCESSOR_COST



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using System.IO;

using qxmobile;
using qxmobile.protobuf;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2013.xx.xx
 * @since:		Unity 4.5.3
 * Function:	Manage socket connections.
 * 
 * Notes:
 * None.
 */ 
public class SocketTool : MonoBehaviour, SocketProcessor, SocketListener {

	#region Socket Config

	public bool m_log_socket_info	= false;

	public bool m_log_socket_processor = false;

	/// gbk
	private const string SOCKET_HEAD_MESSAGE = "tgw_l7_forward\r\nHost:app12345.qzoneapp.com:80\r\n\r\n";
	private bool m_try_send_head_message = true;

	public const string SERVER_PREFIX_DA_LI = "192.168.0.89";				// 大力
	public const int SERVER_PORT_DA_LI = 8586;
	
	public const string SERVER_PREFIX_JIAN_HU = "192.168.0.176";			// 建虎
	public const int SERVER_PORT_JIAN_HU = 8586;
	
	public const string SERVER_PREFIX_INNER_INDIE = "192.168.3.80";			// 内网独立
	public const int SERVER_PORT_INNER_INDIE = 8586;
	
	public const string SERVER_PREFIX_TX = "183.60.77.250";					// TX
	
	public const string SERVER_LIZHAOWEN = "192.168.0.180"; 				//李照文

	
	private const bool DEBUG_SOCKET = true;
	
	private const int SOCKET_TIME_OUT	= 3000;
		
	
	private static string m_server_prefix    = SERVER_PREFIX_INNER_INDIE;

	private static int m_server_port			= SERVER_PORT_JIAN_HU;


	public static void SetServerPrefix( string p_server_prefix, int p_server_port = SERVER_PORT_JIAN_HU ){
//		Debug.Log( "SocketTool.SetServerPrefix( " + p_server_prefix + " : " + p_server_port + " )" );
		m_server_prefix = p_server_prefix;
		
		m_server_port = p_server_port;
	}

	#endregion



	#region Socket Key Data

	public enum SocketState{
		DisConnected,		// Default, off line status
		Connecting,			// try connecting
		Connected,			// connected
		ConnectedFailed,	// fail
		ConnectiontLost		// lost
	}

	public static SocketState m_state = SocketState.DisConnected;


	private Socket m_socket = null;

	private IAsyncResult m_cur_sending = null;

	private QXBuffer m_receive_buffer = null;
	
	private byte[] m_receive_bytes = new byte[ 8192 ];


	private int m_read_index = 0;

	/// receivd queue
	private static Queue<QXBuffer> m_received_messages 		= new Queue<QXBuffer>();

	/// seding queue
	private static Queue<QXBuffer> m_sending_messages 		= new Queue<QXBuffer>();

	/// Debug Use
	private static Queue<QXBuffer> m_delayed_received_message 	= new Queue<QXBuffer>();

	/// processor list
	private static List<SocketProcessor> m_socket_processors 	= new List<SocketProcessor>();

	/// listener list
	private static List<SocketListener> m_socket_listeners 				= new List<SocketListener>();

	/// receiving wait queue
	private static List<ReceivingWaitings> m_receiving_waiting_list 	= new List<ReceivingWaitings>();

	private const char RECEIVING_WAIT_SPLITTER	= '|';

	private class ReceivingWaitings{
		private List<int> m_waiting_list = new List<int>();

		private string m_waiting_str = "";

		private StringBuilder m_str_builder = new StringBuilder();

		public bool IsBingo( int p_cur_proto_id ){
			for( int i = 0; i < m_waiting_list.Count; i++ ){
				int t_waiting_id = m_waiting_list[ i ];

				if( t_waiting_id == p_cur_proto_id ){
					return true;
				}
			}

			return false;
		}

		public void AddReceivingWaiting( int p_waiting_id ){
			m_waiting_list.Add( p_waiting_id );

			UpdateWaitingString();
		}

		/// Seperated By '|':
		/// 
		/// Example:
		/// 21001|21005|50005
		public void SetReceivingWaiting( string p_waiting_string ){
			string[] t_items = p_waiting_string.Split( RECEIVING_WAIT_SPLITTER );

			for( int i = 0; i < t_items.Length; i++ ){
				AddReceivingWaiting( int.Parse( t_items[ i ] ) );
			}

			UpdateWaitingString();
		}

		private void UpdateWaitingString(){
			m_str_builder.Remove( 0, m_str_builder.Length );

			for( int i = 0; i < m_waiting_list.Count; i++ ){
				m_str_builder.Append( m_waiting_list[ i ] );

				if( i != m_waiting_list.Count - 1 ){
					m_str_builder.Append( " | " );
				}
			}

			m_waiting_str = m_str_builder.ToString();
		}

		public string GetReceivingString(){
			return m_waiting_str;
		}
	}

	private static byte[] m_temp_2_bytes_array = new byte[ 4 ];


	private static SocketTool m_instance;

	public static SocketTool Instance(){
		if( m_instance == null )
		{
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();
			
			m_instance = t_gameObject.AddComponent( typeof( SocketTool ) ) as SocketTool;
		}
		
		return m_instance;
	}

	#endregion



	#region Mono

	void Awake(){
		{
			SocketTool.RegisterMessageProcessor( this );

			SocketTool.RegisterSocketListener( this );
		}

		{
			SocketHelper.RegisterGlobalProcessorAndListeners();
		}
	}

	void Start(){

	}

	private long m_ms_before_processor	= 0;

	private float m_last_update_time = 0.0f;

	void Update(){
		if( m_log_socket_info ){
			m_log_socket_info = false;

			LogSocketToolInfo();
		}

		if( m_log_socket_processor ){
			m_log_socket_processor = false;

			LogSocketProcessor();
		}

		if ( m_socket == null ) {
			return;
		}

		if( Time.realtimeSinceStartup - m_last_update_time < ConfigTool.GetFloat( ConfigTool.CONST_NETWORK_SOCKET_UPDATE_INTERVAL ) ){
			return;
		}
		else{
			m_last_update_time = Time.realtimeSinceStartup;
		}

		if( IsConnected() ){
			{
				Process_Network_Waiting();
			}

			{
				UpdateProcessorsAndListeners();
			}

			#if DEBUG_PROCESSOR_COST
			{
				m_ms_before_processor = TimeHelper.GetCurrentTimeMillis();
				
				Process_Received_Messages();
				
				long t_cost_ms = ( TimeHelper.GetCurrentTimeMillis() - m_ms_before_processor );
				
				if( t_cost_ms > 0 ){
					Debug.Log( "Processor Cost in ms: " + t_cost_ms );
				}
			}

			#else
			{
				Process_Received_Messages();
			}
			#endif
		}

		if( m_state == SocketState.ConnectiontLost || m_state == SocketState.ConnectedFailed ){
			Debug.Log( "SocketState.Lost || SocketState.Fail." );

			ConnectionLostOrFail();
		}

		{
			SocketHelper.UpdateNetworkStatusCheck();
		}
	}

	void OnDestroy(){
//		Debug.Log( "SocketTool.OnDestroy()" );

		{
			ShutDown();
			
			m_instance = null;
		}

		{
			SocketTool.UnRegisterMessageProcessor( this );

			SocketTool.UnRegisterSocketListener( this );
		}
	}

	#endregion



	#region Connect & DisConnect
	
	public void Connect(){
		if( m_socket != null ){
			Debug.Log( "Should not connect twice." );

			return;
		}

		// show connectting
		{
			NetworkWaiting.Instance().ShowNetworkSending( "Connecting to server" );
		}

		SetSocketState( SocketState.Connecting );

		m_socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
		
		IPAddress t_address = IPAddress.Parse( m_server_prefix );
		
		IPEndPoint t_port = new IPEndPoint( t_address, m_server_port );
		
		IAsyncResult t_result = m_socket.BeginConnect( t_port, new AsyncCallback( SocketConnectCallback ), m_socket );

		bool t_success = t_result.AsyncWaitHandle.WaitOne( SOCKET_TIME_OUT, false );
		
		if( !t_success ){
			Debug.LogError( "Socket Connect Fail." );

			//Close();

			ConnectFailed();
		}
		else{
			StartReceive();
		}

		// show connectting
		{
			NetworkWaiting.Instance().HideNeworkWaiting();
		}
	}

	void SocketConnectCallback( IAsyncResult p_result ){
		#if DEBUG_SOCKET_CONNECT
		Debug.Log( "SocketConnectCallback()" );
		#endif

		Socket t_socket = ( Socket )p_result.AsyncState;
		
		if (t_socket == null){
			Debug.LogError( "SocketConnectCallback( t_socket == null )" );

			ConnectFailed();

			return;
		}
		
//		bool t_success = true;
		
		try{
			if( m_socket != null ){
				m_socket.EndConnect( p_result );
			}
		}
		catch( System.Exception e ){
			//Close();
			Debug.LogError( "SocketConnectCallback.Exception: " + e );

			ConnectFailed();
			
//			t_success = false;
			
			Debug.Log( "SocketConnectCallback.Exception: " + e );
			
			return;
		}

		SetSocketState( SocketState.Connected );

		{
			lock( m_sending_messages ){
				if( m_sending_messages.Count == 0 ){
					return;
				}

				if( m_socket != null && m_socket.Connected ){
					Debug.Log( "Send Delayed: " + m_sending_messages.Count );

					QXBuffer t_next_buffer = m_sending_messages.Peek();
					
					ExecSend( t_next_buffer );
				}
				else{
					Debug.LogError( "Error: " + m_socket );

					if( m_socket != null ){
						Debug.LogError( "Connected: " + m_socket.Connected );
					}
				}
			}
		}
	}

	// check if still connect
	private bool IsCSSocketConnected(){
		if ( m_socket == null ) {
			return false;
		}

		if( true ){
			return m_socket.Connected;
		}

		bool connectState = true;
		bool blockingState = m_socket.Blocking;
		try{
			byte[] tmp = new byte[1];
			
			m_socket.Blocking = false;

			m_socket.Send(tmp, 0, 0);

			//Debug.Log("Connected!");

			connectState = true; 
		}
		catch (SocketException e){
			// 10035 == WSAEWOULDBLOCK
			if (e.NativeErrorCode.Equals(10035)){
				#if DEBUG_SOCKET_CONNECT
				Debug.Log("Still Connected, but the Send would block");
				#endif

				connectState = true;
			}
			else{
				#if DEBUG_SOCKET_CONNECT
				Debug.Log("Disconnected: error code {0}! " + e.NativeErrorCode);
				#endif

				connectState = false;
			}
		}
		finally
		{
			m_socket.Blocking = blockingState;
		}
		
		//Console.WriteLine("Connected: {0}", client.Connected);

		return connectState;
	}
	
	public static void CloseSocket(){
		#if DEBUG_SOCKET_CONNECT
		Debug.Log( "SocketTool.CloseSocket()" );
		#endif

		{
			SetStateDisConnect();
		}

		try{
			if( m_instance != null ){
				m_instance.ShutDown();
			}
		}
		catch( Exception e ){
			/* possibly throw an exception here, but never mind.
			 * 
			 * in secondary thread, it will throw an exception when coparing monoobjects with null.
			 */
			
			Debug.Log( "SocketTool.Static.Close.Exception: " + e );
		}
	}
	
	private void ShutDown(){
		#if DEBUG_SOCKET_CONNECT
		Debug.Log( "SocketTool.ShutDown()" );
		#endif

		try{
	//		if( m_socket != null && m_socket.Connected ){
			if( m_socket != null && IsCSSocketConnected() ){
				m_socket.Shutdown( SocketShutdown.Both );
				
				m_socket.Close();
			}	
		}
		catch( Exception e ){
			Debug.Log( "Socket.ShutDown.Exception: " + e );
		}

		{
			m_socket = null;
		}
		
		{
			Clear();
		}
	}

	public void SetSocketLost(){
		#if DEBUG_SOCKET_CONNECT
		Debug.Log( "SetSocketLost()" );
		#endif

		ManualLostConnection();
	}

	/// <summary>
	/// No response from server.
	/// </summary>
	public static void ConnectionTimeOut(){
		SocketHelper.CreateTimeOutReConnectWindow( SocketHelper.ReLoginClickCallback );
		
		SetStateDisConnect();
	}

	/// <summary>
	/// Connection exception.
	/// </summary>
	private void ConnectionLostOrFail(){
		SocketHelper.CreateConnectionLostOrFailWindow();
		
		SetStateDisConnect();
	}

	private static void SetSocketState( SocketState p_state ){
		#if DEBUG_SOCKET_STATE
		Debug.Log( "SetSocketState( " + p_state + " )" );
		#endif

		m_state = p_state;
	}

	private static void SetStateDisConnect(){
		#if DEBUG_SOCKET_CONNECT
		Debug.Log( "SetStateDisConnect()" );
		#endif

		SetSocketState( SocketState.DisConnected );
	}

	public static void LogNetworkWaiting(){
		Debug.Log( "SocketTool.LogNetworkWaiting()" );

		for( int i = 0; i < m_receiving_waiting_list.Count; i++ ){
			Debug.Log( i + " Receiving Waiting " + m_receiving_waiting_list[ i ].GetReceivingString() );
		}

		Debug.Log( "----------------- Receiving Wait Count: " + m_receiving_waiting_list.Count + " ---------------" );

		int t_sending_wait_index = 0;

		lock( m_sending_messages ){
			foreach( QXBuffer t_buffer in m_sending_messages ){
				if( t_buffer.IsSendingWait() ){
					Debug.Log( t_sending_wait_index + " Sending Wait " + t_buffer.m_protocol_index );

					t_sending_wait_index++;
				}
			}

			Debug.Log( "----------------- Sending Wait Count: " + t_sending_wait_index + " ---------------" );
		}
	}

	public static void LogSocketToolInfo(){
		Debug.Log ( "--- LogSocketToolInfo() ---" );

		Debug.Log( "State: " + GetSocketState() );

		Debug.Log ( "IsConnected(): " + IsConnected() );

		{
			LogNetworkWaiting();
		}

		if ( m_instance == null ) {
			Debug.LogError( "Socket.m_instance = null." );

			return;
		}

		if ( Instance().m_socket == null ) {
			Debug.LogError( "Socket.m_instance.m_socket = null." );
			
			return;
		}
	}

	#endregion



	#region Network Process

	private GameObject m_multi_login_gb = null;

	public bool OnProcessSocketMessage( QXBuffer p_message ){
		if( p_message.m_protocol_index == ProtoIndexes.GENERAL_ERROR ){
			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			ErrorMessage t_error_message = new ErrorMessage();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Deserialize( t_stream, t_error_message, t_error_message.GetType() );
			
			Debug.LogError( "General Error Code: " + t_error_message.errorCode + 
			               "   Desc: " + t_error_message.errorDesc +
			               "   Client: " + t_error_message.cmd );
			
			SocketHelper.CreateGeneralErrorWindow( t_error_message.errorCode,
			                         t_error_message.errorDesc,
			                         t_error_message.cmd );
			
			return true;
		}
		else if ( p_message.m_protocol_index == ProtoIndexes.MULTI_LOGIN_ERROR ){
			//			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			//			
			//			ErrorMessage t_error_message = new ErrorMessage();
			//			
			//			QiXiongSerializer t_qx = new QiXiongSerializer();
			//			
			//			t_qx.Deserialize( t_stream, t_error_message, t_error_message.GetType() );
			//			
			//			Debug.LogError( "General Error Code: " + t_error_message.errorCode + 
			//			               "   Desc: " + t_error_message.errorDesc +
			//			               "   Client: " + t_error_message.cmd );
			
			Debug.Log("Multi Login Error.");


			SocketTool.CloseSocket();

			if( m_multi_login_gb == null ){
				if( UIBox.BoxExistWithTime( LanguageTemplate.GetText(LanguageTemplate.Text.DISTANCE_LOGIN_1) ) ){
					Debug.Log( "Box already exist." );
				}
				else{
					Debug.Log( "Create new multi login box." );

					m_multi_login_gb = Global.CreateBox( LanguageTemplate.GetText(LanguageTemplate.Text.DISTANCE_LOGIN_1),
						LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TRANS_97),
						"",
						null,
						LanguageTemplate.GetText(LanguageTemplate.Text.DISTANCE_LOGIN_2),

						null,
						MultiLoginCallback,
						null,
						null,
						null,

						false,
						false,
						true, 
						true );

					DontDestroyOnLoad( m_multi_login_gb );
				}
			}
			
			return true;
		}
		
		if( p_message.m_protocol_index == ProtoIndexes.GENERAL_MESSAGE ){
			Debug.Log( "Maybe Old Error Process, not used this version." );
		}
			
		return false;
	}

	public void MultiLoginCallback(int p_i ){
		Debug.Log("MultiLoginCallback( " + p_i + " )");
		
		if( m_multi_login_gb != null ){
			m_multi_login_gb.SetActive( false );
			
			Destroy( m_multi_login_gb );

			m_multi_login_gb = null;
		}

		{
			SceneManager.RequestEnterLogin();
		}
		
	}
	
	#endregion



	#region Network Listener

	public bool OnSocketEvent( QXBuffer p_message ){
		if( p_message == null ){
			return false;
		}

		switch( p_message.m_protocol_index ){
			
		case ProtoIndexes.NETWORK_CHECK_RET:
		{
//			Debug.Log( "Network Status OK." );

			if( SocketHelper.m_socket_check_send_queue.Count > 0 ){
				SocketHelper.ClearNetWorkCheckQueue();
			}

			return true;
		}

		default:
			return false;
		}
	}

	#endregion



	#region Network Delay & Clear

	public void ClearSendAdnReceiveMessages(){
		m_sending_messages.Clear();

		m_received_messages.Clear();

		m_delayed_received_message.Clear();

		if( m_receiving_waiting_list.Count > 0 ){
			LogNetworkWaiting();
		}

		m_receiving_waiting_list.Clear();

		SocketHelper.ClearNetWorkCheckQueue();
	}

	private void Clear(){
		#if DEBUG_CLEAR
//		Debug.Log( "SocketTool.Clear()" );
		#endif

		{
			ClearSendAdnReceiveMessages();
		}

		{
//			for( int i = m_socket_processors.Count - 1; i >= 0; i-- ){
//				SocketProcessor t_processor = m_socket_processors[ i ];
//
//				if( t_processor == null ){
//					m_socket_processors.RemoveAt( i );
//				}
//			}

			m_socket_processors.Clear();

		}

		{
//			for( int i = m_socket_listeners.Count - 1; i >= 0; i-- ){
//				SocketListener t_listener = m_socket_listeners[ i ];
//
//				if( t_listener == null ){
//					m_socket_listeners.RemoveAt( i );
//				}
//			}

			m_socket_listeners.Clear();
		}

		m_socket = null;
		
		{
			m_try_send_head_message = true;

			SetSocketState( SocketState.DisConnected );
		}
		
		if( NetworkWaiting.m_instance_exist ){
			NetworkWaiting.Instance().HideNeworkWaiting();
		}
	}

	#endregion



	#region Receiving Data

	/// asynchronous
	void StartReceive(){
		if( m_socket == null ){
			Debug.LogError( "StartReceive.Socket == null." );

			CloseSocket();

			ConnectFailed();

			return;
		}

		if ( m_socket.Connected ){
			try{
				m_socket.BeginReceive( m_receive_bytes, 0, m_receive_bytes.Length, 
				                      SocketFlags.None, ReceiveCallback, null );
			}
			catch( System.Exception e ){
				Debug.LogError( "StartReceive.Exception: " + e );

				//Close();

				ConnectFailed();
			}
		}
	}

	/// asynchronous
	void ReceiveCallback ( IAsyncResult p_result ){
		if( m_socket == null || !m_socket.Connected ){
			if( m_socket != null ){
				#if DEBUG_SOCKET_CONNECT
				Debug.Log( "ReceiveCallback.return null Connected: " + m_socket.Connected + " -> reconnect." );
				#endif
			}
			else{
				#if DEBUG_SOCKET_CONNECT
				Debug.Log( "ReceieCallback.return null socket: " + m_socket );
				#endif
			}

			return;
		}

		int t_recieved_bytes = -1;
		
		try{
			t_recieved_bytes = m_socket.EndReceive( p_result );

			if( m_receive_buffer == null ){
				m_receive_buffer = QXBuffer.Create();

				m_receive_buffer.BeginWriting( false ).Write( m_receive_bytes, 0, t_recieved_bytes );
			}
			else{
				m_receive_buffer.BeginWriting( true ).Write( m_receive_bytes, 0, t_recieved_bytes );
			}
		}
		catch( System.Exception e ){
			Debug.Log( "ReceiveCallback.Exception 1: " + e );

			//Close();

//			ManualLostConnection();

//			return;
		}

		if( IsDisConnected() ){
//			#if DEBUG_SOCKET_CONNECT
			Debug.Log( "DisConnected, return." );
//			#endif

			return;
		}

		// close if receive 0 length data.
		if ( t_recieved_bytes == 0 ){
//			#if DEBUG_SOCKET_CONNECT
            Debug.Log( "t_receive_bytes == 0, close Socket." );
//			#endif

			CloseSocket();

			return;
		}

//		if ( m_socket.Available == 0 )
		{
			// multi message received
			ProcessMultiMessage();
		}

		try{
//			if( !m_socket.Connected ){
			if( !IsCSSocketConnected() ){
				Debug.LogError( "SocketTool : Faied to connect to server." );
				
				LogServerSet();
				
				//Close();

				ManualLostConnection();

				return;
			}

			m_socket.BeginReceive( m_receive_bytes, 0, m_receive_bytes.Length, 
			                      SocketFlags.None, ReceiveCallback, null );
		}
		catch( System.Exception e ){
			//Close();

			Debug.Log( "ReceiveCallback.Exception 2: " + e );

			ManualLostConnection();
		}
	}

	void ProcessMultiMessage(){
		if( m_receive_buffer == null ){
			Debug.LogError( "Error, Buffer Is Null." );

			ResetRead();

			return;
		}

		byte[] t_bytes = m_receive_buffer.buffer;
		
		while( m_read_index + 4 < m_receive_buffer.position ){
			int t_next_len = 0;
			
			// buff len
			{
				m_temp_2_bytes_array[ 0 ] = t_bytes[ m_read_index + 3 ];
				m_temp_2_bytes_array[ 1 ] = t_bytes[ m_read_index + 2 ];
				m_temp_2_bytes_array[ 2 ] = t_bytes[ m_read_index + 1 ];
				m_temp_2_bytes_array[ 3 ] = t_bytes[ m_read_index + 0 ];

				t_next_len = System.BitConverter.ToInt32( m_temp_2_bytes_array, 0 );

				#if DEBUG_RECEIVE
				Debug.Log( "next len: " + t_next_len );
				#endif
			}

			// data not enough
			if( m_read_index + 4 + t_next_len > m_receive_buffer.position ){
				#if DEBUG_RECEIVE
                Debug.LogWarning( "Data not enough." );
				#endif

				return;
			}
			
			short t_proto_index = 0;
			
			// protocol index
			{
				m_temp_2_bytes_array[ 0 ] = t_bytes[ m_read_index + 5 ];
				m_temp_2_bytes_array[ 1 ] = t_bytes[ m_read_index + 4 ];
				
				t_proto_index = System.BitConverter.ToInt16( m_temp_2_bytes_array, 0 );

				#if DEBUG_RECEIVE
				Debug.Log( "t_proto_index: " + t_proto_index );
				#endif
			}
			
			// buff content
			{
				QXBuffer t_buffer = QXBuffer.Create();

				t_buffer.BeginWriting( false ).Write( t_bytes, 4 + 2 + m_read_index, t_next_len - 2 );

				t_buffer.m_protocol_index = t_proto_index;

				{
					bool t_log_receive = false;

					if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_RECEIVE ) ){
						t_log_receive = true;
					}

					#if DEBUG_RECEIVE
					t_log_receive = true;
					#endif

					if( t_proto_index == ProtoIndexes.Sprite_Move ){
						t_log_receive = false;
					}

//					if( t_proto_index == ProtoIndexes.Sprite_Move && ConfigTool.GetBool( ConfigTool.CONST_LOG_MAINCITY_SPRITE_MOVE ) ){
//						t_log_receive = true;
//					}

					if( t_log_receive ){
						Debug.Log("Receive Socket Message: " + t_proto_index + ", data len: " + t_buffer.size );
					}
				}

				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_RECEIVE_DETAIL ) ){
					LogReceiveMessageDetail( m_receive_buffer );
				}

				{
					SocketHelper.SocketDataReceived();
				}

				if( !ConfigTool.IsEmulatingNetworkLatency() ){
					lock( m_received_messages ){
						m_received_messages.Enqueue( t_buffer );
					}
				}
				else{
					lock( m_delayed_received_message ){
						m_delayed_received_message.Enqueue( t_buffer );
					}
				}
			}
			
			m_read_index += ( 4 + t_next_len );
		}

		if ( m_read_index == m_receive_buffer.position ) {
//			Debug.Log( "Recycle." );

			ResetRead();
		}
		else {
//			Debug.LogError( "Error, Data Split." );
		}
	}

	private void ResetRead(){
		if( m_receive_buffer != null ){
			m_receive_buffer.Recycle ();

			m_receive_buffer = null;
		}

		m_read_index = 0;
	}

	#endregion


	#region Sending Data

	private void SendGBK(){
//		if( !CanSendMessage( -1 ) ){
//			return;
//		}

		SendSocketMessage( SocketTool.SOCKET_HEAD_MESSAGE );
	}

	/// Params:
	/// 
	/// 1.p_receiving_wait_proto_index:	
	/// 	the proto message to wait, -1 means needn't wait.
	public void SendSocketMessage( string p_message, bool p_sending_wait = false, int p_receiving_wait_proto_index = -1 ){
//		if( !CanSendMessage( -2 ) ){
//			return;
//		}

		byte[] t_bytes = Encoding.UTF8.GetBytes( p_message );

		QXBuffer t_buffer = QXBuffer.Create();

		t_buffer.BeginWriting( false ).Write( t_bytes, 0, t_bytes.Length );

		// wait flag
		{
			UpdateSendMessageWaitingFlag( t_buffer,
			                             p_sending_wait,
			                             p_receiving_wait_proto_index );
		}

		SendSocketWith_EmulatingNetworkLatency( t_buffer, 0 );
	}

	/** 
	 * Params:
	 * 
	 * 1.p_receiving_wait_protos( Seperated By '|' ): 
	 *     e.g. 21001|21005|50005 means waiting for 21001 or 21005 or 50005;
	 *     e.g. "": means never wait;
	 **/ 
	public void SendSocketMessage( string p_message, string p_receiving_wait_protos ){
//		if( !CanSendMessage( -3 ) ){
//			return;
//		}

		byte[] t_bytes = Encoding.UTF8.GetBytes( p_message );
		
		QXBuffer t_buffer = QXBuffer.Create();
		
		t_buffer.BeginWriting( false ).Write( t_bytes, 0, t_bytes.Length );
		
		// wait flag
		{
			UpdateSendMessageWaitingFlag( t_buffer,
			                             p_receiving_wait_protos );
		}
		
		SendSocketWith_EmulatingNetworkLatency( t_buffer, 0 );
	}

	/// Params:
	/// 
	/// 1.p_receiving_wait_proto_index:	
	/// 	the proto message to wait, -1 means needn't wait.
	public void SendSocketMessage( short p_protocol_index, bool p_sending_wait = false, int p_receiving_wait_proto_index = -1 ){
//		if( !CanSendMessage( p_protocol_index ) ){
//			return;
//		}

		byte[] t_null_array = null;

		SendSocketMessage( p_protocol_index, ref t_null_array, p_sending_wait, p_receiving_wait_proto_index );
	}

	/** 
	 * Params:
	 * 
	 * 1.p_receiving_wait_protos( Seperated By '|' ): 
	 *     e.g. 21001|21005|50005 means waiting for 21001 or 21005 or 50005;
	 *     e.g. "": means never wait;
	 **/ 
	public void SendSocketMessage( short p_protocol_index, string p_receiving_wait_protos ){
//		if( !CanSendMessage( p_protocol_index ) ){
//			return;
//		}

		byte[] t_null_array = null;
		
		SendSocketMessage( p_protocol_index, ref t_null_array, p_receiving_wait_protos );
	}

	/// Params:
	/// 
	/// 1.p_receiving_wait_proto_index:	the proto message to wait, -1 means needn't wait.
	public void SendSocketMessage( short p_protocol_index, ref byte[] p_protobuf, bool p_sending_wait = false, int p_receiving_wait_proto_index = -1 ){
//		if( !CanSendMessage( p_protocol_index ) ){
//			return;
//		}

		QXBuffer t_buffer = QXBuffer.Create();

		// wait flag
		{
			UpdateSendMessageWaitingFlag( t_buffer,
			                             p_sending_wait,
			                             p_receiving_wait_proto_index );
		}

		SendMessageBody( t_buffer, p_protocol_index, ref p_protobuf );
	}

	// send when connect is false
	private QXBuffer CreateNetworkCheckBuffer(){
		byte[] t_null_array = null;

		QXBuffer t_buffer = QXBuffer.Create();

		FillQXBuffer( t_buffer, ProtoIndexes.NETWORK_CHECK, ref t_null_array );

//		m_socket.BeginSend ( p_buffer.buffer, 0, p_buffer.position );

		return t_buffer;
	}

	/** 
	 * Params:
	 * 
	 * 1.p_receiving_wait_protos( Seperated By '|' ): 
	 *     e.g. 21001|21005|50005 means waiting for 21001 or 21005 or 50005;
	 *     e.g. "": means never wait;
	 **/ 
	public void SendSocketMessage( short p_protocol_index, ref byte[] p_protobuf, string p_receiving_wait_protos ){
//		if( !CanSendMessage( p_protocol_index ) ){
//			return;
//		}

		QXBuffer t_buffer = QXBuffer.Create();
		
		// wait flag
		{
			UpdateSendMessageWaitingFlag( t_buffer,
			                             p_receiving_wait_protos );
		}

		SendMessageBody( t_buffer, p_protocol_index, ref p_protobuf );
	}

	private void SendMessageBody( QXBuffer p_buffer, short p_protocol_index, ref byte[] p_protobuf ){
		FillQXBuffer( p_buffer, p_protocol_index, ref p_protobuf );
		
		// send
		{
			SendSocketWith_EmulatingNetworkLatency( p_buffer, 6 );

		}
	}

	// fill buffer with index, content
	// offset 6
	private void FillQXBuffer( QXBuffer p_buffer, short p_protocol_index, ref byte[] p_protobuf ){
		int t_proto_buf_len = p_protobuf == null ? 0 : p_protobuf.Length;
		
		// buff len
		{
			// 2 bytes about index + buff.length
			int t_len = (int)( t_proto_buf_len + 2 );
			
			byte[] t_bytes = System.BitConverter.GetBytes( t_len );
			
			System.Array.Reverse( t_bytes );
			
			p_buffer.BeginWriting( false ).Write( t_bytes, 0, 4 );
		}
		
		// protocol index
		{
			short t_index = p_protocol_index;
			
			p_buffer.m_protocol_index = p_protocol_index;
			
			byte[] t_bytes = System.BitConverter.GetBytes( t_index );
			
			System.Array.Reverse( t_bytes );
			
			p_buffer.BeginWriting( true ).Write( t_bytes, 0, 2 );
		}
		
		// buff content
		if( p_protobuf != null )
		{
			p_buffer.BeginWriting( true ).Write( p_protobuf, 0, t_proto_buf_len );
		}
	}

	private void SendSocketMessage( QXBuffer p_buffer ){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_SEND ) ){
			Debug.Log( "SendSocketMessage bytes: " + p_buffer.m_protocol_index + ": " + p_buffer.size );
		}

		lock( m_sending_messages ){
			m_sending_messages.Enqueue( p_buffer );

			if( m_sending_messages.Count == 1 ){
				ExecSend( p_buffer );
			}
			else if( m_sending_messages.Count == 0 ){
				m_cur_sending = null;
			}
		}
	}

	private void UpdateSendMessageWaitingFlag( QXBuffer p_buffer, bool p_sending_wait = false, int p_receiving_wait_proto_index = -1 ){
		p_buffer.SetSendingWait( p_sending_wait );
		
		p_buffer.SetReceivingWait( p_receiving_wait_proto_index == -1 ? false : true );
		
		if( p_buffer.IsReceivingWait() ){
			p_buffer.SetSendingWait( true );

			ReceivingWaitings t_waiting = new ReceivingWaitings();

			t_waiting.AddReceivingWaiting( p_receiving_wait_proto_index );

			m_receiving_waiting_list.Add( t_waiting );
		}
	}

	private void UpdateSendMessageWaitingFlag( QXBuffer p_buffer, string p_receiving_wait_protos ){
		if( p_buffer == null ){
			Debug.LogError( "Error, buffer is null." );

			return;
		}

		p_buffer.SetAllWaiting( string.IsNullOrEmpty( p_receiving_wait_protos ) ? false : true );

		if( !string.IsNullOrEmpty( p_receiving_wait_protos ) ){
			p_buffer.SetAllWaiting( ( string.Compare( p_receiving_wait_protos.Trim(), "-1" ) == 0 ) ? false : true );
		}

		if( p_buffer.IsReceivingWait() ){
			p_buffer.SetSendingWait( true );

			ReceivingWaitings t_waiting = new ReceivingWaitings();
			
			t_waiting.SetReceivingWaiting( p_receiving_wait_protos );

			m_receiving_waiting_list.Add( t_waiting );
		}
	}
	
	private void SendCallback( IAsyncResult p_send ){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_SEND ) ){
			Debug.Log( "Socket Send message callback." );
		}

		int t_bytes_sent = 0;

		try{
			if( m_socket != null ){
				t_bytes_sent = m_socket.EndSend( p_send );
			}
			else{
				CloseSocket();

				ManualLostConnection();
			}

			m_cur_sending = null;
		}
		catch ( System.Exception e ){
			t_bytes_sent = 0;

			CloseSocket();

			Debug.LogError( "SendCallback.Exception: " + e );

			ManualLostConnection();

			return;
		}

		#if DEBUG_SEND
		Debug.Log( "Send Done." );
		#endif

		lock( m_sending_messages ){
			QXBuffer t_buffer = m_sending_messages.Dequeue();

			t_buffer.Recycle();

			t_buffer = null;

			if( m_sending_messages.Count == 0 ){
				return;
			}

//			if( t_bytes_sent > 0 && m_socket != null && m_socket.Connected ){
			if( t_bytes_sent > 0 && m_socket != null && IsCSSocketConnected() ){
				QXBuffer t_next_buffer = m_sending_messages.Peek();

				ExecSend( t_next_buffer );

            }
			else{
				m_cur_sending = null;
			}
		}
	}

	private void ExecSend( QXBuffer p_buffer ){
		if( !IsConnected() ){
			Debug.Log( "Delay Send: " + GetSocketState() );

			return;
		}

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_SEND_DETIAL ) ){
			LogSendMessageDetail( p_buffer );
		}

		try{
			#if DEBUG_SEND
			Debug.Log( "ExecSend: " + p_buffer.m_protocol_index + ", " + p_buffer.position );
			#endif

			m_cur_sending = m_socket.BeginSend( p_buffer.buffer, 
			                                   0, p_buffer.position, 
			                                   SocketFlags.None, 
											   SendCallback,
			                                   m_socket );
			
//					bool t_success = m_cur_sending.AsyncWaitHandle.WaitOne( 5000, true );
//					
//					if( !t_success ){
//						Debug.LogError( "Send Message fail, can't connect the server." );
//
//						ManualLostConnection();
//					}
		}
		catch( Exception e ){
			Debug.LogError( "Sending message error: " + e );
		}


	}

	#endregion



	#region Network Waiting
	
	public static bool m_is_waiting_for_latency_over = false;
	
	private void Process_Network_Waiting(){
		// sending tips
		if( NetworkWaiting.Instance().IsShowingSending() ){
			if( m_is_waiting_for_latency_over ){
				return;
			}

			int t_sending_wait_count = GetSendingWaitCount();

			if( t_sending_wait_count <= 0 ){
				NetworkWaiting.Instance().HideNeworkWaiting();
			}
		}

		// receiving tips
		if( NetworkWaiting.Instance().IsHiding() ){
			if( m_receiving_waiting_list.Count > 0 ){
				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_WAITING ) ){
					Debug.Log( "m_receiving_waiting_list: " + m_receiving_waiting_list.Count + " - " + m_receiving_waiting_list[ 0 ].GetReceivingString() );
				}

				#if DEBUG_WAITING
				Debug.Log( "m_receiving_waiting_list: " + m_receiving_waiting_list.Count + " - " + m_receiving_waiting_list[ 0 ].GetReceivingString() );
				#endif

				NetworkWaiting.Instance().ShowNetworkReceiving( m_receiving_waiting_list[ 0 ].GetReceivingString() );
			}
		}
	}
	
	private int GetSendingWaitCount(){
		int t_count = 0;

		lock( m_sending_messages ){
			foreach( QXBuffer t_buffer in m_sending_messages ){
				if( t_buffer.IsSendingWait() ){
					t_count++;
				}
			}
		}

		return t_count;
	}

	#endregion



	#region Socket Listener

	/// Desc:
	/// Register proto listener.
	/// 
	/// Notes:
	/// 1.Listener never response for proto message, just watching for some special proto index.
	public static void RegisterSocketListener( SocketListener p_listener ){
		if( m_socket_listeners.Contains( p_listener ) ){
			return;
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER ) ){
			Debug.Log( "--- RegisterSocketListener: " + p_listener );
		}
		
		m_socket_listeners.Add( p_listener );
	}

	/// Unregister a socket listener.
	public static void UnRegisterSocketListener( SocketListener p_listener ){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER ) ){
			Debug.Log( "--- UnRegisterSocketListener: " + p_listener );
		}

	    if (!m_socket_listeners.Contains(p_listener)) return;
		m_socket_listeners.Remove( p_listener );
	}

	/// return if Buffer is Listened
	private static bool ProcessSocketListeners( QXBuffer p_buffer ){
		bool t_listened = false;

		for( int i = 0; i < m_socket_listeners.Count; i++ ){
			SocketListener t_listener = m_socket_listeners[ i ];

			if( t_listened == null ){
				continue;
			}

			if( t_listener.OnSocketEvent( p_buffer ) ){
				t_listened = true;
			}
		}

		return t_listened;
	}

	#endregion



	#region Sending Data Tips & Wait

	/// <param name="p_buffer">Send Body.</param>
	/// <param name="p_proto_body_offset">Proto Body Offset.</param>
	private void SendSocketWith_EmulatingNetworkLatency( QXBuffer p_buffer, int p_proto_body_offset ){

#if PROTO_TOOL
        ProtoToolManager.Instance.AddToSendProtoIndexWithRefresh(p_buffer, p_proto_body_offset);
#endif
		// gbk
		{
			if( m_try_send_head_message ){
				m_try_send_head_message = false;
				
				SendGBK();
			}
		}
		
		// checker
		{
			if( m_socket == null ){
				Debug.Log( "m_socket == null, skip sending." );

				SocketHelper.CreateConnectionLostOrFailWindow();
				
				return;	
			}

			if( IsDisConnected() ){
				Debug.Log( "DisConnected, skip sending." );

				return;
			}

//			if( !m_socket.Connected ){
			if( !IsCSSocketConnected() ){
				Debug.Log( "Not Connected." );

				LogServerSet();
				
				//m_socket.Close();
				
				ManualLostConnection();
				
				return;
			}
		}
		
		// Show Sending Waiting
		if( p_buffer.IsSendingWait() ){
			NetworkWaiting.Instance().ShowNetworkSending( p_buffer.m_protocol_index );
		}
		
		// exec
		{
			if( ConfigTool.m_is_emulating_latency ){
				m_is_waiting_for_latency_over = true;
				
				StartCoroutine( SendSocket_AfterLatency( p_buffer ) );
			}
			else{
				SendSocketMessage( p_buffer );
			}
		}
	}
	
	private IEnumerator SendSocket_AfterLatency( QXBuffer p_buffer ){
		yield return new WaitForSeconds( ConfigTool.m_emulate_network_latency );
		
		m_is_waiting_for_latency_over = false;
		
		SendSocketMessage( p_buffer );
	}

	#endregion



	#region Processing Received Message, should change to callbacks

	/// Desc:
	/// Register a proto message processor.
	/// 
	/// Notes:
	/// 1.a processor is responsible for target proto message, return true, means target proto is consumed.
	/// 2.if a proto message is never processed, and it also not being listened, will raise a warning log.
	public static void RegisterMessageProcessor( SocketProcessor p_processor ){
		if( m_socket_processors.Contains( p_processor ) ){
			#if UNITY_EDITOR
			Debug.LogWarning( "Already Contained: " + p_processor );
			#endif

			return;
		}

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER ) ){
			Debug.Log( "--- RegisterMessageProcessor: " + p_processor );
		}

		m_socket_processors.Add( p_processor );
	}

	/// Desc:
	/// Unregister a proto processor.
	public static void UnRegisterMessageProcessor( SocketProcessor p_processor ){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER ) ){
			Debug.Log( "--- UnRegisterMessageProcessor: " + p_processor );
		}

		m_socket_processors.Remove( p_processor );
	}

	// validate
	private void UpdateProcessorsAndListeners(){
		if( Time.frameCount % 30 != 0 ){
			return;
		}

//		Debug.Log( "UpdateProcessorsAndListeners()" );

		for( int i = m_socket_processors.Count - 1; i >= 0; i-- ){
			SocketProcessor t_processor = m_socket_processors[ i ];

			if( t_processor == null ){
				m_socket_processors.RemoveAt( i );
			}
		}

		for( int i = m_socket_listeners.Count - 1; i >= 0; i-- ){
			SocketListener t_listener = m_socket_listeners[ i ];
			
			if( t_listener == null ){
				m_socket_listeners.RemoveAt( i );
			}
		}
	}

	private void Process_Received_Messages(){
		int t_message_count = 0;

		QXBuffer t_buffer = null;

		lock( m_received_messages ){
			if( ConfigTool.IsEmulatingNetworkLatency() ){
				lock( m_delayed_received_message ){
					if( m_delayed_received_message.Count > 0 ){
						QXBuffer t_delayed_buffer = m_delayed_received_message.Peek();

						while( t_delayed_buffer != null && t_delayed_buffer.GetSecSinceCreate() > ConfigTool.GetEmulatingNetworkLatency() ){
							m_received_messages.Enqueue( t_delayed_buffer );

							m_delayed_received_message.Dequeue();

							if( m_delayed_received_message.Count > 0 ){
								t_delayed_buffer = m_delayed_received_message.Peek();
							}
							else{
								t_delayed_buffer = null;
							}
						}
					}
				}
			}

			t_message_count = m_received_messages.Count;

			if( t_message_count > 0 ){
				t_buffer = m_received_messages.Peek();
			}
			else{
				t_buffer = null;
			}
		}

		{
			while( t_message_count > 0 ){
//				Debug.Log( "Receive Message Count: " + m_received_messages.Count );

				#if DEBUG_RECEIVE
				Debug.Log( "Received Socket Processing: " + t_buffer.m_protocol_index + ", " + t_buffer.size );
				#endif

				// after lock updated, remove this code for safety reason
//				if( ConfigTool.m_is_emulating_latency ){
//					if( t_buffer.GetTimeAfterCreate() < ConfigTool.m_emulate_network_latency ){
//						//update emulate create time tag
//						foreach( QXBuffer t_buffer_in_queue in m_received_messages ){
//							t_buffer_in_queue.GetTimeAfterCreate();
//						}
//
//						return;
//					}
//				}

				bool t_message_processed = false;

				SocketProcessor t_pre_processor	= null;

				int t_pre_processor_index = -1;

				for( int i = 0; i < m_socket_processors.Count; i++ ){
					SocketProcessor t_processor = m_socket_processors[ i ];

					if( t_processor == null ){
						continue;
					}

					try{
						if( t_processor.OnProcessSocketMessage( t_buffer ) ){
							if( !t_message_processed ){
								t_message_processed = true;

								t_pre_processor = t_processor;

								t_pre_processor_index = i;
								
//							break;
							}
							else{
								Debug.LogError( "Proto Have Multi Processors: " + t_buffer.m_protocol_index + "   - " +
									t_processor + " - " + 
									" pre: " + t_pre_processor + " - " +
									t_pre_processor_index );
							}
						}
					}
					catch( Exception e ){
						Debug.LogError( "Socket Processor Exception: " + e );
					}
				}

				// no matter message processed or not, now process listener

				bool t_message_listened = false;

				try{
					{
						t_message_listened = ProcessSocketListeners( t_buffer );
					}
				}
				catch( Exception e ){
					Debug.LogError( "Socket Listener Exception: " + e );
				}

				#if DEBUG_RECEIVE
				Debug.Log( t_buffer.m_protocol_index + " Status, Processed - Listened: " + t_message_processed + "-" + t_message_listened  );
				#endif

				#if UNITY_EDITOR
				if( !ThirdPlatform.IsThirdPlatform() ){
					if( !t_message_processed ){
						if( !t_message_listened ){
							Debug.LogWarning( "Warning, Socket Message neither processed nor listened: " + 
							                 t_buffer.m_protocol_index + 
							                 ", byte len: " + t_buffer.position +
							                 ", message queue len:" + t_message_count );
						}
//						else{
//							Debug.LogWarning( "Warning, Message Listened, but not processed: " + 
//							                 t_buffer.m_protocol_index + 
//							                 ", byte len: " + t_buffer.position +
//							                 ", message queue len:" + t_message_count );
//						}
					}
				}
				#endif

				// update waiting view
				{
					for( int i = 0; i < m_receiving_waiting_list.Count; i++ ){
						if( m_receiving_waiting_list[ i ].IsBingo( t_buffer.m_protocol_index ) ){
							#if DEBUG_WAITING
							Debug.Log( "Remove: " + m_receiving_waiting_list[ i ].GetReceivingString() + " - " + i );
							#endif

							m_receiving_waiting_list.RemoveAt( i );
							
							break;
						}
					}

					if( NetworkWaiting.Instance().IsShowingReceiving() ){
						if( m_receiving_waiting_list.Count <= 0 ){
							#if DEBUG_WAITING
							Debug.Log( "Hide Waiting, All required message received In Wait List." );
							#endif
							
							NetworkWaiting.Instance().HideNeworkWaiting();
						}
					}
				}

				lock( m_received_messages ){
					// remove processed
					if( m_received_messages.Count > 0 ){
						m_received_messages.Dequeue();
						
						t_buffer.Recycle();
					}

					// update queue info
					{
						t_message_count = m_received_messages.Count;
						
						if( t_message_count > 0 ){
							t_buffer = m_received_messages.Peek();
						}
						else{
							t_buffer = null;
						}
					}
				}
			}
		}
	}

	#endregion



	#region Utilities

	public static Queue<QXBuffer> GetReceivedMessages(){
		return m_received_messages;
	}

	public static Queue<QXBuffer> GetSendingMessages(){
		return m_sending_messages;	
	}

	public static void LogSocketProcessor(){
		Debug.Log( "Socket.Processor.Count: " + m_socket_processors.Count );

		for( int i = 0; i < m_socket_processors.Count; i++ ){
			SocketProcessor t_processor = m_socket_processors[ i ];

			Debug.Log( i + ": " + t_processor );
		}
	}

	public static void LogSocketListener(){
		Debug.Log( "Socket.Listener.Count: " + m_socket_listeners.Count );
		
		for( int i = 0; i < m_socket_listeners.Count; i++ ){
			SocketListener t_processor = m_socket_listeners[ i ];
			
			Debug.Log( i + ": " + t_processor );
		}
	}
		
	private void LogServerSet(){
		Debug.Log( "Cur Server: " + m_server_prefix + " : " + m_server_port );
	}

	private void LogReceiveMessageDetail( QXBuffer p_buffer ){
		Debug.Log( "Receive Message Index: " + p_buffer.m_protocol_index + " " +
		          "   Header & Tailer: " + 
		          p_buffer.buffer[ 0 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 1 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 2 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 3 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 4 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 5 ].ToString( "X2" ) + " " +
		          "   &   " + 
		          p_buffer.buffer[ p_buffer.position - 4 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 3 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 2 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 1 ].ToString( "X2" ) );
	}
	
	private void LogSendMessageDetail( QXBuffer p_buffer ){
		Debug.Log( "Send Message Index: " + p_buffer.m_protocol_index + " " +
		          "   Header & Tailer: " + 
		          p_buffer.buffer[ 0 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 1 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 2 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 3 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 4 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ 5 ].ToString( "X2" ) + " " +
		          "   &   " + 
		          p_buffer.buffer[ p_buffer.position - 4 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 3 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 2 ].ToString( "X2" ) + " " +
		          p_buffer.buffer[ p_buffer.position - 1 ].ToString( "X2" ) );
	}


	/// true only when socket is surely setup.
	public static bool IsConnected(){
		return m_state == SocketState.Connected;
	}

	public static bool IsDisConnected(){
		return m_state == SocketState.DisConnected;
	}

	public static SocketState GetSocketState(){
		return m_state;
	}

	/** Return:
	 * 
	 * true, could send.
	 * false, not connected.
	 * 
	 */
	public static bool CanSendMessage( short p_protocol_index ){
		if( !IsConnected() ){
			Debug.Log( "CheckToSendMessage socket.state: " + GetSocketState() + " - " + p_protocol_index );
		}

		return IsConnected();
	}
	
	public static bool WillReconnect(){
		if( m_state == SocketState.Connecting || m_state == SocketState.Connected ){
			return false;
		}
		
		return true;
	}
	
	private void ConnectFailed(){
		Debug.Log( "ConnectFailed()" );

		// process all received messages.
//		{
//			Process_Received_Messages();
//		}

		if( m_state == SocketState.Connecting || 
		   m_state == SocketState.Connected || 
		   m_state == SocketState.ConnectiontLost ){
			SetSocketState( SocketState.ConnectedFailed );
		}
		
		ShutDown();

		{
			SocketHelper.CreateConnectionLostOrFailWindow();
		}
	}
	
	private void ManualLostConnection(){
		Debug.Log( "SocketTool.ManualLostConnection( " + m_state + " )" );

		// process all received messages.
//		{
//			Process_Received_Messages();
//		}

		if( m_state == SocketState.Connecting || 
		   m_state == SocketState.Connected || 
		   m_state == SocketState.ConnectedFailed ){
			SetSocketState( SocketTool.SocketState.ConnectiontLost );
		}
		
		ShutDown();

		{
			SocketHelper.CreateConnectionLostOrFailWindow();
		}
	}

	#endregion

		
	
	#region Quick Config
	

	
	#endregion


}
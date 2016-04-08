//#define DEBUG_WAITING

//#define DEBUG_WAITING_INFO



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class NetworkWaiting : MonoBehaviour {

	private enum ENUM_NETWORK_WAITING{
		SENDING_WAITING,
		RECEIVING_WAITING,
		HIDING
	}

	public UITexture m_tips_tex;

	public UILabel m_lb_title;

	public UILabel m_lb_time;

	public UILabel m_lb_tips;

	public UISprite m_spt_bg;


	private static NetworkWaiting m_instance = null;

	public static bool m_instance_exist = false;

	private ENUM_NETWORK_WAITING m_waiting_state = ENUM_NETWORK_WAITING.HIDING;


	private string CONST_TITLE_SENDING		= "Network Sending:";

	private string CONST_TITLE_RECEIVING	= "Network Receiving:";

	private string m_default_title = "";

	private float m_show_time	= -10.0f;

	public static NetworkWaiting Instance( bool p_create_if_null = false ){
		if( !m_instance_exist && p_create_if_null ){
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.NETWORK_WAITING_TIPS ),
			                        ResourceLoadCallback );
		}

		return m_instance;
	}

	public static void LoadResourcesUI(){
		Global.ResourcesDotLoad( "_UIs/_CommonAtlas/Network/UI_NetworkWaiting", ResourceLoadCallback );
	}

	public static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		if( !m_instance_exist ){
			GameObject t_sending_tips = (GameObject)GameObject.Instantiate( p_object );

			if( t_sending_tips == null ){
				Debug.LogError( "Instantiate to null." );

				return;
			}

			{
				m_instance = t_sending_tips.GetComponentInChildren<NetworkWaiting>();
				
				m_instance_exist = true;

//				UpdateShowWaiting();
			}
			
			{
				DontDestroyOnLoad( t_sending_tips );
				
				t_sending_tips.SetActive( false );
				
				#if DEBUG_WAITING
				Debug.Log( "Set NetworkWaiting GameObject inactive." );
				#endif
			}

			if( HttpRequest.HaveActiveWWW() ){
				Instance().ShowNetworkSending( HttpRequest.GetFirstWWWUrl() );
			}
		}
	}

	#region Mono

	// Use this for initialization
	void Start () {
		{
			bool t_enable_detail = ConfigTool.GetBool( ConfigTool.CONST_NETWORK_SHOW_STATUS );

			#if DEBUG_WAITING_INFO
			t_enable_detail = true;
			#endif

			{
				m_spt_bg.gameObject.SetActive( true );	
			}

			{
				m_lb_title.gameObject.SetActive( t_enable_detail );
				
				m_lb_time.gameObject.SetActive( t_enable_detail );
				
				m_lb_tips.gameObject.SetActive( t_enable_detail );
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( IsShowingSending() || IsShowingReceiving() ){
			UpdateTitleWithTime();

			CheckTimeOut();
		}

		if( Instance().gameObject.activeSelf && IsHiding() ){
			#if DEBUG_WAITING
			Debug.Log( "NetworkWaiting.Hide.GameObject( " + 
			          IsHiding() + " , " + 
			          Instance().gameObject.activeSelf + " )" );
			#endif

			Instance().gameObject.SetActive( false );

			if( m_lb_tips != null ){
				m_lb_tips.text = "";
			}
		}

		{
			UpdateTipsTexture();
		}
	}

	void OnDestroy(){
		m_instance = null;

		m_instance_exist = false;
	}

	#endregion



	#region Time Update

	private void ResetShowTime(){
		#if DEBUG_WAITING
		Debug.Log( "ResetShowTime: " + Time.realtimeSinceStartup );
		#endif
		m_show_time = Time.realtimeSinceStartup;
	}

	private float GetWaitTime(){
		return Time.realtimeSinceStartup - m_show_time;
	}

	private void CheckTimeOut(){
		if( GetWaitTime() > ConfigTool.GetFloat( ConfigTool.CONST_NETOWRK_SOCKET_TIME_OUT, 10.0f ) ){
			#if DEBUG_WAITING
//			Debug.Log( "Message Time Out: " + Time.realtimeSinceStartup  + " / " + m_show_time + " - " + m_waiting_state );
			#endif

			{
				HideNeworkWaiting();

				SocketTool.Instance().ClearSendAdnReceiveMessages();

				if( m_reconnect_gb == null ){
					#if DEBUG_WAITING
					Debug.Log( "Create Reconnect Box." );

					Debug.Log( "Show ReconnectUI, proto waiting: " + GetWaitingProtoDesc() );
					#endif

					SocketHelper.CreateTimeOutReConnectWindow( ReConnectCallback, OnReconnectBoxCreated );
				}
				else{
//					Debug.Log( "Reconnect Box Already Exist." );
				}
			}
		}
	}

	private GameObject m_reconnect_gb = null;

	public void OnReconnectBoxCreated( GameObject p_gb ){
//		Debug.Log( "OnReconnectBoxCreated( " + p_gb + " )" );

		m_reconnect_gb = p_gb;
	}

	public void ReConnectCallback( int p_i ){
//		Debug.Log( "NetworkWaiting.ReConnectCallback( " + p_i + " )" );

		m_reconnect_gb = null;

//		{
//			if( SocketTool.IsConnected() ){
//				SocketTool.Instance().SendSocketMessage( ProtoIndexes.NETWORK_CHECK, "101" );
//			}
//		}

		{
			SocketTool.CloseSocket();

			SocketHelper.ReLoginClickCallback( 0 );
		}
	}

	void UpdateTipsTexture(){
		if( m_tips_tex == null ){
			return;
		}

		float t_time = Time.realtimeSinceStartup - (int)Time.realtimeSinceStartup;

		Vector3 t_euler = m_tips_tex.gameObject.transform.localEulerAngles;

		t_euler.z = -t_time * 360.0f;

		m_tips_tex.transform.localEulerAngles = t_euler;
	}

	#endregion



	#region Network Process
	
	public bool OnProcessSocketMessage( QXBuffer p_message ){
		if( p_message.m_protocol_index == ProtoIndexes.NETWORK_CHECK_RET ){
			Debug.Log( "Network OK." );
			
			return true;
		}

		return false;
	}
	#endregion



	#region Waiting Status

	public bool IsHiding(){
		if( !m_instance_exist ){
			Debug.LogError( "Error, HideNeworkTips.NetworkSending not found." );

			return true;
		}

		if( Instance().gameObject.activeSelf ){
			return m_waiting_state == ENUM_NETWORK_WAITING.HIDING;
		}

		return true;
	}
		
	public bool IsShowingSending(){
		if( !m_instance_exist ){
			Debug.LogError( "Error, HideNeworkTips.NetworkSending not found." );

			return false;
		}
		
		if( Instance().gameObject.activeSelf ){
			return m_waiting_state == ENUM_NETWORK_WAITING.SENDING_WAITING;
		}
		else{
			return false;
		}
	}

	public bool IsShowingReceiving(){
		if( !m_instance_exist ){
			Debug.LogError( "Error, HideNeworkTips.NetworkSending not found." );

			return false;
		}
		
		if( Instance().gameObject.activeSelf ){
			return m_waiting_state == ENUM_NETWORK_WAITING.RECEIVING_WAITING;
		}
		else{
			return false;
		}
	}

	#endregion



	#region Waiting Config

//	private static bool m_show_waiting_state = true;
//
//	public static void SetShowWaiting( bool p_show_status ){
//		m_show_waiting_state = p_show_status;
//	}
//
//	public static bool GetShowWaiting(){
//		return m_show_waiting_state;
//	}
//
//	private static void UpdateShowWaiting(){
//		if( Instance().m_lb_time != null ){
//			Instance().m_lb_time.gameObject.SetActive( m_show_waiting_state );
//		}
//		
//		if( Instance().m_lb_tips != null ){
//			Instance().m_lb_tips.gameObject.SetActive( m_show_waiting_state );
//		}
//		
//		if( Instance().m_lb_title != null ){
//			Instance().m_lb_title.gameObject.SetActive( m_show_waiting_state );
//		}
//		
//		if( Instance().m_spt_bg != null ){
//			Instance().m_spt_bg.gameObject.SetActive( m_show_waiting_state );
//		}
//	}

	public void ShowNetworkSending( string p_sending ){
		#if DEBUG_WAITING
		Debug.Log( "ShowNetworkSending( " + p_sending + " ) " + Time.frameCount );
		#endif

		{
			m_waiting_state = ENUM_NETWORK_WAITING.SENDING_WAITING;

			UpdateWaitingUI( CONST_TITLE_SENDING, p_sending );
		}

		{
			SetWaitingProtoDesc( p_sending );
		}
	}

	public void ShowNetworkSending( int p_proto_index ){
		#if DEBUG_WAITING
		Debug.Log( "ShowNetworkSending( " + p_proto_index + " ) " + Time.frameCount );
		#endif

		{
			ShowNetworkSending( "" + p_proto_index );
		}

		{
			SetWaitingProtoDesc( "" + p_proto_index );
		}
	}

	public void ShowNetworkReceiving( int p_proto_index ){
		{
			ShowNetworkReceiving( "" + p_proto_index );
		}

		{
			SetWaitingProtoDesc( "" + p_proto_index );
		}
	}

	public void ShowNetworkReceiving( string p_proto_waiting ){
		#if DEBUG_WAITING
		Debug.Log( "ShowNetworkReceiving( " + p_proto_waiting + " ) " + Time.frameCount );
		#endif

		{
			m_waiting_state = ENUM_NETWORK_WAITING.RECEIVING_WAITING;
			
			UpdateWaitingUI( CONST_TITLE_RECEIVING, p_proto_waiting );
		}

		{
			SetWaitingProtoDesc( "" + p_proto_waiting );
		}
	}


	public void HideNeworkWaiting(){
		#if DEBUG_WAITING
		Debug.Log( "HideNeworkWaiting() " + Time.frameCount );
		#endif

		if( !m_instance_exist ){
			Debug.LogError( "Error, HideNeworkTips.NetworkSending not found." );
			
			return;
		}
		
		// update state
		{
//			Instance().gameObject.SetActive( false );

//			if( m_lb_tips != null ){
//				m_lb_tips.text = "";
//			}
			
			m_waiting_state = ENUM_NETWORK_WAITING.HIDING;
		}
	}

	#endregion



	#region Waiting Update

	private string m_waiting_proto_desc = "";

	private string GetWaitingProtoDesc(){
		return m_waiting_proto_desc;
	}

	private void SetWaitingProtoDesc( string p_waiting ){
		m_waiting_proto_desc = p_waiting;
	}

	private void UpdateWaitingUI( string p_title, string p_content ){
		ResetShowTime();

		UpdateWaitingTitle( p_title );

		UpdateWaitingContent( p_content );
	}

	private void UpdateWaitingTitle( string p_title ){
		m_default_title = p_title;

		m_lb_title.text = m_default_title;
	}


	private void UpdateWaitingContent( string p_show ){
		// update state
		{
			Instance().gameObject.SetActive( true );
		}

		if( m_lb_tips != null ){
			if( m_lb_tips.text == "" ){
				m_lb_tips.text = p_show;
			}
			else{
				m_lb_tips.text = m_lb_tips.text + "\n" + p_show;
			}
		}
		else{
			Debug.LogError( "Error, Label = null." );
		}
	}

	private void UpdateTitleWithTime(){
		// add time 
		{
			float t_time = GetWaitTime();

			t_time = MathHelper.FloatPrecision( t_time, 2 );

			m_lb_time.text = t_time + "";
		}
	}

	#endregion
}

//#define DEBUG_SOCKET_HELPER

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class SocketHelper {

	#region Register Global Processor And Listener

	/**Any global listener and processor could be registered here.
	 * 
	 * Notes:
	 * 1.Execute when SocketTool.Awake().
	 * 2.Should not manual enovke this.
	 * 3.Every processor or listener should be only exist once.
 	 */
	public static void RegisterGlobalProcessorAndListeners(){
		#if DEBUG_SOCKET_HELPER
		Debug.Log( "RegisterGlobalProcessorAndListeners()" );
		#endif

		GameObject t_gb = GameObjectHelper.GetDontDestroyOnLoadGameObject ();

		ComponentHelper.AddIfNotExist ( t_gb, typeof(PushAndNotificationHelper) );

		ComponentHelper.AddIfNotExist ( t_gb, typeof(PlayerSceneSyncManager) );
	}

	#endregion

		
	
	#region Message Process
	
	/// <summary>
	/// Send QX Serialized message to server.
	/// </summary>
	/// <param name="value">message to send</param>
	/// <param name="protoIndex">message index</param>
	public static void SendQXMessage( object value, int protoIndex ){
		MemoryStream memStream = new MemoryStream();
		
		QiXiongSerializer qxSer = new QiXiongSerializer();
		qxSer.Serialize(memStream, value);
		byte[] t_protof = memStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage((short)protoIndex, ref t_protof);
	}
	
	/// <summary>
	/// Send QX message index to server.
	/// </summary>
	/// <param name="protoIndex">message index</param>
	public static void SendQXMessage( int protoIndex ){
		SocketTool.Instance().SendSocketMessage((short)protoIndex);
	}
	
	/// <summary>
	/// Transfer source message to target object, used in receiving message from sever.
	/// </summary>
	/// <param name="targetObject">target object</param>
	/// <param name="p_message">source message</param>
	/// <returns>true if transfer succeed, false if not</returns>
	public static bool ReceiveQXMessage( ref object targetObject, QXBuffer p_message, int index ){
		if (p_message == null || p_message.m_protocol_index != index)
		{
			return false;
		}
		
		//Execute received msg.
		MemoryStream memStream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
		QiXiongSerializer qxSer = new QiXiongSerializer();
		qxSer.Deserialize(memStream, targetObject, targetObject.GetType());
		
		return true;
	}
	
	#endregion



	#region Socket Status

	private static float m_last_socket_check_time = 0.0f;
	
	public static Queue<float> m_socket_check_send_queue = new Queue<float>();
	
	private const int MAX_SOCKET_CHECK_QUEUE_COUNT	= 10;
	
	public static void ClearNetWorkCheckQueue(){
		m_socket_check_send_queue.Clear();

		m_last_socket_check_time = 0.0f;
	}
	
	public static void UpdateNetworkStatusCheck(){
		if( !SocketTool.IsConnected() ){
			Debug.Log( "Should only see this int low rate." );
			
			ClearNetWorkCheckQueue();
			
			return;
		}

		// network ping
		{
			UpdateNetworkPing();
		}

		// network status check
		{
			UpdateNetworkStatusSend();
			
			UpdateNetworkStatusReceive();
		}
	}

	private static void UpdateNetworkStatusSend(){
		float t_time = ConfigTool.GetFloat( ConfigTool.CONST_NETWORK_CHECK_TIME );
		
		if( Time.realtimeSinceStartup - m_last_socket_check_time < t_time ){
			return;
		}
		
		m_last_socket_check_time = Time.realtimeSinceStartup;
		
		if( m_socket_check_send_queue.Count >= MAX_SOCKET_CHECK_QUEUE_COUNT ){
			Debug.Log( "Socket Check Queue out of bounds." );
			
			return;
		}
		
		SocketTool.Instance().SendSocketMessage( ProtoIndexes.NETWORK_CHECK, false );
		
		m_socket_check_send_queue.Enqueue( Time.realtimeSinceStartup );
	}
	
	private static void UpdateNetworkStatusReceive(){
		if( m_socket_check_send_queue.Count <= 0 ){
			return;
		}
		
		if ( TimeHelper.GetCurrentTime_Second() - GetDataReceived_Sec () < ConfigTool.GetFloat ( ConfigTool.CONST_NETOWRK_SOCKET_TIME_OUT ) ) {
			ClearNetWorkCheckQueue();
			
			return;
		}
		
		float t_sent_time = m_socket_check_send_queue.Peek();
		
		if( Time.realtimeSinceStartup - t_sent_time > ConfigTool.GetFloat( ConfigTool.CONST_NETOWRK_SOCKET_TIME_OUT ) ){
			Debug.Log( "Socket Status Check Fail: " + ( Time.realtimeSinceStartup - t_sent_time ) );
			
			ClearNetWorkCheckQueue();
			
			Debug.Log( "Proto: 101, Not responding." );
			
			// Tell Server client is going to close connection.
			{
				SocketTool.Instance().SendSocketMessage( ProtoIndexes.C_DROP_CONN, false );
			}

			SocketTool.ConnectionTimeOut();
		}
	}

	#endregion



	#region Receive Data Tag
	
	private static long m_last_data_received_time_ms = 0;
	
	public static void SocketDataReceived(){
		m_last_data_received_time_ms = TimeHelper.GetCurrentTime_MilliSecond();
	}
	
	public static float GetDataReceived_Sec(){
		return m_last_data_received_time_ms / 1000.0f;
	}

	public static long GetDataReceived_MS(){
		return m_last_data_received_time_ms;
	}

	#endregion



	#region Ping

	private static float m_last_ping_time	= 0.0f;

	private const float PING_INTERVAL		= 3.0f;

	private static void UpdateNetworkPing(){
		if( Time.realtimeSinceStartup - m_last_ping_time >= PING_INTERVAL ){
			Console_SetNetwork.OnPing( null, false );

			m_last_ping_time = Time.realtimeSinceStartup;
		}
	}

	#endregion
}

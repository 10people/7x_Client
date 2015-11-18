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
}

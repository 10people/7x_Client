using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXComData {

	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (object value, int protoA ,string protoB = null)
	{
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,value);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),ref t_protof,protoB);
	}

	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (int protoA ,string protoB = null)
	{	
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),protoB);
	}

	/// <summary>
	/// Receives the qx proto message.
	/// </summary>
	/// <returns>The qx proto message.</returns>
	/// <param name="p_message">P_message.</param>
	/// <param name="value">Value.</param>
	public static object ReceiveQxProtoMessage (QXBuffer p_message,object value)
	{
		MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Deserialize(t_stream, value, value.GetType());

		object o_value = value;

		return o_value;
	}
}

using UnityEngine;
using System.Collections;

using qxmobile;
using qxmobile.protobuf;

public class DebugLoginTool : MonoBehaviour, SocketProcessor {



	#region Mono

	void Start () {
		{
			SocketTool.RegisterMessageProcessor( this );
		}
	}
	
	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor( this );
	}

	#endregion

	public void DebugSingleProto(){
//		Debug.Log( "DebugLoginTool.DebugSingleProto() " );
//		
//		if( !SocketTool.Instance().IsConnected() ){
//			SocketTool.Instance().Connect();
//		}

		/*
		if( SocketTool.Instance().IsConnected() ){
			SocketTool.Instance().SendSocketMessage( ProtoIndexes.DEBUG_PROTO_WITHOUT_CONTENT );
		}
		*/
	}
	


	
	#region Network Process
	
	public bool OnProcessSocketMessage( QXBuffer p_message ){
//		Debug.Log( "DebugLoginTool.OnProcessSocketMessage: " + p_message.m_protocol_index );
//		
//		// Debug Proto m_Index
//		if( p_message.m_protocol_index == ProtoIndexes.DEBUG_PROTO_WITHOUT_CONTENT_RET ){
//			Debug.Log( "DEBUG_PROTO_WITHOUT_CONTENT_RET()" );
//			
//			return true;
//		}
		
		return false;
	}

	#endregion



}

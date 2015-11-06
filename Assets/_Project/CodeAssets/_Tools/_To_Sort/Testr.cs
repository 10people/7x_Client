using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;

public class Testr : MonoBehaviour, SocketListener
{

    void Awake(){
        SocketTool.RegisterSocketListener( this );
    }

	void Destroy(){
		SocketTool.UnRegisterSocketListener( this );
	}

    public bool OnSocketEvent(QXBuffer p_message){
		if( p_message == null ){
			return false;
		}

		switch (p_message.m_protocol_index){
			case ProtoIndexes.PVE_PAGE_RET:{
				Debug.Log("有数据返回");
				
				return true;
			}
				
			default:
				return false;
		}
    }
	
}

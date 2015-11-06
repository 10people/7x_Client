using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnterNewScene: MonoBehaviour
{
    public string m_new_scene_name;

    void OnClick()
    {
        Debug.LogError( "Loading new scene" );

		PlayerState tempState = new PlayerState();
		tempState.s_state = State.State_LOADINGSCENE;
		
		MemoryStream tempStream = new MemoryStream();
		QiXiongSerializer t_qx = new QiXiongSerializer();
		t_qx.Serialize(tempStream, tempState);
		
		byte[] t_protof;
		t_protof = tempStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PLAYER_STATE_REPORT, ref t_protof);

//		CityGlobalData.m_nextSceneName = m_new_scene_name;
//
//		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );

		SceneManager.EnterSomeScene( m_new_scene_name );
    }
}

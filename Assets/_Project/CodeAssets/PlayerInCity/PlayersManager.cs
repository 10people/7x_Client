using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayersManager : MonoBehaviour, SocketProcessor { //所有在主城中的玩家管理类

    public static Dictionary<int, ErrorMessage> m_playrHeadInfo = new Dictionary<int, ErrorMessage>();  //主城玩家集合
    void Awake(){

	}

	void Start(){
		SocketTool.RegisterMessageProcessor( this );
	}
	
	//创建玩家
    void CreatePlayer( EnterScene tempEnterPlayer )
    {
		PlayerInCityManager.Instance().CreatePlayer( tempEnterPlayer );
    }

	//更新玩家名字位置
    public static void UpdatePlayerNamePosition() 
    {
		foreach ( GameObject tempPlayer in PlayerInCityManager.m_playrDic.Values )
        {
            if (tempPlayer != null)
            {
                PlayerNameManager.UpdatePlayerNamePosition(tempPlayer.GetComponent<PlayerInCity>());
            }
        }
    }


    public static void DestoryPlayer(ExitScene tempPlayer) //删除玩家
    {
		//删除3d模型
        PlayerInCityManager.DestroyPlayer(tempPlayer); 

		//及对应的名字
        PlayerNameManager.DestroyPlayerName(tempPlayer); 
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.Enter_Scene: //有玩家进入主城
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        EnterScene tempScene = new EnterScene();

                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                        CreatePlayer(tempScene);

                    }
                    return true;

                //case ProtoIndexes.Enter_Scene_Confirm: //我自己进入主城
                //{
                //    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position );

                //    EnterSceneConfirm tempConfirm = new EnterSceneConfirm();

                //    QiXiongSerializer t_qx = new QiXiongSerializer();

                //    t_qx.Deserialize(t_stream,tempConfirm,tempConfirm.GetType());

                //}return true;

                case ProtoIndexes.Sprite_Move: //玩家移动
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        SpriteMove tempMove = new SpriteMove();

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        t_qx.Deserialize(t_stream, tempMove, tempMove.GetType());

                        PlayerInCityManager.UpdatePlayerPosition(tempMove);

                    }
                    return true;

                case ProtoIndexes.ExitScene: //玩家退出主城
                    {
                        //                Debug.Log("sssssssssssssssssssssssssssssssssssssssssssssssssssssss");

                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        ExitScene tempScene = new ExitScene();

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());
                        DestoryPlayer(tempScene);

                    }
                    return true;
                case ProtoIndexes.S_HEAD_STRING: //玩家称号、vip更新
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        ErrorMessage tempScene = new ErrorMessage();

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());
                        if (!m_playrHeadInfo.ContainsKey(tempScene.errorCode))
                        {
                            m_playrHeadInfo.Add(tempScene.errorCode, tempScene);
                        }
                        else
                        {
                            m_playrHeadInfo[tempScene.errorCode] = tempScene;
                        }
                        PlayerNameManager.UpdateAllLabel(tempScene);
                    }
                    return true;

                default: break;
            }
        }
        return false;
    }

    void LateUpdate() //更新玩家位置和玩家名字位置
    {
        PlayerInCityManager.Instance().UpdatePlayerPosition();

        UpdatePlayerNamePosition();
    }

    void OnDestroy()
    {
		SocketTool.UnRegisterMessageProcessor( this );
    }
}

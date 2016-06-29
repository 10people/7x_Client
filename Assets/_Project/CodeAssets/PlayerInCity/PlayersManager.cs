using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlayersManager : MonoBehaviour, SocketProcessor { //所有在主城中的玩家管理类
    public static PlayersManager m_PlsyersInfo;
    public static Dictionary<int, ErrorMessage> m_playrHeadInfo = new Dictionary<int, ErrorMessage>();

    public static int m_Self_UID = 0;

    public struct OtherPlayerInfo
    {
        public int _UID;
        public string _Name;
        public string _AllianceName;
        public string _Designation;
        public string _VInfo;
        public int _Duty;
        public string _Greet;
        public Vector3 _SeverPos;
        public int _RoleId;
        public long _MonarchId;
    }
    //public Dictionary<int, OtherPlayerInfo> m_playerPartInfo = new Dictionary<int, OtherPlayerInfo>();  //主城玩家信息集合
    //public Dictionary<int, OtherPlayerInfo> m_playerDetailedInfo = new Dictionary<int, OtherPlayerInfo>();  //主城玩家信息集合
    
    void Awake(){
        m_PlsyersInfo = this;
    }

	void Start(){
		SocketTool.RegisterMessageProcessor( this );
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
                        OtherPlayerInfo part_info = new OtherPlayerInfo();
                        part_info._Name = tempScene.senderName;
                        part_info._UID = tempScene.uid;
                        part_info._RoleId = tempScene.roleId;
                        part_info._SeverPos = new Vector3(tempScene.posX, tempScene.posY, tempScene.posZ);
                        part_info._MonarchId = tempScene.jzId;
                        part_info._Designation = tempScene.chengHao.ToString();
                        part_info._Duty = tempScene.zhiWu;
                        part_info._AllianceName = tempScene.allianceName;
                        part_info._VInfo = tempScene.vipLevel.ToString();
                        CreatePlayer(part_info);
                    }
                    return true;

                case ProtoIndexes.Enter_Scene_Confirm: //我自己进入主城
                {
                     MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position );

                    EnterSceneConfirm tempConfirm = new EnterSceneConfirm();

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    t_qx.Deserialize(t_stream,tempConfirm,tempConfirm.GetType());
                    m_Self_UID = tempConfirm.uid;
                }
                    return true;

                case ProtoIndexes.Sprite_Move: //玩家移动
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        SpriteMove tempMove = new SpriteMove();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Deserialize(t_stream, tempMove, tempMove.GetType());
                        PlayerInCityManager.m_PlayerInCity.UpdatePlayerPosition(tempMove);
                        UpdatePlayerNamePosition();

                    }
                    return true;

                case ProtoIndexes.ExitScene: //玩家退出主城
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        ExitScene tempScene = new ExitScene();

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());
                        DestoryPlayer(tempScene);

                    }
                    return true;
                case ProtoIndexes.S_HEAD_INFO: //玩家称号、vip更新
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EnterScene tempScene = new EnterScene();
                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());
                        OtherPlayerInfo part_info = new OtherPlayerInfo();
                        part_info._Name = tempScene.senderName;
                        part_info._UID = tempScene.uid;
                        part_info._RoleId = tempScene.roleId;
                        part_info._SeverPos = new Vector3(tempScene.posX, tempScene.posY, tempScene.posZ);
                        part_info._MonarchId = tempScene.jzId;
                        part_info._Designation = tempScene.chengHao.ToString();
                        part_info._Duty = tempScene.zhiWu;
                        part_info._AllianceName = tempScene.allianceName;
                        part_info._VInfo = tempScene.vipLevel.ToString();
                        Fresh_HeadInfo(part_info);
                    }
                    return true;
                case ProtoIndexes.S_HEAD_STRING: //玩家称号、vip更新
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        ErrorMessage tempScene = new ErrorMessage();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());
                        if (tempScene != null)
                        {
                            OtherPlayerInfo part_info = new OtherPlayerInfo();
                            part_info._UID = tempScene.errorCode;
                            if (tempScene.errorDesc.IndexOf('#') > -1)
                            {
                                string[] info = tempScene.errorDesc.Split('#');
                                if (tempScene.errorDesc.IndexOf("Face") > -1)
                                {
                                    string greet_Info = info[8].ToString();

                                    part_info._Greet = greet_Info;
                                }
                                else
                                {
                                    part_info._Greet = null;
                                }
                            }
 
                            Fresh_HeadInfo(part_info);
                        }
                    }
                    return true;

                default: break;
            }
        }
        return false;
    }

    //创建玩家
    void CreatePlayer(OtherPlayerInfo u_info)
    {
        PlayerInCityManager.m_PlayerInCity.CreatePlayer(u_info);
    }
    void LateUpdate()
    {
        UpdatePlayerNamePosition();
    }

    //更新玩家名字位置
    public static void UpdatePlayerNamePosition()
    {
        foreach (GameObject tempPlayer in PlayerInCityManager.m_playrDic.Values)
        {
            if (tempPlayer != null)
            {
                PlayerNameManager.UpdatePlayerNamePosition(tempPlayer.GetComponent<PlayerInCity>().m_playerID, tempPlayer);
            }
        }
    }

    //更新玩家头顶信息
    void Fresh_HeadInfo(OtherPlayerInfo u_info)
    {
        PlayerNameManager.CheckPlayer(u_info);
    }
    public static void DestoryPlayer(ExitScene tempPlayer) //删除玩家
    {
        //删除3d模型
        PlayerInCityManager.DestroyPlayer(tempPlayer);

     
        //及对应的名字
        PlayerNameManager.DestroyPlayerName(tempPlayer);
    }
    void OnDestroy()
    {
        m_PlsyersInfo = null;
        SocketTool.UnRegisterMessageProcessor( this );
    }
}

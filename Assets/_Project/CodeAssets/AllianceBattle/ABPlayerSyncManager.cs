//#define DEBUG_MOVE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABPlayerSyncManager : PlayerManager, SocketListener
    {
        public RootManager m_RootManager;

        public struct DeadInfoCollector
        {
            public int m_UID;
            public int m_RoleID;
            public long m_JunzhuID;

            public string m_KingName;
            public string m_AllianceName;
            public int m_VipLevel;
            public int m_Title;
            public int m_AlliancePost;
            public int m_Nation;
            public int m_Level;
            public int m_BattleValue;
            public float m_TotalBlood;
        }

        /// <summary>
        /// Key for id, value for total ids, contains self player info.
        /// </summary>
        public Dictionary<long, DeadInfoCollector> m_DeadPlayerDic = new Dictionary<long, DeadInfoCollector>();

        public override void AddTrackCamera(OtherPlayerController temp)
        {
            temp.TrackCamera = m_RootManager.TrackCamera;
        }

        private void AddToDeadDic(long u_id, DeadInfoCollector collector)
        {
            if (m_DeadPlayerDic.ContainsKey(u_id)) return;

            m_DeadPlayerDic.Add(u_id, collector);
        }

        private void RemoveFromDeadDic(int u_id)
        {
            if (m_DeadPlayerDic.ContainsKey(u_id))
            {
                m_DeadPlayerDic.Remove(u_id);
            }
        }

        public PlayerDeadNotify m_StoredPlayerDeadNotify;

        /// <summary>
        /// Receive character sync message from server.
        /// </summary>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.ALLIANCE_BATTLE)) return false;

            if (p_message != null)
            {
                //Debug.LogWarning("---------------Receive message");

                //Refresh message state.
                m_LatestServerSyncTime = Time.realtimeSinceStartup;

                switch (p_message.m_protocol_index)
                {
                    //Other player enter scene
                    case ProtoIndexes.Enter_Scene:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            EnterScene tempMsg = new EnterScene();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            //MemoryStream t_stream2 = new MemoryStream(tempMsg2.body, 0, tempMsg2.body.Length);
                            //QiXiongSerializer t_qx2 = new QiXiongSerializer();
                            //EnterScene tempMsg = new EnterScene();
                            //t_qx2.Deserialize(t_stream2, tempMsg, tempMsg.GetType());

                            //tempMsg.posX = tempMsg2.posX;
                            //tempMsg.posY = tempMsg2.posY;
                            //tempMsg.posZ = tempMsg2.posZ;

                            ABPlayerCultureController tempBaseCultureController;

                            //Only limit player pos, change to all if AllianceBattle move type changed.
                            Vector2 limitedPosition = new Vector2(tempMsg.posX, tempMsg.posZ);

                            //Self
                            if (tempMsg.uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                            {
                                tempBaseCultureController = m_RootManager.m_SelfPlayerCultureController;
                                tempBaseCultureController.transform.localPosition = m_RootManager.LimitPlayerPositionByHoldPoint(new Vector3(limitedPosition.x, RootManager.BasicYPosition, limitedPosition.y));

                                //Update self head icon info.
                                m_RootManager.m_AllianceBattleMain.SelfIconSetter.SetPlayer(tempMsg.roleId, true, tempMsg.level, tempMsg.senderName, tempMsg.allianceName, tempMsg.totalLife, tempMsg.currentLife, tempMsg.guojia, tempMsg.vipLevel, tempMsg.zhanli, 0);

                                //Set self part.
                                if (tempMsg.horseType == 1)
                                {
                                    m_RootManager.MyPart = 2;
                                }
                                else if (tempMsg.horseType == 2)
                                {
                                    m_RootManager.MyPart = 1;
                                }
                            }
                            //Other
                            else
                            {
                                if (!CreatePlayer(tempMsg.roleId, tempMsg.uid, m_RootManager.LimitPlayerPositionByHoldPoint(new Vector3(limitedPosition.x, RootManager.BasicYPosition, limitedPosition.y)), m_RootManager.PlayerParentObject.transform))
                                {
                                    Debug.LogError("Cannot create duplicated player.");
                                    return true;
                                }

                                //Add to mesh controller.
                                if (m_PlayerDic.ContainsKey(tempMsg.uid))
                                {
                                    ModelAutoActivator.RegisterAutoActivator(m_PlayerDic[tempMsg.uid].gameObject);
                                }

                                tempBaseCultureController = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>();

                                //Add gizmos.
                                int typeID;

                                if ((!string.IsNullOrEmpty(tempMsg.allianceName) && !AllianceData.Instance.IsAllianceNotExist && (tempMsg.allianceName == AllianceData.Instance.g_UnionInfo.name)) || (JunZhuData.Instance().m_junzhuInfo.name == tempMsg.senderName))
                                {
                                    typeID = tempMsg.senderName == JunZhuData.Instance().m_junzhuInfo.name ? 0 : 2;
                                }
                                else
                                {
                                    typeID = 4;
                                }
                                m_RootManager.m_AllianceBattleMain.m_MapController.AddGizmos(tempMsg.uid, typeID, new Vector3(limitedPosition.x, RootManager.BasicYPosition, limitedPosition.y), 0);
                            }

                            tempBaseCultureController.TrackCamera = m_RootManager.TrackCamera;
                            tempBaseCultureController.KingName = tempMsg.senderName;
                            tempBaseCultureController.AllianceName = tempMsg.allianceName;
                            tempBaseCultureController.AlliancePost = tempMsg.zhiWu;
                            tempBaseCultureController.Vip = tempMsg.vipLevel;
                            tempBaseCultureController.Title = tempMsg.chengHao;
                            tempBaseCultureController.NationID = tempMsg.guojia;
                            tempBaseCultureController.Level = tempMsg.level;
                            tempBaseCultureController.BattleValue = tempMsg.zhanli;
                            tempBaseCultureController.RemainingBlood = tempMsg.currentLife;
                            tempBaseCultureController.TotalBlood = tempMsg.totalLife;
                            tempBaseCultureController.RoleID = tempMsg.roleId;
                            tempBaseCultureController.UID = tempMsg.uid;
                            tempBaseCultureController.JunzhuID = tempMsg.jzId;
                            tempBaseCultureController.SetThis();

                            return true;
                        }
                    //other player moving.
                    case ProtoIndexes.Sprite_Move:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            SpriteMove tempMsg = new SpriteMove();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

#if DEBUG_MOVE
                            Debug.Log("=============Receive, id:" + tempMsg.uid + ", pos:" + new Vector3(tempMsg.posX, tempMsg.posY, tempMsg.posZ) + ", time:" + Time.realtimeSinceStartup);
#endif

                            var temp = m_PlayerDic.Where(item => item.Value.m_UID == tempMsg.uid);
                            if (temp.Any())
                            {
                                int uID = temp.First().Key;
                                UpdatePlayerTransform(uID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), tempMsg.dir);
                            }

                            //Update gizmos.
                            m_RootManager.m_AllianceBattleMain.m_MapController.UpdateGizmosPosition(tempMsg.uid, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), 0);

                            return true;
                        }
                    //other player exit.
                    case ProtoIndexes.ExitScene:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ExitScene tempMsg = new ExitScene();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                //Remove from mesh controller.
                                if (m_PlayerDic.ContainsKey(tempMsg.uid))
                                {
                                    ModelAutoActivator.UnregisterAutoActivator(m_PlayerDic[tempMsg.uid].gameObject);
                                }

                                DestroyPlayer(tempMsg.uid);
                            }

                            if (m_DeadPlayerDic.ContainsKey(tempMsg.uid))
                            {
                                RemoveFromDeadDic(tempMsg.uid);
                            }

                            //Remove gizmos.
                            m_RootManager.m_AllianceBattleMain.m_MapController.RemoveGizmos(tempMsg.uid);

                            return true;
                        }
                    //player/other player dead.
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_DEAD:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerDeadNotify tempMsg = new PlayerDeadNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            //other player dead
                            if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                //Only add player to dead dic
                                AddToDeadDic(tempMsg.uid, new DeadInfoCollector()
                                {
                                    m_RoleID = m_PlayerDic[tempMsg.uid].m_RoleID,
                                    m_UID = m_PlayerDic[tempMsg.uid].m_UID,
                                    m_JunzhuID = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().JunzhuID,
                                    m_KingName = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().KingName,
                                    m_AllianceName = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().AllianceName,
                                    m_VipLevel = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().Vip,
                                    m_Title = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().Title,
                                    m_AlliancePost = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().AlliancePost,
                                    m_Nation = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().NationID,
                                    m_Level = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().Level,
                                    m_BattleValue = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().BattleValue,
                                    m_TotalBlood = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>().TotalBlood,
                                });

                                //Play dead animation.
                                if (m_RootManager.m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(tempMsg.uid, "Dead"))
                                {
                                    //Disable move.
                                    m_PlayerDic[tempMsg.uid].DeactiveMove();

                                    m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(tempMsg.uid, "Dead");
                                }
                            }

                            //self player dead
                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                            {
                                AddToDeadDic(tempMsg.uid, new DeadInfoCollector()
                                {
                                    m_RoleID = CityGlobalData.m_king_model_Id,
                                    m_JunzhuID = m_RootManager.m_SelfPlayerCultureController.JunzhuID,
                                    m_KingName = m_RootManager.m_SelfPlayerCultureController.KingName,
                                    m_AllianceName = m_RootManager.m_SelfPlayerCultureController.AllianceName,
                                    m_VipLevel = m_RootManager.m_SelfPlayerCultureController.Vip,
                                    m_Title = m_RootManager.m_SelfPlayerCultureController.Title,
                                    m_Nation = m_RootManager.m_SelfPlayerCultureController.NationID,
                                    m_Level = m_RootManager.m_SelfPlayerCultureController.Level,
                                    m_BattleValue = m_RootManager.m_SelfPlayerCultureController.BattleValue,
                                    m_AlliancePost = m_RootManager.m_SelfPlayerCultureController.AlliancePost,
                                    m_TotalBlood = m_RootManager.m_SelfPlayerCultureController.TotalBlood,
                                });

                                m_StoredPlayerDeadNotify = tempMsg;

                                //Play dead animation.
                                if (m_RootManager.m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(tempMsg.uid, "Dead"))
                                {
                                    //Disable move.
                                    m_RootManager.m_SelfPlayerController.DeactiveMove();

                                    m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(tempMsg.uid, "Dead");
                                }
                            }

                            //Remove gizmos.
                            m_RootManager.m_AllianceBattleMain.m_MapController.RemoveGizmos(tempMsg.uid);

                            //Show popup text for dead.
                            string l_killerName = "";
                            string l_targetName = "";

                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.killerUid)
                            {
                                l_killerName = ColorTool.Color_Green_00ff00 + m_RootManager.m_SelfPlayerCultureController.KingName + "[-]";
                            }
                            else if (m_PlayerDic.ContainsKey(tempMsg.killerUid))
                            {
                                var controller = m_PlayerDic[tempMsg.killerUid].GetComponent<ABPlayerCultureController>();

                                if (!(((!string.IsNullOrEmpty(controller.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (controller.AllianceName == AllianceData.Instance.g_UnionInfo.name)))))
                                {
                                    l_killerName = ColorTool.Color_Red_c40000 + controller.KingName + "[-]";
                                }
                                else
                                {
                                    if (controller.KingName == JunZhuData.Instance().m_junzhuInfo.name)
                                    {
                                        l_killerName = ColorTool.Color_Green_00ff00 + controller.KingName + "[-]";
                                    }
                                    else
                                    {
                                        l_killerName = ColorTool.Color_Blue_016bc5 + controller.KingName + "[-]";
                                    }
                                }
                            }

                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                            {
                                l_targetName = ColorTool.Color_Green_00ff00 + m_RootManager.m_SelfPlayerCultureController.KingName + "[-]";
                            }
                            else if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                var controller = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>();

                                if (!(((!string.IsNullOrEmpty(controller.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (controller.AllianceName == AllianceData.Instance.g_UnionInfo.name)))))
                                {
                                    l_targetName = ColorTool.Color_Red_c40000 + controller.KingName + "[-]";
                                }
                                else
                                {
                                    if (controller.KingName == JunZhuData.Instance().m_junzhuInfo.name)
                                    {
                                        l_targetName = ColorTool.Color_Green_00ff00 + controller.KingName + "[-]";
                                    }
                                    else
                                    {
                                        l_targetName = ColorTool.Color_Blue_016bc5 + controller.KingName + "[-]";
                                    }
                                }
                            }

                            ClientMain.m_UITextManager.createText(l_killerName + "杀死了" + l_targetName);

                            return true;
                        }
                    //player/other player rebirth.
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_REVIVE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerReviveNotify tempMsg = new PlayerReviveNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            switch (tempMsg.result)
                            {
                                case 0:
                                    {
                                        //Dead dic contains all data.
                                        if (m_DeadPlayerDic.ContainsKey(tempMsg.uid))
                                        {
                                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                                            {
                                                m_RootManager.CreateSelfPlayer(m_DeadPlayerDic[tempMsg.uid].m_RoleID, m_DeadPlayerDic[tempMsg.uid].m_JunzhuID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), m_DeadPlayerDic[tempMsg.uid].m_KingName, m_DeadPlayerDic[tempMsg.uid].m_AllianceName, m_DeadPlayerDic[tempMsg.uid].m_VipLevel, m_DeadPlayerDic[tempMsg.uid].m_Title, m_DeadPlayerDic[tempMsg.uid].m_AlliancePost, m_DeadPlayerDic[tempMsg.uid].m_Nation, m_DeadPlayerDic[tempMsg.uid].m_Level, m_DeadPlayerDic[tempMsg.uid].m_BattleValue, tempMsg.life, m_DeadPlayerDic[tempMsg.uid].m_TotalBlood);

                                                //Show dead dimmer.
                                                m_RootManager.m_AllianceBattleMain.HideDeadWindows();
                                            }
                                            else
                                            {
                                                CreatePlayer(m_DeadPlayerDic[tempMsg.uid].m_RoleID, m_DeadPlayerDic[tempMsg.uid].m_UID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), m_RootManager.PlayerParentObject.transform);

                                                //Add to mesh controller.
                                                if (m_PlayerDic.ContainsKey(tempMsg.uid))
                                                {
                                                    ModelAutoActivator.RegisterAutoActivator(m_PlayerDic[tempMsg.uid].gameObject);
                                                }

                                                var tempPlayerCultureController = m_PlayerDic[tempMsg.uid].GetComponent<ABPlayerCultureController>();

                                                tempPlayerCultureController.TrackCamera = m_RootManager.TrackCamera;
                                                tempPlayerCultureController.KingName = m_DeadPlayerDic[tempMsg.uid].m_KingName;
                                                tempPlayerCultureController.AllianceName = m_DeadPlayerDic[tempMsg.uid].m_AllianceName;
                                                tempPlayerCultureController.Vip = m_DeadPlayerDic[tempMsg.uid].m_VipLevel;
                                                tempPlayerCultureController.AlliancePost = m_DeadPlayerDic[tempMsg.uid].m_AlliancePost;
                                                tempPlayerCultureController.Title = m_DeadPlayerDic[tempMsg.uid].m_Title;
                                                tempPlayerCultureController.NationID = m_DeadPlayerDic[tempMsg.uid].m_Nation;
                                                tempPlayerCultureController.Level = m_DeadPlayerDic[tempMsg.uid].m_Level;
                                                tempPlayerCultureController.BattleValue = m_DeadPlayerDic[tempMsg.uid].m_BattleValue;
                                                tempPlayerCultureController.TotalBlood = m_DeadPlayerDic[tempMsg.uid].m_TotalBlood;
                                                tempPlayerCultureController.RemainingBlood = tempMsg.life;
                                                tempPlayerCultureController.RoleID = m_DeadPlayerDic[tempMsg.uid].m_RoleID;
                                                tempPlayerCultureController.UID = tempMsg.uid;
                                                tempPlayerCultureController.JunzhuID = m_DeadPlayerDic[tempMsg.uid].m_JunzhuID;

                                                tempPlayerCultureController.SetThis();
                                            }

                                            //Add gizmos.
                                            int typeID;

                                            if ((!string.IsNullOrEmpty(m_DeadPlayerDic[tempMsg.uid].m_AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (m_DeadPlayerDic[tempMsg.uid].m_AllianceName == AllianceData.Instance.g_UnionInfo.name)) || (JunZhuData.Instance().m_junzhuInfo.name == m_DeadPlayerDic[tempMsg.uid].m_KingName))
                                            {
                                                typeID = m_DeadPlayerDic[tempMsg.uid].m_KingName == JunZhuData.Instance().m_junzhuInfo.name ? 0 : 2;
                                            }
                                            else
                                            {
                                                typeID = 4;
                                            }
                                            m_RootManager.m_AllianceBattleMain.m_MapController.AddGizmos(tempMsg.uid, typeID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), 0);

                                            RemoveFromDeadDic(tempMsg.uid);
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        ClientMain.m_UITextManager.createText("原地复活元宝不足");
                                        break;
                                    }
                            }

                            return true;
                        }
                }
            }
            return false;
        }

        void Start()
        {
            m_RootManager.CreateSelfPlayer(CityGlobalData.m_king_model_Id, JunZhuData.Instance().m_junzhuInfo.id, new Vector3(PlayerSceneSyncManager.Instance.m_MyselfPosition.x, RootManager.BasicYPosition, PlayerSceneSyncManager.Instance.m_MyselfPosition.z), JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name, JunZhuData.Instance().m_junzhuInfo.vipLv, JunZhuData.m_iChenghaoID, AllianceData.Instance.IsAllianceNotExist ? -1 : AllianceData.Instance.g_UnionInfo.identity, JunZhuData.Instance().m_junzhuInfo.guoJiaId, JunZhuData.Instance().m_junzhuInfo.level, JunZhuData.Instance().m_junzhuInfo.zhanLi);

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_Player_Sync");        }

        void Awake()
        {
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/AllianceBattle/ABHaoJie"));
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/AllianceBattle/ABQinglan"));
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/AllianceBattle/ABQiangwei"));
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/AllianceBattle/ABLuoli"));
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            SocketTool.RegisterSocketListener(this);
        }

        new void OnDestroy()
        {
            base.OnDestroy();

            m_DeadPlayerDic.Clear();

            SocketTool.UnRegisterSocketListener(this);
        }
    }
}

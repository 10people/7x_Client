﻿//#define DEBUG_MOVE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageItemSyncManager : PlayerManager, SocketListener
    {
        public RootManager m_RootManager;

        public static float m_LatestServerSyncTime;

        public struct DeadInfoCollector
        {
            public int m_UID;
            public int m_RoleID;

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

        public override void AddTrackCamera(PlayerController temp)
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
            if (Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE)) return false;

            if (p_message != null)
            {
                //Debug.LogWarning("---------------Receive message");

                //Refresh message state.
                m_LatestServerSyncTime = Time.realtimeSinceStartup;

                switch (p_message.m_protocol_index)
                {
                    //Other player enter scene
                    case ProtoIndexes.ENTER_CARRIAGE_SCENE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            EnterScene tempMsg = new EnterScene();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            CarriageBaseCultureController tempBaseCultureController;

                            //Self
                            if (tempMsg.uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                            {
                                tempBaseCultureController = m_RootManager.m_SelfPlayerCultureController;
                                tempBaseCultureController.transform.localPosition = new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ);

                                //Update self head icon info.
                                m_RootManager.m_CarriageMain.SelfIconSetter.SetThis(tempMsg.roleId, true, tempMsg.level, tempMsg.senderName, tempMsg.allianceName, tempMsg.totalLife, tempMsg.currentLife, tempMsg.guojia, tempMsg.vipLevel, tempMsg.zhanli);
                            }
                            //Other
                            else
                            {
                                if (!CreatePlayer(tempMsg.roleId, tempMsg.uid, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ)))
                                {
                                    Debug.LogError("Cannot create duplicated player.");
                                    return true;
                                }

                                tempBaseCultureController = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>();

                                //Add gizmos.
                                int typeID;
                                if ((string.IsNullOrEmpty(tempMsg.allianceName) || AllianceData.Instance.IsAllianceNotExist || (tempMsg.allianceName != AllianceData.Instance.g_UnionInfo.name)))
                                {
                                    typeID = tempMsg.roleId >= 50000 ? 5 : 4;
                                }
                                else
                                {
                                    if (tempMsg.roleId >= 50000)
                                    {
                                        typeID = tempMsg.senderName == JunZhuData.Instance().m_junzhuInfo.name ? 1 : 3;
                                    }
                                    else
                                    {
                                        typeID = tempMsg.senderName == JunZhuData.Instance().m_junzhuInfo.name ? 0 : 2;
                                    }
                                }
                                m_RootManager.m_CarriageMain.AddGizmos(tempMsg.uid, typeID);
                            }

                            tempBaseCultureController.TrackCamera = m_RootManager.TrackCamera;
                            tempBaseCultureController.KingName = tempMsg.senderName;
                            tempBaseCultureController.AllianceName = tempMsg.allianceName;
                            tempBaseCultureController.IsEnemy =
                                !(((!string.IsNullOrEmpty(tempMsg.allianceName) && !AllianceData.Instance.IsAllianceNotExist && (tempMsg.allianceName == AllianceData.Instance.g_UnionInfo.name))) || (tempMsg.senderName == JunZhuData.Instance().m_junzhuInfo.name));
                            tempBaseCultureController.AlliancePost = tempMsg.zhiWu;
                            tempBaseCultureController.Vip = tempMsg.vipLevel;
                            tempBaseCultureController.Title = tempMsg.chengHao;
                            tempBaseCultureController.NationID = tempMsg.guojia;
                            tempBaseCultureController.Level = tempMsg.level;
                            tempBaseCultureController.BattleValue = tempMsg.zhanli;
                            tempBaseCultureController.HorseLevel = tempMsg.horseType;
                            tempBaseCultureController.Money = tempMsg.worth;
                            tempBaseCultureController.RemainingBlood = tempMsg.currentLife;
                            tempBaseCultureController.TotalBlood = tempMsg.totalLife;
                            tempBaseCultureController.RoleID = tempMsg.roleId;
                            tempBaseCultureController.UID = tempMsg.uid;
                            tempBaseCultureController.SetThis();

                            //Set blood num.
                            m_RootManager.m_CarriageMain.SetRemainingBloodNum(tempMsg.xuePingRemain);

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
                            m_RootManager.m_CarriageMain.UpdateGizmosPosition(tempMsg.uid, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), 0);

                            return true;
                        }
                    //other player exit.
                    case ProtoIndexes.EXIT_CARRIAGE_SCENE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ExitScene tempMsg = new ExitScene();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                DestroyPlayer(tempMsg.uid);
                            }

                            if (m_DeadPlayerDic.ContainsKey(tempMsg.uid))
                            {
                                RemoveFromDeadDic(tempMsg.uid);
                            }

                            //Remove gizmos.
                            m_RootManager.m_CarriageMain.RemoveGizmos(tempMsg.uid);

                            return true;
                        }
                    //player/other player dead.
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_DEAD:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerDeadNotify tempMsg = new PlayerDeadNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            //other player/ carriage dead
                            if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                //Only add player to dead dic
                                if (m_PlayerDic[tempMsg.uid].m_RoleID < 50000)
                                {
                                    AddToDeadDic(tempMsg.uid, new DeadInfoCollector()
                                    {
                                        m_RoleID = m_PlayerDic[tempMsg.uid].m_RoleID,
                                        m_UID = m_PlayerDic[tempMsg.uid].m_UID,
                                        m_KingName = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().KingName,
                                        m_AllianceName = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().AllianceName,
                                        m_VipLevel = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().Vip,
                                        m_Title = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().Title,
                                        m_AlliancePost = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().AlliancePost,
                                        m_Nation = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().NationID,
                                        m_Level = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().Level,
                                        m_BattleValue = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().BattleValue,
                                        m_TotalBlood = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>().TotalBlood,
                                    });
                                }
                            }

                            //self player dead
                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                            {
                                AddToDeadDic(tempMsg.uid, new DeadInfoCollector()
                                {
                                    m_RoleID = CityGlobalData.m_king_model_Id,
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
                            }

                            //Play dead animation.
                            m_RootManager.TryPlayAnimationInAnimator(tempMsg.uid, "Dead");

                            //Remove gizmos.
                            m_RootManager.m_CarriageMain.RemoveGizmos(tempMsg.uid);

                            //Show popup text for dead.
                            string l_killerName = "";
                            string l_targetName = "";

                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.killerUid)
                            {
                                l_killerName = ColorTool.Color_Blue_016bc5 + m_RootManager.m_SelfPlayerCultureController.KingName + "[-]";
                            }
                            else if (m_PlayerDic.ContainsKey(tempMsg.killerUid))
                            {
                                var controller = m_PlayerDic[tempMsg.killerUid].GetComponent<CarriageBaseCultureController>();

                                if (!(((!string.IsNullOrEmpty(controller.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (controller.AllianceName == AllianceData.Instance.g_UnionInfo.name)))))
                                {
                                    l_killerName = ColorTool.Color_Red_c40000 + controller.KingName + "[-]";
                                    if (controller.RoleID >= 50000)
                                    {
                                        l_killerName += "的" + ColorTool.Color_Red_c40000 + "镖马" + "[-]";
                                    }
                                }
                                else
                                {
                                    l_killerName = ColorTool.Color_Blue_016bc5 + controller.KingName + "[-]";
                                    if (controller.RoleID >= 50000)
                                    {
                                        l_killerName += "的" + ColorTool.Color_Blue_016bc5 + "镖马" + "[-]";
                                    }
                                }
                            }

                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                            {
                                l_targetName = ColorTool.Color_Blue_016bc5 + m_RootManager.m_SelfPlayerCultureController.KingName + "[-]";
                            }
                            else if (m_PlayerDic.ContainsKey(tempMsg.uid))
                            {
                                var controller = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>();

                                if (!(((!string.IsNullOrEmpty(controller.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (controller.AllianceName == AllianceData.Instance.g_UnionInfo.name)))))
                                {
                                    l_targetName = ColorTool.Color_Red_c40000 + controller.KingName + "[-]";
                                    if (controller.RoleID >= 50000)
                                    {
                                        l_targetName += "的" + ColorTool.Color_Red_c40000 + "镖马" + "[-]";
                                    }
                                }
                                else
                                {
                                    l_targetName = ColorTool.Color_Blue_016bc5 + controller.KingName + "[-]";
                                    if (controller.RoleID >= 50000)
                                    {
                                        l_targetName += "的" + ColorTool.Color_Blue_016bc5 + "镖马" + "[-]";
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

                            //Dead dic contains all data.
                            if (m_DeadPlayerDic.ContainsKey(tempMsg.uid))
                            {
                                if (PlayerSceneSyncManager.Instance.m_MyselfUid == tempMsg.uid)
                                {
                                    m_RootManager.CreateSelfPlayer(m_DeadPlayerDic[tempMsg.uid].m_RoleID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), m_DeadPlayerDic[tempMsg.uid].m_KingName, m_DeadPlayerDic[tempMsg.uid].m_AllianceName, m_DeadPlayerDic[tempMsg.uid].m_VipLevel, m_DeadPlayerDic[tempMsg.uid].m_Title, m_DeadPlayerDic[tempMsg.uid].m_AlliancePost, m_DeadPlayerDic[tempMsg.uid].m_Nation, m_DeadPlayerDic[tempMsg.uid].m_Level, m_DeadPlayerDic[tempMsg.uid].m_BattleValue, tempMsg.life, m_DeadPlayerDic[tempMsg.uid].m_TotalBlood);

                                    //Show dead dimmer.
                                    m_RootManager.m_CarriageMain.HideDeadWindows();
                                }
                                else
                                {
                                    CreatePlayer(m_DeadPlayerDic[tempMsg.uid].m_RoleID, m_DeadPlayerDic[tempMsg.uid].m_UID, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ));

                                    var tempPlayerCultureController = m_PlayerDic[tempMsg.uid].GetComponent<CarriageBaseCultureController>();

                                    tempPlayerCultureController.TrackCamera = m_RootManager.TrackCamera;
                                    tempPlayerCultureController.KingName = m_DeadPlayerDic[tempMsg.uid].m_KingName;
                                    tempPlayerCultureController.AllianceName = m_DeadPlayerDic[tempMsg.uid].m_AllianceName;
                                    tempPlayerCultureController.Vip = m_DeadPlayerDic[tempMsg.uid].m_VipLevel;
                                    tempPlayerCultureController.AlliancePost = m_DeadPlayerDic[tempMsg.uid].m_AlliancePost;
                                    tempPlayerCultureController.Title = m_DeadPlayerDic[tempMsg.uid].m_Title;
                                    tempPlayerCultureController.NationID = m_DeadPlayerDic[tempMsg.uid].m_Nation;
                                    tempPlayerCultureController.Level = m_DeadPlayerDic[tempMsg.uid].m_Level;
                                    tempPlayerCultureController.BattleValue = m_DeadPlayerDic[tempMsg.uid].m_BattleValue;
                                    tempPlayerCultureController.IsEnemy = string.IsNullOrEmpty(m_DeadPlayerDic[tempMsg.uid].m_AllianceName) || AllianceData.Instance.IsAllianceNotExist || (m_DeadPlayerDic[tempMsg.uid].m_AllianceName != AllianceData.Instance.g_UnionInfo.name);
                                    tempPlayerCultureController.TotalBlood = m_DeadPlayerDic[tempMsg.uid].m_TotalBlood;
                                    tempPlayerCultureController.RemainingBlood = tempMsg.life;
                                    tempPlayerCultureController.RoleID = m_DeadPlayerDic[tempMsg.uid].m_RoleID;
                                    tempPlayerCultureController.UID = tempMsg.uid;

                                    tempPlayerCultureController.SetThis();
                                }

                                //Add gizmos.
                                int typeID;
                                if ((string.IsNullOrEmpty(m_DeadPlayerDic[tempMsg.uid].m_AllianceName) || AllianceData.Instance.IsAllianceNotExist || (m_DeadPlayerDic[tempMsg.uid].m_AllianceName != AllianceData.Instance.g_UnionInfo.name)))
                                {
                                    typeID = m_DeadPlayerDic[tempMsg.uid].m_RoleID >= 50000 ? 5 : 4;
                                }
                                else
                                {
                                    if (m_DeadPlayerDic[tempMsg.uid].m_RoleID >= 50000)
                                    {
                                        typeID = m_DeadPlayerDic[tempMsg.uid].m_KingName == JunZhuData.Instance().m_junzhuInfo.name ? 1 : 3;
                                    }
                                    else
                                    {
                                        typeID = m_DeadPlayerDic[tempMsg.uid].m_KingName == JunZhuData.Instance().m_junzhuInfo.name ? 0 : 2;
                                    }
                                }
                                m_RootManager.m_CarriageMain.AddGizmos(tempMsg.uid, typeID);

                                RemoveFromDeadDic(tempMsg.uid);
                            }

                            return true;
                        }
                }
            }
            return false;
        }

        private void Awake()
        {
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQinglan"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQiangwei"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoli"));
            CarriagePrefab = Resources.Load<GameObject>("_3D/Models/Carriage/Carriage");

            BasicYPosition = -3.0f;
            SocketTool.RegisterSocketListener(this);
        }

        private new void OnDestroy()
        {
            base.OnDestroy();

            m_DeadPlayerDic.Clear();

            SocketTool.UnRegisterSocketListener(this);
        }
    }
}


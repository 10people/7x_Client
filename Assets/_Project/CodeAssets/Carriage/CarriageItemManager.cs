using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageItemManager : PlayerManager, SocketListener
    {
        public RootManager m_RootManager;

        public struct IDCollector
        {
            public int m_UID;
            public int m_RoleID;

            public string m_KingName;
            public string m_AllianceName;
            public float m_TotalBlood;
        }

        /// <summary>
        /// Key for id, value for total ids.
        /// </summary>
        public Dictionary<long, IDCollector> m_DeadPlayerDic = new Dictionary<long, IDCollector>();

        public override void AddTrackCamera(PlayerController temp)
        {
            temp.TrackCamera = m_RootManager.TrackCamera;
        }

        private void AddToDeadDic(long u_id, IDCollector collector)
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
                switch (p_message.m_protocol_index)
                {
                    //Other player enter scene
                    case ProtoIndexes.ENTER_CARRIAGE_SCENE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            EnterScene tempScene = new EnterScene();
                            t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                            if (!CreatePlayer(tempScene.roleId, tempScene.uid, new Vector3(tempScene.posX, tempScene.posY, tempScene.posZ)))
                            {
                                Debug.LogError("Cannot create duplicated player.");
                                return true;
                            }

                            var tempPlayerCultureController = m_PlayerDic[tempScene.uid].GetComponent<CarriagePlayerCultureController>();
                            var tempCarriageCultureController = m_PlayerDic[tempScene.uid].GetComponent<CarriageCultureController>();

                            if (tempPlayerCultureController != null)
                            {
                                tempPlayerCultureController.TrackCamera = m_RootManager.TrackCamera;
                                //tempPlayerCultureController.IsRed = string.IsNullOrEmpty(tempScene.allianceName) || AllianceData.Instance.IsAllianceNotExist || (tempScene.allianceName != AllianceData.Instance.g_UnionInfo.name);
                                //tempPlayerCultureController.KingName = tempScene.senderName;
                                //tempPlayerCultureController.AllianceName = tempScene.allianceName;
                                //tempPlayerCultureController.RemainingBlood = tempScene.remainLife;
                                //tempPlayerCultureController.TotalBlood = tempScene.totalLife;
                                tempPlayerCultureController.SetThis();
                            }

                            if (tempCarriageCultureController != null)
                            {
                                tempCarriageCultureController.TrackCamera = m_RootManager.TrackCamera;
                                //tempCarriageCultureController.IsRed = string.IsNullOrEmpty(tempScene.allianceName) || AllianceData.Instance.IsAllianceNotExist || (tempScene.allianceName != AllianceData.Instance.g_UnionInfo.name);
                                //tempCarriageCultureController.KingName = tempScene.senderName;
                                //tempCarriageCultureController.AllianceName = tempScene.allianceName;
                                //tempCarriageCultureController.RemainingBlood = tempScene.remainLife;
                                //tempCarriageCultureController.TotalBlood = tempScene.totalLife;
                                tempCarriageCultureController.SetThis();
                            }

                            return true;
                        }
                    //other player moving.
                    case ProtoIndexes.Sprite_Move:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            SpriteMove tempMove = new SpriteMove();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMove, tempMove.GetType());

                            int uID = m_PlayerDic.Where(item => item.Value.m_UID == tempMove.uid).First().Key;

                            UpdatePlayerPosition(uID, new Vector3(tempMove.posX, tempMove.posY, tempMove.posZ));

                            return true;
                        }
                    //other player exit.
                    case ProtoIndexes.EXIT_CARRIAGE_SCENE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ExitScene tempScene = new ExitScene();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                            if (m_PlayerDic.ContainsKey(tempScene.uid))
                            {
                                DestroyPlayer(tempScene.uid);
                            }

                            return true;
                        }
                    //player dead.
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_DEAD:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerDeadNotify playerDeadNotify = new PlayerDeadNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, playerDeadNotify, playerDeadNotify.GetType());

                            if (m_PlayerDic.ContainsKey(playerDeadNotify.uid))
                            {
                                AddToDeadDic(playerDeadNotify.uid, new IDCollector()
                                {
                                    m_RoleID = m_PlayerDic[playerDeadNotify.uid].m_RoleID,
                                    m_UID = m_PlayerDic[playerDeadNotify.uid].m_UID,
                                    m_KingName = m_PlayerDic[playerDeadNotify.uid].GetComponent<CarriagePlayerCultureController>().KingName,
                                    m_AllianceName = m_PlayerDic[playerDeadNotify.uid].GetComponent<CarriagePlayerCultureController>().AllianceName,
                                    m_TotalBlood = m_PlayerDic[playerDeadNotify.uid].GetComponent<CarriagePlayerCultureController>().TotalBlood,
                                });
                                DestroyPlayer(playerDeadNotify.uid);
                            }

                            if (PlayerSceneSyncManager.Instance.m_MyselfUid == playerDeadNotify.uid)
                            {
                                AddToDeadDic(playerDeadNotify.uid, new IDCollector()
                                {
                                    m_RoleID = CityGlobalData.m_king_model_Id,
                                    m_KingName = m_RootManager.m_SelfPlayerCultureController.KingName,
                                    m_AllianceName = m_RootManager.m_SelfPlayerCultureController.AllianceName,
                                    m_TotalBlood = m_RootManager.m_SelfPlayerCultureController.TotalBlood,
                                });

                                Destroy(m_RootManager.m_SelfPlayerCultureController.gameObject);
                                m_RootManager.m_SelfPlayerCultureController = null;
                                m_RootManager.m_SelfPlayerCultureController = null;

                                //m_RootManager.m_CarriageUI.DeactiveSkills();
                                //m_RootManager.m_CarriageUI.m_ToAttackId = -1;

                                ////Show dead dimmer.
                                //m_RootManager.m_CarriageUI.ShowDimmer(true);
                            }

                            return true;
                        }
                    //player/other player rebirth.
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_REVIVE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerReviveNotify reviveNotify = new PlayerReviveNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, reviveNotify, reviveNotify.GetType());

                            if (m_DeadPlayerDic.ContainsKey(reviveNotify.uid))
                            {
                                if (PlayerSceneSyncManager.Instance.m_MyselfUid == reviveNotify.uid)
                                {
                                    m_RootManager.InitPlayer(m_DeadPlayerDic[reviveNotify.uid].m_RoleID, m_RootManager.originalPosition.localPosition, m_DeadPlayerDic[reviveNotify.uid].m_KingName, m_DeadPlayerDic[reviveNotify.uid].m_AllianceName, m_DeadPlayerDic[reviveNotify.uid].m_TotalBlood);

                                    ////Show dead dimmer.
                                    //m_RootManager.m_CarriageUI.ShowDimmer(false);
                                }
                                else
                                {
                                    CreatePlayer(m_DeadPlayerDic[reviveNotify.uid].m_RoleID, m_DeadPlayerDic[reviveNotify.uid].m_UID, m_RootManager.originalPosition.localPosition);

                                    var tempBasicController = m_PlayerDic[reviveNotify.uid].GetComponent<CarriagePlayerCultureController>();
                                    tempBasicController.TrackCamera = m_RootManager.TrackCamera;
                                    tempBasicController.IsRed = false;
                                    tempBasicController.KingName = m_DeadPlayerDic[reviveNotify.uid].m_KingName;
                                    tempBasicController.AllianceName = m_DeadPlayerDic[reviveNotify.uid].m_AllianceName;
                                    tempBasicController.TotalBlood = m_DeadPlayerDic[reviveNotify.uid].m_TotalBlood;
                                    tempBasicController.RemainingBlood = tempBasicController.TotalBlood;

                                    tempBasicController.SetThis();

                                    RemoveFromDeadDic(reviveNotify.uid);
                                }
                            }

                            return true;
                        }
                }
            }
            return false;
        }

        private void Awake()
        {
            MaleCharacterPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie");
            FemaleCharacterPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoLi");
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


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class AlliancePlayerManager : PlayerManager, SocketListener
    {
        public RootManager m_RootManager;

        public struct IDCollector
        {
            public long m_PlayerID;
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

        public override void AddTrackCamera(OtherPlayerController temp)
        {
            temp.TrackCamera = m_RootManager.TrackCamera;
        }

        private void AddToDeadDic(long id, IDCollector collector)
        {
            if (m_DeadPlayerDic.ContainsKey(id)) return;

            m_DeadPlayerDic.Add(id, collector);
        }

        private void RemoveFromDeadDic(long id)
        {
            if (m_DeadPlayerDic.ContainsKey(id))
            {
                m_DeadPlayerDic.Remove(id);
            }
        }

        /// <summary>
        /// Receive character sync message from server.
        /// </summary>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public bool OnSocketEvent(QXBuffer p_message)
        {
			if ( Application.loadedLevelName != SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_BATTLE ) ) return false;

            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    case ProtoIndexes.ENTER_FIGHT_SCENE_OK: //Other player enter scene
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            EnterFightScene tempScene = new EnterFightScene();
                            t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                            CreatePlayer(tempScene.junZhuId, tempScene.roleId, tempScene.uid, new Vector3(tempScene.posX, tempScene.posY, tempScene.posZ));

                            var tempBasicController = m_PlayerDic[tempScene.junZhuId].GetComponent<AllianceBasicPlayerController>();
                            tempBasicController.TrackCamera = m_RootManager.TrackCamera;
                            tempBasicController.IsRed = string.IsNullOrEmpty(tempScene.allianceName) || AllianceData.Instance.IsAllianceNotExist || (tempScene.allianceName != AllianceData.Instance.g_UnionInfo.name);
                            tempBasicController.KingName = tempScene.senderName;
                            tempBasicController.AllianceName = tempScene.allianceName;
                            tempBasicController.RemainingBlood = tempScene.remainLife;
                            tempBasicController.TotalBlood = tempScene.totalLife;
                            tempBasicController.SetThis();

                            return true;
                        }
                    case ProtoIndexes.Sprite_Move: //other player moving.
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            SpriteMove tempMove = new SpriteMove();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMove, tempMove.GetType());

                            long uID = m_PlayerDic.Where(item => item.Value.m_UID == tempMove.uid).First().Key;

                            UpdatePlayerPosition(uID, new Vector3(tempMove.posX, tempMove.posY, tempMove.posZ));

                            return true;
                        }
                    case ProtoIndexes.EXIT_FIGHT_SCENE: //other player exit.
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ExitFightScene tempScene = new ExitFightScene();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                            if (m_PlayerDic.ContainsKey(tempScene.junzhuId))
                            {
                                DestroyPlayer(tempScene.junzhuId);
                            }

                            return true;
                        }
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_DEAD: //player dead.
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerDeadNotify playerDeadNotify = new PlayerDeadNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, playerDeadNotify, playerDeadNotify.GetType());

                            if (m_PlayerDic.ContainsKey(playerDeadNotify.junzhuId))
                            {
                                AddToDeadDic(playerDeadNotify.junzhuId, new IDCollector()
                                {
                                    m_PlayerID = m_PlayerDic[playerDeadNotify.junzhuId].m_PlayerID,
                                    m_RoleID = m_PlayerDic[playerDeadNotify.junzhuId].m_RoleID,
                                    m_UID = m_PlayerDic[playerDeadNotify.junzhuId].m_UID,
                                    m_KingName = m_PlayerDic[playerDeadNotify.junzhuId].GetComponent<AllianceBasicPlayerController>().KingName,
                                    m_AllianceName = m_PlayerDic[playerDeadNotify.junzhuId].GetComponent<AllianceBasicPlayerController>().AllianceName,
                                    m_TotalBlood = m_PlayerDic[playerDeadNotify.junzhuId].GetComponent<AllianceBasicPlayerController>().TotalBlood,
                                });
                                DestroyPlayer(playerDeadNotify.junzhuId);
                            }

                            if (JunZhuData.Instance().m_junzhuInfo.id == playerDeadNotify.junzhuId)
                            {
                                AddToDeadDic(playerDeadNotify.junzhuId, new IDCollector()
                                {
                                    m_PlayerID = JunZhuData.Instance().m_junzhuInfo.id,
                                    m_RoleID = CityGlobalData.m_king_model_Id,
                                    m_KingName = m_RootManager.m_AllianceBasicPlayerController.KingName,
                                    m_AllianceName = m_RootManager.m_AllianceBasicPlayerController.AllianceName,
                                    m_TotalBlood = m_RootManager.m_AllianceBasicPlayerController.TotalBlood,
                                });

                                Destroy(m_RootManager.m_AlliancePlayerController.gameObject);
                                m_RootManager.m_AlliancePlayerController = null;
                                m_RootManager.m_AllianceBasicPlayerController = null;

                                m_RootManager.m_AllianceBattleUi.DeactiveAttack();
                                m_RootManager.m_AllianceBattleUi.m_ToAttackId = -1;

                                //Show dead dimmer.
                                m_RootManager.m_AllianceBattleUi.ShowDimmer(true);
                            }

                            return true;
                        }
                    case ProtoIndexes.ALLIANCE_FIGHT_PLAYER_REVIVE: //player rebirth.
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PlayerReviveNotify reviveNotify = new PlayerReviveNotify();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, reviveNotify, reviveNotify.GetType());

                            if (m_DeadPlayerDic.ContainsKey(reviveNotify.junzhuId))
                            {
                                if (JunZhuData.Instance().m_junzhuInfo.id == reviveNotify.junzhuId)
                                {
                                    m_RootManager.InitPlayer(m_DeadPlayerDic[reviveNotify.junzhuId].m_RoleID, m_RootManager.originalPosition1, m_DeadPlayerDic[reviveNotify.junzhuId].m_KingName, m_DeadPlayerDic[reviveNotify.junzhuId].m_AllianceName, m_DeadPlayerDic[reviveNotify.junzhuId].m_TotalBlood);

                                    //Show dead dimmer.
                                    m_RootManager.m_AllianceBattleUi.ShowDimmer(false);
                                }
                                else
                                {
                                    CreatePlayer(reviveNotify.junzhuId, m_DeadPlayerDic[reviveNotify.junzhuId].m_RoleID, m_DeadPlayerDic[reviveNotify.junzhuId].m_UID, m_RootManager.originalPosition1);

                                    var tempBasicController = m_PlayerDic[reviveNotify.junzhuId].GetComponent<AllianceBasicPlayerController>();
                                    tempBasicController.TrackCamera = m_RootManager.TrackCamera;
                                    tempBasicController.IsRed = false;
                                    tempBasicController.KingName = m_DeadPlayerDic[reviveNotify.junzhuId].m_KingName;
                                    tempBasicController.AllianceName = m_DeadPlayerDic[reviveNotify.junzhuId].m_AllianceName;
                                    tempBasicController.TotalBlood = m_DeadPlayerDic[reviveNotify.junzhuId].m_TotalBlood;
                                    tempBasicController.RemainingBlood = tempBasicController.TotalBlood;

                                    tempBasicController.SetThis();

                                    RemoveFromDeadDic(reviveNotify.junzhuId);
                                }
                            }

                            return true;
                        }
                    default:
                        break;
                }
            }
            return false;
        }

        void Awake()
        {
            MaleCharacterPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/AllianceBattleHaoJie");
            FemaleCharacterPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/AllianceBattleLuoLi");
            BasicYPosition = 1.0f;
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

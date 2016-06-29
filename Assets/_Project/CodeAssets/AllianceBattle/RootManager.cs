using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class RootManager : Singleton<RootManager>
    {
        /// <summary>
        /// 1 for attacker, 2 for protecter.
        /// </summary>
        public int MyPart = -1;

        public bool IsAttacker(string allianceName)
        {
            return (MyPart == 1 && allianceName == AllianceData.Instance.g_UnionInfo.name) || (MyPart == 2 && allianceName != AllianceData.Instance.g_UnionInfo.name);
        }

        public AllianceBattleMain m_AllianceBattleMain;
        public ABPlayerSyncManager m_AbPlayerSyncManager;
        public ABMsgManager m_AllianceBattleMsgManager;
        public ABHoldPointManager m_AbHoldPointManager;
        public RTMultiCameraOptimizer m_Top1Hierarchy;

        public GlobalBloodLabelController m_GlobalBloodLabelController;
        public AnimationHierarchyPlayer m_AnimationHierarchyPlayer = new AnimationHierarchyPlayer();

        public Camera TrackCamera;
        public List<Camera> NGUICameraList = new List<Camera>();
        [HideInInspector]
        public Vector3 TrackCameraPosition;
        [HideInInspector]
        public Vector3 TrackCameraRotation;

        public static float BasicYPosition = 22f;

        #region Self Player and Player Sync

        public static GameObject GetPlayerObjectByUID(int p_uID)
        {
            if (PlayerSceneSyncManager.Instance.m_MyselfUid == p_uID)
            {
                if (RootManager.Instance.m_SelfPlayerController != null)
                {
                    return RootManager.Instance.m_SelfPlayerController.gameObject;
                }
                else
                {
                    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
                    {
                        Debug.LogWarning("Cannot get self player.");
                    }
                    return null;
                }
            }
            else
            {
                if (RootManager.Instance.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
                {
                    return RootManager.Instance.m_AbPlayerSyncManager.m_PlayerDic[p_uID].gameObject;
                }
                else
                {
                    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
                    {
                        Debug.LogWarning("Cannot get other player, " + p_uID);
                    }

                    return null;
                }
            }
        }

        public Transform m_AllianceBattlePlayerTransform;

        public GameObject PlayerParentObject;

        public ABPlayerController m_SelfPlayerController
        {
            get { return m_selfPlayerController; }
            set
            {
                m_selfPlayerController = value;
                m_AnimationHierarchyPlayer.m_SinglePlayerController = m_SelfPlayerController;
                m_AllianceBattleMain.m_RTSkillExecuter.m_SelfPlayerController = m_SelfPlayerController;
            }
        }
        private ABPlayerController m_selfPlayerController;

        public ABPlayerCultureController m_SelfPlayerCultureController
        {
            get { return m_selfPlayerCultureController; }
            set
            {
                m_selfPlayerCultureController = value;
                m_AllianceBattleMain.m_RTSkillExecuter.m_SelfPlayerCultureController = m_selfPlayerCultureController;
            }
        }
        private ABPlayerCultureController m_selfPlayerCultureController;

        public void CreateSinglePlayer(int p_roleID, long p_junzhuID, Vector3 p_position, string p_kingName, string p_allianceName, int p_vipLevel, int p_TitleIndex, int p_alliancePost, int p_nation, int p_level, int p_battleValue, float p_remainBlood = -1, float p_totalBlood = -1)
        {
            var tempObject = Instantiate(m_AbPlayerSyncManager.PlayerPrefabList[p_roleID - 1]) as GameObject;
            TransformHelper.ActiveWithStandardize(PlayerParentObject.transform, tempObject.transform);
            tempObject.transform.localPosition = LimitPlayerPositionByHoldPoint(p_position);

            m_SelfPlayerController = tempObject.GetComponent<ABPlayerController>() ?? tempObject.AddComponent<ABPlayerController>();
            m_SelfPlayerCultureController = tempObject.GetComponent<ABPlayerCultureController>();
            tempObject.AddComponent<AutoHideObjects>();
            m_AllianceBattleMain.m_MapController.AddGizmos(PlayerSceneSyncManager.Instance.m_MyselfUid, 0, m_SelfPlayerController.transform.localPosition, m_SelfPlayerController.transform.localEulerAngles.y);

            //Set AllianceBattlePlayerController.
            m_SelfPlayerController.IsRotateCamera = false;
            m_SelfPlayerController.IsUploadPlayerPosition = true;
            m_SelfPlayerController.BaseGroundPosY = BasicYPosition;

            m_SelfPlayerController.TrackCameraPosition = TrackCameraPosition;
            m_SelfPlayerController.TrackCameraRotation = TrackCameraRotation;

            m_SelfPlayerController.m_Joystick = m_AllianceBattleMain.m_Joystick;
            m_SelfPlayerController.TrackCamera = TrackCamera;

            m_SelfPlayerController.m_StartNavDelegate += m_AllianceBattleMain.ShowNavigationAnim;
            m_SelfPlayerController.m_EndNavDelegate += m_AllianceBattleMain.StopNavigationAnim;

            //Set AllianceBattlePlayerCultureController.
            m_SelfPlayerCultureController.TrackCamera = TrackCamera;

            m_SelfPlayerCultureController.KingName = p_kingName;
            m_SelfPlayerCultureController.AllianceName = p_allianceName;
            m_SelfPlayerCultureController.Vip = p_vipLevel;
            m_SelfPlayerCultureController.Title = p_TitleIndex;
            m_SelfPlayerCultureController.AlliancePost = p_alliancePost;
            m_SelfPlayerCultureController.NationID = p_nation;
            m_SelfPlayerCultureController.Level = p_level;
            m_SelfPlayerCultureController.BattleValue = p_battleValue;
            m_SelfPlayerCultureController.RoleID = p_roleID;
            m_SelfPlayerCultureController.JunzhuID = p_junzhuID;
            m_SelfPlayerCultureController.UID = PlayerSceneSyncManager.Instance.m_MyselfUid;
            if (p_totalBlood > 0)
            {
                m_SelfPlayerCultureController.TotalBlood = p_totalBlood;
                m_SelfPlayerCultureController.RemainingBlood = p_remainBlood;
            }

            m_SelfPlayerCultureController.SetThis();

            //Set HeadIcon.
            m_AllianceBattleMain.SelfIconSetter.SetPlayer(p_roleID, true, p_level, p_kingName, p_allianceName, p_totalBlood, p_remainBlood, p_nation, p_vipLevel, p_battleValue, 0);
            m_AllianceBattleMain.SelfIconSetter.gameObject.SetActive(true);
        }

        #endregion

        public Vector3 LimitPlayerPositionByHoldPoint(Vector3 original)
        {
            if (m_AbHoldPointManager.HoldPointDic != null && m_AbHoldPointManager.HoldPointDic.Any())
            {
                var temp = m_AbHoldPointManager.HoldPointDic.Where(item => Vector3.Distance(new Vector3(item.Value.Position.x, 0, item.Value.Position.z), new Vector3(original.x, 0, original.z)) < 2f).ToList();

                if (temp != null && temp.Any())
                {
                    var distance = Vector3.Distance(new Vector3(temp.First().Value.Position.x, 0, temp.First().Value.Position.z), new Vector3(original.x, 0, original.z));

                    if (distance > 0.01f)
                    {
                        return new Vector3(temp.First().Value.Position.x, 0, temp.First().Value.Position.z) + new Vector3((original.x - temp.First().Value.Position.x) / distance * 2, original.y, (original.z - temp.First().Value.Position.z) / distance * 2);
                    }
                    else
                    {
                        return new Vector3(temp.First().Value.Position.x, 0, temp.First().Value.Position.z) + new Vector3(0, original.y, -2f);
                    }
                }
            }

            return original;
        }

        public void UpdateSelfPlayerAlliance(string p_allianceName, int p_alliancePost)
        {
            m_SelfPlayerCultureController.AllianceName = p_allianceName;
            m_SelfPlayerCultureController.AlliancePost = p_alliancePost;
            m_SelfPlayerCultureController.SetThis();

            m_AllianceBattleMain.SelfIconSetter.AllianceLabel.text = (string.IsNullOrEmpty(p_allianceName) || p_allianceName == "***") ? "无联盟" : p_allianceName;
            m_AllianceBattleMain.SelfIconSetter.gameObject.SetActive(true);
        }

        public void UpdateBagItemNum()
        {
            var bagList = BagData.Instance().m_bagItemList;

            var temp = bagList.Where(item => item.itemId == 910011).ToList();
            m_AllianceBattleMain.SetRemainingSummonNum(temp.Any() ? temp.First().cnt : 0);

            temp = bagList.Where(item => item.itemId == 910012).ToList();
            m_AllianceBattleMain.SetRemainingBloodNum(temp.Any() ? temp.First().cnt : 0);
        }

        private List<int> m_effectIDList = new List<int>();
        private List<GameObject> m_effectList = new List<GameObject>();

        private void PreLoadBattleEffect()
        {
            m_effectIDList = RTActionTemplate.templates.Where(item => item.CsOnHit > 0).Select(item => item.CsOnHit).Concat(RTSkillTemplate.templates.Where(item => item.EsOnShot > 0).Select(item => item.EsOnShot)).Concat(RTBuffTemplate.templates.Where(item => item.BuffDisplay > 0).Select(item => item.BuffDisplay)).ToList();

            m_effectIDList.ForEach(item =>
            {
                Global.ResourcesDotLoad(EffectIdTemplate.getEffectTemplateByEffectId(item).path, OnBattleEffectLoaded);
            });

            Global.ResourcesDotLoad(EffectIdTemplate.getEffectTemplateByEffectId(RTSkillTemplate.GetTemplateByID(171).EsOnTrack).path, OnLongSkillEffectLoaded);
        }

        private void OnBattleEffectLoaded(ref WWW www, string path, Object prefab)
        {
            //Store loaded to optimize performance.
            m_effectList.Add(prefab as GameObject);

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.EFFECT, "AB_EFFECT_" + path);
        }

        private void OnLongSkillEffectLoaded(ref WWW www, string path, Object prefab)
        {
            //Store loaded to optimize performance.
            m_AllianceBattleMain.m_LongSkillEffectPrefab = prefab as GameObject;
        }

        void Start()
        {
            var offsetPosition = TrackCamera.transform.localPosition - m_AllianceBattlePlayerTransform.transform.localPosition;
            TrackCameraPosition = new Vector3(offsetPosition.x, TrackCamera.transform.localPosition.y, offsetPosition.z);
            TrackCameraRotation = TrackCamera.transform.localEulerAngles - m_AllianceBattlePlayerTransform.transform.localEulerAngles;

            UpdateBagItemNum();

            //Play AllianceBattle music.
            ClientMain.m_sound_manager.chagneBGSound(100201);

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();

            ////[FIX]Add guide here.
            //if (FreshGuide.Instance().IsActive(100370) && TaskData.Instance.m_TaskInfoDic[100370].progress >= 0)
            //{
            //    UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[100370].m_listYindaoShuju[2]);
            //}

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_Root");

            StartCoroutine(SetMultiTouchCoroutine());
        }

        IEnumerator SetMultiTouchCoroutine()
        {
            if (UICamera.eventHandler.GetComponentInParent<EnterNextScene>() != null)
            {
                yield return new WaitForEndOfFrame();
            }

            UtilityTool.SetMultiTouch(true);
        }

        void Awake()
        {
            PreLoadBattleEffect();

            //Set variables.
            m_AnimationHierarchyPlayer.m_PlayerManager = m_AbPlayerSyncManager;
            m_AnimationHierarchyPlayer.m_SinglePlayerController = m_SelfPlayerController;
            m_AnimationHierarchyPlayer.m_PiorityDic = new Dictionary<int, int>()
            {
                {Animator.StringToHash("Dead"), 0}, {Animator.StringToHash("ShuangdaoSkill1"), 1}, {Animator.StringToHash("ShuangdaoSkill2"), 1}, {Animator.StringToHash("ShuangdaoSkill3"), 1}, {Animator.StringToHash("ShuangdaoSkill4"), 1}, {Animator.StringToHash("EndDadaoSkill"), 2}, {Animator.StringToHash("DadaoSkill"), 3}, {Animator.StringToHash("StartDadaoSkill"), 4}, {Animator.StringToHash("GongjianSkill"), 5}, {Animator.StringToHash("attack_1"), 6}, {Animator.StringToHash("skill_mibao"), 7}, {Animator.StringToHash("Run"), 8}, {Animator.StringToHash("BATC"), 9}, {Animator.StringToHash("Stand"), 10}
            };

            //Initialize messages.
            PlayerState temp = new PlayerState
            {
                s_state = State.State_LEAGUEOFCITY
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);

            if (!AllianceData.Instance.IsAllianceNotExist && AllianceData.Instance.g_UnionInfo.identity > 0)
            {
                m_AllianceBattleMain.CommandBTN.SetActive(true);
                SocketHelper.SendQXMessage(ProtoIndexes.LMZ_CMD_LIST);
            }
            else
            {
                m_AllianceBattleMain.CommandBTN.SetActive(false);
            }
        }

        new void OnDestroy()
        {
            UtilityTool.SetMultiTouch(false);

            base.OnDestroy();
        }

        void OnGUI()
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_UNIT_TEST))
            {
                if (GUILayout.Button("Test long damage num"))
                {
                    m_SelfPlayerCultureController.OnDamage(-647658086, 1, true);
                }
                if (GUILayout.Button("Test skip skill"))
                {
                    if (m_AllianceBattleMain.m_TargetId < 0) return;

                    var temp = m_AbPlayerSyncManager.m_PlayerDic[m_AllianceBattleMain.m_TargetId].transform;

                    SpriteMove tempInfo = new SpriteMove()
                    {
                        uid = PlayerSceneSyncManager.Instance.m_MyselfUid,
                        posX = temp.localPosition.x,
                        posY = temp.localPosition.y,
                        posZ = temp.localPosition.z,
                        dir = temp.localEulerAngles.y
                    };
                    MemoryStream tempStream = new MemoryStream();
                    QiXiongSerializer tempSer = new QiXiongSerializer();
                    tempSer.Serialize(tempStream, tempInfo);
                    byte[] t_protof;
                    t_protof = tempStream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.POS_JUMP, ref t_protof);
                }
            }
        }
    }
}

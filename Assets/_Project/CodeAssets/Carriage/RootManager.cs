using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace Carriage
{
    public class RootManager : Singleton<RootManager>
    {
        public CarriageMain m_CarriageMain;
        public CarriageItemSyncManager m_CarriageItemSyncManager;
        public CarriageMsgManager m_CarriageMsgManager;
        public CarriageSafeArea m_CarriageSafeArea;
        public BiaoJuPage m_BiaoJuPage;

        public CarriageBloodLabelController m_CarriageBloodLabelController;

        public List<TongzhiData> m_CarriageTongzhiDataList
        {
            get { return Global.m_listJiebiaoData; }
        }

        public Camera TrackCamera;
        public List<Camera> NGUICameraList = new List<Camera>();
        [HideInInspector]
        public Vector3 TrackCameraPosition;
        [HideInInspector]
        public Vector3 TrackCameraRotation;

        public Transform originalPosition;

        public GameObject PlayerParentObject
        {
            get
            {
                int i = 0;

                while (i < playerParentObjectList.Count && playerParentObjectList[i].transform.childCount >= PlayerParentChildLimit)
                {
                    i++;
                }

                if (i >= playerParentObjectList.Count)
                {
                    var temp = Instantiate(PlayerParentObjectPrefab);
                    TransformHelper.ActiveWithStandardize(null, temp.transform);
                    playerParentObjectList.Add(temp);
                }

                return playerParentObjectList[i];
            }
        }

        private const int PlayerParentChildLimit = 100;
        public GameObject PlayerParentObjectPrefab;
        private List<GameObject> playerParentObjectList = new List<GameObject>();


        public List<GameObject> PlayerPrefabList = new List<GameObject>();
        public GameObject LinePointParent;
        public GameObject LinePointPrefab;

        public CarriagePlayerController m_SelfPlayerController;
        public CarriagePlayerCultureController m_SelfPlayerCultureController;

        public static float BasicYPosition = -3.24f;

        public void CreateSelfPlayer(int p_roleID, long p_junzhuID, Vector3 p_position, string p_kingName, string p_allianceName, int p_vipLevel, int p_TitleIndex, int p_alliancePost, int p_nation, int p_level, int p_battleValue, float p_remainBlood = -1, float p_totalBlood = -1)
        {
            var tempObject = Instantiate(PlayerPrefabList[p_roleID - 1]) as GameObject;
            TransformHelper.ActiveWithStandardize(PlayerParentObject.transform, tempObject.transform);
            tempObject.transform.localPosition = p_position;

            m_SelfPlayerController = tempObject.GetComponent<CarriagePlayerController>() ?? tempObject.AddComponent<CarriagePlayerController>();
            m_SelfPlayerCultureController = tempObject.GetComponent<CarriagePlayerCultureController>();
            m_CarriageMain.m_MapController.AddGizmos(PlayerSceneSyncManager.Instance.m_MyselfUid, 0, m_SelfPlayerController.transform.localPosition, m_SelfPlayerController.transform.localEulerAngles.y);

            //Set CarriagePlayerController.
            m_SelfPlayerController.IsRotateCamera = false;
            m_SelfPlayerController.IsUploadPlayerPosition = true;
            m_SelfPlayerController.BaseGroundPosY = 0.1f;

            m_SelfPlayerController.TrackCameraPosition = TrackCameraPosition;
            m_SelfPlayerController.TrackCameraRotation = TrackCameraRotation;

            m_SelfPlayerController.m_Joystick = m_CarriageMain.m_Joystick;
            m_SelfPlayerController.TrackCamera = TrackCamera;

            m_SelfPlayerController.m_StartNavDelegate += m_CarriageMain.ShowNavigationAnim;
            m_SelfPlayerController.m_EndNavDelegate += m_CarriageMain.StopNavigationAnim;

            //Set CarriagePlayerCultureController.
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
            m_CarriageMain.SelfIconSetter.SetPlayer(p_roleID, true, p_level, p_kingName, p_allianceName, p_totalBlood, p_remainBlood, p_nation, p_vipLevel, p_battleValue);
            m_CarriageMain.SelfIconSetter.gameObject.SetActive(true);
        }

        public void UpdateSelfPlayerAlliance(string p_allianceName, int p_alliancePost)
        {
            m_SelfPlayerCultureController.AllianceName = p_allianceName;
            m_SelfPlayerCultureController.AlliancePost = p_alliancePost;
            m_SelfPlayerCultureController.SetThis();

            m_CarriageMain.SelfIconSetter.AllianceLabel.text = (string.IsNullOrEmpty(p_allianceName) || p_allianceName == "***") ? "无联盟" : p_allianceName;
            m_CarriageMain.SelfIconSetter.gameObject.SetActive(true);
        }

        #region Animation Hierarchy

        private readonly List<int> m_animationHashListInPiority = new List<int>()
        {
            Animator.StringToHash("Dead"), Animator.StringToHash("attack_4"),Animator.StringToHash("attack_1"),Animator.StringToHash("skill_mibao"), Animator.StringToHash("Run"), Animator.StringToHash("BATC"), Animator.StringToHash("Stand")
        };

        public bool TryPlayAnimationInAnimator(int p_uid, string animationName)
        {
            Animator l_animator;

            if (p_uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                if (m_SelfPlayerController == null && m_SelfPlayerCultureController == null)
                {
                    Debug.LogWarning("Cancel play animation: " + animationName + " in self cause self player not exist");
                    return false;
                }
                l_animator = m_SelfPlayerController.GetComponent<Animator>();
            }
            else
            {
                if (!m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(p_uid))
                {
                    Debug.LogWarning("Cancel play animation: " + animationName + " in other cause other player not exist");
                    return false;
                }
                l_animator = m_CarriageItemSyncManager.m_PlayerDic[p_uid].GetComponent<Animator>();
            }

            if (m_animationHashListInPiority.IndexOf(Animator.StringToHash(animationName)) < 0 || m_animationHashListInPiority.IndexOf(AnimationHelper.GetAnimatorPlayingHash(l_animator)) < 0)
            {
                Debug.LogWarning("Cancel play animation: " + animationName + " in: " + p_uid + " cause animation/ current playing animation not exist in hierarchy.");
                return false;
            }

            if (m_animationHashListInPiority.IndexOf(Animator.StringToHash(animationName)) > m_animationHashListInPiority.IndexOf(AnimationHelper.GetAnimatorPlayingHash(l_animator)))
            {
                Debug.LogWarning("Cancel play animation: " + animationName + " in: " + p_uid + " cause animation hierarchy block, " + m_animationHashListInPiority.IndexOf(Animator.StringToHash(animationName)) + ">" + m_animationHashListInPiority.IndexOf(AnimationHelper.GetAnimatorPlayingHash(l_animator)));
                return false;
            }

            if (p_uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                Debug.LogWarning("Play animation: " + animationName + " in self.");
            }
            l_animator.Play(animationName);

            return true;
        }

        #endregion

        private void BiaoJuLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            var biaojuObject = (GameObject)Instantiate(p_object);
            NGUICameraList.Add(biaojuObject.GetComponentsInChildren<Camera>(true).First());
            biaojuObject.name = "BiaoJu";
            m_BiaoJuPage = biaojuObject.GetComponent<BiaoJuPage>();
            biaojuObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                //banner click anyway.
                m_CarriageMain.m_BannerEffectController.OnAlertEffectClick();

                //carriage click trigger.
                var trackCameraRay = TrackCamera.ScreenPointToRay(Input.mousePosition);
                var trackCameraHits = Physics.RaycastAll(trackCameraRay, Mathf.Infinity);

                var nguiCameraRayList = NGUICameraList.Select(item => item.ScreenPointToRay(Input.mousePosition)).ToList();
                var nguiCameraHits = nguiCameraRayList.Select(item => Physics.RaycastAll(item, Mathf.Infinity).ToList()).Aggregate((i, j) => i.Concat(j).ToList());

                RaycastHit shieldHit = nguiCameraHits.Where(item => (item.collider.transform.gameObject.layer == LayerMask.NameToLayer("NGUI")) || (item.collider.transform.tag == "CarriageItemShield")).FirstOrDefault();
                RaycastHit tempHit = trackCameraHits.Where(item => item.collider.transform.tag == "CarriageItemTrigger").FirstOrDefault();
                if (shieldHit.collider != null && shieldHit.collider.transform != null)
                {
                    return;
                }
                if (tempHit.collider != null && tempHit.collider.transform != null)
                {
                    tempHit.collider.transform.gameObject.SendMessage("OnCarriageItemClick");
                }
            }
        }

        void Start()
        {
            TrackCameraPosition = TrackCamera.transform.localPosition;
            TrackCameraRotation = TrackCamera.transform.localEulerAngles;

            CreateSelfPlayer(CityGlobalData.m_king_model_Id, JunZhuData.Instance().m_junzhuInfo.id, new Vector3(PlayerSceneSyncManager.Instance.m_MyselfPosition.z, BasicYPosition, PlayerSceneSyncManager.Instance.m_MyselfPosition.z), JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name, JunZhuData.Instance().m_junzhuInfo.vipLv, JunZhuData.m_iChenghaoID, AllianceData.Instance.IsAllianceNotExist ? -1 : AllianceData.Instance.g_UnionInfo.identity, JunZhuData.Instance().m_junzhuInfo.guoJiaId, JunZhuData.Instance().m_junzhuInfo.level, JunZhuData.Instance().m_junzhuInfo.zhanLi);

            //Create line point.
            CartRouteTemplate.Templates.ForEach(item =>
            {
                var linePoint = Instantiate(LinePointPrefab);
                TransformHelper.ActiveWithStandardize(LinePointParent.transform, linePoint.transform);
                linePoint.transform.localPosition = new Vector3(item.Position.Last().x, BasicYPosition, item.Position.Last().y);
            });

            //Play carriage music.
            BattleConfigTemplate temp = BattleConfigTemplate.getBattleConfigTemplateByConfigId(600);
            if (temp != null && temp.soundID != 0)
            {
                ClientMain.m_sound_manager.chagneBGSound(temp.soundID);
            }

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();

            //Add guide here.
            if (FreshGuide.Instance().IsActive(100370) && TaskData.Instance.m_TaskInfoDic[100370].progress >= 0)
            {
                UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[100370].m_listYindaoShuju[2]);
            }
        }

        void Awake()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YUNBIAO_MAIN_PAGE),
                        BiaoJuLoadCallBack);

            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQinglan"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQiangwei"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoli"));
            LinePointPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLinePoint");

            PlayerState temp = new PlayerState
            {
                s_state = State.State_YABIAO
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);

            SocketHelper.SendQXMessage(ProtoIndexes.C_GETMABIANTYPE_REQ);

            //Request start carriage times.
            YaBiaoMoreInfoReq temp2 = new YaBiaoMoreInfoReq
            {
                type = 1
            };
            SocketHelper.SendQXMessage(temp2, ProtoIndexes.C_YABIAO_MOREINFO_RSQ);

            // Request I help others list.
            AnswerYaBiaoHelpReq temp3 = new AnswerYaBiaoHelpReq
            {
                ybUid = PlayerSceneSyncManager.Instance.m_MyselfUid
            };
            SocketHelper.SendQXMessage(temp3, ProtoIndexes.C_CHECK_YABIAOHELP_RSQ);

            //Request rob carriage times.
            YaBiaoMoreInfoReq temp4 = new YaBiaoMoreInfoReq
            {
                type = 2
            };
            SocketHelper.SendQXMessage(temp4, ProtoIndexes.C_YABIAO_MOREINFO_RSQ);

            //Request additional start carriage times.
            YaBiaoMoreInfoReq temp5 = new YaBiaoMoreInfoReq
            {
                type = 3
            };
            SocketHelper.SendQXMessage(temp5, ProtoIndexes.C_YABIAO_MOREINFO_RSQ);

            //Request alliance tech info.
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKJ_INFO);
        }

        new void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

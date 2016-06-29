using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllianceBattle;
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

        public GlobalBloodLabelController m_GlobalBloodLabelController;
        public AnimationHierarchyPlayer m_AnimationHierarchyPlayer = new AnimationHierarchyPlayer();

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

        public Transform m_CarriagePlayerTransform;

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

        public CarriagePlayerController m_SelfPlayerController
        {
            get { return m_selfPlayerController; }
            set
            {
                m_selfPlayerController = value;
                m_AnimationHierarchyPlayer.m_SinglePlayerController = m_SelfPlayerController;
                m_CarriageMain.m_RTSkillExecuter.m_SelfPlayerController = m_SelfPlayerController;
            }
        }
        private CarriagePlayerController m_selfPlayerController;

        public CarriagePlayerCultureController m_SelfPlayerCultureController
        {
            get { return m_selfPlayerCultureController; }
            set
            {
                m_selfPlayerCultureController = value;
                m_CarriageMain.m_RTSkillExecuter.m_SelfPlayerCultureController = m_selfPlayerCultureController;
            }
        }
        private CarriagePlayerCultureController m_selfPlayerCultureController;

        public static float BasicYPosition = 20.5f;

        public void CreateSelfPlayer(int p_roleID, long p_junzhuID, Vector3 p_position, string p_kingName, string p_allianceName, int p_vipLevel, int p_TitleIndex, int p_alliancePost, int p_nation, int p_level, int p_battleValue, float p_remainBlood = -1, float p_totalBlood = -1)
        {
            var tempObject = Instantiate(PlayerPrefabList[p_roleID - 1]) as GameObject;
            TransformHelper.ActiveWithStandardize(PlayerParentObject.transform, tempObject.transform);
            tempObject.transform.localPosition = p_position;

            m_SelfPlayerController = tempObject.GetComponent<CarriagePlayerController>() ?? tempObject.AddComponent<CarriagePlayerController>();
            m_SelfPlayerCultureController = tempObject.GetComponent<CarriagePlayerCultureController>();
            tempObject.AddComponent<AutoHideObjects>();
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
            m_CarriageMain.SelfIconSetter.SetPlayer(p_roleID, true, p_level, p_kingName, p_allianceName, p_totalBlood, p_remainBlood, p_nation, p_vipLevel, p_battleValue, 0);
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
            //Abandon this cause using ngui camera would be nicer.

            //#if UNITY_EDITOR||UNITY_STANDALONE
            //            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            //            {
            //                //banner click anyway expect help window opened.
            //                if (!m_CarriageMain.HelpWindowObject.activeInHierarchy)
            //                {
            //                    m_CarriageMain.m_BannerEffectController.OnAlertEffectClick();
            //                }

            //                //carriage click trigger.
            //                var trackCameraRay = TrackCamera.ScreenPointToRay(Input.mousePosition);
            //                var trackCameraHits = Physics.RaycastAll(trackCameraRay, Mathf.Infinity);

            //                var nguiCameraRayList = NGUICameraList.Select(item => item.ScreenPointToRay(Input.mousePosition)).ToList();
            //                var nguiCameraHits = nguiCameraRayList.Select(item => Physics.RaycastAll(item, Mathf.Infinity).ToList()).Aggregate((i, j) => i.Concat(j).ToList());

            //                RaycastHit shieldHit = nguiCameraHits.Where(item => (item.collider.transform.gameObject.layer == LayerMask.NameToLayer("NGUI")) || (item.collider.transform.tag == "CarriageItemShield")).FirstOrDefault();
            //                RaycastHit tempHit = trackCameraHits.Where(item => item.collider.transform.tag == "CarriageItemTrigger").FirstOrDefault();
            //                if (shieldHit.collider != null && shieldHit.collider.transform != null)
            //                {
            //                    return;
            //                }
            //                if (tempHit.collider != null && tempHit.collider.transform != null)
            //                {
            //                    tempHit.collider.transform.gameObject.SendMessage("OnCarriageItemClick");
            //                }
            //            }
            //#else
            //            if (Input.touchCount > 0)
            //            {
            //                //banner click anyway expect help window opened.
            //                if (!m_CarriageMain.HelpWindowObject.activeInHierarchy)
            //                {
            //                    m_CarriageMain.m_BannerEffectController.OnAlertEffectClick();
            //                }

            //                foreach (var touch in Input.touches)
            //                {
            //                    //carriage click trigger.
            //                    var trackCameraRay = TrackCamera.ScreenPointToRay(touch.position);
            //                    var trackCameraHits = Physics.RaycastAll(trackCameraRay, Mathf.Infinity);

            //                    var nguiCameraRayList = NGUICameraList.Select(item => item.ScreenPointToRay(Input.mousePosition)).ToList();
            //                    var nguiCameraHits = nguiCameraRayList.Select(item => Physics.RaycastAll(item, Mathf.Infinity).ToList()).Aggregate((i, j) => i.Concat(j).ToList());

            //                    RaycastHit shieldHit = nguiCameraHits.Where(item => (item.collider.transform.gameObject.layer == LayerMask.NameToLayer("NGUI")) || (item.collider.transform.tag == "CarriageItemShield")).FirstOrDefault();
            //                    RaycastHit tempHit = trackCameraHits.Where(item => item.collider.transform.tag == "CarriageItemTrigger").FirstOrDefault();
            //                    if (shieldHit.collider != null && shieldHit.collider.transform != null)
            //                    {
            //                        return;
            //                    }
            //                    if (tempHit.collider != null && tempHit.collider.transform != null)
            //                    {
            //                        tempHit.collider.transform.gameObject.SendMessage("OnCarriageItemClick");
            //                    }
            //                }
            //            }
            //#endif
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

            //Global.ResourcesDotLoad(EffectIdTemplate.getEffectTemplateByEffectId(RTSkillTemplate.GetTemplateByID(171).EsOnTrack).path, OnLongSkillEffectLoaded);
        }

        private void OnBattleEffectLoaded(ref WWW www, string path, Object prefab)
        {
            //Store loaded to optimize performance.
            m_effectList.Add(prefab as GameObject);

            PrepareForCarriage.UpdateLoadProgress(PrepareForCarriage.LoadModule.EFFECT, "Carriage_EFFECT_" + path);
        }

        //private void OnLongSkillEffectLoaded(ref WWW www, string path, Object prefab)
        //{
        //    //Store loaded to optimize performance.
        //    m_AllianceBattleMain.m_LongSkillEffectPrefab = prefab as GameObject;
        //}

        void Start()
        {
            var offsetPosition = TrackCamera.transform.localPosition - m_CarriagePlayerTransform.transform.localPosition;
            TrackCameraPosition = new Vector3(offsetPosition.x, TrackCamera.transform.localPosition.y, offsetPosition.z);
            TrackCameraRotation = TrackCamera.transform.localEulerAngles - m_CarriagePlayerTransform.transform.localEulerAngles;

            Vector2 limitedPosition = m_CarriageMain.LimitPlayerPositionByCarriageNPC(new Vector2(PlayerSceneSyncManager.Instance.m_MyselfPosition.x, PlayerSceneSyncManager.Instance.m_MyselfPosition.z));
            CreateSelfPlayer(CityGlobalData.m_king_model_Id, JunZhuData.Instance().m_junzhuInfo.id, new Vector3(limitedPosition.x, BasicYPosition, limitedPosition.y), JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name, JunZhuData.Instance().m_junzhuInfo.vipLv, JunZhuData.m_iChenghaoID, AllianceData.Instance.IsAllianceNotExist ? -1 : AllianceData.Instance.g_UnionInfo.identity, JunZhuData.Instance().m_junzhuInfo.guoJiaId, JunZhuData.Instance().m_junzhuInfo.level, JunZhuData.Instance().m_junzhuInfo.zhanLi);

            //Create line point.
            CartRouteTemplate.Templates.ForEach(item =>
            {
                var linePoint = Instantiate(LinePointPrefab);
                TransformHelper.ActiveWithStandardize(LinePointParent.transform, linePoint.transform);
                linePoint.transform.localPosition = new Vector3(item.Position.Last().x, BasicYPosition, item.Position.Last().y);
            });

            //Play carriage music.
            ClientMain.m_sound_manager.chagneBGSound(100201);

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();

            //Add guide here.
            if (FreshGuide.Instance().IsActive(100370) && TaskData.Instance.m_TaskInfoDic[100370].progress >= 0)
            {
                UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[100370].m_listYindaoShuju[2]);
            }

            PrepareForCarriage.UpdateLoadProgress(PrepareForCarriage.LoadModule.INIT, "Carriage_Root");

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

            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YUNBIAO_MAIN_PAGE),
                        BiaoJuLoadCallBack);

            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQinglan"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQiangwei"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoli"));
            LinePointPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLinePoint");

            //Set variables.
            m_AnimationHierarchyPlayer.m_PlayerManager = m_CarriageItemSyncManager;
            m_AnimationHierarchyPlayer.m_SinglePlayerController = m_SelfPlayerController;
            m_AnimationHierarchyPlayer.m_PiorityDic = new Dictionary<int, int>()
                {
                    {Animator.StringToHash("Dead"), 0}, {Animator.StringToHash("attack_4"), 1}, {Animator.StringToHash("attack_1"), 2}, {Animator.StringToHash("skill_mibao"), 3}, {Animator.StringToHash("Run"), 4}, {Animator.StringToHash("BATC"), 5}, {Animator.StringToHash("Stand"), 6}
                };

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

            //Request chou ren list.
            YaBiaoMoreInfoReq temp6 = new YaBiaoMoreInfoReq
            {
                type = 4
            };
            SocketHelper.SendQXMessage(temp6, ProtoIndexes.C_YABIAO_MOREINFO_RSQ);

            //Request alliance tech info.
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKJ_INFO);
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
                if (GUILayout.Button("Test buy blood times"))
                {
                    CommonBuy.Instance.ShowBuy(123, 456, "血瓶", LanguageTemplate.GetText(1801).Replace("n", "5"), m_CarriageMain.DoBuyBloodTimes, 0, 1);
                }
                if (GUILayout.Button("Test vip"))
                {
                    CommonBuy.Instance.ShowVIP();
                }
                if (GUILayout.Button("Test long damage num"))
                {
                    m_SelfPlayerCultureController.OnDamage(-647658086, 1, true);
                }
                if (GUILayout.Button("Test skip skill"))
                {
                    if (m_CarriageMain.m_TargetId < 0) return;

                    var temp = m_CarriageItemSyncManager.m_PlayerDic[m_CarriageMain.m_TargetId].transform;

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

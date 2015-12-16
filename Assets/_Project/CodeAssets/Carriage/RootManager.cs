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

        public Camera TrackCamera;
        public Camera NGUICamera;
        [HideInInspector]
        public Vector3 TrackCameraPosition;
        [HideInInspector]
        public Vector3 TrackCameraRotation;

        public Transform originalPosition;

        public List<GameObject> PlayerPrefabList = new List<GameObject>();

        public CarriagePlayerController m_SelfPlayerController;
        public CarriagePlayerCultureController m_SelfPlayerCultureController;

        public static float BasicYPosition = -3.24f;

        public void CreateSelfPlayer(int p_roleID, Vector3 p_position, string p_kingName, string p_allianceName, int p_vipLevel, int p_TitleIndex, int p_alliancePost, int p_nation, int p_level, int p_battleValue, float p_remainBlood = -1, float p_totalBlood = -1)
        {
            var tempObject = Instantiate(PlayerPrefabList[p_roleID - 1]) as GameObject;
            tempObject.transform.localPosition = p_position;

            m_SelfPlayerController = tempObject.GetComponent<CarriagePlayerController>() ?? tempObject.AddComponent<CarriagePlayerController>();
            m_SelfPlayerCultureController = tempObject.GetComponent<CarriagePlayerCultureController>();
            m_CarriageMain.AddGizmos(PlayerSceneSyncManager.Instance.m_MyselfUid, 0);

            //Set CarriagePlayerController.
            m_SelfPlayerController.IsRotateCamera = false;
            m_SelfPlayerController.IsUploadPlayerPosition = true;
            m_SelfPlayerController.BaseGroundPosY = 0.1f;

            m_SelfPlayerController.TrackCameraPosition = TrackCameraPosition;
            m_SelfPlayerController.TrackCameraRotation = TrackCameraRotation;

            m_SelfPlayerController.m_Joystick = m_CarriageMain.m_Joystick;
            m_SelfPlayerController.TrackCamera = TrackCamera;

            //Set CarriagePlayerCultureController.
            m_SelfPlayerCultureController.TrackCamera = TrackCamera;

            m_SelfPlayerCultureController.IsEnemy = false;
            m_SelfPlayerCultureController.KingName = p_kingName;
            m_SelfPlayerCultureController.AllianceName = p_allianceName;
            m_SelfPlayerCultureController.Vip = p_vipLevel;
            m_SelfPlayerCultureController.Title = p_TitleIndex;
            m_SelfPlayerCultureController.AlliancePost = p_alliancePost;
            m_SelfPlayerCultureController.NationID = p_nation;
            m_SelfPlayerCultureController.Level = p_level;
            m_SelfPlayerCultureController.BattleValue = p_battleValue;
            m_SelfPlayerCultureController.RoleID = p_roleID;
            m_SelfPlayerCultureController.UID = PlayerSceneSyncManager.Instance.m_MyselfUid;
            if (p_totalBlood > 0)
            {
                m_SelfPlayerCultureController.TotalBlood = p_totalBlood;
                m_SelfPlayerCultureController.RemainingBlood = p_remainBlood;
            }

            m_SelfPlayerCultureController.SetThis();

            //Set HeadIcon.
            m_CarriageMain.SelfIconSetter.SetThis(p_roleID, true, p_level, p_kingName, p_allianceName, p_totalBlood, p_remainBlood, p_nation, p_vipLevel, p_battleValue);
            m_CarriageMain.SelfIconSetter.gameObject.SetActive(true);
        }

        #region

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

            l_animator.Play(animationName);

            return true;
        }

        #endregion

        void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                //carriage click trigger.
                var trackCameraRay = TrackCamera.ScreenPointToRay(Input.mousePosition);
                var trackCameraHits = Physics.RaycastAll(trackCameraRay, Mathf.Infinity);

                var nguiCameraRay = NGUICamera.ScreenPointToRay(Input.mousePosition);
                var nguiCameraHits = Physics.RaycastAll(nguiCameraRay, Mathf.Infinity);

                RaycastHit shieldHit = trackCameraHits.Concat(nguiCameraHits).Where(item => (item.transform.gameObject.layer == LayerMask.NameToLayer("NGUI")) || (item.transform.tag == "CarriageItemShield")).FirstOrDefault();
                RaycastHit tempHit = trackCameraHits.Where(item => item.transform.tag == "CarriageItemTrigger").FirstOrDefault();
                if (shieldHit.transform != null)
                {
                    return;
                }
                if (tempHit.transform != null)
                {
                    tempHit.transform.gameObject.SendMessage("OnCarriageItemClick");
                }
            }
        }

        void Start()
        {
            TrackCameraPosition = TrackCamera.transform.localPosition;
            TrackCameraRotation = TrackCamera.transform.localEulerAngles;

            CreateSelfPlayer(CityGlobalData.m_king_model_Id, new Vector3(PlayerSceneSyncManager.Instance.m_MyselfPosition.z, BasicYPosition, PlayerSceneSyncManager.Instance.m_MyselfPosition.z), JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name, JunZhuData.Instance().m_junzhuInfo.vipLv, JunZhuData.m_iChenghaoID, AllianceData.Instance.IsAllianceNotExist ? -1 : AllianceData.Instance.g_UnionInfo.identity, JunZhuData.Instance().m_junzhuInfo.guoJiaId, JunZhuData.Instance().m_junzhuInfo.level, JunZhuData.Instance().m_junzhuInfo.zhanLi);

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();
        }

        void Awake()
        {
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQinglan"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageQiangwei"));
            PlayerPrefabList.Add(Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoli"));

            PlayerState temp = new PlayerState
            {
                s_state = State.State_YABIAO
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);
        }
    }
}

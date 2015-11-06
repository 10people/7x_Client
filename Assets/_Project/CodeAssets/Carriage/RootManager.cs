using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace Carriage
{
    public class RootManager : MonoBehaviour
    {
        #region Stored Data Before Battle

        public static Vector3 m_PlayerLastLocalPosition;
        public static bool IsGotoLastPosition;

        public static YabiaoJunZhuInfo KingInfoBattleWith;
        public static bool IsPlayCarriageDead;

        #endregion

        public CarriagePlayerController m_CarriagePlayerController;
        public CarriageManager m_CarriageManager;
        public SpotPointManager m_SpotPointManager;
        public CarriageUI m_CarriageUi;

        public Camera PlayerTrackCamera;
        public List<GameObject> DimmerObjectList = new List<GameObject>();

        public readonly float BasicYPosition = -3f;

        private readonly Vector3 InitPosition = new Vector3(216, -3, 73);

        private void OnPlayerLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            var tempObject = Instantiate(loadedObject) as GameObject;
            UtilityTool.ActiveWithStandardize(null, tempObject.transform);
            if (IsGotoLastPosition && m_PlayerLastLocalPosition != Vector3.zero)
            {
                IsGotoLastPosition = false;
                tempObject.transform.localPosition = m_PlayerLastLocalPosition;
            }
            else
            {
                tempObject.transform.localPosition = InitPosition;
            }

            m_CarriagePlayerController = tempObject.AddComponent<CarriagePlayerController>();

            m_CarriagePlayerController.IsRotateCamera = false;
            m_CarriagePlayerController.IsUploadPlayerPosition = false;
            m_CarriagePlayerController.BaseGroundPosY = -3;

            m_CarriagePlayerController.m_RootManager = this;
            m_CarriagePlayerController.m_CharacterController = tempObject.GetComponent<CharacterController>();
            m_CarriagePlayerController.m_CharacterController.radius = 1;

            m_CarriagePlayerController.m_NavMeshAgent = tempObject.GetComponent<NavMeshAgent>();

            m_CarriagePlayerController.TrackCamera = PlayerTrackCamera;
            m_CarriagePlayerController.TrackCameraOffsetPosBack = Math.Abs(m_CarriagePlayerController.TrackCamera.transform.localPosition.z - tempObject.transform.localPosition.z);
            m_CarriagePlayerController.TrackCameraOffsetPosUp = Math.Abs(m_CarriagePlayerController.TrackCamera.transform.localPosition.y - BasicYPosition);
            m_CarriagePlayerController.TrackCamera.gameObject.SetActive(true);

            m_CarriagePlayerController.m_Joystick = m_CarriageUi.m_Joystick;

            //Try play init and destroy a carriage, used for player destroy carriage self.
            TryPlayDestroyCarriage();
        }

        void Start()
        {
            UIYindao.m_UIYindao.CloseUI();
            UIYindao.m_UIYindao.setCloseUIEff();
        }

        void Awake()
        {
            CarriageSceneManager.Instance.m_RootManager = this;

            //Refresh all carriages after scene loaded.
            UtilityTool.SendQXMessage(CarriageSceneManager.Instance.s_RoomId, ProtoIndexes.C_BIAOCHE_INFO);

            PlayerState temp = new PlayerState
            {
                s_state = State.State_YABIAO
            };
            UtilityTool.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);

            //load and set player.
            Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
                OnPlayerLoadCallBack);
        }

        public void TryPlayDestroyCarriage()
        {
            //show carriage dead animation.
            if (IsPlayCarriageDead && KingInfoBattleWith != null)
            {
                IsPlayCarriageDead = false;
                //add a carriage model without ui.
                m_CarriageManager.InitACarriage(KingInfoBattleWith);

                StopCoroutine("DoPlayDestroyCarriage");
                //destroy it.
                StartCoroutine(DoPlayDestroyCarriage());
            }
        }

        IEnumerator DoPlayDestroyCarriage()
        {
            yield return new WaitForSeconds(3.0f);

            m_CarriageManager.RefreshACarriage(new BiaoCheState()
            {
                baohuCD = KingInfoBattleWith.baohuCD,
                hp = KingInfoBattleWith.hp,
                junZhuId = KingInfoBattleWith.junZhuId,
                state = 50,
                usedTime = KingInfoBattleWith.usedTime,
                worth = KingInfoBattleWith.worth
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                if (DimmerObjectList.Count != 0)
                {
                    return;
                }

                //Check ngui ui hit.
                Ray ray = m_CarriageUi.GetComponentInChildren<UICamera>().GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit[] tempHits = Physics.RaycastAll(ray, Mathf.Infinity);
                if (tempHits != null && tempHits.Length > 0)
                {
                    Debug.LogWarning("Cancel navi to carriage cause hit ui.");
                    return;
                }

                //Check joystick in handling.
                if (CityGlobalData.m_joystickControl)
                {
                    Debug.LogWarning("Cancel navi to carriage cause joystick on control.");
                    return;
                }

                //House weapon, treasure, book click trigger.
                ray = PlayerTrackCamera.ScreenPointToRay(Input.mousePosition);
                tempHits = Physics.RaycastAll(ray, Mathf.Infinity);

                RaycastHit tempHit = tempHits.Where(item => item.transform.tag == "CarriageItemTrigger").FirstOrDefault();
                RaycastHit shieldHit = tempHits.Where(item => item.transform.tag == "CarriageItemShield").FirstOrDefault();
                if (shieldHit.transform != null)
                {
                    return;
                }
                if (tempHit.transform != null)
                {
                    tempHit.transform.gameObject.SendMessage("OnClick");
                }
            }
        }
    }
}

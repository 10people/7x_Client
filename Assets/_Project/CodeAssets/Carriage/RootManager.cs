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
        public CarriageUI m_CarriageUI;
        public CarriageItemManager m_CarriageItemManager;
        public CarriageMsgManager m_CarriageMsgManager;

        public Camera TrackCamera;
        [HideInInspector]
        public Vector3 TrackCameraPosition;
        [HideInInspector]
        public Vector3 TrackCameraRotation;

        public Transform originalPosition;

        public GameObject HaojiePrefab;
        public GameObject LuoliPrefab;

        public Joystick m_Joystick;
        public CarriagePlayerController m_SelfPlayerController;
        public CarriagePlayerCultureController m_SelfPlayerCultureController;

        public static float BasicYPosition = 0.1f;

        public void InitPlayer(int roleID, Vector3 position, string kingName, string allianceName, float totalBlood = -1)
        {
            var tempObject = Instantiate(roleID <= 2 ? HaojiePrefab : LuoliPrefab) as GameObject;
            tempObject.transform.localPosition = position;

            m_SelfPlayerController = tempObject.GetComponent<CarriagePlayerController>() ?? tempObject.AddComponent<CarriagePlayerController>();
            m_SelfPlayerCultureController = tempObject.GetComponent<CarriagePlayerCultureController>();

            m_SelfPlayerController.IsRotateCamera = false;
            m_SelfPlayerController.IsUploadPlayerPosition = true;
            m_SelfPlayerController.BaseGroundPosY = 0.1f;
            m_SelfPlayerController.m_CharacterSyncDuration = 0.1f;

            m_SelfPlayerController.TrackCameraPosition = TrackCameraPosition;
            m_SelfPlayerController.TrackCameraRotation = TrackCameraRotation;

            m_SelfPlayerController.m_Joystick = m_Joystick;
            m_SelfPlayerController.TrackCamera = TrackCamera;

            m_SelfPlayerCultureController.TrackCamera = TrackCamera;

            m_SelfPlayerCultureController.IsRed = false;
            m_SelfPlayerCultureController.KingName = kingName;
            m_SelfPlayerCultureController.AllianceName = allianceName;
            if (totalBlood > 0)
            {
                m_SelfPlayerCultureController.TotalBlood = totalBlood;
                m_SelfPlayerCultureController.RemainingBlood = m_SelfPlayerCultureController.TotalBlood;
            }

            m_SelfPlayerCultureController.SetThis();
        }

        void Start()
        {
            TrackCameraPosition = TrackCamera.transform.localPosition;
            TrackCameraRotation = TrackCamera.transform.localEulerAngles;

            InitPlayer(CityGlobalData.m_king_model_Id, originalPosition.localPosition, JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name);

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();
        }

        void Awake()
        {
            HaojiePrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageHaoJie");
            LuoliPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageLuoLi");

            PlayerState temp = new PlayerState
            {
                s_state = State.State_YABIAO
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);
        }
    }
}

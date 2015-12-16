using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class RootManager : MonoBehaviour
    {
        public AllianceBattleUI m_AllianceBattleUi;
        public ABPlayerManager m_AbPlayerManager;
        public AllianceBattleHoldPointManager m_AllianceBattleHoldPointManager;

        public Camera TrackCamera;
        [HideInInspector]
        public Vector3 TrackCameraPosition;
        [HideInInspector]
        public Vector3 TrackCameraRotation;

        [HideInInspector]
        public Vector3 originalPosition1;
        [HideInInspector]
        public Vector3 originalPosition2;

        public GameObject HaojiePrefab;
        public GameObject LuoliPrefab;

        public Joystick m_Joystick;
        public ABPlayerController m_AbPlayerController;
        public ABPlayerCultureController m_AbPlayerCultureController;

        public static float BasicYPosition = 0.1f;

        public void InitPlayer(int roleID, Vector3 position, string kingName, string allianceName, float totalBlood = -1)
        {
            var tempObject = Instantiate(roleID <= 2 ? HaojiePrefab : LuoliPrefab) as GameObject;
            tempObject.transform.localPosition = position;

            m_AbPlayerController = tempObject.GetComponent<ABPlayerController>() ?? tempObject.AddComponent<ABPlayerController>();
            m_AbPlayerCultureController = tempObject.GetComponent<ABPlayerCultureController>();

            m_AbPlayerController.IsRotateCamera = false;
            m_AbPlayerController.IsUploadPlayerPosition = true;
            m_AbPlayerController.BaseGroundPosY = 0.1f;

            m_AbPlayerController.TrackCameraPosition = TrackCameraPosition;
            m_AbPlayerController.TrackCameraRotation = TrackCameraRotation;

            m_AbPlayerController.m_Joystick = m_Joystick;
            m_AbPlayerController.TrackCamera = TrackCamera;

            m_AbPlayerCultureController.TrackCamera = TrackCamera;

            m_AbPlayerCultureController.IsRed = false;
            m_AbPlayerCultureController.KingName = kingName;
            m_AbPlayerCultureController.AllianceName = allianceName;
            if (totalBlood > 0)
            {
                m_AbPlayerCultureController.TotalBlood = totalBlood;
                m_AbPlayerCultureController.RemainingBlood = m_AbPlayerCultureController.TotalBlood;
            }

            m_AbPlayerCultureController.SetThis();
        }

        void Start()
        {
            TrackCameraPosition = TrackCamera.transform.localPosition;
            TrackCameraRotation = TrackCamera.transform.localEulerAngles;

            InitPlayer(CityGlobalData.m_king_model_Id, PlayerSceneSyncManager.Instance.m_MyselfPosition, JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name);

            //Close yindao UI.
            UIYindao.m_UIYindao.CloseUI();
        }

        void Awake()
        {
            HaojiePrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/AllianceBattleHaoJie");
            LuoliPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/AllianceBattleLuoLi");

            //Read data from xmls.
            originalPosition1 = new Vector3(LMZBuildingTemplate.GetTemplatesBySide(1).First().Position.x, BasicYPosition, LMZBuildingTemplate.GetTemplatesBySide(1).First().Position.y);
            originalPosition2 = new Vector3(LMZBuildingTemplate.GetTemplatesBySide(2).First().Position.x, BasicYPosition, LMZBuildingTemplate.GetTemplatesBySide(2).First().Position.y);

            PlayerState temp = new PlayerState
            {
                s_state = State.State_FIGHT_SCENE
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);
        }
    }
}

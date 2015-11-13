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
        public AlliancePlayerManager m_AlliancePlayerManager;
        public AllianceBattleHoldPointManager m_AllianceBattleHoldPointManager;

        public Camera TrackCamera;

        [HideInInspector]
        public Vector3 originalPosition1;
        [HideInInspector]
        public Vector3 originalPosition2;

        public GameObject HaojiePrefab;
        public GameObject LuoliPrefab;

        public Joystick m_Joystick;
        public AlliancePlayerController m_AlliancePlayerController;
        public AllianceBasicPlayerController m_AllianceBasicPlayerController;

        public static float BasicYPosition = 0.1f;

        public void InitPlayer(int roleID, Vector3 position, string kingName, string allianceName, float totalBlood = -1)
        {
            var tempObject = Instantiate(roleID <= 2 ? HaojiePrefab : LuoliPrefab) as GameObject;
            tempObject.transform.localPosition = position;

            m_AlliancePlayerController = tempObject.GetComponent<AlliancePlayerController>() ?? tempObject.AddComponent<AlliancePlayerController>();
            m_AllianceBasicPlayerController = tempObject.GetComponent<AllianceBasicPlayerController>();

            m_AlliancePlayerController.IsRotateCamera = false;
            m_AlliancePlayerController.IsUploadPlayerPosition = true;
            m_AlliancePlayerController.BaseGroundPosY = 0.1f;
            m_AlliancePlayerController.m_CharacterSyncDuration = 0.1f;

            m_AlliancePlayerController.m_Joystick = m_Joystick;
            m_AlliancePlayerController.TrackCamera = TrackCamera;

            m_AllianceBasicPlayerController.TrackCamera = TrackCamera;

            m_AllianceBasicPlayerController.IsRed = false;
            m_AllianceBasicPlayerController.KingName = kingName;
            m_AllianceBasicPlayerController.AllianceName = allianceName;
            if (totalBlood > 0)
            {
                m_AllianceBasicPlayerController.TotalBlood = totalBlood;
                m_AllianceBasicPlayerController.RemainingBlood = m_AllianceBasicPlayerController.TotalBlood;
            }

            m_AllianceBasicPlayerController.SetThis();
        }

        void Start()
        {
            InitPlayer(CityGlobalData.m_king_model_Id, originalPosition1, JunZhuData.Instance().m_junzhuInfo.name, AllianceData.Instance.IsAllianceNotExist ? "" : AllianceData.Instance.g_UnionInfo.name);

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
            UtilityTool.SendQXMessage(temp, ProtoIndexes.PLAYER_STATE_REPORT);
        }
    }
}

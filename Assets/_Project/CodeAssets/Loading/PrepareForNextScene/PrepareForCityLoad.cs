using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PrepareForCityLoad : Singleton<PrepareForCityLoad>, SocketListener
{
    private static PrepareForCityLoad m_instance = null;
    //public static PrepareForCityLoad Instance()
    //{
    //    return m_instance;
    //}
    private List<NpcCityTemplate> listNpcTempInfo = new List<NpcCityTemplate>();
    public struct NpcInfo
    {
        public int _Type;
        public string _Name;
        public NpcCityTemplate _NpcTemp;
        public GameObject _Obj;
    };
    public List<NpcInfo> m_listNpcTemp = new List<NpcInfo>();

    NpcInfo _PortSelf;
    NpcInfo _PortOther;
    public const string CONST_CITY_LOADING_CITY_NET = "City_NET";

    public const string CONST_CITY_LOADING_2D_UI = "City_2D_UI";

    public const string CONST_CITY_LOADING_2D_NAME = "City_2D_Name";

    public const string CONST_CITY_LOADING_3D_NPC = "City_3D_NPC";

    public const string CONST_CITY_LOADING_3D_JUNZHU_MODEL = "City_3D_JUNZHU_MODEL";

    public const string CONST_CITY_LOADING_GENERAL_REWARD= "City_GENERAL_REWARD";
 

 

    void Awake()
    {
      m_instance = this;
    }
    // Use this for initialization
    void Start()
    {
        SocketTool.RegisterSocketListener(this);
    }
 
    void OnDestroy()
    {
        //m_instance = null;
    }

    private void InitCityLoading()
    {
        SocketTool.RegisterSocketListener(this);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_CITY_NET, 1, 4);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_UI, 1, 1);

        //if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
        //{
        //    StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 1, 15);
        //}
        //else
        {
            StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 1, 13);
        }
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_JUNZHU_MODEL, 1, 1);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_NAME, 1, 1);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_GENERAL_REWARD, 1, 1);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, PrepareForBattleField.CONST_BATTLE_LOADING_SOUND, 1, 1);
 
    }


    private int m_battle_res_step = 0;

    private const int BATTLE_RES_STEP_TOTAL = 2;

    private GameObject m_CitytempleUI;
    private GameObject m_CitySelfName;
    private GameObject m_CityJunZhuModel;
    private GameObject temple3D;
    private GameObject m_GeneralReward;

    private void Load_2D_UI()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_MAINUI),
                                Load2DCallback);
    }

    public void Load2DCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_CitytempleUI = p_object as GameObject;
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                         CONST_CITY_LOADING_2D_UI, "");
        LoadCityNpc();

    }

    void LoadCityNpc()
    {
        listNpcTempInfo.Clear();
        m_listNpcTemp.Clear();
        _IndexNum = 0;
       // if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        {
            foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
            {
                if (_template.m_Type == 1)
                {
                    listNpcTempInfo.Add(_template);
                }
            }
            int size = listNpcTempInfo.Count;
            for (int i = 0; i < size; i++)
            {
                Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(listNpcTempInfo[i].m_npcShowId),
                                  LoadNpcCallback);
            }
        }
        //else if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
        //{
        //    foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        //    {
        //        if (_template.m_Type == 2 && _template.m_npcId < 1000)
        //        {
        //            listNpcTempInfo.Add(_template);
        //        }
        //    }
        //    int size = listNpcTempInfo.Count;
        //    for (int i = 0; i < size; i++)
        //    {
        //        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(listNpcTempInfo[i].m_npcShowId),
        //                          LoadNpcCallback);
        //    }
        //}
    }
    int _IndexNum = 0;
    public void LoadNpcCallback(ref WWW p_www, string p_path, Object p_object)
    {
        int size = listNpcTempInfo.Count;
        if (ModelTemplate.GetModelIdByPath(p_path) == listNpcTempInfo[_IndexNum].m_npcShowId)
        {
            NpcInfo no = new NpcInfo();
            no._Type = 0;
            no._NpcTemp = listNpcTempInfo[_IndexNum];
            no._Obj = p_object as GameObject;
            m_listNpcTemp.Add(no);
        }
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                         CONST_CITY_LOADING_3D_NPC, "");
        if (_IndexNum < listNpcTempInfo.Count - 1)
        {
            _IndexNum++;
        }
        else
        {
            if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
            {
                Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202), LoadHousePort);
                Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100201), LoadHousePort);
            }
            else
            {
                LoadJunZhuModel();
            }
        }
        index_Port_Num = 0;



    }

    private int index_Port_Num = 0;
    void LoadHousePort(ref WWW p_www, string p_path, Object p_object)
    {
        index_Port_Num++;
        NpcInfo no = new NpcInfo();
        no._Type = 1;
        if (p_path.IndexOf("chuansongmen11") > -1)
        {
            no._Name = "chuansongmen11";
        }
        else if (p_path.IndexOf("chuansongmen22") > -1)
        {
            no._Name = "chuansongmen22";
        }
        no._NpcTemp = listNpcTempInfo[_IndexNum];
        no._Obj = p_object as GameObject;
        m_listNpcTemp.Add(no);
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                 CONST_CITY_LOADING_3D_NPC, "");
        if (index_Port_Num == 2)
        {
            LoadJunZhuModel();
        }
    }

    void LoadSelfName()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
                            LoadSelfNameCallback);
    }

    public void LoadSelfNameCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_CitySelfName = p_object as GameObject;
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
            CONST_CITY_LOADING_2D_NAME, "");

        // CityLoadDone();
        LoadGeneralReward();

    }

    void LoadGeneralReward()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_POP_REWARD_ROOT), ResourceLoadGeneralRewardCallback);
    }

    private void ResourceLoadGeneralRewardCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        m_GeneralReward = p_object as GameObject;
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
         CONST_CITY_LOADING_GENERAL_REWARD, "");

        StartCoroutine(WaitForLoadComplete());
    }
    IEnumerator WaitForLoadComplete()
    {
      yield return new WaitForEndOfFrame();
        CityLoadDone();
    }
        void LoadJunZhuModel()
    {
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
                                ResourceLoadModelCallback);

    }
    private void ResourceLoadModelCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_CityJunZhuModel = p_object as GameObject;
        StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                 CONST_CITY_LOADING_3D_JUNZHU_MODEL, "");
        LoadSelfName();
    }
    public void CityLoadDone()
    {
        NpcManager.m_NpcManager.m_npcObjectItemDic.Clear();
        MainCityRoot.Instance().CreateMainCity(m_CitytempleUI);
        PlayerModelController.m_playerModelController.CreatePlayerModel(m_CityJunZhuModel);
        PlayerSelfNameManagerment.ShowSelfeName(m_CitySelfName);
        for (int i = 0; i < m_listNpcTemp.Count; i++)
        {
            if (m_listNpcTemp[i]._Type == 0)
            {
                NpcManager.m_NpcManager.CreateNPC(m_listNpcTemp[i]);
            }
            else
            {
                if (m_listNpcTemp[i]._Name.IndexOf("11") > -1)
                {
                    _PortSelf = m_listNpcTemp[i];
                }
                if (m_listNpcTemp[i]._Name.IndexOf("22") > -1)
                {
                    _PortOther = m_listNpcTemp[i];
                }

            }
        }

        if (_PortOther._Obj != null)
        {
            NpcManager.m_NpcManager.CreateHousePortal(_PortSelf, _PortOther);
        }
        if (GeneralRewardManager.Instance() == null)
        {
            GameObject obj = GameObject.Instantiate(m_GeneralReward);
            DontDestroyOnLoad(obj);
        }
        EnterNextScene.Instance().DestroyUI();
    }

    private static int m_received_data_for_main_city = 0;

    private const  int REQUEST_DATA_COUNT_FOR_MAINCITY = 4;

    /// Prepare Data For Main City.
    public void Prepare_For_MainCity()
    {
        //Debug.LogError("Prepare_For_MainCityPrepare_For_MainCityPrepare_For_MainCityPrepare_For_MainCity");
        InitCityLoading();
        // reset info
        {
            m_received_data_for_main_city = 0;

            StartCoroutine(CheckingDataForMainCity());
        }

        // request PVE Info
        {
            JunZhuDiaoLuoManager.RequestMapInfo(-1);
        }

        // request JunZhu Info
        {
            // create instance, for battle field use.
            //			Debug.Log ("MainCity");
            JunZhuData.Instance();
            JunZhuData.RequestJunZhuInfo();
        }

        //request Alliance Info
        {
            AllianceData.Instance.RequestData();
        }

        // request Task Info
        {
            TaskData.Instance.RequestData();
        }
        //  request Friend Info
        {
            FriendOperationData.Instance.RequestData();
        }

    }

    private bool _isEnterMainCity = true;
    IEnumerator CheckingDataForMainCity()
    {
   
        while (m_received_data_for_main_city < REQUEST_DATA_COUNT_FOR_MAINCITY)
        {
            yield return new WaitForEndOfFrame();
        }

        // enter pve for 1st battle.
        if (Global.m_iScreenID == 100101 || Global.m_iScreenID == 100102 || Global.m_iScreenID == 100103)
        {
            _isEnterMainCity = false;
           
            DestroyForNextLoading();
            //UnRegister();
            EnterBattleField.EnterBattlePve(1, Global.m_iScreenID % 10, LevelType.LEVEL_NORMAL);
        }
        else
        {
            _isEnterMainCity = true;
            EnterNextScene.DirectLoadLevel();
        }
 
        if (m_received_data_for_main_city == REQUEST_DATA_COUNT_FOR_MAINCITY && _isEnterMainCity)
        {
            UnRegister();
            Load_2D_UI();
        }
    }
    private static float m_preserve_percentage = 0.0f;
    private void DestroyForNextLoading()
    {
        m_preserve_percentage = StaticLoading.GetLoadingPercentage(StaticLoading.m_loading_sections);

     //   UnRegister();
      EnterNextScene.Instance().DestroyUI();
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
     //   Debug.LogError("OnSocketEventOnSocketEventOnSocketEventOnSocketEvent");
        if (p_message == null)
        {
            return false;
        }
        switch (p_message.m_protocol_index)
        {
            case ProtoIndexes.PVE_PAGE_RET:
                {
                    ProcessPVEPageReturn(p_message);

                    m_received_data_for_main_city++;

                    StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                            CONST_CITY_LOADING_CITY_NET, "PVE_PAGE_RET");

                    return true;
                }
            case ProtoIndexes.JunZhuInfoRet:
                {

                    //				Debug.Log( "获得君主数据: " + Global.m_iScreenID );

                    m_received_data_for_main_city++;
                    StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "JunZhuInfoRet");

                    return true;
                }

            case ProtoIndexes.ALLIANCE_HAVE_RESP:
                {

                    //				Debug.Log ("获得有联盟信息");

                    m_received_data_for_main_city++;
                    StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "ALLIANCE_HAVE_RESP");

                    return true;
                }

            case ProtoIndexes.ALLIANCE_NON_RESP:
                {

                    //				Debug.Log ("获得无联盟信息");
                    m_received_data_for_main_city++;
                    StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "ALLIANCE_NON_RESP");
                    return true;
                }
            case ProtoIndexes.S_TaskList:
                {
                    //				Debug.Log( "获得主线任务: " + Global.m_iScreenID );
                    m_received_data_for_main_city++;
                    StaticLoading.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "TaskList");
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }

    private void ProcessPVEPageReturn(QXBuffer p_buffer)
    {
        //		Debug.Log( "ProcessPVEPageReturn( 获得了管卡数据 )" );

        MemoryStream t_stream = new MemoryStream(p_buffer.m_protocol_message, 0, p_buffer.position);

        QiXiongSerializer t_qx = new QiXiongSerializer();

        Section tempInfo = new Section();

        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

        if (tempInfo.s_allLevel == null)
        {
            Debug.Log("tempInfo.s_allLevel == null");
        }
        if (tempInfo.maxCqPassId > CityGlobalData.m_temp_CQ_Section)
        {
            CityGlobalData.m_temp_CQ_Section = tempInfo.maxCqPassId;
        }

        foreach (Level tempLevel in tempInfo.s_allLevel)
        {
            if (!tempLevel.s_pass)
            {

                Global.m_iScreenID = tempLevel.guanQiaId;

                break;
            }
        }
    }
    void UnRegister()
    {
        SocketTool.UnRegisterSocketListener(this);
    }

    public void Prepare_For_AllianceCity11()
    {
        InitCityLoading();

        // reset info
        {
            m_received_data_for_main_city = 0;

            StartCoroutine(CheckingDataForMainCity());
        }


        // request PVE Info
        {
            JunZhuDiaoLuoManager.RequestMapInfo(-1);
        }

        // request JunZhu Info
        {

            JunZhuData.Instance();
            JunZhuData.RequestJunZhuInfo();
        }
        {
 
            TaskData.Instance.RequestData();
        }

        //request Alliance Info
        {
            AllianceData.Instance.RequestData();
        }

        {
            TenementData.Instance.RequestData();
        }

        //  request Friend Info
        {
            FriendOperationData.Instance.RequestData();
        }
    }

}
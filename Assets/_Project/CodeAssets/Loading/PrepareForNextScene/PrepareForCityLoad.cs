//#define DEBUG_PREPARE_FOR_CITY_LOAD

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PrepareForCityLoad : MonoBehaviour, SocketListener
{
    private List<NpcCityTemplate> listNpcTempInfo = new List<NpcCityTemplate>();
    
	public struct NpcInfo{
        public bool _IsEffect;
        public NpcCityTemplate _NpcTemp;
        public GameObject _Obj;
    };

    private List<NpcInfo> m_listNpcTemp = new List<NpcInfo>();

    NpcInfo _PortSelf;
    NpcInfo _PortOther;
    public const string CONST_CITY_LOADING_CITY_NET = "City_NET";

    public const string CONST_CITY_LOADING_2D_UI = "City_2D_UI";

    public const string CONST_CITY_LOADING_2D_NAME = "City_2D_Name";
    public const string CONST_CITY_LOADING_2D_General_Reward = "City_2D_General_Reward";

    public const string CONST_CITY_LOADING_3D_NPC = "City_3D_NPC";

    public const string CONST_CITY_LOADING_3D_JUNZHU_MODEL = "City_3D_JUNZHU_MODEL";

    public const string CONST_CITY_LOADING_GENERAL_REWARD= "City_GENERAL_REWARD";
 

 

    void Awake(){
    
	}
    
	// Use this for initialization
    void Start()
    {
        SocketTool.RegisterSocketListener(this);
    }
 
    void OnDestroy(){
		SocketTool.UnRegisterSocketListener(this);
    }

    private void InitCityLoading()
    {
        SocketTool.RegisterSocketListener(this);
        
		LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_CITY_NET, 3, 4);

		LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_UI, 3, 1);

        //if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
        //{
        //    LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 1, 15);
        //}
        //else
        {
            LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 15, 11);
        }

        LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_JUNZHU_MODEL, 2, 1);

		LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_NAME, 1, 1);
        
		LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_General_Reward, 2, 1);

        LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_GENERAL_REWARD, 2, 1);
    }


    private int m_battle_res_step = 0;

    private const int BATTLE_RES_STEP_TOTAL = 2;

    private GameObject m_CitytempleUI;
    private GameObject m_CitySelfName;
    private GameObject m_CityJunZhuModel;
    private GameObject temple3D;
    private GameObject m_GeneralReward;
 
    private bool _Load2dUIIsDone = false;
    private void Load_2D_UI()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_MAINUI),
                                Load2DCallback);

		// preload online reward
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.PRE_LOAD_EQUIP_ATLAS),
				LoadEquipAtlasCallback );

			Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.PRE_LOAD_ACTIVITY_ATLAS),
				LoadOnLineRewardCallback );
		}
    }

	public void LoadEquipAtlasCallback(ref WWW p_www, string p_path, Object p_object){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );

		ComponentHelper.AddIfNotExist( t_gb, typeof(DestroyAfterRendered) );
	}

	public void LoadOnLineRewardCallback(ref WWW p_www, string p_path, Object p_object){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );

		ComponentHelper.AddIfNotExist( t_gb, typeof(DestroyAfterRendered) );

//		#if UNITY_EDITOR
//		EditorApplication.isPaused = true;
//		#endif
	}

    public void Load2DCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_CitytempleUI = p_object as GameObject;
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                         CONST_CITY_LOADING_2D_UI, "");
        LoadCityNpc();

    }

    void Update()
    {
        if (UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.MAIN_CITY)) && _Load2dUIIsDone)
        {
            _Load2dUIIsDone = false;
            CityLoadDone();
        }
       
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
                if (_template.m_Type == 1 && _template.m_Id != 3)
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
            no._NpcTemp = listNpcTempInfo[_IndexNum];
            no._Obj = p_object as GameObject;
            m_listNpcTemp.Add(no);
        }
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                                         CONST_CITY_LOADING_3D_NPC, "");
        if (_IndexNum < listNpcTempInfo.Count - 1)
        {
            _IndexNum++;
        }
        else
        {
   
                Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202), LoadHousePort);
 
        }
        index_Port_Num = 0;



    }

    private int index_Port_Num = 0;
    void LoadHousePort(ref WWW p_www, string p_path, Object p_object)
    {
        index_Port_Num++;
        NpcInfo no = new NpcInfo();
        no._IsEffect = true;
        no._NpcTemp = listNpcTempInfo[2];
        no._Obj = p_object as GameObject;
        m_listNpcTemp.Add(no);
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                                 CONST_CITY_LOADING_3D_NPC, "");
        //if (index_Port_Num == 2)
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
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
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
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
           CONST_CITY_LOADING_2D_General_Reward, "");
        m_GeneralReward = p_object as GameObject;
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
         CONST_CITY_LOADING_GENERAL_REWARD, "");
        _Load2dUIIsDone = true;


     //   StartCoroutine(WaitForLoadComplete());
    }
    IEnumerator WaitForLoadComplete()
    {
      yield return new WaitForEndOfFrame();
    }
     void LoadJunZhuModel()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MODEL_PARENT),
                        ResourceLoadModelCallback);

        //Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
        //                        ResourceLoadModelCallback);

    }
    private void ResourceLoadModelCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_CityJunZhuModel = p_object as GameObject;
        LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
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
            if (!m_listNpcTemp[i]._IsEffect)
            {
                NpcManager.m_NpcManager.CreateNPC(m_listNpcTemp[i]);
            }
            else
            {
                    _PortOther = m_listNpcTemp[i];
            }
        }

        if (_PortOther._Obj != null)
        {
            NpcManager.m_NpcManager.CreateHousePortal(_PortOther);
        }
        if (GeneralRewardManager.Instance() == null)
        {
            GameObject obj = GameObject.Instantiate(m_GeneralReward);
//			UI2DTool.Instance.AddTopUI(obj);
            DontDestroyOnLoad(obj);
        }
        EnterNextScene.Instance().DestroyUI();
    }

    private static int m_received_data_for_main_city = 0;

    private const  int REQUEST_DATA_COUNT_FOR_MAINCITY = 4;

    /// Prepare Data For Main City.
    public void Prepare_For_MainCity()
    {
		#if DEBUG_PREPARE_FOR_CITY_LOAD
		Debug.Log( "Prepare_For_MainCity()" );
		#endif

        InitCityLoading();
        StartCoroutine(CheckingDataForMainCity());
        StartCoroutine(CheckingDataLoadingTime());
        // reset info
        {
			#if DEBUG_PREPARE_FOR_CITY_LOAD
			Debug.Log( "Prepare_For_MainCity()" );
			#endif

            m_received_data_for_main_city = 0;

           
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
   
        while ( m_received_data_for_main_city < REQUEST_DATA_COUNT_FOR_MAINCITY ){
            //   Global.CreateBox("提示", "数据加载错误", "", null, null, "确定", ReLogin);
#if DEBUG_PREPARE_FOR_CITY_LOAD
          
			Debug.Log( "CheckingDataForMainCity( " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
#endif

            yield return new WaitForEndOfFrame();
        }


        if (_isEnterMainCity)
        {
            EnterNextScene.DirectLoadLevel();
        }
 

        {
			UnRegister();

			Load_2D_UI();
		}
    }
    IEnumerator CheckingDataLoadingTime()
    {
        yield return new WaitForSeconds(20.0f);
 
       if (m_received_data_for_main_city < REQUEST_DATA_COUNT_FOR_MAINCITY)
        {
            _isEnterMainCity = false;
            if (Debug.isDebugBuild)
            {
                Global.CreateBox("提示",
                                    "数据加载失败" + "\n" + ss + "\n" + SocketTool.GetSocketState(),
                                    null,
                                    null,
                                    "确定",
                                    null,
                                    ReLogin,
                                    null,
                                    null,
                                    null,
                                    false,
                                    false,
                                    true);
            }
            else
            {
                Global.CreateBox("提示",
                                       "数据加载失败",
                                       null,
                                       null,
                                       "确定",
                                       null,
                                       ReLogin,
                                       null,
                                       null,
                                       null,
                                       false,
                                       false,
                                       true);
            }
        }
    }
    private static float m_preserve_percentage = 0.0f;
    private void DestroyForNextLoading()
    {
		m_preserve_percentage = LoadingHelper.GetLoadingPercentage(StaticLoading.m_loading_sections);

     //   UnRegister();
      EnterNextScene.Instance().DestroyUI();
    }
    string ss = "";
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

					{
						#if DEBUG_PREPARE_FOR_CITY_LOAD
						Debug.Log( "OnSocketEvent( " + "PVE_PAGE_RET " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
						#endif
					}
                    ss += " PVE_PAGE_RET :";
                    LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                     
                            CONST_CITY_LOADING_CITY_NET, "PVE_PAGE_RET");

                    return true;
                }
            case ProtoIndexes.JunZhuInfoRet:
                {

                    //				Debug.Log( "获得君主数据: " + Global.m_iScreenID );
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    JunZhuInfoRet tempInfo = new JunZhuInfoRet();
                    t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                   JunZhuData.Instance().SetInfo(tempInfo);
                    m_received_data_for_main_city++;

					{
						#if DEBUG_PREPARE_FOR_CITY_LOAD
						Debug.Log( "OnSocketEvent( " + "JunZhuInfoRet " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
						#endif
					}
                    ss += " JunZhuInfoRet :";
                    LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "JunZhuInfoRet");

                    return true;
                }

            case ProtoIndexes.ALLIANCE_HAVE_RESP:
                {

                    //				Debug.Log ("获得有联盟信息");

                    m_received_data_for_main_city++;

					{
						#if DEBUG_PREPARE_FOR_CITY_LOAD
						Debug.Log( "OnSocketEvent( " + "ALLIANCE_HAVE_RESP " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
						#endif
					}
                    ss += " ALLIANCE_HAVE_RESP :";
                    LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "ALLIANCE_HAVE_RESP");

                    return true;
                }

            case ProtoIndexes.ALLIANCE_NON_RESP:
                {

                    //				Debug.Log ("获得无联盟信息");
                    m_received_data_for_main_city++;

					{
						#if DEBUG_PREPARE_FOR_CITY_LOAD
						Debug.Log( "OnSocketEvent( " + "ALLIANCE_NON_RESP " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
						#endif
					}
                    ss += " ALLIANCE_NON_RESP : ";
                    LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
                                   CONST_CITY_LOADING_CITY_NET, "ALLIANCE_NON_RESP");
                    return true;
                }
            case ProtoIndexes.S_TaskList:
                {
                    //				Debug.Log( "获得主线任务: " + Global.m_iScreenID );
                    m_received_data_for_main_city++;

					{
						#if DEBUG_PREPARE_FOR_CITY_LOAD
						Debug.Log( "OnSocketEvent( " + "ALLIANCE_NON_RESP " + m_received_data_for_main_city + " / " + REQUEST_DATA_COUNT_FOR_MAINCITY + " )" );
						#endif
					}
                    ss += " TaskList : ";
                    LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections,
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
//            Debug.Log("tempInfo.s_allLevel == null");
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

    void UnRegister(){
        SocketTool.UnRegisterSocketListener(this);
    }

    public void Prepare_For_AllianceCity11(){
		#if DEBUG_PREPARE_FOR_CITY_LOAD
		Debug.Log( "Prepare_For_AllianceCity11()" );
		#endif

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

    void ReLogin(int index)
    {
        WindowBackShowController.m_SaveEquipBuWei = 0;
        FunctionWindowsCreateManagerment.SetSelectEquipDefault();
        SceneManager.m_isSequencer = false;
        if (UIYindao.m_UIYindao != null && UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        CityGlobalData.m_isAllianceTenentsScene = false;
        CityGlobalData.m_isWashMaxSignal = false;
        SceneManager.m_isSequencer = false;
        CityGlobalData.m_isAllianceTenentsScene = false;

        {
            SceneManager.RequestEnterLogin();
        }
        _isEnterMainCity = true;
    }

}
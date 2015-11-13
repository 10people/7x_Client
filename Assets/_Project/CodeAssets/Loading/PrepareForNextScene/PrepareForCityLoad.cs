using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PrepareForCityLoad : Singleton<PrepareForCityLoad>
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
    public bool m_NetData_IsReday = false;
    public List<NpcInfo> m_listNpcTemp = new List<NpcInfo>();

    NpcInfo _PortSelf;
    NpcInfo _PortOther;
    public const string CONST_CITY_LOADING_2D_UI = "City_2D_UI";

    public const string CONST_CITY_LOADING_2D_NAME = "City_2D_Name";

    public const string CONST_CITY_LOADING_3D_NPC = "City_3D_NPC";

    public const string CONST_CITY_LOADING_3D_JUNZHU_MODEL = "City_3D_JUNZHU_MODEL";

    public const string CONST_CITY_LOADING_SOUND = "City_Sound";

    public const string CONST_CITY_RENDER = "City_Render";

    void Awake()
    {
        //m_instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_NetData_IsReday)
        {
            m_NetData_IsReday = false;
            Load_2D_UI();
        }

    }

    void OnDestroy()
    {
        //m_instance = null;
    }

    private void InitCityLoading()
    {
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_UI, 1, 1);

 
        if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
        {
            StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 1, 15);
        }
        else
        {
            StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_NPC, 1, 13);
        }
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_3D_JUNZHU_MODEL, 1, 1);
        StaticLoading.InitSectionInfo(StaticLoading.m_loading_sections, CONST_CITY_LOADING_2D_NAME, 1, 1);
    }


    private int m_battle_res_step = 0;

    private const int BATTLE_RES_STEP_TOTAL = 2;

    public GameObject m_CitytempleUI;
    public GameObject m_CitySelfName;
    public GameObject m_CityJunZhuModel;
    private GameObject temple3D;

    private void Load_2D_UI()
    {
        InitCityLoading();

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
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
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
        else if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1)
        {
            foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
            {
                if (_template.m_Type == 2 && _template.m_npcId < 1000)
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
        EnterNextScene.Instance().DestroyUI();
    }
}
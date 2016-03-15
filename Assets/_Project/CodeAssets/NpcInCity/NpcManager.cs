using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NpcManager : MonoBehaviour {
    public static NpcManager m_NpcManager;
    public int m_depth = 5;
    public GameObject m_wayHelp;

    public Dictionary<int, NpcObjectItem> m_npcObjectItemDic = new Dictionary<int, NpcObjectItem>();

    public Camera m_camera;

    public Camera m_nguiCamera;

    //public Transform m_talkLayer;

    public UIRoot m_root = null;

    private NpcCityTemplate m_currentNpcTemplate;

    private float m_scale;

    private int HouseId = 0;

    Dictionary<int, Transform> m_currentNpcTransformDic = new Dictionary<int, Transform>();

    Dictionary<int, Transform> m_talkingTransformDic = new Dictionary<int, Transform>();

    private List<GameObject> _listBigHouse = new List<GameObject>();
    private List<GameObject> _listPartHouse = new List<GameObject>();

    LayerMask m_layerMask = 1 << 8;

    Ray m_ray;
    private UICamera.MouseOrTouch m_MouseOrTouch;
    void Awake() {
        m_NpcManager = this;
    }

    void Start() {
        //StartCoroutine( ManualStart() );
    }


    private bool IsNavmeshToTenement = false;
    private bool IsNavmeshToHomeOn = false;
    private bool IsNavmeshToFunction = false;
    private bool IsNavmeshToBigHome = false;
    public bool IsTaskNavMesh = false;

    void Update()
    {
        if (IsNavmeshToTenement)
        {
            if (PlayerModelController.m_playerModelController.m_ObjHero != null)
            {
                IsNavmeshToTenement = false;
                NpcManager.m_NpcManager.setGoToNpc(indexTenement_Enter);
            }
        }

        if (IsNavmeshToHomeOn)
        {
            if (PlayerModelController.m_playerModelController.m_ObjHero != null)
            {
                IsNavmeshToHomeOn = false;
                NpcManager.m_NpcManager.setGoToSelfTenement(CityGlobalData.m_HomeIdSaved);
            }
        }

        if (IsNavmeshToFunction)
        {
            if (PlayerModelController.m_playerModelController.m_ObjHero != null)
            {
                IsNavmeshToFunction = false;
                NpcManager.m_NpcManager.setGoToNpc(CityGlobalData.m_AllianceCityIdSaved);
            }
        }
        if (IsNavmeshToBigHome)
        {
            if (PlayerModelController.m_playerModelController.m_ObjHero != null)
            {
                IsNavmeshToBigHome = false;
                NpcManager.m_NpcManager.setGoToNpc(CityGlobalData.m_AllianceCityIdSaved);
            }
        }
        if (IsTaskNavMesh)
        {
            if (PlayerModelController.m_playerModelController.m_ObjHero != null)
            {
                IsTaskNavMesh = false;
                NpcManager.m_NpcManager.setGoToNpc(CityGlobalData.m_TaskNavID);
            }
        }
    }

    public void ChangePortColor()
    {
        int size_0 = _listPartHouse.Count;
        int size_1 = _listBigHouse.Count;
        for (int i = 0; i < size_0; i++)
        {
            Destroy(_listPartHouse[i]);
        }

        for (int i = 0; i < size_1; i++)
        {
            Destroy(_listBigHouse[i]);
        }
        // m_npcObjectItemDic.Clear();
        _listPartHouse.Clear();
        _listBigHouse.Clear();

        // if (!CityGlobalData.m_isAllianceTenentsScene && JunZhuData.Instance().m_junzhuInfo.lianMengId != 0)
        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        {

            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                // if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId > 50)
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id)
                {
                    HouseId = item.Value.locationId;
                    break;
                }
            }

            Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202), TenementNPCBigHouse);

            // Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100200), TenementNPCPortal);
        }
        //else 
        //{
        //    foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
        //    {
        //        if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId <= 50)
        //        {
        //            HouseId = item.Value.locationId;
        //            break;
        //        }
        //    }
        //  // Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100200), TenementNPCAlliancityPortal);
        //   Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202),TenementNPCTenents);
        //    // CityGlobalData.m_isAllianceTenentsScene = false;
        //}

    }
    private List<NpcCityTemplate> listMainCityTempInfo = new List<NpcCityTemplate>();
    private List<NpcCityTemplate> listAllianceCityTempInfo = new List<NpcCityTemplate>();

    private NpcCityTemplate _tempWorship;
    IEnumerator ManualStart() {
        while (MainCityRoot.Instance().m_objMainUI == null) {
            //			Debug.Log( "NPCManager.ManualStart.Waiting m_objMainUI: " + MainCityRoot.Instance().m_objMainUI );

            yield return new WaitForEndOfFrame();
        }

        m_root = MainCityRoot.Instance().m_objMainUI.GetComponent<UIRoot>();

        m_nguiCamera = Global.GetObj(ref MainCityRoot.Instance().m_objMainUI, "Camera_UI").GetComponent<Camera>();

        //m_talkLayer = Global.GetObj(ref MainCityRoot.Instance().m_objMainUI, "NpcTalkLayer").transform;

        m_scale = m_root.activeHeight / (CityGlobalData.m_ScreenHeight);

        // TODO: replace with new res.
        _listPartHouse.Clear();
        _listBigHouse.Clear();

        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        {
            listMainCityTempInfo.Clear();
            foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
            {
                if (_template.m_Type == 1)
                {
                    listMainCityTempInfo.Add(_template);
                    Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(_template.m_npcShowId),
                                          NPCInMainCityLoadCallback);
                }
            }
        }
        else if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 1 && JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)//if (!CityGlobalData.m_isAllianceTenentsScene && JunZhuData.Instance().m_junzhuInfo.lianMengId != 0)
        {
            listAllianceCityTempInfo.Clear();

            foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
            {
                if (_template.m_Type == 2 && _template.m_npcId < 1000)
                {
                    listAllianceCityTempInfo.Add(_template);
                    Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(_template.m_npcShowId),
                                    TenementNPCInCityLoadCallback);
                }
                //else if (_template.m_Type == 2 && _template.m_npcId == 10000)
                //{
                //    _tempWorship = _template;
                //    Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(_template.m_npcShowId),
                //          WorshipNPCInCityLoadCallback);
                //}
            }
            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                //if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId > 50)
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id)
                {
                    HouseId = item.Value.locationId;
                    break;
                }
            }

            Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202), TenementNPCBigHouse);
            //  Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100200), TenementNPCPortal);
        }
        else if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 2) // if (CityGlobalData.m_isAllianceTenentsScene)
        {
            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId <= 50)
                {
                    HouseId = item.Value.locationId;
                    break;
                }
            }
            Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100200),
                              TenementNPCAlliancityPortal);
            Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100202),
                        TenementNPCTenents);
            // CityGlobalData.m_isAllianceTenentsScene = false;
        }
    }

    public void NPCInMainCityLoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        int size = listMainCityTempInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (ModelTemplate.GetModelIdByPath(p_path) == listMainCityTempInfo[i].m_npcShowId)
            {
                GameObject tempOjbect = Instantiate(p_object) as GameObject;
                tempOjbect.AddComponent<NpcAnimationManagerment>();
                tempOjbect.name = "NpcInCity";

                tempOjbect.transform.parent = this.transform;

                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                tempItem.InitWithNpc(listMainCityTempInfo[i]);

                m_npcObjectItemDic.Add(listMainCityTempInfo[i].m_Id, tempItem);

                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                foreach (PlayerEnterCollider tempCollider in tempEnterList)
                {
                    tempCollider.m_colliser += TriggerEnter;
                }
                listMainCityTempInfo.RemoveAt(i);
                break;
            }
        }
    }

    public void CreateNPC(PrepareForCityLoad.NpcInfo n_Info)
    {
        if (m_root == null)
        {
            m_root = MainCityRoot.Instance().m_objMainUI.GetComponent<UIRoot>();

            m_nguiCamera = Global.GetObj(ref MainCityRoot.Instance().m_objMainUI, "Camera_UI").GetComponent<Camera>();

            m_scale = m_root.activeHeight / (CityGlobalData.m_ScreenHeight);
        }

        GameObject tempOjbect = Instantiate(n_Info._Obj) as GameObject;

        tempOjbect.name = "NpcInCity";
        tempOjbect.AddComponent<NpcAnimationManagerment>();
        tempOjbect.transform.parent = this.transform;

        NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

        tempItem.InitWithNpc(n_Info._NpcTemp);

        m_npcObjectItemDic.Add(n_Info._NpcTemp.m_Id, tempItem);

        PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

        foreach (PlayerEnterCollider tempCollider in tempEnterList)
        {
            tempCollider.m_colliser += TriggerEnter;
        }
    }

    public void CreateHousePortal(PrepareForCityLoad.NpcInfo portal)
    {
        GameObject tempOjbect = null;
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            if (_template.m_Id == 3 && _template.m_Type == 1)
            {
                tempOjbect = Instantiate(portal._Obj) as GameObject;
                tempOjbect.name = "EffectPortal";
                _listBigHouse.Add(tempOjbect);
                tempOjbect.transform.parent = this.transform;
                tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                tempOjbect.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(true);
                tempOjbect.GetComponent<TenementEnterPortal>().m_labName.text = MyColorData.getColorString(4, "[b]" + NameIdTemplate.GetName_By_NameId(_template.m_npcName) + "[/b]");
                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();
                tempItem.InitWithTenementNpc(_template);
                m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();
                foreach (PlayerEnterCollider tempCollider in tempEnterList)
                {
                    tempCollider.m_colliser += TriggerEnter;
                }
                break;
            }
        }
    }
    /*
       public void CreateHousePortal(PrepareForCityLoad.NpcInfo h_Self, PrepareForCityLoad.NpcInfo h_Other)
    {
        int house_id = 0;
        GameObject tempOjbect = null;
        foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
        {
            if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id)
            {
                house_id = item.Value.locationId;
                break;
            }
        }
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            if (_template.m_npcId >= 1001 && _template.m_npcId < 1021)
            {
                if (_template.m_npcId - 1000 == house_id)
                {
                    tempOjbect = Instantiate(h_Self._Obj) as GameObject;
                }
                else
                {
                    tempOjbect = Instantiate(h_Other._Obj) as GameObject;
                }
                tempOjbect.name = "EffectBigHouse";
                _listBigHouse.Add(tempOjbect);
                tempOjbect.transform.parent = this.transform;
                tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                if (TenementData.Instance.m_AllianceCityTenementDic.ContainsKey(_template.m_npcId - 1000))
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = true;
                    tempOjbect.GetComponent<TenementEnterPortal>().m_labName.text = TenementData.Instance.m_AllianceCityTenementDic[_template.m_npcId - 1000].jzName;
                }
                else
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = false;
                }

                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();
                tempItem.InitWithTenementNpc(_template);
                if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                {
                    m_npcObjectItemDic.Remove(_template.m_Id);
                    m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                }
                else
                {
                    m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                }

                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                foreach (PlayerEnterCollider tempCollider in tempEnterList)
                {
                    tempCollider.m_colliser += TriggerEnter;
                }
            }
        }

        //AllianceEffigyManagerment.m_Instance.SetNpcInfo();
    }*/
    void WorshipNPCInCityLoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempOjbect = Instantiate(p_object) as GameObject;

        tempOjbect.name = "NpcInCity";

        tempOjbect.transform.parent = this.transform;
       
        NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

        tempItem.InitWithNpc(_tempWorship);

        m_npcObjectItemDic.Add(_tempWorship.m_Id, tempItem);

        PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

        foreach (PlayerEnterCollider tempCollider in tempEnterList)
        {
            tempCollider.m_colliser += TriggerEnter;
        }

    }
    void TenementNPCInCityLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object )
    {
        int size = listAllianceCityTempInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (ModelTemplate.GetModelIdByPath(p_path) == listAllianceCityTempInfo[i].m_npcShowId)
            {
                GameObject tempOjbect = Instantiate(p_object) as GameObject;
                tempOjbect.AddComponent<NpcAnimationManagerment>();
                tempOjbect.name = "NpcInCity";

                tempOjbect.transform.parent = this.transform;

                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                tempItem.InitWithNpc(listAllianceCityTempInfo[i]);

                m_npcObjectItemDic.Add(listAllianceCityTempInfo[i].m_Id, tempItem);

                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                foreach (PlayerEnterCollider tempCollider in tempEnterList)
                {
                    tempCollider.m_colliser += TriggerEnter;
                }
                listAllianceCityTempInfo.RemoveAt(i);
                break;
            }
        }

        if(CityGlobalData.m_isNavToAllianCity)
        {
            CityGlobalData.m_isNavToAllianCity = false;
           // NpcManager.m_NpcManager.setGoToNpc(CityGlobalData.m_AllianceCityIdSaved);
            IsNavmeshToFunction = true;
        }
    }
    int indexTenement_Enter = 0;
    void TenementNPCPortal(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            if (_template.m_Type == 2 && _template.m_npcId >= 10060 && _template.m_npcId < 10065)
            {
               
                GameObject tempOjbect = Instantiate(p_object) as GameObject;
                if (_template.m_npcId == 10060)
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = 0;
                }
                else if (_template.m_npcId == 10061)
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = 1;
                }
                else if (_template.m_npcId == 10062)
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = 2;
                }
                else if (_template.m_npcId == 10063)
                {
                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = 3;
                }

                tempOjbect.name = "TransferPortal";

                tempOjbect.transform.parent = this.transform;
                //  tempOjbect.transform.localScale = new Vector3(35, 5, 26);
                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                tempItem.InitWithTenementNpc(_template);

                m_npcObjectItemDic.Add(_template.m_Id, tempItem);

                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                foreach (PlayerEnterCollider tempCollider in tempEnterList)
                {
                    tempCollider.m_colliser += TriggerEnter;
                }
            }
           
        }
       // Debug.Log("m_isNavToAllianCityToTenement.m_isNavToAllianCityToTenement.m_isNavToAllianCityToTenement ::" + CityGlobalData.m_isNavToAllianCityToTenement);
        if (CityGlobalData.m_isNavToAllianCityToTenement)
        {
            CityGlobalData.m_isNavToAllianCityToTenement = false;
   
            if (CityGlobalData.m_AllianceCityIdSaved < 1013)
            {
                indexTenement_Enter = 10060;
            }
            else if (CityGlobalData.m_AllianceCityIdSaved >= 1013 && CityGlobalData.m_AllianceCityIdSaved < 1025)
            {
                indexTenement_Enter = 10061;
            }
            else if (CityGlobalData.m_AllianceCityIdSaved >= 1024 && CityGlobalData.m_AllianceCityIdSaved < 1037)
            {
                indexTenement_Enter = 10062;
            }
            else
            {
                indexTenement_Enter = 10063;
            }
            IsNavmeshToTenement = true;
          //  NpcManager.m_NpcManager.setGoToTenementNpc(index_Enter);
        }
    }
    private NpcCityTemplate EffectBigHouse = null;
    void TenementNPCBigHouse(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            // if ( _template.m_Type == 2 && _template.m_npcId >= 1101 && _template.m_npcId < 1106)
            if (_template.m_npcId >= 1001 && _template.m_npcId < 1021)
            {
                if (_template.m_npcId - 1000 == HouseId)
                {
                    EffectBigHouse = _template;
                    Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100201),
                           TenementNPCSelfTenents);
                }
                else
                {
                    GameObject tempOjbect = Instantiate(p_object) as GameObject;

                    tempOjbect.name = "EffectBigHouse";
                    _listBigHouse.Add(tempOjbect);
                    tempOjbect.transform.parent = this.transform;
                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                    if (TenementData.Instance.m_AllianceCityTenementDic.ContainsKey(_template.m_npcId - 1000))
                    {
                        tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = true;
                        tempOjbect.GetComponent<TenementEnterPortal>().m_labName.text = TenementData.Instance.m_AllianceCityTenementDic[_template.m_npcId - 1000].jzName;
                    }
                    else
                    {
                        tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = false;
                    }
                    //  tempOjbect.transform.localScale = new Vector3(35, 5, 26);
                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                    tempItem.InitWithTenementNpc(_template);

                    if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                    {
                        m_npcObjectItemDic.Remove(_template.m_Id);
                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                    }
                    else
                    {
                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                    }
     
                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                    {
                        tempCollider.m_colliser += TriggerEnter;
                    }
                }
            
            }
             
        }
        if (CityGlobalData.m_isNavToHouse)
        {
            CityGlobalData.m_isNavToHouse = false;
         //   IsNavmeshToBigHome = true;
        }
    }
    void TenementNPCSelfTenents(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        NpcCityTemplate _template = new NpcCityTemplate();
        if (EffectBigHouse != null)
        {
            _template = EffectBigHouse;
            EffectBigHouse = null;
        }
        else
        {
            _template = CountryHouse;
        }
        GameObject tempOjbect = Instantiate(p_object) as GameObject;
        _listBigHouse.Add(tempOjbect);
        tempOjbect.name = "EffectBigHouse";
        tempOjbect.transform.parent = this.transform;
        tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
        if (TenementData.Instance.m_AllianceCityTenementDic.ContainsKey(_template.m_npcId - 1000))
        {
            tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = true;
            tempOjbect.GetComponent<TenementEnterPortal>().m_labName.text = TenementData.Instance.m_AllianceCityTenementDic[_template.m_npcId - 1000].jzName;
        }
        else
        {
            tempOjbect.GetComponent<TenementEnterPortal>().m_labName.enabled = false;
        }
        NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();
        tempItem.InitWithTenementNpc(_template);
        if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
        {
            m_npcObjectItemDic.Remove(_template.m_Id);
            m_npcObjectItemDic.Add(_template.m_Id, tempItem);
        }
        else
        {
            m_npcObjectItemDic.Add(_template.m_Id, tempItem);
        }


        PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

        foreach (PlayerEnterCollider tempCollider in tempEnterList)
        {
            tempCollider.m_colliser += TriggerEnter;
        }
      //  Debug.Log("000000000000000000000000000000000000000000000000000000000000000000000000000");

       // Debug.Log("CityGlobalData.m_isNavToHomeCityGlobalData.m_isNavToHome ::" + CityGlobalData.m_isNavToHome);
     //   Debug.Log("CityGlobalData.m_HomeIdSavedCityGlobalData.m_HomeIdSavedCityGlobalData.m_HomeIdSaved " + CityGlobalData.m_HomeIdSaved);
        if (CityGlobalData.m_isNavToHome)
        {
           
         //   Debug.Log("CityGlobalData.m_HomeIdSavedCityGlobalData.m_HomeIdSaved :: " + CityGlobalData.m_HomeIdSaved);
            CityGlobalData.m_isNavToHome = false;
           // NpcManager.m_NpcManager.setGoToSelfTenement(CityGlobalData.m_HomeIdSaved);
            //IsNavmeshToHomeOn = true;
        }
    }

    void TenementNPCAlliancityPortal(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            if (_template.m_Type == 3 && _template.m_npcId  == 1151)
            {
                 
                    GameObject tempOjbect = Instantiate(p_object) as GameObject;

                    tempOjbect.name = "AllianceCityPortal";

                    tempOjbect.transform.parent = this.transform;
               // TenementPortal
                    //  tempOjbect.transform.localScale = new Vector3(35, 5, 26);
                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                    tempItem.InitWithTenementNpc(_template);

           
                    m_npcObjectItemDic.Add(_template.m_Id, tempItem);

                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                    {
                        tempCollider.m_colliser += TriggerEnter;
                    }
               }
        }
    }

    NpcCityTemplate CountryHouse = null;

    void TenementNPCTenents(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        //Debug.Log("  CityGlobalData.m_iAllianceTenentsSceneNum  CityGlobalData.m_iAllianceTenentsSceneNum " + CityGlobalData.m_iAllianceTenentsSceneNum);
        foreach (NpcCityTemplate _template in NpcCityTemplate.m_templates)
        {
            if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0 && _template.m_Type == 3)
            {
                if (HouseId == _template.m_npcId - 1000)
                {
                    //Debug.Log("00000000000000000000000000000000000000000000000000");
                  //  Debug.Log("_template.m_npcId_template.m_npcId   :::::::::::::" + _template.m_npcId);
                    int index_Enter = 0;
                    if (_template.m_npcId < 1013)
                    {
                        index_Enter = 0;
                    }
                    else if (_template.m_npcId >= 1013 && _template.m_npcId < 1025)
                    {
                        index_Enter = 1;
                    }
                    else if (_template.m_npcId >= 1024 && _template.m_npcId < 1037)
                    {
                        index_Enter = 2;
                    }
                    else
                    {
                        index_Enter = 3;
                    }
                    CountryHouse = _template;
                    //Debug.Log("HouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseIdHouseId ::" + HouseId);
                    //Debug.Log("CityGlobalData.m_iAllianceTenentsSceneNum CityGlobalData.m_iAllianceTenentsSceneNum ::" + CityGlobalData.m_iAllianceTenentsSceneNum);
                    //Debug.Log("index_Enterindex_Enterindex_Enterindex_Enterindex_Enterindex_Enterindex_Enterindex_Enterindex_Enter ::" + index_Enter);

                    if (CityGlobalData.m_iAllianceTenentsSceneNum == index_Enter)
                    {
                        Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(100201),
                               TenementNPCSelfTenents);
                    }

                }
                else
                {
                    switch (CityGlobalData.m_iAllianceTenentsSceneNum)
                    {
                        case 0:
                            {
                                if (_template.m_npcId <= 1012)
                                {
                                    GameObject tempOjbect = Instantiate(p_object) as GameObject;

                                    tempOjbect.name = "EffectPortal";
                                    _listPartHouse.Add(tempOjbect);
                                    tempOjbect.transform.parent = this.transform;
                                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                                    //  tempOjbect.transform.localScale = new Vector3(35, 5, 26);
                                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                                    tempItem.InitWithTenementNpc(_template);
                                    //Debug.Log("_template.m_Id_template.m_Id_template.m_Id ::" + _template.m_Id);
                                    if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                                    {
                                        m_npcObjectItemDic.Remove(_template.m_Id);
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }
                                    else
                                    {
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }

                                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                                    {
                                        tempCollider.m_colliser += TriggerEnter;
                                    }

                                }
                            }
                            break;
                        case 1:
                            {
                                if (_template.m_npcId > 1012 && _template.m_npcId <= 1024)
                                {
                                    GameObject tempOjbect = Instantiate(p_object) as GameObject;

                                    tempOjbect.name = "EffectPortal";
                                    _listPartHouse.Add(tempOjbect);
                                    tempOjbect.transform.parent = this.transform;
                                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                                    tempItem.InitWithTenementNpc(_template);
                                    //Debug.Log("_template.m_Id_template.m_Id_template.m_Id ::" + _template.m_Id);
                                    if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                                    {
                                        m_npcObjectItemDic.Remove(_template.m_Id);
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }
                                    else
                                    {
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }

                                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                                    {
                                        tempCollider.m_colliser += TriggerEnter;
                                    }

                                }
                            }
                            break;
                        case 2:
                            {
                                if (_template.m_npcId > 1026 && _template.m_npcId <= 1036)
                                {
                                    GameObject tempOjbect = Instantiate(p_object) as GameObject;

                                    tempOjbect.name = "EffectPortal";
                                    _listPartHouse.Add(tempOjbect);
                                    tempOjbect.transform.parent = this.transform;
                                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                                    tempItem.InitWithTenementNpc(_template);
                                   // Debug.Log("_template.m_Id_template.m_Id_template.m_Id ::" + _template.m_Id);

                                    if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                                    {
                                        m_npcObjectItemDic.Remove(_template.m_Id);
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }
                                    else
                                    {
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }
                                   

                                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                                    {
                                        tempCollider.m_colliser += TriggerEnter;
                                    }

                                }
                            }
                            break;
                        case 3:
                            {
                                if (_template.m_npcId > 1036 && _template.m_npcId <= 1050)
                                {
                                    GameObject tempOjbect = Instantiate(p_object) as GameObject;
                                    _listPartHouse.Add(tempOjbect);
                                    tempOjbect.name = "EffectPortal";

                                    tempOjbect.transform.parent = this.transform;
                                    tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                                    //  tempOjbect.transform.localScale = new Vector3(35, 5, 26);
                                    NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

                                    tempItem.InitWithTenementNpc(_template);
                                    //Debug.Log("_template.m_Id_template.m_Id_template.m_Id ::" + _template.m_Id);
                                    if (m_npcObjectItemDic.ContainsKey(_template.m_Id))
                                    {
                                        m_npcObjectItemDic.Remove(_template.m_Id);
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }
                                    else
                                    {
                                        m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                                    }

                                    PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                                    foreach (PlayerEnterCollider tempCollider in tempEnterList)
                                    {
                                        tempCollider.m_colliser += TriggerEnter;
                                    }

                                }
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }

    }

		// npc读表
    void NPC_Inite(){ 
    
    }

	/*void UpdateNpcName() //更新玩家名字位置和角度
	{
		foreach(NpcObjectItem tempItem in m_npcObjectItemDic.Values)
		{
			tempItem.m_nameLabel.transform.LookAt(m_camera.transform);
		}
	}*/

    private Transform m_trigger_transform = null; 

	/// npc有两个碰撞提 进入外圈 显示npc对话 进入npc内圈 显示功能性ui
	void TriggerEnter(int tempColliderIndex,bool tempEnterState,Transform tempCollider,Transform tempTransform){
		if(tempCollider.name != "Myself") return; //不是跟自己碰撞返回
		
		NpcCityTemplate t_npc_city_template = tempTransform.GetComponent<NpcObjectItem>().m_template;
		
		if(t_npc_city_template.m_dialog1 == null) return; //无气泡提示的npc
		
		if(tempColliderIndex == 0){
			if(tempEnterState == true){
				//Debug.Log("进入内圈");
			}
			else{
				//Debug.Log("退出内圈");
			}
			
		}else
		{
			if(tempEnterState == true)
			{
				//Debug.Log("进入外圈");
				
				if(m_currentNpcTransformDic.ContainsKey(t_npc_city_template.m_Id)) return;

				m_trigger_transform = tempTransform;

				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.UI_MAINCITY_NPC_TALK ),
				                        NPCTalkLoadCallback );
			}else
			{
				GameObject tempObject = m_talkingTransformDic[t_npc_city_template.m_Id].gameObject;
				
				m_talkingTransformDic.Remove(t_npc_city_template.m_Id);
				
				Destroy(tempObject);
				
				m_currentNpcTransformDic.Remove(t_npc_city_template.m_Id);
				
				//Debug.Log("退出外圈");
			}
		}
	}
	
	public void NPCTalkLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		GameObject tempObject = Instantiate( p_object ) as GameObject;

		NpcCityTemplate t_npc_City_template = m_trigger_transform.GetComponent<NpcObjectItem>().m_template;


		//tempObject.transform.parent = m_talkLayer;
		
		tempObject.transform.localScale = Vector3.one;
		
		tempObject.transform.localPosition = Vector3.zero;
		
		tempObject.transform.GetComponent<NpcTalkWithAbout>().m_talkDescribe.text = t_npc_City_template.m_dialog2;
		
		m_currentNpcTransformDic.Add( t_npc_City_template.m_Id, m_trigger_transform );
		
		m_talkingTransformDic.Add( t_npc_City_template.m_Id,tempObject.transform );
	}

	void ShowNpcLayer() //显示UI方法
	{
		//Debug.Log("调用显示UI的方法");

        if (MainCityUI.IsWindowsExist()) return;
		
		if( m_currentNpcTemplate != null ){
			//Debug.Log("现在显示的UI： " + (m_currentNpcTemplate.m_npcName - CityGlobalData.m_npcBaseNum));
			
			switch (m_currentNpcTemplate.m_npcName - CityGlobalData.m_npcBaseNum)
			{
			case 3:
			{
				//弹出装备洗练 pve等
				break;
			}
			case 1:
			{
				break;
			}
			default:break;
			}
		}
	}

 
    void LateUpdate()
	{
        //if(UIYindao.m_UIYindao != null)
        //{
        if (UIYindao.m_UIYindao.m_isQiangzhi)
        {
            return;
        }
        //}
        if (m_talkingTransformDic.Count > 0) //
		{
			foreach(int tempKey in m_currentNpcTransformDic.Keys)
			{
				Vector3 tempPosition = m_currentNpcTransformDic[tempKey].position;
				
				Vector3 tempPosition1 = m_camera.WorldToScreenPoint(new Vector3(tempPosition.x,tempPosition.y + 5.0f,tempPosition.z));
				
				tempPosition1.y -= CityGlobalData.m_HalfScreenHeight;
				
				tempPosition1.x -= CityGlobalData.m_HalfScreenWidth;
				
				m_talkingTransformDic[tempKey].transform.localPosition = new Vector3(tempPosition1.x * m_scale,tempPosition1.y * m_scale,tempPosition1.z);
			}
		}
 
        if ( MainCityUI.m_MainCityUI == null )
        {
			return;
		}
 
        if (QXChatData.Instance.SetOpenChat || CityGlobalData.m_joystickControl || MainCityUI.IsWindowsExist() || JunZhuLevelUpManagerment.m_JunZhuLevelUp != null  || TaskSignalInfoShow.m_TaskSignal != null) return; //现在在操纵摇杆 or 有ui界面弹出  npc不响应点击事件
		{
            if (Input.GetMouseButton(0))
			{
                //if (MainCityUI.IsWindowsExist() || UIYindao.m_UIYindao.m_isOpenYindao
                if (MainCityUI.IsWindowsExist())
                {
                    return;
                }
                Vector3 tempMousePosition = Input.mousePosition;
                m_ray = m_nguiCamera.ScreenPointToRay(tempMousePosition);// 从屏幕发射线
				RaycastHit nguiHit;

                if (Physics.Raycast(m_ray, out nguiHit)) return;//碰到主界面UI按钮

                m_ray = Camera.main.ScreenPointToRay(tempMousePosition);

                RaycastHit hit;
                int t_index = LayerMask.NameToLayer("CityRoles");

                m_depth  = 1 << t_index;
 
                if (Physics.Raycast(m_ray, out hit, 1000.0f, m_depth))
                {
                    if (hit.collider.transform.name.IndexOf("PlayerObject") > -1)
                    {
                        m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
                        EquipSuoData.CreateChaKan(hit.collider.transform.name, m_MouseOrTouch.pos);
                       return;
                    }
                }
                int  t_index2 = LayerMask.NameToLayer("Default");

                int  depth2 = 1 << t_index2;

                if (Physics.Raycast(m_ray,out hit, 10000.0f, depth2)) //碰到3d世界中的npc
				{
					m_currentNpcTemplate = null;
                    if (hit.collider.transform.name == "NpcInCity" || hit.collider.transform.name == "EffectPortal")
                    {
                        PlayerModelController.m_playerModelController.m_agent.enabled = true;
                        m_currentNpcTemplate = hit.collider.transform.GetComponent<NpcObjectItem>().m_template;
  
                        PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
                        if (Vector3.Distance(hit.collider.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
                        {
                            PlayerModelController.m_playerModelController.SelfNavigation(hit.collider.gameObject.transform.position);
                        }
                        else
                        {
                            //  Debug.Log("m_currentNpcTemplate.m_npcId m_currentNpcTemplate.m_npcId  ::" + m_currentNpcTemplate.m_npcId);
                            if (m_currentNpcTemplate.m_npcId == 801)
                            {
                                NewEmailData.Instance().OpenEmail(0);
                            }
                            else
                            {
                                PlayerModelController.m_playerModelController.TidyNpcInfo();
                            }
                        }

                    }
                   
                    {
                        if (PlayerModelController.m_playerModelController.AddFunaction())
                        {
                            PlayerModelController.m_playerModelController.m_showLayer += ShowNpcLayer;
                        }
                    }
                }
			}
		}
	}
    public void EmailLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    public void setGoToNpc(int id)
    {

        {
            if (m_npcObjectItemDic.ContainsKey(id))
            {
                NpcObjectItem temp = m_npcObjectItemDic[id];

                m_currentNpcTemplate = temp.m_template;
                PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
                if (m_currentNpcTemplate.m_npcId != 10000 && Vector3.Distance(temp.gameObject.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
                {
                    PlayerModelController.m_playerModelController.SelfNavigation(temp.gameObject.transform.position);
                }
                else
                {
                    if (m_currentNpcTemplate.m_npcId == 801)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EMAIL),
                              EmailLoadCallback);
                    }
                    else
                    {
                        PlayerModelController.m_playerModelController.TidyNpcInfo();
                    }
                }

                {
                    if (PlayerModelController.m_playerModelController.AddFunaction())
                    {
                        PlayerModelController.m_playerModelController.m_showLayer += ShowNpcLayer;
                    }
                }
            }
        }
       
	}

    public void setGoToSelfTenement(int id)
    {
  
        //if (m_npcObjectItemDic.ContainsKey(id))
        //{
        //    NpcObjectItem temp = m_npcObjectItemDic[id];

        //    m_currentNpcTemplate = temp.m_template;
        //    PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
   
        //    if (Vector3.Distance(temp.gameObject.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
        //    {
        //        PlayerModelController.m_playerModelController.SelfNavigation(temp.gameObject.transform.position);
        //    }
        //    else
        //    {
        //        if ((m_currentNpcTemplate.m_npcId > 1000 || m_currentNpcTemplate.m_npcId == 10000) && m_currentNpcTemplate.m_npcId != 1151 && m_currentNpcTemplate.m_npcId < 10060)
        //        {
        //            PlayerModelController.m_playerModelController.TidyTenementNpcInfo();
        //        }
        //        else if (FunctionOpenTemp.GetTypeByNpcId(m_currentNpcTemplate.m_npcId) > 0)
        //        {
        //            PlayerModelController.m_playerModelController.TidyNpcInfo();
        //        }
        //    }

        //    {
        //        if (PlayerModelController.m_playerModelController.AddFunaction())
        //        {
        //            PlayerModelController.m_playerModelController.m_showLayer += ShowNpcLayer;
        //        }
        //    }
        //}

    }

    public void setGoToTenementNpc(int id)
    {
        CityGlobalData.m_HomeIdSaved = id;
        //int infectId = 0;
        //if (id >= 1001 && id < 1013)
        //{
        //    infectId = 10060;
        //}
        //else if (id >= 1013 && id < 1025)
        //{
        //    infectId = 10061;
        //}
        //else if (id >= 1024 && id < 1037)
        //{
        //    infectId = 10062;
        //}
        //else if (id >= 1037 && id < 1051)
        //{
        //    infectId = 10063;
        //}
        //if (m_npcObjectItemDic.ContainsKey(infectId))
 
        //if (m_npcObjectItemDic.ContainsKey(id))
        //{
        //    NpcObjectItem temp = m_npcObjectItemDic[id];
        //    m_currentNpcTemplate = temp.m_template;
        //    PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
        //    if (Vector3.Distance(temp.gameObject.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 3)
        //    {
        //        PlayerModelController.m_playerModelController.SelfNavigation(temp.gameObject.transform.position);
        //    }
        //    else
        //    {
        //    //    Debug.Log("1111111111111111111111111111111111111111111");
        //        //if (infectId >= 10060)
        //        //{
        //        //  //  Debug.Log("22222222222222222222222222222222222222222222");
        //        //    CityGlobalData.m_isAllianceTenentsScene = true;
        //        //    SceneManager.EnterAllianceCityTenentsCityOne();
        //        //}
        //        //else
        //        {
        //            //   Debug.Log("3333333333333333333333333333333333333333333333");
 
        //            PlayerModelController.m_playerModelController.TidyTenementNpcInfo(); 
        //        }
        //    }

        //    {
        //        if (PlayerModelController.m_playerModelController.AddFunaction())
        //        {
        //            PlayerModelController.m_playerModelController.m_showLayer += ShowNpcLayer;
        //        }
        //    }
        //}
    }
}
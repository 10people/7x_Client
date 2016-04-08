using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NpcManager : MonoBehaviour
{
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

    LayerMask m_layerMask = 1 << 8;

    Ray m_ray;
    private UICamera.MouseOrTouch m_MouseOrTouch;
    void Awake()
    {
        m_NpcManager = this;
    }

    void Start()
    {
        //StartCoroutine( ManualStart() );
    }

    void OnDestroy()
    {
        m_NpcManager = null;
    }


    private bool IsNavmeshToTenement = false;
    private bool IsNavmeshToHomeOn = false;
    private bool IsNavmeshToFunction = false;
    private bool IsNavmeshToBigHome = false;
    public bool IsTaskNavMesh = false;

    private List<NpcCityTemplate> listMainCityTempInfo = new List<NpcCityTemplate>();
    private List<NpcCityTemplate> listAllianceCityTempInfo = new List<NpcCityTemplate>();

    private NpcCityTemplate _tempWorship;


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
        tempOjbect.AddComponent<ModelTouchShowOrMoveManagerment>();
        tempOjbect.transform.parent = this.transform;

        NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();

        tempItem.InitWithNpc(n_Info._NpcTemp);

        m_npcObjectItemDic.Add(n_Info._NpcTemp.m_Id, tempItem);

        PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();
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
                tempOjbect.transform.parent = this.transform;
                tempOjbect.GetComponent<TenementEnterPortal>().m_indexNum = _template.m_npcId;
                tempOjbect.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(true);
                tempOjbect.AddComponent<ModelTouchShowOrMoveManagerment>();
                tempOjbect.GetComponent<TenementEnterPortal>().m_labName.text = MyColorData.getColorString(4, "[b]" + NameIdTemplate.GetName_By_NameId(_template.m_npcName) + "[/b]");
                NpcObjectItem tempItem = tempOjbect.GetComponent<NpcObjectItem>();
                tempItem.InitWithTenementNpc(_template);

                m_npcObjectItemDic.Add(_template.m_Id, tempItem);
                PlayerEnterCollider[] tempEnterList = tempOjbect.GetComponentsInChildren<PlayerEnterCollider>();

                break;
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
        }
    }
}
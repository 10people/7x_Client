using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipGrowthShowInfoManagerment : MonoBehaviour
{
    private static GameObject IconSamplePrefab;
    public List<GameObject> IconParentObjectList = new List<GameObject>();
    private List<IconSampleManager> iconSampleManagerList = new List<IconSampleManager>();

    public GameObject m_Intensify;
    public GameObject m_Wash;
    public UILabel m_LabTitle;

    public GameObject m_TaoZhuangLayer;
    public GameObject m_PlayerParent;
    public GameObject m_EquipOfBody;

    public GameObject m_EquipInfo;
    public int selectedType;
    private List<string> m_role_model_path = new List<string>();

    private readonly List<Vector3> listPos = new List<Vector3>
    {
        new Vector3(-245, -215, -100),
        new Vector3(-245, -214, -100),
        new Vector3(-245, -214, -100),
        new Vector3(-245, -259, -100)
    };

    private bool isInitialized = false;

    private void Initialize()
    {
        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
        else
        {
            WWW temp = null;
            OnIconSampleLoadCallBack(ref temp, null, IconSamplePrefab);
        }
    }

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        iconSampleManagerList.Clear();
        foreach (GameObject item in IconParentObjectList)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.transform.parent = item.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;

            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManagerList.Add(iconSampleManager);
        }

        if (!isInitialized)
        {
            isInitialized = true;
            ShowSharedPartsInfo(m_type);
        }
        else
        {
            isInitialized = true;
        }
    }

    void Start()
    {
        m_EquipOfBody.SetActive(true);

        // TODO: replace with new res.
        {
            // HaoJie
            m_role_model_path.Add(ModelTemplate.GetResPathByModelId(-4));

            // RuYa
            m_role_model_path.Add(ModelTemplate.GetResPathByModelId(-5));

            // YuJie
            m_role_model_path.Add(ModelTemplate.GetResPathByModelId(-6));

            // LuoLi
            m_role_model_path.Add(ModelTemplate.GetResPathByModelId(-7));
        }

        ShowPlayer();
    }

    private int m_type;
    private const int BasicIconSampleDepth = 5;

    private void NullVoidDelegate(GameObject go)
    {

    }

    public void ShowSharedPartsInfo(int type)
    {
        if (!isInitialized)
        {
            m_type = type;
            Initialize();
            return;
        }

        selectedType = type;
        switch (type)
        {
            case 0:
                {
                    m_LabTitle.text = NameIdTemplate.GetName_By_NameId(990041);
                    m_Intensify.SetActive(true);
                    m_Wash.SetActive(false);
                }
                break;
            case 1:
                {
                    m_LabTitle.text = NameIdTemplate.GetName_By_NameId(990042);
                    m_Intensify.SetActive(false);
                    m_Wash.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("Not processed type:" + type);
                break;
        }

        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;

        //Ergodic all equipment holes.
        for (int i = 0; i < iconSampleManagerList.Count; i++) //初始化玩家背包scrollview的item
        {
            iconSampleManagerList.ForEach(item =>
            {
                item.NguiLongPress.NormalPressTriggerWhenLongPress = true;
                item.NguiLongPress.OnNormalPress = SelectEquip;
            });

            //if equipment exist.
            if (tempEquipsOfBodyDic.ContainsKey(i))
            {
                iconSampleManagerList[i].FgSprite.gameObject.SetActive(true);

                var equipmentTemp = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId);
                if (equipmentTemp == null)
                {
                    Debug.LogError("Can't set icon sample cause equipment:" + tempEquipsOfBodyDic[i].itemId + " not found");
                    return;
                }

                var fgSpriteName = !string.IsNullOrEmpty(equipmentTemp.icon) ? equipmentTemp.icon : "";
                var qualityFrameSpriteName = equipmentTemp.color != 0
                    ? IconSampleManager.QualityPrefix + (equipmentTemp.color - 1)
                    : "";

                if (EquipInfoCharge.CanImproveQuality(tempEquipsOfBodyDic[i].itemId,
                    JunZhuData.Instance().m_junzhuInfo.level))
                {
                    iconSampleManagerList[i].SetIconType(IconSampleManager.IconType.equipment);
                    iconSampleManagerList[i].SetIconBasic(BasicIconSampleDepth, fgSpriteName, "", qualityFrameSpriteName);
                    iconSampleManagerList[i].SetIconButtonDelegate(NullVoidDelegate);
                }
                else
                {
                    iconSampleManagerList[i].SetIconType(IconSampleManager.IconType.equipment);
                    iconSampleManagerList[i].SetIconBasic(BasicIconSampleDepth, fgSpriteName, "", qualityFrameSpriteName);
                }
            }
            //equipment not exist.
            else
            {
                if (EquipInfoCharge.IsEquipmentInBag(i))
                {
                    iconSampleManagerList[i].SetIconType(IconSampleManager.IconType.equipment);
                    iconSampleManagerList[i].SetIconBasic(BasicIconSampleDepth);
                    iconSampleManagerList[i].SetIconButtonDelegate(null, NullVoidDelegate);
                }
                else
                {
                    iconSampleManagerList[i].SetIconType(IconSampleManager.IconType.equipment);
                    iconSampleManagerList[i].SetIconBasic(BasicIconSampleDepth);
                }
            }
        }
    }

    private void SelectEquip(GameObject tempObject)
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {

            if (FreshGuide.Instance().IsActive(100050) && TaskData.Instance.m_TaskInfoDic[100050].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_TaskInfoDic[100060].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100060];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }

            else if (FreshGuide.Instance().IsActive(100125) && TaskData.Instance.m_TaskInfoDic[100125].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100125];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
            {
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else
            {
                CityGlobalData.m_isRightGuide = true;
            }

        }
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        int index = iconSampleManagerList.IndexOf(tempObject.GetComponent<IconSampleManager>());
        if (tempEquipsOfBodyDic.ContainsKey(index))
        {
            m_EquipInfo.SetActive(true);
            m_EquipOfBody.SetActive(false);
            m_EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().
                GetEquipInfo(tempEquipsOfBodyDic[index].itemId,
                tempEquipsOfBodyDic[index].dbId,
                selectedType,
                index);
        }
    }

    void ShowPlayer()
    {
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
                                            LoadCallBack);
    }

    public void LoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;

        tempObject.transform.parent = m_PlayerParent.transform;

        tempObject.SetActive(true);

        tempObject.name = m_role_model_path[CityGlobalData.m_king_model_Id - 1];
        tempObject.transform.localPosition = listPos[CityGlobalData.m_king_model_Id - 1];
        tempObject.name = p_object.name;

        // m_PlayerModel.name = m_role_model_path[CityGlobalData.m_king_model_Id - 1];

        tempObject.transform.localPosition = listPos[CityGlobalData.m_king_model_Id - 1];
        tempObject.GetComponent<NavMeshAgent>().enabled = false;
   
       tempObject.GetComponent<Animator>().Play("zhuchengdile");
             


        tempObject.transform.localScale = new Vector3(210, 210, 210);

        tempObject.transform.localRotation = new Quaternion(0, -180, 0, 0);
    
    }
}

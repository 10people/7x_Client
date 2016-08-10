using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipAdvanceLayerManagerment : MonoBehaviour {

    public static EquipAdvanceLayerManagerment m_EquipAdvance;
    public UILabel m_LabGong;
    public UILabel m_LabFang;
    public UILabel m_LabMing;
    public UILabel m_LabTitle;

    public GameObject m_ParentLeft;
    public GameObject m_ParentRight;
    public EventHandler m_Handler;

    public GameObject m_HiddenObj;
    public GameObject m_LabParent1;
    public GameObject m_LabParent2;
    public UILabel m_labObj;
    private UICamera m_Camera = null;
    private List<int> _listReward = new List<int>();
   
    void Awake()
    {
        if (MainCityUI.m_MainCityUI)
        {
            int size = MainCityUI.m_MainCityUI.m_WindowObjectList.Count;
            if (size > 0)
            {
                m_Camera = MainCityUI.m_MainCityUI.m_WindowObjectList[size - 1].GetComponentInChildren<UICamera>();
                if (m_Camera != null)
                    EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, true);
            }
            else
            {
                UI2DTool.Instance.AddTopUI(gameObject);
            }
        }
            m_EquipAdvance = this;

    }

    void sdssd(float f)
    {
        m_labObj.color = new Color(m_labObj.color.r, m_labObj.color.g, m_labObj.color.b, f);
    }
    void Start ()
    {
      //  UIYindao.m_UIYindao.CloseUI();
        m_Handler.m_click_handler += DestroyInfo;
        _listReward.Clear();
        CycleTween.StartCycleTween(m_labObj.gameObject, 1, 0.4f, 0.5f, sdssd);
        //ShowInfo(FunctionWindowsCreateManagerment.m_AdvanceInfo);
    }

    void DestroyInfo(GameObject obj)
    {
        Destroy(gameObject);
    }
    public void ShowInfo(FunctionWindowsCreateManagerment.EquipAdvanceInfo info)
    {
        _listReward.Add(info._equipid);
        _listReward.Add(info._nextid);
        if (info._gongadd > 0)
        {
            m_LabParent1.SetActive(true);
            m_LabParent2.SetActive(false);
            m_LabGong.text = MyColorData.getColorString(9,info._gong.ToString() + " → " + (info._gong + info._gongadd).ToString() +" (" ) + MyColorData.getColorString(4,"+" + info._gongadd.ToString()) + MyColorData.getColorString(9, "↑)");
        }
        else//↑
        {
            m_LabParent1.SetActive(false);
            m_LabParent2.SetActive(true);
            m_LabFang.text = MyColorData.getColorString(9, info._fang.ToString() + " → " + (info._fang + info._fanggadd).ToString() + " (") + MyColorData.getColorString(4, "+" + info._fanggadd) + MyColorData.getColorString(9, "↑)");
            m_LabMing.text = MyColorData.getColorString(9, info._ming.ToString() + " → " + (info._ming + info._minggadd).ToString() + " (") + MyColorData.getColorString(4, "+" + info._minggadd ) + MyColorData.getColorString(9, "↑)");
        }
        for (int i = 0; i < _listReward.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                            ResLoadedSimple);
        }
    }
    private int _indexNum = 0;
    void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_ParentLeft != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            if (_indexNum == 0)
            {
                tempObject.transform.parent = m_ParentLeft.transform;
            }
            else
            {
                tempObject.transform.parent = m_ParentRight.transform;
            }
            tempObject.name = _indexNum.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            if (_indexNum == 0)
            {
                iconSampleManager.SetIconByID(_listReward[_indexNum], "", 10);
            }
            else
            {
                iconSampleManager.SetIconByID(_listReward[_indexNum], "", 10, false, true);
            }
            
            iconSampleManager.SetIconPopText(_listReward[_indexNum], NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum]).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum]).descId));
            if (_indexNum < _listReward.Count - 1)
            {
                _indexNum++;
            }
            else
            {
                m_HiddenObj.SetActive(true);
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_LabTitle.gameObject, EffectIdTemplate.GetPathByeffectId(620214), null);
            }
        }
        else
        {
            p_object = null;
        }
    }
    void OnDestroy()
    {
        JunZhuZhuangBeiInfo.m_isJinJie = false;
        m_EquipAdvance = null;
        if (m_Camera != null)
            EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, false);

        FunctionWindowsCreateManagerment.m_IsEquipAdvance = false;
        UI3DEffectTool.ClearUIFx(m_LabTitle.gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class EquipGrowthGemSelectItemManagerment : MonoBehaviour {
    public UILabel m_LabName;
    public UILabel m_LabNameNum;
    public UILabel m_LabAttribute;
    public UILabel m_LabAttributeNum;
    public UILabel m_LabExp;
    public UILabel m_LabExpNum;
    public UILabel m_LabButtonTitle;
    public delegate void dele_Inlay(long index);
    public EventIndexHandle m_Event;
    dele_Inlay On_touchInlay;
    public GameObject m_IconParent;
    private long _gemDBId;
    private int _gemId = 0;
    void Start()
    {
        m_Event.m_Handle += TouchEvent;
    }

    void TouchEvent(int index)
    {
        if (On_touchInlay != null)
        {
            On_touchInlay(_gemDBId);
        }
    }
    int exp = 0;
    public void ShowInfo(EquipGrowthInlayLayerManagerment.GemSelectInfo gemInfo, dele_Inlay callBack)
    {
        exp = gemInfo._Exp;
        if (gemInfo._isSwitch)
        {
            m_LabButtonTitle.text = "替换";
        }
        else
        {
            m_LabButtonTitle.text = "镶嵌";
        }
        On_touchInlay = callBack;
        _gemDBId = gemInfo._dbid;
        _gemId = gemInfo._Gemid;
        m_LabName.text = NameIdTemplate.GetName_By_NameId(FuWenTemplate.GetFuWenTemplateByFuWenId(gemInfo._Gemid).name);
        m_LabNameNum.text = "X" +gemInfo._Count;
        m_LabAttribute.text = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(FuWenTemplate.GetFuWenTemplateByFuWenId(gemInfo._Gemid).shuXingName)); 
        m_LabAttributeNum.text = MyColorData.getColorString(4,"+" + FuWenTemplate.GetFuWenTemplateByFuWenId(gemInfo._Gemid).shuxingValue.ToString());
        m_LabExp.text = "经验";
        if (FuWenTemplate.GetFuWenTemplateByFuWenId(gemInfo._Gemid).lvlupExp > 0)
        {
            m_LabExpNum.text = gemInfo._Exp + "/" + FuWenTemplate.GetFuWenTemplateByFuWenId(gemInfo._Gemid).lvlupExp;
        }
        else
        {
            m_LabExpNum.text = "已达最高";
        }
       
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
    }

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_IconParent != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_IconParent.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(_gemId, "", 4);
            iconSampleManager.SetIconPopText(_gemId,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_gemId).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_gemId).descId));
            iconSampleManager.tipItemData = TipItemData.createTipItemData();
            iconSampleManager.tipItemData.setExp(exp);
            iconSampleManager.tipItemData.setTouchPosition(TipItemData.ScreenPosition.RIGHT);
            iconSampleObject.transform.localScale = Vector3.one * 0.65f;
 
        }
        else
        {
            p_object = null;
        }
    }
}

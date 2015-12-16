using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AllianceItemManagerment : MonoBehaviour {

    //public UISprite m_SpriteIcon;
    public GameObject m_ObjGou;
    public UILabel m_LabName;
    public UILabel m_LabLevel;
    public UILabel m_LabCountry;
    public UILabel m_LabShengWang;
    public UILabel m_LabMengZhu;
    public List<EventIndexHandle> m_listEvent;
    private int ItemId = 0;
    public UILabel m_labButtonName;

    public delegate void OnClick_Touch(int id);
    OnClick_Touch CallBackTouch;
    public delegate void OnClick_Application(int country_id,int alliance_id);
    OnClick_Application CallBackAppliacetion;
    private int _Country = 0;
    private bool _isCanApply = false;
	void Start () 
    {
        m_listEvent.ForEach(p => p.m_Handle += TouchIndex);
	}

    void TouchIndex(int index)
    {
        if (index == 0)
        {
            if (CallBackTouch != null)
            {
                CallBackTouch(ItemId);
            }
        }
        else
        {
            if (CallBackAppliacetion != null)
            {
                if (_isCanApply)
                {
                    if (_Country != JunZhuData.Instance().m_junzhuInfo.guoJiaId)
                    {
                        CallBackAppliacetion(_Country, ItemId);
                    }
                    else
                    {
                        CallBackAppliacetion(-1, ItemId);
                    }
                }
                else
                {
                    EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                                                , LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_APPLY_HOUR)
                                                , "");
                }
            }
        
        }
    
    }
    void OnClick()
    {
     
    }
    public void ShowAllianceItem(AllianceLayerManagerment.AllianceItemInfo aii, OnClick_Touch callback,OnClick_Application application)
    {
        ItemId = aii.id;
        _Country = aii.country;
        _isCanApply = aii.isCanApply;
        m_labButtonName.text = !aii.isApply ? "申请":"已申请";
        m_listEvent[1].GetComponent<ButtonColorManagerment>().ButtonsControl(!aii.isApply);
        m_LabName.text = "<" + aii.name + ">";
        m_LabLevel.text = aii.level.ToString();
        m_LabCountry.text = NameIdTemplate.GetName_By_NameId(aii.country);
        m_LabShengWang.text = aii.shengwang.ToString();
        m_LabMengZhu.text = aii.mengzhu;
        if (callback != null)
        {
            CallBackTouch = callback;
        }
        CallBackAppliacetion = application;
    }
}

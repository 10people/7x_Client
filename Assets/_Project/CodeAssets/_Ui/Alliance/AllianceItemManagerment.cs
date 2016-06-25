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
    public UILabel m_LabDemandLevel;
    public UILabel m_LabDemandMilitaryRank;
    public UISprite m_SpriteCountry;
    public List<EventIndexHandle> m_listEvent;
    public GameObject m_ObjRed;
    public GameObject m_ObjYellow;
    private int ItemId = 0;
    public UILabel m_labAlpply;
    public UILabel m_labButtonContent;
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
              CallBackAppliacetion(_Country, ItemId);
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
        m_LabDemandLevel.text = aii.applyLevel.ToString();
        m_LabDemandMilitaryRank.text =   MilitaryRankName(aii.MilitaryRank);
        m_SpriteCountry.spriteName = "nation_" + aii.country;
        if (!aii.isApply)
        {
            if (aii.Ren_Now < aii.Ren_Max)
            {
                if (!aii.isApply && aii.ShenPiId != 0)
                {
                    m_ObjRed.SetActive(true);
                    m_ObjYellow.SetActive(false);
                    m_labButtonContent.text = "立即加入";
                }
                else
                {
                    m_ObjRed.SetActive(false);
                    m_ObjYellow.SetActive(true);
                    m_labButtonContent.text = "申请加入";
                }
              //      m_labButtonName.text = !aii.isApply && aii.ShenPiId == 0 ? "申请" : "立即加入";
            }
            else
            {
                m_ObjYellow.SetActive(true);
                m_labButtonContent.text = "成员已满";
            }
        }
        else
        {
            m_ObjRed.SetActive(false);
            m_ObjYellow.SetActive(true);
            m_labButtonContent.text = "已申请";
        }
        m_listEvent[1].GetComponent<UIButton>().enabled = aii.isApply && aii.Ren_Now >= aii.Ren_Max ? true : false;
        m_listEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(!aii.isApply && aii.Ren_Now < aii.Ren_Max,1);
        m_LabName.text = "<" + aii.name + ">";
        m_LabLevel.text = aii.level.ToString();
        m_LabCountry.text = aii.Ren_Now.ToString() + "/" + aii.Ren_Max.ToString();//NameIdTemplate.GetName_By_NameId(aii.country);
        m_LabShengWang.text = aii.cityCount.ToString();
        m_LabMengZhu.text = aii.mengzhu;
        if (callback != null)
        {
            CallBackTouch = callback;
        }
        CallBackAppliacetion = application;
    }

    private string MilitaryRankName(int index)
    {
        switch (index)
        {
            case 1:
                {
                    return "小卒";
                }
                break;
            case 2:
                {
                    return "步兵";
                }
                break;
                      case 3:
                {
                    return "骑士";
                }
                break;
            case 4:
                {
                    return "禁卫";
                }
                break;
            case 5:
                {
                    return "校尉"; 
                }
                break;
            case 6:
                {
                    return "先锋";
                }
                break;
            case 7:
                {
                    return "将军";
                }
                break;
            case 8:
                {
                    return "元帅";
                }
                break;
            case 9:
                {
                    return "诸侯";
                }
                break;
        }
        return "";
    }
}

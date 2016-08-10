using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SettingBlockedItemManagerment : MonoBehaviour 
{
    public UILabel m_LabLevel;
    public UILabel m_LabName;
    public UISprite m_SpriteVip;
    public UILabel m_LabAllianceName;
    public UILabel m_PowerCount;
    public GameObject m_AllianceObject;
    public GameObject m_ObjectV;
    public UISprite m_HeadIcon;
    public EventIndexHandle m_Event;
    public List<UISprite> m_listSprite;
    public delegate void OnClick_Touch(long id,string name);
    OnClick_Touch CallBackTouch;
    
    private long ItemId = 0;
	void Start () 
    {
       m_Event.m_Handle += TouchInfo;
	}

    void TouchInfo(int index)
    {
      CallBackTouch(ItemId,transform.name);  
    }
 
    public  void ShowInfo( BlackJunzhuInfo info ,OnClick_Touch callback)
    {
        ItemId = info.junzhuId;
        CallBackTouch = callback;
        if (string.IsNullOrEmpty(info.lianMengName))
        {
            m_AllianceObject.SetActive(true);
            m_LabAllianceName.text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT) ;
        }
        else
        {
            m_AllianceObject.SetActive(true);
            m_LabAllianceName.text = MyColorData.getColorString(12, "<" + info.lianMengName + ">");
        }
        //  m_HeadIcon.spriteName = icon;
        m_LabLevel.text = info.level.ToString()  ;
        m_LabName.text = info.name;
        if (info.vipLv > 0)
        {
            m_ObjectV.SetActive(true);
            m_SpriteVip.spriteName = "v" + info.vipLv.ToString();
        }
        else
        {
            m_ObjectV.SetActive(false);
        }
       // m_HeadIcon.spriteName = "PlayerIcon" + info.iconId; //info.iconId;
        m_PowerCount.text = info.zhanLi.ToString();
        m_listSprite[2].spriteName = "junxian" + info.junXian;
        m_listSprite[1].spriteName = "nation_" + info.guojia;
        m_listSprite[0].spriteName = "PlayerIcon" + info.iconId.ToString();
       
    }
 
}

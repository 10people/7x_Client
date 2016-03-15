using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FriendOperationItemManagerment : MonoBehaviour
{
    public List<EventIndexHandle> m_listEvent;
    public List<UILabel> m_listLabel;
    public GameObject m_ShowGameobject;
    public GameObject m_AllianceInfo;
    public List<UISprite> m_listSprite;
    public GameObject m_GaoLiangKuang;
    public GameObject m_BackGround;
   
    public delegate void OnClick_TouchEmail(long id);
    OnClick_TouchEmail CallBackEmail;

    public delegate void OnClick_TouchForbid(long id, string name);
    OnClick_TouchForbid CallBackForbid;

    public delegate void OnClick_TouchDelete(long id, string name);
    OnClick_TouchDelete CallBackDelete;

    //public delegate void OnClick_TouchItem();
    //OnClick_TouchItem CallBackTouchItem;

    public delegate void OnClick_TouchMesh(int index);
    OnClick_TouchMesh CallBackTouchMesh;

    
    private long Save_ID = 0;

	void Start ()
    {
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
    }

    void TouchEvent(int index)
    {
       switch (index)
       {
           case 0:
               {
                   CallBackEmail(Save_ID);
               }
               break;
           case 1:
               {
                   CallBackForbid(Save_ID, transform.name);
               }
              break;
           case 2:
              {
                  CallBackDelete(Save_ID, transform.name);
              }
              break;
           case 3:
              {
                  m_ShowGameobject.SetActive(false);
                  m_GaoLiangKuang.SetActive(false);
                  m_ShowGameobject.transform.localPosition = new Vector3(254, -3, 0);
                  m_BackGround.transform.localPosition = new Vector3(-84, 20, 0);
                //  CallBackTouchMesh();
              }
              break;
           case 4:
              {
                  m_GaoLiangKuang.SetActive(true);
              //    m_ShowGameobject.SetActive(true);
                  CallBackTouchMesh(int.Parse(transform.name));
              }
              break;
           default:
               break;
       }

    }

    public void ShowInfo(FriendJunzhuInfo info, OnClick_TouchEmail email, OnClick_TouchForbid forbid, OnClick_TouchDelete dele,OnClick_TouchMesh touchmesh /*,OnClick_TouchItem touchitem*/)
    {
        CallBackTouchMesh = touchmesh;
       // CallBackTouchItem = touchitem;
        Save_ID = info.ownerid;
        CallBackEmail = email;
        CallBackForbid = forbid;
        CallBackDelete = dele;
        m_listLabel[0].text = "LV " + info.level.ToString() + "  " + info.name; 
        m_listSprite[0].spriteName = "PlayerIcon" + info.iconId.ToString();
        if (info.level > 0)
        {
            m_listLabel[3].text = "V" + info.vipLv.ToString();
        }
        else
        {
            m_listLabel[3].text = "V0";
        }

        m_listLabel[2].text = info.zhanLi.ToString();

        if (string.IsNullOrEmpty(info.lianMengName))
        {
            m_listLabel[1].text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT);
            m_AllianceInfo.SetActive(true);
        }
        else
        {
            m_listLabel[1].text = "<"+ info.lianMengName+ ">";
            m_AllianceInfo.SetActive(true);
        }
        m_listSprite[2].spriteName = "junxian" + info.junXian;
       m_listSprite[1].spriteName = "nation_" + info.guojia;
               
    }

    
}

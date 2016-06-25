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

    public GameObject m_ObjV;
    public GameObject m_BackGround;
    public UISprite m_SpriteVip;
    private GameObject _YJQHboxObj = null;
    public delegate void OnClick_TouchEmail(long id);
    OnClick_TouchEmail CallBackEmail;

    public delegate void OnClick_TouchForbid(long id, string name);
    OnClick_TouchForbid CallBackForbid;

    public delegate void OnClick_TouchDelete(long id, string name);
    OnClick_TouchDelete CallBackDelete;

    //public delegate void OnClick_TouchItem();
    //OnClick_TouchItem CallBackTouchItem;

    public delegate void OnClick_TouchMesh(long id,float pos_Y, string name);
    OnClick_TouchMesh CallBackTouchMesh;

    public UILabel m_LabZZZ;
    private long Save_ID = 0;

    public GameObject m_Back_0;
    public GameObject m_Back_1;
    void Start ()
    {
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
    }

    void TouchEvent(int index)
    {
       switch (index)
       {
           //case 0:
           //    {
           //        CallBackEmail(Save_ID);
           //    }
           //    break;
           //case 1:
           //    {
           //        CallBackForbid(Save_ID, transform.name);
           //    }
           //   break;
           //case 2:
           //   {
           //       CallBackDelete(Save_ID, transform.name);
           //   }
           //   break;
           //case 3:
           //   {
           //       m_ShowGameobject.SetActive(false);
           //       m_GaoLiangKuang.SetActive(false);
           //       m_ShowGameobject.transform.localPosition = new Vector3(254, -3, 0);
           //       m_BackGround.transform.localPosition = new Vector3(-84, 20, 0);
           //     //  CallBackTouchMesh();
           //   }
           //   break;
           case 4:
              {
                  m_GaoLiangKuang.SetActive(true);
              //    m_ShowGameobject.SetActive(true);
                  CallBackTouchMesh(Save_ID,transform.localPosition.y, transform.name);
              }
              break;
           default:
               break;
       }

    }

    public void ShowInfo(FriendJunzhuInfo info, OnClick_TouchMesh touchmesh /*,OnClick_TouchItem touchitem*/)
    {
        CallBackTouchMesh = touchmesh;
       // CallBackTouchItem = touchitem;
        Save_ID = info.ownerid;
        //CallBackEmail = email;
        //CallBackForbid = forbid;
        //CallBackDelete = dele;
        m_listLabel[0].text = "Lv " + info.level.ToString() + "  " + info.name; 
        m_listSprite[0].spriteName = "PlayerIcon" + info.iconId.ToString();
 
        if (info.vipLv > 0)
        {
            m_ObjV.SetActive(true);
            m_SpriteVip.spriteName = "v" + info.vipLv.ToString();
        }
        else
        {
            m_ObjV.SetActive(false);
        }

        m_listLabel[2].text = info.zhanLi.ToString();

        if (string.IsNullOrEmpty(info.lianMengName))
        {
            m_listLabel[1].text = MyColorData.getColorString(12, "<" + LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT) + ">");
            m_AllianceInfo.SetActive(true);
        }
        else
        {
            m_listLabel[1].text = MyColorData.getColorString(12, "<" +info.lianMengName+ ">");
            m_AllianceInfo.SetActive(true);
        }
        m_listSprite[2].spriteName = "junxian" + info.junXian;
        m_listSprite[1].spriteName = "nation_" + info.guojia;
        if (info.offlineTime <= 0)
        {
            m_Back_1.SetActive(false);
            m_Back_0.SetActive(true);
            m_LabZZZ.text = MyColorData.getColorString(4, "在线");

            return;
        }
        else
        {
            m_Back_0.SetActive(false);
            m_Back_1.SetActive(true);
        }
        if (info.offlineTime > (60 * 60 * 24))
        {
            int i = (int)(info.offlineTime) / (60 * 60 * 24);

            m_LabZZZ.text = "离线" + i + "天";
        }
        else if (info.offlineTime < (60 * 60 * 24) && info.offlineTime > (60 * 60))
        {
            int i = (int)(info.offlineTime) / (60 * 60);
            //Debug.Log ("mMemberInfo.i = " +i);
            m_LabZZZ.text = "离线" + i + "小时";
        }
        else if (info.offlineTime < 60 * 60 && info.offlineTime > 60)
        {
            int i = (int)(info.offlineTime) / (60);

            m_LabZZZ.text = "离线" + i + "分钟";
        }
        else if (info.offlineTime < 60)
        {
            m_LabZZZ.text = "离线1分钟";
        }

    }
}

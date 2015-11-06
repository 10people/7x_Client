using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ActivityItemManagerment : MonoBehaviour
{
    public EventIndexHandle m_TouchEvent;
    public List<UISprite> m_listSprite;
    public List<UILabel> m_listLabel;
    public List<GameObject> m_ListObject;
    public GameObject m_ParentObj;
    public delegate void OnClick_TouchActivity(int id);
    OnClick_TouchActivity CallBackButtonEvent;
    public GameObject m_Tanhao;

    private int saveId = 0;
    private GameObject SendObj;
	void Start () 
    {
	  m_TouchEvent.m_Handle += TouchEvent;
	}

    void TouchEvent(int index)
    {
        CallBackButtonEvent(int.Parse(m_ParentObj.name));
    
    }
    public void ShowInfo(HuoDongTemplate info, OnClick_TouchActivity callback)
    {
        if (info.id == 1 && PushAndNotificationHelper.IsShowRedSpotNotification(140))
        {
          //  Debug.Log("PushAndNotificationHelper.IsShowRedSpotNotification(140)PushAndNotificationHelper.IsShowRedSpotNotification(140) ::" + PushAndNotificationHelper.IsShowRedSpotNotification(140));
            m_Tanhao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(140));
        }
        else if (info.id == 3 && PushAndNotificationHelper.IsShowRedSpotNotification(144))
        {
            m_Tanhao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(144));
        }
        CallBackButtonEvent = callback;
        saveId = info.id;

//        Debug.Log("info.idinfo.idinfo.id ::" + info.id);

        if (info.type == 0)
        {
            m_ListObject[0].SetActive(false);
            m_ListObject[1].SetActive(true);
        }
        else
        {
            m_listSprite[0].spriteName = "Activity_icon_" + info.id.ToString();
            if (info.state == 0)
            {
               // m_listSprite[0].spriteName = "Activity_icon_" + info.id.ToString();
   
                m_listLabel[0].text = info.title;
                m_listLabel[3].text = info.buttonTitle;
                m_listLabel[2].text = info.awardDesc;
                m_listLabel[1].text = info.desc;
            }
            else
            {
             //   m_listSprite[0].spriteName = "Activity_icon_" + info.id.ToString();
                m_listLabel[0].text = info.title;
                m_listLabel[2].text = info.awardDesc;
                m_listLabel[1].text = info.desc;

                m_Tanhao.SetActive(false);
                m_listLabel[3].text = info.buttonTitleComplete;
                if (info.buttonCompleteTouch == 0)
                {
                    ButtonsControl(m_TouchEvent.gameObject, false);
                }
            }
        }
    }

    void ButtonsControl(GameObject obj, bool colliderEnable)
    {
        if (obj.transform.FindChild("Background").GetComponent<TweenColor>() == null)
        {
            obj.transform.FindChild("Background").gameObject.AddComponent<TweenColor>();
            obj.transform.FindChild("Background").gameObject.AddComponent<TweenColor>().enabled = false;
        }
        if (colliderEnable)
        {
            obj.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            obj.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            obj.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            obj.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
        }

        obj.transform.FindChild("Background").GetComponent<TweenColor>().duration = 0.2f;
        obj.transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        obj.GetComponent<Collider>().enabled = colliderEnable;
        SendObj = obj;
        EventDelegate.Add(obj.transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);

    }
    void TweenColorDestroy()
    {
        if (SendObj.transform.FindChild("Background").GetComponent<TweenColor>() != null)
        {
            EventDelegate.Remove(SendObj.transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);
            Destroy(SendObj.transform.FindChild("Background").GetComponent<TweenColor>());
        }
    }
	
 
}

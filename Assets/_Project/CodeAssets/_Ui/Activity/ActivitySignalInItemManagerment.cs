using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ActivitySignalInItemManagerment : MonoBehaviour 
{
    public List<GameObject> m_listGameobject;
   // public UISprite m_Icon;
     public List<UILabel> m_listLabel;
  //  public EventIndexHandle m_Event;
    public delegate void OnClick_TouchEachDay();
    OnClick_TouchEachDay CallBackSignalIn;
    private bool _isTouchEnable = false;
	void Start ()
    {
	}

    struct ItemInfo
    {
      public int iconType;
      public int count;
      public string icon;
      public bool isTouch;

    };
    private ItemInfo rewardInfo = new ItemInfo();

    public void ShowInfo(QiandaoAward reward, bool isGuang, bool isMesh, OnClick_TouchEachDay callback)
    {
//        Debug.Log("isMeshisMeshisMeshisMesh ::" + isMesh);
        CallBackSignalIn = callback;
        m_listGameobject[0].gameObject.SetActive(reward.state == 1);
        m_listGameobject[2].gameObject.SetActive(isMesh && reward.state == 0);
        _isTouchEnable = isMesh && reward.state == 0 ? true : false;
        rewardInfo.isTouch = (reward.state == 1);
        rewardInfo.count = reward.awardNum;
        rewardInfo.icon = reward.awardId.ToString();
        rewardInfo.iconType = reward.awardType;


        if (reward.vipDouble > 0)
        {
            m_listLabel[0].text = "[b]V" + reward.vipDouble.ToString() + NameIdTemplate.GetName_By_NameId(990052) + "[/b]";
            m_listGameobject[1].SetActive(true);
        }
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
    }

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {


        GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        iconSampleObject.SetActive(true);
        iconSampleObject.transform.parent = transform;
        iconSampleObject.transform.localPosition = Vector3.zero;
        iconSampleObject.transform.localScale = Vector3.one;
        IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

       iconSampleManager.SetIconByID(int.Parse(rewardInfo.icon), rewardInfo.count.ToString());
		iconSampleManager.SetIconPopText(int.Parse(rewardInfo.icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(rewardInfo.icon)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(rewardInfo.icon)).descId));
        if (rewardInfo.isTouch)
        {
            iconSampleManager.NguiLongPress.OnNormalPress = OnTouchEvent;
        }
    }

    void OnTouchEvent(GameObject tempObject)
    {
        if (!_isTouchEnable)
        {
            CallBackSignalIn();
        }
    }

}

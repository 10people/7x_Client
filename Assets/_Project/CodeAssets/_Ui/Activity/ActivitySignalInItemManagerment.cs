using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ActivitySignalInItemManagerment : MonoBehaviour
{
    public List<GameObject> m_listGameobject;
    public Animator m_GouAnimator;
    public GameObject m_BackObj;
    public GameObject m_Back_2Obj;
    public List<UILabel> m_listLabel;
    public EventPressIndexHandle m_Event;
    public delegate void OnClick_TouchEachDay();
    OnClick_TouchEachDay CallBackSignalIn;
    public delegate void OnClick_TouchRetroactive(int index);
    OnClick_TouchRetroactive CallBack_Retroactive;
    public delegate void dele_AnimationFinish();
    dele_AnimationFinish CallBackAnimationFinish;

    private bool _isTouchEnable = false;
    [HideInInspector]
    public bool m_NowSignalIn = false;
    void Start()
    {
        m_Event.m_Handle += Retroactive;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(245), ResourceLoadCallback_0);
    }

    void Retroactive(int index)
    {
        switch (m_SignalInState)
        {
            case 0:
                {
                    Global.CreateFunctionIcon(1901);
                }
                break;
            case 1:
                {
                    CallBack_Retroactive(int.Parse(transform.name));
                }
                break;
            case 2:
                {
                    ClientMain.m_UITextManager.createText("已补签");
                }
                break;
        }
    }
    public void ResourceLoadCallback_0(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        RuntimeAnimatorController anim = (RuntimeAnimatorController)p_object;
        m_GouAnimator.runtimeAnimatorController = anim;
    }
    struct ItemInfo
    {
        public int iconType;
        public int count;
        public int id;
        public bool isTouch;

    };
    public int m_SignalInState;
  
    private ItemInfo rewardInfo = new ItemInfo();
    private int _state = 0;
    private bool _isSpark = false;
    public void ShowInfo(QiandaoAward reward, bool isGuang, bool isMesh, OnClick_TouchEachDay callback, OnClick_TouchRetroactive callback_TouchRetroactive, dele_AnimationFinish callback_finish = null)
    {
        m_SignalInState = reward.isDouble;
        CallBack_Retroactive = callback_TouchRetroactive;
        m_BackObj.SetActive(reward.bottomColor == 0);
        m_Back_2Obj.SetActive(reward.bottomColor == 1);
        if (callback_finish != null)
        {
            CallBackAnimationFinish = callback_finish;
        }
        CallBackSignalIn = callback;
        m_NowSignalIn = reward.state == 1;
        m_listGameobject[0].gameObject.SetActive(reward.state == 1);
        if (reward.state == 1)
        {
            m_GouAnimator.enabled = true;
        }
        else
        {
            m_GouAnimator.enabled = false;
        }
        _state = reward.state;
        m_Event.gameObject.SetActive(isMesh && reward.state == 0 && reward.vipDouble > 0 && reward.isDouble < 2);
        _isSpark = isMesh && reward.state == 0;
        m_listGameobject[2].gameObject.SetActive(isMesh && reward.state == 0);
        _isTouchEnable = isMesh && reward.state == 0 ? true : false;
        rewardInfo.isTouch = (reward.state == 1);
        rewardInfo.count = reward.awardNum;
        rewardInfo.id = reward.awardId;
        rewardInfo.iconType = reward.awardType;
        if (reward.vipDouble > 0)
        {
            m_listLabel[0].text = "[b]V" + reward.vipDouble.ToString() + NameIdTemplate.GetName_By_NameId(990052) + "[/b]";
            m_listGameobject[1].SetActive(true);
        }
        else
        {
            m_listGameobject[1].SetActive(false);
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
        int color = CommonItemTemplate.getCommonItemTemplateById(rewardInfo.id).color;
        iconSampleManager.SetIconByID(rewardInfo.id, rewardInfo.count.ToString(),2);
        iconSampleManager.SetIconPopText(rewardInfo.id, NameIdTemplate.GetName_By_NameId(
            CommonItemTemplate.getCommonItemTemplateById(rewardInfo.id).nameId),
            DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(rewardInfo.id).descId));
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
    public void AnimationPlay()
    {
        if (CallBackAnimationFinish != null)
        CallBackAnimationFinish();
    }

}

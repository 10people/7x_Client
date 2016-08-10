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
    public GameObject m_RedPot;
    public GameObject m_Back_2Obj;
    public List<UILabel> m_listLabel;
    public GameObject m_ObjEffect;
    public EventPressIndexHandle m_Event;
    public delegate void OnClick_TouchEachDay();
    OnClick_TouchEachDay CallBackSignalIn;
    public delegate void OnClick_TouchRetroactive(int index);
    OnClick_TouchRetroactive CallBack_Retroactive;
    public delegate void dele_AnimationFinish();
    dele_AnimationFinish CallBackAnimationFinish;
    public List<GameObject> m_listEffect;
    private bool _isTouchEnable = false;
    [HideInInspector]
    public bool m_NowSignalIn = false;
    public QiandaoAward m_SignalInfo;
    void Start()
    {
        m_Event.m_Handle += Retroactive;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(245), ResourceLoadCallback_0);
    }
    private bool _isCanSignalIn = false;
    void Update()
    {
        if (_isCanSignalIn && m_SignalInfo.state == 1)
        {
          _isCanSignalIn = false;
            ShowEffect();
        }
    }

    void Retroactive(int index)
    {
        switch (m_SignalInState)
        {
            case 0:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.vipLv >= m_SignalInfo.vipDouble)
                    {
                        CallBack_Retroactive(int.Parse(transform.name));
                    }
                    else
                    {
                        Global.CreateFunctionIcon(1901);
                    }
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
    public int m_SignalIns = 0;
    private ItemInfo rewardInfo = new ItemInfo();
    private int _state = 0;
    private bool _isSpark = false;
    private bool _Mesh = false;
    public void ShowInfo(QiandaoAward reward, bool isGuang, bool isMesh, OnClick_TouchEachDay callback, OnClick_TouchRetroactive callback_TouchRetroactive, dele_AnimationFinish callback_finish = null)
    {
        _Mesh = isMesh;
        m_SignalInfo = reward;

        if (  m_SignalInfo.state == 1)
        {
            _isCanSignalIn = true;
        }
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
        m_SignalIns = reward.state;
   

        _state = reward.state;
        m_Event.gameObject.SetActive(isMesh && reward.state == 0 && reward.vipDouble > 0 && m_SignalInState < 2);
     
        m_RedPot.gameObject.SetActive(isMesh && reward.state == 0 && reward.vipDouble > 0 && m_SignalInState < 2
                                     && JunZhuData.Instance().m_junzhuInfo.vipLv >= reward.vipDouble);
        if (isMesh && reward.state == 0 && reward.vipDouble > 0)
        {
            m_GouAnimator.gameObject.SetActive(isMesh && reward.state == 0 && reward.vipDouble > 0 && m_SignalInState == 2);
        }
        else
        {
            m_GouAnimator.gameObject.SetActive(true);
        }
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

    int index_num = 0;
    void ShowEffect()
    {
        m_ObjEffect.SetActive(true);
       StartCoroutine(waitInfo());
    }

    IEnumerator waitInfo()
    {
        m_listEffect[index_num].SetActive(true);
        yield return new WaitForSeconds(0.05f);
        index_num++;
        if (index_num < m_listEffect.Count)
        {
            m_listEffect[index_num].SetActive(true);
            if (index_num > 0)
            {
                m_listEffect[index_num - 1].SetActive(false);
            }
            if (m_SignalIns == 1)
            {
                StartCoroutine(waitInfo());
            }
            else
            {
                m_ObjEffect.SetActive(false);
            }
        }
        else
        {
             m_listEffect[index_num - 1].SetActive(false);
            index_num = 0;
            StartCoroutine(waitInfo());
        }
      
    }

   public void FreshRedPot()
    {
        m_RedPot.gameObject.SetActive(_Mesh && m_SignalInfo.state == 0 && m_SignalInfo.vipDouble > 0 && m_SignalInState < 2
                                        && JunZhuData.Instance().m_junzhuInfo.vipLv >= m_SignalInfo.vipDouble);
    }
}

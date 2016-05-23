using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class SignalInManagerment : MonoBehaviour, SocketProcessor
{
    public static SignalInManagerment m_SignalIn;
    public GameObject m_ObjBack;
    public GameObject m_ObjVSprite;
    public GameObject m_ObjVSprite2;

    public UISprite m_SpriteQianDao;
    public UILabel m_LabName;
    public UILabel m_LabTitle;
    public UILabel m_LabSignalTop;
 
    public UILabel m_LabDes;
    //public UISprite m_SpriteMiBao;
    public UITexture m_TextureIcon;
    public UILabel m_LabSignalInButton;
    public UILabel m_LabSignalInButtonRight;
    public List<EventIndexHandle> m_listEvent;
    public UIGrid m_Grid;
    public GameObject m_ParentDes;
    public UIGrid m_GridReward;
    public GameObject m_MainParent;
    public ScaleEffectController m_SEC;
    public GameObject m_Durable_UI;
    public GameObject m_ObjHidden;
    public GameObject m_ObjDesInfo;
 
    public UISprite m_SpriteShowV;
    public UILabel m_LabShowVDes;
    public UILabel m_LabShowVBottomDes;

 
    public UISprite m_SpriteGetV;

    public GameObject m_ObjAllSignal;
    private bool isillustrate = false;

    public GameObject m_ObjTopLeft;

    public GameObject m_ObjShowV;
    public GameObject m_ObjGetV;
    public EventIndexHandle m_EventEffect;
    public Animator m_Anim_0;
    public Animator m_Anim_1;
    public Animator m_Anim_2;

    private List<ActivitySignalInItemManagerment> _listSignalInItem = new List<ActivitySignalInItemManagerment>();
    private bool _isGeneralRules = false;
    void Awake()
    {
        m_SignalIn = this;
    }
    public enum TouchType
    {
        BUTTON_NONE,
        BUTTON_UP
    }
    public TouchType m_ButtonTouch;
    void Start()
    {
        m_ButtonTouch = TouchType.BUTTON_UP;
        m_EventEffect.m_Handle += EffectHidden;
        if (FreshGuide.Instance().IsActive(100175) && TaskData.Instance.m_TaskInfoDic[100175].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100175;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100177) && TaskData.Instance.m_TaskInfoDic[100177].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100177;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(226), ResourceLoadCallback_0);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(227), ResourceLoadCallback_1);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(228), ResourceLoadCallback_2);
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "签到",0,0);
       
        m_SEC.OpenCompleteDelegate += RequestSignalInInfo;
        m_listEvent.ForEach(p => p.m_Handle += Touch);
    }

    void OnEnable()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void EffectHidden(int index)
    {
        m_ObjBack.GetComponent<FadeInOrOutManagerment>().FadeEffect(FadeInFinish);
        m_ObjVSprite.GetComponent<FadeInOrOutManagerment>().FadeEffect(FadeInFinish);
    }

    void RequestSignalInInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALINLIST_REQ,true);
    }

    void SignalIn()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALIN_REQ);
    }

    void Touch(int index)
    {
        UIYindao.m_UIYindao.CloseUI();
        switch (index)
        {
            case 0:
                {
                    SignalIn();
                }
                break;
            case 1:
                {
                    _isGeneralRules = true;
                  // m_ObjDesInfo.SetActive(true);
                    GeneralControl.Instance.LoadRulesPrefab(QianDaoMonthTemplate.getQianDaoMonthTemplateByMonth(currentMonth).icon.ToString());
                    //m_ObjSignal.SetActive(false);
                    //ShowQianDaoDes();
                }
                break;
            case 2:
                {
                    m_ObjDesInfo.SetActive(false);
                   // m_ObjSignal.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    private List<bool> _listBagsState = new List<bool>();
    void ShowQianDaoDes()
    {
        m_LabDes.text = QianDaoMonthTemplate.getQianDaoMonthTemplateByMonth(currentMonth).desc;
    }

    private List<QiandaoAward> listSignalInInfo = new List<QiandaoAward>();

    private int CurrentSignalInDays = 0;
    private int CurrentDayNum = 0;
    private int CurrentState = 0;
    private int currentMonth = 0;
    private int currentDate = 0;
    
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_SIGNALINLIST_RESP://返回签到信息列表
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetQiandaoResp ReponseInfo = new GetQiandaoResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                        CurrentSignalInDays = ReponseInfo.cnt;
                        CurrentDayNum = ReponseInfo.curDate;
                        listSignalInInfo.Clear();
                        currentMonth = ReponseInfo.award[0].month;
                        currentDate = ReponseInfo.curDate;
                        m_LabSignalTop.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_1) 
                                              + MyColorData.getColorString(5,ReponseInfo.cnt.ToString()) 
                                              + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_2);

                        if (ReponseInfo.award != null)
                        {
                        
                            m_LabTitle.text = ReponseInfo.desc;
                            Show(ReponseInfo.icon.ToString());
                            if (WetherSignalIn(ReponseInfo))
                            {
                                SparkleEffectItem.CloseSparkle(m_SpriteQianDao.gameObject);
                                m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                                m_LabSignalInButton.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_0);// "已签到";
                                //m_LabSignalInButtonRight.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_3)
                                //    + MyColorData.getColorString(5, 4)
                                //    + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_4);
                          //      m_LabSignalInButtonRight.text = "每日[ff0000]4[-]点可领新的签到奖励!";
                            }
                            else
                            {
                                m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
                                SparkleEffectItem.OpenSparkle(m_SpriteQianDao.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
                               // m_LabSignalInButtonRight.text = "";

                            }

                            for (int i = 0; i < ReponseInfo.award.Count; i++)
                            {
                                //if (CurrentSignalInDays < ReponseInfo.award[i].day && ReponseInfo.award[i].awardId == ReponseInfo.icon && WetherSignalIn(ReponseInfo))
                                //{
                                //    m_LabSignalBottom.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_5) 
                                //        + MyColorData.getColorString(5, (ReponseInfo.award[i].day - CurrentSignalInDays).ToString())
                                //        + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_6);
                                //}
                                listSignalInInfo.Add(ReponseInfo.award[i]);
                            }
                         
                          
                            //   required int32 allQianNum = 7; // 历史总共签到次数)

                            ShowSignalIn();
                            m_ObjHidden.SetActive(true);
                        }
                        return true;
                    }
                    break;
                case ProtoIndexes.S_SIGNALIN_RESP://返回签到
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        QiandaoResp ReponseInfo = new QiandaoResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        UIYindao.m_UIYindao.CloseUI();
                        if (ReponseInfo.result == 0)
                        {
                            SingnalFinish();
                            SparkleEffectItem.CloseSparkle(m_SpriteQianDao.gameObject);
                            m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_ObjHidden.SetActive(true);
                            m_LabSignalInButton.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_0);// "已签到";
                            //m_LabSignalInButtonRight.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_3)
                            //    + MyColorData.getColorString(5, 4)
                            //    + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_4);
                            listSignalReward.Clear();
                    
                            string award = "";
                            for (int i = 0; i < ReponseInfo.award.Count; i++)
                            {
                                listSignalReward.Add(ReponseInfo.award[i]);
                                if (i > 0)
                                {
                                    award += "#" + ReponseInfo.award[i].awardType + ":" + ReponseInfo.award[i].awardId + ":" + ReponseInfo.award[i].awardNum;
                                }
                                else
                                {
                                    award += ReponseInfo.award[i].awardType + ":" + ReponseInfo.award[i].awardId + ":" + ReponseInfo.award[i].awardNum;
                                }
                            }
                         
                            FunctionWindowsCreateManagerment.ShowRAwardInfo(award);
                            PushAndNotificationHelper.SetRedSpotNotification(140, false);
                          //  SignalRewardInfo();
                        }
                        else
                        {

                        }
                        return true;
                    }
                case ProtoIndexes.S_RETROACTIVE_RESP://补签信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        QiandaoResp ReponseInfo = new QiandaoResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        _listSignalInItem[_TouchNum].m_SignalInState = 2;
                        _listSignalInItem[_TouchNum].m_Event.gameObject.SetActive(false);
                        string award = "";
                        for (int i = 0; i < ReponseInfo.award.Count; i++)
                        {
                            if (i > 0)
                            {
                                award += "#" + ReponseInfo.award[i].awardType + ":" + ReponseInfo.award[i].awardId + ":" + ReponseInfo.award[i].awardNum;
                            }
                            else
                            {
                                award += ReponseInfo.award[i].awardType + ":" + ReponseInfo.award[i].awardId + ":" + ReponseInfo.award[i].awardNum;
                            }
                        }

                        FunctionWindowsCreateManagerment.ShowRAwardInfo(award);
                        return true;
                    }


                    }
            }
        return false;
    }

    public void VipEffect()
    {
        m_ObjGetV.SetActive(true);
        m_ObjBack.GetComponent<FadeInOrOutManagerment>().FadeEffect(FadeInFinish, true);
        m_ObjVSprite.GetComponent<FadeInOrOutManagerment>().FadeEffect(FadeInFinish, true);
        m_ObjAllSignal.SetActive(false);
      
    }

    void SingnalFinish()
    {
        int size = _listSignalInItem.Count;
        for (int i = 0; i < size; i++)
        {
            if (_listSignalInItem[i].m_NowSignalIn)
            {
                _listSignalInItem[i].m_NowSignalIn = false;
                _listSignalInItem[i].m_listGameobject[0].gameObject.SetActive(false);
                _listSignalInItem[i].m_listGameobject[2].gameObject.SetActive(true);
               // _listSignalInItem[i].m_GouAnimator.gameObject.SetActive(true);
             //   _listSignalInItem[i].m_GouAnimator.enabled = true;
            }
        }
    }
    
    void SingnalUpdate()
    {
     RequestSignalInInfo();
    }
    bool WetherSignalIn(GetQiandaoResp info)
    {
        for (int i = 0; i < info.award.Count; i++)
        {
            if (info.award[i].state == 1)
            {
                return false;
            }
        }
        return true;
    }
    private MiBaoXmlTemp mmibaoxml;
    private void Show(string iconName)
    {
        m_LabName.text = NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).nameId);
        //0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；9强化材料
       mmibaoxml = MiBaoXmlTemp.getMiBaoXmlTempById(int.Parse(iconName));




      //  m_TextureIcon.mainTexture = (Texture)Resources.Load(Res2DTemplate.GetResPath(Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString());
  
    }
    Vector3 pos_start = Vector3.zero;

 

 
    void ShowSignalIn()
    {
        _listSignalInItem.Clear();
     //   MainCityUI.m_MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        int child_Count = m_Grid.transform.childCount;
        for (int i = 0; i < child_Count; i++)
        {
            Destroy(m_Grid.transform.GetChild(i).gameObject);
        }
        index_SignalInNum = 0;
        for (int i = 0; i < listSignalInInfo.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SIGNAL_ITEM),
                                           LoadSignalInItemCallback);
        }
    }
    float pos_x2 = 0;
    int index_TopUpReward = 0;
    private Vector3 startPos2;
    int index_SignalInNum = 0;
    public void LoadSignalInItemCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_Grid != null)
        {
            GameObject tempObj = Instantiate(p_object) as GameObject;
            tempObj.transform.parent = m_Grid.transform;
            tempObj.name = index_SignalInNum.ToString();
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            ActivitySignalInItemManagerment tem = tempObj.GetComponent<ActivitySignalInItemManagerment>();
            _listSignalInItem.Add(tem);
            if (listSignalInInfo[index_SignalInNum].state == 0)
            {
                tempObj.GetComponent<ActivitySignalInItemManagerment>().ShowInfo(listSignalInInfo[index_SignalInNum],
                                                                         listSignalInInfo[index_SignalInNum].state == 0,
                                                                         index_SignalInNum < CurrentSignalInDays ? true : false,
                                                                         SignalIn, Retroactive);
            }
            else
            {
                tempObj.GetComponent<ActivitySignalInItemManagerment>().ShowInfo(listSignalInInfo[index_SignalInNum],
                                                                                   listSignalInInfo[index_SignalInNum].state == 0,
                                                                                   index_SignalInNum < CurrentSignalInDays ? true : false,
                                                                                   SignalIn, Retroactive,SingnalUpdate);
            }
         
          
            if (index_SignalInNum < listSignalInInfo.Count - 1)
            {
                index_SignalInNum++;
            }
            m_Grid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    int _TouchNum = 0;
    void Retroactive(int index)
    {
        _TouchNum = index;
        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetVipDoubleReq tempAdvanceReq = new GetVipDoubleReq();
        tempAdvanceReq.id = index;
        t_qx.Serialize(tempStream, tempAdvanceReq);

        byte[] t_protof;
        t_protof = tempStream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_RETROACTIVE_REQ, ref t_protof);

    }
    //public void LoadDesItemCallback(ref WWW p_www, string p_path, Object p_object)
    //{
    //    if (m_GridDes != null)
    //    {
    //        GameObject boxObj = Instantiate(p_object) as GameObject;
    //        boxObj.transform.parent = m_GridDes.transform;
    //        boxObj.transform.localPosition = Vector3.zero;
    //        boxObj.transform.localScale = Vector3.one;
    //        if (index_ActivityNum < 2)
    //        {
    //            index_ActivityNum++;
    //        }
    //    }
    //    else
    //    {
    //        p_object = null;
    //    }

    //}

    private List<QiandaoAward> listSignalReward = new List<QiandaoAward>();
    float pos_x = 0;
    int index_SignalReward = 0;
    private Vector3 startPos;
    int row = 5;

    void SignalRewardInfo()
    {
        m_GridReward.transform.localPosition = new Vector3(ParentPosOffset(listSignalReward.Count, 112), -20, 0);
        for (int i = 0; i < listSignalReward.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }

    }

    private int ParentPosOffset(int count, int distance)
    {
        if (count % 2 == 0)
        {
            if (count / 2 > 1)
            {
                return -1 * distance / 2 * (count / 2 + 1);
            }
            else
            {
                return -1 * distance / 2 * count / 2;
            }
        }
        else
        {
            return -1 * distance * (count / 2);
        }
    }
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {

        if (m_GridReward != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_GridReward.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID(listSignalReward[index_SignalReward].awardId, listSignalReward[index_SignalReward].awardNum.ToString());
            iconSampleManager.SetIconPopText(listSignalReward[index_SignalReward].awardId, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listSignalReward[index_SignalReward].awardId).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listSignalReward[index_SignalReward].awardId).descId));

            if (index_SignalReward < listSignalReward.Count - 1)
            {
                index_SignalReward++;
            }
            m_GridReward.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void OnDisable()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    void OnDestroy()
    {
        m_SignalIn = null;
        SocketTool.UnRegisterMessageProcessor(this);
    }

    bool _isEnanble = true;
    void FadeInFinish(int index)
    {
        if (index == 0)
        {
            if (_isEnanble)
            {
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EventEffect.gameObject, EffectIdTemplate.GetPathByeffectId(100108), null);
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EventEffect.gameObject, EffectIdTemplate.GetPathByeffectId(620215), null);
                
                m_ObjBack.GetComponent<Animator>().enabled = true;
                m_ObjVSprite.GetComponent<Animator>().enabled = true;
                m_ObjVSprite2.GetComponent<Animator>().enabled = true;
            }
            _isEnanble = false;
        }
        else
        {
            UI3DEffectTool.ClearUIFx(m_EventEffect.gameObject);
           _isEnanble = true;
            m_ObjGetV.SetActive(false);
            m_ButtonTouch = SignalInManagerment.TouchType.BUTTON_UP;
            m_ObjAllSignal.SetActive(true);
            ClientMain.closePopUp();
        }
    }

    public void ResourceLoadCallback_0(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        RuntimeAnimatorController anim = (RuntimeAnimatorController)p_object;
        m_Anim_0.runtimeAnimatorController = anim;
    }

    public void ResourceLoadCallback_1(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        RuntimeAnimatorController anim = (RuntimeAnimatorController)p_object;
        m_Anim_1.runtimeAnimatorController = anim;
 
    }

    public void ResourceLoadCallback_2(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        RuntimeAnimatorController anim = (RuntimeAnimatorController)p_object;
      
        m_Anim_2.runtimeAnimatorController = anim;
    }
}

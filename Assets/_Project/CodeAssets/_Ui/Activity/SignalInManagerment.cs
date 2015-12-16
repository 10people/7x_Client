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
    public UILabel m_LabName;
    public UILabel m_LabTitle;
    public UILabel m_LabSignalTop;
    public UILabel m_LabSignalBottom;
    public UILabel m_LabDes;
    public UISprite m_SpriteMiBao;
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
    //public GameObject m_ObjSignal;
    private bool isillustrate = false;

    public UIAtlas m_Atlas_Pieces;
    public UIAtlas m_Atlas_MiBao;
    public UIAtlas m_Atlas_FuShi;
    public UIAtlas m_Atlas_Commom;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        m_SEC.OpenCompleteDelegate += RequestSignalInInfo;
        m_listEvent.ForEach(p => p.m_Handle += Touch);
    }

    void RequestSignalInInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALINLIST_REQ);
    }

    void SignalIn()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALIN_REQ);
    }


    void Touch(int index)
    {
        switch (index)
        {
            case 0:
                {
                    SignalIn();
                }
                break;
            case 1:
                {
                    m_ObjDesInfo.SetActive(true);
                    //m_ObjSignal.SetActive(false);
                    ShowQianDaoDes();
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

    void ShowQianDaoDes()
    {
        m_LabDes.text = DescIdTemplate.GetDescriptionById(QianDaoMonthTemplate.getDescIdTemplateByMonth(currentMonth));
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
                            m_LabSignalBottom.text = "";
                            m_LabTitle.text = ReponseInfo.desc;
                            Show(ReponseInfo.icon.ToString());
                            if (WetherSignalIn(ReponseInfo))
                            {
                                m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                                m_LabSignalInButton.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_0);// "已签到";
                                m_LabSignalInButtonRight.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_3)
                                    + MyColorData.getColorString(5, 4)
                                    + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_4);
                            }
                            else
                            {
                                m_LabSignalInButtonRight.text = "";
                                m_LabSignalBottom.text = "";
                            }

                            for (int i = 0; i < ReponseInfo.award.Count; i++)
                            {
                                if (CurrentSignalInDays < ReponseInfo.award[i].day && ReponseInfo.award[i].awardId == ReponseInfo.icon && WetherSignalIn(ReponseInfo))
                                {
                                    m_LabSignalBottom.text = LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_5) 
                                        + MyColorData.getColorString(5, (ReponseInfo.award[i].day - CurrentSignalInDays).ToString())
                                        + LanguageTemplate.GetText(LanguageTemplate.Text.SIGNAL_IN_DES_6);
                                }
                                listSignalInInfo.Add(ReponseInfo.award[i]);
                            }
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

                        if (ReponseInfo.result == 0)
                        {
                            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALINLIST_REQ);
                            m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_ObjHidden.SetActive(true);
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
                    break;

            }
        }
        return false;
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
    private void Show(string iconName)
    {
        m_LabName.text = NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).nameId);
        //0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；9强化材料
        if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 5)
        {
            m_SpriteMiBao.atlas = m_Atlas_Pieces;
            m_SpriteMiBao.spriteName = iconName;
        }
        else if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 4)
        {
            m_SpriteMiBao.atlas = m_Atlas_MiBao;
            m_SpriteMiBao.spriteName = iconName;
        }
        else if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 7 || CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 8)
        {
            m_SpriteMiBao.atlas = m_Atlas_FuShi;
            m_SpriteMiBao.type = UISprite.Type.Simple;
            m_SpriteMiBao.spriteName = iconName;
        }
        else
        {
            m_SpriteMiBao.atlas = m_Atlas_Commom;
            m_SpriteMiBao.spriteName = iconName;
        }
    }
    Vector3 pos_start = Vector3.zero;
    void ShowSignalIn()
    {
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
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;

            tempObj.GetComponent<ActivitySignalInItemManagerment>().ShowInfo(listSignalInInfo[index_SignalInNum], 
                                                                     listSignalInInfo[index_SignalInNum].state == 0, 
                                                                     index_SignalInNum < CurrentSignalInDays ? true : false, 
                                                                     SignalIn);
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
        SocketTool.UnRegisterMessageProcessor(this);
    }
}

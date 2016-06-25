using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ActivityLayerManagerment : MonoBehaviour, SocketProcessor
{
    public List<GameObject> m_listObject;
    public List<UILabel> m_listLabel;
    public List<EventIndexHandle> m_listEvent;

    public List<GameObject> m_listParent;

   

    public GameObject m_MainParent;
    private List<HuoDongTemplate> listCurrentInfo = new List<HuoDongTemplate>();

    private bool isillustrate = false;

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        //// ShowActivity();
        // ShowSignalIn();
        m_listEvent.ForEach(p => p.m_Handle += Touch);
        GetActivityInfo();
    }

    void GetActivityInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ACTIVITY_REQ);
    }


    void SignalInInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALINLIST_REQ);
    }

    void SignalIn()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALIN_REQ);
    }

    void TopUpInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TOPUP_REQ);
    }
    void TopUpLingQuInfo()
    {
        m_listObject[6].SetActive(false);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TOPUPLQ_REQ);
    }

    void Touch(int index)
    {
        switch (index)
        {
            case 0:
                {
                    isillustrate = false;
                    m_listEvent[1].transform.FindChild("Background").gameObject.SetActive(true);
                    m_listEvent[1].transform.FindChild("Background2").gameObject.SetActive(false);
                    m_listObject[3].SetActive(false);
                    m_listObject[4].SetActive(true);

                    m_listObject[0].SetActive(true);
                    m_listObject[1].SetActive(false);
                }
                break;
            case 1:
                {
                    if (!isillustrate)
                    {
                        isillustrate = true;
                        m_listEvent[1].transform.FindChild("Background").gameObject.SetActive(false);
                        m_listEvent[1].transform.FindChild("Background2").gameObject.SetActive(true);
                        m_listObject[3].SetActive(true);
                        m_listObject[4].SetActive(false);
                        ShowQianDaoDes();
                    }
                    else
                    {
                        isillustrate = false;
                        m_listEvent[1].transform.FindChild("Background").gameObject.SetActive(true);
                        m_listEvent[1].transform.FindChild("Background2").gameObject.SetActive(false);
                        m_listObject[3].SetActive(false);
                        m_listObject[4].SetActive(true);
                    }

                }
                break;
            case 2:
                {
                    if (CurrentState == 0)
                    {
                        m_listObject[6].transform.FindChild("LabelLingQu").gameObject.SetActive(false);
                        m_listObject[6].transform.FindChild("LabelTopUp").gameObject.SetActive(true);
                        EquipSuoData.TopUpLayerTip();
                        Destroy(m_MainParent);
                    }
                    else
                    {
                        m_listObject[6].transform.FindChild("LabelLingQu").gameObject.SetActive(true);
                        m_listObject[6].transform.FindChild("LabelTopUp").gameObject.SetActive(false);
                        TopUpLingQuInfo();
                    }
                    
                   
                   // m_listObject[0].SetActive(true);
                  //  m_listObject[2].SetActive(false);
                }
                break;
            case 3:
                {
                    m_listObject[0].SetActive(true);
                    m_listObject[2].SetActive(false);
                }
                break;
            case 4:
                {
                    //m_listObject[0].SetActive(true);
                  //  m_listObject[1].SetActive(false);
                  //  m_listObject[0].SetActive(true);
                    m_listObject[5].SetActive(false);
               

                }
                break;
            case 99:
                {
                    m_listObject[0].SetActive(true);
                    //  m_listObject[1].SetActive(false);
                    m_listObject[8].SetActive(false);

                }
                break;
            default:
                break;
        }
    }

    void ShowDifferentLayer(int index)
    {
        CurrentState = listCurrentInfo[index].state;
        switch (listCurrentInfo[index].id)
        {
            case 1:
                {
                    m_listObject[0].SetActive(false);
                    m_listObject[1].SetActive(true);
                    SignalInInfo();
                }
                break;
            case 2:
                {
                    m_listObject[0].SetActive(false);
                    m_listObject[2].SetActive(true);
                    TopUpInfo();
                }
                break;
            case 3:
                {
                    m_listObject[0].SetActive(false);
                    m_listObject[8].SetActive(true);
                    ShowLimit();
                   
         
                }
                break;
            default:
                break;
        }
    }

    void ShowLimit()
    { 
    
    
    }

    void ShowQianDaoDes()
    {
        m_listLabel[3].text =  QianDaoMonthTemplate.getQianDaoMonthTemplateByMonth(currentMonth).desc;
    
    }
    private List<ActivityInfo> listActivitysInfo = new List<ActivityInfo>();
    private List<QiandaoAward> listSignalInInfo = new List<QiandaoAward>();
    private  List<AwardInfo> listFirstTopUpInfo = new List<AwardInfo>(); 
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
                case ProtoIndexes.S_ACTIVITY_RESP://返回活动信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetActivityListResp ReponseInfo = new GetActivityListResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                        if (ReponseInfo.activityList != null)
                        {
                     
                            TidyActivityInfo(ReponseInfo.activityList);
                        }
                        return true;
                    }
                    break;
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
                        m_listLabel[0].text = currentMonth.ToString();
                        m_listLabel[1].text = CurrentSignalInDays.ToString();
                        if (ReponseInfo.award == null)
                        {
                           // Debug.Log("nullnullnullnullnullnullnullnullnullnull");
                        }
                        else
                        {
                            for (int i = 0; i < ReponseInfo.award.Count; i++)
                            {
                                listSignalInInfo.Add(ReponseInfo.award[i]);
                            }
                            ShowSignalIn();
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
                        //CurrentState = 1;
                       // CurrentSignalInDays++;
                      //  m_listLabel[1].text = CurrentSignalInDays.ToString();
                        if (ReponseInfo.result == 0)
                        {
                            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_SIGNALINLIST_REQ);
                             m_listObject[5].SetActive(true);
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
                                    award += ReponseInfo.award[i].awardType +":" +ReponseInfo.award[i].awardId + ":" + ReponseInfo.award[i].awardNum;
                                }
                             }
                            FunctionWindowsCreateManagerment.ShowRAwardInfo(award);
                            PushAndNotificationHelper.SetRedSpotNotification(140, false);
                            SignalRewardInfo();
                            // ShowSignalIn();
                            //GetActivityInfo();
                            listCurrentInfo[0].state = 1;
                            ShowActivity();
                        }
                        else
                        {

                        }
                        return true;
                    }
                    break;
                case ProtoIndexes.S_TOPUP_RESP://返回首冲信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();
                  
                        GetShouchong ReponseInfo = new GetShouchong();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                        m_listLabel[2].text = ReponseInfo.desc;
                        listFirstTopUpInfo.Clear();
                        for (int i = 0; i < ReponseInfo.award.Count; i++)
                        {
                            listFirstTopUpInfo.Add(ReponseInfo.award[i]);
                        }
                        ShowTopUpLingQu();
                        
                        return true;
                    }
                    break;
                case ProtoIndexes.S_TOPUPLQ_RESP://返回首冲领取
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ShouChongAward ReponseInfo = new ShouChongAward();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        
                        if (ReponseInfo.result == 0)
                        {
                            m_listObject[7].SetActive(true);
                            GetActivityInfo();
                            StartCoroutine(WaiforSecond());
                        }
                        return true;
                    }
                    break;

            }
        }
        return false;
    }
    IEnumerator WaiforSecond()
    {
      yield  return new WaitForSeconds(0.8f);
      m_listObject[7].SetActive(false);
      m_listObject[6].SetActive(true);
      m_listObject[0].SetActive(true);
      m_listObject[2].SetActive(false);
    }



    Vector3 pos_start = Vector3.zero;
    void TidyActivityInfo(List<ActivityInfo> list)
    {
        listCurrentInfo.Clear();
        int child_size = m_listParent[0].transform.childCount;

        int size = HuoDongTemplate.templates.Count;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (HuoDongTemplate.templates[j].id == list[i].id)
                {
                      HuoDongTemplate.templates[j].state = list[i].state;
                      if (HuoDongTemplate.templates[j].type != 0)
                      {
                          HuoDongTemplate.templates[j].desc = list[i].desc;
                          HuoDongTemplate.templates[j].awardDesc = list[i].awardDesc;
                      }
                    listCurrentInfo.Add(HuoDongTemplate.templates[j]);

                }
            }
        }
        ShowActivity();
    }
    void ShowActivity()
    {
        int child_Count = m_listParent[0].transform.childCount;
        index_ActivityNum = 0;
        for (int i = 0; i < child_Count; i++)
        {
            Destroy(m_listParent[0].transform.GetChild(i).gameObject);
        }

        int size = listCurrentInfo.Count;
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_ITEM),
                                           LoadActivityItemCallback);
         
        }
    }

    void ShowSignalIn()
    {
        int child_Count = m_listParent[3].transform.childCount;
        for (int i = 0; i < child_Count; i++)
        {
            Destroy(m_listParent[3].transform.GetChild(i).gameObject);
        }
        index_SignalInNum = 0;
        for (int i = 0; i < listSignalInInfo.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SIGNAL_ITEM),
                                           LoadSignalInItemCallback);
        }
    }


    //void ShowSignalInillustrate()
    //{
    //    WWW p_www = null;
    //    LoadActivityItemCallback(ref p_www, "", m_IllustrateItem);
    //}

    float pos_x2 = 0;
    int index_TopUpReward = 0;
    private Vector3 startPos2;

    private bool buttonState = false;
    void ShowTopUpLingQu()
    {
        int chid_count = m_listParent[2].transform.childCount;
        for (int i = 0; i < chid_count; i++)
        {
            Destroy(m_listParent[2].transform.GetChild(i).gameObject);
        }
        if (CurrentState == 0)
        {
            m_listObject[6].transform.FindChild("LabelTopUp").gameObject.SetActive(true);
            m_listObject[6].transform.FindChild("LabelLingQu").gameObject.SetActive(false);
        }
        else
        {
            m_listObject[6].transform.FindChild("LabelTopUp").gameObject.SetActive(false);
            m_listObject[6].transform.FindChild("LabelLingQu").gameObject.SetActive(true);
        }
        index_TopUpReward = 0;
        startPos2 = new Vector3(-130 + (row - listSignalReward.Count) * 60, 0, 0);
        pos_x2 = startPos2.x;

        for (int i = 0; i < listFirstTopUpInfo.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), ShowFirstTopUpReward);
        }

    }


    public void ShowFirstTopUpReward(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_listParent[2].transform != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            startPos2.x = pos_x2 + index_TopUpReward * 140;
            iconSampleObject.transform.parent = m_listParent[2].transform;
            iconSampleObject.transform.localPosition = startPos2;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(listFirstTopUpInfo[index_TopUpReward].awardId, listFirstTopUpInfo[index_TopUpReward].awardNum.ToString(),20);
			iconSampleManager.SetIconPopText(listFirstTopUpInfo[index_TopUpReward].awardId, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listFirstTopUpInfo[index_TopUpReward].awardId).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listFirstTopUpInfo[index_TopUpReward].awardId).descId));
            if (index_TopUpReward < listFirstTopUpInfo.Count - 1)
            {
                index_TopUpReward++;
            }
        }
        else
        {
            p_object = null;
        }
    }


    int index_ActivityNum = 0;

    public void LoadActivityItemCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_listParent[0].transform != null)
        {
            GameObject tempObj = Instantiate(p_object) as GameObject;
            tempObj.transform.parent = m_listParent[0].transform;
            tempObj.transform.name = index_ActivityNum.ToString();
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<ActivityItemManagerment>().ShowInfo(listCurrentInfo[index_ActivityNum], ShowDifferentLayer);
            if (index_ActivityNum < listCurrentInfo.Count - 1)
            {
                index_ActivityNum++;
            }
            m_listParent[0].GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }

    }


    int index_SignalInNum = 0;
    public void LoadSignalInItemCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObj = Instantiate(p_object) as GameObject;
        tempObj.transform.parent = m_listParent[3].transform;
        tempObj.transform.localPosition = Vector3.zero;
        tempObj.transform.localScale = Vector3.one;

        
        if (index_SignalInNum < listSignalInInfo.Count - 1)
        {
            index_SignalInNum++;
        }
        m_listParent[3].GetComponent<UIGrid>().repositionNow = true;
    }

    public void LoadDesItemCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        boxObj.transform.parent = m_listParent[1].transform;
        boxObj.transform.localPosition = Vector3.zero;
        boxObj.transform.localScale = Vector3.one;
        if (index_ActivityNum < 2)
        {
            index_ActivityNum++;
        }

    }

    public void LoadTopUpLingQuCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        boxObj.transform.parent = m_listParent[1].transform;
        boxObj.transform.localPosition = Vector3.zero;
        boxObj.transform.localScale = Vector3.one;
        if (index_ActivityNum < 2)
        {
            index_ActivityNum++;
        }

    }
    private List<QiandaoAward> listSignalReward = new List<QiandaoAward>();
    float pos_x = 0;
    int index_SignalReward = 0;
    private Vector3 startPos;
    int row = 5;

    void SignalRewardInfo()
    {
        m_listParent[4].transform.localPosition = new Vector3(ParentPosOffset(listSignalReward.Count, 112), -20, 0);
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

        if (m_listParent[4] != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_listParent[4].transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
 
            iconSampleManager.SetIconByID(listSignalReward[index_SignalReward].awardId, listSignalReward[index_SignalReward].awardNum.ToString());
            iconSampleManager.SetIconPopText(listSignalReward[index_SignalReward].awardId, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listSignalReward[index_SignalReward].awardId).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listSignalReward[index_SignalReward].awardId).descId));
           
            if (index_SignalReward < listSignalReward.Count - 1)
            {
                index_SignalReward++;
            }
            m_listParent[4].GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    void Retroactive(int index)
    {


    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}

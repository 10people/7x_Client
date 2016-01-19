using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class TopUpLayerManagerment : MonoBehaviour, SocketProcessor
{
    public List<UILabel> m_listLab = new List<UILabel>();

    public List<UIGrid> m_listGrid = new List<UIGrid>();
    public List<GameObject> m_listGameObject = new List<GameObject>();
    public List<EventHandler> m_listEvent = new List<EventHandler>();
    public EventIndexHandle m_Confirm;

    public GameObject m_TeQuan;
    public GameObject m_ItemParent;
    public UIProgressBar m_ProgressBar;
    public UIScrollView m_SView;
    public GameObject m_NeedObject;
    [HideInInspector]
    public bool isSpecial = false;

    private bool istouched = false;
    private int vipLevelUseed = 1;
    private int earnYuabao = 0;
    private int vipLevelCurrent = 1;
    private Dictionary<int, ChongTimes> BuyInfoDic = new Dictionary<int, ChongTimes>();
    private Dictionary<int, GameObject> TopUpItemDic = new Dictionary<int, GameObject>();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
	void Start ()
    {
        m_listEvent.ForEach(p=>p.m_handler += EventTouch);
        m_Confirm.m_Handle += ConFirmTouch;
        RequestData();
	}
    public void RequestData()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_VIPINFO_REQ);
    }

    void Update()
    {
       
    }
    void ConFirmTouch(int index)
    {
        m_listGameObject[3].SetActive(false);
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();

        RechargeReq topupInfo = new RechargeReq();
        topupInfo.type = ChongZhiTemplate.templates[0].id;
        topupInfo.amount = ChongZhiTemplate.templates[0].needNum;
 
       // topupInfo.isSure = true;
 
        t_qx.Serialize(t_tream, topupInfo);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_RECHARGE_REQ, ref t_protof);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_VIPINFO_RESP://返回充值界面信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        VipInfoResp  ReponseInfo = new VipInfoResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        BuyInfoDic.Clear();
                        if (ReponseInfo.infos != null)
                        {
                            int size = ReponseInfo.infos.Count;

                            for (int i = 0; i < size; i++)
                            {
                                BuyInfoDic.Add(ReponseInfo.infos[i].id, ReponseInfo.infos[i]);
                            }
                        }

                        if (ReponseInfo.isMax)
                        {
                            vipLevelCurrent = ReponseInfo.vipLevel;
                            m_listLab[0].text = "VIP" + ReponseInfo.vipLevel.ToString();
                            m_listGameObject[4].SetActive(false);
                            m_listLab[3].text = "";
                            m_ProgressBar.value = 1.0f;
                        }
                        else
                        {
                            vipLevelCurrent = ReponseInfo.vipLevel;
                            m_listLab[0].text = "VIP" + ReponseInfo.vipLevel.ToString();
                            m_listLab[1].text = "VIP" + (ReponseInfo.vipLevel + 1).ToString();
                            m_listLab[2].text = (ReponseInfo.needYb - ReponseInfo.hasYb).ToString();
                            m_listLab[3].text = ReponseInfo.hasYb.ToString() + "/" + ReponseInfo.needYb.ToString();
                            m_ProgressBar.value = ReponseInfo.hasYb / float.Parse(ReponseInfo.needYb.ToString());
                        }
                        m_listGameObject[0].SetActive(true);
                        if (isSpecial)
                        {
                            isSpecial = false;
                            TouchVipButton();
                        }
                        else
                        {
                            m_listGameObject[1].SetActive(true);
                            ShowItem();
                        }
                        return true;
                    }
                    break;
                case ProtoIndexes.S_RECHARGE_RESP://返回充值信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        RechargeResp ReponseInfo = new RechargeResp();
                   
                        t_qx.Deserialize(t_tream,ReponseInfo,ReponseInfo.GetType());
                 
                        if (ReponseInfo.isSuccess)
                        {
                            RequestData();
                            earnYuabao = ReponseInfo.sumAoumnt;
                       
                            {
                               
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxCallbackZero);
                            }
                        }
                        else
                        {
                            if (ReponseInfo.yueKaLeftDays != null && ReponseInfo.yueKaLeftDays > 0 && ReponseInfo.yueKaLeftDays < 5)
                            {
                                m_listLab[4].text = ReponseInfo.yueKaLeftDays.ToString();
                                m_listGameObject[3].SetActive(true);
                            }
                            else if (ReponseInfo.yueKaLeftDays != null && ReponseInfo.yueKaLeftDays > 0 && ReponseInfo.yueKaLeftDays >= 5)
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                           UIBoxLoadYueKaCallback);
                            }


                            if (ReponseInfo.msg.Equals("forbid"))
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxLoadCallbackZero);
                            }
                        }

                        return true;
                    }
                    break;
            }     
        }
        return false;
    }

    public void UIBoxLoadYueKaCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_MONTH_CARD_TAG_0) + "5" + LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_MONTH_CARD_TAG_1);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }
    public void UIBoxLoadCallbackZero(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.TOP_UP_FORBID);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    public void UIBoxCallbackZero(ref WWW p_www, string p_path, Object p_object)
    {
        if (TopUpItemDic.ContainsKey(_ColliderEnableIndex))
        {
            TopUpItemDic[_ColliderEnableIndex].GetComponent<Collider>().enabled = true;
        }
        GameObject boxObj = Instantiate(p_object) as GameObject;
         
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.TOP_UP_SUCCESS) + earnYuabao + NameIdTemplate.GetName_By_NameId(900002);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }
    void EventTouch(GameObject obj)
    {
        if (obj.name.Equals("ButtonLeft"))
        {
            vipLevelUseed--;
            m_listLab[5].text = "VIP" + vipLevelUseed.ToString() + NameIdTemplate.GetName_By_NameId(990047);
            ShowTeQuanItem();
        }
        else if (obj.name.Equals("ButtonRight"))
        {
            vipLevelUseed++;
            m_listLab[5].text = "VIP" + vipLevelUseed.ToString() + NameIdTemplate.GetName_By_NameId(990047);
            ShowTeQuanItem();
        }
        else
        {
            if (!istouched)
            {
                istouched = true;
                obj.transform.FindChild("LabChongZhi").gameObject.SetActive(true);
                obj.transform.FindChild("LabTeQuan").gameObject.SetActive(false);
                m_listGameObject[1].SetActive(false);
                m_listGameObject[2].SetActive(true);
                m_listLab[5].text = "VIP" + vipLevelUseed.ToString() + NameIdTemplate.GetName_By_NameId(990047);
                if (vipLevelCurrent == 0)
                {
                    vipLevelCurrent = 1;
                }

                vipLevelUseed = vipLevelCurrent;
                ShowTeQuanItem();
            }
            else
            {
                istouched = false;
                obj.transform.FindChild("LabChongZhi").gameObject.SetActive(false);
                obj.transform.FindChild("LabTeQuan").gameObject.SetActive(true);
                m_listGameObject[1].SetActive(true);
                m_listGameObject[2].SetActive(false);
                ShowItem();
            }
        }
    
    }
    int index = 0;

    void TouchVipButton()
    {
        istouched = true;
        m_NeedObject.transform.FindChild("LabChongZhi").gameObject.SetActive(true);
        m_NeedObject.transform.FindChild("LabTeQuan").gameObject.SetActive(false);
        m_listGameObject[1].SetActive(false);
        m_listGameObject[2].SetActive(true);
        m_listLab[5].text = "VIP" + vipLevelUseed.ToString() + NameIdTemplate.GetName_By_NameId(990047);
        if (vipLevelCurrent == 0)
        {
            vipLevelCurrent = 1;
        }

        vipLevelUseed = vipLevelCurrent;
        ShowTeQuanItem();
    }
    void ShowItem()
    {
        row = 0;
        col = 0;
        int size = m_ItemParent.transform.childCount;
        index = 0;
        TopUpItemDic.Clear();
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                Destroy(m_ItemParent.transform.GetChild(i).gameObject); 
            }
            for (int j = 0; j < ChongZhiTemplate.templates.Count; j++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_ITEM),
                       ResLoaded);
            }
        }
        else
        {
            for (int j = 0; j < ChongZhiTemplate.templates.Count; j++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_ITEM),
                       ResLoaded);
            }
        }
    }
    void ShowTeQuanItem()
    {
        m_listLab[5].text = "VIP" + vipLevelUseed.ToString() + NameIdTemplate.GetName_By_NameId(990047);
        if (vipLevelUseed > 1)
        {
            m_listEvent[1].gameObject.SetActive(true);
        }
        else if (vipLevelUseed == 1)
        {
            m_listEvent[1].gameObject.SetActive(false);
        }

        if (vipLevelUseed == VipTemplate.templates[VipTemplate.templates.Count - 1].lv)
        {
            m_listEvent[2].gameObject.SetActive(false);
        }
        else 
        {
            m_listEvent[2].gameObject.SetActive(true);
        }


        int size = m_TeQuan.transform.childCount;
      if (size > 0)
      {
          Destroy(m_TeQuan.transform.GetChild(0).gameObject);
      }
      Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_TEQUAN_ITEM),
                   ResTeQuanLoaded);
    }
    int row = 0;
    int col = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.name = ChongZhiTemplate.templates[index].id.ToString();
        tempObject.transform.parent = m_ItemParent.transform;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localPosition = new Vector3(row*290,-col * 200,-10);
        if (BuyInfoDic.ContainsKey(ChongZhiTemplate.templates[index].id) && BuyInfoDic[ChongZhiTemplate.templates[index].id].times > 0)
        {
            if (ChongZhiTemplate.templates[index].extraYuanbao > 0)
            {
                tempObject.GetComponent<TopUpItemManagerment>().ShowInfo(ChongZhiTemplate.templates[index].id, ChongZhiTemplate.templates[index].type, NameIdTemplate.GetName_By_NameId(990048) + ChongZhiTemplate.templates[index].extraYuanbao.ToString() + NameIdTemplate.GetName_By_NameId(900002), ChongZhiTemplate.templates[index].needNum.ToString(), ChongZhiTemplate.templates[index].addNum.ToString(), BuyInfoDic[ChongZhiTemplate.templates[index].id].times, CallBack);
            }
            else
            {
                tempObject.GetComponent<TopUpItemManagerment>().ShowInfo(ChongZhiTemplate.templates[index].id, ChongZhiTemplate.templates[index].type, "", ChongZhiTemplate.templates[index].needNum.ToString(), ChongZhiTemplate.templates[index].addNum.ToString(),BuyInfoDic[ChongZhiTemplate.templates[index].id].times, CallBack);
            }
        }
        else
        {
            tempObject.GetComponent<TopUpItemManagerment>().ShowInfo(ChongZhiTemplate.templates[index].id, ChongZhiTemplate.templates[index].type, ChongZhiTemplate.templates[index].desc, ChongZhiTemplate.templates[index].needNum.ToString(), ChongZhiTemplate.templates[index].addNum.ToString(),0, CallBack); 
        }

        TopUpItemDic.Add(ChongZhiTemplate.templates[index].id, tempObject);

        if (index < ChongZhiTemplate.templates.Count - 1)
        {
            index++;
            row++;
            if (ChongZhiTemplate.templates.Count % 2 == 0)
            {
                if (row == (ChongZhiTemplate.templates.Count / 2))
                {
                    row = 0;
                    col++;
                }
            }
            else
            {
                if (row == (ChongZhiTemplate.templates.Count / 2) + 1)
                {
                    row = 0;
                    col++;
                }
            
            }
          
        }
     
    
    }

    void ResTeQuanLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);

        tempObject.transform.parent = m_TeQuan.transform;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localPosition = Vector3.zero;
        string[] ss = DescIdTemplate.GetDescriptionById(VipTemplate.GetVipInfoByLevel(vipLevelUseed).desc).Split('。');
        string content = "";
        int size = ss.Length;
        for (int i = 0; i < size; i++)
        {
            content += ss[i];
            if (i < size - 1)
            {
                content += "\n";
            }
        }
        tempObject.GetComponent<TopUpTeQuanItemManagerment>().ShowInfo(content);
        //m_listGrid[0].Reposition();
    }
    private int _ColliderEnableIndex = 0;
    void CallBack(int index,int consume)
    {
        TopUpItemDic[index].GetComponent<Collider>().enabled = false;
        _ColliderEnableIndex = index;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        
        RechargeReq topupInfo = new RechargeReq();
        topupInfo.type = index;
        topupInfo.amount = consume;
    
        t_qx.Serialize(t_tream, topupInfo);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_RECHARGE_REQ, ref t_protof);
    }


   
    void OnDestroy()
    {
        TopUpLoadManagerment.m_isLoaded = false;
        SocketTool.UnRegisterMessageProcessor(this);
    }
}

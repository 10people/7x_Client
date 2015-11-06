using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class OnlineRewardcGiftBagsManagerment : MonoBehaviour, SocketProcessor
{
    public UILabel m_LabelChat;
    public UILabel m_LabTitle;
    public UIGrid m_GridObject;
    public GameObject m_HiddenObject;
    public UILabel m_labelZaiXian;
    public UILabel m_labelQiRi;
    private List<HuoDongInfo> _listOnlineInfo = new List<HuoDongInfo>();
    private int typeid = 0;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
    }

    void OnEnable()
    {
        typeid = CityGlobalData.m_Limite_Activity_Type;
        if (CityGlobalData.m_Limite_Activity_Type == 1542000)
        {
            m_LabelChat.text = LanguageTemplate.GetText(LanguageTemplate.Text.LIMIT_TIME_ACTIVITIES_3);
            m_labelZaiXian.gameObject.SetActive(true);
        }
        else
        {
          m_labelQiRi.gameObject.SetActive(true);
          m_LabelChat.text = LanguageTemplate.GetText(LanguageTemplate.Text.LIMIT_TIME_ACTIVITIES_2);
        }
        RequestData(CityGlobalData.m_Limite_Activity_Type);
    }
   
    public void RequestData(int type)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        XinShouXSActivity xinshou = new XinShouXSActivity();
        xinshou.typeId = type;

        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_INFO_REQ, ref t_protof); 
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_XINSHOU_XIANSHI_INFO_RESP:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        XinShouXianShiInfo ReponseInfo = new XinShouXianShiInfo();
                       t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                       
                       TidyData(ReponseInfo);
                       m_HiddenObject.SetActive(true);
                        return true;
                    }
                    break;
                case ProtoIndexes.S_XINSHOU_XIANSHI_AWARD_RESP:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        ReturnAward ReponseInfo = new ReturnAward();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                   
                        int s_size = m_GridObject.transform.childCount;
     
                        for (int i = 0; i < s_size; i++)
                        {
                            if (int.Parse(m_GridObject.transform.GetChild(i).transform.name) == ReponseInfo.huodongId)
                            {
                                m_GridObject.transform.GetChild(i).GetComponent<OnlineRewardcGiftBagsItemManagerment>().m_EventTouch.gameObject.SetActive(false);
                                m_GridObject.transform.GetChild(i).GetComponent<OnlineRewardcGiftBagsItemManagerment>().m_LingQuObj.SetActive(true);
                                break;
                            }
                        }
                       
                        for (int i = 0; i < _listOnlineInfo.Count; i++)
                        {
                            if ( _listOnlineInfo[i].huodongId == ReponseInfo.huodongId)
                            {
                                _listOnlineInfo[i].state = 30;
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(_listOnlineInfo[i].jiangli);
                                break;
                            }
                        }

                        if (CityGlobalData.m_Limite_Activity_Type == 1542000)
                        {
                            MainCityUIRB.SetRedAlert(15, false);
                        }
                        else if (CityGlobalData.m_Limite_Activity_Type != 1542000 && WetherComplete())
                        {
                            MainCityUIRB.SetRedAlert(16, false);
                        }

                    
                        int size_2 = _listOnlineInfo.Count;
                        if (int.Parse(XianshiHuodongTemp.GetXianShiHuoDongById(ReponseInfo.huodongId).doneCondition) > 100)
                        {
                            for (int i = 0; i < size_2; i++)
                            {
                                if (_listOnlineInfo[i].huodongId == ReponseInfo.huodongId)
                                {
                                    if (i + 1 < size_2 )
                                    {
                                      MainCityUIRB.ShowTimeCalc(int.Parse(XianshiHuodongTemp.GetXianShiHuoDongById(_listOnlineInfo[i+1].huodongId).doneCondition));
                                    }
                                    break;
                                }
                            }
                        }
                            return true;
                    }
                    break;
            }
        }
        return false;
    }

    void TidyData(XinShouXianShiInfo data)
    {
        _listOnlineInfo.Clear();
        int size = data.huodong.Count;
        for (int i = 0; i < size; i++)
        {
            _listOnlineInfo.Add(data.huodong[i]);
        }
        int size_ = _listOnlineInfo.Count;
        for (int k = 0; k < size_; k++)
        {
            CreateItem();
        }
    }

    void CreateItem()
    {
       Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_ITEM),
                         ResLoaded);
    }
    private int _indexNum = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_GridObject != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _listOnlineInfo[_indexNum].huodongId.ToString();
            tempObject.transform.parent = m_GridObject.transform;
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.GetComponent<OnlineRewardcGiftBagsItemManagerment>().ShowInfo(_listOnlineInfo[_indexNum], GetAward);
            if (_indexNum < _listOnlineInfo.Count - 1)
            {
                _indexNum++;
            }
        }
        else
        {
            p_object = null;
        }
        m_GridObject.repositionNow = true;
    }
    private int _indexTouch = 0;
    void GetAward(int index)
    {
        _indexTouch = index;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GainAward xinshou = new GainAward();
        xinshou.typeId = typeid;
        xinshou.huodongId = index;
        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_AWARD_REQ, ref t_protof); 
     
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }

    private bool WetherComplete()
    {
        for (int i = 0; i < _listOnlineInfo.Count; i++)
        {
            if (_listOnlineInfo[i].state == 10)
            {
                return false;
            }
        }
        return true;
    }
}
 

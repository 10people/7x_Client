using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class OnlineRewardcGiftTimesManagerment : MonoBehaviour, SocketProcessor
{
    public UISprite m_SpriteLQ;
    public UILabel m_LabMiddleTitle;
    public UILabel m_LabBottomTitle;
    public UILabel m_LabBottomCount;
    public GameObject m_BottomLabParent;

    public GameObject m_HiddenObject;
 
    public UIGrid m_RewardParent;

    public GameObject m_MainParent;
    public List<EventIndexHandle> m_listEvent;
    public ScaleEffectController m_SEC;
    private List<HuoDongInfo> _listOnlineInfo = new List<HuoDongInfo>();
    public struct RewardInfo
    {
        public int id;
        public int count;
    }
    public List<RewardInfo> _listReward = new List<RewardInfo>();
 
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        m_SEC.OpenCompleteDelegate += RequestRewardcGiftTimes;
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
    }

    private void TouchEvent(int index)
    {
        if (index == 0)
        {

        }
        else
        {
            GetAward();
        }

    }
    private bool _isTimeDown = false;
    private int _TimeCount = 0;
    void Update()
    {
        if (_isTimeDown)
        {
            _isTimeDown = false;
            _TimeCount--;
            StartCoroutine(TimeDown());
        }
    }
 
    public void RequestRewardcGiftTimes()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        XinShouXSActivity xinshou = new XinShouXSActivity();
        xinshou.typeId = 1542000;
        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_INFO_REQ, ref t_protof, p_receiving_wait_proto_index: ProtoIndexes.S_XINSHOU_XIANSHI_INFO_RESP);
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
                        _TimeCount = ReponseInfo.remainTime;
                        TidyData(ReponseInfo);
                        m_HiddenObject.SetActive(true);
                        UI2DTool.Instance.AddTopUI(GameObjectHelper.GetRootGameObject(gameObject));
                        return true;
                    }
                    break;
                case ProtoIndexes.S_XINSHOU_XIANSHI_AWARD_RESP:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        ReturnAward ReponseInfo = new ReturnAward();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        for (int i = 0; i < _listOnlineInfo.Count; i++)
                        {
                            if (_listOnlineInfo[i].huodongId == ReponseInfo.huodongId)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(_listOnlineInfo[0].jiangli);
                                _listOnlineInfo.RemoveAt(i);
                                break;
                            }
                        }

                        if (_listOnlineInfo.Count > 0)
                        {
                            int size = m_RewardParent.transform.childCount;
                            for (int i = 0;i < size;i++)
                            {
                                Destroy(m_RewardParent.transform.GetChild(i).gameObject);
                            }
                            _TimeCount = _listOnlineInfo[0].shengTime;
                            MainCityUI.SetRedAlert(15, false);
                            MainCityUIRB.ShowTimeCalc(int.Parse(XianshiHuodongTemp.GetXianShiHuoDongById(_listOnlineInfo[0].huodongId).doneCondition));
                            RewardData(_listOnlineInfo[0]);
                        }
                        else
                        {
                            Destroy(m_MainParent);
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
            int id = 0;
            if (data.huodong[i].state == 10)
            {
                id = data.huodong[i].huodongId;
                _listOnlineInfo.Add(data.huodong[i]);
 
            }
            else if (data.huodong[i].state == 40)
            {
                id = data.huodong[i].huodongId;
                _listOnlineInfo.Add(data.huodong[i]);
            }
  
        }
        if (_listOnlineInfo.Count > 0)
        {
            RewardData(_listOnlineInfo[0]);
        }
    }
    private 
    IEnumerator TimeDown()
    {
        yield return new WaitForSeconds(1.0f);
        m_LabBottomCount.text = MyColorData.getColorString(4, TimeHelper.GetUniformedTimeString(_TimeCount));
        if (_TimeCount > 0)
        {
            _isTimeDown = true;
        }
        else
        {
            m_BottomLabParent.gameObject.SetActive(false);
            SparkleEffectItem.OpenSparkle(m_SpriteLQ.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
            m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
        }
    }

    void RewardData(HuoDongInfo info)
    {
        _listReward.Clear();
        if (info.state == 10) //奖励状态10：未领取 20：已领取 30：超时不能领取 40:不可领取
        {
           m_BottomLabParent.gameObject.SetActive(false);
            SparkleEffectItem.OpenSparkle(m_SpriteLQ.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
            m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
        }
        else if (info.state == 40)
        {
            m_BottomLabParent.gameObject.SetActive(true);
           SparkleEffectItem.CloseSparkle(m_SpriteLQ.gameObject);
           m_listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
          //  _TimeCount = info.shengTime;
            m_LabBottomCount.text = MyColorData.getColorString(4, TimeHelper.GetUniformedTimeString(_TimeCount));
            _isTimeDown = true;
        }
        m_LabMiddleTitle.text = XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).desc;
        if (info.jiangli.IndexOf("#") > -1)
        {
            string[] ss = info.jiangli.Split('#');
            for (int j = 0; j < ss.Length; j++)
            {
                string[] award = ss[j].Split(':');
                RewardInfo reward = new RewardInfo();

                reward.id = int.Parse(award[1]);

                reward.count = int.Parse(award[2]);
                _listReward.Add(reward);
            }
        }
        else
        {
            string[] award = info.jiangli.Split(':');
            RewardInfo reward = new RewardInfo();
            reward.id = int.Parse(award[1]);
            reward.count = int.Parse(award[2]);
            _listReward.Add(reward);
        }
        CreateItem();
    }

    void CreateItem()
    {
        _indexNum = 0;
        int size = _listReward.Count;
        m_RewardParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(size, 60), 0, 0);
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                              ResLoadedSimple);
        }
    }
    private int _indexNum = 0;
    void GetAward()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GainAward xinshou = new GainAward();
        xinshou.typeId = 1542000;
        xinshou.huodongId = _listOnlineInfo[0].huodongId;
        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_AWARD_REQ, ref t_protof);
    }

    void OnDisable()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    //void OnDestroy()
    //{
    //    SocketTool.UnRegisterMessageProcessor(this);
    //}

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
 
    void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_RewardParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _listReward[_indexNum].id.ToString();
            tempObject.transform.parent = m_RewardParent.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(_listReward[_indexNum].id, _listReward[_indexNum].count.ToString(), 10);

            iconSampleManager.SetIconPopText(_listReward[_indexNum].id, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).descId));

            tempObject.transform.localScale = Vector3.one * 0.4f;

            if (_indexNum < _listReward.Count - 1)
            {
                _indexNum++;
            }
        }
        else
        {
            p_object = null;
        }
        m_RewardParent.repositionNow = true;
    }
}

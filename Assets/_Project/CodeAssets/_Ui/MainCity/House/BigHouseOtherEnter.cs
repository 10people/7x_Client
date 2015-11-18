using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class BigHouseOtherEnter : MonoBehaviour, SocketProcessor
{
    public BigHouseOther m_BigHouseOther;

    public UILabel BigTitle;
    public UILabel SmallTitle;
    public UILabel DescLabel;
    public UIEventListener ExchangeBtnListener;
    public UIEventListener EnterBtnListener;
    public UIEventListener BgListener;

    public void SetEnterInfo()
    {
        BigTitle.text = m_BigHouseOther.m_HouseSimpleInfo.jzName;
        SmallTitle.text = "<" + FangWuInfoTemplate.GetNameById(m_BigHouseOther.m_HouseSimpleInfo.locationId - 100) +
                          ">-" + HouseBasic.GetStateStr(m_BigHouseOther.m_HouseSimpleInfo.state, false);

        //[FIX]disable enter
        EnterBtnListener.gameObject.SetActive(false);

        if (m_BigHouseOther.m_HouseSimpleInfo.jzId == -1)
        {
            EnterBtnListener.gameObject.SetActive(false);

            DescLabel.text =
                (string.IsNullOrEmpty(m_BigHouseOther.m_HouseSimpleInfo.firstOwner) || string.IsNullOrEmpty(m_BigHouseOther.m_HouseSimpleInfo.firstHoldTime)) ?
                FangWuInfoTemplate.GetNoConsDescriptionById(m_BigHouseOther.m_HouseSimpleInfo.locationId - 100) :
                string.Format(FangWuInfoTemplate.GetNoOwnerDescriptionById(m_BigHouseOther.m_HouseSimpleInfo.locationId - 100), m_BigHouseOther.m_HouseSimpleInfo.firstOwner, m_BigHouseOther.m_HouseSimpleInfo.firstHoldTime);
        }
        else
        {
            DescLabel.text =
                string.Format(FangWuInfoTemplate.GetDescriptionById(m_BigHouseOther.m_HouseSimpleInfo.locationId - 100), m_BigHouseOther.m_HouseSimpleInfo.firstOwner, m_BigHouseOther.m_HouseSimpleInfo.firstHoldTime);
        }
    }

    private void OnBGClick(GameObject go)
    {
        Destroy(m_BigHouseOther.gameObject);
    }

    private void OnEnterClick(GameObject go)
    {
        m_BigHouseOther.OnEnterClick();
    }

    /// <summary>
    /// Exchange
    /// </summary>
    /// <param name="go"></param>
    private void OnExchangeClick(GameObject go)
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
            ExchangeHouseCallBack);
    }

    /// <summary>
    /// In protect period
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeFailForProtectPeriod(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("竞拍失败",
            null, string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.BIG_HOUSE_EXCHANGE_COOL_DOWN).Replace("X", "{0}").Replace("Y", "{1}"), 0, 0),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    /// <summary>
    /// exchange big house confirm
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeHouseCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("竞拍确认",
            null, string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.BIG_HOUSE_EXCHANGE_CONFIRM).Replace("xxxx", "{0}"),
            m_BigHouseOther.m_HouseSimpleInfo.hworth * CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 3) / 100),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            BoxExchangeHouse);
    }

    void BoxExchangeHouse(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                ExchangeHouse temp = new ExchangeHouse
                {
                    targetId = m_BigHouseOther.m_HouseSimpleInfo.jzId
                };
                SocketHelper.SendQXMessage(temp, ProtoIndexes.C_PAI_BIG_HOUSE);
                break;
        }
    }

    /// <summary>
    /// not enough contributation
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeFailForNotEnoughContributationCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        string[] formatStrArr = LanguageTemplate.GetText(LanguageTemplate.Text.BIG_HOUSE_NO_CONTRIBUTATION)
            .Split(new[] { "****" }, StringSplitOptions.RemoveEmptyEntries);
        string formatStr = formatStrArr[0] + "{0}" + formatStrArr[1] + "{1}" + formatStrArr[2];

        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("竞拍失败",
            null, string.Format(formatStr, 0, 0),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            null);
    }

    /// <summary>
    /// exchange big house succeed
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("竞拍成功",
            null, string.Format("恭喜您使用{0}贡献值成功竞拍此间豪宅！", 0),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
        //TenementData.Instance.RequestData();

        Destroy(m_BigHouseOther.gameObject);
    }

    void OnEnable()
    {
        ExchangeBtnListener.onClick = OnExchangeClick;
        EnterBtnListener.onClick = OnEnterClick;

        BgListener.onClick = OnBGClick;
    }

    void OnDisable()
    {
        ExchangeBtnListener.onClick = null;
        EnterBtnListener.onClick = null;
        BgListener.onClick = null;
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //exchange big house result
                case ProtoIndexes.S_PAI_BIG_HOUSE:
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    ExchangeResult tempInfo = new ExchangeResult();
                    t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                    switch (tempInfo.code)
                    {
                        case 0:
                            //succeed
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeSucceedCallBack);
                            break;
                        case 500:
                            //not enought contributation.
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForNotEnoughContributationCallBack);
                            break;
                        case 600:
                            //protect period.
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForProtectPeriod);
                            break;
                        case 700:
                            //unknow error.
                            Debug.LogError("Can't exchange cause unknow error.");
                            break;
                    }
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}

using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class SmallHouseOtherEnter : MonoBehaviour, SocketListener
{
    public SmallHouseOther m_SmallHouseOther;

    public UILabel BigTitle;
    public UILabel SmallTitle;
    public UILabel DescLabelNotAlliance;
    public UILabel DescLabelIsAlliance;
    public UIEventListener ExchangeBtnListener;
    public UIEventListener CancelExchangeBtnListener;
    public UIEventListener EnterBtnListener;
    public UIEventListener SwitchBtnListener;
    public UIEventListener BgListener;

    public void SetEnterInfo()
    {
        //I am alliance leader.
        if (AllianceData.Instance.g_UnionInfo.identity == 2)
        {
            DescLabelIsAlliance.gameObject.SetActive(true);
            DescLabelNotAlliance.gameObject.SetActive(false);

            //disable switch and enter if no owner
            if (m_SmallHouseOther.m_HouseSimpleInfo.jzId == -1)
            {
                SwitchBtnListener.gameObject.SetActive(false);
                EnterBtnListener.gameObject.SetActive(false);
                DescLabelIsAlliance.text = 
                    (string.IsNullOrEmpty(m_SmallHouseOther.m_HouseSimpleInfo.firstOwner) || string.IsNullOrEmpty(m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime)) ? 
                    FangWuInfoTemplate.GetNoConsDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100) : 
                    string.Format(FangWuInfoTemplate.GetNoOwnerDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100), m_SmallHouseOther.m_HouseSimpleInfo.firstOwner, m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime);
            }
            else
            {
                SwitchBtnListener.gameObject.SetActive(true);

                //[FIX]disable enter
                //EnterBtnListener.gameObject.SetActive(true);
                EnterBtnListener.gameObject.SetActive(false);
                DescLabelIsAlliance.text = string.Format(FangWuInfoTemplate.GetDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100), m_SmallHouseOther.m_HouseSimpleInfo.firstOwner, m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime);
            }
        }
        else
        {
            DescLabelIsAlliance.gameObject.SetActive(false);
            DescLabelNotAlliance.gameObject.SetActive(true);
            SwitchBtnListener.gameObject.SetActive(false);

            //disable enter if no owner
            if (m_SmallHouseOther.m_HouseSimpleInfo.jzId == -1)
            {
                EnterBtnListener.gameObject.SetActive(false);
                DescLabelNotAlliance.text = 
                    (string.IsNullOrEmpty(m_SmallHouseOther.m_HouseSimpleInfo.firstOwner) || string.IsNullOrEmpty(m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime)) ?
                    FangWuInfoTemplate.GetNoConsDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100) :
                    string.Format(FangWuInfoTemplate.GetNoOwnerDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100), m_SmallHouseOther.m_HouseSimpleInfo.firstOwner, m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime);
            }
            else
            {
                //[FIX]disable enter
                //EnterBtnListener.gameObject.SetActive(true);
                EnterBtnListener.gameObject.SetActive(false);
                DescLabelNotAlliance.text = string.Format(FangWuInfoTemplate.GetDescriptionById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100), m_SmallHouseOther.m_HouseSimpleInfo.firstOwner, m_SmallHouseOther.m_HouseSimpleInfo.firstHoldTime);
            }
        }

        BigTitle.text = m_SmallHouseOther.m_HouseSimpleInfo.jzName;
        SmallTitle.text = "<" + FangWuInfoTemplate.GetNameById(m_SmallHouseOther.m_HouseSimpleInfo.locationId + 100) +
                          ">-" + HouseBasic.GetStateStr(m_SmallHouseOther.m_HouseSimpleInfo.state, true);
    }

    private void OnBGClick(GameObject go)
    {
        Destroy(m_SmallHouseOther.gameObject);
    }

    private void OnEnterClick(GameObject go)
    {
        m_SmallHouseOther.OnEnterClick();
    }

    /// <summary>
    /// cancel exchange house request.
    /// </summary>
    /// <param name="go"></param>
    private void OnCancelExchangeClick(GameObject go)
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
            CancelExchangeCallBack);
    }

    void CancelExchangeCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_CANCEL_EXCHANGE_CONFIRM),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            BoxCancelExchange);
    }

    void BoxCancelExchange(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                //cancel exchange
                UtilityTool.SendQXMessage(ProtoIndexes.C_CANCEL_EXCHANGE);
                break;
        }
    }

    /// <summary>
    /// cancel exchange house succeed
    /// </summary>
    private void CancelExchangeSucceed()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
            CancelExchangeSucceedCallBack);
    }

    void CancelExchangeSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_CANCELED_EXCHANGE),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);

        CancelExchangeBtnListener.gameObject.SetActive(false);
        ExchangeBtnListener.gameObject.SetActive(true);
    }

    /// <summary>
    /// exchange house
    /// </summary>
    /// <param name="go"></param>
    private void OnExchangeClick(GameObject go)
    {
        //I am alliance leader!
        if (AllianceData.Instance.g_UnionInfo.identity == 2)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                            ExchangeHouseCallBack);
            return;
        }

        //Only for living
        if (m_SmallHouseOther.m_HouseSimpleInfo.state == 20)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForOnlyLivingCallBack);
        }
        //Exchange
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                ExchangeHouseCallBack);
        }
    }

    void ExchangeFailForOnlyLivingCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_LIVING),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    void ExchangeFailForApplyingOtherCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_1_AT_1_TIME),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    void ExchangeFailForInsufficientAllianceTimeCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_ALLIANCE_TIME_INSUFFICIENT),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    void ExchangeFailForCoolingDownCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_LATER),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    /// <summary>
    /// exchange house confirm
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeHouseCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("换房申请",
            null,
            AllianceData.Instance.g_UnionInfo.identity == 2 ?
            LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_CONFIRM1) :
            LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_CONFIRM2),
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
                //No exchange card.
                if (BagData.Instance().m_bagItemList.FirstOrDefault(item => item.itemId == 900016) == null)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        ExchangeFailForNoCardCallBack);
                }
                else
                {
                    //has house owner
                    if (m_SmallHouseOther.m_HouseSimpleInfo.jzId != -1)
                    {
                        //send exchange request
                        ExchangeHouse temp = new ExchangeHouse
                        {
                            targetId = m_SmallHouseOther.m_HouseSimpleInfo.jzId
                        };
                        UtilityTool.SendQXMessage(temp, ProtoIndexes.C_HOUSE_EXCHANGE_RQUEST);
                    }
                    else
                    //no house owner 
                    {
                        //send exchange request
                        ExchangeEHouse temp = new ExchangeEHouse
                        {
                            targetloc = m_SmallHouseOther.m_HouseSimpleInfo.locationId
                        };
                        UtilityTool.SendQXMessage(temp, ProtoIndexes.C_EHOUSE_EXCHANGE_RQUEST);
                    }
                }
                break;
        }
    }

    void ExchangeFailForNoCardCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_NO_EXCHANGE_CARD1),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            BoxBuyExchangeCard);
    }

    void BoxBuyExchangeCard(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                //go to pawn shop.
                if (GameObject.Find("Pawnshop(Clone)") == null)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_PAWNSHOP), PawnShopLoadCallback);
                }

                Destroy(m_SmallHouseOther.gameObject);

                break;
        }
    }

    private void PawnShopLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }

    /// <summary>
    /// exchange request sent
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeRequestSentCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("交换成功",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGE_SENT),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);

        CancelExchangeBtnListener.gameObject.SetActive(true);
        ExchangeBtnListener.gameObject.SetActive(false);

        //TenementData.Instance.RequestData();
    }

    /// <summary>
    /// exchange house succeed
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void ExchangeSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("交换成功",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_EXCHANGED),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);

        //TenementData.Instance.RequestData();

        Destroy(m_SmallHouseOther.gameObject);
    }

    /// <summary>
    /// allliace leader set state.
    /// </summary>
    /// <param name="go"></param>
    void OnSwitchClick(GameObject go)
    {
        //fail for sell setted by player.
        if (m_SmallHouseOther.m_HouseSimpleInfo.state == 10)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), SwitchFailForSellingSettedByPlayerCallBack);
        }
        //switch to sell
        else if (m_SmallHouseOther.m_HouseSimpleInfo.state == 20)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), SwitchToSellCallBack);
        }
        //switch to living
        else if (m_SmallHouseOther.m_HouseSimpleInfo.state == 505)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), SwitchToLivingCallBack);
        }
    }

    void SwitchFailForSellingSettedByPlayerCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改状态",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_LEADER_CANT_SWITCH),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    void SwitchToSellCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改状态",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_LEADER_SWITCH_TO_SELL_CONFIRM),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            BoxSwitchToSell);
    }

    void BoxSwitchToSell(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                SetHouseState temp = new SetHouseState();
                temp.state = 505;
                temp.open4my = m_SmallHouseOther.m_HouseSimpleInfo.open4my ? 1 : 0;
                temp.targetId = m_SmallHouseOther.m_HouseSimpleInfo.jzId;
                temp.locationId = m_SmallHouseOther.m_HouseSimpleInfo.locationId;

                UtilityTool.SendQXMessage(temp, ProtoIndexes.C_SET_HOUSE_STATE);
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), SwitchToSellSucceedCallBack);

                //TenementData.Instance.RequestData();
                break;
        }
    }

    void SwitchToSellSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改状态",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_LEADER_SWITCH_TO_SELL),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            OnCloseHouseUICallBack);
    }

    void SwitchToLivingCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改状态",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_LEADER_SWITCH_TO_LIVING_CONFIRM),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            BoxSwitchToLiving);
    }

    void BoxSwitchToLiving(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                SetHouseState temp = new SetHouseState();
                temp.state = 20;
                temp.open4my = m_SmallHouseOther.m_HouseSimpleInfo.open4my ? 1 : 0;
                temp.targetId = m_SmallHouseOther.m_HouseSimpleInfo.jzId;
                temp.locationId = m_SmallHouseOther.m_HouseSimpleInfo.locationId;

                UtilityTool.SendQXMessage(temp, ProtoIndexes.C_SET_HOUSE_STATE);
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), SwitchToLivingSucceedCallBack);

                //TenementData.Instance.RequestData();
                break;
        }
    }

    void SwitchToLivingSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改状态",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_LEADER_SWITCH_TO_LIVING),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            OnCloseHouseUICallBack);
    }

    void OnCloseHouseUICallBack(int i)
    {
        Destroy(m_SmallHouseOther.gameObject);
    }

    void OnEnable()
    {
        ExchangeBtnListener.onClick = OnExchangeClick;
        CancelExchangeBtnListener.onClick = OnCancelExchangeClick;
        EnterBtnListener.onClick = OnEnterClick;
        SwitchBtnListener.onClick = OnSwitchClick;
        BgListener.onClick = OnBGClick;
    }

    void OnDisable()
    {
        ExchangeBtnListener.onClick = null;
        CancelExchangeBtnListener.onClick = null;
        EnterBtnListener.onClick = null;
        SwitchBtnListener.onClick = null;
        BgListener.onClick = null;
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //small house exchange result
                case ProtoIndexes.S_HOUSE_EXCHANGE_RESULT:
                case ProtoIndexes.S_EHOUSE_EXCHANGE_RESULT:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ExchangeResult tempInfo = new ExchangeResult();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.code)
                        {
                            case 0:
                                //succeed
                                if (AllianceData.Instance.g_UnionInfo.identity != 2)
                                {
                                    Global.ResourcesDotLoad(
                                        Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        ExchangeRequestSentCallBack);
                                }
                                else
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeSucceedCallBack);
                                }
                                break;
                            case 10:
                                //no exchange card
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForNoCardCallBack);
                                break;
                            case 100:
                                //only one apply.
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForApplyingOtherCallBack);
                                break;
                            case 200:
                                //Cooling down
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForCoolingDownCallBack);
                                break;
                            case 300:
                                //alliance time minor to 3 days.
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForInsufficientAllianceTimeCallBack);
                                break;
                            case 400:
                                //only living
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ExchangeFailForOnlyLivingCallBack);
                                break;
                            case 700:
                                //unknow error.
                                Debug.LogError("Can't exchange cause unknow error.");
                                break;
                        }
                        return true;
                    }
                //cancel small house exchange result
                case ProtoIndexes.S_CANCEL_EXCHANGE:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ExchangeResult tempInfo = new ExchangeResult();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.code)
                        {
                            case 0:
                                //succeed
                                CancelExchangeSucceed();
                                break;
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}

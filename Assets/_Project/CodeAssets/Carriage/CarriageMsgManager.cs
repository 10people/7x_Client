using System;
using UnityEngine;
using System.Collections;
using System.IO;
using Carriage;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class CarriageMsgManager : MonoBehaviour, SocketListener
{
    public RootManager m_RootManager;

    public YabiaoJunZhuList m_YabiaoJunZhuList;

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_YABIAO_BUY_RESP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BuyCountsResp temp = new BuyCountsResp();
                        t_qx.Deserialize(t_tream, temp, temp.GetType());

                        if (CityGlobalData.GetYunBiaoBuyType == 20)
                        {
                            switch (temp.result)
                            {
                                //succeed
                                case 10:
                                    {
                                        m_YabiaoJunZhuList.jieBiaoCiShu = temp.leftJBTimes;

                                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnBuyRobTimesSucceed);
                                        break;
                                    }
                                //not enough ingot
                                case 20:
                                    {
                                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnNotEnoughIngot);
                                        break;
                                    }
                                //buy times exhausted
                                case 30:
                                    {
                                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnCannotBuyTimes);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
            }
        }
        return false;
    }

    #region BuyRobTimes

    public void OnBuyRobTimesConfirm()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnBuyRobTimesConfirm);
    }

    private void OnBuyRobTimesConfirm(ref WWW p_www, string p_path, Object p_object)
    {
        int totalTimes = VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).JiebiaoTimes;

        if (m_YabiaoJunZhuList.buyCiShu + 1 <= totalTimes)
        {
            PurchaseTemplate template = PurchaseTemplate.GetBuyRobCarriageTime(m_YabiaoJunZhuList.buyCiShu + 1);

            if (template.price > JunZhuData.Instance().m_junzhuInfo.yuanBao)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnNotEnoughIngot);
                return;
            }

            buyCost = template.price;

            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_46).Replace("X", template.price.ToString()).Replace("Y", template.number.ToString()).Replace("N", (totalTimes - m_YabiaoJunZhuList.buyCiShu).ToString()),
                null,
                LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                OnBuyRobTimes);
        }
        else
        {
            Debug.LogWarning("cannot buy rob carriage time");

            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnCannotBuyTimes);
        }
    }

    private int buyCost;

    private void OnBuyRobTimes(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                {
                    //Goto recharge.
                    if (JunZhuData.Instance().m_junzhuInfo.yuanBao < buyCost)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ReChargeCallBack);
                        return;
                    }

                    BuyCountsReq temp = new BuyCountsReq()
                    {
                        type = 20
                    };
                    CityGlobalData.SetYunBiaoBuyType = 20;
                    SocketHelper.SendQXMessage(temp, ProtoIndexes.C_YABIAO_BUY_RSQ);

                    break;
                }
        }
    }

    public void ReChargeCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_47),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            OnBoxReCharge);
    }

    private void OnBoxReCharge(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                //goto recharge.
                TopUpLoadManagerment.m_instance.LoadPrefab(true);
                break;
            default:
                Debug.LogError("UIBox callback para:" + i + " is not correct.");
                break;
        }
    }

    private void OnCannotBuyTimes(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_49).Replace("x", (JunZhuData.Instance().m_junzhuInfo.vipLv + 1).ToString()),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    private void OnNotEnoughIngot(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_47),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            OnGotoRecharge);
    }

    private void OnBuyRobTimesSucceed(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, "购买成功",
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);

        //Set cd time to 0 to initialize carriage ui.
        m_RootManager.m_CarriageUI.SetKingInfo(m_YabiaoJunZhuList.jieBiaoCiShu, 0);
    }

    private void OnGotoRecharge(int i)
    {
        switch (i)
        {
            case 1:
                TopUpLoadManagerment.m_instance.LoadPrefab(true);
                break;
        }
    }

    #endregion

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}

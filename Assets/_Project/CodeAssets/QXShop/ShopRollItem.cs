using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class ShopRollItem : MonoBehaviour
{
    public ShopRollController m_ShopRollController;

    public int m_Type;

    public GameObject m_IngotObject;
    public UILabel m_CostIngotNumLabel;
    public UILabel m_RemainingTimesLabel;
    public UIButton m_BuyBtn;
    public UILabel m_BuyBtnLabel;
    public GameObject m_BuyRedAlert;

    public GameObject m_AnimObject;

    public WubeiFangInfo m_WubeiFangInfo;

    [HideInInspector]
    public bool IsCloseClickManually = false;

    public void SetThis()
    {
        if (m_WubeiFangInfo.freeTimes > 0)
        {
            m_BuyBtnLabel.text = "免费" + m_WubeiFangInfo.freeTimes + "次";

            m_IngotObject.SetActive(false);

            m_BuyRedAlert.SetActive(true);

            m_RemainingTimesLabel.gameObject.SetActive(false);

            m_BuyBtn.isEnabled = true;
        }
        else
        {
            m_BuyBtnLabel.text = "元宝购买";

            m_RemainingTimesLabel.gameObject.SetActive(true);

            if (m_WubeiFangInfo.remainYuanbaoBuyTimes > 0)
            {
                m_IngotObject.SetActive(true);
                m_CostIngotNumLabel.text = m_WubeiFangInfo.costYuanbao.ToString();

                m_RemainingTimesLabel.text = "今日还可以购买" + ColorTool.Color_Red_c40000 + m_WubeiFangInfo.remainYuanbaoBuyTimes + "[-]次";

                m_BuyBtn.isEnabled = true;
            }
            else
            {
                m_IngotObject.SetActive(false);

                m_RemainingTimesLabel.text = "购买次数已用完";

                m_BuyBtn.isEnabled = true;
            }

            m_BuyRedAlert.SetActive(false);

        }
    }

    public void OnBuyClick()
    {
        if (IsCloseClickManually)
        {
            return;
        }

        if (m_WubeiFangInfo.freeTimes <= 0 && m_WubeiFangInfo.remainYuanbaoBuyTimes <= 0)
        {
            Global.CreateFunctionIcon(1901);
            return;
        }

        if (m_WubeiFangInfo.freeTimes <= 0 && m_WubeiFangInfo.remainYuanbaoBuyTimes > 0 && m_WubeiFangInfo.costYuanbao > JunZhuData.Instance().m_junzhuInfo.yuanBao)
        {
            Global.CreateFunctionIcon(101);
            return;
        }

        m_ShopRollController.m_TargetBuyItem = this;

        WubeiFangBuy temp = new WubeiFangBuy() { type = m_Type };
		SocketHelper.SendQXMessage(temp, ProtoIndexes.C_WUBEIFANG_BUY,ProtoIndexes.S_WUBEIFANG_BUY_RESP);

        //Close guide.
        UIYindao.m_UIYindao.CloseUI();
    }

    public void PlayAnim()
    {
        m_AnimObject.SetActive(false);

        m_AnimObject.SetActive(true);
    }
}

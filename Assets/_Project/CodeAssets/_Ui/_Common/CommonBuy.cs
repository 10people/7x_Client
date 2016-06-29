using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

/// <summary>
/// Only use this in carriage.
/// </summary>
public class CommonBuy : Singleton<CommonBuy>
{
    /// <summary>
    /// 购买物品
    /// </summary>
    /// <param name="ingot">花费元宝数</param>
    /// <param name="times">购买个数/次数</param>
    /// <param name="p_buyItemStr">购买物品名字</param>
    /// <param name="doBuyDelegate">购买操作回调</param>
    public void ShowBuy(int ingot, int times, string p_buyItemStr, string p_buyItemDesc, DelegateHelper.VoidDelegate doBuyDelegate, int vip, int remainingTimes)
    {
        m_times = times;
        m_ingot = ingot;
        m_buyItemStr = p_buyItemStr;
        m_doBuyDelegate = doBuyDelegate;
        m_buyItemDesc = p_buyItemDesc;
        m_requiredVip = vip;
        m_remainingTimes = remainingTimes;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ConfirmBuy);
    }

    private string m_buyItemStr;
    private int m_ingot;
    private int m_times;
    private string m_buyItemDesc;
    private DelegateHelper.VoidDelegate m_doBuyDelegate;
    private int m_requiredVip;
    private int m_remainingTimes;

    public GameObject VipObject;
    public UISprite VipSprite;

    private void ConfirmBuy(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             m_buyItemDesc + "是否消耗" + m_ingot + "元宝购买" + m_times + m_buyItemStr + "?", null,
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
             ConfirmBuy, null, null, null, false, true, true, false, 100,
             0, m_requiredVip);
    }

    private void ConfirmBuy(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                if (JunZhuData.Instance().m_junzhuInfo.vipLv < m_requiredVip)
                {
                    ShowVIP();
                    return;
                }
                if (m_remainingTimes <= 0)
                {
                    ShowVIP();
                    return;
                }
                if (JunZhuData.Instance().m_junzhuInfo.yuanBao < m_ingot)
                {
                    ShowIngot();
                    return;
                }

                if (m_doBuyDelegate != null)
                {
                    m_doBuyDelegate();
                }
                break;
        }
    }

    public void ShowIngot()
    {
        Global.CreateFunctionIcon(101);
    }

    /// <summary>
    /// VIP限制
    /// </summary>
    /// <param name="vip">升级到的VIP</param>
    [Obsolete]
    public void ShowVIP(int vip)
    {
        m_vip = vip;

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ConfirmVIP);
    }

    public void ShowVIP()
    {
        Global.CreateFunctionIcon(1901);
    }

    private int m_vip;

    private void ConfirmVIP(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        //uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
        //     null, LanguageTemplate.GetText(1514),
        //     null,
        //     LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
        //     null);

        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             null, "升级到VIP" + m_vip + "可购买额外次数,\n是否前往查看VIP权限?",
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
             ConfirmVIP);
    }

    private void ConfirmVIP(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                EquipSuoData.TopUpLayerTip(null, false, 1, null, true);
                break;
        }
    }

    void OnDestroy()
    {
        base.OnDestroy();
    }
}
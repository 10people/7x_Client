using UnityEngine;
using System.Collections;

public class CommonRecharge : Singleton<CommonRecharge>
{
    public void ShowBuy(int ingot, int times, DelegateUtil.VoidDelegate doBuyDelegate)
    {
        m_times = times;
        m_ingot = ingot;
        m_doBuyDelegate = doBuyDelegate;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ConfirmBuy);
    }

    private int m_ingot;
    private int m_times;
    private DelegateUtil.VoidDelegate m_doBuyDelegate;

    private void ConfirmBuy(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             null, "是否消耗" + m_ingot + "元宝购买" + m_times + "次数",
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
             ConfirmBuy);
    }

    private void ConfirmBuy(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= m_ingot)
                {
                    if (m_doBuyDelegate != null)
                    {
                        m_doBuyDelegate();
                    }
                }
                else
                {
                    ShowRecharge();
                }
                break;
        }
    }

    private void ShowRecharge()
    {
        EquipSuoData.TopUpLayerTip();
    }

    

    public void ShowVIP(int vip)
    {
        m_vip = vip;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ConfirmVIP);
    }

    private int m_vip;

    private void ConfirmVIP(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
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

	void OnDestroy(){
		base.OnDestroy();
	}
}

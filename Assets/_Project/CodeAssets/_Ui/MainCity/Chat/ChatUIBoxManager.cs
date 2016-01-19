using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

/// <summary>
/// Base class in chat sys.
/// </summary>
public class ChatUIBoxManager : MonoBehaviour
{
    public ChatBaseWindow m_ChatBaseWindow;

    [HideInInspector]
    public long JunZhuIDToBeShielded = -1;

    public void SendMsgCD()
    {
        ClientMain.m_UITextManager.createText(MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_TOO_FAST)));
    }

    [HideInInspector]
    public string ShieldName = "";
    public void ShieldCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SHIELD).Replace("***", ShieldName),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            OnBoxShield);
    }

    public void ShieldSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             null, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_SHIELD),
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
             null);
    }

    [HideInInspector]
    public string AddFriendName = "";
    public void AddFriendSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             null, "您已成功添加" + AddFriendName + "到好友列表",
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
             null);
    }

    public void PayCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SEND_1) + "{0}" + LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SEND_2), PurchaseTemplate.GetBuyWorldChat_Price(1)),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            OnBoxPay);
    }

    public void ReChargeCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_RECHARGE),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            OnBoxReCharge);
    }

    public void NoAllianceCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_NO_ALLIANCE),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    public void NoTeneCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                     null, LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_NOTENE),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    private void OnBoxShield(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                JoinToBlacklist tempMsg = new JoinToBlacklist
                {
                    junzhuId = JunZhuIDToBeShielded
                };
                SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
                break;
            default:
                Debug.LogError("UIBox callback para:" + i + " is not correct.");
                break;
        }
    }

    private void OnBoxPay(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                m_ChatBaseWindow.m_ChatBaseSendController.SendMessageWithInputField();
                break;
            default:
                Debug.LogError("UIBox callback para:" + i + " is not correct.");
                break;
        }
    }

    private void OnBoxReCharge(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                //goto recharge.
                EquipSuoData.TopUpLayerTip();
                break;
            default:
                Debug.LogError("UIBox callback para:" + i + " is not correct.");
                break;
        }
    }
}

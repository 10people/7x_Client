using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class TimeCalc : Singleton<TimeCalc>
{
    public void StartOpenSendMessageCD(float time, ChatBaseWindow baseWindow)
    {
        StartCoroutine(OpenSendMessageCDCoroutine(time, baseWindow));
    }

    public void StopOpenSendMessageCD()
    {
        StopCoroutine("OpenSendMessageCDCoroutine");
    }

    private IEnumerator OpenSendMessageCDCoroutine(float time, ChatBaseWindow baseWindow)
    {
        yield return new WaitForSeconds(time);

        baseWindow.m_ChatBaseSendController.NotInSendMessageCD = true;
    }

    public void StartCheckMsgSucceedCoroutine(float time, ChatBaseWindow baseWindow, ChatBaseDataHandler dataHandler)
    {
        StartCoroutine(CheckMsgSucceedCoroutine(time, baseWindow, dataHandler));
    }

    public void StopCheckMsgSucceedCoroutine()
    {
        StopCoroutine("CheckMsgSucceedCoroutine");
    }

    private IEnumerator CheckMsgSucceedCoroutine(float time, ChatBaseWindow baseWindow, ChatBaseDataHandler dataHandler)
    {
        yield return new WaitForSeconds(time);

        if (!baseWindow.m_ChatBaseSendController.m_SendedChatStruct.isReceived)
        {
            ChatBaseDataHandler.ChatStruct tempChatStruct = baseWindow.m_ChatBaseSendController.m_SendedChatStruct;

            //Safety check.
            if (tempChatStruct.m_ChatLogItem != null)
            {
                PoolManagerListController.Instance.ReturnItem("ChatDataItem", tempChatStruct.m_ChatLogItem.gameObject);

                tempChatStruct.m_ChatLogItem = null;
            }

            dataHandler.storedChatStructList.Add(tempChatStruct);

            dataHandler.RestrictChatStructListNum();

            if (baseWindow.CurrentChannel == tempChatStruct.m_ChatPct.channel)
            {
                dataHandler.Refresh(2);
            }
        }

        baseWindow.m_ChatBaseSendController.m_SendedChatStruct = null;
    }
}

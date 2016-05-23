using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

public class ShopRollController : MonoBehaviour, SocketListener
{
    public List<ShopRollItem> m_RollItemList = new List<ShopRollItem>();

    public ShopRollItem m_TargetBuyItem;

    public void SetThis(WubeiFangInfoResp m_storedResp)
    {
        m_storedResp.wubeiFangInfoList.ForEach(infoItem =>
        {
            var temp = m_RollItemList.Where(rollItem => rollItem.m_Type == infoItem.type).ToList();
            if (temp.Any())
            {
                temp.First().m_WubeiFangInfo = infoItem;
                temp.First().SetThis();
            }
        });
    }

    public void SetThis(WubeiFangInfo infoItem)
    {
        var temp = m_RollItemList.Where(rollItem => rollItem.m_Type == infoItem.type).ToList();
        if (temp.Any())
        {
            temp.First().m_WubeiFangInfo = infoItem;
            temp.First().SetThis();
        }
    }

    private List<RewardData> rewardList = new List<RewardData>();

    public IEnumerator ShowRollResult(List<WubeiFangAwardInfo> info)
    {
        m_RollItemList.ForEach(item => item.IsCloseClickManually = true);

        if (m_TargetBuyItem != null)
        {
            m_TargetBuyItem.PlayAnim();
            m_TargetBuyItem = null;
        }

        yield return new WaitForSeconds(1.0f);

        rewardList = info.Select(item => new RewardData(item.itemId, item.itemNum)).ToList();

        MiBaoGlobleData.Instance().GetCommAwards(info.Select(item => item.itemId).ToList(), info.Select(item => item.itemNum).ToList(), ShowRollResult);
    }

    private void ShowRollResult()
    {
        GeneralRewardManager.Instance().CreateReward(rewardList);

        m_RollItemList.ForEach(item => item.IsCloseClickManually = false);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_WUBEIFANG_BUY_RESP:
                    {
                        object temp = new WubeiFangBuyResp();
                        SocketHelper.ReceiveQXMessage(ref temp, p_message, ProtoIndexes.S_WUBEIFANG_BUY_RESP);

                        var wubeiFangBuyResp = temp as WubeiFangBuyResp;

                        switch (wubeiFangBuyResp.result)
                        {
                            case 0:
                                {
                                    StopCoroutine("ShowRollResult");
                                    StartCoroutine(ShowRollResult(wubeiFangBuyResp.awardList));
                                    SetThis(wubeiFangBuyResp.wubeiFangInfo);

                                    break;
                                }
                            case 1:
                                {
                                    Global.CreateFunctionIcon(101);

                                    break;
                                }
                            case 2:
                                {
                                    Global.CreateFunctionIcon(1901);

                                    break;
                                }
                        }

                        return true;
                    }
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

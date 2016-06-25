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

        if (m_RollItemList.All(item => item.m_WubeiFangInfo.freeTimes <= 0))
        {
            if (ShopData.Instance.ShopBtnRedId(ShopData.ShopType.ROLL) != -1)
            {
                PushAndNotificationHelper.SetRedSpotNotification(ShopData.Instance.ShopBtnRedId(ShopData.ShopType.ROLL), false);

                ShopPage.shopPage.shopBtnList[0].transform.FindChild("Red").gameObject.SetActive(ShopData.Instance.IsBtnRed(ShopData.ShopType.ROLL));
            }
        }
    }

    private List<WubeiFangAwardInfo> rewardToShow = new List<WubeiFangAwardInfo>();
    private List<RewardData> rewardList = new List<RewardData>();
    private List<BagItem> bagItemList = new List<BagItem>();

    private void ShowRollResultAnim()
    {
        m_RollItemList.ForEach(item => item.IsCloseClickManually = true);

        if (m_TargetBuyItem != null)
        {
            m_TargetBuyItem.PlayAnim();
        }

        IsCheckClick = true;

        if (TimeHelper.Instance.IsTimeCalcKeyExist("ShopRollResult"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("ShopRollResult");
        }
        TimeHelper.Instance.AddOneDelegateToTimeCalc("ShopRollResult", 1f, EndRollResultAnim);
    }

    private void EndRollResultAnim()
    {
        if (TimeHelper.Instance.IsTimeCalcKeyExist("ShopRollResult"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("ShopRollResult");
        }

        IsCheckClick = false;

        if (m_TargetBuyItem != null)
        {
            m_TargetBuyItem.m_AnimObject.SetActive(false);
            m_TargetBuyItem = null;
        }

        ShowRollResultWindow();
    }

    private void ShowRollResultWindow()
    {
        rewardList = rewardToShow.Select(item => new RewardData(item.itemId, item.itemNum)).ToList();

        //MiBaoGlobleData.Instance().GetCommAwards(info.Select(item => item.itemId).ToList(), info.Select(item => item.itemNum).ToList(), ShowRollResult);
        bagItemList = rewardToShow.Select(item => new BagItem() { itemId = item.itemId, cnt = item.itemNum }).ToList();
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnRollResultLoadCallBack);
    }

    private void OnRollResultLoadCallBack(ref WWW www, string path, Object prefab)
    {
        UIBox uibox = (Instantiate(prefab) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("获得奖励",
            null, null,
            bagItemList,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            OnClickRollResultWindow, null, null, null, false, true, true, true);
        //Set smaller font size to reveal icon.
        uibox.m_labelDis1.fontSize = 23;

        UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.PopUI_2, uibox.gameObject, EffectIdTemplate.getEffectTemplateByEffectId(620246).path);
    }

    private void OnClickRollResultWindow(int i)
    {
        ShowRollResult();
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
                                    rewardToShow = wubeiFangBuyResp.awardList;
                                    ShowRollResultAnim();
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

    public bool IsCheckClick = false;

    void Update()
    {
        if (IsCheckClick)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                EndRollResultAnim();
            }
        }
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

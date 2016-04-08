using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                case ProtoIndexes.S_MengYouKuaiBao_Resq:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        PromptMSGResp tempInfo = new PromptMSGResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        if (tempInfo.msgList != null && tempInfo.msgList.Any())
                        {
                            Global.upDataTongzhiData(tempInfo.msgList.Select(item => new TongzhiData(item)).ToList());
                            m_RootManager.m_CarriageMain.m_MainCityUiTongzhi.upDataShow();
                            m_RootManager.m_CarriageMain.ExecuteReportData();
                        }
                        break;
                    }
                case ProtoIndexes.S_MengYouKuaiBao_PUSH:
                    {
                        MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                        SuBaoMSG msg = new SuBaoMSG();
                        xiongSerializer.Deserialize(stream, msg, msg.GetType());

                        Global.upDataTongzhiData(new List<TongzhiData>() { new TongzhiData(msg) });
                        m_RootManager.m_CarriageMain.m_MainCityUiTongzhi.upDataShow();
                        m_RootManager.m_CarriageMain.ExecuteReportData();
                        break;
                    }
                case ProtoIndexes.S_Prompt_Action_Resp:
                    {
                        MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                        PromptActionResp msg = new PromptActionResp();
                        xiongSerializer.Deserialize(stream, msg, msg.GetType());

                        if (msg.result != 10)
                        {
                            ClientMain.m_UITextManager.createText("通知已过时");

                            return true;
                        }

                        switch (msg.subaoType)
                        {
                            //transport to position.

                            //help carriage that under attack.
                            case 101:
                            case 102:
                                {
                                    m_RootManager.m_CarriageMain.TpToPosition(new Vector2(msg.posX, msg.posZ));

                                    break;
                                }
                            //join to carriage help list.
                            case 104:
                            case 105:
                                {
                                    ClientMain.m_UITextManager.createText("成功加入协助");
                                    m_RootManager.m_CarriageMain.TpToPosition(new Vector2(msg.posX, msg.posZ));
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }

                        if (msg.fujian == null || msg.fujian == "")
                        {
                            break;
                        }
                        string tempFujian = msg.fujian;
                        List<string> funjianlist = new List<string>();
                        while (tempFujian.IndexOf("#") != -1)
                        {
                            funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
                        }
                        funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
                        List<RewardData> RewardDataList = new List<RewardData>();
                        for (int i = 0; i < funjianlist.Count; i++)
                        {
                            tempFujian = funjianlist[i];
                            Global.NextCutting(ref tempFujian, ":");
                            RewardData Rewarddata = new RewardData(int.Parse(Global.NextCutting(ref tempFujian, ":")), int.Parse(Global.NextCutting(ref tempFujian, ":")));
                            RewardDataList.Add(Rewarddata);
                        }
                        GeneralRewardManager.Instance().CreateReward(RewardDataList);
                        break;
                    }
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
                                        //m_YabiaoJunZhuList.jieBiaoCiShu = temp.leftJBTimes;
                                        Debug.LogError("Confirmed, key deleted, will never run here again.");

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
                        return true;
                    }
                case ProtoIndexes.S_BIAOCHE_STATE:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BiaoCheState tempMsg = new BiaoCheState();
                        t_qx.Deserialize(t_tream, tempMsg, tempMsg.GetType());

                        //Refresh carriage info.
                        var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Value.GetComponent<CarriageBaseCultureController>().UID == tempMsg.uid).ToList();
                        if (temp.Any())
                        {
                            var temp2 = temp.First().Value.GetComponent<CarriageCultureController>();
                            if (temp2 != null)
                            {
                                temp2.ProgressPercent = (int)tempMsg.jindu;
                                temp2.ProtectStateRemaining = tempMsg.baohuCD / 1000;
                                temp2.SpeedStateRemaining = tempMsg.jiasuTime / 1000;
                                temp2.UpdateInfo();
                            }
                        }

                        return true;
                    }
                //Skill
                case ProtoIndexes.S_FIGHT_ATTACK_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        FightAttackResp tempInfo = new FightAttackResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        if (tempInfo.result != Result.SUCCESS)
                        {
                            switch (tempInfo.result)
                            {
                                case Result.SKILL_DISTANCE_ERROR:
                                    {
                                        ClientMain.m_UITextManager.createText("目标不在技能距离范围内");
                                        return true;
                                    }
                                //case Result.SKILL_COOL_TIME:
                                //    {
                                //        //Show cd if not normal attack.
                                //        if (tempInfo.skillId != 101)
                                //        {
                                //            ClientMain.m_UITextManager.createText("技能正在冷却中");
                                //        }
                                //        return true;
                                //    }
                                case Result.SKILL_NOT_EXIST:
                                    {
                                        ClientMain.m_UITextManager.createText("技能不存在");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.TARGET_NOT_EXIST:
                                    {
                                        ClientMain.m_UITextManager.createText("目标不存在");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.TARGET_IN_SAFE_AREA:
                                    {
                                        ClientMain.m_UITextManager.createText("目标在安全区");
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_SELF:
                                    {
                                        ClientMain.m_UITextManager.createText("不能对自己使用");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_OTHER:
                                    {
                                        ClientMain.m_UITextManager.createText("不能对他人使用");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_TEAMMATE:
                                    {
                                        ClientMain.m_UITextManager.createText("不能对友方使用");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_ENEMY:
                                    {
                                        ClientMain.m_UITextManager.createText("不能对敌方使用");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.CART_IN_PROTECT_TIME:
                                    {
                                        ClientMain.m_UITextManager.createText("本马使用了带保护效果的马具,在马具生效期间无法被攻击");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                                case Result.DAY_NOT_GET_AWARD_TIMES:
                                    {
                                        ClientMain.m_UITextManager.createText("今日的劫镖次数已用完,可与其他玩家切磋身手");
                                        m_RootManager.m_CarriageMain.TryCancelChaseToAttack();
                                        return true;
                                    }
                            }

                            return true;
                        }

                        //skill
                        if (tempInfo.skillId >= 0)
                        {
                            m_RootManager.m_CarriageMain.ExecuteSkill(tempInfo);
                        }
                        else
                        {
                            Debug.LogError("Received skill id is negative, please check.");
                        }

                        return true;
                    }
                //buff
                case ProtoIndexes.BUFFER_INFO:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BufferInfo tempInfo = new BufferInfo();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        m_RootManager.m_CarriageMain.ExecuteBuff(tempInfo);

                        return true;
                    }
                //Recover in safe area.
                case ProtoIndexes.SAFE_AREA_BOOLD_RETURN_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        SafeAreaBloodReturn tempInfo = new SafeAreaBloodReturn();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        m_RootManager.m_CarriageMain.ExecuteRecover(tempInfo);

                        return true;
                    }
                //I Request other help response
                case ProtoIndexes.S_YABIAO_HELP_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        YaBiaoHelpResp tempInfo = new YaBiaoHelpResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.code)
                        {
                            case 10:
                                ClientMain.m_UITextManager.createText("操作成功");
                                break;
                            case 20:
                                ClientMain.m_UITextManager.createText("操作失败");
                                break;
                        }

                        return true;
                    }
                //I Help other response
                case ProtoIndexes.S_ANSWER_YBHELP_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        AnswerYaBiaoHelpResp tempInfo = new AnswerYaBiaoHelpResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        return true;
                    }
                //My help list response
                case ProtoIndexes.S_YABIAO_XIEZHU_LIST_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        XieZhuJunZhuResp tempInfo = new XieZhuJunZhuResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        //Refresh my help list.
                        m_RootManager.m_CarriageMain.m_MyHelperListController.SetThis(tempInfo);

                        return true;
                    }
                //Other help me Notify
                case ProtoIndexes.S_ASK_YABIAO_HELP_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        AskYaBiaoHelpResp tempInfo = new AskYaBiaoHelpResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        if (tempInfo.jz != null)
                        {
                            if (m_RootManager.m_CarriageMain.m_MyHelperListController.m_StoredXieZhuJunZhuResp == null)
                            {
                                m_RootManager.m_CarriageMain.m_MyHelperListController.m_StoredXieZhuJunZhuResp = new XieZhuJunZhuResp();
                            }
                            if (m_RootManager.m_CarriageMain.m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz == null)
                            {
                                m_RootManager.m_CarriageMain.m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz = new List<XieZhuJunZhu>();
                            }
                            m_RootManager.m_CarriageMain.m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz.Add(tempInfo.jz);
                            m_RootManager.m_CarriageMain.m_MyHelperListController.UpdateGrids();
                        }

                        return true;
                    }
                //Speed up response
                case ProtoIndexes.S_CARTJIASU_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        JiaSuResp tempInfo = new JiaSuResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.resCode)
                        {
                            case 10:
                                ClientMain.m_UITextManager.createText("加速成功!");
                                m_RootManager.m_CarriageMain.WhipSkillController.TryStartSelfCD();
                                break;
                            case 20:
                                ClientMain.m_UITextManager.createText("加速失败!");
                                break;
                            case 30:
                                ClientMain.m_UITextManager.createText("镖马正在加速中, 请稍后再试!");
                                break;
                        }

                        return true;
                    }
                //Buy blood times.
                case ProtoIndexes.S_BUY_XUEPING_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BuyXuePingResp tempInfo = new BuyXuePingResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        m_RootManager.m_CarriageMain.IsCanClickBuyBloodTimes = true;

                        switch (tempInfo.resCode)
                        {
                            case 40:
                                if (tempInfo.remainTimes > 0)
                                {
                                    CommonBuy.Instance.ShowBuy(tempInfo.nextCost, tempInfo.nextGet, "血瓶", LanguageTemplate.GetText(1801).Replace("n", "5"), m_RootManager.m_CarriageMain.DoBuyBloodTimes);
                                }
                                else
                                {
                                    CommonBuy.Instance.ShowVIP(JunZhuData.Instance().m_junzhuInfo.vipLv + 1);
                                }

                                m_RootManager.m_CarriageMain.SetRemainingBloodNum(tempInfo.remainXuePing);
                                break;
                            default:
                                m_RootManager.m_CarriageMain.SetRemainingBloodNum(tempInfo.remainXuePing);
                                break;
                        }

                        return true;
                    }
                //Buy full rebirth time.
                case ProtoIndexes.S_BUY_FULL_REBIRTH_TIME_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BuyAllLifeReviveTimesResp tempInfo = new BuyAllLifeReviveTimesResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.result)
                        {
                            case 0:
                                CommonBuy.Instance.ShowBuy(tempInfo.cost, tempInfo.getTimes, "次数", "", m_RootManager.m_CarriageMain.DoBuyRebirthFullTime);
                                break;
                            case 2:
                                ClientMain.m_UITextManager.createText("元宝不足");
                                break;
                            case 3:
                                CommonBuy.Instance.ShowVIP(tempInfo.vipLevel);
                                break;
                            case 4:
                                ClientMain.m_UITextManager.createText("今日购买次数已用完");
                                break;
                            case 100:
                                ClientMain.m_UITextManager.createText("购买成功");
                                if (m_RootManager.m_CarriageMain.DeadWindow.activeInHierarchy)
                                {
                                    m_RootManager.m_CarriageMain.FullRebirthTimesLabel.text = "今日剩余满血复活次数" + ColorTool.Color_Red_c40000 + tempInfo.remainTimes + "[-]";
                                    m_RootManager.m_CarriageMain.QuickRebirthButton.isEnabled = tempInfo.remainTimes > 0;
                                    //m_RootManager.m_CarriageMain.QuickRebirthButton.UpdateColor(tempInfo.remainTimes > 0, true);
                                }
                                break;
                        }

                        return true;
                    }
                //Set whip type.
                case ProtoIndexes.S_GETMABIANTYPE_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        MaBianTypeResp tempInfo = new MaBianTypeResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        m_RootManager.m_CarriageMain.UpdateWhipCD(tempInfo.type == 2);

                        return true;
                    }
                //Set start carriage remaining time.
                case ProtoIndexes.S_YABIAO_MOREINFO_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        YaBiaoMoreInfoResp tempInfo = new YaBiaoMoreInfoResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.type)
                        {
                            case 1:
                                {
                                    m_RootManager.m_CarriageMain.RemainingStartCarriageTimes = tempInfo.count;
                                    break;
                                }
                            case 2:
                                {
                                    m_RootManager.m_CarriageMain.RemainingRobCarriageTimes = tempInfo.count;
                                    break;
                                }
                            case 3:
                                {
                                    m_RootManager.m_CarriageMain.RemainingAdditionalStartTimes = tempInfo.count;
                                    break;
                                }
                        }

                        return true;
                    }
                case ProtoIndexes.S_YABIAO_ENEMY_4_SIGN_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        YaBiaoEnemyResp tempInfo = new YaBiaoEnemyResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        m_RootManager.m_CarriageMain.ChourenList = tempInfo.enemy;
                        m_RootManager.m_CarriageMain.RefreshChourenState();

                        break;
                    }
                //Update I help other list.
                case ProtoIndexes.S_CHECK_YABIAOHELP_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        CheckYabiaoHelpResp tempInfo = new CheckYabiaoHelpResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        if (tempInfo.ybjzId != null)
                        {
                            m_RootManager.m_CarriageMain.m_IHelpOtherJunzhuIdList = tempInfo.ybjzId;
                        }
                        else
                        {
                            m_RootManager.m_CarriageMain.m_IHelpOtherJunzhuIdList.Clear();
                        }

                        return true;
                    }
                //Update alliance state
                case ProtoIndexes.ALLIANCE_STATE_NOTIFY:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        PlayerAllianceState tempInfo = new PlayerAllianceState();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        if (m_RootManager.m_SelfPlayerCultureController != null && m_RootManager.m_SelfPlayerCultureController.JunzhuID == tempInfo.junzhuId)
                        {
                            m_RootManager.UpdateSelfPlayerAlliance(tempInfo.allianceName, tempInfo.title);
                        }

                        var controllerList = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Values.Select(item => item.GetComponent<CarriageBaseCultureController>()).ToList();
                        controllerList.ForEach(item =>
                        {
                            if (item.JunzhuID == tempInfo.junzhuId)
                            {
                                item.AllianceName = tempInfo.allianceName;
                                item.AlliancePost = tempInfo.title;
                                item.SetThis();
                            }
                        });

                        return true;
                    }
                //Receive alliance tech info.
                case ProtoIndexes.S_LMKJ_INFO:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        JianZhuList tempInfo = new JianZhuList();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        //Get recover tech.
                        try
                        {
                            //204, recovering
                            m_RootManager.m_CarriageMain.m_SafeAresRecoveringLevel = tempInfo.list[12 - 1].lv;
                            m_RootManager.m_CarriageMain.SetRecoveringIcon(m_RootManager.m_CarriageMain.m_IsInSafeArea);
                            //205, carriage buff.
                            m_RootManager.m_CarriageMain.m_CarriageBuffLevel = tempInfo.list[13 - 1].lv;
                            m_RootManager.m_CarriageMain.SetCarriageBuffIcon(m_RootManager.m_CarriageMain.MyCarriageGameObject.activeInHierarchy);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error when try get alliance tech.");
                        }

                        return true;
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
                        EquipSuoData.TopUpLayerTip(null, false, 0, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_47));
                        // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ReChargeCallBack);
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
                TopUpLoadManagerment.LoadPrefab(true);
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
        //m_RootManager.m_CarriageUI.SetKingInfo(m_YabiaoJunZhuList.jieBiaoCiShu, 0);
    }

    private void OnGotoRecharge(int i)
    {
        switch (i)
        {
            case 1:
                TopUpLoadManagerment.LoadPrefab(true);
                break;
        }
    }

    #endregion

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        Global.ScendNull(ProtoIndexes.C_MengYouKuaiBao_Req);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}

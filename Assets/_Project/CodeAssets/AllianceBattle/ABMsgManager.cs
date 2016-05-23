using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllianceBattle;
using qxmobile.protobuf;

namespace AllianceBattle
{
    /// <summary>
    /// These code is not my intention, I write this cause I got severe suppression in my coding, I am not responsible for any OO design in this class.
    /// </summary>
    public class ABMsgManager : MonoBehaviour, SocketListener
    {
        public RootManager m_RootManager;

        public BattlefieldInfoResp m_BattlefieldInfoResp;

        // Use this for initialization
        void Awake()
        {
            SocketTool.RegisterSocketListener(this);
        }

        // Update is called once per frame
        void OnDestroy()
        {
            SocketTool.UnRegisterSocketListener(this);
        }

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    //case ProtoIndexes.S_MengYouKuaiBao_Resq:
                    //    {
                    //        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    //        QiXiongSerializer t_qx = new QiXiongSerializer();
                    //        PromptMSGResp tempInfo = new PromptMSGResp();
                    //        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                    //        if (tempInfo.msgList != null && tempInfo.msgList.Any())
                    //        {
                    //            Global.upDataTongzhiData(tempInfo.msgList.Select(item => new TongzhiData(item)).ToList());
                    //            m_RootManager.m_AllianceBattleMain.m_MainCityUiTongzhi.upDataShow();
                    //            m_RootManager.m_AllianceBattleMain.ExecuteReportData();
                    //        }
                    //        break;
                    //    }
                    //case ProtoIndexes.S_MengYouKuaiBao_PUSH:
                    //    {
                    //        MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    //        QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                    //        SuBaoMSG msg = new SuBaoMSG();
                    //        xiongSerializer.Deserialize(stream, msg, msg.GetType());

                    //        Global.upDataTongzhiData(new List<TongzhiData>() { new TongzhiData(msg) });
                    //        m_RootManager.m_AllianceBattleMain.m_MainCityUiTongzhi.upDataShow();
                    //        m_RootManager.m_AllianceBattleMain.ExecuteReportData();
                    //        break;
                    //    }
                    //case ProtoIndexes.S_Prompt_Action_Resp:
                    //    {
                    //        MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    //        QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                    //        PromptActionResp msg = new PromptActionResp();
                    //        xiongSerializer.Deserialize(stream, msg, msg.GetType());

                    //        if (msg.result != 10)
                    //        {
                    //            ClientMain.m_UITextManager.createText("通知已过时");

                    //            return true;
                    //        }

                    //        switch (msg.subaoType)
                    //        {
                    //            //transport to position.

                    //            //help AllianceBattle that under attack.
                    //            case 101:
                    //            case 102:
                    //                {
                    //                    m_RootManager.m_AllianceBattleMain.TpToPosition(new Vector2(msg.posX, msg.posZ));

                    //                    break;
                    //                }
                    //            //join to AllianceBattle help list.
                    //            case 104:
                    //            case 105:
                    //                {
                    //                    ClientMain.m_UITextManager.createText("成功加入协助");
                    //                    m_RootManager.m_AllianceBattleMain.TpToPosition(new Vector2(msg.posX, msg.posZ));
                    //                    break;
                    //                }
                    //            default:
                    //                {
                    //                    break;
                    //                }
                    //        }

                    //        if (msg.fujian == null || msg.fujian == "")
                    //        {
                    //            break;
                    //        }
                    //        string tempFujian = msg.fujian;
                    //        List<string> funjianlist = new List<string>();
                    //        while (tempFujian.IndexOf("#") != -1)
                    //        {
                    //            funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
                    //        }
                    //        funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
                    //        List<RewardData> RewardDataList = new List<RewardData>();
                    //        for (int i = 0; i < funjianlist.Count; i++)
                    //        {
                    //            tempFujian = funjianlist[i];
                    //            Global.NextCutting(ref tempFujian, ":");
                    //            RewardData Rewarddata = new RewardData(int.Parse(Global.NextCutting(ref tempFujian, ":")), int.Parse(Global.NextCutting(ref tempFujian, ":")));
                    //            RewardDataList.Add(Rewarddata);
                    //        }
                    //        GeneralRewardManager.Instance().CreateReward(RewardDataList);
                    //        break;
                    //    }
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
                                            return true;
                                        }
                                    case Result.TARGET_NOT_EXIST:
                                        {
                                            ClientMain.m_UITextManager.createText("目标不存在");
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
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_OTHER:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对他人使用");
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_TEAMMATE:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对友方使用");
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_ENEMY:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对敌方使用");
                                            return true;
                                        }
                                    case Result.CART_IN_PROTECT_TIME:
                                        {
                                            ClientMain.m_UITextManager.createText("本马使用了带保护效果的马具,在马具生效期间无法被攻击");
                                            return true;
                                        }
                                    case Result.DAY_NOT_GET_AWARD_TIMES:
                                        {
                                            ClientMain.m_UITextManager.createText("今日的劫镖次数已用完,可与其他玩家切磋身手");
                                            return true;
                                        }
                                }

                                return true;
                            }

                            //skill
                            if (tempInfo.skillId >= 0)
                            {
                                m_RootManager.m_AllianceBattleMain.ExecuteSkill(tempInfo);
                            }
                            else
                            {
                                Debug.LogError("Received skill id is negative, please check.");
                            }

                            return true;
                        }
                    //blood change
                    case ProtoIndexes.Life_Change:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            FightAttackResp tempInfo = new FightAttackResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.ExecuteBloodChange(tempInfo);

                            break;
                        }
                    //AOE skill
                    case ProtoIndexes.AOE_SKILL:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            AOESkill tempInfo = new AOESkill();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.ExecuteSkill(tempInfo);

                            break;
                        }
                    case ProtoIndexes.SKILL_STOP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            ErrorMessage tempInfo = new ErrorMessage();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.StopAOESkill(tempInfo);

                            break;
                        }
                    //buff
                    case ProtoIndexes.BUFFER_INFO:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BufferInfo tempInfo = new BufferInfo();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.ExecuteBuff(tempInfo);

                            return true;
                        }
                    //Buy blood times.
                    case ProtoIndexes.LMZ_BUY_XueP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BuyXuePingResp tempInfo = new BuyXuePingResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.IsCanClickBuyBloodTimes = true;

                            switch (tempInfo.resCode)
                            {
                                case 40:
                                    if (tempInfo.remainTimes > 0)
                                    {
                                        m_RootManager.m_AllianceBattleMain.ShowBuyItemWindow(ABBuyItemWindowManager.Type.Blood, tempInfo.nextCost, JunZhuData.Instance().m_junzhuInfo.yuanBao);
                                    }
                                    else
                                    {
                                        CommonBuy.Instance.ShowVIP(JunZhuData.Instance().m_junzhuInfo.vipLv + 1);
                                    }

                                    m_RootManager.m_AllianceBattleMain.SetRemainingBloodNum(tempInfo.remainXuePing);
                                    break;
                                default:
                                    m_RootManager.m_AllianceBattleMain.SetRemainingBloodNum(tempInfo.remainXuePing);
                                    break;
                            }

                            return true;
                        }
                    //Buy summon times.
                    case ProtoIndexes.LMZ_BUY_Summon:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BuyXuePingResp tempInfo = new BuyXuePingResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.IsCanClickBuySummonTimes = true;

                            switch (tempInfo.resCode)
                            {
                                case 40:
                                    if (tempInfo.remainTimes > 0)
                                    {
                                        m_RootManager.m_AllianceBattleMain.ShowBuyItemWindow(ABBuyItemWindowManager.Type.Summon, tempInfo.nextCost, JunZhuData.Instance().m_junzhuInfo.yuanBao);
                                    }
                                    else
                                    {
                                        CommonBuy.Instance.ShowVIP(JunZhuData.Instance().m_junzhuInfo.vipLv + 1);
                                    }

                                    m_RootManager.m_AllianceBattleMain.SetRemainingSummonNum(tempInfo.remainXuePing);
                                    break;
                                default:
                                    m_RootManager.m_AllianceBattleMain.SetRemainingSummonNum(tempInfo.remainXuePing);
                                    break;
                            }

                            return true;
                        }
                    //Score detail.
                    case ProtoIndexes.LMZ_SCORE_LIST:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ScoreList tempMsg = new ScoreList();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.m_ScoreDataList = tempMsg.list.Select(item => new ABScoreItemController.ScoreData()
                            {
                                Name = item.roleName,
                                Rank = item.rank,
                                ComboKill = item.lianSha,
                                TotalKill = item.killCnt,
                                Score = item.jiFen
                            }).ToList();

                            m_RootManager.m_AllianceBattleMain.ShowScoreWindow();

                            break;
                        }
                    //Commands.
                    case ProtoIndexes.LMZ_CMD_LIST:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            PromptMSGResp tempMsg = new PromptMSGResp();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.m_CmdItemDataList = tempMsg.msgList.Select(item => new ABCmdItemController.CmdItemData()
                            {
                                ID = (int)item.subaoId,
                                Desc = item.subao
                            }).ToList();

                            break;
                        }
                    case ProtoIndexes.LMZ_CMD_ONE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            ClientMain.m_UITextManager.createText(ColorTool.Color_Red_c40000 + tempMsg.errorDesc + "[-]");

                            break;
                        }
                    //Summon
                    case ProtoIndexes.LMZ_ZhaoHuan:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.ExecuteSummon(tempMsg.errorCode);

                            break;
                        }
                    //Battle stat
                    case ProtoIndexes.LMZ_SCORE_ONE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.UpdateBattleStat(tempMsg.cmd, tempMsg.errorCode);

                            break;
                        }
                    //Big buff
                    case ProtoIndexes.LMZ_ChengHao:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.AddBigBuff(tempMsg.errorCode);

                            break;
                        }
                    //skip skill.
                    case ProtoIndexes.POS_JUMP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            SpriteMove tempMsg = new SpriteMove();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            if (tempMsg.uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                            {
                                if (m_RootManager.m_SelfPlayerController != null)
                                {
                                    m_RootManager.m_SelfPlayerController.transform.localPosition = new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ);
                                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, tempMsg.dir, 0);
                                }
                            }
                            else
                            {
                                var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Where(item => item.Value.m_UID == tempMsg.uid);
                                if (temp.Any())
                                {
                                    int uID = temp.First().Key;
                                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[uID].transform.localPosition = new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ);
                                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[uID].transform.localEulerAngles = new Vector3(0, tempMsg.dir, 0);
                                }
                            }

                            //Update gizmos.
                            m_RootManager.m_AllianceBattleMain.m_MapController.UpdateGizmosPosition(tempMsg.uid, new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ), 0);

                            return true;
                        }
                    //[Obsolete]Set UI and player info.
                    case ProtoIndexes.ALLIANCE_BATTLE_FIELD_RESP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattlefieldInfoResp tempInfo = new BattlefieldInfoResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_BattlefieldInfoResp = tempInfo;

                            //Set character blood.
                            if (m_RootManager.m_SelfPlayerCultureController != null)
                            {
                                m_RootManager.m_SelfPlayerCultureController.TotalBlood = m_BattlefieldInfoResp.totalLife;
                                m_RootManager.m_SelfPlayerCultureController.RemainingBlood = m_BattlefieldInfoResp.remainLife;
                                m_RootManager.m_SelfPlayerCultureController.SetThis();
                            }
                            //Set character position.
                            m_RootManager.m_SelfPlayerController.transform.localPosition = new Vector3(m_BattlefieldInfoResp.posX, m_RootManager.m_SelfPlayerController.transform.localPosition.y, m_BattlefieldInfoResp.posZ);

                            return true;
                        }
                    //Refresh info.
                    case ProtoIndexes.ALLIANCE_BATTLE_FIELD_NOTIFY:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattlefieldInfoNotify tempInfo = new BattlefieldInfoNotify();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AbHoldPointManager.UpdateHoldPoints(tempInfo.campInfos);
                            m_RootManager.m_AllianceBattleMain.UpdateBattleTime(tempInfo.endRemainTime, tempInfo.winSide);

                            if (tempInfo.winSide < 0)
                            {
                                m_RootManager.m_AbHoldPointManager.TryAddShield();
                            }
                            else
                            {
                                m_RootManager.m_AbHoldPointManager.TryRemoveShield();
                            }

                            return true;
                        }
                    case ProtoIndexes.LMZ_OVER:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            ABResult tempInfo = new ABResult();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_RootManager.m_AllianceBattleMain.ShowBattleResult(tempInfo.IsSucceed, tempInfo.PersonalScore, tempInfo.Rank, tempInfo.KillNum, tempInfo.AllianceResult, tempInfo.GainItem);

                            break;
                        }
                    //[Obsolete]End battle msg.
                    case ProtoIndexes.ALLIANCE_BATTLE_RESULT:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattleResultAllianceFight tempInfo = new BattleResultAllianceFight();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            BattleControlor.BattleResult result = tempInfo.result ? BattleControlor.BattleResult.RESULT_WIN : BattleControlor.BattleResult.RESULT_LOSE;
                            List<Enums.Currency> currencyList = new List<Enums.Currency>();
                            List<int> numList = new List<int>();
                            tempInfo.awardItems.ForEach(item =>
                            {
                                currencyList.Add(Enums.Currency.GongXian);
                                numList.Add(item.awardNum);
                            });
                            int second = tempInfo.costTime;

                            EnterBattleResult.showBattleResult(result, currencyList, numList, second, second, m_RootManager.m_AllianceBattleMain.OnReturnClick);

                            return true;
                        }
                }
            }
            return false;
        }
    }
}
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

        [Obsolete]
        public BattlefieldInfoResp m_BattlefieldInfoResp;

        public DelegateHelper.VoidDelegate ExecuteAfterScoreList;

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
                    //            case 501:
                    //                {
                    //                    ClientMain.m_UITextManager.createText("成功加入协助");
                    //                    m_RootManager.m_AllianceBattleMain.TpToPosition(new Vector2(msg.posX, msg.posZ));
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
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                    case Result.TARGET_NOT_EXIST:
                                        {
                                            ClientMain.m_UITextManager.createText("目标不存在");
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_SELF:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对自己使用");
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_OTHER:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对他人使用");
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_TEAMMATE:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对友方使用");
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                    case Result.SKILL_TARGET_NOT_ENEMY:
                                        {
                                            ClientMain.m_UITextManager.createText("不能对敌方使用");
                                            m_RootManager.m_AllianceBattleMain.TryCancelChaseToAttack();
                                            return true;
                                        }
                                }

                                return true;
                            }

                            //skill
                            if (tempInfo.skillId >= 0)
                            {
                                //play been attack effect if hold.
                                if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(tempInfo.targetUid) && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[tempInfo.targetUid].IsHold && tempInfo.damage > 0)
                                {
                                    m_RootManager.m_AbHoldPointManager.TryPlayAlert(m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[tempInfo.targetUid].GetComponent<RPGBaseCultureController>().AlliancePost);
                                }

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
                    //[Obsolete]AOE skill
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

                            //m_RootManager.m_AllianceBattleMain.StopAOESkill(tempInfo.errorCode);

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
                                case 20:
                                case 40:
                                    m_RootManager.m_AllianceBattleMain.ShowBuyItemWindow(ABBuyItemWindowManager.Type.Blood, tempInfo.nextCost, tempInfo.remainTimes);
                                    break;
                                default:
                                    if (m_RootManager.m_AllianceBattleMain.m_AbBuyItemWindowManager.gameObject.activeInHierarchy)
                                    {
                                        m_RootManager.m_AllianceBattleMain.m_AbBuyItemWindowManager.SetThis(ABBuyItemWindowManager.Type.Blood, tempInfo.nextCost, tempInfo.remainTimes);
                                    }
                                    break;
                            }

                            m_RootManager.m_AllianceBattleMain.SetRemainingBloodNum(tempInfo.remainXuePing);

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
                                case 20:
                                case 40:
                                    m_RootManager.m_AllianceBattleMain.ShowBuyItemWindow(ABBuyItemWindowManager.Type.Summon, tempInfo.nextCost, tempInfo.remainTimes);
                                    break;
                                default:
                                    if (m_RootManager.m_AllianceBattleMain.m_AbBuyItemWindowManager.gameObject.activeInHierarchy)
                                    {
                                        m_RootManager.m_AllianceBattleMain.m_AbBuyItemWindowManager.SetThis(ABBuyItemWindowManager.Type.Summon, tempInfo.nextCost, tempInfo.remainTimes);
                                    }
                                    break;
                            }

                            m_RootManager.m_AllianceBattleMain.SetRemainingSummonNum(tempInfo.remainXuePing);

                            return true;
                        }
                    //Score detail.
                    case ProtoIndexes.LMZ_SCORE_LIST:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ScoreList tempMsg = new ScoreList();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            m_RootManager.m_AllianceBattleMain.m_AbScoreWindowController.DefenderBTN.SetActive(tempMsg.cityId >= 510201);
                            m_RootManager.m_AllianceBattleMain.m_AbScoreWindowController.CannotClickDefenderBTN.SetActive(tempMsg.cityId < 510201);

                            m_RootManager.m_AllianceBattleMain.m_AbScoreWindowController.m_ScoreDataList = tempMsg.list.Select(item => new ABScoreItemController.ScoreData()
                            {
                                Name = item.roleName,
                                Rank = item.rank,
                                ComboKill = item.lianSha,
                                TotalKill = item.killCnt,
                                Score = item.jiFen,
                                ID = item.jzId,
                                AllianceName = item.lmName,
                                GongXun = item.gx,
                                RoleID = item.roleId
                            }).ToList();

                            if (ExecuteAfterScoreList != null)
                            {
                                ExecuteAfterScoreList();
                                ExecuteAfterScoreList = null;
                            }

                            break;
                        }
                    //Command list.
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
                    //Command response.
                    case ProtoIndexes.LMZ_CMD_ONE:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            if (tempMsg.cmd == 20160530)
                            {
                                m_RootManager.m_AllianceBattleMain.StartCommandCDCalc((int)(float.Parse(tempMsg.errorDesc) / 1000));

                                if (tempMsg.errorCode == 1)
                                {
                                    m_RootManager.MyPart = 2;
                                }
                                else if (tempMsg.errorCode == 2)
                                {
                                    m_RootManager.MyPart = 1;
                                }

                                //Init after set part.
                                m_RootManager.m_AllianceBattleMain.RefreshAttackerGainedBuff();
                            }
                            else
                            {
                                m_RootManager.m_AllianceBattleMain.StartCommandCDCalc();

                                ClientMain.m_UITextManager.createText(tempMsg.errorDesc);
                                m_RootManager.m_AllianceBattleMain.ShowCommandMapEffect(tempMsg.errorCode);
                            }

                            break;
                        }
                    //Summon
                    case ProtoIndexes.LMZ_ZhaoHuan:
                        {
                            MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                            SuBaoMSG msg = new SuBaoMSG();
                            xiongSerializer.Deserialize(stream, msg, msg.GetType());

                            if (msg.otherJzId == -999 && msg.eventId == -501)
                            {
                                //set cd.
                                var triggeredSkill = m_RootManager.m_AllianceBattleMain.m_SkillControllers.Where(item => item.m_Index == 200).ToList();
                                if (triggeredSkill.Any())
                                {
                                    triggeredSkill.First().TryStartAmountOfSelfCD(float.Parse(msg.subao) / 1000, false);
                                }

                                //set play video config.
                                if (msg.configId == 0)
                                {
                                    //Play guide video.
                                    VideoHelper.PlayDramaVideo(EffectIdTemplate.GetPathByeffectId(700003), true);
                                }
                            }
                            else
                            {
                                m_RootManager.m_AllianceBattleMain.ExecuteSummon((int)msg.otherJzId, msg.subao);
                            }

                            break;
                        }
                    //Summon
                    case ProtoIndexes.LMZ_fenShen:
                        {
                            MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer xiongSerializer = new QiXiongSerializer();
                            ErrorMessage msg = new ErrorMessage();
                            xiongSerializer.Deserialize(stream, msg, msg.GetType());

                            //set cd.
                            var triggeredSkill = m_RootManager.m_AllianceBattleMain.m_SkillControllers.Where(item => item.m_Index == 300).ToList();
                            if (triggeredSkill.Any())
                            {
                                triggeredSkill.First().TryStartAmountOfSelfCD(msg.cmd / 1000f);
                            }

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
                    //Tulu buff
                    case ProtoIndexes.LMZ_ChengHao:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            ErrorMessage tempMsg = new ErrorMessage();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            //Add buff
                            if (tempMsg.cmd == 1)
                            {
                                m_RootManager.m_AllianceBattleMain.SetBigBuff(true, tempMsg.errorCode, int.Parse(tempMsg.errorDesc));
                            }
                            //add buff without notification
                            else if (tempMsg.cmd == 2)
                            {
                                m_RootManager.m_AllianceBattleMain.SetBigBuff(true, tempMsg.errorCode, int.Parse(tempMsg.errorDesc), false);
                            }
                            //Remove buff
                            else if (tempMsg.cmd == 0)
                            {
                                m_RootManager.m_AllianceBattleMain.SetBigBuff(false, tempMsg.errorCode, int.Parse(tempMsg.errorDesc));
                            }

                            break;
                        }
                    //Skip skill, use this for tp.
                    case ProtoIndexes.POS_JUMP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            SpriteMove tempMsg = new SpriteMove();
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            t_qx.Deserialize(t_stream, tempMsg, tempMsg.GetType());

                            //float yPos;
                            Vector3 tempPos = new Vector3(tempMsg.posX, RootManager.BasicYPosition, tempMsg.posZ);
                            //if (TransformHelper.RayCastXToFirstCollider(tempPos, out yPos))
                            //{
                            //    tempPos = new Vector3(tempMsg.posX, yPos, tempMsg.posZ);
                            //}

                            if (tempMsg.uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                            {
                                if (m_RootManager.m_SelfPlayerController != null)
                                {
                                    m_RootManager.m_SelfPlayerController.transform.localPosition = tempPos;
                                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, tempMsg.dir, 0);
                                }
                            }
                            else
                            {
                                var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Where(item => item.Value.m_UID == tempMsg.uid);
                                if (temp.Any())
                                {
                                    int uID = temp.First().Key;
                                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[uID].transform.localPosition = tempPos;
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

                            //m_RootManager.m_AbHoldPointManager.UpdateHoldPoints(tempInfo.campInfos);
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

                            m_RootManager.m_AllianceBattleMain.m_ABResultManager.ShowBattleResult(tempInfo.IsSucceed, tempInfo.PersonalScore, tempInfo.Rank, tempInfo.KillNum, tempInfo.lmGX);

                            break;
                        }
                    case ProtoIndexes.ALLIANCE_FIRE_NOTIFY:
                        {
                            m_RootManager.m_AllianceBattleMain.m_ABResultManager.ShowBattleResult(false, 0, 1, 0, 0, false);

                            break;
                        }
                }
            }
            return false;
        }
    }
}
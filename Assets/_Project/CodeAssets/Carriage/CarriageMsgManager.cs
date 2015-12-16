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
//                                        m_YabiaoJunZhuList.jieBiaoCiShu = temp.leftJBTimes;
						Debug.LogError( "Confirmed, key deleted, will never run here again." );

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
                        //TODO: replace with the new.
                        var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Value.GetComponent<CarriageBaseCultureController>().UID == tempMsg.uid).ToList();
                        if (temp.Any())
                        {
                            var temp2 = temp.First().Value.GetComponent<CarriageCultureController>();
                            if (temp2 != null)
                            {
                                temp2.ProgressPercent = (int)tempMsg.jindu;
                                temp2.ProtectStateRemaining = tempMsg.baohuCD;
                                temp2.SpeedStateRemaining = tempMsg.jiasuTime;
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
                                case Result.SKILL_COOL_TIME:
                                    {
                                        ClientMain.m_UITextManager.createText("技能正在冷却中");
                                        return true;
                                    }
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
                                        ClientMain.m_UITextManager.createText("技能不能对自己使用");
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_OTHER:
                                    {
                                        ClientMain.m_UITextManager.createText("技能不能对他人使用");
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_TEAMMATE:
                                    {
                                        ClientMain.m_UITextManager.createText("技能不能对友方使用");
                                        return true;
                                    }
                                case Result.SKILL_TARGET_NOT_ENEMY:
                                    {
                                        ClientMain.m_UITextManager.createText("技能不能对敌方使用");
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
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ReChargeCallBack);
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
                TopUpLoadManagerment.m_instance.LoadPrefab(true);
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
                TopUpLoadManagerment.m_instance.LoadPrefab(true);
                break;
        }
    }

    #endregion

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}

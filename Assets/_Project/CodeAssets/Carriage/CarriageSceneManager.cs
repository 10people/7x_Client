using System;
using UnityEngine;
using System.Collections;
using System.IO;
using Carriage;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class CarriageSceneManager : Singleton<CarriageSceneManager>, SocketListener
{
    public int s_RoomId
    {
        get { return s_roomId; }
        set
        {
            s_roomId = value;
            isRoomIdSetted = true;
        }
    }

    private int s_roomId;
    private bool isRoomIdSetted;

    public YabiaoJunZhuList s_YabiaoJunZhuList;

    public RootManager m_RootManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="l_roomId"></param>
    public void EnterCarriage(int l_roomId)
    {
        s_RoomId = l_roomId;

        EnterYaBiaoScene temp = new EnterYaBiaoScene { roomId = s_RoomId };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_ENTER_YABIAOSCENE);

        SceneManager.EnterCarriage();
    }

    public void ReturnCarriage(bool isPlayCarriageDead = false)
    {
        if (!isRoomIdSetted)
        {
            Debug.LogError("Never entered a carriage scene, cannot return.");
            return;
        }

        RootManager.IsGotoLastPosition = true;
        RootManager.IsPlayCarriageDead = isPlayCarriageDead;
        EnterCarriage(s_RoomId);
    }

    public void ReturnMainCity()
    {
        //if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        //{
            SceneManager.EnterMainCity();
        //}
        //else
        //{
        //    SceneManager.EnterAllianceCity();
        //}
    }

    /// <summary>
    /// Init all carriages, clear all and initialize, used when first enter scene.
    /// </summary>
    public void InitAllCarriages()
    {
        if (m_RootManager == null)
        {
            Debug.LogError("Can not init all carriages when carriage root not initialized.");
            return;
        }

        m_RootManager.m_CarriageManager.InitAllCarriages();
        m_RootManager.m_CarriageUi.InitCarriageGrid();

        //Refresh carriage self info.
        m_RootManager.m_CarriageUi.SetKingInfo(s_YabiaoJunZhuList.jieBiaoCiShu, s_YabiaoJunZhuList.lengqueCD);
    }

    /// <summary>
    /// Refresh a carriage(include destroy), used when refresh info from server.
    /// </summary>
    public void RefreshACarriage(BiaoCheState l_biaoCheState)
    {
        Debug.Log("Refresh a carriage:" + l_biaoCheState.junZhuId);

        if (m_RootManager == null)
        {
            Debug.LogError("Can not refresh a carriage when carriage root not initialized.");
            return;
        }

        ////Destroy carriage entrance.
        //if (l_biaoCheState.state == 40 || l_biaoCheState.state == 50)
        //{
        //    m_RootManager.DestroyACarriage((int)l_biaoCheState.junZhuId);
        //    return;
        //}

        m_RootManager.m_CarriageManager.RefreshACarriage(l_biaoCheState);
        m_RootManager.m_CarriageUi.RefreshCarriageUIItem(l_biaoCheState);
    }

    /// <summary>
    /// Init a carriage, used when add a new carriage in the midway.
    /// </summary>
    public void InitACarriage(YabiaoJunZhuInfo l_yabiaoJunZhuInfo)
    {
        Debug.Log("Init a carriage:" + l_yabiaoJunZhuInfo.junZhuId);

        if (m_RootManager == null)
        {
            Debug.LogError("Can not init a carriage when carriage root not initialized.");
            return;
        }

        m_RootManager.m_CarriageManager.InitACarriage(l_yabiaoJunZhuInfo);
        m_RootManager.m_CarriageUi.InitACarriage(l_yabiaoJunZhuInfo);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //Init carriage list info.
                case ProtoIndexes.S_BIAOCHE_INFO_RESP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        YabiaoJunZhuList temp = new YabiaoJunZhuList();
                        t_qx.Deserialize(t_tream, temp, temp.GetType());

                        if (Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE))
                        {
                            Debug.LogWarning("Do not sync s_YabiaoJunZhuList in CarriageSceneManager cause not initialized.");
                            return false;
                        }
                        else
                        {
                            s_YabiaoJunZhuList = temp;
                            InitAllCarriages();

                            //Refresh mibao skill choose effect.
                            m_RootManager.m_CarriageUi.RefreshMibaoSkillEffect();

                            return true;
                        }

                        break;
                    }
                //Update carriage info.
                case ProtoIndexes.S_BIAOCHE_STATE:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BiaoCheState temp = new BiaoCheState();
                        t_qx.Deserialize(t_tream, temp, temp.GetType());

                        if (s_YabiaoJunZhuList == null || Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE))
                        {
                            Debug.LogWarning("Do not sync BiaoCheState in CarriageSceneManager cause not initialized.");
                            return false;
                        }
                        else
                        {
                            RefreshACarriage(temp);
                            return true;
                        }

                        break;
                    }
                //Init a  carriage.
                case ProtoIndexes.S_YABIAO_ENTER_RESP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        YabiaoJunZhuInfo temp = new YabiaoJunZhuInfo();
                        t_qx.Deserialize(t_tream, temp, temp.GetType());

                        if (Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE))
                        {
                            Debug.LogWarning("Do not sync BiaoCheState in CarriageSceneManager cause not initialized.");
                            return false;
                        }
                        else
                        {
                            InitACarriage(temp);
                            return true;
                        }

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
                                        s_YabiaoJunZhuList.jieBiaoCiShu = temp.leftJBTimes;

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
                        break;
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

        if (s_YabiaoJunZhuList.buyCiShu + 1 <= totalTimes)
        {
            PurchaseTemplate template = PurchaseTemplate.GetBuyRobCarriageTime(s_YabiaoJunZhuList.buyCiShu + 1);

            if (template.price > JunZhuData.Instance().m_junzhuInfo.yuanBao)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnNotEnoughIngot);
                return;
            }

            buyCost = template.price;

            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_46).Replace("X", template.price.ToString()).Replace("Y", template.number.ToString()).Replace("N", (totalTimes - s_YabiaoJunZhuList.buyCiShu).ToString()),
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
        m_RootManager.m_CarriageUi.SetKingInfo(s_YabiaoJunZhuList.jieBiaoCiShu, 0);
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

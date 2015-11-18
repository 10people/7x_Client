using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

public class SmallHouseSelf : HouseBasic, IHouseSelf, SocketListener
{
    [HideInInspector]
    public bool IsBigHouseExist = false;

    public SmallHouseSelfOperation m_SmallHouseSelfOperation;
    public SmallHouseSelfEnter m_SmallHouseSelfEnter;

    [HideInInspector]
    public HouseExpInfo m_HouseExpInfo;

    public override void ShowTreasure()
    {
        if (m_HouseSecretCardShower != null)
        {
            m_HouseSecretCardShower.gameObject.SetActive(true);
            m_HouseSecretCardShower.SetSecretCard(m_TreasureShowed);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HOUSE_SECRET_CARD_SHOWER), OnTreasureLoadCallBack);
        }
    }

    void OnTreasureLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        var tempObject = Instantiate(loadedObject as GameObject) as GameObject;
        m_HouseSecretCardShower = tempObject.GetComponent<HouseSecretCardShower>();
        m_HouseSecretCardShower.m_House = this;

        TransformHelper.ActiveWithStandardize(transform, m_HouseSecretCardShower.transform);

        ShowTreasure();
    }

    public override void ShowWeapon()
    {
        if (m_HouseWeaponShower != null)
        {
            m_HouseWeaponShower.gameObject.SetActive(true);
            m_HouseWeaponShower.SetWindow(m_WeaponShowed);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HOUSE_WEAPON_SHOWER), OnWeaponLoadCallBack);
        }
    }

    void OnWeaponLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        var tempObject = Instantiate(loadedObject as GameObject) as GameObject;
        m_HouseWeaponShower = tempObject.GetComponent<HouseWeaponShower>();
        m_HouseWeaponShower.m_House = this;

        TransformHelper.ActiveWithStandardize(transform, m_HouseWeaponShower.transform);

        ShowWeapon();
    }

    public override void OnBookClick()
    {
        if (m_OldBookWindow != null)
        {
            m_OldBookWindow.gameObject.SetActive(true);
            m_OldBookWindow.OldBookMode = OldBookWindow.Mode.OldBookSelf;
            m_OldBookWindow.RefreshUI();
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.OLD_BOOK_WINDOW), OnBookLoadCallBack);
        }
    }

    void OnBookLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        var tempObject = Instantiate(loadedObject as GameObject) as GameObject;
        m_OldBookWindow = tempObject.GetComponent<OldBookWindow>();
        m_OldBookWindow.IsSelfHouse = true;
        m_OldBookWindow.m_House = this;

        TransformHelper.ActiveWithStandardize(transform, m_OldBookWindow.transform);

        OnBookClick();
    }

    public override void OnEnterClick()
    {
        m_SmallHouseSelfEnter.gameObject.SetActive(false);
        MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.HouseSelf;

        EnterOrExitHouse temp = new EnterOrExitHouse();
        temp.houseId = m_HouseSimpleInfo.jzId;
        temp.code = 10;
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_EnterOrExitHouse);

        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene
        {
            senderName = SystemInfo.deviceName,
            uid = 0,
            posX = 0,
            posY = 0,
            posZ = 0
        };
        SocketHelper.SendQXMessage(tempEnterScene, ProtoIndexes.Enter_HouseScene);

        SceneManager.EnterHouse();
    }

    public override void OnExitClick()
    {
        HousePlayerController.s_HousePlayerController.m_CompleteNavDelegate = DoExit;
        base.OnExitClick();
    }

    private void DoExit()
    {
        Destroy(m_HouseRootManager.m_mainCityUi.gameObject);
        Destroy(gameObject);
        MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.MainCity;

        EnterOrExitHouse temp = new EnterOrExitHouse();
        temp.houseId = m_HouseSimpleInfo.jzId;
        temp.code = 20;
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_EnterOrExitHouse);

        //ExitScene tempExitScene = new ExitScene
        //{
        //    uid = HousePlayerController.s_uid,
        //};
        //SocketHelper.SendQXMessage(tempExitScene, ProtoIndexes.Exit_HouseScene);

        SceneManager.EnterAllianceCity();
    }

    public void OnOperationClick()
    {
        m_SmallHouseSelfOperation.gameObject.SetActive(true);

        m_SmallHouseSelfOperation.RefreshWindow();
    }

    public void OnFitmentOrDonateClick()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), FitmentCallBack);
    }

    public void OnSwitchDoorClick()
    {
        m_SmallHouseSelfOperation.IsLock = !m_SmallHouseSelfOperation.IsLock;
        m_SmallHouseSelfOperation.SendSwitchMsg();
    }

    public void OnSwitchSellClick()
    {
        if (m_SmallHouseSelfOperation.HouseState == 10)
        {
            m_SmallHouseSelfOperation.HouseState = 20;
            m_SmallHouseSelfOperation.SendSwitchMsg();
        }
        else if (m_SmallHouseSelfOperation.HouseState == 20)
        {
            m_SmallHouseSelfOperation.HouseState = 10;
            m_SmallHouseSelfOperation.SendSwitchMsg();
        }
        //can't switch sell or living state cause sell setted by leader
        else if (m_SmallHouseSelfOperation.HouseState == 505)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CantSetLivingCallBack);
            return;
        }
    }

    /// <summary>
    /// can't switch sell or living state cause sell setted by leader
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    private void CantSetLivingCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("修改失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_CANT_SWITCH1),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    /// <summary>
    /// kick out visitor
    /// </summary>
    public void OnGetOutClick()
    {
        m_SmallHouseSelfOperation.OnPopOKBTNClick(null);
        var playerInfo = m_SmallHouseSelfOperation.m_playerInfoList[
            m_SmallHouseSelfOperation.m_HouseVisitorControllerList.IndexOf(
                m_SmallHouseSelfOperation.SelectedVisitorController)];

        OffVisitorInfo temp = new OffVisitorInfo();
        temp.code = 20;
        temp.houseId = m_HouseSimpleInfo.jzId;
        temp.visitorId = playerInfo.jzId;
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_SHOT_OFF_HOUSE_VISITOR);

        m_SmallHouseSelfOperation.m_playerInfoList.Remove(playerInfo);
        m_SmallHouseSelfOperation.RefreshVisitorsGrid();
    }

    public void OnReceiveClick()
    {
        if (m_HouseExpInfo.cur == 0)
        {
            return;
        }

        SocketHelper.SendQXMessage(ProtoIndexes.C_GET_SMALLHOUSE_EXP);
    }

    public void FitmentCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        //get error when cool time not over.
        if (m_HouseExpInfo.coolTime > 0)
        {
            Debug.LogError("Fitment not cool down over.");
            return;
        }

        //left time info.
        if (m_HouseExpInfo.leftUpTimes <= 0)
        {
            if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipTemplate.templates.Max(item => item.lv))
            {
                UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
                uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
                uibox.setBox("装修房屋",
                    null, string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_FITMENT_COOL_DOWN1), JunZhuData.Instance().m_junzhuInfo.vipLv + 1, VipTemplate.templates.FirstOrDefault(item => item.lv == (JunZhuData.Instance().m_junzhuInfo.vipLv + 1)).houseFitmentNum + 3),
                    null,
                    LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                    null);
            }
            else
            {
                UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
                uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
                uibox.setBox("装修房屋",
                    null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_FITMENT_COOL_DOWN2),
                    null,
                    LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                    null);
            }
        }
        else
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox("房屋装修",
                null, string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_FITMENT1).Replace("N", "{0}").Replace("xxx", "{1}").Replace("yyy", "{2}").Replace("n", "{3}"), m_HouseExpInfo.curGongxian, m_HouseExpInfo.needGongXian, m_HouseExpInfo.gainHouseExp, m_HouseExpInfo.leftUpTimes),
                null,
                LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                BoxFitment);
        }
    }

    void BoxFitment(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                //Contributation not enough.
                if (AllianceData.Instance.g_UnionInfo.contribution < m_HouseExpInfo.needGongXian)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ContributationNotEnoughCallBack);
                    return;
                }

                SocketHelper.SendQXMessage(ProtoIndexes.C_UP_HOUSE);

                m_SmallHouseSelfOperation.ShowFitmentPopLabel();
                break;
        }
    }

    /// <summary>
    /// can't fitment cause contributation not enough
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    private void ContributationNotEnoughCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("贡献不足",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.SMALL_HOUSE_FITMENT_NO_CONTRIBUTATION),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //small hosue exp result
                case ProtoIndexes.S_GET_HOUSE_EXP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        HouseExpInfo houseExpInfo = new HouseExpInfo();
                        t_qx.Deserialize(t_tream, houseExpInfo, houseExpInfo.GetType());

                        TenementData.Instance.m_AllianceCityTenementExp = houseExpInfo;

                        m_SmallHouseSelfOperation.RefreshWindow();
                        return true;
                    }
            }
        }
        return false;
    }

    void Awake()
    {
        base.Awake();
        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        base.OnDestroy();
        SocketTool.UnRegisterSocketListener(this);
    }
}

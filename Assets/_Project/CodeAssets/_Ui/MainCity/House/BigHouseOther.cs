using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

/// <summary>
/// 
/// </summary>
public class BigHouseOther : HouseBasic, SocketListener
{
    public BigHouseOtherEnter m_BigHouseOtherEnter;

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
            m_OldBookWindow.OldBookMode = OldBookWindow.Mode.ExchangeBoxOther;
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
        m_OldBookWindow.IsSelfHouse = false;
        m_OldBookWindow.m_House = this;

        TransformHelper.ActiveWithStandardize(transform, m_OldBookWindow.transform);

        OnBookClick();
    }

    public override void OnEnterClick()
    {
        //Can't enter.
        if (!m_HouseSimpleInfo.open4my)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), DoorLockCallBack);
            return;
        }

        m_BigHouseOtherEnter.gameObject.SetActive(false);
        MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.HouseOther;

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

    /// <summary>
    /// can't enter cause door lock
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void DoorLockCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("禁止进入",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_LOCK1),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    /// <summary>
    /// been kicked out
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    void KickOutCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("离开房间",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_BEEN_KICKED1),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            KickOut);
    }

    void KickOut(int i)
    {
        switch (i)
        {
            case 1:
                OnExitClick();
                break;
        }
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

        ////SceneManager.EnterAllianceCity();
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //been kicked out
                case ProtoIndexes.S_SHOT_OFF_HOUSE_VISITOR:
                    MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    OffVisitorInfo offVisitorInfo = new OffVisitorInfo();
                    t_qx.Deserialize(t_tream, offVisitorInfo, offVisitorInfo.GetType());

                    if (offVisitorInfo.code == 10 && offVisitorInfo.visitorId == JunZhuData.Instance().m_junzhuInfo.id && offVisitorInfo.houseId == m_HouseSimpleInfo.jzId)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), KickOutCallBack);
                        return true;
                    }
                    break;
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

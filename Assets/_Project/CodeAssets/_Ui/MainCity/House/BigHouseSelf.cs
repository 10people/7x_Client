using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

/// <summary>
/// 
/// </summary>
public class BigHouseSelf : HouseBasic, IHouseSelf, SocketListener
{
    public BigHouseSelfOperation m_BigHouseSelfOperation;
    public BigHouseSelfEnter m_BigHouseSelfEnter;

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

        UtilityTool.ActiveWithStandardize(transform, m_HouseSecretCardShower.transform);

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

        UtilityTool.ActiveWithStandardize(transform, m_HouseWeaponShower.transform);

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

        UtilityTool.ActiveWithStandardize(transform, m_OldBookWindow.transform);

        OnBookClick();
    }

    public override void OnEnterClick()
    {
        m_BigHouseSelfEnter.gameObject.SetActive(false);
        MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.HouseSelf;

        EnterOrExitHouse temp = new EnterOrExitHouse();
        temp.houseId = m_HouseSimpleInfo.jzId;
        temp.code = 10;
        UtilityTool.SendQXMessage(temp, ProtoIndexes.C_EnterOrExitHouse);

        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene
        {
            senderName = SystemInfo.deviceName,
            uid = 0,
            posX = 0,
            posY = 0,
            posZ = 0
        };
        UtilityTool.SendQXMessage(tempEnterScene, ProtoIndexes.Enter_HouseScene);

        SceneManager.EnterHouse();
    }

    public override void OnExitClick()
    {
		HousePlayerController.s_HousePlayerController.m_CompleteNavDelegate = DoExit;
        base.OnExitClick();

    }

	private void DoExit()
	{
				Destroy (m_HouseRootManager.m_mainCityUi.gameObject);
				Destroy (gameObject);
				MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.MainCity;
		
				EnterOrExitHouse temp = new EnterOrExitHouse ();
				temp.houseId = m_HouseSimpleInfo.jzId;
				temp.code = 20;
				UtilityTool.SendQXMessage (temp, ProtoIndexes.C_EnterOrExitHouse);

                //ExitScene tempExitScene = new ExitScene
                //{
                //    uid = HousePlayerController.s_uid,
                //};
                //UtilityTool.SendQXMessage(tempExitScene, ProtoIndexes.Exit_HouseScene);

				SceneManager.EnterAllianceCity ();
		}

    public void OnOperationClick()
    {
        m_BigHouseSelfOperation.gameObject.SetActive(true);

        m_BigHouseSelfOperation.RefreshWindow();
    }

    /// <summary>
    /// donate to alliance
    /// </summary>
    public void OnFitmentOrDonateClick()
    {
        AllianceData.Instance.RequestData();
        if (!AllianceData.Instance.IsAllianceNotExist)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT), AllianceHaveLoadCallback);
        }
    }

    private void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }

        //GO to donation.
        MyAllianceMain temp = tempObject.GetComponent<MyAllianceMain>();
        temp.OpenDonationBtn();
    }

    /// <summary>
    /// switch door open or lock
    /// </summary>
    public void OnSwitchDoorClick()
    {
        m_BigHouseSelfOperation.IsLock = !m_BigHouseSelfOperation.IsLock;
        m_BigHouseSelfOperation.SendSwitchMsg();
    }

    /// <summary>
    /// empty in big house
    /// </summary>
    public void OnSwitchSellClick()
    {

    }

    /// <summary>
    /// kick out visitor
    /// </summary>
    public void OnGetOutClick()
    {
        m_BigHouseSelfOperation.OnPopOKBTNClick(null);
        var playerInfo = m_BigHouseSelfOperation.m_playerInfoList[
            m_BigHouseSelfOperation.m_HouseVisitorControllerList.IndexOf(
                m_BigHouseSelfOperation.SelectedVisitorController)];

        OffVisitorInfo temp = new OffVisitorInfo();
        temp.code = 20;
        temp.houseId = m_HouseSimpleInfo.jzId;
        temp.visitorId = playerInfo.jzId;
        UtilityTool.SendQXMessage(temp, ProtoIndexes.C_SHOT_OFF_HOUSE_VISITOR);

        m_BigHouseSelfOperation.m_playerInfoList.Remove(playerInfo);
        m_BigHouseSelfOperation.RefreshVisitorsGrid();
    }

    public void OnReceiveClick()
    {
        if (m_HouseExpInfo.cur == 0)
        {
            return;
        }

        UtilityTool.SendQXMessage(ProtoIndexes.C_GET_BIGHOUSE_EXP);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //server send house exp
                case ProtoIndexes.S_GET_HOUSE_EXP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        HouseExpInfo houseExpInfo = new HouseExpInfo();
                        t_qx.Deserialize(t_tream, houseExpInfo, houseExpInfo.GetType());

                        TenementData.Instance.m_AllianceCityTenementExp = houseExpInfo;

                        m_BigHouseSelfOperation.RefreshWindow();
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

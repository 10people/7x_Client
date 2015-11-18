using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class SmallHouseSelfOperation : MonoBehaviour, SocketListener
{
    #region General

    public SmallHouseSelf m_SmallHouseSelf;
    public UIEventListener CloseListener;

    public void RefreshWindow()
    {
        //Get data.
        m_SmallHouseSelf.m_HouseSimpleInfo =
            TenementData.Instance
                .m_AllianceCityTenementDic.FirstOrDefault(item => item.Key == m_SmallHouseSelf.m_HouseSimpleInfo.locationId).Value;
        m_SmallHouseSelf.m_HouseExpInfo = TenementData.Instance.m_AllianceCityTenementExp;

        IsLock = !m_SmallHouseSelf.m_HouseSimpleInfo.open4my;
        HouseState = m_SmallHouseSelf.m_HouseSimpleInfo.state;

        //Set left bar 1.
        totalValueBar1 = m_SmallHouseSelf.m_HouseExpInfo.lvupNeedExp;
        currentValueBar1 = m_SmallHouseSelf.m_HouseExpInfo.curExp;
        LeftBar1Left.text = "Lv" + m_SmallHouseSelf.m_HouseExpInfo.level;
        LeftBar1Right.text = "Lv" + (m_SmallHouseSelf.m_HouseExpInfo.level + 1);
        FitmentPopLabel.text = "+" + m_SmallHouseSelf.m_HouseExpInfo.gainHouseExp;

        RefreshFitmentCoolDown();

        //Set left bar 2.
        if (!m_SmallHouseSelf.IsBigHouseExist)
        {
            OpenCloseObjectRoot.SetActive(true);

            LeftPart2Info.SetActive(false);
            LeftBar2.gameObject.SetActive(true);
            LeftLabel2.gameObject.SetActive(true);
            LeftBTN2.gameObject.SetActive(true);

            totalValueBar2 = m_SmallHouseSelf.m_HouseExpInfo.max;
            currentValueBar2 = m_SmallHouseSelf.m_HouseExpInfo.cur;
            LeftBar2Left.text = "0";
            LeftBar2Right.text = m_SmallHouseSelf.m_HouseExpInfo.max.ToString();
        }
        else
        {
            OpenCloseObjectRoot.SetActive(false);

            LeftPart2Info.SetActive(true);
            LeftBar2.gameObject.SetActive(false);
            LeftLabel2.gameObject.SetActive(false);
            LeftBTN2.gameObject.SetActive(false);
        }
        SetBars();

        //Set Right part.
        if (!m_SmallHouseSelf.IsBigHouseExist)
        {
            RightPartInfo.SetActive(false);

            SocketHelper.SendQXMessage(ProtoIndexes.C_GET_HOUSE_VISITOR);
        }
        else
        {
            RightPartInfo.SetActive(true);
        }

        //Set left btn 1 according to house fitment left times.
        LeftBTN1.GetComponent<UIButton>().isEnabled = m_SmallHouseSelf.m_HouseExpInfo.leftUpTimes > 0;

        //Set left btn 2.
        if (!m_SmallHouseSelf.IsBigHouseExist)
        {
            LeftBTN2.GetComponent<UIButton>().isEnabled = m_SmallHouseSelf.m_HouseExpInfo.cur != 0;
        }
    }

    private void OnCloseClick(GameObject go)
    {
        //[FIX]close
        Destroy(m_SmallHouseSelf.gameObject);
        //gameObject.SetActive(false);
    }

    void OnEnable()
    {
        CloseListener.onClick = OnCloseClick;
        LeftBTN1.onClick = OnFitmentClick;
        LeftBTN2.onClick = OnReceiveClick;
        PopGetOutListener.onClick = OnPopGetOutBTNClick;
        PopOkListener.onClick = OnPopOKBTNClick;

        SellingLivingLis.onClick = OnSellLivingClick;
        OpenCloseLis.onClick = OnOpenCloseClick;

        HouseModelController.TryAddToHouseDimmer(gameObject);

        //Request exp info.
        SocketHelper.SendQXMessage(ProtoIndexes.C_GET_MYSELF_HOUSE_EXP);

        //Set red alert info.
        ReceiveBTNRedAlert.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(500040));
    }

    void OnDisable()
    {
        CloseListener.onClick = null;
        LeftBTN1.onClick = null;
        LeftBTN2.onClick = null;
        PopGetOutListener.onClick = null;
        PopOkListener.onClick = null;

        SellingLivingLis.onClick = OnSellLivingClick;
        OpenCloseLis.onClick = OnOpenCloseClick;

        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        SellingLivingLis = UIEventListener.Get(SellLivingObject);
        OpenCloseLis = UIEventListener.Get(OpenCloseObject);
    }

    void OnDestroy()
    {
        StopAllCoroutines();

        SocketTool.UnRegisterSocketListener(this);
    }

    #endregion

    #region Left Part

    private const string splitStr = "/";

    public UILabel LeftLabel1;
    public UILabel LeftLabel2;

    public UILabel LeftBar1Left;
    public UILabel LeftBar1Right;

    public UILabel LeftBar1Label;
    public UIProgressBar LeftBar1;
    private float totalValueBar1;
    private float currentValueBar1;

    public UILabel LeftBar2Left;
    public UILabel LeftBar2Right;

    public UILabel LeftBar2Label;
    public UIProgressBar LeftBar2;
    private float totalValueBar2;
    private float currentValueBar2;

    public UIEventListener LeftBTN1;
    public UIEventListener LeftBTN2;

    public GameObject ReceiveBTNRedAlert;

    public GameObject LeftPart2Info;

    public UILabel FitmentPopLabel;
    public UILabel FitmentCoolDownLabel;

    /// <summary>
    /// set left bar1 and bar2
    /// </summary>
    private void SetBars()
    {
        LeftBar1Label.text = currentValueBar1 + splitStr + totalValueBar1;
        LeftBar1.value = currentValueBar1 / totalValueBar1;

        if (!m_SmallHouseSelf.IsBigHouseExist)
        {
            LeftBar2Label.text = currentValueBar2.ToString();
            LeftBar2.value = currentValueBar2 / totalValueBar2;
        }
    }

    private const float FadeDuration = 0.5f;
    private const float StayDuration = 1.0f;

    /// <summary>
    /// active label for a while, then deactive, both with fade effect.
    /// </summary>
    public void ShowFitmentPopLabel()
    {
        StartCoroutine(DoShowFitmentPopLabel());
    }

    private IEnumerator DoShowFitmentPopLabel()
    {
        FitmentPopLabel.gameObject.SetActive(true);

        //fade in
        Color tempColor = FitmentPopLabel.color;
        FitmentPopLabel.color = new Color(tempColor.r, tempColor.g, tempColor.b, 0.01f);
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", new Color(tempColor.r, tempColor.g, tempColor.b, 0.01f),
            "to", new Color(tempColor.r, tempColor.g, tempColor.b, 1.0f),
            "time", FadeDuration,
            "easetype", "linear",
            "onupdate", "UpdateLabelColor"));

        //stay
        yield return new WaitForSeconds(StayDuration);

        //fade out
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", new Color(tempColor.r, tempColor.g, tempColor.b, 1.0f),
            "to", new Color(tempColor.r, tempColor.g, tempColor.b, 0.01f),
            "time", FadeDuration,
            "easetype", "linear",
            "onupdate", "UpdateLabelColor"));
    }

    private void UpdateLabelColor(Color color)
    {
        FitmentPopLabel.color = color;
    }

    private void RefreshFitmentCoolDown()
    {
        if (m_SmallHouseSelf.m_HouseExpInfo.coolTime > 0)
        {
            FitmentCoolDownLabel.text = m_SmallHouseSelf.m_HouseExpInfo.coolTime / 60 + ":" +
                                        m_SmallHouseSelf.m_HouseExpInfo.coolTime % 60 + " 后可以装修";
            FitmentCoolDownLabel.gameObject.SetActive(true);
            LeftBTN1.gameObject.SetActive(false);

            StartCoroutine("DoExecuteFitmentCoolDown");
        }
        else
        {
            FitmentCoolDownLabel.gameObject.SetActive(false);
            LeftBTN1.gameObject.SetActive(true);

            StopCoroutine("DoExecuteFitmentCoolDown");
        }
    }

    private IEnumerator DoExecuteFitmentCoolDown()
    {
        while (m_SmallHouseSelf.m_HouseExpInfo.coolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            m_SmallHouseSelf.m_HouseExpInfo.coolTime--;
            FitmentCoolDownLabel.text = m_SmallHouseSelf.m_HouseExpInfo.coolTime / 60 + ":" +
                                        m_SmallHouseSelf.m_HouseExpInfo.coolTime % 60 + " 后可以装修";
        }

        RefreshFitmentCoolDown();
    }

    #endregion

    #region Right Part

    [HideInInspector]
    public GameObject VisitorPrefab;

    public UIGrid VisitorGrid;

    public GameObject RightPartInfo;

    [HideInInspector]
    public List<HouseVisitorController> m_HouseVisitorControllerList = new List<HouseVisitorController>();

    public List<VisitorInfo> m_playerInfoList = new List<VisitorInfo>();

    /// <summary>
    /// refresh visitors' grid
    /// </summary>
    public void RefreshVisitorsGrid()
    {
        if (VisitorPrefab != null)
        {
            WWW temp = null;
            OnVisitorLoadCallBack(ref temp, null, VisitorPrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HOUSE_VISITOR), OnVisitorLoadCallBack);
        }
    }

    public void OnVisitorLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (VisitorPrefab == null)
        {
            VisitorPrefab = loadedObject as GameObject;
        }

        //set item's num to specific
        TransformHelper.AddOrDelItem(VisitorGrid.transform, VisitorPrefab, m_playerInfoList.Count);

        m_HouseVisitorControllerList.Clear();
        //ergodic each visitor
        for (int i = 0; i < m_playerInfoList.Count; i++)
        {
            var visitorController = VisitorGrid.transform.GetChild(i).GetComponent<HouseVisitorController>();
            visitorController.m_SmallHouseSelfOperation = this;
            visitorController.m_PlayerInfo = m_playerInfoList[i];
            visitorController.SetController();

            m_HouseVisitorControllerList.Add(visitorController);
        }

        //reposition
        VisitorGrid.Reposition();
    }

    private void OnFitmentClick(GameObject go)
    {
        m_SmallHouseSelf.OnFitmentOrDonateClick();
    }

    private void OnReceiveClick(GameObject go)
    {
        m_SmallHouseSelf.OnReceiveClick();

        //disable red alert.
        ReceiveBTNRedAlert.SetActive(false);
        PushAndNotificationHelper.SetRedSpotNotification(500040, false);
    }

    #endregion

    #region Buttom Part

    public GameObject SellLivingObject;
    public GameObject OpenCloseObject;
    public GameObject OpenCloseObjectRoot;
    private UIEventListener SellingLivingLis;
    private UIEventListener OpenCloseLis;

    public Transform LeftPos;
    public Transform RightPos;

    /// <summary>
    /// set lock and sell state
    /// </summary>
    public void SendSwitchMsg()
    {
        SetHouseState temp = new SetHouseState();
        temp.state = HouseState;
        temp.open4my = IsLock ? 0 : 1;
        temp.targetId = m_SmallHouseSelf.m_HouseSimpleInfo.jzId;
        temp.locationId = m_SmallHouseSelf.m_HouseSimpleInfo.locationId;

        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_SET_HOUSE_STATE);

        //TenementData.Instance.RequestData();
    }

    /// <summary>
    /// House state, move sprite automaticlly.
    /// </summary>
    public int HouseState
    {
        get { return houseState; }
        set
        {
            houseState = value;
            switch (value)
            {
                //house for sell
                case 10:
                //house for sell strictly by alliance leader
                case 505:
                    isLiving = false;
                    MoveSlideBTN(SellLivingObject, isLiving);
                    break;
                //house for living
                case 20:
                    isLiving = true;
                    MoveSlideBTN(SellLivingObject, isLiving);
                    break;
            }
        }
    }
    private int houseState;
    private bool isLiving;

    /// <summary>
    /// Is house close or open to friends, move sprite and send msg automaticlly.
    /// </summary>
    public bool IsLock
    {
        get { return isLock; }
        set
        {
            isLock = value;
            MoveSlideBTN(OpenCloseObject, value);
        }
    }
    private bool isLock;

    private const float MoveDuration = 0.2f;

    public void MoveSlideBTN(GameObject go, bool isMoveToLeft)
    {
        iTween.MoveTo(go, iTween.Hash(
            "time", MoveDuration,
            "position", isMoveToLeft ? LeftPos.localPosition : RightPos.localPosition,
            "easetype", "linear",
            "islocal", true));
    }

    private void OnSellLivingClick(GameObject go)
    {
        m_SmallHouseSelf.OnSwitchSellClick();
    }

    private void OnOpenCloseClick(GameObject go)
    {
        m_SmallHouseSelf.OnSwitchDoorClick();
    }

    #endregion

    #region Pop Part of house visitor

    public GameObject PopPart;
    [HideInInspector]
    public GameObject IconSamplePrefab;

    public Transform IconSampleParent;
    public UILabel PopPlayerName;
    public UILabel PopPlayerLevel;

    public UILabel PopContributation;
    public UILabel PopMilitaryRank;
    public UILabel PopOfficialPost;

    public UIEventListener PopGetOutListener;
    public UIEventListener PopOkListener;

    [HideInInspector]
    public HouseVisitorController SelectedVisitorController;

    public void ShowPopWindow()
    {
        if (IconSamplePrefab != null)
        {
            WWW temp = null;
            OnIconLoadCallBack(ref temp, null, IconSamplePrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconLoadCallBack);
        }
    }

    public void OnIconLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = loadedObject as GameObject;
        }

        PopPart.SetActive(true);

        //set visitor label text
        PopPlayerName.text = SelectedVisitorController.m_PlayerInfo.jzName;
        PopPlayerLevel.text = SelectedVisitorController.m_PlayerInfo.level + "级";
        PopContributation.text = SelectedVisitorController.m_PlayerInfo.gongxian.ToString();
        PopMilitaryRank.text = HouseBasic.GetMilitaryRankStr(SelectedVisitorController.m_PlayerInfo.junxian);
        PopOfficialPost.text = HouseBasic.GetAllianceOfficialPostStr(SelectedVisitorController.m_PlayerInfo.guanxian);

        //instance icon and set.
        var tempObject = Instantiate(IconSamplePrefab) as GameObject;
        TransformHelper.ActiveWithStandardize(IconSampleParent, tempObject.transform);
        var manager = tempObject.GetComponent<IconSampleManager>();
        manager.SetIconType(IconSampleManager.IconType.null_type);
        //[COMPLETE]set player icon
        manager.SetIconBasic(5, "900001", "", "pinzhi4");
    }

    void CannotSetForInsufficientAllianceTimeCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("申请失败",
            null, LanguageTemplate.GetText(LanguageTemplate.Text.HOUSE_ALLIANCE_TIME_INSUFFICIENT),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    public void OnPopOKBTNClick(GameObject go)
    {
        PopPart.SetActive(false);
    }

    private void OnPopGetOutBTNClick(GameObject go)
    {
        m_SmallHouseSelf.OnGetOutClick();
    }

    #endregion

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //kick out house visitor result
                case ProtoIndexes.S_GET_HOUSE_VISITOR:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        HouseVisitorInfo houseVisitorInfo = new HouseVisitorInfo();
                        t_qx.Deserialize(t_tream, houseVisitorInfo, houseVisitorInfo.GetType());

                        m_playerInfoList = houseVisitorInfo.list ?? new List<VisitorInfo>();
                        RefreshVisitorsGrid();

                        return true;
                    }
                case ProtoIndexes.S_RETURN_MYSELF_HOUSE_EXP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        HouseExpInfo houseExpInfo = new HouseExpInfo();
                        t_qx.Deserialize(t_tream, houseExpInfo, houseExpInfo.GetType());

                        StopCoroutine("DoExecuteFitmentCoolDown");
                        RefreshFitmentCoolDown();

                        return true;
                    }
                //can not set state cause alliance time minor to 3 days.
                case ProtoIndexes.S_HOUSE_EXCHANGE_RESULT:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ExchangeResult tempInfo = new ExchangeResult();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        switch (tempInfo.code)
                        {
                            case 300:
                                //alliance time minor to 3 days.
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CannotSetForInsufficientAllianceTimeCallBack);
                                break;
                            case 700:
                                //unknow error.
                                Debug.LogError("Can't exchange cause unknow error.");
                                break;
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }
}

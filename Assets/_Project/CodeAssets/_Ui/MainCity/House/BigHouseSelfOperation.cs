using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class BigHouseSelfOperation : MonoBehaviour, SocketListener
{
    #region General

    public BigHouseSelf m_BigHouseSelf;
    public UIEventListener CloseListener;

    public void RefreshWindow()
    {
        //Get data.
        m_BigHouseSelf.m_HouseSimpleInfo =
            TenementData.Instance
                .m_AllianceCityTenementDic.FirstOrDefault(item => item.Key == m_BigHouseSelf.m_HouseSimpleInfo.locationId).Value;
        m_BigHouseSelf.m_HouseExpInfo = TenementData.Instance.m_AllianceCityTenementExp;

        IsLock = !m_BigHouseSelf.m_HouseSimpleInfo.open4my;

        LeftBar2.gameObject.SetActive(true);
        LeftLabel2.gameObject.SetActive(true);
        LeftBTN2.gameObject.SetActive(true);

        //set left bar2.
        totalValueBar2 = m_BigHouseSelf.m_HouseExpInfo.max;
        currentValueBar2 = m_BigHouseSelf.m_HouseExpInfo.cur;
        LeftBar2Left.text = "0";
        LeftBar2Right.text = m_BigHouseSelf.m_HouseExpInfo.max.ToString();
        SetBars();

        RightPartInfo.SetActive(false);

        //get visitors and set
        SocketHelper.SendQXMessage(ProtoIndexes.C_GET_HOUSE_VISITOR);

        //get and set label text in language template.
        WorthLabel.text = (m_BigHouseSelf.m_HouseSimpleInfo.hworth * CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 3) / 100).ToString();
        AllianceLevelLabel.text = "Lv " + AllianceData.Instance.g_UnionInfo.level;
        LeftLabel1PopupLabel.text = string.Format(LanguageTemplate.GetText(LanguageTemplate.Text.BIG_HOUSE_INFO).Replace("2、", "\n2、").Replace("3、", "\n3、").Replace("4、", "\n4、").Replace("5、", "\n5、"),
            m_BigHouseSelf.m_HouseSimpleInfo.hworth,
            CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 1),
            CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 2),
            CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 3),
            CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 1),
            CanshuTemplate.GetValueByKey(CanshuTemplate.HOUSE_JINGPAI_PREFIX + 4));
        //set left label1 long press.
        var longPress = LeftLabel1.GetComponent<NGUILongPress>();
        longPress.LongTriggerType = NGUILongPress.TriggerType.Press;
        longPress.NormalPressTriggerWhenLongPress = false;
        longPress.OnLongPress = OnLongPress;
        longPress.OnLongPressFinish = OnLongPressFinish;
    }

    private void OnCloseClick(GameObject go)
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        CloseListener.onClick = OnCloseClick;
        LeftBTN1.onClick = OnDonateClick;
        LeftBTN2.onClick = OnReceiveClick;
        PopGetOutListener.onClick = OnPopGetOutBTNClick;
        PopOkListener.onClick = OnPopOKBTNClick;

        OpenCloseLis.onClick = OnOpenCloseClick;

        HouseModelController.TryAddToHouseDimmer(gameObject);
    }

    void OnDisable()
    {
        CloseListener.onClick = null;
        LeftBTN1.onClick = null;
        LeftBTN2.onClick = null;
        PopGetOutListener.onClick = null;
        PopOkListener.onClick = null;

        OpenCloseLis.onClick = OnOpenCloseClick;

        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        OpenCloseLis = UIEventListener.Get(OpenCloseObject);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }

    #endregion

    #region Left Part

    public UILabel LeftLabel1;
    public GameObject LeftLabel1PopupObject;
    public UIWidget LeftLabel1PopupWidget;
    public UILabel LeftLabel1PopupLabel;
    public UILabel LeftLabel2;

    //public UILabel LeftBar1Left;
    //public UILabel LeftBar1Right;

    //public UILabel LeftBar1Label;
    //public UIProgressBar LeftBar1;
    //private float totalValueBar1;
    //private float currentValueBar1;

    public UILabel LeftBar2Left;
    public UILabel LeftBar2Right;

    public UILabel LeftBar2Label;
    public UIProgressBar LeftBar2;
    private float totalValueBar2;
    private float currentValueBar2;

    public UIEventListener LeftBTN1;
    public UIEventListener LeftBTN2;

    public UILabel WorthLabel;
    public UILabel AllianceLevelLabel;

    /// <summary>
    /// set all bars with stored value.
    /// </summary>
    private void SetBars()
    {
        LeftBar2Label.text = currentValueBar2.ToString();
        LeftBar2.value = currentValueBar2 / totalValueBar2;
    }

    #region Left Label1 Long Press

    private void OnLongPress(GameObject go)
    {
        StartCoroutine(LongPressCoroutine());
    }

    private IEnumerator LongPressCoroutine()
    {
        LeftLabel1PopupObject.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        var tempPos = LeftLabel1PopupObject.transform.localPosition;
        LeftLabel1PopupObject.transform.localPosition = new Vector3(tempPos.x, 0 - LeftLabel1PopupWidget.height / 2.0f, tempPos.z);
    }

    private void OnLongPressFinish(GameObject go)
    {
        LeftLabel1PopupObject.gameObject.SetActive(false);
    }

    #endregion

    #endregion

    #region Right Part

    [HideInInspector]
    public GameObject VisitorPrefab;

    public UIGrid VisitorGrid;
    public GameObject RightPartInfo;

    [HideInInspector]
    public List<HouseVisitorController> m_HouseVisitorControllerList = new List<HouseVisitorController>();

    public List<VisitorInfo> m_playerInfoList = new List<VisitorInfo>();

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

        //set item num to specific
        TransformHelper.AddOrDelItem(VisitorGrid.transform, VisitorPrefab, m_playerInfoList.Count);

        //set each visitor controller
        m_HouseVisitorControllerList.Clear();
        for (int i = 0; i < m_playerInfoList.Count; i++)
        {
            var visitorController = VisitorGrid.transform.GetChild(i).GetComponent<HouseVisitorController>();
            visitorController.m_BigHouseSelfOperation = this;
            visitorController.m_PlayerInfo = m_playerInfoList[i];
            visitorController.SetController();

            m_HouseVisitorControllerList.Add(visitorController);
        }

        //reposition
        VisitorGrid.Reposition();
    }

    private void OnDonateClick(GameObject go)
    {
        m_BigHouseSelf.OnFitmentOrDonateClick();
    }

    private void OnReceiveClick(GameObject go)
    {
        m_BigHouseSelf.OnReceiveClick();
    }

    #endregion

    #region Buttom Part

    public GameObject OpenCloseObject;
    private UIEventListener OpenCloseLis;

    public Transform LeftPos;
    public Transform RightPos;

    /// <summary>
    /// switch door open or lock
    /// </summary>
    public void SendSwitchMsg()
    {
        SetHouseState temp = new SetHouseState();
        temp.open4my = IsLock ? 0 : 1;
        temp.targetId = m_BigHouseSelf.m_HouseSimpleInfo.jzId;
        temp.locationId = m_BigHouseSelf.m_HouseSimpleInfo.locationId;

        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_SET_HOUSE_STATE);

        //TenementData.Instance.RequestData();
    }

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

    /// <summary>
    /// door lock slide move duration
    /// </summary>
    private const float MoveDuration = 0.2f;

    public void MoveSlideBTN(GameObject go, bool isMoveToLeft)
    {
        iTween.MoveTo(go, iTween.Hash(
            "time", MoveDuration,
            "position", isMoveToLeft ? LeftPos.localPosition : RightPos.localPosition,
            "easetype", "linear",
            "islocal", true));
    }

    private void OnOpenCloseClick(GameObject go)
    {
        m_BigHouseSelf.OnSwitchDoorClick();
    }

    #endregion

    #region Pop Part of visitor

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

        //set label text
        PopPlayerName.text = SelectedVisitorController.m_PlayerInfo.jzName;
        PopPlayerLevel.text = SelectedVisitorController.m_PlayerInfo.level + "级";
        PopContributation.text = SelectedVisitorController.m_PlayerInfo.gongxian.ToString();
        PopMilitaryRank.text = HouseBasic.GetMilitaryRankStr(BaiZhanTemplate.getBaiZhanTemplateById(SelectedVisitorController.m_PlayerInfo.junxian).templateName);
        PopOfficialPost.text = HouseBasic.GetAllianceOfficialPostStr(SelectedVisitorController.m_PlayerInfo.guanxian);

        //instance icon and set
        var tempObject = Instantiate(IconSamplePrefab) as GameObject;
        TransformHelper.ActiveWithStandardize(IconSampleParent, tempObject.transform);
        var manager = tempObject.GetComponent<IconSampleManager>();
        manager.SetIconType(IconSampleManager.IconType.null_type);
        manager.SetIconBasic(5, "900001", "", "pinzhi4");
    }

    public void OnPopOKBTNClick(GameObject go)
    {
        PopPart.SetActive(false);
    }

    private void OnPopGetOutBTNClick(GameObject go)
    {
        m_BigHouseSelf.OnGetOutClick();
    }

    #endregion

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //kick out visitor
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
                default:
                    return false;
            }
        }
        return false;
    }
}

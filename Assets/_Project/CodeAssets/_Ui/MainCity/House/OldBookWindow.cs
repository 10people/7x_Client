using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

/// <summary>
/// old book window in house sys.
/// </summary>
public class OldBookWindow : MonoBehaviour, SocketListener
{
    [HideInInspector]
    public HouseBasic m_House;
    [HideInInspector]
    public bool IsSelfHouse = true;

    [HideInInspector]
    public List<OldBookSelfController> m_OldBookSelfControllerList = new List<OldBookSelfController>();
    /// <summary>
    /// Old book item exist in bag.
    /// </summary>
    [HideInInspector]
    public List<BagItem> OldBookSelfInfoList = new List<BagItem>();

    [HideInInspector]
    public List<ExchangeOtherController> ExchangeOtherControllerList = new List<ExchangeOtherController>();
    /// <summary>
    /// other alliancers' exchange box.
    /// </summary>
    [HideInInspector]
    public List<HuanWuInfo> ExchangeOtherInfoList = new List<HuanWuInfo>();

    public ExchangeSelfController m_ExchangeSelfController;

    public class ToExchange
    {
        public int playerId;
        public int itemId;
        public int boxId;
    }
    //stored to exchange box
    public ToExchange MyToExchange;
    public ToExchange OtherToExchange;

    public UIEventListener ShowFriendsOldBookBTN;
    public UIEventListener ShowSelfOldBookBTN;
    public UIEventListener CloseBTN;

    public GameObject InfoLabel;

    public UIScrollView m_ScrollView;
    public UIScrollBar m_ScrollBar;
    public UIGrid m_Grid;
    public UISlider mSlider;

    private GameObject OldBookSelfPrefab;
    private GameObject ExchangeOtherPrefab;

    public UILabel HouseTechAdditionLabel;

    public GameObject TopLeftManualAnchor;
    public GameObject TopRightManualAnchor;

    public class ExchangeInfo
    {
        public int itemId;
        public int boxId;
    }

    public enum Mode
    {
        OldBookSelf,
        ExchangeBoxOther
    }

    /// <summary>
    /// Set old book window mode, clear grid's child when switch mode.
    /// </summary>
    [HideInInspector]
    public Mode OldBookMode
    {
        get { return oldBookMode; }
        set
        {
            if (oldBookMode != value)
            {
                while (m_Grid.transform.childCount != 0)
                {
                    var child = m_Grid.transform.GetChild(0);
                    child.parent = null;
                    Destroy(child.gameObject);
                }
            }

            oldBookMode = value;
        }
    }

    private Mode oldBookMode;

    public void RefreshUI()
    {
        //refresh old book window's grid.
        RefreshGrid();

        //set self exchange box.
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_HUAN_WU_INFO);
    }

    /// <summary>
    /// Refresh window's grid according to mode.
    /// </summary>
    public void RefreshGrid()
    {
        switch (OldBookMode)
        {
            case Mode.OldBookSelf:
                ShowSelfOldBookBTN.gameObject.SetActive(false);
                ShowFriendsOldBookBTN.gameObject.SetActive(true);

                //init old book fragment list.
                InfoLabel.SetActive(false);
                InitOldBookSelf();
                break;
            case Mode.ExchangeBoxOther:
                ShowSelfOldBookBTN.gameObject.SetActive(true);

                ShowFriendsOldBookBTN.gameObject.SetActive(false);
                //init others' exchange box list.
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_HUAN_WU_LIST);
                break;
            default:
                Debug.LogError("Not defined mode in old book window.");
                break;
        }
    }

    #region OldBookSelf

    /// <summary>
    /// refresh old book fragment list.
    /// </summary>
    private void InitOldBookSelf()
    {
        //Get old book fragment.
        OldBookSelfInfoList = BagData.Instance().getProp(new int[] { 11, 12, 13, 14, 15 });

        if (OldBookSelfPrefab != null)
        {
            WWW temp = null;
            OnOldBookSelfLoadCallBack(ref temp, "", OldBookSelfPrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.OLD_BOOK_ITEM_SELF), OnOldBookSelfLoadCallBack);
        }
    }

    private const int OldBookSelfStartId = 941001;
    public readonly List<string> OldBookSelfLogoList = new List<string> { "伏羲古卷", "神农古卷", "轩辕古卷", "少昊古卷", "颛顼古卷" };
    private void OnOldBookSelfLoadCallBack(ref WWW www, string temp, Object loadedObject)
    {
        if (OldBookSelfPrefab == null)
        {
            OldBookSelfPrefab = loadedObject as GameObject;
        }

        //instance object
        TransformHelper.AddOrDelItem(m_Grid.transform, OldBookSelfPrefab, 5);

        //add to controller list
        m_OldBookSelfControllerList.Clear();
        foreach (Transform child in m_Grid.transform)
        {
            var controller = child.gameObject.GetComponent<OldBookSelfController>();
            controller.m_OldBookWindow = this;
            m_OldBookSelfControllerList.Add(controller);
        }

        //init controllers
        for (int i = 0; i < 5; i++)
        {
            m_OldBookSelfControllerList[i].WholeBookItemId = OldBookSelfStartId + 1000 * i - 1;

            m_OldBookSelfControllerList[i].BagItemIdList.Clear();
            m_OldBookSelfControllerList[i].NumList.Clear();
            m_OldBookSelfControllerList[i].OldBookInfoLabel.text = OldBookSelfLogoList[i];
            m_OldBookSelfControllerList[i].OldBookLogoSprite.spriteName = (OldBookSelfStartId - 1 + 1000 * i).ToString();

            for (int j = 0; j < 5; j++)
            {
                m_OldBookSelfControllerList[i].BagItemIdList.Add(OldBookSelfStartId + 1000 * i + 1 * j);
                var bagItemTemp =
                    OldBookSelfInfoList.FirstOrDefault(item => item.itemId == m_OldBookSelfControllerList[i].BagItemIdList[j]);
                m_OldBookSelfControllerList[i].NumList.Add(bagItemTemp == null ? 0 : bagItemTemp.cnt);
            }

            m_OldBookSelfControllerList[i].Init();
        }

        //reposition
        m_Grid.cellHeight = OldBookSelfPrefab.GetComponent<UISprite>().height;

        m_Grid.Reposition();

        NGUIHelper.SetScrollBarValue(m_ScrollView, m_ScrollBar, 0.01f);
    }

    #endregion

    #region ExchangeOther

    /// <summary>
    /// refresh others' exchange box list.
    /// </summary>
    private void RefreshExchangeBoxOther()
    {
        //SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LM_HOUSE_INFO);
        if (ExchangeOtherPrefab != null)
        {
            WWW temp = null;
            OnExchangeOtherLoadCallBack(ref temp, "", ExchangeOtherPrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.OLD_BOOK_ITEM_OTHER), OnExchangeOtherLoadCallBack);
        }
    }

    private void OnExchangeOtherLoadCallBack(ref WWW www, string temp, Object loadedObject)
    {
        if (ExchangeOtherPrefab == null)
        {
            ExchangeOtherPrefab = loadedObject as GameObject;
        }

        if (ExchangeOtherInfoList == null || ExchangeOtherInfoList.Count == 0)
        {
            InfoLabel.SetActive(true);
            return;
        }
        else
        {
            InfoLabel.SetActive(false);
        }

        //instance object
        TransformHelper.AddOrDelItem(m_Grid.transform, ExchangeOtherPrefab, ExchangeOtherInfoList.Count);

        //add to controller list
        ExchangeOtherControllerList.Clear();
        foreach (Transform child in m_Grid.transform)
        {
            var controller = child.gameObject.GetComponent<ExchangeOtherController>();
            controller.m_OldBookWindow = this;
            ExchangeOtherControllerList.Add(controller);
        }

        //init controllers
        for (int i = 0; i < ExchangeOtherInfoList.Count; i++)
        {
            ExchangeOtherControllerList[i].PlayerId = (int)ExchangeOtherInfoList[i].jzId;
            ExchangeOtherControllerList[i].PlayerInfoLabel.text = ExchangeOtherInfoList[i].jzName + "的换物箱";

            ExchangeOtherControllerList[i].ExchangeOtherInfoList.Clear();
            {
                ExchangeOtherControllerList[i].ExchangeOtherInfoList.Add(new ExchangeInfo { boxId = 1, itemId = int.Parse(ExchangeOtherInfoList[i].slot1) });
                ExchangeOtherControllerList[i].ExchangeOtherInfoList.Add(new ExchangeInfo { boxId = 2, itemId = int.Parse(ExchangeOtherInfoList[i].slot2) });
                ExchangeOtherControllerList[i].ExchangeOtherInfoList.Add(new ExchangeInfo { boxId = 3, itemId = int.Parse(ExchangeOtherInfoList[i].slot3) });
                ExchangeOtherControllerList[i].ExchangeOtherInfoList.Add(new ExchangeInfo { boxId = 4, itemId = int.Parse(ExchangeOtherInfoList[i].slot4) });
                ExchangeOtherControllerList[i].ExchangeOtherInfoList.Add(new ExchangeInfo { boxId = 5, itemId = int.Parse(ExchangeOtherInfoList[i].slot5) });
            }

            ExchangeOtherControllerList[i].Init();
        }

        //reposition
        m_Grid.cellHeight = ExchangeOtherPrefab.GetComponent<UISprite>().height;

        m_Grid.Reposition();

        NGUIHelper.SetScrollBarValue(m_ScrollView, m_ScrollBar, 0.01f);
    }

    #endregion

    #region ExchangeSelf

    #endregion

    /// <summary>
    /// to exchange check
    /// </summary>
    public void CheckExchange()
    {
        if (MyToExchange != null && OtherToExchange != null)
        {
            ExchangeItem temp = new ExchangeItem
            {
                selfIdx = MyToExchange.boxId,
                targetIdx = OtherToExchange.boxId,
                targetJzId = OtherToExchange.playerId,
                targetItemId = OtherToExchange.itemId.ToString(),
                selfItemId = MyToExchange.itemId.ToString()
            };

            SocketHelper.SendQXMessage(temp, ProtoIndexes.C_HUAN_WU_EXCHANGE);

            MyToExchange = null;
            OtherToExchange = null;
            m_ExchangeSelfController.IconSampleManagerList.ForEach(manager => manager.SelectFrameSprite.gameObject.SetActive(false));
            ExchangeOtherControllerList.ForEach(
            controller =>
                controller.IconSampleManagerList.ForEach(
                    manager => manager.SelectFrameSprite.gameObject.SetActive(false)));
        }
    }

    private void OnShowSelfClick(GameObject go)
    {
        if (IsSelfHouse)
        {
            OldBookMode = Mode.OldBookSelf;
            RefreshUI();
        }
        else
        {
            OnCloseClick(null);
        }
    }

    private void OnCloseClick(GameObject go)
    {
        //  gameObject.SetActive(false);
        NewAlliancemanager.Instance().BackToThis(this.gameObject);
        //[FIX]Destroy all.
        // Destroy(m_House.gameObject);
    }

    private void OnShowFriendsClick(GameObject go)
    {
        OldBookMode = Mode.ExchangeBoxOther;
        RefreshUI();
    }

    void OnEnable()
    {
        ShowSelfOldBookBTN.onClick = OnShowSelfClick;
        CloseBTN.onClick = OnCloseClick;
        ShowFriendsOldBookBTN.onClick = OnShowFriendsClick;
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LM_HOUSE_INFO);
        HouseModelController.TryAddToHouseDimmer(gameObject);

        //Add guide here.
        if (FreshGuide.Instance().IsActive(400010) && TaskData.Instance.m_TaskInfoDic[400010].progress >= 0)
        {
            UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[400010].m_listYindaoShuju[1]);
        }
    }

    void OnDisable()
    {
        ShowSelfOldBookBTN.onClick = null;
        CloseBTN.onClick = null;
        ShowFriendsOldBookBTN.onClick = null;

        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }

    void Start()
    {
        TopLeftManualAnchor.transform.localPosition = new Vector3(-480 - ClientMain.m_iMoveX, 320 + ClientMain.m_iMoveY, 0);
        TopRightManualAnchor.transform.localPosition = new Vector3(480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY, 0);
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        MainCityUI.setGlobalBelongings(gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);
        MainCityUI.setGlobalTitle(TopLeftManualAnchor, "古卷", 0, 0);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }

    private void OnExchangeFailCallBack(ref WWW www, string path, Object loadedObject)
    {
        UIBox uibox = (Instantiate(loadedObject) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.EXCHANG_FAIL_1),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.EXCHANG_FAIL_2),
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
                //others' exchange box list data.
                case ProtoIndexes.S_HUAN_WU_LIST:
                    MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    LianMengBoxes AllianceExchanges = new LianMengBoxes();
                    t_qx.Deserialize(t_tream, AllianceExchanges, AllianceExchanges.GetType());

                    //Show only a box if in other's house.
                    if (IsSelfHouse)
                    {
                        ExchangeOtherInfoList = AllianceExchanges.boxList;
                    }
                    //                    else
                    //                    {
                    //                        var temp = AllianceExchanges.boxList == null ? null : AllianceExchanges.boxList.FirstOrDefault(item => item.jzId == m_House.m_HouseSimpleInfo.jzId);
                    //                        ExchangeOtherInfoList = temp != null ? new List<HuanWuInfo> { temp } : new List<HuanWuInfo>();
                    //                    }

                    RefreshExchangeBoxOther();
                    return true;
                //self exchange box data.
                case ProtoIndexes.S_HUAN_WU_INFO:
                    MemoryStream t_tream2 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx2 = new QiXiongSerializer();
                    HuanWuInfo ExchangeSelfInfo = new HuanWuInfo();
                    t_qx2.Deserialize(t_tream2, ExchangeSelfInfo, ExchangeSelfInfo.GetType());

                    m_ExchangeSelfController.ExchangeSelfInfoList.Clear();
                    m_ExchangeSelfController.ExchangeSelfInfoList.Add(new ExchangeInfo { boxId = 1, itemId = int.Parse(ExchangeSelfInfo.slot1) });
                    m_ExchangeSelfController.ExchangeSelfInfoList.Add(new ExchangeInfo { boxId = 2, itemId = int.Parse(ExchangeSelfInfo.slot2) });
                    m_ExchangeSelfController.ExchangeSelfInfoList.Add(new ExchangeInfo { boxId = 3, itemId = int.Parse(ExchangeSelfInfo.slot3) });
                    m_ExchangeSelfController.ExchangeSelfInfoList.Add(new ExchangeInfo { boxId = 4, itemId = int.Parse(ExchangeSelfInfo.slot4) });
                    m_ExchangeSelfController.ExchangeSelfInfoList.Add(new ExchangeInfo { boxId = 5, itemId = int.Parse(ExchangeSelfInfo.slot5) });

                    m_ExchangeSelfController.RefreshExchangeBoxSelf();
                    return true;
                //old book fragment exchage result
                case ProtoIndexes.S_HUAN_WU_EXCHANGE:
                    MemoryStream t_tream3 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx3 = new QiXiongSerializer();
                    ExItemResult exchangeItemResult = new ExItemResult();
                    t_qx3.Deserialize(t_tream3, exchangeItemResult, exchangeItemResult.GetType());

                    if (exchangeItemResult.code == 0)
                    {
                        //                        Debug.Log("Old book fragment exchange succeed.");
                        RefreshUI();
                    }
                    else if (exchangeItemResult.code == 2)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnExchangeFailCallBack);
                    }
                    return true;
                //combine old boook result
                case ProtoIndexes.S_EX_CAN_JUAN_JIANG_LI:
                    MemoryStream t_tream4 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx4 = new QiXiongSerializer();
                    ExCanJuanJiangLi oldBookCombine = new ExCanJuanJiangLi();
                    t_qx4.Deserialize(t_tream4, oldBookCombine, oldBookCombine.GetType());

                    RefreshGrid();
                    return true;
                //bag data changed.
                case ProtoIndexes.S_BagInfo:
                    //                    Debug.Log("Update old book self cause bag data changed.");
                    if (oldBookMode == Mode.OldBookSelf)
                    {
                        InitOldBookSelf();
                    }
                    return true;
                case ProtoIndexes.S_LM_HOUSE_INFO:
                    {
                        MemoryStream t_tream5 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx5 = new QiXiongSerializer();
                        BatchSimpleInfo AllianceTenementInfo = new BatchSimpleInfo();
                        t_qx5.Deserialize(t_tream5, AllianceTenementInfo, AllianceTenementInfo.GetType());
                        SetHouseExp(AllianceTenementInfo.expInfo);
                        //                        Debug.Log("请求经验返回");
#if HOUSE_TEST
					
					if (MainCityUI.m_MainCityUI != null){
						GameObject temp = TransformHelper.FindChild(MainCityUI.m_MainCityUI.transform, "HouseEnter").gameObject;
						
						BTNTest temp2 = temp.GetComponent<BTNTest>();
						
						temp2.DicKeyStr = string.Join(",", m_AllianceCityTenementDic.Keys.Select(item => item.ToString()).ToArray());
					}
					
#endif

                        return true;
                    }
                case ProtoIndexes.S_GET_HOUSE_EXP:
                    {
                        MemoryStream t_tream6 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx6 = new QiXiongSerializer();
                        HouseExpInfo houseExpInfo = new HouseExpInfo();
                        t_qx6.Deserialize(t_tream6, houseExpInfo, houseExpInfo.GetType());

                        SetHouseExp(houseExpInfo);
                        //                        Debug.Log("领取经验返回");
                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }
    private float totalValueBar2;
    private float currentValueBar2;

    #region House Exp

    public UILabel MaxExp;
    public UILabel CurExp;
    public GameObject LingQuBtn;

    void SetHouseExp(HouseExpInfo mmHouseExpInfo)
    {
        totalValueBar2 = mmHouseExpInfo.max;
        currentValueBar2 = mmHouseExpInfo.cur;
        mSlider.value = currentValueBar2 / totalValueBar2;
        MaxExp.text = mmHouseExpInfo.max.ToString();
        CurExp.text = mmHouseExpInfo.cur.ToString();
        if (currentValueBar2 == 0)
        {
            LingQuBtn.SetActive(false);
        }
        else
        {
            LingQuBtn.SetActive(true);
        }

        HouseTechAdditionLabel.text = "受联盟科技影响\n" + LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level(301, mmHouseExpInfo.kejiLevel).desc;
    }

    public void LingQu()
    {
        SocketHelper.SendQXMessage(ProtoIndexes.C_GET_SMALLHOUSE_EXP);

        //Remove red alert.
        PushAndNotificationHelper.SetRedSpotNotification(600800, false);
        NewAlliancemanager.Instance().Refreshtification();
    }

    #endregion
}

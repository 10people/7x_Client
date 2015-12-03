using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MainCityUILT : MYNGUIPanel, SocketListener
{
    #region Execute this delegate after init

    public static bool IsDoDelegateAfterInit = false;
    public delegate void MainCityUILTDelegate();
    public static MainCityUILTDelegate m_MainCityUILTDelegate;
    public static List<bool> m_CommandList = new List<bool>();

    #endregion

    public UISprite m_UISpriteHeroIcon;

    public UILabel m_playerName;
    public UILabel m_playrLevel;
    public UILabel m_playrVipLevel;

    public UILabel m_leagueName;
	
    public GameObject PopupObject;
    public UILabel PopupLabel;
    public UISprite NationSprite;
	public UISprite m_SpriteExp;

    public UILabel m_ZhanLiLabel;

    public GameObject m_objLianmeng;
    public GameObject m_objLabelHero;
    //public GameObject m_objMoney;
    public GameObject m_objLianmengbg;
    //public GameObject m_objWulianmengbg;

    public NGUILongPress MoneyDetailLongPress;
    public NGUILongPress IngotDetailLongPress;
    public NGUILongPress EnergyDetailLongPress;

    public MainCityZhanliChange m_MainCityZhanliChange;

    private bool m_isLianmeng = true;

    /// <summary>
    /// Show main tip window
    /// </summary>
    public static void ShowMainTipWindow()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAIN_CITY_TIP), OnMainCityTipLoadCallBack);
    }

    private static void OnMainCityTipLoadCallBack(ref WWW www, string str, object obj)
    {
        GameObject mainCityTip = Instantiate(obj as GameObject) as GameObject;
        MainCityUI.TryAddToObjectList(mainCityTip);
        mainCityTip.SetActive(true);
        CityGlobalData.m_isRightGuide = true;
    }

    //IEnumerator InitWithLayer()
    //{
    //    while (JunZhuData.Instance().m_junzhuInfo == null)
    //    {
    //        yield return 0;
    //    }
    //    Init();
    //}

    private void RefreshJunZhuInfo()
    {
        //player info
        m_UISpriteHeroIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
        m_playerName.text = JunZhuData.Instance().m_junzhuInfo.name;
        m_playrLevel.text = "Lv" + JunZhuData.Instance().m_junzhuInfo.level.ToString();
        m_playrVipLevel.text = "VIP" + JunZhuData.Instance().m_junzhuInfo.vipLv.ToString();
        NationSprite.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
		m_SpriteExp.SetDimensions(Global.getBili(122, (float)JunZhuData.Instance().m_junzhuInfo.exp, (float)JunZhuData.Instance().m_junzhuInfo.expMax), 8);
		//m_SpriteExp
        if (Global.m_iZhanli == 0)
        {
            Global.m_iZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
        }
        m_ZhanLiLabel.text = Global.m_iZhanli.ToString();

		MainCityUI.m_MainCityUI.m_MainCityUIRT.RefreshJunZhuInfo();
    }

    public void RefreshBattleValue()
    {
        //		Debug.Log("Global.m_iZhanli="+Global.m_iZhanli);

        if (Global.m_iZhanli == 0)
        {
            Global.m_iZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
        }
        m_ZhanLiLabel.text = Global.m_iZhanli.ToString();
    }

    public void RefreshAllianceInfo()
    {
        //alliance exist
        if (!AllianceData.Instance.IsAllianceNotExist)
        {
            m_isLianmeng = true;
            m_objLianmeng.SetActive(true);

            //try set alliance name
            if (AllianceData.Instance.g_UnionInfo != null && AllianceData.Instance.g_UnionInfo.name != null)
            {
                m_leagueName.text = "<" + AllianceData.Instance.g_UnionInfo.name + ">";
            }
        }
        else
        {
            m_isLianmeng = false;
            m_objLianmeng.SetActive(true);

            m_leagueName.text = "无联盟";
        }
    }

    /// <summary>
    /// init 左上角君主信息
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        if (JunZhuData.Instance() != null && JunZhuData.Instance().m_junzhuInfo != null)
        {
            //wait frames.
            while (JunZhuData.Instance() == null)
            {
                yield return new WaitForEndOfFrame();
            }
            while (JunZhuData.Instance().m_junzhuInfo == null)
            {
                yield return new WaitForEndOfFrame();
            }
            if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
            {
                while (PlayerModelController.m_playerModelController == null)
                {
                    yield return new WaitForEndOfFrame();
                }

                //while (PlayerModelController.m_playerModelController.m_playerName == null)
                //{
                //    yield return new WaitForEndOfFrame();
                //}
            }

            RefreshJunZhuInfo();
        }

        RefreshBattleValue();

        RefreshAllianceInfo();

        //Execute delegate if has.
        if (IsDoDelegateAfterInit)
        {
            IsDoDelegateAfterInit = false;
            if (m_MainCityUILTDelegate != null)
            {
                m_MainCityUILTDelegate();
                m_MainCityUILTDelegate = null;
            }
        }
    }

    public void JunzhuLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (TaskData.Instance.m_iCurMissionIndex == 100115 && TaskData.Instance.m_TaskInfoDic[100115].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100115;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else
            {
                CityGlobalData.m_isRightGuide = true;
            }
        }
    }

    private void OnMoneyDetailClick(GameObject go)
    {
        ShowTip.showTip(900001);
    }

    private void OnIngotDetailClick(GameObject go)
    {
        ShowTip.showTip(900002);
    }

    private void OnEnergyDetailClick(GameObject go)
    {
        ShowTip.showTip(900003);
    }

    private void OnCloseDetail(GameObject go)
    {
        ShowTip.close();
    }

    private const float ShowEnergyDetailDuration = 5.0f;

    #region Mono

    void Awake()
    {
        MoneyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
        MoneyDetailLongPress.NormalPressTriggerWhenLongPress = false;
        IngotDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
        IngotDetailLongPress.NormalPressTriggerWhenLongPress = false;
        EnergyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
        EnergyDetailLongPress.NormalPressTriggerWhenLongPress = false;

        MoneyDetailLongPress.OnLongPress = OnMoneyDetailClick;
        IngotDetailLongPress.OnLongPress = OnIngotDetailClick;
        EnergyDetailLongPress.OnLongPress = OnEnergyDetailClick;
        MoneyDetailLongPress.OnLongPressFinish = OnCloseDetail;
        IngotDetailLongPress.OnLongPressFinish = OnCloseDetail;
        EnergyDetailLongPress.OnLongPressFinish = OnCloseDetail;

        SocketTool.RegisterSocketListener(this);
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Init());
    }

    void OnDestroy()
    {
        MoneyDetailLongPress.OnLongPress = null;
        IngotDetailLongPress.OnLongPress = null;
        EnergyDetailLongPress.OnLongPress = null;
        MoneyDetailLongPress.OnLongPressFinish = null;
        IngotDetailLongPress.OnLongPressFinish = null;
        EnergyDetailLongPress.OnLongPressFinish = null;

        SocketTool.UnRegisterSocketListener(this);
    }

    #endregion

    #region fulfil my ngui panel

    /// <summary>
    /// my click in my ngui panel
    /// </summary>
    /// <param name="ui"></param>
    public override void MYClick(GameObject ui)
    {
		if(MainCityUI.IsWindowsExist())
		{
			return;
		}
		if(ui.name.IndexOf("LT_Email") != -1)
		{
			NewEmailData.Instance ().OpenEmail (NewEmailData.EmailOpenType.EMAIL_MAIN_PAGE);
		}
		else if(ui.name.IndexOf("LT_Bianqiang") != -1)
		{
			if (!MainCityUI.IsWindowsExist())
			{
				ShowMainTipWindow();
			}
		}
        else if (ui.name.IndexOf("LT_Button_HeroHead") != -1)
        {
            CityGlobalData.m_JunZhuTouXiangGuide = false;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_LAYER_AMEND),
                                    JunzhuLayerLoadCallback);
        }
		else if (ui.name.IndexOf("LT_VIPButton") != -1)
		{
			TopUpLoadManagerment.m_instance.LoadPrefabSpecial(true, true);
		}
    }

    public override void MYMouseOver(GameObject ui)
    {

    }

    public override void MYMouseOut(GameObject ui)
    {

    }

    public override void MYPress(bool isPress, GameObject ui)
    {

    }

    public override void MYelease(GameObject ui)
    {

    }

    public override void MYondrag(Vector2 delta)
    {

    }

    public override void MYoubleClick(GameObject ui)
    {

    }

    public override void MYonInput(GameObject ui, string c)
    {

    }

    #endregion

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message == null)
        {
            return false;
        }

        switch (p_message.m_protocol_index)
        {
            case ProtoIndexes.JunZhuInfoRet:
                {
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    JunZhuInfoRet tempInfo = new JunZhuInfoRet();

                    t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                    JunZhuData.Instance().m_junzhuInfo = tempInfo;

                    RefreshJunZhuInfo();
                    RefreshBattleValue();
                    RefreshAllianceInfo();

                    return true;
                }
            default:
                return false;
        }
    }
}

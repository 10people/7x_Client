using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public GameObject m_objRedEmali;
	public BoxChangeScale m_BoxChangeScale;
    //public GameObject m_objWulianmengbg;


    public MainCityZhanliChange m_MainCityZhanliChange;
	public MainCityTaskManager m_MainCityTaskManagerMain;
	public MainCityTaskManager m_MainCityTaskManagerOther;
	public MainCityOpenFunction m_MainCityOpenFunction;

    private bool m_isLianmeng = true;

	void Awake()
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	// Use this for initialization
	void Start()
	{
		StartCoroutine(Init());
//		Debug.Log(m_MainCityTaskManagerMain);
		m_MainCityTaskManagerMain.setData(TaskData.Instance.ShowId);
		m_MainCityTaskManagerOther.setData(TaskData.Instance.m_iShowOtherId);
		m_MainCityOpenFunction.upShow();
		setLTPos(true);
	}

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
        m_playrVipLevel.text = "V" + JunZhuData.Instance().m_junzhuInfo.vipLv.ToString();
        NationSprite.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
		m_SpriteExp.SetDimensions(Global.getBili(122, (float)JunZhuData.Instance().m_junzhuInfo.exp, (float)JunZhuData.Instance().m_junzhuInfo.expMax), 8);
		//m_SpriteExp
		m_ZhanLiLabel.text = JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString();
		
		MainCityUI.m_MainCityUI.m_MainCityUIRT.RefreshJunZhuInfo();
    }

    public void RefreshBattleValue()
    {
        //		Debug.Log("Global.m_iZhanli="+Global.m_iZhanli);
		m_ZhanLiLabel.text = JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString();
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

	public void setLTPos(bool init)
	{
		if(MainCityUI.m_MainCityUI.m_MainCityListButton_L == null)
		{
			return;
		}
		int BY = 200;
		if(m_MainCityTaskManagerMain.gameObject.activeSelf)
		{
			m_MainCityTaskManagerMain.gameObject.transform.localPosition = new Vector3(11f, (float)-BY, 0f);
			BY += 65;
		}
		if(m_MainCityTaskManagerOther.gameObject.activeSelf)
		{
			m_MainCityTaskManagerOther.gameObject.transform.localPosition = new Vector3(11f, (float)-BY, 0f);
			BY += 65;
		}
		if(m_MainCityOpenFunction.gameObject.activeSelf)
		{
			m_MainCityOpenFunction.gameObject.transform.localPosition = new Vector3(11f, (float)-BY, 0f);
			BY += 65;
		}
		BY += 10;

		MainCityUI.m_MainCityUI.m_MainCityListButton_L.setBPos(50, -BY, 0, -80);
		MainCityUI.m_MainCityUI.m_MainCityListButton_L.setPos();
		if(init)
		{
			MainCityUI.m_MainCityUI.m_MainCityListButton_L.setEndPos();
		}
		else
		{
			MainCityUI.m_MainCityUI.m_MainCityListButton_L.setMove(null);
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

    public void ClickTasID(int id)
	{
		if (TaskData.Instance.m_TaskInfoDic.ContainsKey(id))
		{
			if(TaskData.Instance.m_TaskInfoDic[id].m_sSprite != "null")
			{
				Global.m_sPanelWantRun = TaskData.Instance.m_TaskInfoDic[id].m_sSprite;
			}
			if(TaskData.Instance.m_TaskInfoDic[id].progress < 0)
			{
				overMission(id);
			}
			else
			{
				if (TaskData.Instance.m_TaskInfoDic[id].LinkNpcId != -1 && TaskData.Instance.m_TaskInfoDic[id].FunctionId == -1)
				{
					NpcManager.m_NpcManager.setGoToNpc(TaskData.Instance.m_TaskInfoDic[id].LinkNpcId);
				}
				else if (TaskData.Instance.m_TaskInfoDic[id].LinkNpcId == -1 && TaskData.Instance.m_TaskInfoDic[id].FunctionId != -1)
				{
					if(FunctionOpenTemp.IsHaveID(TaskData.Instance.m_TaskInfoDic[id].FunctionId))
					{
						GameObject tempObj = new GameObject();
						tempObj.name = "MainCityUIButton_" + TaskData.Instance.m_TaskInfoDic[id].FunctionId;
						MainCityUI.m_MainCityUI.MYClick(tempObj);
					}
					else
					{
						ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(TaskData.Instance.m_TaskInfoDic[id].FunctionId).m_sNotOpenTips);
					}
				}
			}

		}
		else if(TaskData.Instance.m_TaskDailyDic.ContainsKey(id))
		{
			if(TaskData.Instance.m_TaskDailyDic[id].Script != "null")
			{
				Global.m_sPanelWantRun = TaskData.Instance.m_TaskDailyDic[id].Script;
			}
			if (TaskData.Instance.m_TaskDailyDic[id].LinkNpcId != -1 && TaskData.Instance.m_TaskDailyDic[id].FunctionId == -1)
			{
				NpcManager.m_NpcManager.setGoToNpc(TaskData.Instance.m_TaskDailyDic[id].LinkNpcId);
			}
			else if (TaskData.Instance.m_TaskDailyDic[id].LinkNpcId == -1 && TaskData.Instance.m_TaskDailyDic[id].FunctionId != -1)
			{
				GameObject tempObj = new GameObject();
				tempObj.name = "MainCityUIButton_" + TaskData.Instance.m_TaskDailyDic[id].FunctionId;
				
				MainCityUI.m_MainCityUI.MYClick(tempObj);
			}
		}
	}

	public void overMission(int id)
	{
		TaskSignalInfoShow.m_TaskId = id;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_EFFECT), MainCityUI.m_MainCityUI.AddUIPanel);
	}

    void OnDestroy()
    {
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
//		Debug.Log(ui.name);

		if(MainCityUI.IsWindowsExist())
		{
			return;
		}
		if(ui.name.IndexOf("LT_Email") != -1)
		{
			NewEmailData.Instance().OpenEmail (NewEmailData.EmailOpenType.EMAIL_MAIN_PAGE);
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
            EquipSuoData.TopUpLayerTip(null,false,1,null,true);
		}
		else if(ui.name.IndexOf("LT_TaskMainButton") != -1)
		{
			ClickTasID(TaskData.Instance.ShowId);
		}
		else if(ui.name.IndexOf("LT_TaskOtherButton") != -1)
		{
			ClickTasID(TaskData.Instance.m_iShowOtherId);
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

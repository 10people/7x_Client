using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PveUImanager :MYNGUIPanel
{
	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

    public ScaleEffectController m_ScaleEffectController;

	int maxChanpter = 13;

    public static int buyTimes;
    public GameObject desdroymap;
    public static PveUImanager instances;

    public UILabel TiliLabel;

   // public GameObject RecoverToliCips;

    //public UILabel showTiLiClips;
    public UILabel show_All_TiLiClips;

	public GameObject CQ_btnEffect;
	public GameObject PT_btnEffect;

    public GameObject RightBtn;
    public GameObject LeftBtn;
    public GameObject ChooseChapterBtn;

    private float mTime;

    public UILabel MapName;
	//public UILabel JiangLi_Name;
    public GameObject CQ_btn;
	public GameObject PT_btn;

	public GameObject GetAwardBtn;

    public GameObject DontOpenLvTips;

	public NGUILongPress EnergyDetailLongPress;

	public UILabel m_DontOpenLvTips;

	public GameObject m_ChaperAwardUI;//领取章节奖励界面

//	public GameObject NeedScoleUI;
    void Awake()
    {
        instances = this;
		EnergyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress.OnLongPress = OnEnergyDetailClick;
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "过关斩将", 0, 0);
    }

    void Start()
    {
      //  RecoverToliCips.transform.localScale = new Vector3(0, 0, 0);
        LeftBtn.SetActive(false);
        RightBtn.SetActive(true);

//		UIRoot root = GameObject.FindObjectOfType<UIRoot>();
//		if (root != null) {
//			float s = (float)root.activeHeight / Screen.height;
//			int height = Mathf.CeilToInt (Screen.height * s);
//			int width = Mathf.CeilToInt (Screen.width * s);
//			float v1 = (float)height / (float)640;
//			float v2 = (float)width / (float)960;
//			NeedScoleUI.transform.localScale = new Vector3(v2,v1,1);
//			//Debug.Log ("NeedScoleUI.localScale = " + NeedScoleUI.transform.localScale);
//		} else {
//			//Debug.Log ("root == null");
//		}
    }
	public void OPenEffect(GameObject Btn)
	{
		int id = 620225;
		Btn.GetComponent<UISprite>().alpha = 1f;
		UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,Btn,
		                                   EffectIdTemplate.GetPathByeffectId(id),null,false);
	}
	void OnDestroy(){
		instances = null;
	}

	public void ChangeStateBtn()
	{
		if(CityGlobalData.PT_Or_CQ)
		{
			UIToggle mUItoggle = PT_btn.GetComponent<UIToggle>();
			//mUItoggle.startsActive = true;
			mUItoggle.value = !mUItoggle.value;
			OPenEffect (PT_btnEffect);
		
		}else
		{
			OPenEffect (CQ_btnEffect);
			UIToggle mUItoggle = CQ_btn.GetComponent<UIToggle>();
			//mUItoggle.startsActive = true;
			mUItoggle.value = !mUItoggle.value;

		}
	}

	
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{

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
	void Update()
	{
		
		TiliLabel.text = JunZhuData.Instance().m_junzhuInfo.tili.ToString() + "/" + JunZhuData.Instance().m_junzhuInfo.tiLiMax.ToString();
  
		if(!CQ_btn.activeInHierarchy)
		{
			//Debug.Log("CityGlobalData.m_LastSection = "+CityGlobalData.m_LastSection);
			if((CityGlobalData.m_temp_CQ_Section  > 1 ||(CityGlobalData.m_LastSection > 1&& FunctionOpenTemp.GetWhetherContainID( 109))&&CityGlobalData.m_temp_CQ_Section  > 0))
			{
				CQ_btn.SetActive(true);

			}

		}

      //  buyTimes = JunZhuData.Instance().m_junzhuInfo.tiLipurchaseTime;
		if (!CityGlobalData.PT_Or_CQ )
        {
			GetAwardBtn.SetActive(false);
			if (MapData.mapinstance.CurrChapter < 2)
            {
                LeftBtn.SetActive(false);
				RightBtn.SetActive(true);
            }
            else
            {
                LeftBtn.SetActive(true);
                RightBtn.SetActive(true);
            }
           
        }
		else
		{
			GetAwardBtn.SetActive(true);
			if (MapData.mapinstance.CurrChapter <= 1)
			{
				LeftBtn.SetActive(false);
				RightBtn.SetActive(true);
			}
			else
			{
				LeftBtn.SetActive(true);
				RightBtn.SetActive(true);
			}
		}
		if(CityGlobalData.PT_Or_CQ)
		{
			if (PassLevelBtn.Instance().mDataback&&MapData.mapinstance.MapdataBack) {
				
				if(PassLevelBtn.Instance().Sec_idlist == null)
				{
					return;
				}
				if(PassLevelBtn.Instance().Sec_idlist != null || PassLevelBtn.Instance().Sec_idlist.Count > 0)
				{
					//Debug.Log ("MainCityUI.m_MainCityUI.m_WindowObjectList.Count = "+MainCityUI.m_MainCityUI.m_WindowObjectList.Count);
					if( m_ChaperAwardUI == null)
					{
						MapData.mapinstance.MapdataBack = false;
						GetPveSectionAward();
					}
				}
			}
		}
    }

    /// <summary>
    /// 普通关卡的按钮
    /// </summary>
    public void Com_Gamelv()
    {

		MapData.mapinstance.GuidLevel = 0;
		choosemap.UpAndDownbtn = true;
		if (CityGlobalData.PT_Or_CQ)
        {
            return;
        }
		CityGlobalData.PT_Or_CQ = true;
		UI3DEffectTool.ClearUIFx (CQ_btnEffect);
		OPenEffect(PT_btnEffect);
      //  MapData.mapinstance.Is_Com_Lv = true;
		if(FreshGuide.Instance().IsActive(100320)&& TaskData.Instance.m_TaskInfoDic[100320].progress >= 0)
		{
			//			Debug.Log("攻打传奇关卡");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100320];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);

			return;
		}
        MapData.sendmapMessage(-1);
    }

    /// <summary>
    /// 传奇关卡的按钮
    /// </summary>
    public void CQ_Gamelv()
    {

		choosemap.UpAndDownbtn = true;
	
	    CloseArt();
		if (!CityGlobalData.PT_Or_CQ )
        {
            return;
        }
		CityGlobalData.PT_Or_CQ = false;
       // MapData.mapinstance.Is_Com_Lv = false;
		MapData.mapinstance.GuidLevel = 0;
		UI3DEffectTool.ClearUIFx (PT_btnEffect);
		//Debug.Log ("CityGlobalData.m_temp_CQ_Section =  "+CityGlobalData.m_temp_CQ_Section);
		OPenEffect(CQ_btnEffect);
		MapData.sendmapMessage(CityGlobalData.m_temp_CQ_Section);
	}
	
    /// <summary>
    /// 显示关卡的名字
    /// </summary>
    /// <param name="mChaper"></param>
    public void ShowChapterName(int mChaper)
    {
        PveTempTemplate mPveTempTemplate = PveTempTemplate.GetPveTemplate_By_Chapter_Id(mChaper);
        string Map_Name = NameIdTemplate.GetName_By_NameId(mPveTempTemplate.bigName);

        MapName.text = Map_Name;
    }

    public void BuyEnergyLoadResourceCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempOjbect = Instantiate(p_object) as GameObject;

        tempOjbect.transform.parent = transform;
    }

    /// <summary>
    /// 购买体力方法
    /// </summary>
    public void BuyTiLi()
    {
       
		JunZhuData.Instance().BuyTiliAndTongBi (true,false,false);

    }

	public void OnEnergyDetailClick(GameObject go)//显示体力恢复提示
    {
		ShowTip.showTip (900003);
//        RecoverToliCips.transform.localScale = new Vector3(1, 1, 1);
//        Invoke("diseCoverTiLiClips", 1.5f);
    }
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}

	void diseCoverTiLiClips()
	{
		//RecoverToliCips.transform.localScale = new Vector3(0, 0, 1);
	}
	public void GetPveSectionAward()
	{
		ClientMain.addPopUP (80, 0, "", null);

	}
	public void ShowPveAwardUI ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_REMIND_MI_BAO ),OpenLockLoadBack);
	}

	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		if(m_ChaperAwardUI != null)
		{
			return;
		}
		m_ChaperAwardUI = ( GameObject )Instantiate( p_object );
		m_ChaperAwardUI.name = "PVELevelPass";

		m_ChaperAwardUI.transform.localPosition = new Vector3(100,100,0);

		m_ChaperAwardUI.transform.localScale  = Vector3.one;

		PassLevelAward mPassLevelAward = m_ChaperAwardUI.GetComponent<PassLevelAward>();
		mPassLevelAward.Init ();
		MapData.mapinstance.CloseEffect();
		PassLevelBtn.Instance().CloseEffect ();

		Debug.Log ("加载通关奖励界面");
	}
	private IEnumerator discovertips()
	{
		yield return new WaitForSeconds(1.0f);
		DontOpenLvTips.SetActive(false);
	}
	public void RightMoveMap()//右移地图
    {
        //if(MapData.mapinstance.Lv.Count != MapData.mapinstance.PassLv.Count)return;
        if (mTime > 0)
        {
            return;
        }

        mTime = 2f;
        StartCoroutine(CountTime());

		if (CityGlobalData.PT_Or_CQ )
        {
            
			foreach(NewPveLevelInfo mLevel in MapData.mapinstance.Pve_Level_InfoList )
			{
				if(!mLevel.litter_Lv.s_pass)
				{
//					DontOpenLvTips.SetActive(true);
//
//					m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_94);
//
//					StartCoroutine(discovertips());
					string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_94);
					ClientMain.m_UITextManager.createText(mstr);
					return;
				}
			}
			if (MapData.mapinstance.CurrChapter  > maxChanpter)
			{
				// Debug.Log("显示未开启提示");
//				DontOpenLvTips.SetActive(true);
//
//				m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);

				string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);
				ClientMain.m_UITextManager.createText(mstr);

				StartCoroutine(discovertips());

				return;
			}
			if (MapData.mapinstance.nowCurrChapter < maxChanpter)
            {
                choosemap.UpAndDownbtn = true;
                MapData.mapinstance.JYLvs = 0;
                MapData.mapinstance.Starnums = 0;

                MapData.mapinstance.Lv.Clear();
                //choosemap.nextMap+= 1;
                MapData.mapinstance.CurrChapter += 1;
                MapData.sendmapMessage(MapData.mapinstance.CurrChapter);
                //MapData.mapinstance.IsCloseGuid = true;
                //MapData.mapinstance.IsShowGuid = true;
                //MapData.mapinstance.deletelvfun();
            }
			else
			{
				// Debug.Log("显示未开启提示");
//				DontOpenLvTips.SetActive(true);
//				
//				m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);
//				
//				StartCoroutine(discovertips());
				string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);
				ClientMain.m_UITextManager.createText(mstr);
				return;
			}
        }
        else
        {
		
			foreach(Level mLevel in MapData.mapinstance.CQLv )
			{
				if(!mLevel.chuanQiPass)
				{
//					DontOpenLvTips.SetActive(true);
//					
//					m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_94);
//					
//					StartCoroutine(discovertips());
					string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_94);
					ClientMain.m_UITextManager.createText(mstr);
					return;
				}
			}
			if (MapData.mapinstance.CurrChapter  > maxChanpter)
			{
				// Debug.Log("显示未开启提示");
//				DontOpenLvTips.SetActive(true);
//				
//				m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);
//				
//				StartCoroutine(discovertips());
				string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_96);
				ClientMain.m_UITextManager.createText(mstr);
				return;
			}
            if (MapData.mapinstance.CurrChapter < CityGlobalData.m_temp_CQ_Section)
            {
			//	Debug.Log(" you yidong3333");
                choosemap.UpAndDownbtn = true;
                MapData.mapinstance.JYLvs = 0;
                MapData.mapinstance.Starnums = 0;

                //MapData.mapinstance.Lv.Clear();
                //choosemap.nextMap+= 1;
				MapData.mapinstance.CurrChapter += 1;
				MapData.sendmapMessage(MapData.mapinstance.CurrChapter);
                //MapData.mapinstance.deletelvfun();
            }
			else
			{
//				DontOpenLvTips.SetActive(true);
//				
//				m_DontOpenLvTips.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_95);
//				
//				StartCoroutine(discovertips());

				string mstr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_95);
				ClientMain.m_UITextManager.createText(mstr);

			}
        }
    }

    public void LeftMoveMap()//左移动地图
    {
        if (mTime > 0)
        {
            return;
        }

        mTime = 2f;
        StartCoroutine(CountTime());

		if (CityGlobalData.PT_Or_CQ)
        {
			if (MapData.mapinstance.CurrChapter > 1)
            {
                choosemap.UpAndDownbtn = true;
                MapData.mapinstance.JYLvs = 0;
                MapData.mapinstance.Starnums = 0;
                MapData.mapinstance.Lv.Clear();
                //choosemap.nextMap -= 1;
                MapData.mapinstance.CurrChapter -= 1;
                MapData.sendmapMessage(MapData.mapinstance.CurrChapter);
                // MapData.mapinstance.IsCloseGuid = true;
            }
        }
        else
        {
			if (MapData.mapinstance.CurrChapter > 1)
            {
                choosemap.UpAndDownbtn = true;
                MapData.mapinstance.JYLvs = 0;
                MapData.mapinstance.Starnums = 0;
                MapData.mapinstance.Lv.Clear();
                //choosemap.nextMap -= 1;
				MapData.mapinstance.CurrChapter -= 1;
				MapData.sendmapMessage(MapData.mapinstance.CurrChapter);
                //MapData.mapinstance.IsCloseGuid = true;
            }
        }
    }

    private IEnumerator CountTime()//翻页间隔时间
    {
        if (mTime > 0)
        {
            mTime -= 1f;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CountTime());
        }
    }
	public GameObject AwardRoot;
    public void ChooseChapterLoadResourceCallback(ref WWW p_www, string p_path, Object p_object)
    {
		if(CityGlobalData.PveLevel_UI_is_OPen)
		{
			return;
		}
		CityGlobalData.PveLevel_UI_is_OPen = true;
        GameObject tempOjbect = Instantiate(p_object) as GameObject;

        tempOjbect.transform.parent = this.transform;

        tempOjbect.transform.localPosition = new Vector3(0, 0, 0);

        tempOjbect.transform.localScale = new Vector3(1, 1, 1);

        ChapterUImaneger mChapterUImaneger = tempOjbect.GetComponent<ChapterUImaneger>();
		//Debug.Log ("CityGlobalData.PT_Or_CQ = "+CityGlobalData.PT_Or_CQ);
		if (CityGlobalData.PT_Or_CQ)
        {

			mChapterUImaneger.AllChapers = MapData.mapinstance.AllChapteres;

            mChapterUImaneger.CurrChaper = MapData.mapinstance.nowCurrChapter;
        }
        else
        {
            mChapterUImaneger.AllChapers = CityGlobalData.m_temp_CQ_Section;

			mChapterUImaneger.CurrChaper = MapData.mapinstance.nowCurrChapter;
        }

        mChapterUImaneger.Init();
    }

    public void PopChooeChapter()//弹出选择章节的UI
    {
        MapData.mapinstance.IsCloseGuid = true;

		MapData.mapinstance.CloseEffect();
		if(CityGlobalData.PT_Or_CQ)
		{
			PassLevelBtn.Instance().CloseEffect ();
		}
		MapData.mapinstance.ClosewPVEGuid ();

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVE_CHOOSE_CHAPTER), ChooseChapterLoadResourceCallback);

    }
	public GameObject Art;

	public void ShowArt()
	{
		Art.SetActive (true);
	}
	public void CloseArt()
	{
		Art.SetActive (false);
	}
	/// <summary>
	/// Helps the button.
	/// </summary>
	public void HelpBtn()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.PVE_HELP_DESC),ActiveGuide);
		UIYindao .m_UIYindao.CloseUI ();
		MapData.mapinstance.CloseEffect ();
	}
	void ActiveGuide ()
	{
		MapData.mapinstance.OpenEffect ();
		MapData.mapinstance.ShowPVEGuid();

	}
    public void BackToMainCity()//返回到主城中
    {
        m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
    }

    void DoCloseWindow()
    {
		EnterGuoGuanmap.Instance().ShouldOpen_id = 0;
        MainCityUI.TryRemoveFromObjectList(desdroymap);
        Global.m_isOpenPVP = false;
		CityGlobalData .PT_Or_CQ = true;
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
		if(m_ChaperAwardUI)
		{
			Destroy(m_ChaperAwardUI);
		}
        //MapData.mapinstance.IsCloseGuid = true;
//		WindowBackShowController.CreateSaveWindow("Secret(Clone)");
//		WindowBackShowController.CreateSaveWindow("JUN_ZHU_LAYER_AMEND");
        Destroy(desdroymap);
    }
}

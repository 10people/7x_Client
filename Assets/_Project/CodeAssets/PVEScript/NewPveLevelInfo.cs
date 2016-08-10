using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class NewPveLevelInfo : MonoBehaviour {

	[HideInInspector]public Level litter_Lv;

	[HideInInspector]public Vector2 Zuob1;
	
	[HideInInspector]public Vector2 Zuob2;

	public List<UISprite> stars = new List<UISprite>();

	public GameObject spriteStar_UIroot;

	public GameObject BoxBtn;

	[HideInInspector]public bool Lv_IsOpen = false;
	
	public UILabel mLanguage;
	
	public UILabel mLanguageLabel;
	
	public UILabel OpebSkillLanguage;

	private int needjunzhuLevel;//当前英雄等级

	public GameObject PtLevelBtn;

	public GameObject BossLevelBtn;

	public static int CurLev;

	int RenWuId;

	public enum m_Level_Type 
	{ 
		Putong = 0, 
		
		JingYing, 
		
		ChuanQi 
		
	} 
	void Start () {
	
		Startsendmasg = true;
	}
	

	void Update () {
	
		if(litter_Lv.renWuId  > 0)
		{
			if(FreshGuide.Instance().IsActive(litter_Lv.renWuId) &&TaskData.Instance.m_TaskInfoDic[litter_Lv.renWuId].progress < 0)
			{
				litter_Lv.renWuId = -1;
			}
		}
		if(mLanguage.gameObject.activeInHierarchy)
		{
			if(PlayLanguage)
			{
				Vector3 tempScale = mLanguage.gameObject.transform.localScale;
				float addNum = 0.05f;
				if (tempScale == Vector3.one)
				{
					PlayLanguage = false;
					addNum = 0.05f;
					ChangeTimeTolanguage ();
				}
				if (tempScale.x < 1&&PlayLanguage)
				{
					tempScale.x += addNum;
					tempScale.y += addNum;
					tempScale.z += addNum;
				}
				mLanguage.gameObject.transform.localScale = tempScale;
			}
			
		}
	}

	int STarTime;
	bool PlayLanguage = false;

	void  StopTime()
	{
		STarTime = Random.Range (5,20);
		mLanguage.gameObject.transform.localScale = Vector3.zero;
		Invoke ("PoP_QiPao",STarTime);
	}
	void  ChangeTimeTolanguage()
	{
		Invoke ("StopTime",3f);
	}
	private void PoP_QiPao()
	{
		PlayLanguage = true;
	}

	public UISprite mPtDikuang;//普通关卡地盘

	public UISprite mPtShuangJian;//普通关卡双剑

	public UISprite mPtQizi;//普通关卡旗帜

	public UISprite mBossSprite;//boss关卡地盘

	public void Init()
	{
		StopTime ();
		PlayLanguage = false;
		if(mLanguage.gameObject.activeInHierarchy)
		{
			mLanguage.gameObject.transform.localScale = Vector3.zero;
		}
		needjunzhuLevel = JunZhuData.Instance().m_junzhuInfo.level;
		int levelState = litter_Lv.type;  
		// 0 普通  1 精英  2  传奇
		if(levelState == (int)m_Level_Type.Putong)
		{
			spriteStar_UIroot.SetActive(false);

			BossLevelBtn.SetActive(false);
			
			PtLevelBtn.SetActive(true);

			mPtQizi.gameObject.SetActive(false);

			PutongLevel();
		}
		if(levelState == (int)m_Level_Type.JingYing)
		{
			spriteStar_UIroot.SetActive(true);
			
			JYLevel();
		}
		if(levelState == (int)m_Level_Type.ChuanQi)
		{
			spriteStar_UIroot.SetActive(true);
		}
	}
	private void PutongLevel()
	{
		if(litter_Lv.s_pass)
		{
			OpebSkillLanguage.gameObject.SetActive(false);
			mPtDikuang.color = Color.white;
			mPtShuangJian.color = Color.white;
			PtLevelBtn.GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			PveTempTemplate mPveTemp = PveTempTemplate.GetPveTemplate_By_id (litter_Lv.guanQiaId);
			if(mPveTemp.OPenSkillLabel != "")
			{
				OpebSkillLanguage.gameObject.SetActive(true);
				OpebSkillLanguage.text = mPveTemp.OPenSkillLabel;
			}
			else{
				OpebSkillLanguage.gameObject.SetActive(false);
			}
			
		}
		if(litter_Lv.guanQiaId%10 == 1)
		{
			if(!litter_Lv.s_pass)
			{
				Lv_IsOpen = true;
			}
		}
		else
		{
			if(!litter_Lv.s_pass)
			{
				for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
				{
					if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId -1)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
						{
							mPtDikuang.color = Color.white;
							mPtShuangJian.color = Color.white;
							PtLevelBtn.GetComponent<BoxCollider>().enabled = true;
							
							Lv_IsOpen = true;
							
							break;
						}
					}
					
					mPtDikuang.color = Color.gray;
					mPtShuangJian.color = Color.gray;
					PtLevelBtn.GetComponent<BoxCollider>().enabled = false;
				}
				
			}
		}
		if(Lv_IsOpen)
		{
			int effectId =  100171;
			MapData.mapinstance.ShowEffectLevelid = litter_Lv.guanQiaId;
			MapData.mapinstance.OpenEffect();
		}
		else
		{
			PtLevelBtn.GetComponent<BoxCollider>().enabled = false;
		}
	}
	private void JYLevel()
	{
		PveTempTemplate mPveTemp = PveTempTemplate.GetPveTemplate_By_id (litter_Lv.guanQiaId);
		
		List<NpcTemplate> mNpcTemplate = new List<NpcTemplate>();
		mNpcTemplate = NpcTemplate.GetNpcTemplates_By_npcid(mPveTemp.npcId);

		bool isBoss = false;
		int mModelid = 0;
		foreach(NpcTemplate mNpc in mNpcTemplate)
		{
			if(mNpc.type == 4||mNpc.type == 5)
			{
				mModelid = mNpc.modelId;
				break;
			}
		}
		
		if(mPveTemp.BigIcon == 1)
		{
			BossLevelBtn.SetActive(true);
			isBoss = true;
			PtLevelBtn.SetActive(false);
			LoadBossModel(mModelid);
		}
		else
		{
			BossLevelBtn.SetActive(false);
			isBoss = false;
			PtLevelBtn.SetActive(true);
		}

		mLanguage.gameObject.SetActive(true);
		if(!litter_Lv.s_pass)
		{
			if(mPveTemp.bubble == "" || mPveTemp.bubble ==null)
			{
				mLanguage.gameObject.SetActive(true);
			}
			else
			{
				mLanguage.gameObject.SetActive(true);
				mLanguageLabel.text = mPveTemp.bubble;
			}
		}
		else
		{

			mLanguage.gameObject.SetActive(false);
		}
		if(litter_Lv.guanQiaId%10 != 1)
		{
			if(litter_Lv.s_pass)
			{
				Lv_IsOpen = true;
			}
			else{
				
				for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
				{
					if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId -1)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
						{
							Lv_IsOpen = true;

							break;
						}
					}
				}
			}
		}
		if(Lv_IsOpen)
		{
			if(isBoss)
			{
				BossLevelBtn.GetComponent<BoxCollider>().enabled = true;
				mBossSprite.color = Color.white;
				
			}else
			{
				PtLevelBtn.GetComponent<BoxCollider>().enabled = true;
				mPtDikuang.color = Color.white;
				mPtShuangJian.color = Color.white;
				mPtQizi.color = Color.white;
			}
		}
		else
		{
			
			if(isBoss)
			{
				BossLevelBtn.GetComponent<BoxCollider>().enabled = false;
				mBossSprite.color = Color.gray;
			}else
			{
				PtLevelBtn.GetComponent<BoxCollider>().enabled = false;
				mPtDikuang.color = Color.gray;
				mPtShuangJian.color = Color.gray;
				mPtQizi.color = Color.gray;
			}
		}
		if(litter_Lv.s_pass)
		{
			ShowStar();
		}
		ShowBox (true);
	}
	private void ChuanQiLevel()
	{
		LegendPveTemplate L_pvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(litter_Lv.guanQiaId);
		List<LegendNpcTemplate> mLegendNpcTemplateList = new List<LegendNpcTemplate>();
		mLegendNpcTemplateList = LegendNpcTemplate.GetLegendNpcTemplates_By_npcid(L_pvetemp.npcId);
		mLanguage.gameObject.SetActive(true);
		if(!litter_Lv.chuanQiPass)
		{
			if(L_pvetemp.bubble == "" || L_pvetemp.bubble ==null)
			{
				mLanguage.gameObject.SetActive(true);
			}
			else
			{
				mLanguage.gameObject.SetActive(true);
				mLanguageLabel.text = L_pvetemp.bubble;
			}
		}
		else
		{
			mLanguage.gameObject.SetActive(false);
		}
		int mModelid = 0;
		foreach(LegendNpcTemplate mNpc in mLegendNpcTemplateList)
		{
			if(mNpc.type == 4||mNpc.type == 5)
			{
			   mModelid = mNpc.modelId;
			}
		}
		bool isBoss = false;

		if(L_pvetemp.BigIcon == 1)
		{
			LoadBossModel(mModelid);
			BossLevelBtn.SetActive(true);
			isBoss = true;
			PtLevelBtn.SetActive(false);
		}
		else
		{
			BossLevelBtn.SetActive(false);
			isBoss = false;
			PtLevelBtn.SetActive(true);
		}
		int Lvpos = 0;
		
		for(int i = 0; i<MapData.mapinstance.CQLv.Count; i++)
		{
			if(litter_Lv == MapData.mapinstance.CQLv[i])
			{
				Lvpos = i;
			}
		}
		if(Lvpos == 0)
		{
			if(!litter_Lv.chuanQiPass)
			{
				Lv_IsOpen = true;	

			}
		}
		else
		{
			if(Lvpos == MapData.mapinstance.CQLv.Count - 1)
			{
				if(MapData.mapinstance.CQLv[Lvpos-1].chuanQiPass)
				{
					if(!litter_Lv.chuanQiPass)
					{
						Lv_IsOpen = true;
					}
				}
			}
			else
			{
				if(!litter_Lv.chuanQiPass)
				{
					if(MapData.mapinstance.CQLv[Lvpos-1].chuanQiPass)
					{
						Lv_IsOpen = true;
					}
				}
			}
	
	    }
		
		
		
		if(litter_Lv.chuanQiPass)
		{
			ShowStar();
		}
		if(Lv_IsOpen)
		{
			if(isBoss)
			{
				BossLevelBtn.GetComponent<BoxCollider>().enabled = true;
				mBossSprite.color = Color.white;
				
			}else
			{
				PtLevelBtn.GetComponent<BoxCollider>().enabled = true;
				mPtDikuang.color = Color.white;
				mPtShuangJian.color = Color.white;
				mPtQizi.color = Color.white;
			}
		}
		else
		{
			
			if(isBoss)
			{
				BossLevelBtn.GetComponent<BoxCollider>().enabled = false;
				mBossSprite.color = Color.gray;
			}else
			{
				PtLevelBtn.GetComponent<BoxCollider>().enabled = false;
				mPtDikuang.color = Color.gray;
				mPtShuangJian.color = Color.gray;
				mPtQizi.color = Color.gray;
			}
		}
		ShowBox (false);
	}
	void ShowStar()
	{
		spriteStar_UIroot.SetActive (true);
		
		int starnum = 0;//litter_Lv.starNum;
		
		//Debug.Log ("starnum"+starnum);
		if (CityGlobalData.PT_Or_CQ) {
			for (int j = 0; j < litter_Lv.starInfo.Count; j++) {
				if (litter_Lv.starInfo [j].finished) {
					starnum += 1;
				}
			}
		} 
		else 
		{
			for (int j = 0; j < litter_Lv.cqStarInfo.Count; j++) {
				if (litter_Lv.cqStarInfo [j].finished) {
					starnum += 1;
				}
			}
		}
		for(int i = 0; i < starnum; i++)
		{
			stars[i].spriteName = "BigStar";
			
			stars[i].gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
		}
	}
	private void LoadBossModel(int modelid)
	{
		if(modelid == 0)
		{
			Debug.Log("modelid = 0 ");
			return;
		}
		Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(modelid),
		                        LoadCallback );
	}
	[HideInInspector]public GameObject m_PlayerModel;
	public GameObject m_PlayerParent;

	public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		if(m_PlayerModel)
		{
			Destroy(m_PlayerModel);
		}
		
		m_PlayerModel = Instantiate(p_object) as GameObject;
		
		m_PlayerModel.SetActive( true );
		
		m_PlayerModel.transform.parent = m_PlayerParent.transform;
		
		m_PlayerModel.name = p_object.name;
		
		GameObjectHelper.SetGameObjectLayerRecursive( m_PlayerModel, m_PlayerModel.transform.parent.gameObject.layer );
		

		m_PlayerModel.GetComponent<NavMeshAgent>().enabled = false;
		
		BaseAI mBaseAI = m_PlayerModel.GetComponent<BaseAI>();
		Animator mAnimator = m_PlayerModel.GetComponent<Animator>();
		mAnimator.applyRootMotion = false;
		if(mBaseAI != null )
		{
			Destroy(mBaseAI);
			mBaseAI.enabled = false;
		}
		m_PlayerModel.AddComponent <DramaStorySimulation>();	
		//m_PlayerModel.GetComponent<Animator>().Play("zhuchengidle");
		
		m_PlayerModel.transform.localScale = new Vector3(1,1,1);
		
		m_PlayerModel.transform.Rotate (0, -180, 0);// = new Vector3 (0,40,0);
		
		//		m_PlayerModel.transform.localRotation = new Quaternion(0,20,0,0);
		//		m_PlayerModel.transform.rotation = new Quaternion(0,163,0,0);
		
		//		m_PlayerModel.GetComponent<PlayerWeaponManagerment>().ShowWeapon(3);
		m_PlayerModel.transform.localPosition = new Vector3(0, 0, 0);
	}
	public UISprite Boxsprite;
	public void ShowBox(bool jingyin)
	{
		//Debug.Log ("jingyin = "+jingyin);
		if(jingyin)
		{
			if(!litter_Lv.s_pass)
			{
				BoxBtn.GetComponent<BoxCollider>().enabled = false;
				
				BoxBtn.SetActive (false);
				
			}
			else
			{
				int passFinishnum = 0;
				
				int Getrawardnum = 0;
				for(int j = 0 ; j < litter_Lv.starInfo.Count; j++)
				{
					if(litter_Lv.starInfo[j].finished)
					{
						passFinishnum += 1;
						if(litter_Lv.starInfo[j].getRewardState)
						{
							Getrawardnum +=1;
						}
					}
				}
				if(Getrawardnum >= passFinishnum)
				{
					BoxBtn.SetActive (false);
					return;
				}
				if(passFinishnum == litter_Lv.starInfo.Count)
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.SetActive (false);
					}
					else
					{
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}else
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
						
						BoxBtn.SetActive (true);
						
						BoxBtn.gameObject.transform.localScale = new Vector3(0.7f,0.7f,1);
						
						Boxsprite.color = new Color(0,0,0,255);
					}
					else
					{
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}
			}
		}
		else
		{
			if(!litter_Lv.chuanQiPass)
			{
				BoxBtn.GetComponent<BoxCollider>().enabled = false;
				
				BoxBtn.SetActive (false);
				
			}
			else
			{
				int passFinishnum = 0;
				
				int Getrawardnum = 0;
				for(int j = 0 ; j < litter_Lv.cqStarInfo.Count; j++)
				{
					if(litter_Lv.cqStarInfo[j].finished)
					{
						passFinishnum += 1;
						if(litter_Lv.cqStarInfo[j].getRewardState)
						{
							Getrawardnum +=1;
						}
					}
				}
				if(Getrawardnum >= passFinishnum)
				{
					BoxBtn.SetActive (false);
					return;
				}
				if(passFinishnum == litter_Lv.cqStarInfo.Count)
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.SetActive (false);
					}
					else
					{
						//Debug.Log("aaaa");
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}else
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
						//Debug.Log("bbbbbbb");
						BoxBtn.SetActive (true);
						
						BoxBtn.gameObject.transform.localScale = new Vector3(0.7f,0.7f,1);
						
						Boxsprite.color = new Color(0,0,0,255);
					}
					else
					{
						//Debug.Log("ccccc");
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}
			}
		}
		
	}
	public void GetAwardBtn()
	{
		// 领取奖励按钮 宝箱
		//UI3DEffectTool.ClearUIFx (BoxBtn);
		MapData.mapinstance.CloseEffect();
		if(CityGlobalData.PT_Or_CQ)
		{
			PassLevelBtn.Instance().CloseEffect ();
		}
		
		MapData.mapinstance.ClosewPVEGuid ();
		
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_GRADE_REWARD),LoadResourceCallback2);
	}
	public bool Startsendmasg = true;
	public void LoadResourceCallback2(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
		
		tempOjbect.transform.parent = GameObject.Find ("Mapss").transform;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = Vector3.zero;
		
		PveStarAwardpanel mPveStarAwardpanel = tempOjbect.GetComponent<PveStarAwardpanel>();
		
		mPveStarAwardpanel.M_Level = litter_Lv;
		
		mPveStarAwardpanel.Init ();
	}
	public void POPLevelInfo()
	{
		createpveui(Startsendmasg);
	}
	void createpveui(bool issended)
	{
		if (issended)
		{
			Startsendmasg = false;
			
			if(CityGlobalData.PT_Or_CQ)
			{
				if(MapData.mapinstance.Lv.ContainsKey(litter_Lv.guanQiaId))
				{
					
					CurLev = litter_Lv.guanQiaId;
					
					StartCoroutine(ChangerDataState());
					
					RenWuId = litter_Lv.renWuId;
					
					//					Debug.Log("RenWuId = " +RenWuId);
					
					if(RenWuId <= 0) // <=
					{
						if(needjunzhuLevel >= litter_Lv.s_level ){//
							
							if(JunZhuData.Instance().m_junzhuInfo.zhanLi > PveTempTemplate.GetPveTemplate_By_id(CurLev).PowerLimit)
							{
								ShowUIbaseBackData ();
							}
							else{
								Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadPowerUpBack);
							}
						}
						else{
							
							string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_LIMIT)+litter_Lv.s_level.ToString ()+LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_UP_LEVEL)+LanguageTemplate.GetText(LanguageTemplate.Text.GET_EXP);
							
							ClientMain.m_UITextManager.createText(Contain1);
							//Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							//							                        LoadResourceCallback );
						}
						
					}
					else{
						
						string Contain2 = ZhuXianTemp.GeTaskTitleById (RenWuId);
						
						string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT)+Contain2;
						
						ClientMain.m_UITextManager.createText(Contain1);
						//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
					}
				}
			}
			else{
				if(MapData.mapinstance.CQLv.Contains(litter_Lv))
				{
					CurLev = litter_Lv.guanQiaId;
					
					StartCoroutine(ChangerDataState());
					
					
					RenWuId = litter_Lv.renWuId;
					if(RenWuId <= 0)
					{
						if(needjunzhuLevel >= litter_Lv.s_level ){//
							
							if(JunZhuData.Instance().m_junzhuInfo.zhanLi > LegendPveTemplate.GetlegendPveTemplate_By_id(CurLev).PowerLimit)
							{
								ShowUIbaseBackData ();
							}
							else{
								//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadPowerUpBack);
								
								int lv = 0;
								
								if(CityGlobalData.PT_Or_CQ)
								{
									lv = PveTempTemplate.GetPveTemplate_By_id (CurLev).PowerLimit;
								}
								else{
									
									lv = LegendPveTemplate.GetlegendPveTemplate_By_id (CurLev).PowerLimit;
								}
								string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
								
								string Contain1 =  LanguageTemplate.GetText(LanguageTemplate.Text.POWER_LIMIT);
								
								string Contain2 = lv.ToString ();
								ClientMain.m_UITextManager.createText(Contain2);
							}
						}
						else{
							
							string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_LIMIT)+litter_Lv.s_level.ToString ()+LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_UP_LEVEL)+LanguageTemplate.GetText(LanguageTemplate.Text.GET_EXP);
							
							ClientMain.m_UITextManager.createText(Contain1);
							//Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							//							                        LoadResourceCallback );
						}
						
					}
					else{
						
						string Contain2 = ZhuXianTemp.GeTaskTitleById (RenWuId);
						
						string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT)+Contain2;
						
						ClientMain.m_UITextManager.createText(Contain1);
						//	Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
					}
				}
			}
			
		}		
	}
	void LoadPowerUpBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		int lv = 0;
		
		if(CityGlobalData.PT_Or_CQ)
		{
			lv = PveTempTemplate.GetPveTemplate_By_id (CurLev).PowerLimit;
		}
		else{
			
			lv = LegendPveTemplate.GetlegendPveTemplate_By_id (CurLev).PowerLimit;
		}
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		
		string Contain1 =  LanguageTemplate.GetText(LanguageTemplate.Text.POWER_LIMIT);
		
		string Contain2 = lv.ToString ();
		
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(title,Contain1, Contain2,null,Comfirm,null,null,null,null);
	}

	IEnumerator ChangerDataState()
	{
		yield return new WaitForSeconds (1.0f);
		
		Startsendmasg = true;
	}
	void ShowUIbaseBackData()
	{
		MapData.mapinstance.ShowYinDao = false;
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_UI),loadback);	
	}
	GameObject tempOjbect_PVEUI;//掉落显示页面
	public void loadback(ref WWW p_www,string p_path, Object p_object)
	{
		//		if(CityGlobalData.PveLevel_UI_is_OPen)
		//		{
		//			return;
		//		}
		CityGlobalData.PveLevel_UI_is_OPen = true;
		if(tempOjbect_PVEUI == null)
		{
			tempOjbect_PVEUI = Instantiate (p_object)as GameObject;
			MainCityUI.TryAddToObjectList (tempOjbect_PVEUI);
		}
		
		MapData.mapinstance.CloseEffect();
		if(CityGlobalData.PT_Or_CQ)
		{
			PassLevelBtn.Instance().CloseEffect ();
		}
		MapData.mapinstance.ClosewPVEGuid ();
		tempOjbect_PVEUI.transform.localPosition = new Vector3(0,400,0);
		
		tempOjbect_PVEUI.transform.localScale = new Vector3 (1,1,1);
		
		NewPVEUIManager mNewPVEUIManager = tempOjbect_PVEUI.GetComponent<NewPVEUIManager>();
		
		mNewPVEUIManager.mLevel = litter_Lv;
		mNewPVEUIManager.Init ();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class HYRetearceEnemy : MYNGUIPanel , SocketProcessor { //突袭藏宝点

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public GameObject NoMiBaoSkillMind;

	public SparkleEffectItem mSparkleEffectItem;
	public UILabel Boci;
	public HuangYeTreasure mHuangYeTreasure;

	public UILabel TimeAndAllTimes;//剩余次数和总次数

	public UILabel Res_Name;//剩余次数和总次数


	public GameObject ShowPlayerDoingTiaoZhan;
	public GameObject ShowBtnBox; //遮罩
	//public GameObject ChangeMiBaoBtn;

	public UILabel Lv_Instruction;//描述

	List<int> GetPveEnemyId = new List<int>();
	
	List<int> soldires = new List<int>();
	List<int> heros = new List<int>();
	List<int> Bosses = new List<int>();
	List<int> Zhi_Ye = new List<int>();
	
	int EnemyNumBers = 4;//显示敌人数量
	int distance = 100;//敌人头像距离
	int countDistance = 250;//偏移量
	
	private int awardNum;//掉落物品个数
	
	[HideInInspector]public GameObject IconSamplePrefab;

	public UILabel CloseTime;//剩余重置之间显示
	private int m_Time;
	public GameObject EnemyRoot;
	public GameObject DropthingsRoot;

	public GameObject m_objInBattle;
	public UISprite MiBaoicon;
	private bool m_isEnterBattle = false;

	string CancleBtn;
	
	string confirmStr;

	private static HYRetearceEnemy g_HYRetearceEnemy;

	public NGUILongPress EnergyDetailLongPress1;
	public MiBaoSkillTips mMiBaoSkillTips;

	public GameObject KuaiSuAwardRoot;

	public int mGuanqia_id;

	public static HYRetearceEnemy Instance()
	{
		if (!g_HYRetearceEnemy)
		{
			g_HYRetearceEnemy = (HYRetearceEnemy)GameObject.FindObjectOfType (typeof(HYRetearceEnemy));
		}
		
		return g_HYRetearceEnemy;
	}


	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		int mibaoid = M_Treas_info.zuheId;
		if(mibaoid<=0)
		{
			return;
		}
		ShowTip.showTip (mibaoid);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);

		g_HYRetearceEnemy = null;
	}

	void Start () {
	
		CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}

	List <int > awardidlist = new List<int> ();
	List <string > awardNumlist = new List<string> ();
	private void ShowFastAward()
	{
		HuangYePveTemplate mHYtemp = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.guanQiaId);
		awardidlist.Clear ();
		awardNumlist.Clear ();

		string m_AwardString = mHYtemp.fastAward;

		string [] s = m_AwardString.Split ('#');


		string m_AwardString_Per = mHYtemp.perFastAward;
		
		string [] s1 = m_AwardString_Per.Split ('#');

		for(int i = 0; i < s.Length; i ++)
		{
			string [] h = s[i].Split (':');
			int mid = int.Parse(h[1]);
			awardidlist.Add(mid);
			awardNumlist.Add(h[2]);

		}
		for(int i = 0; i < s1.Length; i ++)
		{
			string [] h = s1[i].Split (':');
			int mid = int.Parse(h[1]);
			awardidlist.Add(mid);
			awardNumlist.Add(h[2]);
			
		}
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), m_OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			m_OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	private void m_OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		//		Debug.Log ("numPara = " +numPara);
	
		for(int m = 0; m < awardidlist.Count ; m ++)
		{
			if (IconSamplePrefab == null)
			{
				IconSamplePrefab = p_object as GameObject;
			}
			
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			iconSampleObject.SetActive(true);
			iconSampleObject.transform.parent = KuaiSuAwardRoot.transform;
			iconSampleObject.transform.localPosition = new Vector3(0+50*m, 0, 0);
			
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(awardidlist[m]);
			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
			string mdesc = DescIdTemplate.GetDescriptionById(awardidlist[m]);

			iconSampleManager.SetIconByID(mItemTemp.id, awardNumlist[m].ToString(), 3);
			iconSampleManager.SetIconPopText(awardidlist[m], mNameIdTemplate.Name, mdesc);
			iconSampleObject.transform.localScale = new Vector3(0.4f,0.4f,1);
			
		 }
	}
	public void Init()
	{
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		EnemyNumBers = 4;
		initHYTreasureBattleUI ();
		RefreshRetearce();
		ShowBtnBox.SetActive(false);
		ShowFastAward ();
	}

	public void initHYTreasureBattleUI()
	{
		//BooldNumb.text = mHuangYeTreasure.jindu+"%";

		//Boci.text = "当前波次("+M_Treas_info.thisBoCi.ToString()+"/"+M_Treas_info.allBoCi.ToString()+")";

		HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.guanQiaId);

		string mHuangYeDesc = DescIdTemplate.GetDescriptionById (mHuangYePveTemplate.descId);

		string mName = NameIdTemplate.GetName_By_NameId (mHuangYePveTemplate.nameId);

		//TuiJianMiBaoicon.spriteName = "";

		string TuiJiAnMiBAo = mHuangYePveTemplate.recMibaoSkill;
		string []m_str = TuiJiAnMiBAo.Split(',');
		for (int i = 0; i < m_str.Length; i++)
		{
			GameObject mobg = (GameObject)Instantiate(mMiBaoSkillTips.gameObject);	
			mobg.SetActive(true);
			mobg.transform.parent = mMiBaoSkillTips.gameObject.transform.parent;
			mobg.transform.localPosition = mMiBaoSkillTips.gameObject.transform.localPosition + new Vector3(i * 70 - (m_str.Length - 1) * 35, 0, 0);
			mobg.transform.localScale = Vector3.one;
			if(m_str[i] != ""&&m_str[i] != null){
				mobg.GetComponent<MiBaoSkillTips>().Skillid = int.Parse(m_str[i]);
				mobg.GetComponent<MiBaoSkillTips>().mibao_name = m_str[i];
			}
			mobg.GetComponent<MiBaoSkillTips>().Init();
		}

		char[] separator = new char[]{'#'};
		
		string[] s = mHuangYeDesc.Split (separator);
		
		string desText = "";
		for(int j = 0; j < s.Length; j++ )
		{
			desText += s[j]+"\r\n";
		}
		Lv_Instruction.text = desText;
//		Res_Name.text = mName;
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, mName, 0, 0);
		//m_UISlider.value = (float)( mHuangYeTreasure.jindu )/ (float)(100);
		int id = mHuangYeTreasure.guanQiaId;

		//		InitDropthings(id);    bugid 48800 不显示奖励了

		CityGlobalData.IsOPenHyLeveUI = true;
	}
	void InitDropthings(int m_id)
	{
		awardNum = 6;

		List<int> t_items = new List<int>();

		HuangYePveTemplate pvetemp = HuangYePveTemplate.getHuangYePveTemplatee_byid(m_id);

		Debug.Log ("pvetemp.award = " +pvetemp.award);

		char[] t_items_delimiter = { '#' };
		
		char[] t_item_id_delimiter = { ':' };
		
		string[] t_item_strings = pvetemp.award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);

			if(!t_items.Contains(int.Parse(t_finals[1])))
			{
				t_items.Add(int.Parse(t_finals[1]));
			}

		}
				Debug.Log ("t_items.count  " +t_items.Count);
		
		int initNum;
		if (awardNum >= t_items.Count)
		{
			initNum = t_items.Count;
		}
		else
		{
			initNum = awardNum;
		}
		
		numPara = initNum;
		itemsPara = t_items;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private int numPara;
	private List<int> itemsPara;
	
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
//		Debug.Log ("numPara = " +numPara);

		int pos = 0;

		for (int n = 0; n < numPara; n++)
		{
			Debug.Log ("itemsPara[n] = " +itemsPara[n]);
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
			Debug.Log ("mAwardTemp.count = " +mAwardTemp.Count);
			for (int i = 0; i < mAwardTemp.Count; i++)
			{
				if(mAwardTemp[i].weight != 0)
				{
					pos += 1;

					if(pos > 4)
					{
						return;
					}

					if (IconSamplePrefab == null)
					{
						IconSamplePrefab = p_object as GameObject;
					}
					
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
					iconSampleObject.SetActive(true);
					iconSampleObject.transform.parent = DropthingsRoot.transform;
                    iconSampleObject.transform.localPosition = new Vector3(-150 + (pos - 1) * 100, -20, 1);

                    var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[i].itemId);
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);

				    iconSampleManager.SetIconByID(mItemTemp.id, "", 7);
                    iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, mNameIdTemplate.Name, mdesc);
					iconSampleObject.transform.localScale = new Vector3(0.82f,0.82f,1);
				}
			}
		}
	}
	List<HuangyeNPCTemplate> mHuangyeNPCTemplateList = new List<HuangyeNPCTemplate> ();
	void initEnemy( )
	{
		mHuangyeNPCTemplateList.Clear ();


		for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
		{
			//Debug.Log(" M_Treas_info.npcInfos = "+ M_Treas_info.npcInfos[i].npcId);
			HuangyeNPCTemplate m_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(M_Treas_info.npcInfos[i].npcId);

			mHuangyeNPCTemplateList.Add(m_HuangyeNPCTemplate);
		}
//		for (int i = 0; i < mHuangyeNPCTemplateList.Count-1; i ++)
//		{
//			for(int j = i+1; j < mHuangyeNPCTemplateList.Count; )
//			{
//				if(mHuangyeNPCTemplateList[i].modelId == mHuangyeNPCTemplateList[j].modelId)
//				{
//					
//					mHuangyeNPCTemplateList.RemoveAt(j);
//				}
//				else{
//					j ++;
//				}
//			}
//		}
		for (int i = 0; i < mHuangyeNPCTemplateList.Count-1; i ++)
		{
			for(int j = i+1 ; j < mHuangyeNPCTemplateList.Count; j++)
			{
				if(mHuangyeNPCTemplateList[i].type > mHuangyeNPCTemplateList[j].type)
				{
					HuangyeNPCTemplate mLegendNpc = mHuangyeNPCTemplateList[i];
					mHuangyeNPCTemplateList[i] = mHuangyeNPCTemplateList[j];
					mHuangyeNPCTemplateList[j] = mLegendNpc ;
				}
			}
		}
		getEnemyData ();
	}
	
	void getEnemyData()
	{
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreatePuTong_Enemys);
		}
		else
		{
			WWW temp = null;
			OnCreatePuTong_Enemys(ref temp, null, IconSamplePrefab);
		}
	}
	private List<GameObject> iconSampleObjectList = new List<GameObject> (); 
	private void OnCreatePuTong_Enemys(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		int count = mHuangyeNPCTemplateList.Count;
		if(count > 10)
		{
			count =  10;
		}
		foreach(GameObject mobg in iconSampleObjectList)
		{
			Destroy(mobg);
		}
		iconSampleObjectList.Clear ();
		for (int n = 0; n < count; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			int allenemy = mHuangyeNPCTemplateList.Count;
//			if(allenemy > 4)
//			{
//				allenemy = 4;
//			}
			
			//iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			HuangyeNPCTemplate mHuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(mHuangyeNPCTemplateList[n].id);
			float boold = 1.0f;
			for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
			{
				if(mHuangyeNPCTemplateList[n].id == M_Treas_info.npcInfos[i].npcId)
				{
//					Debug.Log("M_Treas_info.npcInfos[i].remainHP = " +M_Treas_info.npcInfos[i].remainHP);
//					Debug.Log("M_Treas_info.npcInfos[i].totalHP = " +M_Treas_info.npcInfos[i].totalHP);
					boold = (float)M_Treas_info.npcInfos[i].remainHP/(float)M_Treas_info.npcInfos[i].totalHP;
					
//					Debug.Log("boold = " +boold);
				}
			}
			
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mHuangyeNPCTemplate.name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mHuangyeNPCTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mHuangyeNPCTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
		
			if(mHuangyeNPCTemplate.type == 4)
			{
				//Debug.Log("boss");
				rightButtomSpriteName = "boss";
			}
			if(mHuangyeNPCTemplate.type == 5)
			{
				rightButtomSpriteName = "JunZhu";
			}
			if(boold > 0)
			{
				iconSampleManager.SetProgressBar(boold);
			}
			else
			{
				iconSampleManager.MiddleSprite.gameObject.SetActive(true);
			}
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(3, mHuangyeNPCTemplate.icon.ToString(), "", "", boold <= 0);
			//iconSampleManager.SetIconPopText(popTextTitle, popTextDesc);
			iconSampleManager.SetIconPopText(0,popTextTitle, popTextDesc,0);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			iconSampleObject.transform.localScale = new Vector3(0.6f,0.6f,1);
			iconSampleObjectList.Add(iconSampleObject);
		}
		EnemyRoot.GetComponent<UIGrid>().repositionNow = true;
	}
	private void OnCreateBossCallBack(ref WWW p_www, string p_path, Object p_object)
	{
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
////		Debug.Log ("createBossPara = " +createBossPara);
//		for (int n = 0; n < createBossPara; n++)
//		{
//			//Debug.Log ("bossnum = " +n);
//
//			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//			iconSampleObject.SetActive(true);
//			iconSampleObject.transform.parent = EnemyRoot.transform;
//			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//			
//			if (allenemy >= EnemyNumBers)
//			{
//				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, -20, 0);
//			}
//			else
//			{
//				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
//			}
//			
//			HuangyeNPCTemplate mHuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(Bosses[n]);
//			float boold = 1.0f;
//			for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
//			{
//				if(Bosses[n] == M_Treas_info.npcInfos[i].npcId)
//				{
//					boold = (float)M_Treas_info.npcInfos[i].remainHP/(float)M_Treas_info.npcInfos[i].totalHP;
//
//					Debug.Log("boold = " +boold);
//				}
//			}
//
//			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mHuangyeNPCTemplate.name);
//			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mHuangyeNPCTemplate.level.ToString();
//			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mHuangyeNPCTemplate.desc).description;
//			
//			string leftTopSpriteName = null;
//			var rightButtomSpriteName = "boss";
//			if(boold > 0)
//			{
//				iconSampleManager.SetProgressBar(boold);
//			}
//			else
//			{
//			    iconSampleManager.MiddleSprite.gameObject.SetActive(true);
//			}
//
//			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//            iconSampleManager.SetIconBasic(7, mHuangyeNPCTemplate.icon.ToString(), "", "", boold <= 0);
//			//iconSampleManager.SetIconPopText(popTextTitle, popTextDesc);
//			iconSampleManager.SetIconPopText(0,popTextTitle, popTextDesc,0);
//			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
//		}
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index){
			case ProtoIndexes.S_HYTREASURE_BATTLE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				HYTreasureBattleResp mHYTreasureBattleResp = new HYTreasureBattleResp();
				
				t_qx.Deserialize(t_stream, mHYTreasureBattleResp, mHYTreasureBattleResp.GetType());

//				Debug.Log("打开宝藏洞信息返回了");

				InitUI(mHYTreasureBattleResp);

				return true;
			}
		
			default: return false;
			}
			
		}
		
		return false;
	}



	private GameObject RankUI;
	MaxDamageRankResp m_MaxDamageRank;

	void InitRankUI(MaxDamageRankResp m_MaxDamageRankResp)
	{
		if(RankUI == null)
		{
			m_MaxDamageRank = m_MaxDamageRankResp;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.DAMAGERANK), LoadRankUIBack); // 还没添加路劲，
		}
	}


	void LoadRankUIBack(ref WWW p_www, string p_path, Object p_object)
	{
		RankUI = Instantiate (p_object) as GameObject;
		
		RankUI.SetActive (true);
		RankUI.transform.parent = this.transform;
		
		RankUI.transform.localPosition = Vector3.zero;
		
		RankUI.transform.localScale = Vector3.one;

		My_DamageRank mMy_DamageRank = RankUI.GetComponent<My_DamageRank>();

		mMy_DamageRank.mRankList = m_MaxDamageRank;

		mMy_DamageRank.m_levelid = mHuangYeTreasure.guanQiaId;

		mMy_DamageRank.Init ();

		//HY_UIManager.Instance().ShowOrClose ();
	}

	IEnumerator CountTime()
	{
		while(m_Time > 0)
		{
			yield return new WaitForSeconds (1.0f);
			m_Time -=1;

			int D = (int)(m_Time/(60*60*24));
			int H = (int)((m_Time%(60*60*24))/(60*60));
			int M = (int)(((m_Time%(60*60*24))%(60*60))/60);
			int S = (int)(m_Time%60);
			
			CloseTime.text = D.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.DAY)+H.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.HOUR)+M.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE)+S.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);

		}
		CloseTime.text = "0"+LanguageTemplate.GetText(LanguageTemplate.Text.DAY)+"0"+LanguageTemplate.GetText(LanguageTemplate.Text.HOUR)+"0"+LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE)+"0"+LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);
		//ChangeMiBaoBtn.SetActive(false);
		ShowBtnBox.SetActive(false);

	}
	public HYTreasureBattleResp M_Treas_info;
	public void InitUI(HYTreasureBattleResp Treas_info)
	{
		M_Treas_info = Treas_info;

		m_Time = Treas_info.remainTime ;

		if(M_Treas_info.thisBoCi != null && M_Treas_info.allBoCi != null)
		{
			Boci.text = "当前波次("+M_Treas_info.thisBoCi.ToString()+"/"+M_Treas_info.allBoCi.ToString()+")";
		}
		if (!m_isEnterBattle) {
			initEnemy( );
			
			ShowMiBaoSkill ();
			
			StopCoroutine ("CountTime");
			
			StartCoroutine("CountTime");
			
			ShowRemainTime ();
		}
		if(Treas_info.status == 1 ) //有人在挑战中
		{
			ShowPlayerDoingTiaoZhan.SetActive(true);
	
			m_objInBattle.SetActive(false);

//				Debug.Log (" Treas_info.battleName== "+Treas_info.battleName);
		}
		else
		{
			//ChangeMiBaoBtn.SetActive(true);
			ShowPlayerDoingTiaoZhan.SetActive(false);

			m_objInBattle.SetActive(true);

//				Debug.Log(m_isEnterBattle);

			if(m_isEnterBattle)
			{
//					Debug.Log("asdasd");
				if(Treas_info.conditionIsOk)
				{
				    Global.m_isOpenHuangYe = true;
					CityGlobalData.IsOPenHyLeveUI = false;
					HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.guanQiaId);
					Debug.Log("mHuangYeTreasure.guanQiaId = "+mHuangYeTreasure.guanQiaId);
					Debug.Log("mHuangYeTreasure.id = "+mHuangYeTreasure.id);
					EnterBattleField.EnterBattleHYPve (mHuangYeTreasure.id, mHuangYePveTemplate);
				}
				else
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),NoCondiction);
				}
			}
		}

		m_isEnterBattle = false;
	}

	public void ShowMiBaoSkill()
	{

		if(M_Treas_info.zuheId < 1 )
		{
			if(!MiBaoGlobleData.Instance().GetMiBaoskillOpen())
			{
				mSparkleEffectItem.enabled = false ;
			}
			else
			{
				mSparkleEffectItem.enabled = true ;
			}
			MiBaoicon.spriteName = "";
			MiBaoicon.gameObject.SetActive(false);
			NoMiBaoSkillMind.SetActive(MiBaoGlobleData.Instance().GetMiBaoskillOpen());
			return;
		}
		NoMiBaoSkillMind.SetActive(false);
		MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(M_Treas_info.zuheId);
		mSparkleEffectItem.enabled = false ;
		MiBaoicon.gameObject.SetActive(true);
		MiBaoicon.spriteName = mMiBaoskill.icon.ToString ();
		//TimeAndAllTimes.text = M_Treas_info.timesOfDay.ToString () + "/" + M_Treas_info.totalTimes.ToString ();
	}
	public void ShowRemainTime()
	{
	
		TimeAndAllTimes.text = M_Treas_info.timesOfDay.ToString () + "/" + M_Treas_info.totalTimes.ToString ();
	}
	public void Trea_ChangeMiBaoBtn() //  选择秘宝
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeSkillLoadBack);
	}


	void ChangeSkillLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.HY_TreSend ),M_Treas_info.zuheId );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
		
	}
	void HavNoTime(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "您今日的挑战次数已经用完!";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,str, null,null,confirmStr,null,null,null,null);
	}
	public void HY_EnterBattle() //  进入战斗接口
	{
		if(M_Treas_info.timesOfDay <= 0) // 次数为0
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),HavNoTime);
			return;
		}

		RefreshRetearce();
		m_isEnterBattle = true;
	}
	public void Refresh()
	{
		RefreshRetearce();
	}
	public void RefreshRetearce()
	{
		HYTreasureBattle mHYTreasureBattle = new HYTreasureBattle ();
		
		MemoryStream HYTreasureBattleStream = new MemoryStream ();
		
		QiXiongSerializer HYTreasureBattleper = new QiXiongSerializer ();
//		Debug.Log ("mHuangYeTreasure.id = "+mHuangYeTreasure.id);
		mHYTreasureBattle.id = mHuangYeTreasure.id;
		
		HYTreasureBattleper.Serialize (HYTreasureBattleStream,mHYTreasureBattle);
		
		byte[] t_protof;
		
		t_protof = HYTreasureBattleStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.C_HYTREASURE_BATTLE, 
			ref t_protof,
			true,
			ProtoIndexes.S_HYTREASURE_BATTLE_RESP );
	}
	public void ShowDamageRank()
	{
		// 显示伤害排名
		MaxDamageRankReq mMaxDamageRankReq = new MaxDamageRankReq ();
		
		MemoryStream HYTreasureBattleStream = new MemoryStream ();
		
		QiXiongSerializer HYTreasureBattleper = new QiXiongSerializer ();

		//Debug.Log ("mHuangYeTreasure.id = "+mHuangYeTreasure.id);

		mMaxDamageRankReq.id = mHuangYeTreasure.id;
		
		HYTreasureBattleper.Serialize (HYTreasureBattleStream,mMaxDamageRankReq);
		
		byte[] t_protof;
		
		t_protof = HYTreasureBattleStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.MAX_DAMAGE_RANK_REQ, 
			ref t_protof,
			true,
			ProtoIndexes.MAX_DAMAGE_RANK_RESP );
	}

	public void AddTimes(int i)
	{
	   if(i == 2)
		{
			//Debug.Log("M_Treas_info.buyCiShuInfo = "+M_Treas_info.buyCiShuInfo);
			if(M_Treas_info.buyCiShuInfo == 1)
			{
				if(M_Treas_info.leftBuyCiShu <= 0)
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
				}
				else{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenBuyUILoadBack);
				}
			}
			if(M_Treas_info.buyCiShuInfo == 2)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpVipLoadBack);
			}
			if(M_Treas_info.buyCiShuInfo == 3)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
			}
		}
	}
	void UpVipLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		int vip = 3;
		if(JunZhuData.Instance().m_junzhuInfo.vipLv > 3)
		{
			vip = 7;
		}
//		if(JunZhuData.Instance().m_junzhuInfo.vipLv > 7)
//		{
//			vip = 11;
//		}
//		if(JunZhuData.Instance().m_junzhuInfo.vipLv > 7)
//		{
//			vip = 15;
//		}
		string str =LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc0)+vip.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc1)+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
		
		uibox.setBox(titleStr,null, str,null,confirmStr,null,null,null,null);
	}
	void NoCondiction(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		HuangYePveTemplate mHuangyePve = HuangYePveTemplate.getHuangYePveTemplatee_byid(mHuangYeTreasure.guanQiaId);

		string str = "只有先通关过关斩将第"+mHuangyePve.condition.ToString()+"章才能挑战该宝藏点！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, str,null,confirmStr,null,null,null,null);
	}
	void buyFailLoad(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		int vip = 3;
		string str = "";
		if (JunZhuData.Instance().m_junzhuInfo.vipLv < 3) {
			str = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc0)+vip.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc1)+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);

		}
		if(JunZhuData.Instance().m_junzhuInfo.vipLv >= 3&& JunZhuData.Instance().m_junzhuInfo.vipLv<7)
		{
			vip = 7;
			str = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc0)+vip.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc1)+LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
		}

		if (JunZhuData.Instance().m_junzhuInfo.vipLv >= 7) {

			str  = "今日的购买次数已经用完了，请明日再来吧。";
		}
		uibox.setBox(titleStr,null, str,null,confirmStr,null,null,null,null);
	}
	
	void OpenBuyUILoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买次数";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "您是否要花费"+M_Treas_info.buyNextMoney.ToString()+"元宝购买"+M_Treas_info.buyNextCiShu.ToString()+"次挑战次数？\r\n 今日还可购买"+M_Treas_info.leftBuyCiShu.ToString()+"次。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, str,null,CancleBtn,confirmStr,SureBuy,null,null);
	}
	void SureBuy(int i)
	{
		int mMoney  = JunZhuData.Instance().m_junzhuInfo.yuanBao; 
		
		
		if(i == 2)
		{
			if(mMoney < M_Treas_info.buyNextMoney)
			{
                EquipSuoData.TopUpLayerTip();
            }
			else
			{
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.HY_BUY_BATTLE_TIMES_REQ);
			}
		}
	}
	public void BuyTiLi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
	}
	/// <summary>
	/// Buy_s the money.
	/// </summary>
	public void Buy_Money()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
	}
	/// <summary>
	/// Buy_s the yuan bao.
	/// </summary>
	public void Buy_YuanBao()
	{
		MainCityUI.ClearObjectList();
		EquipSuoData.TopUpLayerTip();
		//		QXTanBaoData.Instance().CheckFreeTanBao();
	}
	public void OnStronger()
	{
		
		MainCityUILT.ShowMainTipWindow();
		
		Global.m_isOpenPVP = false;
		
	}
	public void Backbtn()
	{
		CityGlobalData.IsOPenHyLeveUI = false;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
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
}

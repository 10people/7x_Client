using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class HYRetearceEnemy : MonoBehaviour , SocketProcessor { //突袭藏宝点

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

	public UILabel mTiLi;
	
	public UILabel mTongBi;
	
	public UILabel mYuanBao;

	public UISprite TuiJianMiBaoicon;

	public static HYRetearceEnemy Instance ()
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
	

	void Update () {

		mTiLi.text = JunZhuData.Instance ().m_junzhuInfo.tili.ToString();
		
		mTongBi.text = JunZhuData.Instance ().m_junzhuInfo.jinBi.ToString();
		
		mYuanBao.text = JunZhuData.Instance ().m_junzhuInfo.yuanBao.ToString();
	
	}

	public void Init()
	{
		EnemyNumBers = 4;
		initHYTreasureBattleUI ();
		RefreshRetearce();
		ShowBtnBox.SetActive(false);

	}

	public void initHYTreasureBattleUI()
	{
		//BooldNumb.text = mHuangYeTreasure.jindu+"%";

		HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);

		string mHuangYeDesc = DescIdTemplate.GetDescriptionById (mHuangYePveTemplate.descId);

		string mName = NameIdTemplate.GetName_By_NameId (mHuangYePveTemplate.nameId);

		TuiJianMiBaoicon.spriteName = "";
		char[] separator = new char[]{'#'};
		
		string[] s = mHuangYeDesc.Split (separator);
		
		string desText = "";
		for(int j = 0; j < s.Length; j++ )
		{
			desText += s[j]+"\r\n";
		}
		Lv_Instruction.text = desText;
		Res_Name.text = mName;
		//m_UISlider.value = (float)( mHuangYeTreasure.jindu )/ (float)(100);
		int id = mHuangYeTreasure.fileId;

		InitDropthings(id);

		CityGlobalData.IsOPenHyLeveUI = true;
	}
	void InitDropthings(int m_id)
	{
		awardNum = 6;

		List<int> t_items = new List<int>();

		HuangYePveTemplate pvetemp = HuangYePveTemplate.getHuangYePveTemplatee_byid(m_id);

//		Debug.Log ("pvetemp.award = " +pvetemp.award);

		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = pvetemp.award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);

			if(!t_items.Contains(int.Parse(t_finals[0])))
			{
				t_items.Add(int.Parse(t_finals[0]));
			}

		}
		//		Debug.Log ("t_items.count  " +t_items.Count);
		
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
			//Debug.Log ("itemsPara[n] = " +itemsPara[n]);
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
			//Debug.Log ("mAwardTemp.count = " +mAwardTemp.Count);
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
					iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
				}
			}
		}
	}
	void initEnemy( )
	{
	     	
		List<HuangyeNPCTemplate> mHuangyeNPCTemplateList = new List<HuangyeNPCTemplate> ();

		mHuangyeNPCTemplateList.Clear ();

		for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
		{
			//Debug.Log(" M_Treas_info.npcInfos = "+ M_Treas_info.npcInfos[i].npcId);

			HuangyeNPCTemplate m_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(M_Treas_info.npcInfos[i].npcId);

			mHuangyeNPCTemplateList.Add(m_HuangyeNPCTemplate);
		}

//			HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (m_id);
//		    int npcid = mHuangYePveTemplate.npcId;

//		List<HuangyeNPCTemplate> mHuangyeNPCTemplateList = HuangyeNPCTemplate.GetHuangyeNPCTemplates_By_npcid(npcid);
		
		foreach(HuangyeNPCTemplate mHuangyeNPCTemplate in mHuangyeNPCTemplateList)
		{
			int icom = int.Parse(mHuangyeNPCTemplate.icon);
			if(icom != 0&& mHuangyeNPCTemplate.type == 4&& !Bosses.Contains(mHuangyeNPCTemplate.id)) // boss
			{
				Bosses.Add(mHuangyeNPCTemplate.id);
			}
			
			if(icom != 0&& mHuangyeNPCTemplate.type == 3&& !heros.Contains(mHuangyeNPCTemplate.id)) // hero
			{

				heros.Add(mHuangyeNPCTemplate.id);

			}
			
			if(icom != 0&& mHuangyeNPCTemplate.type == 2&& !soldires.Contains(mHuangyeNPCTemplate.id)) // Solider
			{

				soldires.Add(mHuangyeNPCTemplate.id);

			}
		}
//		for (int i = 0; i < soldires.Count-1; i ++)
//		{
//			
//			HuangyeNPCTemplate m_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id (soldires [i]);
//			
//			for(int j = i+1; j < soldires.Count; )
//			{
//				HuangyeNPCTemplate j_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id (soldires [j]);
//				if(m_HuangyeNPCTemplate.profession == j_HuangyeNPCTemplate.profession)
//				{
//					
//					soldires.RemoveAt(j);
//				}
//				else{
//					j ++;
//				}
//			}
//			
//		}
//		
//		
//		for (int i = 0; i < heros.Count-1; i ++)
//		{
//			HuangyeNPCTemplate m_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id (heros [i]);
//			
//			for(int j = i+1; j < heros.Count; )
//			{
//				HuangyeNPCTemplate j_HuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id (heros [j]);
//				if(m_HuangyeNPCTemplate.profession == j_HuangyeNPCTemplate.profession)
//				{
//					
//					heros.RemoveAt(j);
//				}
//				else{
//					j ++;
//				}
//			}
//			
//		}
		getEnemyData ();
	}
	
	void getEnemyData()
	{
		//List<string> EyName = new List<string>(GetPveTempID.NewEnemy.Keys);
		
		int bossnum = Bosses.Count;
		int heronum = heros.Count;
		int solder = soldires.Count;
		
		Debug.Log ("boss个数：" + bossnum);
		Debug.Log ("hero个数：" + heronum);
		Debug.Log ("soldier个数：" + solder);
		Debug.Log ("EnemyNumBers：" + EnemyNumBers);
		
		if (bossnum > 0)//BOSS个数不为0
		{
			if (bossnum < EnemyNumBers)//boss不大于大于6个
			{
				if (heronum > 0)//w武将个数大于0
				{
					if (heronum + bossnum < EnemyNumBers)//w武将和bpss的总个数小于6
					{
						if (solder > 0)
						{
							if (heronum + bossnum + solder > EnemyNumBers)
							{
								createBoss(bossnum);
								Debug.Log("1ci");
								createHeros(heronum);
								createSoliders(EnemyNumBers - (bossnum + heronum));
							}
							else
							{
								createBoss(bossnum);
								Debug.Log("2ci");
								createHeros(heronum);
								createSoliders(solder);
							}
						}
						else
						{
							createBoss(bossnum);
							Debug.Log("3ci");
							createHeros(heronum);
						}
					}
					else
					{//boss和武将的和大于6了 只创建6个
						createBoss(bossnum);
						Debug.Log("4ci");
						createHeros(EnemyNumBers - bossnum);
					}
				}
				else
				{
					//ww武将为0
					if (solder > 0)
					{
						if (solder + bossnum > EnemyNumBers)
						{
							createBoss(bossnum);
							createSoliders(EnemyNumBers - bossnum);
						}
						else
						{
							createBoss(bossnum);
							createSoliders(solder);
						}
					}
					else
					{
						createBoss(bossnum);
					}
				}
			}
			else
			{
				//boss的个数大于6
				createBoss(EnemyNumBers);
			}
		}
		else
		{
			//假如boss的个数为0000000000
			if (heronum > 0)//w武将个数大于0
			{
				if (heronum < EnemyNumBers)//w武将和bpss的总个数小于6
				{
					if (solder > 0)
					{
						if (heronum + solder <= EnemyNumBers)
						{
							createHeros(heronum);
//							Debug.Log("5ci");
							createSoliders(solder);
						}
						else
						{
//							Debug.Log("6ci");
							createHeros(heronum);
							createSoliders(EnemyNumBers - heronum);
						}
					}
					else
					{
//						Debug.Log("7ci");
						createHeros(heronum);
					}
				}
				else
				{
//					Debug.Log("8ci");
					createHeros(EnemyNumBers);
				}
			}
			else
			{
				if (solder > EnemyNumBers)
				{
					createSoliders(EnemyNumBers);
				}
				else
				{
					createSoliders(solder);
				}
			}
		}
		//this.gameObject.GetComponent<UIGrid>().repositionNow = true;
	}
	private void OnCreateBossCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
//		Debug.Log ("createBossPara = " +createBossPara);
		for (int n = 0; n < createBossPara; n++)
		{
			//Debug.Log ("bossnum = " +n);

			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			iconSampleObject.SetActive(true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
			}
			
			HuangyeNPCTemplate mHuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(Bosses[n]);
			float boold = 1.0f;
			for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
			{
				if(Bosses[n] == M_Treas_info.npcInfos[i].npcId)
				{
					boold = (float)M_Treas_info.npcInfos[i].remainHP/(float)M_Treas_info.npcInfos[i].totalHP;

					Debug.Log("boold = " +boold);
				}
			}

			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mHuangyeNPCTemplate.name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mHuangyeNPCTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mHuangyeNPCTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "boss";
			if(boold > 0)
			{
				iconSampleManager.SetProgressBar(boold);
			}
			else
			{
			    iconSampleManager.MiddleSprite.gameObject.SetActive(true);
			}

			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
            iconSampleManager.SetIconBasic(7, mHuangyeNPCTemplate.icon.ToString(), "", "", boold <= 0);
			//iconSampleManager.SetIconPopText(popTextTitle, popTextDesc);
			iconSampleManager.SetIconPopText(0,popTextTitle, popTextDesc,0);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createBossPara;
	
	private int allenemy
	{
		get { return Bosses.Count + heros.Count + soldires.Count; }
	}
	
	void createBoss(int i)
	{
		createBossPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateBossCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateBossCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private void OnCreateHeroCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
//		Debug.Log ("createHeroPara = " +createHeroPara);
		for (int n = 0; n < createHeroPara; n++)
		{

			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

			iconSampleObject.transform.parent = EnemyRoot.transform;

			iconSampleObject.SetActive(true);

			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - Bosses.Count - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - Bosses.Count - n) * distance - countDistance, -20, 0);
			}
			Debug.Log("heros[n] = "+heros[n]);
			HuangyeNPCTemplate mHuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(heros[n]);
			float boold = 1.0f;
			for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
			{
				if(heros[n] == M_Treas_info.npcInfos[i].npcId)
				{
					boold = (float)M_Treas_info.npcInfos[i].remainHP/(float)M_Treas_info.npcInfos[i].totalHP;
					
//					Debug.Log("boold = " +boold);
				}
			}
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mHuangyeNPCTemplate.name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mHuangyeNPCTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mHuangyeNPCTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
			if(boold > 0)
			{
				iconSampleManager.SetProgressBar(boold);
			}
            else
            {
                iconSampleManager.MiddleSprite.gameObject.SetActive(true);
            }

			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
            iconSampleManager.SetIconBasic(7, mHuangyeNPCTemplate.icon.ToString(), "", "", boold <= 0);
			iconSampleManager.SetIconPopText(0,popTextTitle, popTextDesc,0);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createHeroPara;
	
	void createHeros(int i)
	{
		createHeroPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateHeroCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateHeroCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private void OnCreateSoldierCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
//		Debug.Log ("createSoldierPara = " +createSoldierPara);
		for (int n = 0; n < createSoldierPara; n++)
		{
			//Debug.Log ("solder = " +n);
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			iconSampleObject.transform.parent = EnemyRoot.transform;
			iconSampleObject.SetActive(true);
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, -20, 0);
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
			
			HuangyeNPCTemplate mHuangyeNPCTemplate = HuangyeNPCTemplate.GetHuangyeNPCTemplate_By_id(soldires[n]);
			float boold = 1.0f;
			for(int i = 0 ; i < M_Treas_info.npcInfos.Count; i ++)
			{
				if(soldires[n] == M_Treas_info.npcInfos[i].npcId)
				{
					boold = (float)M_Treas_info.npcInfos[i].remainHP/(float)M_Treas_info.npcInfos[i].totalHP;
					
//					Debug.Log("boold = " +boold);
				}
			}
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mHuangyeNPCTemplate.name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mHuangyeNPCTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mHuangyeNPCTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
			if(boold > 0)
			{
				iconSampleManager.SetProgressBar(boold);
			}
            else
            {
                iconSampleManager.MiddleSprite.gameObject.SetActive(true);
            }

			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
            iconSampleManager.SetIconBasic(7, mHuangyeNPCTemplate.icon.ToString(), "", "", boold <= 0);
			iconSampleManager.SetIconPopText(0,popTextTitle, popTextDesc,0);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createSoldierPara;
	
	void createSoliders(int i)
	{
		createSoldierPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateSoldierCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateSoldierCallBack(ref temp, null, IconSamplePrefab);
		}
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
			case ProtoIndexes.MAX_DAMAGE_RANK_RESP :
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MaxDamageRankResp mMaxDamageRankResp = new MaxDamageRankResp();
				
				t_qx.Deserialize(t_stream, mMaxDamageRankResp, mMaxDamageRankResp.GetType());

				Debug.Log("_____排行榜");
				InitRankUI(mMaxDamageRankResp);
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

		mMy_DamageRank.Init ();

		//HY_UIManager.Instance ().ShowOrClose ();
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

		initEnemy( );

		ShowMiBaoSkill ();

//		Debug.Log (" 1 somebody Treas_info.status == "+Treas_info.status);

		StopCoroutine ("CountTime");

		StartCoroutine("CountTime");

		ShowRemainTime ();

//		Debug.Log(Treas_info.timesOfDay);


//			Debug.Log(Treas_info.status);

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
						HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);
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
			MiBaoicon.spriteName = "";
			return;
		}
		MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(M_Treas_info.zuheId);

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
		
		string str = "您今日的挑战次数已经用完，请先购买挑战次数";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
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

		Debug.Log ("mHuangYeTreasure.id = "+mHuangYeTreasure.id);

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

	public void AddTimes()
	{
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
	void UpVipLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "Vip等级不足，提升Vip等级可增加购买次数！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}
	void NoCondiction(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		HuangYePveTemplate mHuangyePve = HuangYePveTemplate.getHuangYePveTemplatee_byid(mHuangYeTreasure.fileId);

		string str = "只有先通关过关斩将第"+mHuangyePve.condition.ToString()+"章才能挑战该宝藏点！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}
	void buyFailLoad(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "对不起，您今日购买次数已用尽！改日再来吧";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}
	
	void OpenBuyUILoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买次数";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "您是否要花费"+M_Treas_info.buyNextMoney.ToString()+"元宝购买"+M_Treas_info.buyNextCiShu.ToString()+"次挑战次数？\r\n 今日还可购买"+M_Treas_info.leftBuyCiShu.ToString()+"次";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,SureBuy,null,null);
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
	public void Backbtn()
	{
		CityGlobalData.IsOPenHyLeveUI = false;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
	}

}

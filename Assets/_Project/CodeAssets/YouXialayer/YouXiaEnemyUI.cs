using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YouXiaEnemyUI : MonoBehaviour,SocketProcessor {

	public UILabel Best_Num;

	public UILabel All_Num;

	public UILabel Instruction;

	public UILabel Difficult;

	public string StrDifficult;

	public string Youxia_Name;

	public UILabel YouxiaName;

	public int l_id;

	public int big_id;

	public static YouXiaEnemyUI mYouXiaEnemyUI;

	public YouXiaInfo m_You_XiaInfo;

	public Transform EnemyRoot;

	public Transform AwardRoot;

	[HideInInspector]
	public GameObject IconSamplePrefab;
	
	List<int> GetPveEnemyId = new List<int>();
	
	List<int> soldires = new List<int>();
	List<int> heros = new List<int>();
	List<int> Bosses = new List<int>();
	List<int> Zhi_Ye = new List<int>();
	
	int EnemyNumBers = 0;//显示敌人数量

	private int awardNum = 4;//掉落物品个数

	int distance = 120;//敌人头像距离

	int countDistance = 360;//偏移量

	List <GameObject> mHerosIcon = new List<GameObject> (); 

	List <GameObject> m_AwardIcon = new List<GameObject> (); 

	public YouXiaGuanQiaInfoResp m_YouXiaGuanQiaInfoResp;

	private bool Can_Saodang;

	public GameObject obj_SaodangBtn;

	public GameObject obj_EnterBattleBtn;

	public GameObject ChangeMiBaoSKillBtn;

	public PveSaoDangRet saodinfo;

	public UISprite MiBaoSkillIcon;
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);

		mYouXiaEnemyUI = this;
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void SaodangBtn()  // w未定协议7 23
	{
		if(EnterYouXiaBattle.GlobleEnterYouXiaBattle.m_Time == 1)
		{
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
		YouXiaSaoDangReq saodanginfo = new YouXiaSaoDangReq ();
		
		MemoryStream saodangstream = new MemoryStream ();
		
		QiXiongSerializer saodangSer = new QiXiongSerializer ();
		
		int i = l_id;

		saodanginfo.guanQiaId = l_id;

		saodanginfo.times = 1;

		saodangSer.Serialize (saodangstream, saodanginfo);
		
		byte[] t_protof;
		
		t_protof = saodangstream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YOUXIA_SAO_DANG_REQ,ref t_protof);

	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YOUXIA_SAO_DANG_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
						
				PveSaoDangRet tempInfo = new PveSaoDangRet();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				saodinfo = tempInfo;

				getSaoDangData(tempInfo);

				InitUIData();

				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);

				ChooseYouXiaUIManager.mChooseYouXiaUIManager.mYouXia_Info.remainTimes -= 1;
				ChooseYouXiaUIManager.mChooseYouXiaUIManager.Init();
				return true;
			}
			case ProtoIndexes.S_YOUXIA_GUANQIA_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaGuanQiaInfoResp tempInfo = new YouXiaGuanQiaInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				m_YouXiaGuanQiaInfoResp = tempInfo;

				showChengJi(tempInfo);

				Debug.Log("扫荡接收完毕。。。。");

				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}

	void showChengJi(YouXiaGuanQiaInfoResp mtempInfo)
	{
		Best_Num.text = mtempInfo.bestScore.ToString ();

		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);

		All_Num.text = myouxia.maxNum.ToString ();

		Debug.Log ("是否可以扫荡 = "+mtempInfo.saoDang);
		Debug.Log ("剩余次数 = "+m_You_XiaInfo.remainTimes );
		Debug.Log ("倒计时 = "+mtempInfo.time );
		if(mtempInfo.saoDang && m_You_XiaInfo.remainTimes > 0)
		{
			obj_SaodangBtn.SetActive(true);
		}
		else
		{
			obj_SaodangBtn.SetActive(false);

		}
		if(mtempInfo.time > 0)
		{
			obj_EnterBattleBtn.SetActive(false);
		}
	}

	void getSaoDangData(PveSaoDangRet mtempInfo)
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),LoadResourceCallback);
	}


	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
		GameObject obj = GameObject.Find ("YouXiaEnemy(Clone)");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localPosition = new Vector3 (0,0,0);
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		SaoDangManeger mSaoDangManeger = tempOjbect.GetComponent<SaoDangManeger>();
		
		mSaoDangManeger.m_PveSaoDangRet = saodinfo;
		
		mSaoDangManeger.SaodangType = 2;
		
		mSaoDangManeger.Init ();
	}

	public void Init()
	{
		InitUIData ();

		CreateEnemy ();
	
		CreateAward ();
	}
	void InitUIData()
	{
		YouXiaGuanQiaInfoReq mYouXiaGuanQiaInfoReq = new YouXiaGuanQiaInfoReq ();
		
		MemoryStream YouXiaGuanQiaInfoReqtream = new MemoryStream ();
		
		QiXiongSerializer YouXiaGuanQiaInfoReqSer = new QiXiongSerializer ();
		
		int i = l_id;
		
		mYouXiaGuanQiaInfoReq.guanQiaId = i;
		
		YouXiaGuanQiaInfoReqSer.Serialize (YouXiaGuanQiaInfoReqtream, mYouXiaGuanQiaInfoReq);
		
		byte[] t_protof;
		
		t_protof = YouXiaGuanQiaInfoReqtream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YOUXIA_GUANQIA_REQ,ref t_protof);
		
		Difficult.text = StrDifficult;
		
		YouxiaName.text = Youxia_Name;
		
		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
		
		string mDesc = DescIdTemplate.GetDescriptionById (mYouxiaPveTemplate.smaDesc);
		
		Instruction.text = mDesc;

		Debug.Log ("m_You_XiaInfo.zuheId = "+m_You_XiaInfo.zuheId);

		ShowMiBaoSkillIcon ();

		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			ChangeMiBaoSKillBtn.SetActive(false);
		}
		else
		{
			ChangeMiBaoSKillBtn.SetActive(true);
		}
	}

	public void ShowMiBaoSkillIcon()
	{
	
		if(m_You_XiaInfo.zuheId < 1)
		{
			MiBaoSkillIcon.spriteName = "";
		}
		else
		{
			MiBaoSkillTemp mMiBAo = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi(m_You_XiaInfo.zuheId,2);
			
			MiBaoSkillIcon.spriteName = mMiBAo.icon.ToString();
		}
	}
	public void EnterBattleBtn()
	{
		Debug.Log ("big_id = "+big_id +"l_id%10 = " +l_id%10);
		if(EnterYouXiaBattle.GlobleEnterYouXiaBattle.m_Time == 1)
		{
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
		EnterBattleField.EnterBattleYouXia (big_id, l_id%10);
	}
	public void ChangerMiBaoSkillBtn()
	{
		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			return;
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadMiBaoBack);
	}

	void LoadMiBaoBack(ref WWW p_www, string p_path, Object p_object)
	{

		EnterYouXiaBattle.GlobleEnterYouXiaBattle.SecendNeedCloseObg = this.gameObject;

		GameObject mChoose_MiBao = Instantiate (p_object) as GameObject;
		
		mChoose_MiBao.SetActive (true);
		
		mChoose_MiBao.transform.parent = this.transform.parent;
		
		mChoose_MiBao.transform.localPosition = Vector3.zero;
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();

		int mibaotype = 0;

		switch(big_id)
		{
		case 1:
			mibaotype = (int)CityGlobalData.MibaoSkillType.YX_JinBi;
			break;
		case 2:
			mibaotype = (int)CityGlobalData.MibaoSkillType.YX_Cailiao;
			break;
		case 3:
			mibaotype = (int)CityGlobalData.MibaoSkillType.YX_Jingpo;
			break;

		default:
			break;
		}
		mChangeMiBaoSkill.GetRootName (this.gameObject.name);
		mChangeMiBaoSkill.Init(mibaotype, m_You_XiaInfo.zuheId);
		EnterYouXiaBattle.GlobleEnterYouXiaBattle.SecondShowOrClose ();
	}

	public void CloseBtn()
	{
		GameObject desgameobj = GameObject.Find ("Enter_YouXiaBattle(Clone)");

		MainCityUI.TryRemoveFromObjectList(desgameobj);

		Destroy (desgameobj);
	}
	public void BackBtn()
	{
		EnterYouXiaBattle.GlobleEnterYouXiaBattle.ThirdShowOrClose ();
		Destroy (this.gameObject);
	}
	private int NpcId;
	private void CreateEnemy()
	{
		foreach(GameObject con in mHerosIcon)
		{
			Destroy(con);
			
		}
		mHerosIcon.Clear ();
		
		soldires.Clear();
		heros.Clear();
		Bosses.Clear();
		Zhi_Ye.Clear();

		EnemyNumBers = 4;

		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
		 
		List<YouXiaNpcTemplate> mYouXiaNpcTemplateList = YouXiaNpcTemplate.GetYouXiaNpcTemplates_By_npcid(mYouxiaPveTemplate.npcId);

		Debug.Log ("l_id = " +l_id);

		NpcId = mYouxiaPveTemplate.npcId;

		Debug.Log ("mYouxiaPveTemplate.npcId = " +mYouxiaPveTemplate.npcId);

		Debug.Log ("mYouXiaNpcTemplateList.conut = " +mYouXiaNpcTemplateList.Count);

		foreach(YouXiaNpcTemplate mYouXiaNpcTemplate in mYouXiaNpcTemplateList)
		{
			int icn = int.Parse(mYouXiaNpcTemplate.icon);

			if(mYouXiaNpcTemplate.type == 4&& !Bosses.Contains(mYouXiaNpcTemplate.EnemyId) && icn != 0) // boss
			{
				Bosses.Add(mYouXiaNpcTemplate.id);
			}

			if(mYouXiaNpcTemplate.type == 3&& !heros.Contains(mYouXiaNpcTemplate.id)&& icn != 0) // hero
			{

				heros.Add(mYouXiaNpcTemplate.id);
				
			}
			
			if(mYouXiaNpcTemplate.type == 2&& !soldires.Contains(mYouXiaNpcTemplate.id)&& icn != 0) // Solider
			{
				
				soldires.Add(mYouXiaNpcTemplate.id);
				
			}
		}
	
		for (int i = 0; i < soldires.Count-1; i ++)
		{
			
			YouXiaNpcTemplate m_YouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id (soldires [i],NpcId);
			
			for(int j = i+1; j < soldires.Count; )
			{
				YouXiaNpcTemplate j_YouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id (soldires [j],NpcId);

				if(m_YouXiaNpcTemplate.profession == j_YouXiaNpcTemplate.profession)
				{
					soldires.RemoveAt(j);
				}
				else{
					j ++;
				}
			}
			
		}
				
		for (int i = 0; i < heros.Count-1; i ++)
		{
			//Debug.Log("heros[i] = "+heros[i]);
			YouXiaNpcTemplate m_YouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id (heros [i],NpcId);
			
			for(int j = i+1; j < heros.Count; )
			{
				YouXiaNpcTemplate j_YouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id (heros [j],NpcId);

				if(m_YouXiaNpcTemplate.profession == j_YouXiaNpcTemplate.profession)
				{
					heros.RemoveAt(j);
				}
				else{
					j ++;
				}
			}
		
		}

		getEnemyData();
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
								createHeros(heronum);
								createSoliders(EnemyNumBers - (bossnum + heronum));
							}
							else
							{
								createBoss(bossnum);
								createHeros(heronum);
								createSoliders(solder);
							}
						}
						else
						{
							createBoss(bossnum);
							createHeros(heronum);
						}
					}
					else
					{//boss和武将的和大于6了 只创建6个
						createBoss(bossnum);
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
							createSoliders(solder);
						}
						else
						{
							createHeros(heronum);
							createSoliders(EnemyNumBers - heronum);
						}
					}
					else
					{
						createHeros(heronum);
					}
				}
				else
				{
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
		
		for (int n = 0; n < createBossPara; n++)
		{
			
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

			mHerosIcon.Add(iconSampleObject);

			iconSampleObject.SetActive (true);

			iconSampleObject.transform.parent = EnemyRoot;

			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, 0, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, 0, 0);
			}

				//EnemyNameid = LegendNpcTemplate.GetEnemyNameId_By_EnemyId(Bosses[n]);
				
			YouXiaNpcTemplate mYouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id(Bosses[n],NpcId);
				
		    NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mYouXiaNpcTemplate.Name);
		    var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mYouXiaNpcTemplate.level.ToString();
		    var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mYouXiaNpcTemplate.desc).description;
			
		    string leftTopSpriteName = null;
			var rightButtomSpriteName = "boss";
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
		    iconSampleManager.SetIconBasic(15, mYouXiaNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
	
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
		
		for (int n = 0; n < createHeroPara; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot;
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - Bosses.Count - n) * distance - countDistance, 0, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - Bosses.Count - n) * distance - countDistance, 0, 0);
			}
			
			Debug.Log("eros[n] = " +heros[n]);
			YouXiaNpcTemplate mYouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id(heros[n],NpcId);
			
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mYouXiaNpcTemplate.Name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mYouXiaNpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mYouXiaNpcTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
			Debug.Log("mYouXiaNpcTemplate.icon.ToString() = " +mYouXiaNpcTemplate.icon.ToString());
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(15, mYouXiaNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			
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
		
		for (int n = 0; n < createSoldierPara; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

			mHerosIcon.Add(iconSampleObject);
			
			iconSampleObject.SetActive (true);

			iconSampleObject.transform.parent = EnemyRoot;

			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, 0, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, 0, 0);
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
			
			int EnemyNameid = 0;

			YouXiaNpcTemplate mYouXiaNpcTemplate = YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id(soldires[n],NpcId);
			
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mYouXiaNpcTemplate.Name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mYouXiaNpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mYouXiaNpcTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";

			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(15, mYouXiaNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			
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

	private int numPara;
	private List<int> itemsPara;

	private void CreateAward()
	{
		foreach(GameObject maward in m_AwardIcon)
		{
			Destroy(maward);
			
		}
		m_AwardIcon.Clear ();

		List<int> t_items = new List<int>();

		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById(l_id);

		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = mYouxiaPveTemplate.awardId.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[0] != "")
			{
				t_items.Add(int.Parse(t_finals[0]));
			}
		}
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
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		int pos = 0;
		
		Debug.Log("numPara = " +numPara);
		
		for (int n = 0; n < numPara; n++)
		{
			//Debug.Log("itemsPara[n] = " +itemsPara[n]);
			
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
			
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
					m_AwardIcon.Add(iconSampleObject);
					iconSampleObject.SetActive(true);
					iconSampleObject.transform.parent = AwardRoot;
                    iconSampleObject.transform.localPosition = new Vector3(-240 + (pos - 1) * 120, 1, 1);
					iconSampleObject.transform.localScale = Vector3.one;

                    var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

                    NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);

                    iconSampleManager.SetIconByID(mAwardTemp[i].itemId, "", 15);
                    iconSampleManager.SetIconPopText(mAwardTemp[0].itemId, mNameIdTemplate.Name, mdesc, 1);
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class HuangyeZhengRong : MonoBehaviour {
//	public HuangYeResource mHuangYeResource;//
//	public HuangYeTreasure mHuangYeTreasure;
//	public UILabel Zhanli;
//	[HideInInspector]public bool IsRes; // 判断是否是资源岛  如果不是则为宝藏点
//
//
//	List<int> GetPveEnemyId = new List<int>();
//	
//	List<int> soldires = new List<int>();
//	List<int> heros = new List<int>();
//	List<int> Bosses = new List<int>();
//	List<int> Zhi_Ye = new List<int>();
//
//	int EnemyNumBers = 0;//显示敌人数量
//	int distance = 120;//敌人头像距离
//	int countDistance = 360;//偏移量
//
//	private int awardNum;//掉落物品个数
//
//	[HideInInspector]public GameObject IconSamplePrefab;
//
//	public GameObject EnemyRoot;
//	public GameObject DropthingsRoot;
//	void Start () {
//	
//	}
//	
//
//	void Update () {
//	
//	}
//
//	public void init()
//	{
//		EnemyNumBers = 6;
//		soldires.Clear();
//		heros.Clear();
//		Bosses.Clear();
//		Zhi_Ye.Clear();
//		Debug.Log ("IsRes = "+IsRes);
//		if(IsRes)
//		{
//			int id = mHuangYeResource.fileId;
//			initResEnemy(id);
//			//InitDropthings(id);
//		}
//		else
//		{
//			int id = mHuangYeTreasure.fileId;
//			initResEnemy(id);
//			InitDropthings(id);
//		}
//	}
//	void InitDropthings(int m_id)
//	{
//		awardNum = 6;
//		List<int> t_items = new List<int>();
//		HuangYePveTemplate pvetemp = HuangYePveTemplate.getHuangYePveTemplatee_byid(m_id);
//		//		Debug.Log ("pvetemp.awardId ：" +pvetemp.awardId);
//		Debug.Log ("pvetemp.award = " +pvetemp.award);
//		char[] t_items_delimiter = { ',' };
//		
//		char[] t_item_id_delimiter = { '=' };
//		
//		string[] t_item_strings = pvetemp.award.Split(t_items_delimiter);
//		
//		for (int i = 0; i < t_item_strings.Length; i++)
//		{
//			string t_item = t_item_strings[i];
//			
//			string[] t_finals = t_item.Split(t_item_id_delimiter);
//			
//			t_items.Add(int.Parse(t_finals[0]));
//		}
//		//		Debug.Log ("t_items.count  " +t_items.Count);
//		
//		int initNum;
//		if (awardNum >= t_items.Count)
//		{
//			initNum = t_items.Count;
//		}
//		else
//		{
//			initNum = awardNum;
//		}
//		
//		numPara = initNum;
//		itemsPara = t_items;
//		
//		if (IconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
//		}
//		else
//		{
//			WWW temp = null;
//			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
//		}
//	}
//
//	private int numPara;
//	private List<int> itemsPara;
//
//	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		Debug.Log ("numPara = " +numPara);
//		for (int n = 0; n < numPara; n++)
//		{
//			Debug.Log ("itemsPara[n] = " +itemsPara[n]);
//			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
//			Debug.Log ("mAwardTemp.count = " +mAwardTemp.Count);
//			for (int i = 0; i < mAwardTemp.Count; i++)
//			{
//				if (IconSamplePrefab == null)
//				{
//					IconSamplePrefab = p_object as GameObject;
//				}
//				
//				GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//				iconSampleObject.SetActive(true);
//				iconSampleObject.transform.parent = DropthingsRoot.transform;
//				iconSampleObject.transform.localPosition = new Vector3(-240 + (i + n) * 120, 1, 1);
//				
//				var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//				
//				ItemTemp mItemTemp = ItemTemp.getItemTempById(mAwardTemp[i].itemId);
//				var iconSpriteName = mItemTemp.icon;
//				
//				NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
//				string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);
//				var popTitle = mNameIdTemplate.Name;
//				var popDesc = mdesc;
//				
//				iconSampleManager.SetIconType(IconSampleManager.IconType.item);
//				iconSampleManager.SetIconBasic(15, iconSpriteName);
//				iconSampleManager.SetIconPopText(popTitle, popDesc, 1);
//			}
//		}
//	}
//	void initResEnemy(int m_id)
//	{
//
//		Debug.Log ("id = " +m_id);
//		Debug.Log ("IsRes = " +IsRes);
//		if(IsRes)
//		{
//
//			//d后台发 的Enemy显示
//
//		}
//		else{
//
//			HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (m_id);
//			GetPveEnemyId = HuangyeNPCTemplate.getEnemyidlist_by_Npcid (mHuangYePveTemplate.npcId);
//
//			Debug.Log("mHuangYePveTemplate.npcid = "+mHuangYePveTemplate.npcId);
//
//			Zhanli.text = mHuangYePveTemplate.power.ToString ();  // 战斗力				Debug.Log ("GetPveEnemyId.Count = " +GetPveEnemyId.Count);
//			for (int m = 0; m < GetPveEnemyId.Count; m++)
//			{
//				EnemyTemp E_temp = EnemyTemp.getEnemyTempById(GetPveEnemyId[m]);
//				
//				if (E_temp.zhiye >= 21 && E_temp.zhiye < 30)
//				{
//					//士兵
//					if (!soldires.Contains(GetPveEnemyId[m]))
//					{
//						if (!Zhi_Ye.Contains(E_temp.zhiye))
//						{
//							soldires.Add(GetPveEnemyId[m]);
//							Zhi_Ye.Add(E_temp.zhiye);
//						}
//					}
//				}
//				else if (E_temp.zhiye <= 14)
//				{
//					//武将
//					if (!heros.Contains(GetPveEnemyId[m]))
//					{
//						heros.Add(GetPveEnemyId[m]);
//					}
//				}
//				else if (E_temp.zhiye >= 30)
//				{
//					//Boss
//					if (!Bosses.Contains(GetPveEnemyId[m]))
//					{
//						Bosses.Add(GetPveEnemyId[m]);
//					}
//				}
//			}
//			getEnemyData ();
//		}
//
//	}
//
//	void getEnemyData()
//	{
//		//List<string> EyName = new List<string>(GetPveTempID.NewEnemy.Keys);
//		
//		int bossnum = Bosses.Count;
//		int heronum = heros.Count;
//		int solder = soldires.Count;
//		
//		//		Debug.Log ("boss个数：" + bossnum);
//		//		Debug.Log ("hero个数：" + heronum);
//		//		Debug.Log ("soldier个数：" + solder);
//		
//		if (bossnum > 0)//BOSS个数不为0
//		{
//			if (bossnum < EnemyNumBers)//boss不大于大于6个
//			{
//				if (heronum > 0)//w武将个数大于0
//				{
//					if (heronum + bossnum < EnemyNumBers)//w武将和bpss的总个数小于6
//					{
//						if (solder > 0)
//						{
//							if (heronum + bossnum + solder > EnemyNumBers)
//							{
//								createBoss(bossnum);
//								createHeros(heronum);
//								createSoliders(EnemyNumBers - (bossnum + heronum));
//							}
//							else
//							{
//								createBoss(bossnum);
//								createHeros(heronum);
//								createSoliders(solder);
//							}
//						}
//						else
//						{
//							createBoss(bossnum);
//							createHeros(heronum);
//						}
//					}
//					else
//					{//boss和武将的和大于6了 只创建6个
//						createBoss(bossnum);
//						createHeros(EnemyNumBers - bossnum);
//					}
//				}
//				else
//				{
//					//ww武将为0
//					if (solder > 0)
//					{
//						if (solder + bossnum > EnemyNumBers)
//						{
//							createBoss(bossnum);
//							createSoliders(EnemyNumBers - bossnum);
//						}
//						else
//						{
//							createBoss(bossnum);
//							createSoliders(solder);
//						}
//					}
//					else
//					{
//						createBoss(bossnum);
//					}
//				}
//			}
//			else
//			{
//				//boss的个数大于6
//				createBoss(EnemyNumBers);
//			}
//		}
//		else
//		{
//			//假如boss的个数为0000000000
//			if (heronum > 0)//w武将个数大于0
//			{
//				if (heronum < EnemyNumBers)//w武将和bpss的总个数小于6
//				{
//					if (solder > 0)
//					{
//						if (heronum + solder <= EnemyNumBers)
//						{
//							createHeros(heronum);
//							createSoliders(solder);
//						}
//						else
//						{
//							createHeros(heronum);
//							createSoliders(EnemyNumBers - heronum);
//						}
//					}
//					else
//					{
//						createHeros(heronum);
//					}
//				}
//				else
//				{
//					createHeros(EnemyNumBers);
//				}
//			}
//			else
//			{
//				if (solder > EnemyNumBers)
//				{
//					createSoliders(EnemyNumBers);
//				}
//				else
//				{
//					createSoliders(solder);
//				}
//			}
//		}
//		//this.gameObject.GetComponent<UIGrid>().repositionNow = true;
//	}
//	private void OnCreateBossCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
//		
//		for (int n = 0; n < createBossPara; n++)
//		{
//			
//			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//			iconSampleObject.SetActive(true);
//			iconSampleObject.transform.parent = EnemyRoot.transform;
//			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//			
//			if (allenemy >= EnemyNumBers)
//			{
//				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, 0, 0);
//			}
//			else
//			{
//				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, 0, 0);
//			}
//			
//			int EnemyNameid = NpcTemplate.GetEnemyNameId_By_EnemyId(Bosses[n]);
//			NameIdTemplate EnemyNamestr = NameIdTemplate.getNameIdTemplateByNameId(EnemyNameid);
//			
//			EnemyTemp enemyif = EnemyTemp.getEnemyTempById(Bosses[n]);
//			
//			var popTextTitle = EnemyNamestr.Name + " " + "LV" + enemyif.level;
//			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(EnemyNameid).description;
//			
//			string leftTopSpriteName;
//			if (mLeGendNpcTemp.gongjiType == 11)
//			{
//				leftTopSpriteName = "dun";//盾
//			}
//			else if (mLeGendNpcTemp.gongjiType == 12)
//			{
//				leftTopSpriteName = "piccar";//车
//			}
//			else if (mLeGendNpcTemp.gongjiType == 13)
//			{
//				leftTopSpriteName = "gong";//弓箭
//			}
//			else if (mLeGendNpcTemp.gongjiType == 14)
//			{
//				leftTopSpriteName = "hours";//Horse////........马
//			}
//			else
//			{
//				
//				leftTopSpriteName = null;
//			}
//			
//			var rightButtomSpriteName = "boss";
//			
//			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//			iconSampleManager.SetIconBasic(15, "Enemy1");
//			iconSampleManager.SetIconPopText(popTextTitle, popTextDesc, 1);
//			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//		}
//	}
//	
//	private int createBossPara;
//	
//	private int allenemy
//	{
//		get { return Bosses.Count + heros.Count + soldires.Count; }
//	}
//	
//	void createBoss(int i)
//	{
//		createBossPara = i;
//		
//		if (IconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateBossCallBack);
//		}
//		else
//		{
//			WWW temp = null;
//			OnCreateBossCallBack(ref temp, null, IconSamplePrefab);
//		}
//	}
//	
//	private void OnCreateHeroCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
//		
//		for (int n = 0; n < createHeroPara; n++)
//		{
//			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//			iconSampleObject.transform.parent = EnemyRoot.transform;
//			iconSampleObject.SetActive(true);
//			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//			
//			if (allenemy >= EnemyNumBers)
//			{
//				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - Bosses.Count - n) * distance - countDistance, 0, 0);
//			}
//			else
//			{
//				iconSampleObject.transform.localPosition = new Vector3((allenemy - Bosses.Count - n) * distance - countDistance, 0, 0);
//			}
//			
//			int EnemyNameid = NpcTemplate.GetEnemyNameId_By_EnemyId(heros[n]);
//			NameIdTemplate EnemyNamestr = NameIdTemplate.getNameIdTemplateByNameId(EnemyNameid);
//			EnemyTemp enemyif = EnemyTemp.getEnemyTempById(heros[n]);
//			
//			var popTextTitle = EnemyNamestr.Name + " " + "LV" + enemyif.level;
//			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(EnemyNameid).description;
//			
//			string leftTopSpriteName;
//			if (mLeGendNpcTemp.gongjiType == 11)
//			{
//				leftTopSpriteName = "dun";//盾
//			}
//			else if (mLeGendNpcTemp.gongjiType == 12)
//			{
//				leftTopSpriteName = "piccar";//车
//			}
//			else if (mLeGendNpcTemp.gongjiType == 13)
//			{
//				leftTopSpriteName = "gong";//弓箭
//			}
//			else if (mLeGendNpcTemp.gongjiType == 14)
//			{
//				leftTopSpriteName = "hours";//Horse////........马
//			}
//			else
//			{
//				leftTopSpriteName = null;
//			}
//			
//			string rightButtomSpriteName = null;
//			
//			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//			iconSampleManager.SetIconBasic(15, "Enemy1");
//			iconSampleManager.SetIconPopText(popTextTitle, popTextDesc, 1);
//			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//		}
//	}
//	
//	private int createHeroPara;
//	
//	void createHeros(int i)
//	{
//		createHeroPara = i;
//		
//		if (IconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateHeroCallBack);
//		}
//		else
//		{
//			WWW temp = null;
//			OnCreateHeroCallBack(ref temp, null, IconSamplePrefab);
//		}
//	}
//	
//	private void OnCreateSoldierCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		if (IconSamplePrefab == null)
//		{
//			IconSamplePrefab = p_object as GameObject;
//		}
//		
//		for (int n = 0; n < createSoldierPara; n++)
//		{
//			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
//			iconSampleObject.transform.parent = EnemyRoot.transform;
//			iconSampleObject.SetActive(true);
//			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
//			
//			if (allenemy >= EnemyNumBers)
//			{
//				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - (Bosses.Count + heros.Count)
//				                                                        - n) * distance - countDistance, 0, 0);
//			}
//			else
//			{
//				iconSampleObject.transform.localPosition = new Vector3((allenemy - (Bosses.Count + heros.Count)
//				                                                        - n) * distance - countDistance, 0, 0);
//			}
//			iconSampleObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
//			
//			int EnemyNameid = NpcTemplate.GetEnemyNameId_By_EnemyId(soldires[n]);
//			NameIdTemplate EnemyNamestr = NameIdTemplate.getNameIdTemplateByNameId(EnemyNameid);
//			EnemyTemp enemyif = EnemyTemp.getEnemyTempById(soldires[n]);
//			
//			var popTextTitle = EnemyNamestr.Name + " " + "LV" + enemyif.level;
//			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(EnemyNameid).description;
//			
//			string leftTopSpriteName;
//			if (mLeGendNpcTemp.gongjiType == 11)
//			{
//				leftTopSpriteName = "dun";//盾
//			}
//			else if (mLeGendNpcTemp.gongjiType == 12)
//			{
//				leftTopSpriteName = "piccar";//车
//			}
//			else if (mLeGendNpcTemp.gongjiType == 13)
//			{
//				leftTopSpriteName = "gong";//弓箭
//			}
//			else if (mLeGendNpcTemp.gongjiType == 14)
//			{
//				leftTopSpriteName = "hours";//Horse////........马
//			}
//			else
//			{
//				leftTopSpriteName = null;
//			}
//			
//			string rightButtomSpriteName = null;
//			
//			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//			iconSampleManager.SetIconBasic(15, "Enemy1");
//			iconSampleManager.SetIconPopText(popTextTitle, popTextDesc, 1);
//			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//		}
//	}
//	
//	private int createSoldierPara;
//	
//	void createSoliders(int i)
//	{
//		createSoldierPara = i;
//		
//		if (IconSamplePrefab == null)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateSoldierCallBack);
//		}
//		else
//		{
//			WWW temp = null;
//			OnCreateSoldierCallBack(ref temp, null, IconSamplePrefab);
//		}
//	}
//	public void EnterBattleBtn()
//	{
//
//	}
//	public void Back()
//	{
//		Destroy (this.gameObject);
//	}
}

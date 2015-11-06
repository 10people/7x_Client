using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralTiaoZhan : MonoBehaviour {

	public static GeneralTiaoZhan generalTiaoZhan;
	/// <summary>
	/// PVP INFO
	/// </summary>
	private ChallengeResp challengeInfo;
	private int yinDaoState;
	public int SetYinDaoState
	{
		set{yinDaoState = value;}
	}

	/// <summary>
	/// 荒野
	/// </summary>
	//private BattleResouceResp hyBattleResourceRes;//挑战资源点返回信息
	//private ResourceNpcInfo resourceNpcInfo;//资源点npc信息
	//private HuangYeResource hyResource;//资源点信息

	/// <summary>
	/// 掠夺阵容信息
	/// </summary>
	private LveGoLveDuoResp lueDuoOpponentRes;
	public GameObject lueDuoObj;
	public UILabel getGongJinNum;
	public UILabel mGongJinNum;
	public UILabel lueDuoTime;

	private GameObject iconSamplePrefab;
	private List<IconSampleManager> comIconList = new List<IconSampleManager>();//itemList
	public GameObject mLockIconObj;//我的技能锁
	public GameObject eLockIconObj;//敌方技能锁

	public enum ZhenRongType
	{
		PVP,
		HUANG_YE,
		LUE_DUO,
	}
	private ZhenRongType zhenRongType = ZhenRongType.PVP;

	//**********我的阵容信息*********//
	public UILabel m_ZhanLi;//我的战力

	public UISprite m_SkillIcon;//我的秘宝技能icon

	private int m_zuHeId;//我的秘宝技能组合id
	public int SetZuHeId
	{
		set{m_zuHeId = value;}
	}
	//**********我的阵容信息*********//

	//**********敌方阵容信息*********//
	public UILabel e_ZhanLi;//敌方战力

	public UISprite e_SkillIcon;//敌方秘宝技能icon

	private int e_zuHeId;//敌方秘宝技能组合id

	private int activeMibaoCount;//敌人激活秘宝数
	//**********敌方阵容信息*********//

	/// <summary>
	/// 佣兵icon显示位置
	/// </summary>
	private readonly Vector3 e_Hero_Pos = Vector3.zero;
	private readonly Vector3 m_Hero_Pos = Vector3.zero;
	private readonly Vector3 e_Soldiers_Pos = new Vector3(-120, -10, 0);
	private readonly Vector3 m_Soldiers_Pos = new Vector3(120, 10, 0);
	private const int iconBasicDepth = 15;
	private int bingCount = 4;//佣兵最大个数

	/// <summary>
	/// 头像item父对象
	/// </summary>
	public GameObject myItemParent;
	public GameObject enemyItemParent;

	public GameObject backBtnObj;

	public GameObject changeSkillBtn;//更换秘宝技能按钮

	void Awake ()
	{
		generalTiaoZhan = this;
	}

	/// <summary>
	/// PVP挑战阵容页面
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="yinDaoState">新手引导步骤</param>
	public void InItPvpChallengePage (ZhenRongType tempType,ChallengeResp tempInfo)
	{
		zhenRongType = tempType;

		challengeInfo = tempInfo;

		backBtnObj.SetActive (false);

		m_zuHeId = tempInfo.myZuheId;
		e_zuHeId = tempInfo.oppZuheId;
		activeMibaoCount = tempInfo.oppActivateMiBaoCount;

		m_ZhanLi.text = tempInfo.myZhanli.ToString();//显示我的战力
		e_ZhanLi.text = tempInfo.oppoZhanli.ToString();//显示敌方战力

		//引导相关
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			int state = -1;
			switch (yinDaoState)
			{
			case 1:
				state = 5;
				break;

			case 2:
				state = 6;
				break;
			default:
				break;
			}
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[state]);
		}

		InItGuYongBingInfo ();
		ShowMyMiBaoSkill ();
		ShowEnemyMiBaoInfo ();

		ShowChangeSkillEffect (true);
	}

//	/// <summary>
//	/// 荒野挑战阵容页面
//	/// </summary>
//	/// <param name="tempType">Temp type.</param>
//	/// <param name="tempRes">Temp res.</param>
//	/// <param name="tempInfo">Temp info.</param>
//	public void InItHuangYeChallengePage(ZhenRongType tempType,BattleResouceResp tempRes,ResourceNpcInfo tempInfo,HuangYeResource tempResource,int resourceType,int eSkillId,int eZhanLi)
//	{
//		zhenRongType = tempType;
//
//		//hyBattleResourceRes = tempRes;
//		//resourceNpcInfo = tempInfo;
//		//hyResource = tempResource;
//		
//		//m_SkillId = tempRes.zuheId;
//		e_SkillId = eSkillId;
//		
//		HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid (tempResource.fileId);//m描述
//		
//		string mDescIdTemplate = DescIdTemplate.GetDescriptionById (mHuangyeTemplate.descId);
//		
//		string mName = NameIdTemplate.GetName_By_NameId (mHuangyeTemplate.nameId);
//		//Res_Name.text = mName;
//		if (resourceType != 1)//1-我的资源点 2-敌方资源点
//		{
//			InItGuYongBingInfo ();
//			ShowMyMiBaoSkill ();
//
//			m_ZhanLi.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString();//显示我的战力
//			
//			e_ZhanLi.text = eZhanLi.ToString();//显示敌方战力
//		}
//	}

	/// <summary>
	/// 掠夺阵容页面
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempLueDuoRes">Temp lue duo res.</param>
	public void InItLueDuoChallengePage (ZhenRongType tempType,LveGoLveDuoResp tempLueDuoRes)
	{
		zhenRongType = tempType;

		lueDuoOpponentRes = tempLueDuoRes;

		m_zuHeId = tempLueDuoRes.myGongjiZuheId;
		e_zuHeId = tempLueDuoRes.oppFangShouZuheId;
		activeMibaoCount = tempLueDuoRes.oppoActivateMiBaoCount;

		m_ZhanLi.text = tempLueDuoRes.myZhanli.ToString();//显示我的战力
		
		e_ZhanLi.text = tempLueDuoRes.oppoZhanli.ToString();//显示敌方战力

		lueDuoObj.SetActive (true);
		getGongJinNum.text = tempLueDuoRes.lostGongJin.ToString ();
		mGongJinNum.text = tempLueDuoRes.gongJin.ToString ();
		lueDuoTime.text = "剩余掠夺次数：" + (LueDuoData.Instance.lueDuoData.all - LueDuoData.Instance.lueDuoData.used) +
						"/" + LueDuoData.Instance.lueDuoData.all;

		InItGuYongBingInfo ();
		ShowMyMiBaoSkill ();
		ShowEnemyMiBaoInfo ();

		LueDuoData.Instance.IsStop = false;
		ShowChangeSkillEffect (true);
	}

	/// <summary>
	/// 初始化佣兵阵容
	/// </summary>
	void InItGuYongBingInfo ()
	{
		foreach (IconSampleManager comIcon in comIconList)
		{
			Destroy(comIcon.gameObject);
		}
		comIconList.Clear();
		
		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack(ref tempWww, null, iconSamplePrefab);
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		//store loaded prefab.
		if (iconSamplePrefab == null)
		{
			iconSamplePrefab = p_object as GameObject;
		}

		//***********我的阵容阵列***********//
		GameObject m_Hero = Instantiate(iconSamplePrefab) as GameObject;
		m_Hero.transform.parent = myItemParent.transform;
		m_Hero.transform.localPosition = m_Hero_Pos;
		m_Hero.SetActive(true);
		IconSampleManager mHeroIconSample = m_Hero.GetComponent<IconSampleManager>();
		mHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);
		int roleId = CityGlobalData.m_king_model_Id;
		mHeroIconSample.SetIconBasic(iconBasicDepth,"PlayerIcon" + roleId,
		                             JunZhuData.Instance().m_junzhuInfo.level.ToString());
		m_Hero.transform.localScale = Vector3.one * 1.1f;

		//显示我的雇佣兵
		int m_YongBingCount = 0;
		switch (zhenRongType)
		{
		case ZhenRongType.PVP:
		{
			m_YongBingCount = challengeInfo.mySoldiers.Count;
			break;
		}
//		case ZhenRongType.HUANG_YE:
//		{
//			m_YongBingCount = hyBattleResourceRes.yongBingList.Count;
//			break;
//		}
		case ZhenRongType.LUE_DUO:
		{
			if (lueDuoOpponentRes.mySoldiers.Count > bingCount)
			{
				m_YongBingCount = bingCount;
			}
			else
			{
				m_YongBingCount = lueDuoOpponentRes.mySoldiers.Count;
			}
			break;
		}
		default:
			break;
		}
		for (int i = 0; i < m_YongBingCount; i++)
		{
			GameObject mSoldier = Instantiate(iconSamplePrefab) as GameObject;
			mSoldier.transform.parent = myItemParent.transform;
			mSoldier.transform.localPosition = m_Soldiers_Pos + new Vector3(110 * i, 0, 0);
			mSoldier.SetActive(true);
			
			IconSampleManager mSoldierIconSample = mSoldier.GetComponent<IconSampleManager>();
			mSoldierIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);

			int profession = 0;//佣兵职业
			int iconId = 0;//佣兵头像id
			int needLevel = 0;
			int nameId = 0;//名字id
			int desId = 0;//描述

			switch (zhenRongType)
			{
			case ZhenRongType.PVP:
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(challengeInfo.mySoldiers[i].id);
				profession = yongBingTemp.profession;
				iconId = yongBingTemp.icon;
				needLevel = yongBingTemp.needLv;
				nameId = yongBingTemp.m_name;
				desId = yongBingTemp.description;

				break;
			}
//			case ZhenRongType.HUANG_YE:
//			{
//				HY_GuYongBingTempTemplate hyYongBingTemp = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(hyBattleResourceRes.yongBingList[i]);
//				profession = hyYongBingTemp.profession;
//				iconId = hyYongBingTemp.icon;
//				needLevel = hyYongBingTemp.needLv;
//				break;
//			}
			case ZhenRongType.LUE_DUO:
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(lueDuoOpponentRes.mySoldiers[i].id);
				profession = yongBingTemp.profession;
				iconId = yongBingTemp.icon;
				needLevel = yongBingTemp.needLv;
				nameId = yongBingTemp.m_name;
				desId = yongBingTemp.description;

				break;
			}
			default:
				break;
			}

			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(nameId);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + needLevel.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(desId).description;

			string rBottomSpriteName = "";
			mSoldierIconSample.SetIconBasic(iconBasicDepth,iconId.ToString (),needLevel.ToString());
			mSoldierIconSample.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			mSoldierIconSample.SetIconDecoSprite(Profession(profession), rBottomSpriteName);

			mSoldier.transform.localScale = Vector3.one * 0.9f;
			comIconList.Add(mSoldierIconSample);
		}

		//***********敌方阵容阵列***********//
		GameObject e_Hero = Instantiate(iconSamplePrefab) as GameObject;
		e_Hero.transform.parent = enemyItemParent.transform;
		e_Hero.transform.localPosition = e_Hero_Pos;
		e_Hero.SetActive(true);
		IconSampleManager eHeroIconSample = e_Hero.GetComponent<IconSampleManager>();

		if (zhenRongType == ZhenRongType.HUANG_YE)
		{
//			eHeroIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//			//enemyHeroIconSample.SetIconDecoSprite(null, "boss");
//			HuangyePvpNpcTemplate hyPvpNpcTemp = HuangyePvpNpcTemplate.getHuangyepvpNPCTemplate_by_Npcid(resourceNpcInfo.bossId);
//			eHeroIconSample.SetIconBasic(iconBasicDepth,hyPvpNpcTemp.icon.ToString(),hyPvpNpcTemp.level.ToString());
		}
		else if (zhenRongType == ZhenRongType.PVP)
		{
			eHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);
			//enemyHeroIconSample.SetIconDecoSprite(null, "boss");
			eHeroIconSample.SetIconBasic(iconBasicDepth,"PlayerIcon" + challengeInfo.oppoRoleId, 
			                             challengeInfo.oppoLevel.ToString());
		}
		else if (zhenRongType == ZhenRongType.LUE_DUO)
		{
			eHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);
			//enemyHeroIconSample.SetIconDecoSprite(null, "boss");
			eHeroIconSample.SetIconBasic(iconBasicDepth,"PlayerIcon" + lueDuoOpponentRes.oppoRoleId, 
			                             lueDuoOpponentRes.oppoLevel.ToString());
		}

		e_Hero.transform.localScale = Vector3.one * 1.1f;

		//显示敌方的雇佣兵
		int e_YongBingCount = 0;
		switch (zhenRongType)
		{
		case ZhenRongType.PVP:
		{
			e_YongBingCount = challengeInfo.oppoSoldiers.Count;
			break;
		}
//		case ZhenRongType.HUANG_YE:
//		{
//			e_YongBingCount = resourceNpcInfo.yongBingId.Count;
//			break;
//		}
		case ZhenRongType.LUE_DUO:
		{
			if (lueDuoOpponentRes.oppoSoldiers.Count > bingCount)
			{
				e_YongBingCount = bingCount;
			}
			else
			{
				e_YongBingCount = lueDuoOpponentRes.oppoSoldiers.Count;
			}
			break;
		}
		default:
			break;
		}
		for (int i = 0;i < e_YongBingCount;i ++)
		{
			GameObject eSoldier = Instantiate(iconSamplePrefab) as GameObject;
			eSoldier.transform.parent = enemyItemParent.transform;
			eSoldier.transform.localPosition = e_Soldiers_Pos + new Vector3(-110 * i, 0, 0);
			eSoldier.SetActive(true);

			IconSampleManager eSoldierIconSample = eSoldier.GetComponent<IconSampleManager>();
			eSoldierIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);

			int profession = 0;//佣兵职业
			int iconId = 0;//佣兵头像id
			int needLevel = 0;
			int nameId = 0;//名字id
			int desId = 0;//描述

			switch (zhenRongType)
			{
			case ZhenRongType.PVP:
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(challengeInfo.oppoSoldiers[i].id);
				profession = yongBingTemp.profession;
				iconId = yongBingTemp.icon;
				needLevel = yongBingTemp.needLv;
				nameId = yongBingTemp.m_name;
				desId = yongBingTemp.description;

				break;
			}
//			case ZhenRongType.HUANG_YE:
//			{
//				HY_GuYongBingTempTemplate hyYongBingTemp = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(resourceNpcInfo.yongBingId[i]);
//				profession = hyYongBingTemp.profession;
//				iconId = hyYongBingTemp.icon;
//				needLevel = hyYongBingTemp.needLv;
//				break;
//			}
			case ZhenRongType.LUE_DUO:
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(lueDuoOpponentRes.oppoSoldiers[i].id);
				profession = yongBingTemp.profession;
				iconId = yongBingTemp.icon;
				needLevel = yongBingTemp.needLv;
				nameId = yongBingTemp.m_name;
				desId = yongBingTemp.description;

				break;
			}
			default:
				break;
			}

			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(nameId);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + needLevel.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(desId).description;

			string rBottomSpriteName = "";
			eSoldierIconSample.SetIconBasic(iconBasicDepth, iconId.ToString(), needLevel.ToString());
			eSoldierIconSample.SetIconDecoSprite(Profession(profession), rBottomSpriteName);
			eSoldierIconSample.SetIconPopText(0, popTextTitle, popTextDesc, 1);

			eSoldier.transform.localScale = Vector3.one * 0.9f;
			comIconList.Add(eSoldierIconSample);
		}
	}

	/// <summary>
	/// 显示我的秘宝信息
	/// </summary>
	void ShowMyMiBaoSkill()
	{
		var miBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;

		for(int i = 0;i < miBaoInfo.mibaoGroup.Count;i ++)
		{
			if(miBaoInfo.mibaoGroup[i].zuheId == m_zuHeId)
			{
				if (miBaoInfo.mibaoGroup[i].hasActive == 1)
				{
					//秘宝技能可用
					mLockIconObj.SetActive (false);
					MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (m_zuHeId);
					Debug.Log ("mSkill.icon:" + mSkill.icon);
					m_SkillIcon.spriteName = mSkill.icon.ToString();

					break;
				}
				else
				{
					//秘宝技能不可用
					mLockIconObj.SetActive (true);
					m_SkillIcon.spriteName = "";
				}
			}
			else
			{
				//秘宝技能不可用
				mLockIconObj.SetActive (true);
				m_SkillIcon.spriteName = "";
			}
		}
	}

	/// <summary>
	/// 是否显示切换技能按钮特效
	/// </summary>
	public void ShowChangeSkillEffect (bool isOpen)
	{
		if (isOpen)
		{
			var mibaoGroupList = MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup;
			
			List<int> zuHeIdList = new List<int> ();
			
			for (int i = 0;i < mibaoGroupList.Count;i ++)
			{
				zuHeIdList.Add (mibaoGroupList[i].zuheId);
			}
			
			if (zuHeIdList.Contains (m_zuHeId))
			{
				changeSkillBtn.SetActive (true);
				UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
			}
			else
			{
				if (MiBaoGlobleData.Instance ().GetMiBaoskillOpen ())
				{
					changeSkillBtn.SetActive (true);
					UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,changeSkillBtn,
					                                               EffectIdTemplate.GetPathByeffectId(100006));
				}
				else
				{
					UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
				}
			}
		}
		else
		{
			UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
		}
	}

	/// <summary>
	/// 显示敌对的秘宝信息(不是自己资源点时候现在该秘宝函数)
	/// </summary>
	void ShowEnemyMiBaoInfo ()
	{
		var miBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;

		if (activeMibaoCount >= 2)
		{
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (e_zuHeId);
			e_SkillIcon.spriteName = miBaoSkillTemp.skill.ToString ();
		}
		else
		{
			e_SkillIcon.spriteName = "";
			eLockIconObj.SetActive(true);
		}
	}

	/// <summary>
	/// 佣兵职业
	/// </summary>
	/// <param name="index">Index.</param>
	private string Profession (int index)
	{
		string yongBingTypeStr = "";
		switch (index)
		{
		case 1:
			yongBingTypeStr = "dun";//盾
			break;
		case 2:
			yongBingTypeStr = "gang";//Horse////枪
			break;
		case 3:
			yongBingTypeStr = "gong";//弓箭
			break;
		case 4:
			yongBingTypeStr = "piccar";//车
			break;
		case 5:
			yongBingTypeStr = "hours";//Horse////马
			break;
		default:
			break;
		}
		return yongBingTypeStr;
	}

	/// <summary>
	/// 更换秘宝按钮
	/// </summary>
	public void ChangeMiBaoSkillBtn ()
	{
		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			return;
		}

		ShowChangeSkillEffect (false);

		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			UIYindao.m_UIYindao.CloseUI ();
		}

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeSkillLoadBack);
	}

	void ChangeSkillLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.SetActive(true);
		
		mChoose_MiBao.transform.parent = this.transform.parent;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0,0,-500);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		ChangeMiBaoSkill changeSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();
		
		switch (zhenRongType)
		{
		case ZhenRongType.HUANG_YE:

			changeSkill.Init((int)CityGlobalData.MibaoSkillType.HY_ResSend, m_zuHeId);

			break;

		case ZhenRongType.PVP:

			changeSkill.Init((int)CityGlobalData.MibaoSkillType.PvpSend, m_zuHeId);

			break;

		case ZhenRongType.LUE_DUO:

			changeSkill.Init((int)CityGlobalData.MibaoSkillType.LueDuo_GongJi, m_zuHeId);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 刷新秘宝技能信息
	/// </summary>
	public void RefreshMiBaoSkillInfo (int tempId)
	{
		m_zuHeId = tempId;

		ShowMyMiBaoSkill ();
	}

	/// <summary>
	/// 进入战斗按钮
	/// </summary>
	public void EnterBattle ()
	{
		switch (zhenRongType)
		{
//		case ZhenRongType.HUANG_YE:
//			
//			//进入荒野资源点战斗
//			HuangYePVPTemplate hyPvpTemp = HuangYePVPTemplate.getHuangYePVPTemplate_byid (hyResource.fileId);
//			EnterBattleField.EnterBattleHYPvp (hyResource.id, resourceNpcInfo.bossId, hyPvpTemp);
//			
//			break;
//			
		case ZhenRongType.PVP:
			
			BaiZhanUnExpected.unExpected.TiaoZhanStateReq (null,2,challengeInfo);
			
			break;
			
		case ZhenRongType.LUE_DUO:
			
			EnterBattleField.EnterBattleLueDuo (lueDuoOpponentRes.oppoId);
			
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 返回按钮
	/// </summary>
	public void BackBtn ()
	{
		if (zhenRongType == ZhenRongType.LUE_DUO)
		{
			GameObject lueDuo = GameObject.Find ("LueDuo");
			if (lueDuo)
			{
				LueDuoManager.ldManager.ShowChangeSkillEffect (true);
			}
		}
		Destroy (this.gameObject);
	}

	/// <summary>
	/// 关闭按钮
	/// </summary>
	public void DestroyRoot ()
	{
		Global.m_isOpenBaiZhan = false;

		switch (zhenRongType)
		{
//		case ZhenRongType.HUANG_YE:
//
//			GameObject HuangYe = GameObject.Find ("HuangYe_TiaoZhanZhengRong(Clone)");
//			if (HuangYe)
//			{
//				Destroy (HuangYe);
//			}
//			break;
			
		case ZhenRongType.PVP:
			
			GameObject baizhan = GameObject.Find ("BaiZhan");
			if (baizhan)
			{
				Destroy (baizhan);
			}
			
			break;
			
		case ZhenRongType.LUE_DUO:

			switch (LueDuoData.Instance.GetWhichType)
			{
			case LueDuoData.WhichOpponent.LUE_DUO:
			{
				GameObject lueDuo = GameObject.Find ("LueDuo");
				if (lueDuo)
				{
					Destroy (lueDuo);
				}
				break;
			}
			case LueDuoData.WhichOpponent.RANKLIST:
			{
				GameObject allianceMember = GameObject.Find ("AllianceMemberWindow(Clone)");
				if (allianceMember)
				{
					Destroy (allianceMember);
				}

				GameObject kingDetalInfo = GameObject.Find ("KingDetailInfoWindow(Clone)");
				if (kingDetalInfo)
				{
					Destroy (kingDetalInfo);
				}

				GameObject rankObj = GameObject.Find ("RankWindow(Clone)");
				if (rankObj)
				{
					Destroy (rankObj);
				}
				break;
			}
			case LueDuoData.WhichOpponent.CHAT:
			{
				GameObject chatObj = GameObject.Find ("UIChat(Clone)");
				if (chatObj)
				{
					ChatOpenCloseController chatOpen = chatObj.GetComponentInChildren <ChatOpenCloseController> ();
					chatOpen.OnCloseWindowClick (gameObject);
					Destroy (this.gameObject);
				}
				break;
			}
			default:
				break;
			}
			break;
		default:
			break;
		}
	}
}

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
	public int YinDaoState
	{
		set{yinDaoState = value;}
		get{return yinDaoState;}
	}
	private bool pvpReset = false;//是否重置百战
	public bool PvpReset
	{
		set{pvpReset = value;}
		get{return pvpReset;}
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

	public GameObject changeSkillBtn;//更换秘宝技能按钮

	public ScaleEffectController sEffectController;

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
		sEffectController.OnOpenWindowClick ();

		zhenRongType = tempType;

		challengeInfo = tempInfo;

		m_zuHeId = tempInfo.myZuheId;
		e_zuHeId = tempInfo.oppZuheId;

		m_ZhanLi.text = tempInfo.myZhanli.ToString();//显示我的战力
		e_ZhanLi.text = tempInfo.oppoZhanli.ToString();//显示敌方战力

		//引导相关
		int state = YinDaoState == 1 ? 5 : 6;
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,state);

		PvpReset = false;

		InItGuYongBingInfo ();
		ShowMyMiBaoSkill ();
		ShowEnemyMiBaoInfo ();

		ShowChangeSkillEffect (true);
	}

	/// <summary>
	/// 掠夺阵容页面
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempLueDuoRes">Temp lue duo res.</param>
	public void InItLueDuoChallengePage (ZhenRongType tempType,LveGoLveDuoResp tempLueDuoRes)
	{
		sEffectController.OnOpenWindowClick ();

		zhenRongType = tempType;

		lueDuoOpponentRes = tempLueDuoRes;

		m_zuHeId = tempLueDuoRes.myGongjiZuheId;
		e_zuHeId = tempLueDuoRes.oppFangShouZuheId;

		m_ZhanLi.text = tempLueDuoRes.myZhanli.ToString();//显示我的战力
		
		e_ZhanLi.text = tempLueDuoRes.oppoZhanli.ToString();//显示敌方战力

		lueDuoObj.SetActive (true);
		getGongJinNum.text = tempLueDuoRes.lostGongJin.ToString ();
		mGongJinNum.text = tempLueDuoRes.gongJin.ToString ();
		lueDuoTime.text = "剩余掠夺次数：" 
						+ (PlunderData.Instance.PlunderDataResp ().all - PlunderData.Instance.PlunderDataResp ().used) 
						+ "/" + PlunderData.Instance.PlunderDataResp ().all;

		InItGuYongBingInfo ();
		ShowMyMiBaoSkill ();
		ShowEnemyMiBaoInfo ();

//		LueDuoData.Instance.IsStop = false;
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

		if (zhenRongType == ZhenRongType.PVP)
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
		var skillList = MiBaoGlobleData.Instance().G_MiBaoInfo.skillList;

		if (skillList == null)
		{
			return;
		}

		for (int i = 0;i < skillList.Count;i ++)
		{
			if (m_zuHeId == skillList[i].activeZuheId)
			{
				mLockIconObj.SetActive (false);
				MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (m_zuHeId);
				m_SkillIcon.spriteName = mSkill.icon.ToString();
				break;
			}
			else
			{
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
			var skillList = MiBaoGlobleData.Instance().G_MiBaoInfo.skillList;
			
			if (skillList != null)
			{
				List<int> skillIdList = new List<int>();
				foreach (SkillInfo skill in skillList)
				{
					skillIdList.Add (skill.activeZuheId);
				}

				if (skillIdList.Contains (m_zuHeId))
				{
					UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
				}
				else
				{
					UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,changeSkillBtn,
					                                               EffectIdTemplate.GetPathByeffectId(100006));
				}
			}
			else
			{
				UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
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
		var skillList = MiBaoGlobleData.Instance().G_MiBaoInfo.skillList;
		for (int i = 0;i < skillList.Count;i ++)
		{
			if (e_zuHeId == skillList[i].activeZuheId)
			{
				eLockIconObj.SetActive(false);
				MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (e_zuHeId);
				e_SkillIcon.spriteName = miBaoSkillTemp.skill.ToString ();

				break;
			}
			else
			{
				e_SkillIcon.spriteName = "";
				eLockIconObj.SetActive(true);
			}
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

		if(QXComData.CheckYinDaoOpenState (100180))
		{
			UIYindao.m_UIYindao.CloseUI ();
		}

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeSkillLoadBack);
	}

	void ChangeSkillLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();

		mNewMiBaoSkill.Init ((int)(CityGlobalData.MibaoSkillType.PvpSend ), m_zuHeId );

		MainCityUI.TryAddToObjectList(mChoose_MiBao);

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
		case ZhenRongType.PVP:

			PvpData.Instance.PvpChallengeResp = challengeInfo;
			PvpData.Instance.PlayerStateCheck (PvpData.PlayerState.STATE_GENERAL_TIAOZHAN_PAGE);

			break;
			
		case ZhenRongType.LUE_DUO:
			
			EnterBattleField.EnterBattleLueDuo (lueDuoOpponentRes.oppoId);
			
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 关闭按钮
	/// </summary>
	public void DestroyRoot ()
	{
		switch (zhenRongType)
		{	
		case ZhenRongType.PVP:

			Global.m_isOpenBaiZhan = false;
			PvpData.Instance.IsOpenPvpByBtn = false;

			sEffectController.OnCloseWindowClick ();
			sEffectController.CloseCompleteDelegate += DisActiveObj;

			break;
			
		case ZhenRongType.LUE_DUO:

			Destroy (gameObject);

			break;
		default:
			break;
		}
	}

	void DisActiveObj ()
	{
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);

		if (PvpReset)
		{
			PvpData.Instance.PvpDataReq ();
		}
	}
}

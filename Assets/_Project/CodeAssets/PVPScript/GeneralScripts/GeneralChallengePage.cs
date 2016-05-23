using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralChallengePage : GeneralInstance<GeneralChallengePage> {

	public enum ChallengeType
	{
		SPORT,
		PLUNDER,
	}

	private ChallengeType m_challengeType;

	public delegate void ChallengeDelegate ();
	public ChallengeDelegate M_ChallengeDelegate;

	public ScaleEffectController sEffectController;
	public GameObject m_topRightObj;

	private GameObject iconSamplePrefab;

	#region ProtoMessage
	private ChallengeResp m_sportResp;//竞技
	private LveGoLveDuoResp m_plunderResp;//掠夺
	#endregion

	#region Player
	public UILabel m_zhanLiLabel;
	public UISprite m_skill;
	public GameObject m_lock;
	public GameObject m_add;
	public GameObject m_parent;
	private GameObject m_heroObj;
	private List<GameObject> m_bingItemList = new List<GameObject> ();

	public UILabel m_getJiFen;
	public UILabel m_jiFen;
	public UILabel m_plunderCount;

	private int m_zhanLiNum;
	private int m_skillId;
	private List<YongBingInfo> m_yongBingList = new List<YongBingInfo> ();

	#endregion

	#region Enemy
	public UILabel e_zhanLiLabel;
	public UISprite e_skill;
	public GameObject e_lock;
	public GameObject e_parent;
	private GameObject e_heroObj;
	private List<GameObject> e_bingItemList = new List<GameObject> ();

	private int e_zhanLiNum;
	private int e_skillId;

	private int e_roleId;
	private int e_level;

	private List<YongBingInfo> e_yongBingList = new List<YongBingInfo> ();
	#endregion

	public GameObject m_changeSkillBtn;

	public GameObject m_plunderInfoObj;
	private readonly Dictionary<int,string> bingDic = new Dictionary<int, string>()
	{
		{1,"dun"},//盾兵
		{2,"gang"},//枪兵
		{3,"gong"},//弓兵
		{4,"piccar"},//车兵
		{5,"hours"},//骑兵
	};
	private const int m_iconBasicDepth = 15;
	private const int m_maxCount = 4;//佣兵最大个数

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (m_topRightObj);
	}

	public void InItChallengePage (ChallengeType tempType,object tempObjectResp)
	{
		m_plunderInfoObj.SetActive (tempType == ChallengeType.PLUNDER ? true : false);

		switch (tempType)
		{
		case ChallengeType.SPORT:
			m_sportResp = tempObjectResp as ChallengeResp;

			m_zhanLiNum = m_sportResp.myZhanli;
			e_zhanLiNum = m_sportResp.oppoZhanli;

			m_skillId = m_sportResp.myZuheId;
			e_skillId = m_sportResp.oppZuheId;

			e_roleId = m_sportResp.oppoRoleId;
			e_level = m_sportResp.oppoLevel;

			m_yongBingList = CreateYongBingList (tempType,m_sportResp.mySoldiers);
			e_yongBingList = CreateYongBingList (tempType,m_sportResp.oppoSoldiers);

			break;
		case ChallengeType.PLUNDER:
			m_plunderResp = tempObjectResp as LveGoLveDuoResp;

			m_zhanLiNum = m_plunderResp.myZhanli;
			e_zhanLiNum = m_plunderResp.oppoZhanli;
			

			m_getJiFen.text = m_plunderResp.lostGongJin.ToString ();
			m_jiFen.text = m_plunderResp.gongJin.ToString ();
		
			m_plunderCount.text = "剩余掠夺次数：" + (m_plunderResp.all - m_plunderResp.used) + "/" + m_plunderResp.all;
			
			m_skillId = m_plunderResp.myGongjiZuheId;
			e_skillId = m_plunderResp.oppFangShouZuheId;
			
			e_roleId = m_plunderResp.oppoRoleId;
			e_level = m_plunderResp.oppoLevel;
			
			m_yongBingList = CreateYongBingList (tempType,m_plunderResp.mySoldiers);
			e_yongBingList = CreateYongBingList (tempType,m_plunderResp.oppoSoldiers);

			break;
		default:
			break;
		}

		m_zhanLiLabel.text = "我方战力：" + m_zhanLiNum.ToString ();
		e_zhanLiLabel.text = "对手战力：" + e_zhanLiNum.ToString ();

		CreateBattleLineUp ();
		ShowSkill ();
	}

	/// <summary>
	/// Creates the yong bing list.
	/// </summary>
	/// <returns>The yong bing list.</returns>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempList">Temp list.</param>
	private List<YongBingInfo> CreateYongBingList (ChallengeType tempType,object tempList)
	{
		List<List<int>> yongBingIdList = new List<List<int>> ();
		switch (tempType)
		{
		case ChallengeType.SPORT:
			List<GuYongBing> sportSoldierList = tempList as List<GuYongBing>;
			for (int i = 0;i < sportSoldierList.Count;i ++)
			{
				List<int> idList = new List<int>();
				idList.Add (sportSoldierList[i].id);
				idList.Add (0);
				yongBingIdList.Add (idList);
			}
			break;
		case ChallengeType.PLUNDER:
			List<Bing> plunderSoldierList = tempList as List<Bing>;
			for (int i = 0;i < plunderSoldierList.Count;i ++)
			{
				List<int> idList = new List<int>();
				idList.Add (plunderSoldierList[i].id);
				idList.Add (plunderSoldierList[i].hp);
				yongBingIdList.Add (idList);
			}
			break;
		default:
			break;
		}

		List<YongBingInfo> bingInfoList = new List<YongBingInfo> ();
		for (int i = 0;i < yongBingIdList.Count;i ++)
		{
			if (i < m_maxCount)
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(yongBingIdList[i][0]);
				YongBingInfo bingInfo = new YongBingInfo();
				bingInfo.yongBingId = yongBingIdList[i][0];
				bingInfo.yongBingHp = yongBingIdList[i][1];
				bingInfo.profession = yongBingTemp.profession;
				bingInfo.iconId = yongBingTemp.icon;
				bingInfo.level = yongBingTemp.needLv;
				bingInfo.nameId = yongBingTemp.m_name;
				bingInfo.desId = yongBingTemp.description;
				bingInfoList.Add (bingInfo);
			}
		}
		
		return bingInfoList;
	}

	void ShowSkill ()
	{
		m_skill.spriteName = m_skillId > 0 ? MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (m_skillId).icon.ToString () : "";
		
		e_lock.SetActive (e_skillId > 0 ? false : true);
		e_skill.spriteName = e_skillId > 0 ? MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (e_skillId).icon.ToString () : "";
		
		ShowMiBaoSkillBtnEffect (true);
	}

	void ShowMiBaoSkillBtnEffect (bool isActive)
	{
		UISprite sprite = m_changeSkillBtn.GetComponent<UISprite> ();
		UILabel label = m_changeSkillBtn.GetComponentInChildren<UILabel> ();

		if (m_skillId > 0)
		{
			m_add.SetActive (false);
			m_lock.SetActive (false);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,7);
			sprite.color = Color.white;
			label.color = Color.white;
		}
		else
		{
			if (isActive)
			{
				if (QXComData.CanSelectMiBaoSkill ())
				{
					sprite.color = Color.white;
					sprite.color = Color.white;
					m_add.SetActive (true);
					m_lock.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,6);
				}
				else
				{
					m_lock.SetActive (true);
					m_add.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,7);
					sprite.color = Color.black;
					sprite.color = Color.black;
				}
			}
		}
	}
	
	/// <summary>
	/// Creates the battle line up.
	/// </summary>
	/// <param name="tempMList">Temp M list.</param>
	/// <param name="tempEList">Temp E list.</param>
	void CreateBattleLineUp ()
	{
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
		if (iconSamplePrefab == null) 
		{
			iconSamplePrefab = p_object as GameObject;
		}

		m_bingItemList = InItItemList (1,m_heroObj,m_parent,m_yongBingList,m_bingItemList);
		e_bingItemList = InItItemList (2,e_heroObj,e_parent,e_yongBingList,e_bingItemList);
	}

	/// <summary>
	/// Ins it item list.
	/// </summary>
	/// <returns>The it item list.</returns>
	/// <param name="initType">Init type.1-my 2-enemy</param>
	/// <param name="tempHeroObj">Temp hero object.</param>
	/// <param name="tempYongBingList">Temp yong bing list.</param>
	/// <param name="tempItemList">Temp item list.</param>
	private List<GameObject> InItItemList (int initType,GameObject tempHeroObj,GameObject tempParentObj,List<YongBingInfo> tempYongBingList,List<GameObject> tempItemList)
	{
		if (tempHeroObj == null)
		{
			tempHeroObj = Instantiate (iconSamplePrefab) as GameObject;
		}
		
		tempHeroObj.SetActive(true);
		tempHeroObj.transform.parent = tempParentObj.transform;
		tempHeroObj.transform.localPosition = Vector3.zero;
		
		IconSampleManager heroIconSample = tempHeroObj.GetComponent<IconSampleManager>();
		heroIconSample.SetIconType (IconSampleManager.IconType.mainCityAtlas);

		switch (initType)
		{
		case 1:

			heroIconSample.SetIconBasic(m_iconBasicDepth,"PlayerIcon" + CityGlobalData.m_king_model_Id,QXComData.JunZhuInfo ().level.ToString());

			break;
		case 2:

			heroIconSample.SetIconBasic (m_iconBasicDepth,"PlayerIcon" + e_roleId,e_level.ToString());

			break;
		default:
			break;
		}

		tempHeroObj.transform.localScale = Vector3.one * 1.1f;
		
		int createCount = tempYongBingList.Count - tempItemList.Count;
		int exitCount = tempItemList.Count;
		if (createCount > 0)
		{
			for (int i = 0;i < createCount;i ++)
			{
				GameObject soldier = (GameObject)Instantiate (iconSamplePrefab);
				
				soldier.SetActive(true);
				soldier.transform.parent = tempHeroObj.transform;

				switch (initType)
				{
				case 1:

					soldier.transform.localPosition = new Vector3(120 + 110 * (i + exitCount),10,0);

					break;
				case 2:

					soldier.transform.localPosition = new Vector3(-120 - 110 * (i + exitCount),-10,0);

					break;
				default:
					break;
				}
				
				tempItemList.Add (soldier);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (createCount);i ++)
			{
				Destroy (tempItemList[tempItemList.Count - 1]);
				tempItemList.RemoveAt (tempItemList.Count - 1);
			}
		}

		for (int i = 0; i < tempYongBingList.Count; i++)
		{
			IconSampleManager soldierIconSample = tempItemList[i].GetComponent<IconSampleManager>();
			soldierIconSample.SetIconType (IconSampleManager.IconType.pveHeroAtlas);
			
			string popName = NameIdTemplate.getNameIdTemplateByNameId (tempYongBingList[i].nameId).Name + " " + "LV" + tempYongBingList[i].level.ToString();
			var popDesc = DescIdTemplate.getDescIdTemplateByNameId (tempYongBingList[i].desId).description;
			
			string rBottomSpriteName = "";
			soldierIconSample.SetIconBasic (m_iconBasicDepth,tempYongBingList[i].iconId.ToString (),tempYongBingList[i].level.ToString());
			soldierIconSample.SetIconPopText (0,popName,popDesc,1);
			soldierIconSample.SetIconDecoSprite (bingDic[tempYongBingList[i].profession],rBottomSpriteName);
			
			tempItemList[i].transform.localScale = Vector3.one * 0.9f;
		}

		return tempItemList;
	}

	/// <summary>
	/// Refreshs my mi bao skill info.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	public void RefreshMyMiBaoSkillInfo (int tempSkillId)
	{
		m_skillId = tempSkillId;
		ShowSkill ();
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ChangeSkillBtn":
			if(!MiBaoGlobleData.Instance().GetEnterChangeMiBaoSkill_Oder ())
			{
				return;
			}

			int openId = -1;
			switch (m_challengeType)
			{
			case ChallengeType.SPORT:
				
				openId = (int)CityGlobalData.MibaoSkillType.PvpSend;
				UIYindao.m_UIYindao.CloseUI ();
				
				break;
			case ChallengeType.PLUNDER:
				
				openId = (int)CityGlobalData.MibaoSkillType.LueDuo_GongJi;
				
				break;
			default:
				break;
			}
			
			MiBaoGlobleData.Instance().OpenMiBaoSkillUI (openId,m_skillId);
			break;
		case "EnterFight":

			switch (m_challengeType)
			{	
			case ChallengeType.SPORT:

				SportData.Instance.SportChallenge (m_sportResp.oppoId,m_sportResp.oppoRank,SportData.EnterPlace.ENTER_BTN);
				
				break;
				
			case ChallengeType.PLUNDER:
				
				EnterBattleField.EnterBattleLueDuo (m_plunderResp.oppoId);
				PlunderData.Instance.CheckPlunderTimes ();
				
				break;
			default:
				break;
			}

			break;
		case "CloseBtn":

			if (M_ChallengeDelegate != null)
			{
				M_ChallengeDelegate ();
			}

			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

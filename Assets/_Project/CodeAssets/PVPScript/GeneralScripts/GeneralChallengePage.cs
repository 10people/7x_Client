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
	public GameObject m_topLeftObj;

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
		QXComData.LoadTitleObj (m_topLeftObj,"挑战阵容");
	}

	public void InItChallengePage (ChallengeType tempType,object tempObjectResp)
	{
		m_challengeType = tempType;
		m_plunderInfoObj.SetActive (tempType == ChallengeType.PLUNDER ? true : false);

		switch (tempType)
		{
		case ChallengeType.SPORT:
			m_sportResp = tempObjectResp as ChallengeResp;

//			for (int i = 0;i < m_sportResp.mySoldiers.Count;i ++)
//			{
//				Debug.Log ("mySoldiers.id:" + m_sportResp.mySoldiers[i].id);
//			}
//
//			for (int i = 0;i < m_sportResp.oppoSoldiers.Count;i ++)
//			{
//				Debug.Log ("oppoSoldiers.id:" + m_sportResp.oppoSoldiers[i].id);
//			}

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
		if (m_skillId > 0)
		{
			QXComData.ClearEffect (m_skill.gameObject);
			m_add.SetActive (false);
			m_lock.SetActive (false);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,7);
			QXComData.SetBtnState (m_changeSkillBtn,true);
		}
		else
		{
			if (isActive)
			{
				if (QXComData.CanSelectMiBaoSkill ())
				{
					QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_skill.gameObject,620233);
					QXComData.SetBtnState (m_changeSkillBtn,true);
					m_add.SetActive (true);
					m_lock.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,6);
				}
				else
				{
					QXComData.ClearEffect (m_skill.gameObject);
					m_lock.SetActive (true);
					m_add.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,7);
					QXComData.SetBtnState (m_changeSkillBtn,false);
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
			m_bingItemList = InItItemList (1,m_parent,m_yongBingList,m_bingItemList);
			e_bingItemList = InItItemList (2,e_parent,e_yongBingList,e_bingItemList);
		}
	}
	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = Instantiate (p_object) as GameObject;
		iconSamplePrefab.SetActive (false);

		m_bingItemList = InItItemList (1,m_parent,m_yongBingList,m_bingItemList);
		e_bingItemList = InItItemList (2,e_parent,e_yongBingList,e_bingItemList);
	}

	/// <summary>
	/// Ins it item list.
	/// </summary>
	/// <returns>The it item list.</returns>
	/// <param name="initType">Init type.1-my 2-enemy</param>
	/// <param name="tempYongBingList">Temp yong bing list.</param>
	/// <param name="tempItemList">Temp item list.</param>
	private List<GameObject> InItItemList (int initType,GameObject tempParentObj,List<YongBingInfo> tempYongBingList,List<GameObject> tempItemList)
	{
		tempItemList = QXComData.CreateGameObjectList (iconSamplePrefab,tempYongBingList.Count + 1,tempItemList);

		for (int i = 0;i < tempItemList.Count;i ++)
		{
			tempItemList[i].SetActive(true);
			tempItemList[i].transform.parent = tempParentObj.transform;

			if (i == tempItemList.Count - 1)
			{
				tempItemList[i].transform.localPosition = Vector3.zero;
				IconSampleManager heroIconSample = tempItemList[i].GetComponent<IconSampleManager>();
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
				
				tempItemList[i].transform.localScale = Vector3.one * 1.1f;
			}
			else
			{
				switch (initType)
				{
				case 1:
					
					tempItemList[i].transform.localPosition = new Vector3(120 + 110 * i,10,0);
					
					break;
				case 2:
					
					tempItemList[i].transform.localPosition = new Vector3(-120 - 110 * i,-10,0);
					
					break;
				default:
					break;
				}

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
		case "MibaoSkill":
			ChangeSkill ();
			break;
		case "ChangeSkillBtn":
			ChangeSkill ();
			break;
		case "EnterFight":

			switch (m_challengeType)
			{	
			case ChallengeType.SPORT:

				SportData.Instance.SportChallenge (m_sportResp.oppoId,m_sportResp.oppoRank,SportData.EnterPlace.ENTER_BTN);
				
				break;
				
			case ChallengeType.PLUNDER:
				Debug.Log ("EnterPlunder");
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

	void ChangeSkill ()
	{
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
		
		MiBaoGlobleData.Instance().OpenMiBaoSkillUI (openId,m_skillId,RefreshSillBtnState);
	}

	void RefreshSillBtnState (int id,bool isActive)
	{
		Debug.Log ("id:" + id);
		m_skillId = id;
		ShowSkill ();
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

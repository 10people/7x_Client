using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralChallengePage : MonoBehaviour {

	public static GeneralChallengePage gcPage;

	#region ProtoMessage
	private ChallengeResp pvpResp;//百战
	private LveGoLveDuoResp plunderResp;//掠夺
	#endregion

	#region MyInfo
	public UILabel mZhanLiLabel;
	public UISprite mSkillIcon;
	public GameObject mLockObj;
	public GameObject addObj;
	private int mSkillId;
	#endregion
	
	#region EnemyInfo
	public UILabel eZhanLiLabel;
	public UISprite eSkillIcon;
	public GameObject eLockObj;
	private int eSkillId;

	private int eRoleId;
	private int eLevel;
	#endregion

	#region PlunderInfo
	public GameObject plunderInfoObj;
	public UILabel getGongJinNum;
	public UILabel mGongJinNum;
	public UILabel lueDuoTime;
	#endregion

	public GameObject mParent;
	public GameObject eParent;

	private GameObject mHeroObj;
	private GameObject eHeroObj;

	private List<GameObject> mItemList = new List<GameObject> ();
	private List<GameObject> eItemList = new List<GameObject> ();

	private List<YongBingInfo> mYongBingList = new List<YongBingInfo> ();
	private List<YongBingInfo> eYongBingList = new List<YongBingInfo> ();
	
	private GameObject iconSamplePrefab;
	
	private readonly Dictionary<int,string> armsDic = new Dictionary<int, string>()
	{
		{1,"dun"},//盾兵
		{2,"gang"},//枪兵
		{3,"gong"},//弓兵
		{4,"piccar"},//车兵
		{5,"hours"},//骑兵
	};
	private const int iconBasicDepth = 15;
	private const int maxCount = 4;//佣兵最大个数

	public List<EventHandler> challengehandlerList = new List<EventHandler> ();

	private GeneralControl.ChallengeType challengeType;

	public ScaleEffectController sEffectController;

	public GameObject topRightObj;

	void Awake ()
	{
		gcPage = this;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (topRightObj);
	}

	/// <summary>
	/// PVP
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempResp">Temp resp.</param>
	public void InItPvpChallengePage (GeneralControl.ChallengeType tempType,ChallengeResp tempResp)
	{
		sEffectController.OnOpenWindowClick ();

		challengeType = tempType;
		pvpResp = tempResp;

		PvpReset = false;

		plunderInfoObj.SetActive (false);

		mZhanLiLabel.text = "我方战力：" + tempResp.myZhanli.ToString ();
		eZhanLiLabel.text = "对手战力：" + tempResp.oppoZhanli.ToString ();

		mSkillId = tempResp.myZuheId;
		eSkillId = tempResp.oppZuheId;
		InItMiBaoInfo ();

		eRoleId = pvpResp.oppoRoleId;
		eLevel = pvpResp.oppoLevel;

		mYongBingList = CreatePvpYongBingList (tempResp.mySoldiers);
		eYongBingList = CreatePvpYongBingList (tempResp.oppoSoldiers);

		CreateBattleLineUp ();

		foreach (EventHandler handler in challengehandlerList)
		{
			handler.m_click_handler -= ChallengeHandlerClickBack;
			handler.m_click_handler += ChallengeHandlerClickBack;
		}

//		Debug.Log ("QXComData.CheckYinDaoOpenState (100255):" + QXComData.CheckYinDaoOpenState (100255));
		if (!QXComData.CheckYinDaoOpenState (100200))
		{
			UIYindao.m_UIYindao.CloseUI ();
		}

		Global.m_isOpenBaiZhan = false;
	}

	/// <summary>
	/// 掠夺
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempResp">Temp resp.</param>
	public void InItPlunderChallengePage (GeneralControl.ChallengeType tempType,LveGoLveDuoResp tempResp)
	{
		sEffectController.transform.localScale = Vector3.one;

		challengeType = tempType;
		plunderResp = tempResp;

		mZhanLiLabel.text = "我方战力：" + tempResp.myZhanli.ToString ();
		eZhanLiLabel.text = "对手战力：" + tempResp.oppoZhanli.ToString ();

		plunderInfoObj.SetActive (true);
		getGongJinNum.text = tempResp.lostGongJin.ToString ();
		mGongJinNum.text = tempResp.gongJin.ToString ();
//		Debug.Log ("tempResp.all:" + tempResp.all + "|||||tempResp.all:" + tempResp.used);
		lueDuoTime.text = "剩余掠夺次数：" + (tempResp.all - tempResp.used) + "/" + tempResp.all;

		mSkillId = tempResp.myGongjiZuheId;
		eSkillId = tempResp.oppFangShouZuheId;
		InItMiBaoInfo ();

		eRoleId = plunderResp.oppoRoleId;
		eLevel = plunderResp.oppoLevel;

		mYongBingList = CreatePlunderYongBingList (tempResp.mySoldiers);
		eYongBingList = CreatePlunderYongBingList (tempResp.oppoSoldiers);

		CreateBattleLineUp ();

		foreach (EventHandler handler in challengehandlerList)
		{
			handler.m_click_handler -= ChallengeHandlerClickBack;
			handler.m_click_handler += ChallengeHandlerClickBack;
		}
	}

	/// <summary>
	/// Ins it mi bao info.
	/// </summary>
	void InItMiBaoInfo ()
	{
//		Debug.Log ("mSkillId:" + mSkillId + "||eSkillId:" + eSkillId);
//		mLockObj.SetActive (mSkillId > 0 ? false : true);
		mSkillIcon.spriteName = mSkillId > 0 ? MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (mSkillId).icon.ToString () : "";

		eLockObj.SetActive (eSkillId > 0 ? false : true);
		eSkillIcon.spriteName = eSkillId > 0 ? MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (eSkillId).icon.ToString () : "";

		ShowMiBaoSkillBtnEffect (true);
	}

	void ShowMiBaoSkillBtnEffect (bool isActive)
	{
		UISprite sprite = challengehandlerList [0].GetComponent<UISprite> ();
		UILabel label = challengehandlerList [0].GetComponentInChildren<UILabel> ();

		if (mSkillId > 0)
		{
			addObj.SetActive (false);
			mLockObj.SetActive (false);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);
//			UI3DEffectTool.ClearUIFx (challengehandlerList[0].gameObject);
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
					addObj.SetActive (true);
					mLockObj.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,4);
				}
				else
				{
					mLockObj.SetActive (true);
					addObj.SetActive (false);
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);
					sprite.color = Color.black;
					sprite.color = Color.black;
//					UI3DEffectTool.ClearUIFx (challengehandlerList[0].gameObject);
				}
			}
		}
	}

	/// <summary>
	/// Creates the yong bing list.
	/// </summary>
	/// <returns>The yong bing list.</returns>
	/// <param name="tempList">Temp list.</param>
	private List<YongBingInfo> CreatePvpYongBingList (List<GuYongBing> tempList)
	{
		List<YongBingInfo> bingInfoList = new List<YongBingInfo> ();
		
		for (int i = 0;i < tempList.Count;i ++)
		{
			if (i < maxCount)
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(tempList[i].id);
				YongBingInfo bingInfo = new YongBingInfo();
				bingInfo.yongBingId = tempList[i].id;
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

	/// <summary>
	/// Creates the yong bing list.
	/// </summary>
	/// <returns>The yong bing list.</returns>
	/// <param name="tempList">Temp list.</param>
	private List<YongBingInfo> CreatePlunderYongBingList (List<Bing> tempList)
	{
		List<YongBingInfo> bingInfoList = new List<YongBingInfo> ();
		
		for (int i = 0;i < tempList.Count;i ++)
		{
			if (i < maxCount)
			{
				GuYongBingTempTemplate yongBingTemp = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(tempList[i].id);
				YongBingInfo bingInfo = new YongBingInfo();
				bingInfo.yongBingId = tempList[i].id;
				bingInfo.yongBingHp = tempList[i].hp;
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

		mItemList = InItItemList (1,mHeroObj,mParent,mYongBingList,mItemList);
		eItemList = InItItemList (2,eHeroObj,eParent,eYongBingList,eItemList);
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

			heroIconSample.SetIconBasic(iconBasicDepth,"PlayerIcon" + CityGlobalData.m_king_model_Id,JunZhuData.Instance().m_junzhuInfo.level.ToString());

			break;
		case 2:

			heroIconSample.SetIconBasic (iconBasicDepth,"PlayerIcon" + eRoleId,eLevel.ToString());

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
			soldierIconSample.SetIconBasic (iconBasicDepth,tempYongBingList[i].iconId.ToString (),tempYongBingList[i].level.ToString());
			soldierIconSample.SetIconPopText (0,popName,popDesc,1);
			soldierIconSample.SetIconDecoSprite (armsDic[tempYongBingList[i].profession],rBottomSpriteName);
			
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
		if (!QXComData.CheckYinDaoOpenState (100200))
		{
			UIYindao.m_UIYindao.CloseUI ();
		}
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);
		mSkillId = tempSkillId;
		InItMiBaoInfo ();
	}

	void ChallengeHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "ChangeSkillBtn":

			if(!MiBaoGlobleData.Instance().GetEnterChangeMiBaoSkill_Oder ())
			{
//				Debug.Log ("return");
				return;
			}
//			Debug.Log ("mSkillId:" + mSkillId);

			int openId = -1;

			switch (challengeType)
			{
			case GeneralControl.ChallengeType.PVP:

				openId = (int)CityGlobalData.MibaoSkillType.PvpSend;
				UIYindao.m_UIYindao.CloseUI ();

				break;
			case GeneralControl.ChallengeType.PLUNDER:

				openId = (int)CityGlobalData.MibaoSkillType.LueDuo_GongJi;

				break;
			default:
				break;
			}

			MiBaoGlobleData.Instance().OpenMiBaoSkillUI (openId,mSkillId);

			break;
		case "EnterFight":

			switch (challengeType)
			{	
			case GeneralControl.ChallengeType.PVP:

				PvpData.Instance.PvpChallengeResp = pvpResp;
				PvpData.Instance.PlayerStateCheck (PvpData.PlayerState.STATE_GENERAL_TIAOZHAN_PAGE);
				
				break;
				
			case GeneralControl.ChallengeType.PLUNDER:
				
				EnterBattleField.EnterBattleLueDuo (plunderResp.oppoId);
				PlunderData.Instance.CheckPlunderTimes ();

				break;
			default:
				break;
			}

			break;
		case "CloseBtn":

			switch (challengeType)
			{	
			case GeneralControl.ChallengeType.PVP:

				sEffectController.OnCloseWindowClick ();
				sEffectController.CloseCompleteDelegate += DisActiveObj;
				
				break;
				
			case GeneralControl.ChallengeType.PLUNDER:
				
				gameObject.SetActive (false);
				
				break;
			default:
				break;
			}

			break;
		default:
			break;
		}
	}

	private bool pvpReset = false;//是否重置百战
	public bool PvpReset
	{
		set{pvpReset = value;}
		get{return pvpReset;}
	}
	public void DisActiveObj ()
	{
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	
		if (PvpReset)
		{
			PvpData.Instance.PvpDataReq ();
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportPage : GeneralInstance<SportPage> {

	public delegate void SportPageDelegate ();
	private SportPageDelegate m_sportPageDelegate;

	private List<RewardData> m_rewardList = new List<RewardData> ();

	[HideInInspector]
	public BaiZhanInfoResp SportResp;
	public BaiZhanTemplate SportTemp;

	private string m_textStr;

	private bool m_isShopYinDao = false;

	public GameObject m_anchorTR;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (m_anchorTR);
	}

	public void InItSportPage (BaiZhanInfoResp tempResp,SportPageDelegate tempDelegate)
	{
		//yindao
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);

		SportResp = tempResp;
		m_sportPageDelegate = tempDelegate;
		m_pageObj.transform.localScale = Vector3.one;//0,96f
		m_rewardList.Clear ();

		if (SportResp.rankAward > 0)
		{
			m_rewardList.Add (new RewardData(900002,SportResp.rankAward));
			SportData.Instance.SportOperateReq (SportData.SportOperate.GET_RANK_REWARD);
		}
		else
		{
			Global.m_isSportDataInItEnd = true;
			m_isShopYinDao = true;
		}

		InItPlayerInfo ();
		InItJunXianRoomInfo ();
	}

	#region EnemyJunXianRoomInfo

	public GameObject m_sportJunXianObj;
	private List<GameObject> m_junXianRoomList = new List<GameObject>();

	public readonly Dictionary<int,string> M_RoomBoxDic = new Dictionary<int, string>()
	{
		{1,"200:175"},{2,"200:180"},{3,"210:180"},{4,"210:190"},{5,"220:190"},{6,"240:240"},{7,"400:280"},{8,"460:330"},{9,"495:345"},
	};

	private readonly Dictionary<int,Vector3> m_texPosDic = new Dictionary<int, Vector3>()
	{
		{101,new Vector3 (395,230,0)},//小卒
		{204,new Vector3 (480,-30,0)},{203,new Vector3 (450,120,0)},{202,new Vector3 (205,220,0)},{201,new Vector3 (35,270,0)},//步兵
		{304,new Vector3 (455,-30,0)},{303,new Vector3 (315,40,0)},{302,new Vector3 (25,175,0)},{301,new Vector3 (-135,275,0)},//骑士
		{404,new Vector3 (345,-130,0)},{403,new Vector3 (180,-30,0)},{402,new Vector3 (-35,130,0)},{401,new Vector3 (-230,210,0)},//禁卫
		{504,new Vector3 (180,-200,0)},{503,new Vector3 (100,-60,0)},{502,new Vector3 (-170,65,0)},{501,new Vector3 (-330,170,0)},//校尉
		{604,new Vector3 (395,230,0)},{603,new Vector3 (0,-110,0)},{602,new Vector3 (-220,15,0)},{601,new Vector3 (-360,80,0)},//先锋
		{703,new Vector3 (-15,-300,0)},{702,new Vector3 (-290,-230,0)},{701,new Vector3 (-415,-45,0)},//少将
		{802,new Vector3 (-225,-275,0)},{801,new Vector3 (-430,-185,0)},//元帅
		{901,new Vector3 (-465,-305,0)},//诸侯
	};

	private void InItJunXianRoomInfo ()
	{
		List<BaiZhanTemplate> sportTemplateList = BaiZhanTemplate.templates;
		m_junXianRoomList = QXComData.CreateGameObjectList (m_sportJunXianObj,sportTemplateList.Count,m_junXianRoomList);
		for (int i = 0;i < m_junXianRoomList.Count;i ++)
		{
			string[] posLen = sportTemplateList[i].fangWuZuoBiao.Split (',');
			m_junXianRoomList[i].transform.localPosition = new Vector3(float.Parse (posLen[0]),float.Parse (posLen[1]),0);
			SportJunXian sportJunXian = m_junXianRoomList[i].GetComponent<SportJunXian> ();
			sportJunXian.m_sportTemp = sportTemplateList[i];
			sportJunXian.InItJunXianRoom ();
		}

		MoveToMyHeadPos ();
	}

	#endregion

	#region PlayerInfo

	public UILabel m_playerName;
	public UILabel m_zhanLi;
	public UILabel m_junXian;
	public UILabel m_chanChu;
	public UILabel m_shouYi;
	public UILabel m_countTime;
	public UILabel m_sportCd;
	public UILabel m_getRewardCd;

	public GameObject m_addBtnObj;
	public GameObject m_clearCdBtnObj;

	public void InItPlayerInfo ()
	{
		SportTemp = BaiZhanTemplate.GetBaiZhanTempByRank (SportResp.rank);
//		Debug.Log ("SportResp.time:" + SportResp.time);
		m_playerName.text = MyColorData.getColorString (1,QXComData.JunZhuInfo ().name);
		m_zhanLi.text = AddColor("战力：" + QXComData.JunZhuInfo ().zhanLi);
		m_junXian.text = AddColor("军衔：" + QXComData.GetJunXianName (SportTemp.jibie));
		m_chanChu.text = AddColor("产出：" + SportResp.weiWangHour + "威望/小时");
		m_shouYi.text = AddColor("威望：" + SportResp.hasWeiWang + "威望");

		int curVipMaxTime = VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv).bugBaizhanTime + 5;
		string countTimeStr = "";
		if (SportResp.totalTimes == curVipMaxTime)
		{
			if (SportResp.leftTimes == 0)
			{
				countTimeStr = "今日次数用尽";
			}
			else
			{
				countTimeStr = "剩余次数：" + SportResp.leftTimes + "/" + SportResp.totalTimes;
			}
		}
		else
		{
			countTimeStr = "剩余次数：" + SportResp.leftTimes + "/" + SportResp.totalTimes;
		}
		m_countTime.text = AddColor(countTimeStr);

		m_addBtnObj.SetActive (SportResp.leftTimes > 0 ? false : SportResp.totalTimes == curVipMaxTime ? false : true);

		m_clearCdBtnObj.SetActive (SportResp.time > 0 ? true : false);

		StopCoroutine ("SportCd");
		if (SportResp.time > 0)
		{
			StartCoroutine ("SportCd");
		}
		else
		{
			m_sportCd.text = "";
		}

		StopCoroutine ("GetReward");
		if (SportResp.nextTimeTo21 <= 0)
		{
			m_getRewardCd.text = "";
		}
		else
		{
			StartCoroutine ("GetReward");
		}
	}

	private string AddColor (string tempStr)
	{
		return "[e15a00]" + tempStr + "[-]";
	}

	IEnumerator SportCd ()
	{
		while (SportResp.time > 0)
		{
			SportResp.time --;
			m_sportCd.text = AddColor("冷却时间：" + TimeHelper.GetUniformedTimeString (SportResp.time));
			yield return new WaitForSeconds (1);

			if (SportResp.time <= 0)
			{
				SportResp.time = 0;
				m_sportCd.text = "";

				StopCoroutine ("SportCd");
			}
		}
	}

	IEnumerator GetReward ()
	{
		while (SportResp.nextTimeTo21 > 0)
		{
			SportResp.nextTimeTo21 --;

			yield return new WaitForSeconds (1);

			if (SportResp.nextTimeTo21 <= 0)
			{
				m_getRewardCd.text = "";
				SportResp.nextTimeTo21 = 0;
			}
			else
			{
				m_getRewardCd.text = QXComData.TimeFormat (SportResp.nextTimeTo21);
			}
		}
	}

	/// <summary>
	/// Refreshs the player info.
	/// </summary>
	/// <param name="tempRank">Temp rank.</param>
	public void RefreshPlayerInfo (int tempRank)
	{
		SportResp.rank = tempRank;
		SportTemp = BaiZhanTemplate.GetBaiZhanTempByRank (SportResp.rank);
		m_junXian.text = AddColor("军衔：" + SportTemp.jibie);
		m_chanChu.text = AddColor("产出：" + SportTemp.produceSpeed);

		//更新头像位置
		MoveToMyHeadPos ();
	}

	/// <summary>
	/// Refreshs the player info.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void RefreshPlayerInfo (BuyCiShuInfo tempInfo)
	{
		SportResp.leftTimes = tempInfo.leftTimes;
		SportResp.totalTimes = tempInfo.totalTimes;

		m_addBtnObj.SetActive (false);
		m_countTime.text = AddColor("剩余次数：" + SportResp.leftTimes + "/" + SportResp.totalTimes);
	}

	/// <summary>
	/// Refreshs the player info.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void RefreshPlayerInfo (CleanCDInfo tempInfo)
	{
		SportResp.cdYuanBao = tempInfo.nextCDYB;
		SportResp.time = 0;
		StopCoroutine ("SportCd");
		m_clearCdBtnObj.SetActive (false);
		m_sportCd.text = "";
	}

	/// <summary>
	/// Refreshs the sport enemy list.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void RefreshSportEnemyList (PayChangeInfo tempInfo)
	{
		SportResp.huanYiPiYB = tempInfo.nextHuanYiPiYB;

		RefreshOtherInfo otherInfo = new RefreshOtherInfo ();
		otherInfo.oppoList = tempInfo.oppoList;
		OpenJunXianRoom (otherInfo);
	}

	#endregion

	#region FindOwnJunXianPosition
	
	public GameObject m_ownHeadObj;
	public UISprite m_headIcon;
	private Vector3 m_pos;		//定位
	private Vector3 m_targetPos;
	private iTween.EaseType m_targetType;
	private float m_moveTime;

	public UIRoot m_root;
	public Camera m_camera;
	public UIAnchor m_centerAnchor;
	public GameObject m_pageObj;

	enum PagePosType
	{
		L_B,
		R_B,
		R_T,
		L_T,
	}
	private PagePosType m_page_posType;

	void MoveToMyHeadPos ()
	{
		m_headIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
		foreach (GameObject obj in m_junXianRoomList)
		{
			SportJunXian sportJunXian = obj.GetComponent<SportJunXian> ();
			if (SportTemp.id == sportJunXian.m_sportTemp.id)
			{
				m_ownHeadObj.transform.parent = obj.transform.parent;
				m_ownHeadObj.transform.localPosition = obj.transform.localPosition + new Vector3(0,375,0);
				m_pos = obj.transform.localPosition;

				m_targetPos = obj.transform.localPosition + new Vector3 (0, 75, 0);
				m_targetType = iTween.EaseType.easeOutQuart;
				m_moveTime = 1;
				break;
			}
		}
//		m_pos = m_junXianRoomList [m_junXianRoomList.Count - 1].transform.localPosition;

		UITexture pageTex = m_pageObj.GetComponent<UITexture> ();

		bool left = m_pos.x < 0 ? true : false;
		float disX = (float)(pageTex.width / 2) - Mathf.Abs (m_pos.x);
		bool top = m_pos.y > 0 ? true : false;
		float disY = (float)(pageTex.width / 2) - Mathf.Abs (m_pos.y);
		Debug.Log ("disX:" + disX);
		Debug.Log ("disY:" + disY);
		Debug.Log ("screen.width:" + Screen.width);
		Debug.Log ("screen.height:" + Screen.height);

		Debug.Log ("m_centerAnchortion:" + m_centerAnchor.transform.localPosition);
		Debug.Log ("m_root:" + m_root.transform.localPosition);

		Vector3 worldPos = m_pos + m_texPosDic [SportTemp.id] + m_centerAnchor.transform.localPosition + m_root.transform.localPosition;
		Debug.Log ("worldPos:" + worldPos);
		Vector3 screenPos = m_camera.WorldToViewportPoint (worldPos);
		Debug.Log ("screenPos:" + screenPos);
		if (left && top)
		{
			m_page_posType = PagePosType.L_T;
		}
		else if (left && !top)
		{
			m_page_posType = PagePosType.L_B;
		}
		else if (!left && top)
		{
			m_page_posType = PagePosType.R_T;
		}
		else
		{
			m_page_posType = PagePosType.R_B;
		}

		if (m_pageObj.transform.localPosition != m_texPosDic[SportTemp.id])
		{
			PageMove (m_texPosDic[SportTemp.id]);
		}

		if (disX < Screen.width / 2 - Math.Abs ((m_pos + m_texPosDic[SportTemp.id]).x))
		{
			
		}
		switch (m_page_posType)
		{
		case PagePosType.L_B:



			break;
		case PagePosType.L_T:
			break;
		case PagePosType.R_B:
			break;
		case PagePosType.R_T:
			break;
		default:
			break;
		}

		PageScale ();
		HeadItween ();
	}

	void PageMove (Vector3 pos)
	{
		Hashtable move = new Hashtable ();
		move.Add ("time",m_moveTime);
		move.Add ("easetype",iTween.EaseType.easeInOutQuad);
		move.Add ("position",pos);
		move.Add ("islocal",true);
		iTween.MoveTo (m_pageObj,move);
	}

	void PageScale ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",1);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("scale",Vector3.one);
		scale.Add ("islocal",true);
//		scale.Add ("oncomplete","HeadItween");
//		scale.Add ("oncompletetarget",gameObject);
		iTween.ScaleTo (m_pageObj,scale);
	}

	void HeadItween ()
	{
		Hashtable move = new Hashtable ();
		move.Add ("time",m_moveTime);
		move.Add ("easetype",m_targetType);
		move.Add ("position",m_targetPos);
		move.Add ("islocal",true);
		move.Add ("oncomplete","HeadItweenBack");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (m_ownHeadObj,move);
	}

	void HeadItweenBack ()
	{
		m_moveTime = 1.5f;
		m_targetType = iTween.EaseType.easeInOutQuad;
		Hashtable move = new Hashtable ();
		move.Add ("time",m_moveTime);
		move.Add ("easetype",iTween.EaseType.easeInOutQuad);
		move.Add ("position",m_targetPos + new Vector3(0,25,0));
		move.Add ("islocal",true);
		move.Add ("oncomplete","HeadItween");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (m_ownHeadObj,move);
	}

	#endregion

	#region JunXianRoomInfo

	public GameObject m_junXianRoomObj;

	public void OpenJunXianRoom (RefreshOtherInfo tempInfo)
	{
		SetJunXianRoomObj (true);
		SportEnemyPage.m_instance.InItSportEnemyPage (tempInfo,SportEnemyPageClickCallBack);
	}

	void SportEnemyPageClickCallBack ()
	{
		SetJunXianRoomObj (false);
	}

	public void SetJunXianRoomObj (bool tempActive)
	{
		m_junXianRoomObj.SetActive (tempActive);
	}

	#endregion

	#region SportEnemyInfo

	public GameObject m_sportEnemyObj;
	private OpponentInfo m_sportEnemyInfo;

	public void OpenSportEnemy (OpponentInfo tempInfo)
	{
		m_sportEnemyInfo = tempInfo;
		SetSportEnemyObj (true);
		SportEnemyInfo.m_instance.InItSportEnemyInfo (tempInfo,SportEnemyInfoClickCallBack);
	}

	public void SportEnemyInfoClickCallBack (int i)
	{
		switch (i)
		{
		case 1:
			if (m_sportEnemyInfo.junZhuId == QXComData.JunZhuInfo ().id)
			{
				//不能打自己
				m_textStr = "不能挑战自己！";
				QXComData.CreateBoxDiy (m_textStr,true,null);
				return;
			}
			SportData.Instance.SportChallenge (m_sportEnemyInfo.junZhuId,m_sportEnemyInfo.rank,SportData.EnterPlace.PLAYER);
			break;
		case 2:
			SetJunXianRoomObj (true);
			SetSportEnemyObj (false);
			break;
		case 3:
			SetJunXianRoomObj (true);
			SetSportEnemyObj (false);
			break;
		default:
			break;
		}
	}

	public void SetSportEnemyObj (bool tempActive)
	{
		m_sportEnemyObj.SetActive (tempActive);
	}

	#endregion

	#region HighRankReward
	public GameObject m_getHighRankRewardObj;

	//打开所领取的排名奖励
	public void GetHighRankReward ()
	{
		m_getHighRankRewardObj.SetActive (true);

		SportHighRank.m_instance.InItHighRank (m_rewardList);
		SportHighRank.m_instance.M_SportHighRankDelegate = GetHighRankRewardBack;
	}

	void GetHighRankRewardBack ()
	{
		m_getHighRankRewardObj.SetActive (false);
//		Global.m_isOpenBaiZhan = false;
		Global.m_isSportDataInItEnd = true;
		m_isShopYinDao = true;
	}

	//打开剩余可领元宝窗口
	void OpenHighRankReward ()
	{
		m_dailyRankRewardObj.SetActive (true);
		SportResult.m_instance.InItRankReward ();
		SportResult.m_instance.M_SportResultDelegate = SportResultDelegateCallBack;
	}

	void HighRankDelegateCallBack ()
	{
		m_dailyRankRewardObj.SetActive (false);
	}

	#endregion

	#region DailyRankReward
	[HideInInspector]public List<RewardData> m_dailyRewardList = new List<RewardData>();

	public GameObject m_dailyRankRewardObj;

	public void GetDailyRankReward ()
	{
		SportResp.nextTimeTo21 = -1;
		
		GeneralRewardManager.Instance().CreateReward (m_dailyRewardList);
	}

	private void OpenDailyRankReward ()
	{
		m_dailyRankRewardObj.SetActive (true);
		SportResult.m_instance.InItSportResult (SportResp.rank);
		SportResult.m_instance.M_SportResultDelegate = SportResultDelegateCallBack;
	}

	void SportResultDelegateCallBack ()
	{
		m_dailyRankRewardObj.SetActive (false);
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ShopBtn":

			ShopData.Instance.OpenShop (ShopData.ShopType.WEIWANG);

			break;
		case "RecordBtn":

			SportData.Instance.SportRecordReq ();

			break;
		case "RankBtn":

			Rank.RootController.CreateRankWindow (2);

			break;
		case "DailyRewardBtn":

			if (SportResp.nextTimeTo21 == 0)
			{
				BaiZhanTemplate baizhanTemp = BaiZhanTemplate.GetBaiZhanTempByRank (SportResp.rank);
				string[] rewardLength = baizhanTemp.dayAward.Split ('#');

				m_dailyRewardList.Clear ();

				for (int i = 0;i < rewardLength.Length;i ++)
				{
					string[] rewardLen = rewardLength[i].Split (':');
					RewardData data = new RewardData(int.Parse (rewardLen[1]),int.Parse (rewardLen[2]));
					m_dailyRewardList.Add (data);
				}
				SportData.Instance.SportOperateReq (SportData.SportOperate.GET_DAILY_REWARD);
			}
			else
			{
				OpenDailyRankReward ();
			}

			break;
		case "HighRewardBtn":

			OpenHighRankReward ();

			break;
		case "RulesBtn":

			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_HELP_DESC));

			break;
		case "CloseBtn":

			m_sportPageDelegate ();

			break;
		case "AddBtn":

			if (SportResp.leftCanBuyCount == 0)
			{
				if (QXComData.JunZhuInfo ().vipLv < QXComData.maxVipLevel)
				{
					string textDes1 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_7);
					string textDes2 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_8).Replace ('*',char.Parse ((JunZhuData.Instance().m_junzhuInfo.vipLv + 1).ToString ()));
					string textDes3 = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
					//					textStr = "今日剩余购买次数已用完,V特权等级提升到" + (JunZhuData.Instance().m_junzhuInfo.vipLv + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
					m_textStr = textDes1 + "\n" + textDes2 + "\n" + textDes3;
					QXComData.CreateBoxDiy (m_textStr,true,null);
				}
				else
				{
					m_textStr = "今日已无剩余购买次数...";
					QXComData.CreateBoxDiy (m_textStr,true,null);
				}
			}
			else
			{
				m_textStr = "是否使用" + SportResp.buyNeedYB + "元宝购买" + SportResp.buyNumber + "次挑战次数？\n\n今日还可购买" + SportResp.leftCanBuyCount + "回";
				QXComData.CreateBoxDiy (m_textStr,false,SportData.Instance.BuyTimes);
			}

			break;
		case "ClearCdBtn":

			SportData.Instance.M_ClearByBtn = true;
			m_textStr = "是否使用" + SportResp.cdYuanBao + "元宝清除挑战冷却？";
			QXComData.CreateBoxDiy (m_textStr,false,SportData.Instance.ClearCd);

			break;
		default:
			SportJunXian junXian = ui.transform.parent.GetComponent<SportJunXian> ();
			if (junXian != null)
			{
				SportData.Instance.SportOperateReq (SportData.SportOperate.SPORT_ENEMY_PAGE,junXian.m_sportTemp.id);
			}
			break;
		}
	}

	void Update ()
	{
		if (m_isShopYinDao && QXComData.CheckYinDaoOpenState (100220))
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100220,2);
			m_isShopYinDao = false;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

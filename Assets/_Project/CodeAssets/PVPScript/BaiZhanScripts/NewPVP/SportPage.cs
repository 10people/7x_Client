#define SetMapPos
#define CheckBug
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

	public GameObject m_recordRed;

	public GameObject m_dailyRewardBtn;
	public GameObject m_rankRewardBtn;

	private string m_textStr;

	private bool m_isShopYinDao = false;
//	private bool m_moveFinished = false;

	public GameObject m_anchorTR;
	public GameObject m_anchorTL;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (m_anchorTR);
		QXComData.LoadTitleObj (m_anchorTL,"百战千军");
	}

	public void InItSportPage (BaiZhanInfoResp tempResp,SportPageDelegate tempDelegate)
	{
//		m_moveFinished = false;
		//yindao
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);

		UIDragObject dragObj = m_pageObj.GetComponent<UIDragObject> ();
		dragObj.enabled = !QXComData.CheckYinDaoOpenState (100200);

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
            TaskData.Instance.IsCanShowComplete = true;

            Global.m_isSportDataInItEnd = true;
			m_isShopYinDao = true;
		}

		m_recordRed.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (300105));

		InItPlayerInfo ();
	}

	#region EnemyJunXianRoomInfo

	public GameObject m_sportJunXianObj;
	private List<GameObject> m_junXianRoomList = new List<GameObject>();

	public readonly Dictionary<int,string> M_RoomBoxDic = new Dictionary<int, string>()
	{
		{1,"120:100"},{2,"130:110"},{3,"180:130"},{4,"180:130"},{5,"200:150"},{6,"200:160"},{7,"270:170"},{8,"300:240"},{9,"400:280"},
	};

	private readonly Dictionary<int,Vector3> m_texPosDic = new Dictionary<int, Vector3>()
	{
		{101,new Vector3 (450,230,0)},//小卒
		{204,new Vector3 (480,-30,0)},{203,new Vector3 (450,120,0)},{202,new Vector3 (205,220,0)},{201,new Vector3 (35,270,0)},//步兵
		{304,new Vector3 (455,-30,0)},{303,new Vector3 (315,40,0)},{302,new Vector3 (25,175,0)},{301,new Vector3 (-135,275,0)},//骑士
		{404,new Vector3 (345,-130,0)},{403,new Vector3 (180,-30,0)},{402,new Vector3 (-35,130,0)},{401,new Vector3 (-230,210,0)},//禁卫
		{504,new Vector3 (180,-200,0)},{503,new Vector3 (100,-60,0)},{502,new Vector3 (-170,65,0)},{501,new Vector3 (-330,170,0)},//校尉
		{604,new Vector3 (240,-170,0)},{603,new Vector3 (0,-110,0)},{602,new Vector3 (-220,15,0)},{601,new Vector3 (-360,80,0)},//先锋
		{703,new Vector3 (-15,-300,0)},{702,new Vector3 (-290,-230,0)},{701,new Vector3 (-415,-45,0)},//少将
		{802,new Vector3 (-225,-275,0)},{801,new Vector3 (-430,-185,0)},//元帅
		{901,new Vector3 (-410,-305,0)},//诸侯
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

	public UILabel m_vipAdd;
	public UISprite m_vipIcon;

	public GameObject m_addBtnObj;
	public GameObject m_clearCdBtnObj;

	public void InItPlayerInfo ()
	{
		SportTemp = BaiZhanTemplate.GetBaiZhanTempByRank (SportResp.rank);
//		Debug.Log ("SportResp.time:" + SportResp.time);
		m_playerName.text = MyColorData.getColorString (101,QXComData.JunZhuInfo ().name);
		m_zhanLi.text = AddColor("战力：" + QXComData.JunZhuInfo ().zhanLi);
		m_junXian.text = AddColor("军衔：" + QXComData.GetJunXianName (SportTemp.jibie));
		m_chanChu.text = AddColor("产出：" + SportResp.weiWangHour + "威望/小时");
		m_shouYi.text = AddColor("威望：" + SportResp.hasWeiWang + "威望");

//		m_vipIcon.spriteName = QXComData.JunZhuInfo ().vipLv >= 6 ? "v" + QXComData.JunZhuInfo ().vipLv : "";
//		m_vipAdd.text = QXComData.JunZhuInfo ().vipLv >= 6 ? "(      +" + (VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv).baizhanPara - 1.0) * 100 + "%)" : "";

		m_vipIcon.spriteName = "v" + 6;
		m_vipAdd.text = "       以上额外产出" + (VipTemplate.GetVipInfoByLevel (6).baizhanPara - 1.0) * 100 + "%";

		int curVipMaxTime = VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv).bugBaizhanTime + 5;
		string countTimeStr = "";

		#if CheckBug
		if (SportResp == null)
		{
			QXComData.CreateBoxDiy ("SportResp is null",true,null);
		}
		else
		{
			if (SportResp.leftTimes == null)
			{
				QXComData.CreateBoxDiy ("LeftTimes is null",true,null);
			}
			else
			{
				if (SportResp.leftCanBuyCount == null)
				{
					QXComData.CreateBoxDiy ("leftCanBuyCount is null",true,null);
				}
				else
				{
					if (SportResp.totalTimes == null)
					{
						QXComData.CreateBoxDiy ("totalTimes is null",true,null);
					}
					else
					{
						if (QXComData.JunZhuInfo () == null)
						{
							QXComData.CreateBoxDiy ("JunZhuInfo is null",true,null);
						}
						else 
						{
							if (QXComData.JunZhuInfo ().vipLv == null)
							{
								QXComData.CreateBoxDiy ("vipLv is null",true,null);
							}
							else if (QXComData.maxVipLevel == null)
							{
								QXComData.CreateBoxDiy ("maxVipLevel is null",true,null);
							}
						}
					}
				}
			}
		}
		#endif

		if (SportResp.leftTimes == 0)
		{
			if (SportResp.leftCanBuyCount == 0)
			{
				countTimeStr = QXComData.JunZhuInfo ().vipLv >= QXComData.maxVipLevel ? "今日次数用尽" : "剩余次数：" + SportResp.leftTimes + "/" + SportResp.totalTimes;
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

//		m_addBtnObj.SetActive (SportResp.leftTimes > 0 ? false : SportResp.totalTimes == curVipMaxTime ? false : true);
		m_addBtnObj.SetActive (SportResp.leftTimes > 0 ? false : (SportResp.leftCanBuyCount > 0 ? true : (QXComData.JunZhuInfo ().vipLv >= QXComData.maxVipLevel ? false : true)));

		m_clearCdBtnObj.SetActive (SportResp.leftTimes > 0 ? (SportResp.time > 0 ? true : false) : false);

		SportCdTime (SportResp.time);

		if (SportResp.nextTimeTo21 > 0)
		{
			GetRewardCdTime (SportResp.nextTimeTo21);
		}
		else
		{
			m_getRewardCd.text = "";
		}

		DailyRewardBtnEffect ();

		InItJunXianRoomInfo ();
	}

	public void DailyRewardBtnEffect ()
	{
		QXComData.ClearEffect (m_dailyRewardBtn);
		QXComData.ClearEffect (m_rankRewardBtn);

		if (SportResp.nextTimeTo21 == 0)
		{
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_dailyRewardBtn,620247);
		}
		else
		{
			QXComData.ClearEffect (m_dailyRewardBtn);
		}

		if (SportResp.historyHighRank > 1 && SportResp.rankAward == 0)
		{
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_rankRewardBtn,620247);
		}
		else
		{
			QXComData.ClearEffect (m_rankRewardBtn);
		}
	}

	private string AddColor (string tempStr)
	{
		return "[ffffff]" + tempStr + "[-]";
	}

	#region SportCdTime
	private float m_sportCdTime;
	void SportCdTime (int cdTime)
	{
		m_sportCdTime = cdTime;
		if (TimeHelper.Instance.IsTimeCalcKeyExist("SportCdTime"))
		{
			TimeHelper.Instance.RemoveFromTimeCalc("SportCdTime");
		}
		TimeHelper.Instance.AddEveryDelegateToTimeCalc("SportCdTime", cdTime, OnUpdateSportTime);
	}

	private void OnUpdateSportTime (int p_time)
	{
		if (m_sportCdTime - p_time > 0)
		{
			m_sportCd.text = AddColor((SportResp.leftTimes > 0 ? "冷却时间：" : "重置时间：") + TimeHelper.GetUniformedTimeString (m_sportCdTime - p_time));
//			Debug.Log ("m_sportCdTime - p_time:" + (m_sportCdTime - p_time));
		}
		else
		{
			TimeHelper.Instance.RemoveFromTimeCalc("SportCdTime");
			SportResp.time = 0;
			m_sportCd.text = "";
			m_clearCdBtnObj.SetActive (false);
		}
	}

	#endregion

	#region GetRewardCdTime
	private float m_getRewardCdTime;
	void GetRewardCdTime (int cdTime)
	{
		m_getRewardCdTime = cdTime;
		if (TimeHelper.Instance.IsTimeCalcKeyExist("GetRewardCdTime"))
		{
			TimeHelper.Instance.RemoveFromTimeCalc("GetRewardCdTime");
		}
		TimeHelper.Instance.AddEveryDelegateToTimeCalc("GetRewardCdTime", cdTime, OnUpdateGetRewardTime);
	}
	
	private void OnUpdateGetRewardTime (int p_time)
	{
		if (m_getRewardCdTime - p_time > 0)
		{
			m_getRewardCd.text = TimeHelper.GetUniformedTimeString (m_getRewardCdTime - p_time);
//			Debug.Log ("m_getRewardCdTime - p_time:" + (m_getRewardCdTime - p_time));
		}
		else
		{
			TimeHelper.Instance.RemoveFromTimeCalc("GetRewardCdTime");
			SportResp.nextTimeTo21 = 0;
			m_getRewardCd.text = "";
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_dailyRewardBtn,620247);
		}
	}
	
	#endregion

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
		SportResp.time = tempInfo.cdTime;

		m_clearCdBtnObj.SetActive (SportResp.leftTimes > 0 ? (SportResp.time > 0 ? true : false) : false);

		SportCdTime (SportResp.time);
	}

	/// <summary>
	/// Refreshs the player info.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void RefreshPlayerInfo (CleanCDInfo tempInfo)
	{
		SportResp.cdYuanBao = tempInfo.nextCDYB;
		SportResp.time = 0;
		SportCdTime (SportResp.time);
		m_clearCdBtnObj.SetActive (false);
		m_sportCd.text = "";
	}

	/// <summary>
	/// Refreshs the wei wang.
	/// </summary>
	/// <param name="tempWeiWang">Temp wei wang.</param>
	public void RefreshWeiWang (int tempWeiWang)
	{
		SportResp.hasWeiWang = tempWeiWang;
		m_shouYi.text = AddColor("威望：" + SportResp.hasWeiWang + "威望");
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

	private Vector3 m_targetPos;
	private iTween.EaseType m_targetType;
	private float m_moveTime;

	public Camera m_camera;
	public GameObject m_pageObj;

	public GameObject m_tempObj1;
	public GameObject m_tempObj2;

	void MoveToMyHeadPos ()
	{
		m_headIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
//		SportTemp.id = 901;
		float pageWidth = m_pageObj.GetComponent<UITexture> ().width;
		float pageHeight = m_pageObj.GetComponent<UITexture> ().height;
		foreach (GameObject obj in m_junXianRoomList)
		{
			SportJunXian sportJunXian = obj.GetComponent<SportJunXian> ();
			if (SportTemp.id == sportJunXian.m_sportTemp.id)
			{
				m_ownHeadObj.transform.parent = obj.transform.parent;
				m_ownHeadObj.transform.localPosition = obj.transform.localPosition + new Vector3(0,375,0);

				m_tempObj1.transform.localPosition = m_texPosDic[SportTemp.id] - new Vector3(pageWidth / 2,pageHeight / 2,0);
				m_tempObj2.transform.localPosition = m_texPosDic[SportTemp.id] + new Vector3(pageWidth / 2,pageHeight / 2,0);

				m_targetPos = obj.transform.localPosition + new Vector3 (0, 75, 0);
				m_targetType = iTween.EaseType.easeOutQuart;
				m_moveTime = 1;
				break;
			}
		}
	
		#if SetMapPos
		
		Vector3 sToView1 = m_camera.ScreenToViewportPoint (new Vector3 (0,0,0));
		Vector3 sToView2 = m_camera.ScreenToViewportPoint (new Vector3 (Screen.width,Screen.height,0));
		
		Vector3 sToViewPos1 = m_camera.WorldToViewportPoint (m_tempObj1.transform.position);
		Vector3 sToViewPos2 = m_camera.WorldToViewportPoint (m_tempObj2.transform.position);

//		Debug.Log ("sToView1:" + sToView1);
//		Debug.Log ("sToView2:" + sToView2);
//
//		Debug.Log ("sToViewPos1:" + sToViewPos1);
//		Debug.Log ("sToViewPos2:" + sToViewPos2);
		
		Vector3 tarPos = m_texPosDic[SportTemp.id];

		if (sToViewPos1.x > sToView1.x)
		{
			float dis = (sToViewPos1.x - sToView1.x) * Screen.width;
			tarPos.x = m_texPosDic[SportTemp.id].x - dis;
		}
		if (sToViewPos1.y > sToView1.y)
		{
			float dis = (sToViewPos1.y - sToView1.y) * Screen.height;
			tarPos.y = m_texPosDic[SportTemp.id].y - dis;
		}

		if (sToViewPos2.x < sToView2.x)
		{
			float dis = (sToView2.x - sToViewPos2.x) * Screen.width;
			tarPos.x = m_texPosDic[SportTemp.id].x + dis;
		}
		if (sToViewPos2.y < sToView2.y)
		{
			float dis = (sToView2.y - sToViewPos2.y) * Screen.height;
			tarPos.y = m_texPosDic[SportTemp.id].y + dis;
		}

		#endif

		if (m_pageObj.transform.localPosition != tarPos)
		{
			PageMove (tarPos);
		}
		else
		{
//			m_moveFinished = true;
		}

		PageScale ();
		if (QXComData.CheckYinDaoOpenState (100200))
		{
			m_ownHeadObj.SetActive (false);
		}
		else
		{
			m_ownHeadObj.SetActive (true);
			HeadItween ();
		}
	}

	void PageMove (Vector3 pos)
	{
		Hashtable move = new Hashtable ();
		move.Add ("time",m_moveTime);
		move.Add ("easetype",iTween.EaseType.easeInOutQuad);
		move.Add ("position",pos);
		move.Add ("islocal",true);

		move.Add ("oncomplete","MoveEnd");
		move.Add ("oncompletetarget",gameObject);

		iTween.MoveTo (m_pageObj,move);
	}

	void MoveEnd ()
	{
//		m_moveFinished = true;
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

		QXComData.ClearEffect (m_dailyRewardBtn);
		QXComData.ClearEffect (m_rankRewardBtn);
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

		SportResp.rankAward = 0;
		SportHighRank.m_instance.InItHighRank (m_rewardList);
		SportHighRank.m_instance.M_SportHighRankDelegate = GetHighRankRewardBack;

		QXComData.ClearEffect (m_dailyRewardBtn);
		QXComData.ClearEffect (m_rankRewardBtn);
	}

	void GetHighRankRewardBack ()
	{
		m_getHighRankRewardObj.SetActive (false);

		DailyRewardBtnEffect ();

//		Global.m_isOpenBaiZhan = false;
		Global.m_isSportDataInItEnd = true;
        TaskData.Instance.IsCanShowComplete = true;
        m_isShopYinDao = true;
	}

	//打开剩余可领元宝窗口
	void OpenHighRankReward ()
	{
		m_dailyRankRewardObj.SetActive (true);
		SportResult.m_instance.InItRankReward ();
		SportResult.m_instance.M_SportResultDelegate = SportResultDelegateCallBack;

		QXComData.ClearEffect (m_dailyRewardBtn);
		QXComData.ClearEffect (m_rankRewardBtn);
	}

	void HighRankDelegateCallBack ()
	{
		m_dailyRankRewardObj.SetActive (false);

		DailyRewardBtnEffect ();
	}

	#endregion

	#region DailyRankReward
	[HideInInspector]public List<RewardData> m_dailyRewardList = new List<RewardData>();

	public GameObject m_dailyRankRewardObj;

	public void GetDailyRankReward ()
	{
		SportResp.nextTimeTo21 = -1;

		PushAndNotificationHelper.SetRedSpotNotification (300108,false);

		if (WarPage.m_instance != null)
		{
			WarPage.m_instance.CheckRedPoint ();
		}

		QXComData.ClearEffect (m_dailyRewardBtn);

		GeneralRewardManager.Instance().CreateReward (m_dailyRewardList);
	}

	private void OpenDailyRankReward ()
	{
		m_dailyRankRewardObj.SetActive (true);
		SportResult.m_instance.InItSportResult (SportResp.historyHighRank);
		SportResult.m_instance.M_SportResultDelegate = SportResultDelegateCallBack;

		QXComData.ClearEffect (m_dailyRewardBtn);
		QXComData.ClearEffect (m_rankRewardBtn);
	}

	void SportResultDelegateCallBack ()
	{
		m_dailyRankRewardObj.SetActive (false);

		DailyRewardBtnEffect ();
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
//		if (!m_moveFinished)
//		{
//			return;
//		}
		switch (ui.name)
		{
		case "ShopBtn":

			ShopData.Instance.OpenShop (ShopData.ShopType.WEIWANG);

			break;
		case "RecordBtn":

			QXComData.ClearEffect (m_dailyRewardBtn);
			QXComData.ClearEffect (m_rankRewardBtn);

			SportData.Instance.SportRecordReq ();
			PushAndNotificationHelper.SetRedSpotNotification (300105,false);
			m_recordRed.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (300105));
			if (WarPage.m_instance != null)
			{
				WarPage.m_instance.CheckRedPoint ();
			}

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

			if (SportResp.time > 0 || SportResp.leftTimes == 0)
			{
				PushAndNotificationHelper.SetRedSpotNotification (300103,false);
				if (WarPage.m_instance != null)
				{
					WarPage.m_instance.CheckRedPoint ();
				}
			}

			StopCdTime ();

			m_sportPageDelegate ();

			break;
		case "AddBtn":

			SportData.Instance.BuyChallengeTimes ();

			break;
		case "ClearCdBtn":

			SportData.Instance.M_ClearByBtn = true;
			m_textStr = "是否使用" + SportResp.cdYuanBao + "元宝清除挑战冷却？";
			QXComData.CreateBoxDiy (m_textStr,false,SportData.Instance.ClearCd,true,0,QXComData.SportClearCdVipLevel);

			break;
		default:
			SportJunXian junXian = ui.transform.parent.GetComponent<SportJunXian> ();
			if (junXian != null)
			{
				if (Input.touchCount > 1)
				{
					return;
				}
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

	void StopCdTime ()
	{
		if (TimeHelper.Instance.IsTimeCalcKeyExist ("SportCdTime"))
		{
			TimeHelper.Instance.RemoveFromTimeCalc("SportCdTime");
		}
		
		if (TimeHelper.Instance.IsTimeCalcKeyExist ("GetRewardCdTime"))
		{
			TimeHelper.Instance.RemoveFromTimeCalc("GetRewardCdTime");
		}
	}

	new void OnDestroy ()
	{
		StopCdTime ();
		base.OnDestroy ();
	}
}

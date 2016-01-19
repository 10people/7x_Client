using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BiaoJuPage : MonoBehaviour {

	public static BiaoJuPage bjPage;

	public enum BiaoJuPageType
	{
		MAIN_PAGE,
		HORSE_PAGE,
		RECORD_PAGE,
	}
	private BiaoJuPageType pType = BiaoJuPageType.MAIN_PAGE;

	public UILabel biaoJuTitle;

	private readonly Dictionary<BiaoJuPageType,string> titleDic = new Dictionary<BiaoJuPageType, string> ()
	{
		{BiaoJuPageType.MAIN_PAGE,"镖局"},
		{BiaoJuPageType.HORSE_PAGE,"镖局马场"},
		{BiaoJuPageType.RECORD_PAGE,"劫镖记录"}
	};

	public GameObject mainPageObj;
	public GameObject horsePageObj;
	public GameObject recordPageObj;

	public ScaleEffectController sEffectController;

	private bool isFirstOpen = true;

	private string textStr;

	public enum AtlasType
	{
		COM,
		MAINCITYLAYER,
		YUNBIAO,
	}
	public UIAtlas comAtlas;
	public UIAtlas mainCityAtlas;
	public UIAtlas biaoJuAtlas;

	public GameObject recordRed;
	public GameObject enemyRed;

	public GameObject anchorTopRight;

	void Awake ()
	{
		bjPage = this;
	}

	void Start ()
	{
		//mainPage rules
		//LanguageTemp LID:533-538 LanguageTemplate:YUN_BIAO_78-YUN_BIAO_83
		rulesLabel.fontSize = 20;
		List<string> ruleList = new List<string> ();
		string ybTimeStr = "";
		for (int i = 0;i < 6;i ++)
		{
			string s = "YUN_BIAO_" + (78 + i);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			if (i == 1)
			{
				//开服时间
				string openTime = CanshuTemplate.GetStrValueByKey (CanshuTemplate.OPENTIME_YUNBIAO);
				string closeTime = CanshuTemplate.GetStrValueByKey (CanshuTemplate.CLOSETIME_YUNBIAO);
				ruleStr += "[dc0600]" + openTime + "—" + closeTime + "[-]";
				ybTimeStr = ruleStr;
			}
			
			if (i == 3)
			{
				string[] strLen = ruleStr.Split ('*');
				
				ruleStr = strLen[0] + "[00ff00]" + 50 + "[-]" + strLen[1];
			}
			
			if (i != 1)
			{
				ruleList.Add (ruleStr);
			}
		}
		for (int i = 0;i < ruleList.Count;i ++)
		{
			ybTimeStr += "\n" + ruleList[i];
		}
		rulesLabel.text = ybTimeStr;

		//horsePage rules
		//LanguageTemp LID:437-440 LanguageTemplate:YUN_BIAO_4-YUN_BIAO_7
		List<string> horseRuleList = new List<string> ();
		for (int i = 0;i < 4;i ++)
		{
			string s = "YUN_BIAO_" + (4 + i);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			horseRuleList.Add (ruleStr);
		}
		for (int i = 0;i < horseRuleList.Count;i ++)
		{
			if (i < horseRuleList.Count - 1)
			{
				horseRuleLabel.text += horseRuleList[i] + "\n";
			}
			else
			{
				horseRuleLabel.text += horseRuleList[i];
			}
		}

		//enemyPage rules
		string enemyRule = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_72);//规则说明
		string[] enemyRuleLength = enemyRule.Split ('：');
		enemyRules.text = enemyRuleLength[0] + "：\n       " + enemyRuleLength[1];

		QXComData.LoadYuanBaoInfo (anchorTopRight);
	}

	void SwitchPage (BiaoJuPageType tempType)
	{
		pType = tempType;
		biaoJuTitle.text = titleDic[tempType];
		mainPageObj.SetActive (tempType == BiaoJuPageType.MAIN_PAGE ? true : false);
		horsePageObj.SetActive (tempType == BiaoJuPageType.HORSE_PAGE ? true : false);
		recordPageObj.SetActive (tempType == BiaoJuPageType.RECORD_PAGE ? true : false);
	}

	/// <summary>
	/// Gets the atlas.
	/// </summary>
	/// <returns>The atlas.</returns>
	/// <param name="tempType">Temp type.</param>
	public UIAtlas GetAtlas (AtlasType tempType)
	{
		UIAtlas atlas = new UIAtlas ();
		switch (tempType)
		{
		case AtlasType.COM:
			atlas = comAtlas;
			break;
		case AtlasType.MAINCITYLAYER:
			atlas = mainCityAtlas;
			break;
		case AtlasType.YUNBIAO:
			atlas = biaoJuAtlas;
			break;
		default:
			break;
		}
		return atlas;
	}

	#region BiaoJuMainPage

	private YabiaoMainInfoResp biaoJuResp;

	public List<EventHandler> mainPageBtnList = new List<EventHandler> ();

	public UILabel rulesLabel;
	public UILabel numLabel;

	private int needYb;//购买运镖次数需要的元宝

	/// <summary>
	/// Gets the biao ju resp.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void GetBiaoJuResp (YabiaoMainInfoResp tempResp)
	{
		biaoJuResp = tempResp;
		SwitchPage (BiaoJuPageType.MAIN_PAGE);
		if (isFirstOpen)
		{
			sEffectController.OnOpenWindowClick ();
			isFirstOpen = false;
		}

		string countTimeStr = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_42);
		string[] countTimeStrLength = countTimeStr.Split ('*');

		numLabel.text = countTimeStrLength[0] + "[dc0600]" + tempResp.yaBiaoCiShu.ToString () + "[-]" + countTimeStrLength[1];
		countDownTimeLabel.text = MyColorData.getColorString (10,"今日剩余") + MyColorData.getColorString (biaoJuResp.yaBiaoCiShu > 0 ? 4 : 5,biaoJuResp.yaBiaoCiShu.ToString ()) + MyColorData.getColorString (10,"次");

		foreach (EventHandler handler in mainPageBtnList)
		{
			handler.m_handler -= BiaoJuBtnHandlerClickBack;
			handler.m_handler += BiaoJuBtnHandlerClickBack;
		}

		SetPlunderRed (1,tempResp.isNew4History);
		SetPlunderRed (2,tempResp.isNew4Enemy);
	}

	void BiaoJuBtnHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "YunBiaoBtn":

			if (!biaoJuResp.isOpen)
			{
				textStr = "押镖活动未开启！";
				QXComData.CreateBox (1,textStr,true,null);
			}
			else
			{
				if (biaoJuResp.yaBiaoCiShu > 0)
				{
					//请求镖局马场
					BiaoJuData.Instance.OpenBiaoJuHorse ();
				}
				else
				{
					int myVipLevel = JunZhuData.Instance ().m_junzhuInfo.vipLv;
					if (myVipLevel >= VipFuncOpenTemplate.GetNeedLevelByKey (19))//购买运镖次数开启条件
					{
						int haveBuyTime = biaoJuResp.buyCiShu;//已经购买几回
						VipTemplate vipTemp = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance ().m_junzhuInfo.vipLv);
						int canBuyYBTimes = vipTemp.YunbiaoTimes;//可以购买几回
						
						if (myVipLevel <= QXComData.maxVipLevel)
						{
							if (haveBuyTime < canBuyYBTimes)
							{
								PurchaseTemplate purchasTemp = PurchaseTemplate.GetPurchaseTempByTime (haveBuyTime + 1);
								needYb = purchasTemp.price;
								
								textStr = "您今日的运镖次数已用尽！\n确定消耗" + purchasTemp.price + "元宝购买"
									+ purchasTemp.number + "次运镖次数？\n您还有" + (canBuyYBTimes - haveBuyTime) + "次购买机会！";
								QXComData.CreateBox (1,textStr,false,BuyYBTimesCallBack);//是否购买*************************
							}
							else
							{
								if (myVipLevel == QXComData.maxVipLevel)
								{
									textStr = "您今日的运镖次数已用尽！\n\n已无购买机会...";
									QXComData.CreateBox (1,textStr,true,null);
								}
								else
								{
									int countVip = QXComData.maxVipLevel - myVipLevel;
									int nextCanBuyVip = 0;
									
									for (int i = 0;i < countVip;i ++)
									{
										VipTemplate cVipTemp = VipTemplate.GetVipInfoByLevel (myVipLevel + (i + 1));
										if (cVipTemp.YunbiaoTimes > canBuyYBTimes)
										{
											nextCanBuyVip = myVipLevel + (i + 1);
											break;
										}
									}
									
									textStr = "您今日的购买运镖次数已用尽！\n\n提升到VIP" + nextCanBuyVip + "级可以够买更多次数！\n\n是否跳转到充值？";
									QXComData.CreateBox (1,textStr,false,TurnToVip);//跳转到充值**************************
								}
							}
						}
					}
					else
					{
						textStr = "升到VIP" + VipFuncOpenTemplate.GetNeedLevelByKey (19) + "级可购买运镖次数！\n\n是否跳转到充值？";
						QXComData.CreateBox (1,textStr,false,TurnToVip);//跳转到充值**************************
					}
				}
			}

			break;
		case "RecordBtn":

			BiaoJuData.Instance.BiaoJuRecordReq ();
			SetPlunderRed (1,false);

			break;
		case "EnemyBtn":

			BiaoJuData.Instance.EnemyPageReq ();

			break;
		case "CloseBtn":

			if (pType == BiaoJuPageType.MAIN_PAGE)
			{
				sEffectController.OnCloseWindowClick ();
				sEffectController.CloseCompleteDelegate += CloseBiaoJu;
			}
			else
			{
				SwitchPage (BiaoJuPageType.MAIN_PAGE);
			}

			break;
		default:
			break;
		}
	}

	//是否购买运镖次数
	void BuyYBTimesCallBack (int i)
	{
		if (i == 2)
		{
			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao >= needYb)
			{
				BiaoJuData.Instance.BuyYunBiaoTimeReq ();
			}
			else
			{
				LackYuanbao ();
			}
		}
	}
	/// <summary>
	/// Lacks the yuanbao.
	/// </summary>
	public void LackYuanbao ()
	{
		textStr = "元宝不足，是否跳转到充值？";
		QXComData.CreateBox (1,textStr,false,TurnToVip);
	}
	//跳转到充值
	void TurnToVip (int i)
	{
		if (i == 2)
		{

		}
	}

	#endregion

	#region BiaoJuHorsePage
	private YabiaoMenuResp horsePageInfo;
	//horseDic[type][]:0-name 1-horseItemIcon 2-setHorseItemIcon 3-pinzhi 4-point pos
	private readonly Dictionary<int,string[]> horseDic = new Dictionary<int, string[]> ()
	{
		{1,new string[]{"骡马","horse1","horseIcon1","pinzhi0","0"}},
		{2,new string[]{"驮马","horse2","horseIcon2","pinzhi1","-7.5"}},
		{3,new string[]{"膘马","horse3","horseIcon3","pinzhi3","-1"}},
		{4,new string[]{"骏马","horse4","horseIcon4","pinzhi6","4"}},
		{5,new string[]{"神驹","horse5","horseIcon5","pinzhi9","9.5"}}
	};

	public List<EventHandler> horsePageBtnList = new List<EventHandler> ();

	private List<BiaoJuHorseInfo> horseInfoList = new List<BiaoJuHorseInfo> ();

	public GameObject horseItemObj;
	private List<GameObject> horseList = new List<GameObject> ();

	public GameObject propItemObj;
	private List<GameObject> propItemList = new List<GameObject> ();

	private List<HorsePropInfo> allPropInfoList = new List<HorsePropInfo> ();
	private int buyCount;

	public UILabel horseShouYi;
	public UILabel horseRuleLabel;
	public UILabel countDownTimeLabel;

	public GameObject setHorseWindowObj;
	public GameObject horsePropWindowObj;

	public UISprite stopSprite;

	public GameObject selectBox;

	private float animateTime;
	private int moveCount;
	private int startCount;

	private bool shining;//是否播放选择框闪烁动画
	private int shiningCount;//闪烁次数
	private float alphaCount;

	private int curHorseLevel;
	public int CurHorseLevel {set{curHorseLevel = value;} get{return curHorseLevel;}}//当前马等级

	public void GetHorseResp (YabiaoMenuResp tempResp)
	{
		//reset info
		{
			startCount = 0;
			moveCount = 25;
			animateTime = 0.01f;

			shining = false;
			shiningCount = 0;
			alphaCount = 0;

			buyCount = 0;

			targetEnemy = null;
		}

		SwitchPage (BiaoJuPageType.HORSE_PAGE);
	
		horsePageInfo = tempResp;

		CurHorseLevel = tempResp.horse;

		if (horseList.Count <= 0)
		{
			for (int i = 0;i < 5;i ++)
			{
				GameObject horseItem = (GameObject)Instantiate (horseItemObj);
				
				horseItem.SetActive (true);
				horseItem.transform.parent = horseItemObj.transform.parent;
				horseItem.transform.localPosition = new Vector3 (-370 + i * 185,0,0);
				horseItem.transform.localScale = Vector3.one;
				
				horseList.Add (horseItem);
			}
		}

		horseInfoList.Clear ();
		for (int i = 0;i < horseList.Count;i ++)
		{
			BiaoJuHorseInfo horseInfo = new BiaoJuHorseInfo();
			horseInfo.horseId = i + 1;
			horseInfo.horseItemId = 902000 + i + 1;
			horseInfo.shouYi = GetHorseAwardNum (i + 1) - GetHorseAwardNum (1);
//			horseInfo.horseName = horseDic[i + 1][0];
			CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (i + 1);
			horseInfo.horseName = cartTemp.Name;
			horseInfo.needVipLevel = cartTemp.vipMin;
			horseInfo.upNeedMoney = cartTemp.ShengjiCost;
			horseInfoList.Add (horseInfo);

			BiaoJuHorseItem horseItem = horseList[i].GetComponent<BiaoJuHorseItem> ();
			horseItem.InItHorseItem (horseInfo);
		}

		if (propItemList.Count == 0)
		{
			propItemList.Add (propItemObj);
			for (int i = 0;i < 2;i ++)
			{
				GameObject propItem = (GameObject)Instantiate (propItemObj);
				propItem.transform.parent = propItemObj.transform.parent;
				propItem.transform.localPosition = new Vector3(105 * i,0,0);
				propItem.transform.localScale = propItemObj.transform.localScale;
				propItemList.Add (propItem);
			}

			foreach (MaJuTemplate temp in MaJuTemplate.GetMaJuTemplateList ())
			{
				if (temp.id != 910007)
				{
					HorsePropInfo propInfo = HorsePropInfo.CreateHorseProp (temp.id,
					                                                        NameIdTemplate.GetName_By_NameId (temp.nameId),
					                                                        temp.iconId,
					                                                        temp.colorId,
					                                                        DescIdTemplate.GetDescriptionById (temp.descId),
					                                                        temp.priceId,
					                                                        false);
					allPropInfoList.Add (propInfo);
				}
			}
		}

		InItHorseProp (tempResp.horseprop);

		HorseRandomAnimation (horsePageInfo.isNewHorse,horsePageInfo.horse);
//		HorseRandomAnimation (true,horsePageInfo.horse);

		foreach (EventHandler handler in horsePageBtnList)
		{
			handler.m_handler -= HorsePageBtnHandlerClickBack;
			handler.m_handler += HorsePageBtnHandlerClickBack;
		}
	}

	/// <summary>
	/// Ins it horse property.
	/// </summary>
	/// <param name="tempProp">Temp property.</param>
	public void InItHorseProp (HorseProp tempProp)
	{
		buyCount = 0;
		List<HorsePropInfo> horsePropInfoList = new List<HorsePropInfo> ();
		if (tempProp.toolId != null)
		{
			foreach (int id in tempProp.toolId)
			{
				foreach (HorsePropInfo propInfo in allPropInfoList)
				{
					if (id == propInfo.id)
					{
						buyCount ++;
						propInfo.isBuy = true;
						horsePropInfoList.Add (propInfo);
					}
				}
			}
		}
		else
		{
			foreach (HorsePropInfo propInfo in allPropInfoList)
			{
				propInfo.isBuy = false;
			}
		}

		for (int i = 0;i < propItemList.Count;i ++)
		{
			UISprite iconSprite = propItemList[i].transform.FindChild ("Icon").gameObject.GetComponent<UISprite> ();
			if (i < horsePropInfoList.Count)
			{
				iconSprite.atlas = GetAtlas (AtlasType.YUNBIAO);
				iconSprite.spriteName = horsePropInfoList[i].iconId.ToString ();
			}
			else
			{
				iconSprite.atlas = GetAtlas (AtlasType.COM);
				iconSprite.spriteName = "add";
			}

			EventHandler handler = propItemList[i].GetComponent<EventHandler> ();
			handler.m_handler -= OpenHorsePropWindow;
			handler.m_handler += OpenHorsePropWindow;
		}
	}

	/// <summary>
	/// Haves the property count.
	/// </summary>
	/// <returns>The property count.</returns>
	public int HavePropCount ()
	{
		return buyCount;
	}

	void HorsePageBtnHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "AddLevelBtn":

			OpenSetHorseWindow ();

			break;
		case "BuyPropsBtn":

			OpenHorsePropWindow (gameObject);

			break;
		case "YunBiaoBtn":

			//开始运镖
			if (biaoJuResp.yaBiaoCiShu <= 0)
			{
				//次数不够
			}
			else
			{
				textStr = "确定消耗一个运镖次数吗？\n\n如果运镖成功，收益会通过邮件发送给您。";
				QXComData.CreateBox (1,textStr,false,YunBiaoBtnClickBack);
			}

			break;
		default:
			break;
		}
	}

	void YunBiaoBtnClickBack (int i)
	{
		if (i == 2)
		{
			//发送运镖请求
			BiaoJuData.Instance.BeginYunBiaoReq ();
		}
	}
	
	/// <summary>
	/// 获得收益数值
	/// </summary>
	public int GetHorseAwardNum (int type)
	{
		JunzhuShengjiTemplate junZhuUpLevelTemp = JunzhuShengjiTemplate.GetJunZhuShengJi (JunZhuData.Instance ().m_junzhuInfo.level);
		int xiShu = junZhuUpLevelTemp.xishu;
		
		CartTemplate cartemp = CartTemplate.GetCartTemplateByType (type);
		float award = cartemp.ProfitPara * xiShu;
		
		return (int)award;
	}

	/// <summary>
	/// Horses the string info.
	/// </summary>
	/// <returns>The string info.</returns>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempIndex">Temp index.</param>
	public string HorseStringInfo (int tempType,int tempIndex)
	{
		if (!horseDic.ContainsKey (tempType))
		{
			Debug.LogError ("Cant't find key in horseDic");
		}
		else
		{
			if (tempIndex >= horseDic[tempType].Length || tempIndex < 0)
			{
				Debug.LogError ("Cant't find value in horseDic,index is out:" + tempIndex);
			}
		}
		return horseDic[tempType][tempIndex];
	}
	
	/// <summary>
	/// Opens the set horse window.
	/// </summary>
	public void OpenSetHorseWindow ()
	{
		setHorseWindowObj.SetActive (true);
		SetHorseWindow setHorse = setHorseWindowObj.GetComponent<SetHorseWindow> ();
		setHorse.InItSetHorseWindow (horseInfoList,horsePageInfo.horse);
	}

	/// <summary>
	/// Refreshs the horse page.
	/// </summary>
	/// <param name="tempEndType">Temp end type.</param>
	public void RefreshHorsePage (int tempEndType)
	{
		horsePageInfo.horse = tempEndType;
		CurHorseLevel = tempEndType;

		OpenSetHorseWindow ();

		HorseRandomAnimation (false,horsePageInfo.horse);
	}

	/// <summary>
	/// 随机马的动画
	/// </summary>
	void HorseRandomAnimation (bool isRandom,int horseType)
	{
		if (isRandom)
		{
			//随机动画
			ZheZhaoControl (true);
			StartCoroutine ("HorseSelectAnimate");
		}
		else
		{
			MoveToTargetPos (horseType - 1);
		}
	}

	/// <summary>
	/// Zhes the zhao control.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	void ZheZhaoControl (bool isActive)
	{
		stopSprite.alpha = 0.1f;
		stopSprite.gameObject.SetActive (isActive);
	}

	/// <summary>
	/// 选择框移动到指定位置 
	/// </summary>
	void MoveToTargetPos (int i)
	{
		int indexNumber = 0;
		if (i > 4)
		{
			indexNumber = i % 5;
		}
		else
		{
			indexNumber = i;
		}
		selectBox.transform.localPosition = new Vector3 (-370 + 185 * indexNumber,0,0);
		
		if (indexNumber < 1)
		{
			horseShouYi.text = GetHorseAwardNum (1).ToString ();
		}
		else
		{
			horseShouYi.text = MyColorData.getColorString (10,GetHorseAwardNum (1).ToString ()) + MyColorData.getColorString (4,"+" + (GetHorseAwardNum (indexNumber + 1) - GetHorseAwardNum (1)).ToString ());
		}
	}

	//选马动画
	IEnumerator HorseSelectAnimate ()
	{
		while (startCount < moveCount + horsePageInfo.horse)
		{
			MoveToTargetPos (startCount);
			startCount ++;

			if (startCount < moveCount + horsePageInfo.horse - 5)
			{
				animateTime = 0.01f;
			}
			else
			{
				if (animateTime < 0.5f)
				{
					animateTime += 0.08f;
				}
				else
				{
					animateTime = 0.5f;
				}
			}
			
			if (startCount == moveCount + horsePageInfo.horse)
			{
				//播放选择框闪烁动画
				StopCoroutine ("HorseSelectAnimate");
				StartCoroutine ("Shining");
			}
			
			yield return new WaitForSeconds (animateTime);
		}
	}

	IEnumerator Shining ()
	{
		yield return new WaitForSeconds (1);
		shining = true;
	}

	/// <summary>
	/// Opens the horse property window.
	/// </summary>
	public void OpenHorsePropWindow (GameObject obj)
	{
		horsePropWindowObj.SetActive (true);
		HorsePropWindow horseProp = horsePropWindowObj.GetComponent<HorsePropWindow> ();
		horseProp.InItHorsePropWindow (allPropInfoList);
	}

	#endregion

	void Update ()
	{
		if (shining)
		{
			if (shiningCount < 3)
			{
				alphaCount -= Time.deltaTime * 15f;
				
				if(alphaCount <= -1 )
				{
					alphaCount = 1;
					shiningCount ++;
				}
				
				selectBox.GetComponent<UIWidget> ().alpha = Mathf.Abs (alphaCount);
			}
			else
			{
				StopCoroutine ("Shining");
				ZheZhaoControl (false);
				shining = false;
			}
		}
	}

	#region RecordPage

	private YBHistoryResp recordResp;

	public UIScrollView recordSc;
	public UIScrollBar recordSb;

	public UIGrid recordGrid;
	public GameObject recordItemObj;
	private List<GameObject> recordItemList = new List<GameObject> ();

	public UILabel descLabel;
	public UILabel recordNumLabel;

	/// <summary>
	/// Ins it record page.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void InItRecordPage (YBHistoryResp tempResp)
	{
		SwitchPage (BiaoJuPageType.RECORD_PAGE);

		descLabel.text = tempResp.historyList.Count == 0 ? "没有劫镖记录" : "";
		recordNumLabel.text = "条数" + tempResp.historyList.Count + "/50";

		recordItemList = QXComData.CreateGameObjectList (recordItemObj,recordGrid,tempResp.historyList.Count,recordItemList);

		for (int i = 0;i < tempResp.historyList.Count - 1;i ++)
		{
			for (int j = 0;j < tempResp.historyList.Count - i - 1;j ++)
			{
				if (tempResp.historyList[j].time < tempResp.historyList[j + 1].time)
				{
					YBHistory tempHis = tempResp.historyList[j];
					
					tempResp.historyList[j] = tempResp.historyList[j + 1];
					
					tempResp.historyList[j + 1] = tempHis;
				}
			}
		}

		recordSc.enabled = tempResp.historyList.Count > 4 ? true : false;
		recordSb.gameObject.SetActive (tempResp.historyList.Count > 4 ? true : false);

		for (int i = 0;i < recordItemList.Count;i ++)
		{
			recordSc.UpdateScrollbars (true);
			BiaoJuRecordItem record = recordItemList[i].GetComponent<BiaoJuRecordItem> ();
			record.InItRecordItem (tempResp.historyList[i]);
		}
	}

	#endregion

	#region EnemyPage

	private EnemiesResp enemyResp;

	public GameObject enemyPageObj;

	public List<EventHandler> enemyPageBtnList = new List<EventHandler> ();

	public UIScrollView enemySc;
	public UIScrollBar enemySb;

	public UIGrid enemyGrid;

	public GameObject enemyItemObj;
	private List<GameObject> enemyItemList = new List<GameObject> ();

	public UILabel enemyNumLabel;
	public UILabel enemyDesLabel;

	public UILabel enemyRules;

	private bool isOpenEnemyPage = false;

	public ScaleEffectController enemySEffectController;

	private EnemiesInfo targetEnemy;

	private float refreshTime;

	/// <summary>
	/// Ins it enemy page.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void InItEnemyPage (EnemiesResp tempResp)
	{
		enemyResp = tempResp;
		enemyPageObj.SetActive (true);

		refreshTime = 10;
		StartCoroutine ("RefreshEnemyPage");

		if (!isOpenEnemyPage)
		{
			isOpenEnemyPage = true;
			enemySEffectController.OnOpenWindowClick ();
		}

		enemyItemList = QXComData.CreateGameObjectList (enemyItemObj,enemyGrid,tempResp.enemyList.Count,enemyItemList);

		for (int i = 0;i < enemyItemList.Count;i ++)
		{
			BiaoJuEnemyItem enemy = enemyItemList[i].GetComponent<BiaoJuEnemyItem> ();
			enemy.InItEnemyItem (tempResp.enemyList[i],targetEnemy);

			EventHandler handler = enemyItemList[i].GetComponent<EventHandler> ();
			handler.m_handler -= EnemyHandlerClickBack;
			handler.m_handler += EnemyHandlerClickBack;
		}

		enemyDesLabel.text = tempResp.enemyList.Count > 0 ? "" : LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_77);//还无人劫镖
		enemyNumLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_71) + tempResp.enemyList.Count + "/50";//仇家

		foreach (EventHandler handler in enemyPageBtnList)
		{
			handler.m_handler -= EnemyPageBtnHandlerClickBack;
			handler.m_handler += EnemyPageBtnHandlerClickBack;
		}
	}

	void EnemyHandlerClickBack (GameObject obj)
	{
		BiaoJuEnemyItem enemy = obj.GetComponent<BiaoJuEnemyItem> ();
		GetJieBiaoTarget (enemy.GetEnemiesInfo ());
	}

	void EnemyPageBtnHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "EnemyZheZhao":

			foreach (EnemiesInfo enemyInfo in enemyResp.enemyList)
			{
				if (enemyInfo.state == 10)
				{
					SetPlunderRed (2,true);
					break;
				}
				else
				{
					SetPlunderRed (2,false);
				}
			}
			
			CloseEnemyPage ();

			break;
		case "CloseBtn":

			foreach (EnemiesInfo enemyInfo in enemyResp.enemyList)
			{
				if (enemyInfo.state == 10)
				{
					SetPlunderRed (2,true);
					break;
				}
				else
				{
					SetPlunderRed (2,false);
				}
			}

			CloseEnemyPage ();

			break;
		case "JieBiaoBtn":

			if (targetEnemy == null)
			{
				textStr = "请选择一个正在进行运镖的仇人劫镖！";
				QXComData.CreateBox (1,textStr,true,null);
				return;
			}
			if (targetEnemy.state == 20)
			{
				textStr = targetEnemy.junZhuName + "还没开始运镖，无法劫镖！";
				QXComData.CreateBox (1,textStr,true,null);
			}
			else
			{
				CloseEnemyPage ();
				sEffectController.OnCloseWindowClick ();
				sEffectController.CloseCompleteDelegate += CloseBiaoJu;
			}

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Gets the jie biao target.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	void GetJieBiaoTarget (EnemiesInfo tempInfo)
	{
		targetEnemy = tempInfo;
		foreach (GameObject obj in enemyItemList)
		{
			BiaoJuEnemyItem enemy = obj.GetComponent<BiaoJuEnemyItem> ();
			enemy.SetSelectBox (enemy.GetEnemiesInfo ().junZhuId == targetEnemy.junZhuId ? true : false);
		}
	}

	IEnumerator RefreshEnemyPage ()
	{
		while (refreshTime > 0)
		{
			yield return new WaitForSeconds (1);
			refreshTime --;

			if (refreshTime <= 0)
			{
				//请求仇人列表
				BiaoJuData.Instance.EnemyPageReq ();
				StopCoroutine ("RefreshEnemyPage");
			}
		}
	}

	void CloseEnemyPage ()
	{
		isOpenEnemyPage = false;
		StopCoroutine ("RefreshEnemyPage");
		enemyPageObj.SetActive (false);
	}

	#endregion

	/// <summary>
	/// Sets the plunder red.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="isRed">If set to <c>true</c> is red.</param>
	public void SetPlunderRed (int type,bool isRed)
	{
		switch (type)
		{
		case 1:

			recordRed.SetActive (isRed);

			break;
		case 2:

			enemyRed.SetActive (isRed);

			break;
		default:
			break;
		}
	}

	public void CloseBiaoJu ()
	{
		isFirstOpen = true;
		gameObject.SetActive (false);
	}
}

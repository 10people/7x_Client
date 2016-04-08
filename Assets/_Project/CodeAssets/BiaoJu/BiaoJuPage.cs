using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

namespace Carriage
{
	public class BiaoJuPage : MonoBehaviour {

		public static BiaoJuPage bjPage;

		public enum BiaoJuPageType
		{
			MAIN_PAGE,
			HORSE_PAGE,
		}
		private BiaoJuPageType pType = BiaoJuPageType.MAIN_PAGE;

		public UILabel biaoJuTitle;

		private readonly Dictionary<BiaoJuPageType,string> titleDic = new Dictionary<BiaoJuPageType, string> ()
		{
			{BiaoJuPageType.MAIN_PAGE,"镖局"},
			{BiaoJuPageType.HORSE_PAGE,"镖局马场"},
		};

		public GameObject mainPageObj;
		public GameObject horsePageObj;

		public ScaleEffectController sEffectController;

		private bool isFirstOpen = true;

		private string textStr;
		private string fuLiTimeStr;

		public enum AtlasType
		{
			COM,
			MAINCITYLAYER,
			YUNBIAO,
		}
		public UIAtlas comAtlas;
		public UIAtlas mainCityAtlas;
		public UIAtlas biaoJuAtlas;

		public GameObject anchorTopRight;

		private string fuLiDesStr = "括号内为福利次数，优先消耗";

		void Awake ()
		{
			bjPage = this;
		}

		void Start ()
		{
			fuLiTimeStr = MyColorData.getColorString (5,YunBiaoTemplate.GetValueByKey (YunBiaoTemplate.incomeAdd_startTime2) + "—" + YunBiaoTemplate.GetValueByKey (YunBiaoTemplate.incomeAdd_endTime2));

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
					ruleStr += MyColorData.getColorString (4,openTime + "—" + closeTime + "[-]") + "      福利时段：" + fuLiTimeStr;
					ybTimeStr = ruleStr;
				}
				
//				if (i == 3)
//				{
//					string[] strLen = ruleStr.Split ('*');
//					
//					ruleStr = strLen[0] + "[00ff00]" + 50 + "[-]" + strLen[1];
//				}
				
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

			QXComData.LoadYuanBaoInfo (anchorTopRight);
		}

		void SwitchPage (BiaoJuPageType tempType)
		{
			pType = tempType;
			biaoJuTitle.text = titleDic[tempType];
			biaoJuTitle.GetComponent<UILabelType> ().init ();
			mainPageObj.SetActive (tempType == BiaoJuPageType.MAIN_PAGE ? true : false);
			horsePageObj.SetActive (tempType == BiaoJuPageType.HORSE_PAGE ? true : false);
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
		public YabiaoMainInfoResp BiaoJuResp
		{
			get{return biaoJuResp;}
			set{biaoJuResp = value;}
		}

		public List<EventHandler> mainPageBtnList = new List<EventHandler> ();

		public UILabel rulesLabel;
		public UILabel numLabel;

		public UILabel fuLiDesLabel;
		public UILabel fuLiDesLabel2;

		private int needYb;//购买运镖次数需要的元宝

		public GameObject yunBiaoBtnObj;

		private bool isStartYinDao = false;//是否已经引导中

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

			numLabel.text = countTimeStrLength[0] + MyColorData.getColorString (4,(tempResp.yaBiaoCiShu - RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes).ToString ()) 
							+ (RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes > 0 ? MyColorData.getColorString (5,"(" + RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes + ")") : "") + countTimeStrLength[1];

			fuLiDesLabel.text = RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes > 0 ? MyColorData.getColorString (5,fuLiDesStr) : "";
//			Debug.Log ("biaoJuResp.yaBiaoCiShu:" + biaoJuResp.yaBiaoCiShu);
//			Debug.Log ("RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes:" + RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes);
			countDownTimeLabel.text = MyColorData.getColorString (10,"今日剩余") 
							+ MyColorData.getColorString (4,(biaoJuResp.yaBiaoCiShu - RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes).ToString ()) 
							+ MyColorData.getColorString (5,RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes > 0 ? "(" + RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes + ")" : "")	
							+ MyColorData.getColorString (10,"次");

			fuLiDesLabel2.text = RootManager.Instance.m_CarriageMain.RemainingAdditionalStartTimes > 0 ? MyColorData.getColorString (5,fuLiDesStr) : "";

			SparkleEffectItem se = yunBiaoBtnObj.GetComponent<SparkleEffectItem> ();
			if (biaoJuResp.yaBiaoCiShu > 0 && biaoJuResp.isOpen)
			{
//				Debug.Log ("SparkleEffectItem1");
//				Debug.Log ("SparkleEffectItem：" + se.m_style);
				SparkleEffectItem.OpenSparkle (yunBiaoBtnObj,SparkleEffectItem.MenuItemStyle.None);
//				Debug.Log ("SparkleEffectItem：" + se.m_style);
			}
			else
			{
//				Debug.Log ("SparkleEffectItem2");
				SparkleEffectItem.CloseSparkle (yunBiaoBtnObj);
			}

			UIWidget[] btnWidgets = yunBiaoBtnObj.GetComponentsInChildren<UIWidget> ();
			foreach (UIWidget widget in btnWidgets)
			{
				widget.color = biaoJuResp.yaBiaoCiShu > 0 ? Color.white : Color.grey;
			}

			foreach (EventHandler handler in mainPageBtnList)
			{
				handler.m_click_handler -= BiaoJuBtnHandlerClickBack;
				handler.m_click_handler += BiaoJuBtnHandlerClickBack;
			}

			if (QXComData.CheckYinDaoOpenState (100370))
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,5);

			}
			else
			{
				UIYindao.m_UIYindao.CloseUI ();
			}

			isStartYinDao = true;
		}

		void BiaoJuBtnHandlerClickBack (GameObject obj)
		{
			if (!isStartYinDao)
			{
				return;
			}
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
						textStr = "每日" + fuLiTimeStr + "可领取额外的运镖次数";
						QXComData.CreateBox (1,textStr,true,null);
					}
				}

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
				if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= needYb)
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
			QXComData.CreateBoxDiy (textStr,false,TurnToVip);
		}

		/// <summary>
		/// Turns to vip.
		/// </summary>
		public void TurnToVip (int i)
		{
			if (i == 2)
			{
				EquipSuoData.TopUpLayerTip (RootManager.Instance.m_BiaoJuPage.gameObject,true);
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
		private int propReward;
		public int PropReward {set{propReward = value;} get{return propReward;}}//马具加成

		public UILabel horseShouYi;
		public UILabel horseRuleLabel;
		public UILabel countDownTimeLabel;

		public GameObject setHorseWindowObj;
		public GameObject horsePropWindowObj;
		public GameObject rewardTipsWindowObj;

		public UISprite stopSprite;

		public GameObject selectBox;

		private float animateTime;//随机马动画时间
		private int moveCount;
		private int startCount;

		private bool shining;//是否播放选择框闪烁动画
		private int shiningCount;//闪烁次数
		private float alphaCount;

		private int curHorseLevel;
		public int CurHorseLevel {set{curHorseLevel = value;} get{return curHorseLevel;}}//当前马等级

		private bool isHorseYinDao = false;

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
				PropReward = 0;
				isHorseYinDao = false;
			}

			SwitchPage (BiaoJuPageType.HORSE_PAGE);
		
			horsePageInfo = tempResp;
//			Debug.Log ("tempResp.horse:" + tempResp.horse);
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

			if (horsePageInfo.isNewHorse)
			{
				UIYindao.m_UIYindao.CloseUI ();
			}
			else
			{
				if (horsePageInfo.horse < 5)
				{
					QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,6);
				}
				else
				{
					if (CheckGaoJiMaBian ())
					{
						QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,12);
					}
					else
					{
						QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,9);
					}
				}
			}

			HorseRandomAnimation (horsePageInfo.isNewHorse,horsePageInfo.horse);
	//		HorseRandomAnimation (true,horsePageInfo.horse);

			foreach (EventHandler handler in horsePageBtnList)
			{
				handler.m_click_handler -= HorsePageBtnHandlerClickBack;
				handler.m_click_handler += HorsePageBtnHandlerClickBack;
			}
		}

		/// <summary>
		/// Ins it horse property.
		/// </summary>
		/// <param name="tempProp">Temp property.</param>
		public void InItHorseProp (HorseProp tempProp)
		{
			horsePageInfo.horseprop = tempProp;
			buyCount = 0;
			PropReward = 0;
			JunzhuShengjiTemplate junZhuUpLevelTemp = JunzhuShengjiTemplate.GetJunZhuShengJi (JunZhuData.Instance().m_junzhuInfo.level);
			int xiShu = junZhuUpLevelTemp.xishu;

			List<HorsePropInfo> horsePropInfoList = new List<HorsePropInfo> ();
			if (tempProp.toolId != null)
			{
				foreach (int id in tempProp.toolId)
				{
//					Debug.Log ("id:" + id);
					foreach (HorsePropInfo propInfo in allPropInfoList)
					{
						if (id == propInfo.id)
						{
							buyCount ++;
							propInfo.isBuy = true;
							horsePropInfoList.Add (propInfo);

							//计算收益
							MaJuTemplate maJuTemp = MaJuTemplate.GetMaJuTemplateById (propInfo.id);
							PropReward += (int)(maJuTemp.profitPara * xiShu); 
//							Debug.Log ("PropReward:" + PropReward);
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
					iconSprite.spriteName = "yuan" + horsePropInfoList[i].iconId.ToString ();
				}
				else
				{
					iconSprite.atlas = GetAtlas (AtlasType.COM);
					iconSprite.spriteName = "add";
				}

				iconSprite.width = iconSprite.height = i < horsePropInfoList.Count ? 80 : 35;

				EventHandler handler = propItemList[i].GetComponent<EventHandler> ();
				handler.m_click_handler -= OpenHorsePropWindow;
				handler.m_click_handler += OpenHorsePropWindow;
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
			if (!isHorseYinDao)
			{
				return;
			}
//			Debug.Log ("bbbbb");
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
//					textStr = "确定消耗一个运镖次数吗？\n\n如果运镖成功，收益会通过邮件发送给您。";
//					QXComData.CreateBox (1,textStr,false,YunBiaoBtnClickBack);
					OpenRewardTipsWindow ();
				}

				break;
			default:
				break;
			}
		}
		
		/// <summary>
		/// 获得收益数值
		/// </summary>
		public int GetHorseAwardNum (int type)
		{
			JunzhuShengjiTemplate junZhuUpLevelTemp = JunzhuShengjiTemplate.GetJunZhuShengJi (JunZhuData.Instance().m_junzhuInfo.level);
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
			SetHorseWindow.setHorse.InItSetHorseWindow (horseInfoList,horsePageInfo.horse);
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
				isHorseYinDao = true;
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
				horseShouYi.text = MyColorData.getColorString (10,GetHorseAwardNum (1).ToString ());
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
		/// Checks the gao ji ma bian.
		/// </summary>
		public bool CheckGaoJiMaBian ()
		{
			if (horsePageInfo.horseprop.toolId == null)
			{
				return false;
			}
			foreach (int id in horsePageInfo.horseprop.toolId)
			{
				if (id == 910008)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Opens the horse property window.
		/// </summary>
		public void OpenHorsePropWindow (GameObject obj)
		{
			horsePropWindowObj.SetActive (true);
			HorsePropWindow.propWindow.InItHorsePropWindow (allPropInfoList);
		}

		/// <summary>
		/// Opens the reward tips window.
		/// </summary>
		public void OpenRewardTipsWindow ()
		{
			rewardTipsWindowObj.SetActive (true);
			RewardTipsWindow.rewardTips.InItRewardTips (CloseRewardTipsWindow);
		}

		void CloseRewardTipsWindow (int i)
		{
			switch (i)
			{
			case 1:
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,12);
				break;
			case 2:
				BiaoJuData.Instance.BeginYunBiaoReq ();
				break;
			default:
				break;
			}

			rewardTipsWindowObj.SetActive (false);
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

					//check yindao state
					{
						if (horsePageInfo.horse < 5)
						{
							QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,6);
						}
						else
						{
							if (CheckGaoJiMaBian ())
							{
								QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,12);
							}
							else
							{
								QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,9);
							}
						}
					}

					ZheZhaoControl (false);

					shining = false;
				}

				isHorseYinDao = true;
			}
		}

		public void CloseBiaoJu ()
		{
			PushAndNotificationHelper.SetRedSpotNotification (312, biaoJuResp.yaBiaoCiShu > 0 ? true : false);

			isStartYinDao = false;

			isFirstOpen = true;
			gameObject.SetActive (false);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarPage : GeneralInstance<CityWarPage> {

	public delegate void CityWarDelegate ();
	public CityWarDelegate M_CityWarDelegate;

	public CityFightInfoResp CityResp;

	public GameObject m_cityObj;
	private List<GameObject> m_cityList = new List<GameObject>();

	[HideInInspector]public bool M_HaveNormalCity = false;	//是否已对某个城池宣战

	public UISprite m_tuiJian;
	public UILabel m_tuiJianLabel;
	public GameObject m_otherCityBtn;

	public UILabel m_allianceName;
	public UILabel m_allianceLevel;
	public UILabel m_earth;
	public UISprite m_allianceIcon;

	public List<UILabel> m_timeLabelList = new List<UILabel>();
	public UILabel m_bidDesLabel;
	public UISprite m_curTimeSprite;
	private bool m_addAlpha = false;

	private Vector3 m_tuiJianPos;
	private bool m_moveUp;

	[HideInInspector]
	public readonly Dictionary<int,string[]> M_TimeLabelDic = new Dictionary<int, string[]>()
	{
		//0 - 郡城战时段 1 - 时段name 2 - 城池状态spriteName
		{0,new string[]{LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_1),"宣战时段","YellowPoint"}},
		{1,new string[]{LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_2),"揭晓时段","GreenPoint"}},
		{2,new string[]{LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_3),"战斗时段","RedPoint"}},
		{3,new string[]{"","其它时段",""}},
	};

	[HideInInspector]
	public string m_xuanZhan = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_19);//宣战
	public string m_xuanZhanFail = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_20);//宣战失败
	public string m_attack = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_21);//进攻
	public string m_attackBefore = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_45);//进攻备战
	public string m_fangShou = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_22);//防守
	public string m_fangShouBefore = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_44);//防守备战
	public string m_rec = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_18);//推荐

	private bool m_isOpenFirst = true;
	public GameObject m_texObj;

	#region Red
	public GameObject m_rewardRed;
	#endregion

	public GameObject m_anchorTL;
	public GameObject m_anchorTR;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		QXComData.LoadTitleObj (m_anchorTL,"郡城战");
		QXComData.LoadYuanBaoInfo (m_anchorTR);
	}

	public void InItCityWarPage (CityFightInfoResp tempResp)
	{
		UIYindao.m_UIYindao.CloseUI ();

		CityResp = tempResp;
		M_HaveNormalCity = false;

		for (int i = 0;i < m_timeLabelList.Count;i ++)
		{
			m_timeLabelList[i].text = M_TimeLabelDic[i][1] + M_TimeLabelDic[i][0];
		}

		if (tempResp.interval == 3)
		{
			m_curTimeSprite.gameObject.SetActive (false);

			//set redPoint
			PushAndNotificationHelper.SetRedSpotNotification (300500,false);
//			NewAlliancemanager.Instance ().ShowJunChengZhanAlert ();
			CityWarData.Instance.SetAllianceRed ();
		}
		else
		{
			m_curTimeSprite.gameObject.SetActive (true);
			m_curTimeSprite.transform.localPosition = m_timeLabelList [tempResp.interval].transform.parent.transform.localPosition - new Vector3(9,0,0);
			m_curTimeSprite.alpha = 1;
		}

		m_allianceName.text = QXComData.AllianceName (QXComData.AllianceInfo ().name);
		m_allianceLevel.text = QXComData.AllianceInfo ().level.ToString ();
		m_allianceIcon.spriteName = QXComData.AllianceInfo ().icon.ToString ();
		m_earth.text = CityResp.myCityCount.ToString ();

		m_cityList = QXComData.CreateGameObjectList (m_cityObj,CityResp.cityList.Count,m_cityList);
		for (int i = 0;i < m_cityList.Count;i ++)
		{
			JCZCityTemplate cityTemp = JCZCityTemplate.GetJCZCityTemplateById (CityResp.cityList[i].cityId);
			float x = cityTemp.zuobiaoX;
			float y = cityTemp.zuobiaoY;

			m_cityList[i].transform.localPosition = new Vector3(x,y,0);

			CityItem cityItem = m_cityList[i].GetComponent<CityItem> ();
			cityItem.InItCityItem (CityResp.cityList[i]);

			if (tempResp.interval == 0)
			{
				if (CityResp.cityList[i].cityState2 == 1)
				{
					M_HaveNormalCity = true;
				}
			}
		}
//		Debug.Log ("M_HaveNormalCity:" + M_HaveNormalCity);
		InItTuiJianState ();

		foreach (UILabel label in m_infoLabelList)
		{
			Destroy (label.gameObject);
		}
		m_infoLabelList.Clear ();
		m_infoList.Clear ();
		for (int i = 0;i < tempResp.bidList.Count;i ++)
		{
			InItInfoLabel (tempResp.bidList[i]);
		}

		if (m_isOpenFirst)
		{
			m_isOpenFirst = false;
			m_texObj.transform.localScale = Vector3.one * 0.95f;
			m_texObj.transform.localPosition = new Vector3 (255,-85,0);
			TexScale ();
		}

		#region RedShow
		SetRewardRed ();
		#endregion
	}

	void InItTuiJianState ()
	{
		CityTween ();
	
//		Debug.Log ("recCityId:" + CityResp.recCityId);
		switch (CityResp.interval)
		{
		case 0:
			m_bidDesLabel.transform.localPosition = new Vector3(10,-95,0);
			m_bidDesLabel.text = MyColorData.getColorString (45,LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_4));
			if (CityResp.recCityId == 0)
			{
				m_tuiJian.spriteName = "RedArrow";
//				m_tuiJianLabel.text = "[0dbce8]" + m_xuanZhan + "[-]";//宣战
				m_tuiJianLabel.text = MyColorData.getColorString (5,m_attackBefore);//进攻备战
			}
			else
			{
				m_tuiJian.spriteName = CityResp.recCityId > 0 ? "GreenArrow" : "";
				m_tuiJianLabel.text = CityResp.recCityId > 0 ? MyColorData.getColorString (4,m_rec) : "";//推荐
			}
			break;
		case 1:

			//此时段仍可对野城宣战
			m_bidDesLabel.transform.localPosition = new Vector3(210,-95,0);
			m_bidDesLabel.text = CityResp.recCityId > 0 ? MyColorData.getColorString (45,LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_5)) : "";

			if (CityResp.recCityId == 0)
			{
				m_tuiJian.spriteName = "RedArrow";
//				m_tuiJianLabel.text = MyColorData.getColorString (5,m_attack);//进攻
				m_tuiJianLabel.text = MyColorData.getColorString (5,m_attackBefore);//进攻备战
			}
			else if (CityResp.recCityId > 0)
			{
				m_tuiJian.spriteName = "GreenArrow";
				m_tuiJianLabel.text = MyColorData.getColorString (4,m_rec);//推荐
			}
			else
			{
				m_tuiJian.spriteName = "";
				m_tuiJianLabel.text = "";
			}
			break;
		case 2:

			m_bidDesLabel.transform.localPosition = new Vector3(410,-95,0);
			m_bidDesLabel.text = (CityResp.recCityId == 0 || M_HaveNormalCity) ? MyColorData.getColorString (45,LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_6)) : "";

			m_tuiJian.spriteName = CityResp.recCityId == 0 ? "RedArrow" : "";
			m_tuiJianLabel.text = CityResp.recCityId == 0 ? MyColorData.getColorString (5,m_attack) : "";//进攻
			break;
		case 3:
			m_bidDesLabel.text = "";
			m_tuiJian.spriteName = "";
			m_tuiJianLabel.text = "";
			break;
		default:
			break;
		}
	}

	void TexScale ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",1);
		scale.Add ("scale",Vector3.one);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("islocal",true);
		iTween.ScaleTo (m_texObj,scale);
	}

	void CityTween ()
	{
		m_tuiJian.transform.localPosition = new Vector3 (0,65,0);
		m_tuiJianPos = m_tuiJian.transform.localPosition;
		m_moveUp = true;
		
		TuiJianTween ();
	}

	void TuiJianTween ()
	{
		Hashtable move = new Hashtable ();
		move.Add ("position",m_moveUp ? m_tuiJianPos + new Vector3(0,15,0) : m_tuiJianPos);
		move.Add ("time",1);
		move.Add ("easetype",iTween.EaseType.easeInOutQuad);
		move.Add ("oncomplete","JianTouTweenBack");
		move.Add ("oncompletetarget",gameObject);
		move.Add ("islocal",true);
		iTween.MoveTo (m_tuiJian.gameObject,move);
	}
	
	void JianTouTweenBack ()
	{
		if (m_moveUp)
		{
			m_moveUp = false;
		}
		else
		{
			m_moveUp = true;
		}
		TuiJianTween ();
	}

	public void RefreshRecState (int cityId,BidRecord bidRecord = null)
	{
		int type = JCZCityTemplate.GetJCZCityTemplateById (cityId).type; 
		switch (type)
		{
		case 1:		//名城
			for (int i = 0;i < m_cityList.Count;i ++)
			{
				CityItem cityItem = m_cityList[i].GetComponent<CityItem> ();
				if (cityItem.M_CityInfo.cityId == cityId)
				{
					CityResp.cityList[i].cityState2 = 1;
					cityItem.InItCityItem (CityResp.cityList[i]);
				}
			}
			M_HaveNormalCity = true;
			if (CityResp.recCityId != 0)
			{
				CityResp.recCityId = -1;
			}
			CityWarBidPage.m_instance.RefreshBidRecord (bidRecord);
			break;
		case 2:		//野城
			CityResp.haveHufu -= JCZCityTemplate.GetJCZCityTemplateById (cityId).cost;
			CityResp.recCityId = 0;
			m_bidDesLabel.text = "";
			OtherCity.m_instance.RefreshOtherCity (cityId);
			break;
		default:
			break;
		}

		InItTuiJianState ();
	}

	#region Information
	public UILabel m_infoLabel;
	private List<UILabel> m_infoLabelList = new List<UILabel> ();
	private List<string> m_infoList = new List<string> ();

	public void InItInfoLabel (BidMsgInfo tempInfo)
	{
		string cityName = NameIdTemplate.GetName_By_NameId (JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId).name);
		string bidTime = tempInfo.bidTime.ToString ();
		string info = MyColorData.getColorString (5,tempInfo.allianceName) + "对[e5e205]" + cityName + "[-]" + m_xuanZhan;//+ "  " + QXComData.UTCToTimeString (tempInfo.bidTime * 1000,"HH:mm");

		if (m_infoList.Contains (info))
		{
			return;
		}

		GameObject infoLabelObj = (GameObject)Instantiate (m_infoLabel.gameObject);
		UILabel infoLabel = infoLabelObj.GetComponent<UILabel> ();
		infoLabel.text = info;

		m_infoList.Add (info);
		m_infoLabelList.Add (infoLabel);
		
		infoLabelObj.transform.parent = m_infoLabel.transform.parent;
		infoLabelObj.transform.localScale = Vector3.one;
		if (m_infoLabelList.Count < 4)
		{
			infoLabelObj.SetActive (true);
			infoLabelObj.transform.localPosition = new Vector3(115,40 - m_infoLabelList.Count * 20,0);
		}
		else
		{
			GameObject firstLabelObj = m_infoLabelList[0].gameObject;
			m_infoLabelList.RemoveAt (0);
			m_infoList.RemoveAt (0);
			CityInfoLabel cityInfoLabel = firstLabelObj.GetComponent<CityInfoLabel> ();
			cityInfoLabel.MoveLabel (new Vector3 (115,40,0),true);
			
			for (int i = 0;i < m_infoLabelList.Count;i ++)
			{
				CityInfoLabel Label = m_infoLabelList[i].GetComponent<CityInfoLabel> ();
				Label.MoveLabel (new Vector3 (115,20 - i * 20,0),false);

				m_infoLabelList[i].gameObject.SetActive (true);
			}
		}
	}
	
	#endregion

	#region RewardInfo

	public GameObject m_rewardPageObj;

	public void OpenReward (CityWarData.CW_RewardType tempType,CityWarRewardResp tempResp)
	{
		m_rewardPageObj.SetActive (true);
		CityWarReward.m_instance.InItCityWarReward (tempType,tempResp);
		CityWarReward.m_instance.M_CWRewardDelegate = CityWarRewardDelegateCallBack;
	}

	void CityWarRewardDelegateCallBack ()
	{
		m_rewardPageObj.SetActive (false);
	}

	#endregion

	#region OtherCity

	public GameObject m_otherCityObj;

	public void OpenOtherCity (CityFightInfoResp tempResp)
	{
		m_otherCityObj.SetActive (true);
		OtherCity.m_instance.InItOtherCity (tempResp);
		OtherCity.m_instance.M_OtherCityDelegate = OtherCityDelegateCallBack;
	}

	void OtherCityDelegateCallBack ()
	{
		m_otherCityObj.SetActive (false);
	}

	#endregion

	#region JiFen

	public void OpenJiFenPage (OtherCity.PageType m_pageType,CityWarScoreResultResp tempResp)
	{
		m_otherCityObj.SetActive (true);
		OtherCity.m_instance.InItJiFenPage (m_pageType,tempResp);
		switch (m_pageType)
		{
		case OtherCity.PageType.JIFEN_BID:
			BidPageDelegateCallBack ();
			OtherCity.m_instance.M_OtherCityDelegate = BidJiFenPageCallBack;
			break;
		case OtherCity.PageType.JIFEN_OTHER_CITY:
			OtherCity.m_instance.M_OtherCityDelegate = OtherCityDelegateCallBack;
			break;
		default:
			break;
		}
	}

	void BidJiFenPageCallBack ()
	{
		m_otherCityObj.SetActive (false);
		m_bidObj.SetActive (true);
	}

	#endregion

	#region CityWarBrand

	public GameObject m_brandPageObj;

	public void OpenBrand (CityWarGrandResp tempResp)
	{
		m_brandPageObj.SetActive (true);
		CityWarBrand.m_instance.InItBrandPage (tempResp);
		CityWarBrand.m_instance.M_CityWarBrandDelegate = CityWarBrandDelegateCallBack;
	}

	void CityWarBrandDelegateCallBack ()
	{
		m_brandPageObj.SetActive (false);
	}

	#endregion

	#region CityWarBid

	public GameObject m_bidObj;

	public void OpenBid (CityWarBidResp tempResp,CityInfo tempInfo)
	{
		m_bidObj.SetActive (true);
		CityWarBidPage.m_instance.InItBidPage (tempResp,tempInfo);
		CityWarBidPage.m_instance.M_BidDelegate = BidPageDelegateCallBack;
	}

	void BidPageDelegateCallBack ()
	{
		m_bidObj.SetActive (false);
	}

	#endregion

	#region RefreshRed

	public void SetRewardRed ()
	{
		m_rewardRed.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (310410) || FunctionOpenTemp.IsShowRedSpotNotification (310420) ? true : false);
//		NewAlliancemanager.Instance ().ShowJunChengZhanAlert ();
		CityWarData.Instance.SetAllianceRed ();
	}

	#endregion

	IEnumerator RefreshPage ()
	{
		while (CityResp.countDown > 0)
		{
			CityResp.countDown --;

			yield return new WaitForSeconds (1);

			if (CityResp.countDown <= 0)
			{
				CityWarData.Instance.OpenCityWar (CityWarData.CityWarType.NORMAL_CITY);
			}
		}
	}

	void Update ()
	{
		if (m_addAlpha)
		{
			if (m_curTimeSprite.alpha < 1)
			{
				m_curTimeSprite.alpha += 0.05f;
				if (m_curTimeSprite.alpha >= 1)
				{
					m_curTimeSprite.alpha = 1;
					m_addAlpha = false;
				}
			}
			else
			{
				m_addAlpha = false;
			}
		}
		else
		{
			if (m_curTimeSprite.alpha > 0)
			{
				m_curTimeSprite.alpha -= 0.05f;
				if (m_curTimeSprite.alpha <= 0)
				{
					m_curTimeSprite.alpha = 0;
					m_addAlpha = true;
				}
			}
			else
			{
				m_addAlpha = true;
			}
		}
	}

//	private int index = 0;
	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "RewardBtn":

//			BidMsgInfo info = new BidMsgInfo();
//			info.allianceName = "测试" + index;
//			info.cityId = 510103;
//			info.bidTime = System.DateTime.Now.Millisecond;
//			InItInfoLabel (info);
//			index ++;
//			Debug.Log (System.DateTime.UtcNow.Millisecond);
//			return;
			CityWarData.Instance.RewardReq (CityWarData.CW_RewardType.ALLIANCE);

			break;
		case "GrandBtn":

			CityWarData.Instance.BrandReq (1);

			break;
		case "OtherCityBtn":

			CityWarData.Instance.OpenCityWar (CityWarData.CityWarType.OTHER_CITY);

			break;
		case "CloseBtn":

			if (M_CityWarDelegate != null)
			{
				M_CityWarDelegate ();
				m_isOpenFirst = true;
			}

			break;
		case "MoveBtn":

			VideoHelper.PlayVideo (EffectIdTemplate.GetPathByeffectId (700003),VideoHelper.VideoControlMode.None,ClickMove);

			break;
		default:

			CityItem cityItem = ui.transform.parent.GetComponent<CityItem> ();
			if (cityItem.M_CityInfo != null)
			{
				CityWarData.Instance.BidReq (cityItem.M_CityInfo);
			}

			break;
		}
	}

	void ClickMove ()
	{

	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

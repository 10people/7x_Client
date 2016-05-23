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
		//0 - 郡城战状态-起 1 - 郡城战状态-终 2 - 时段name 3 - 城池状态spriteName
		{0,new string[]{"declaration_startTime","declaration_endTime","宣战时段","YellowPoint"}},
		{1,new string[]{"preparation_startTime","preparation_endTime","揭晓时段","GreenPoint"}},
		{2,new string[]{"fighting_startTime","fighting_endTime","战斗时段","RedPoint"}},
		{3,new string[]{"","","其它时段",""}},
	};

	private bool m_isOpenFirst = true;
	public GameObject m_texObj;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItCityWarPage (CityFightInfoResp tempResp)
	{
		CityResp = tempResp;
		M_HaveNormalCity = false;

		for (int i = 0;i < m_timeLabelList.Count;i ++)
		{
			m_timeLabelList[i].text = M_TimeLabelDic[i][2] + JCZTemplate.GetJCZTemplateByKey (M_TimeLabelDic[i][0]).value + "~" + JCZTemplate.GetJCZTemplateByKey (M_TimeLabelDic[i][1]).value;
		}

		if (tempResp.interval == 3)
		{
			m_curTimeSprite.gameObject.SetActive (false);
		}
		else
		{
			m_curTimeSprite.gameObject.SetActive (true);
			m_curTimeSprite.transform.localPosition = m_timeLabelList [tempResp.interval].transform.parent.transform.localPosition - new Vector3(5,0,0);
			m_curTimeSprite.alpha = 1;
		}

		m_allianceName.text = QXComData.AllianceInfo ().name;
		m_allianceLevel.text = "等级[0dbce8]  " + QXComData.AllianceInfo ().level.ToString () + "[-]";
		m_allianceIcon.spriteName = QXComData.AllianceInfo ().icon.ToString ();
		m_earth.text = "领地[0dbce8]  " + CityResp.myCityCount + "[-]";

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
		Debug.Log ("M_HaveNormalCity:" + M_HaveNormalCity);
		InItTuiJianState ();

		m_bidDesLabel.text = tempResp.interval == 1 ? (tempResp.recCityId > 0 ? MyColorData.getColorString (45,"此时段还可宣战野城") : "") : "";

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
			m_texObj.transform.localScale = Vector3.one * 0.9f;
			m_texObj.transform.localPosition = new Vector3 (20,0,0);
			TexScale ();
		}
	}

	void InItTuiJianState ()
	{
		CityTween ();
	
		Debug.Log ("recCityId:" + CityResp.recCityId);
		switch (CityResp.interval)
		{
		case 0:
			if (CityResp.recCityId == 0)
			{
				m_tuiJian.spriteName = "BlueArrow";
				m_tuiJianLabel.text = "[0dbce8]宣战[-]";
			}
			else
			{
				m_tuiJian.spriteName = CityResp.recCityId > 0 ? "GreenArrow" : "";
				m_tuiJianLabel.text = CityResp.recCityId > 0 ? MyColorData.getColorString (4,"推荐") : "";
			}
			break;
		case 1:
			if (CityResp.recCityId == 0)
			{
				m_tuiJian.spriteName = "RedArrow";
				m_tuiJianLabel.text = MyColorData.getColorString (5,"进攻");
			}
			else if (CityResp.recCityId > 0)
			{
				m_tuiJian.spriteName = "GreenArrow";
				m_tuiJianLabel.text = MyColorData.getColorString (4,"推荐");
			}
			else
			{
				m_tuiJian.spriteName = "";
				m_tuiJianLabel.text = "";
			}
			break;
		case 2:
			m_tuiJian.spriteName = CityResp.recCityId == 0 ? "RedArrow" : "";
			m_tuiJianLabel.text = CityResp.recCityId == 0 ? MyColorData.getColorString (5,"进攻") : "";
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
		string info = MyColorData.getColorString (5,tempInfo.allianceName) + "对[e5e205]" + cityName + "[-]宣战  " + QXComData.UTCToTimeString (tempInfo.bidTime * 1000,"HH:mm");

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
		default:

			CityItem cityItem = ui.transform.parent.GetComponent<CityItem> ();
			if (cityItem.M_CityInfo != null)
			{
				CityWarData.Instance.BidReq (cityItem.M_CityInfo);
			}

			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

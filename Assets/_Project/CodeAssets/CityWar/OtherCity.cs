using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class OtherCity : GeneralInstance<OtherCity> {

	public enum PageType
	{
		OTHER_CITY,
		JIFEN_OTHER_CITY,
		JIFEN_BID,
	}
	private PageType m_pageType;

	public delegate void OtherCityDelegate ();
	public OtherCityDelegate M_OtherCityDelegate;

	public UILabel m_title;

	new void Awake ()
	{
		base.Awake ();
	}

	void InItPageType (PageType tempType)
	{
		m_pageType = tempType;
		m_otherCityBtn.SetActive (tempType == PageType.JIFEN_BID ? false : true);
		m_jiFenBtn.SetActive (tempType == PageType.JIFEN_BID ? false : true);
		m_otherCityObj.SetActive (tempType == PageType.OTHER_CITY ? true : false);
		m_jiFenObj.SetActive (tempType != PageType.OTHER_CITY ? true : false);

		m_title.text = "[b]" + (tempType ==  PageType.OTHER_CITY ? "义渠野战" : "积分战况") + "[-]";

		if (tempType != PageType.JIFEN_BID)
		{
			QXComData.SetBtnState (m_otherCityBtn,tempType == PageType.OTHER_CITY ? true : false);
			QXComData.SetBtnState (m_jiFenBtn,tempType == PageType.JIFEN_OTHER_CITY ? true : false);
		}
	}

	#region OtherCity
	private CityFightInfoResp m_OtherCityResp;

	public GameObject m_otherCityObj;

	public GameObject m_otherCityBtn;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;

	public GameObject m_otherCity;
	private List<GameObject> m_otherCityList = new List<GameObject> ();

	public UILabel m_rule;

	private OtherCityItem m_otherCityItem;

	private int curCityId;		//当前挑战难度郡城id
	private int curXuanZhanId;		//当前宣战城池id
	
	public void InItOtherCity (CityFightInfoResp tempResp)
	{
		m_OtherCityResp = tempResp;

		CityWarPage.m_instance.CityResp.haveHufu = tempResp.haveHufu;

		InItPageType (PageType.OTHER_CITY);

		curCityId = 0;
		curXuanZhanId = 0;
		for (int i = 0;i < tempResp.cityList.Count;i ++)
		{
			if (tempResp.cityList[i].cityState == 0)
			{
				curCityId = curCityId == 0 ? tempResp.cityList[i].cityId : curCityId;
			}
			if (tempResp.cityList[i].cityState2 != 0)
			{
				curXuanZhanId = tempResp.cityList[i].cityId;
			}
		}

		m_otherCityList = QXComData.CreateGameObjectList (m_otherCity,tempResp.cityList.Count,m_otherCityList);

		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
			m_otherCityList[i].transform.localPosition = new Vector3(i * 150,0,0);
			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			cityItem.InItOtherCity (tempResp.cityList[i],curCityId,curXuanZhanId);
		}

		m_sc.UpdateScrollbars (true);

		SetPos ();

		m_sc.enabled = m_otherCityList.Count < 4 ? false : true;
//		m_sb.gameObject.SetActive (false);
		m_rule.text = QXComData.yellow + LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_14) + "[-]";
	}

	void SetPos ()
	{
		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			if (cityItem.M_CityInfo.cityState2 == 1)
			{
				UIWidget widget = m_otherCityList[i].GetComponent<UIWidget>();
				
				float widgetValue = m_sc.GetWidgetValueRelativeToScrollView (widget).x;
				if (widgetValue < 0 || widgetValue > 1)
				{
					m_sc.SetWidgetValueRelativeToScrollView(widget, 0);
					float scrollValue = m_sc.GetSingleScrollViewValue();
					if (scrollValue >= 1) m_sb.value = 0.99f;
					if (scrollValue <= 0) m_sb.value = 0.01f;
				}

				break;
			}
		}
	}

	public void RefreshOtherCity (int cityId)
	{
		int xuanZhanId = cityId;
		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			if (cityId == cityItem.M_CityInfo.cityId)
			{
				cityItem.M_CityInfo.cityState2 = 1;
				cityItem.InItOtherCity (cityItem.M_CityInfo,curCityId,xuanZhanId);
			}
			else
			{
				cityItem.InItOtherCity (cityItem.M_CityInfo,curCityId,xuanZhanId);
			}
		}
	}

	#endregion

	#region JiFen

	public enum JiFenType
	{
		ATTACK = 2,
		DEFENSE = 1,
	}

	private JiFenType m_jiFenType;

	private CityWarScoreResultResp m_scoreResp;

	private List<ScoreInfo> m_attackList = new List<ScoreInfo>();
	private List<ScoreInfo> m_defenseList = new List<ScoreInfo>();

	public GameObject m_jiFenObj;
	public GameObject m_jiFenBtn;

	public UIScrollView m_jiFenSc;
	public UIScrollBar m_jiFenSb;

	public GameObject m_jiFen;
	private List<GameObject> m_jiFenList = new List<GameObject>();

	public UILabel m_jiFenDes;
	public UILabel m_numDes;

	private GameObject m_tempObject;
	private GameObject m_floatBtnObj;

	public GameObject m_attackBtn;
	public GameObject m_defenseBtn;

	public GameObject m_select;

	public GameObject m_floatPanel;

	private long m_jzId;

	public void InItJiFenPage (PageType tempType,CityWarScoreResultResp tempResp)
	{
		m_scoreResp = tempResp;

		InItPageType (tempType);

		m_attackList.Clear ();
		m_defenseList.Clear ();

		for (int i = 0;i < tempResp.scoreList.Count;i ++)
		{
			if (tempResp.scoreList[i].side == 2)
			{
				m_attackList.Add (tempResp.scoreList[i]);
			}
			else
			{
				m_defenseList.Add (tempResp.scoreList[i]);
			}
		}

		JiFenType defaultType = JiFenType.ATTACK;
		for (int i = 0;i < tempResp.scoreList.Count;i ++)
		{
			if (QXComData.JunZhuInfo ().id == tempResp.scoreList[i].jzId)
			{
				defaultType = tempResp.scoreList[i].side == 1 ? JiFenType.DEFENSE : JiFenType.ATTACK;
				break;
			}
		}

		ChangeJiFenPage (defaultType,defaultType == JiFenType.ATTACK ? m_attackList : m_defenseList);

		string ruleStr = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_49).Replace ("**","$"); 
		string[] ruleStrLen = ruleStr.Split ('$');
		ruleStrLen[0] = QXComData.UTCToTimeString (tempResp.date * 1000,"MM-dd");
		string[] dateLen = ruleStrLen[0].Split ('-');
		ruleStrLen[0] = "[cb02d8]" + dateLen[0] + "月" + dateLen[1] + "日[-]";
		ruleStr = ruleStrLen[0] + QXComData.yellow + tempResp.cityName + "[-]" + ruleStrLen[1];

		bool isHaveData =  tempResp.scoreList.Count > 0 ? true : false;
		m_jiFenDes.text = isHaveData ? ruleStr : LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_48);

//		m_attackBtn.SetActive (isHaveData);
//		m_defenseBtn.SetActive (isHaveData);

//		m_jiFenDes.transform.localPosition = new Vector3(isHaveData ? -105 : 0,-170,0);

		if (m_floatBtnObj == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
		}
	}

	void FloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		m_floatBtnObj = p_object as GameObject;
	}

	void ChangeJiFenPage (JiFenType tempType,List<ScoreInfo> tempList)
	{
		m_jiFenType = tempType;

		m_select.transform.parent = tempType == JiFenType.ATTACK ? m_attackBtn.transform : m_defenseBtn.transform;
		m_select.transform.localPosition = Vector3.zero;
		m_select.transform.localScale = Vector3.one;

//		QXComData.SetBtnState (m_attackBtn,tempType == JiFenType.ATTACK ? true : false);
		QXComData.SetBtnState (m_defenseBtn,m_scoreResp.isNpc == 0 ? true : false);

		for (int i = 0;i < tempList.Count - 1;i ++)
		{
			for (int j = 0;j < tempList.Count - i - 1;j ++)
			{
				if (tempList[j].rank > tempList[j + 1].rank)
				{
					ScoreInfo tempInfo = tempList[j];
					tempList[j] = tempList[j + 1];
					tempList[j + 1] = tempInfo;
				}
			}
		}
		
		m_jiFenList = QXComData.CreateGameObjectList (m_jiFen,tempList.Count,m_jiFenList);
		for (int i = 0;i < m_jiFenList.Count;i ++)
		{
			m_jiFenList[i].transform.localPosition = new Vector3(0,-i * 48,0);
			m_jiFenSc.UpdateScrollbars (true);
			CWScoreItem scoreItem = m_jiFenList[i].GetComponent<CWScoreItem> ();
			scoreItem.InItScore (tempList[i]);
		}
		
		m_jiFenSc.enabled = m_jiFenList.Count > 5 ? true : false;
		m_jiFenSb.gameObject.SetActive (m_jiFenList.Count > 5 ? true : false);
		
		m_numDes.text = m_jiFenList.Count > 0 ? "" : LanguageTemplate.GetText (tempType == JiFenType.ATTACK ? LanguageTemplate.Text.JUNCHENGZHAN_JIFEN_1 : LanguageTemplate.Text.JUNCHENGZHAN_JIFEN_2);
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		if (m_tempObject != null)
		{
			Destroy (m_tempObject);
		}
		else
		{
			m_jzId = -1;
		}
		switch (ui.name)
		{
		case "OtherCityBtn":
			if (m_pageType != PageType.OTHER_CITY)
			{
				InItPageType (PageType.OTHER_CITY);
			}
			break;
		case "JiFenBtn":
			if (m_pageType != PageType.JIFEN_OTHER_CITY)
			{
//				InItJiFenPage (PageType.JIFEN_OTHER_CITY,m_scoreResp);
				CityWarData.Instance.CityWarJiFenReq (PageType.JIFEN_OTHER_CITY,-1);
			}
			break;
		case "ZheZhao":

			if (M_OtherCityDelegate != null)
			{
				M_OtherCityDelegate ();
				M_OtherCityDelegate = null;
				m_otherCityBtn.SetActive (false);
				m_jiFenBtn.SetActive (false);
			}

			break;

		case "AttackBtn":

			if (m_jiFenType != JiFenType.ATTACK)
			{
				ChangeJiFenPage (JiFenType.ATTACK,m_attackList);
			}

			break;

		case "DefenseBtn":

			if (m_jiFenType != JiFenType.DEFENSE && m_scoreResp.isNpc == 0)
			{
				ChangeJiFenPage (JiFenType.DEFENSE,m_defenseList);
			}

			break;

		case "EnterFightBtn":

			OtherCityItem cityItem = ui.transform.parent.GetComponent<OtherCityItem> ();
			if (cityItem.M_CityInfo != null)
			{
				m_otherCityItem = cityItem;
				if (cityItem.M_canJoin)
				{
					if (cityItem.M_CityInfo.cityState2 == 1)
					{
						CityWarOperateReq operateReq = new CityWarOperateReq();
						operateReq.operateType = CityOperateType.ENTER_FIGHT;
						operateReq.cityId = cityItem.M_CityInfo.cityId;
						CityWarData.Instance.CityOperate (operateReq);
					}
					else
					{
						int m_identity = QXComData.AllianceInfo ().identity;//0-成员 1-副门主 2-盟主
						if (m_identity == 0)
						{
							CityWarOperateReq operateReq = new CityWarOperateReq();
							operateReq.operateType = CityOperateType.BID;
							operateReq.cityId = m_otherCityItem.M_CityInfo.cityId;
							operateReq.price = m_otherCityItem.M_Template.cost;
							CityWarData.Instance.CityOperate (operateReq);
						}
						else
						{
	//						string text = "您的联盟拥有虎符" + MyColorData.getColorString (4,CityWarPage.m_instance.CityResp.haveHufu.ToString ()) + "\n是否花费[e5e205]" + JCZCityTemplate.GetJCZCityTemplateById (cityItem.M_CityInfo.cityId).cost + "[-]虎符对该城池宣战？";
							string text = LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_15).Replace ("N",MyColorData.getColorString (4,CityWarPage.m_instance.CityResp.haveHufu.ToString ()));
							text = text.Replace ("XX","[e5e205]" + cityItem.m_cost + "[-]");
							QXComData.CreateBoxDiy (text,false,OtherCityBid);
						}
					}
				}
			}

			break;
		default:

			CWScoreItem scoreItem = ui.GetComponent<CWScoreItem> ();
//			Debug.Log ("scoreItem:" + scoreItem.m_scoreInfo.jzId);
			if (scoreItem != null)
			{
				if (scoreItem.m_scoreInfo.jzId != m_jzId)
				{
					m_jzId = scoreItem.m_scoreInfo.jzId;
					m_tempObject = (GameObject)Instantiate(m_floatBtnObj);
					FloatButtonsController floatBtn = m_tempObject.GetComponent<FloatButtonsController>();
					floatBtn.Initialize(FloatButtonsConfig.GetConfig(scoreItem.m_scoreInfo.jzId, scoreItem.m_scoreInfo.jzName, scoreItem.m_scoreInfo.lmName, new List<GameObject> (){}, ClampScrollView), true);
//					TransformHelper.ActiveWithStandardize(scoreItem.transform.parent.transform, m_tempObject.transform);
//					m_tempObject.transform.parent = scoreItem.transform.parent.transform;
					m_tempObject.transform.parent = m_floatPanel.transform;
					Vector3 pos = scoreItem.gameObject.transform.localPosition + new Vector3(0,120,0) + m_jiFenSc.transform.localPosition;
					m_tempObject.transform.localPosition = pos + new Vector3(95,3,0);
					m_tempObject.transform.localScale = Vector3.one * 0.6f;

//					UISprite widget = scoreItem.GetComponentInChildren<UISprite>();
//					float widgetValue = m_jiFenSc.GetWidgetValueRelativeToScrollView (widget).y;
//					m_jiFenSc.SetWidgetValueRelativeToScrollView(widget, 0);
					
					//clamp scroll bar value.
					//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
					//set 0.99 and 0.01 cause same bar value not taken in execute.
//					float scrollValue = m_jiFenSc.GetSingleScrollViewValue();
//					if (scrollValue >= 1) m_jiFenSb.value = 0.99f;
//					if (scrollValue <= 0) m_jiFenSb.value = 0.01f;
				}
				else
				{
					m_jzId = -1;
//					Debug.Log ("m_jiFenSb.value1:" + m_jiFenSb.value);
//					if (m_jiFenSb.value >= 0.99f)
//					{
//						SpringPanel sp = m_jiFenSc.GetComponent<SpringPanel>();
//						if (sp == null)
//						{
//							sp = m_jiFenSc.gameObject.AddComponent<SpringPanel> ();
//						}
//						sp.target = new Vector3(0,-38,0);
//						sp.enabled = true;
//						//							if (sp != null && m_jiFenSc.transform.localPosition.y > sp.target.y ) sp.enabled = true;
//						m_jiFenSc.UpdateScrollbars (true);
//					}
				}
			}
			else
			{
				m_jzId = -1;
//				Debug.Log ("m_jiFenSb.value2:" + m_jiFenSb.value);
//				if (m_jiFenSb.value >= 0.99f)
//				{
//					SpringPanel sp = m_jiFenSc.GetComponent<SpringPanel>();
//					if (sp == null)
//					{
//						sp = m_jiFenSc.gameObject.AddComponent<SpringPanel> ();
//					}
//					sp.target = new Vector3(0,-38,0);
//					sp.enabled = true;
////					if (sp != null && m_jiFenSc.transform.localPosition.y > sp.target.y ) sp.enabled = true;
//					m_jiFenSc.UpdateScrollbars (true);
//				}
			}

			break;
		}
	}

	void ClampScrollView()
	{
		//clamp scroll bar value.
		//set 0.99 and 0.01 cause same bar value not taken in execute.
		StartCoroutine(DoClampScrollView());
	}
	
	IEnumerator DoClampScrollView()
	{
		yield return new WaitForEndOfFrame();
		
		m_jiFenSc.UpdateScrollbars(true);
		float scrollValue = m_jiFenSc.GetSingleScrollViewValue();
		if (scrollValue >= 1) m_jiFenSb.value = 0.99f;
		if (scrollValue <= 0) m_jiFenSb.value = 0.01f;
	}

	void OtherCityBid (int i)
	{
		if (i == 2)
		{
			CityWarOperateReq operateReq = new CityWarOperateReq();
			operateReq.operateType = CityOperateType.BID;
			operateReq.cityId = m_otherCityItem.M_CityInfo.cityId;
			operateReq.price = m_otherCityItem.M_Template.cost;
			CityWarData.Instance.CityOperate (operateReq);
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

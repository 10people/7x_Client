using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarBidPage : GeneralInstance<CityWarBidPage> {

	public delegate void BidDelegate ();
	public BidDelegate M_BidDelegate;

	public CityWarBidResp BidResp;
	private CityInfo m_cityInfo;

	public UILabel m_title;
	public UILabel m_speed;
	public UILabel m_general;
	public UILabel m_huFuNum;
	public GameObject m_select;
	public GameObject m_bidBtn;

	public UILabel m_enterNum;

	public GameObject m_checkSprite;

	public UILabel m_desLabel;

	public UISprite m_bidBtnSprite;
	public UILabel m_bidBtnLabel;
	private readonly Dictionary<int,string> m_bidLabelDic = new Dictionary<int, string>()
	{
		{0,"宣战竞拍"},{1,"加  价"},{2,"宣战成功"},{3,"竞拍失败"},
	};
	private readonly string[] m_btnSpriteLen = new string[]{"SmallYellowBtn","btn_yellow_219x74"};

	public List<GameObject> m_huFuList = new List<GameObject> ();
	private readonly Dictionary<int,int> m_huFuDic = new Dictionary<int, int> ()
	{
		{0,1},{1,5},{2,10},
	};

	private int m_identity;

	private int m_defaultIndex = 0;

	private bool m_moveEnd = true;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItBidPage (CityWarBidResp tempResp,CityInfo tempInfo)
	{
		BidResp = tempResp;
		m_cityInfo = tempInfo;
		m_showAddEffect = false;

		m_identity = QXComData.AllianceInfo ().identity;//0-成员 1-副门主 2-盟主

		JCZCityTemplate jczCityTemp = JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId);

		string cityName = NameIdTemplate.GetName_By_NameId (jczCityTemp.name);
		string cityLevel = "Lv" + jczCityTemp.allianceLv;
		m_title.text = cityName + "  " + cityLevel;

		m_speed.text = "资源产出：[cd02d8]" + jczCityTemp.awardShow + "功勋/天[-]";

		string generalStr = "";
		switch (tempInfo.cityState)
		{
		case 0://npc
			generalStr = jczCityTemp.NPCname;//"NPC"
			break;
		case 1://己方
			generalStr = tempResp.general + "<" + tempResp.allianceName + ">";
			break;
		case 2://敌人
			generalStr = tempResp.general + "<" + tempResp.allianceName + ">";
			break;
		default:
			break;
		}

		m_general.text = "镇守大将：" + MyColorData.getColorString (5,generalStr);

		m_huFuNum.text = m_identity != 0 ? MyColorData.getColorString (1,"联盟拥有数量：") + MyColorData.getColorString (CityWarPage.m_instance.CityResp.haveHufu == 0 ? 5 : 4,CityWarPage.m_instance.CityResp.haveHufu.ToString ()) : "";

		m_bidBtnLabel.text = m_bidLabelDic[tempResp.bidState];

		m_enterNum.text = CityWarPage.m_instance.CityResp.interval == 3 ? (tempResp.lmNum > 0 ? MyColorData.getColorString (4,tempResp.lmNum.ToString ()) + "个盟友正在作战" : "") : "";

		DesStrState ();

		ShowSelect (m_defaultIndex);

		InItRecord ();

		StartCoroutine ("RefreshBidPage");
	}

	void DesStrState ()
	{
		string desStr = "";
		string colorCode = "";
		int btnSpriteIndex = 0;
		switch (BidResp.bidState)
		{
		case 0:
			btnSpriteIndex = 0;
			colorCode = "[e5e205]";
			desStr = "使用虎符出价竞拍，\n出价最高联盟获得本城的攻打权";
			break;
		case 1:
			btnSpriteIndex = 0;
			colorCode = "[e5e205]";
			desStr = "每日[d80202]18[-]点揭晓结果，\n竞拍失败全额返还虎符";
			break;
		case 2:
			btnSpriteIndex = 0;
			colorCode = "[e5e205]";
			desStr = "[d80202]21:00[-]准时开战！\n不要忘记集结盟友哦！";
			break;
		case 3:
			btnSpriteIndex = 1;
			colorCode = "[d80202]";
			desStr = "虎符已全额返还";
			break;
		default:
			break;
		}
		m_bidBtnSprite.spriteName = m_btnSpriteLen[btnSpriteIndex];
		m_desLabel.text = colorCode + desStr + "[-]";
	}

	void ShowSelect (int index)
	{
		m_select.transform.localPosition = m_huFuList [index].transform.localPosition;
	}

	#region BidRecord
	public UIWidget m_bidRecordWidget;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;
	
	public GameObject m_recordObj;
	private List<GameObject> m_recordList = new List<GameObject>();

	public UILabel m_noRecordDes;

	public UILabel m_addLabel;
	private int m_addNum;
	private bool m_showAddEffect = false;	//是否显示飘字

	private string m_winName;

	private bool m_isOpenBidRecord = false;

	public void InItRecord ()
	{
		m_showAddEffect = false;
		
		m_winName = BidResp.recordList.Count > 0 ? BidResp.recordList[0].allianceName : "";

		CreateRecordList ();
	}

	void CreateRecordList ()
	{
		m_recordList = QXComData.CreateGameObjectList (m_recordObj,BidResp.recordList.Count,m_recordList);

		for (int i = 0;i < BidResp.recordList.Count - 1;i ++)
		{
			for (int j = 0;j < BidResp.recordList.Count - i - 1;j ++)
			{
				if (BidResp.recordList[j].time < BidResp.recordList[j + 1].time)
				{
					BidRecord tempRecord = BidResp.recordList[j];
					BidResp.recordList[j] = BidResp.recordList[j + 1];
					BidResp.recordList[j + 1] = tempRecord;
				}
			}
		}

		GameObject m_obj = null;

		for (int i = 0;i < m_recordList.Count;i ++)
		{
			m_recordList[i].transform.localPosition = new Vector3(0,-40 * i,0);
			
			m_sc.UpdateScrollbars (true);
			
			CWBidRecordItem bidRecord = m_recordList[i].GetComponent<CWBidRecordItem> ();
			
			bidRecord.InItRecordItem (BidResp.recordList[i],m_winName);

			if (BidResp.recordList[i].allianceName == QXComData.AllianceInfo ().name)
			{
				m_obj = m_recordList[i];
			}
		}

		if (m_obj != null)
		{
			if (m_recordList.Count > 8)
			{
				QXComData.SetWidget (m_sc,m_sb,m_obj);
			}
			if (m_showAddEffect)
			{
				AddLabelUp (m_obj.transform.localPosition + new Vector3(50,140,0),m_obj.transform.localPosition + new Vector3(50,155,0));
			}
		}

		m_sc.enabled = m_recordList.Count > 8 ? true : false;
		m_sb.gameObject.SetActive (m_recordList.Count > 8 ? true : false);
		
		m_noRecordDes.text = m_recordList.Count > 0 ? "" : "暂无联盟宣战";
	}

	public void RefreshBidRecord (BidRecord tempRecord)
	{
		Debug.Log ("allianceName:" + tempRecord.allianceName);
		Debug.Log ("huFuNum:" + tempRecord.huFuNum);
		Debug.Log ("time:" + tempRecord.time);
		BidResp.bidState = 1;
		m_bidBtnLabel.text = m_bidLabelDic[BidResp.bidState];
		DesStrState ();

		for (int i = 0;i < BidResp.recordList.Count;i ++)
		{
			if (BidResp.recordList[i].allianceName == tempRecord.allianceName)
			{
				m_showAddEffect = true;
				m_addNum = tempRecord.huFuNum - BidResp.recordList[i].huFuNum;
				BidResp.recordList[i] = tempRecord;

				break;
			}
			else
			{
				m_showAddEffect = false;
			}
		}

		if (!m_showAddEffect)
		{
			BidResp.recordList.Add (tempRecord);
			CityWarPage.m_instance.CityResp.haveHufu -= tempRecord.huFuNum;
		}
		else
		{
			CityWarPage.m_instance.CityResp.haveHufu -= m_addNum;
		}

		m_huFuNum.text = MyColorData.getColorString (1,"联盟拥有数量：") + MyColorData.getColorString (CityWarPage.m_instance.CityResp.haveHufu <= 0 ? 5 : 4,CityWarPage.m_instance.CityResp.haveHufu.ToString ());

		CreateRecordList ();
		BidIteen (true);
	}

	#endregion

	#region TweenMove
	public GameObject m_bgObj;
	void BidIteen (bool open)
	{
		m_moveEnd = false;
		m_isOpenBidRecord = open;
		if (!open)
		{
			m_bidRecordWidget.gameObject.SetActive (false);
		}

		Hashtable move = new Hashtable ();
		move.Add ("time",0.5f);
		move.Add ("position",new Vector3(open ? 175 : 15,0,0));
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("islocal",true);
		move.Add ("oncomplete","MoveEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (m_bgObj,move);
	}

	void MoveEnd ()
	{
		if (m_isOpenBidRecord)
		{
			m_bidRecordWidget.gameObject.SetActive (true);
			m_checkSprite.transform.localRotation = Quaternion.Euler (0,0,-90);
		}
		else
		{
			m_checkSprite.transform.localRotation = Quaternion.Euler (0,0,90);
		}

		m_moveEnd = true;
	}

	void AddLabelUp (Vector3 form,Vector3 to)
	{
		m_addLabel.transform.localPosition = form;
		m_addLabel.alpha = 1;
		m_addLabel.text = MyColorData.getColorString (4,"+" + m_addNum);

		Hashtable move = new Hashtable ();
		move.Add ("time",0.5f);
		move.Add ("position",to);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("islocal",true);
		iTween.MoveTo (m_addLabel.gameObject,move);
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "BidBtn":
		{
			if(CityWarPage.m_instance.CityResp.interval == 0)
			{
				if (CityWarPage.m_instance.CityResp.recCityId > 0 && !CityWarPage.m_instance.M_HaveNormalCity)
				{
					BidReq ();
				}
				else
				{
					if (BidResp.bidState != 0)
					{
						BidReq ();
					}
					else
					{
						string text = "今日您已对其他城池宣战，是否为多线战斗做好了准备？";
						QXComData.CreateBoxDiy (text,false,BidBtnCallBack);
					}
				}
			}
			else
			{
				if (BidResp.bidState == 0 || BidResp.bidState == 1)
				{
					BidReq ();
				}
			}

			break;
		}
		case "EnterBtn":
		{
			CityWarOperateReq operateReq = new CityWarOperateReq();
			operateReq.operateType = CityOperateType.ENTER_FIGHT;
			operateReq.cityId = m_cityInfo.cityId;
			CityWarData.Instance.CityOperate (operateReq);

			break;
		}
		case "CheckBtn":

			if (m_isOpenBidRecord)
			{
				BidIteen (false);
			}
			else
			{
				if (CityWarPage.m_instance.CityResp.interval == 0)
				{
					if (m_identity != 0)
					{
						//查看竞拍记录
						BidIteen (true);
					}
					else
					{
						//普通盟员
						ClientMain.m_UITextManager.createText ("宣战时段只有盟主或副盟主才可查看！");
					}
				}
				else
				{
					//查看竞拍记录
					BidIteen (true);
				}
			}

			break;
		case "ZheZhao":

			if (M_BidDelegate != null && m_moveEnd)
			{
				m_defaultIndex = 0;
				m_isOpenBidRecord = false;
				m_checkSprite.transform.localRotation = Quaternion.Euler (0,0,90);
				m_bgObj.transform.localPosition = new Vector3(15,0,0);
				m_bidRecordWidget.gameObject.SetActive (false);
				M_BidDelegate ();
			}

			break;
		default:

			if (ui.name.IndexOf ("Add") != -1)
			{
				m_defaultIndex = int.Parse (ui.name.Substring (3,1));
				ShowSelect (int.Parse (ui.name.Substring (3,1)));
			}

			break;
		}
	}

	void BidBtnCallBack (int i)
	{
		if (i == 2)
		{
			BidReq ();
		}
	}

	void BidReq ()
	{
		CityWarOperateReq operateReq = new CityWarOperateReq();
		operateReq.operateType = CityOperateType.BID;
		operateReq.cityId = m_cityInfo.cityId;
		operateReq.price = m_huFuDic[m_defaultIndex];
		CityWarData.Instance.CityOperate (operateReq);
	}

	IEnumerator RefreshBidPage ()
	{
		while (BidResp.refreshTime > 0)
		{
			BidResp.refreshTime --;

			yield return new WaitForSeconds (1);

			if (BidResp.refreshTime <= 0)
			{
				CityWarData.Instance.BidReq (m_cityInfo);
			}
		}
	}

	void Update ()
	{
		if (m_addLabel.alpha > 0)
		{
			m_addLabel.alpha -= Time.deltaTime;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

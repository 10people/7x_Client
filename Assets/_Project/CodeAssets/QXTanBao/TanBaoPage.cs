using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TanBaoPage : MonoBehaviour {

	public static TanBaoPage tbPage;

	private ExploreInfoResp tanBaoResp;

	public List<EventHandler> tbBtnHandlerList = new List<EventHandler> ();

	public List<UILabel> labelList = new List<UILabel> ();
	private Dictionary<string,UILabel> labelDic = new Dictionary<string, UILabel> ();
	private readonly string[] labelName = new string[]
	{
		"Kd_Des","Kd_Single","Kd_Spend",
		"Kj_Des","Kj_Single","Kj_Spend"
	};

	private readonly Dictionary<string,int> costDic = new Dictionary<string, int>()
	{
		{"KdSingle",40011},
		{"KdSpend",40012},
		{"KjSingle",40001},
		{"KjSpend",40002}
	};

	public GameObject kdRedObj;
	public GameObject kjRedObj;

	public GameObject kdTenBtnObj;
	public GameObject kjTenBtnObj;

	private GameObject m_iconSample;
	private GameObject m_kdIconSample;
	private GameObject m_kjIconSample;

	private int kdCdTime;
	private int kjCdTime;

	public ScaleEffectController sEffectController;

	public GameObject anchorTopRight;

	private bool isOpenFirst = true;

	private string tenFunctionDes;
	private bool isTenTimesOpen = false;

	void Awake ()
	{
		tbPage = this;
	}

	void OnDestroy ()
	{
		tbPage = null;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (anchorTopRight);
	}

	/// <summary>
	/// Gets the TB info resp.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void GetTBInfoResp (ExploreInfoResp tempResp)
	{
		tanBaoResp = tempResp;

		if (isOpenFirst)
		{
			sEffectController.OnOpenWindowClick ();

			isOpenFirst = false;
		}

		if (labelDic.Count == 0)
		{
			for (int i = 0;i < labelList.Count;i ++)
			{
				labelDic.Add (labelName[i],labelList[i]);
			}
		}

		labelDic ["Kd_Spend"].text = TanBaoCost (costDic ["KdSpend"]).ToString ();
		labelDic ["Kj_Spend"].text = TanBaoCost (costDic ["KjSpend"]).ToString ();

		if (QXComData.CheckYinDaoOpenState (100160) || QXComData.CheckYinDaoOpenState (100257))
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100160,2);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100257,1);
		}
		else
		{
			UIYindao.m_UIYindao.CloseUI ();
		}

		if (m_iconSample == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),IconSampleLoadCallBack);
		}
		else
		{
			CreateIconSample (m_kdIconSample,kdTenBtnObj.transform.parent.gameObject,920001);
			CreateIconSample (m_kjIconSample,kjTenBtnObj.transform.parent.gameObject,920002);
		}

		InItKuangDong ();
		InItKuangJing ();

		tenFunctionDes = FunctionOpenTemp.GetTemplateById (1103).m_sNotOpenTips + "。";

		TenTimesBtn (kdTenBtnObj);
		TenTimesBtn (kjTenBtnObj);

		CheckTBRed ();

		foreach (EventHandler handler in tbBtnHandlerList)
		{
			handler.m_click_handler -= TBBtnHandlerCallBack;
			handler.m_click_handler += TBBtnHandlerCallBack;
		}

		//Set TreasureBoxUIBtn's Effect DisActive
		{
			if (TreasureCityUIBR.m_instance != null)
			{
				TreasureCityUIBR.m_instance.OpenWindow = true;
				TreasureCityUIBR.m_instance.ClearBtnEffect ();
			}
		}
	}

	void TenTimesBtn (GameObject obj)
	{
		isTenTimesOpen = FunctionOpenTemp.IsHaveID (1103);

		UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget> ();
		foreach (UIWidget widget in widgets)
		{
			widget.color = isTenTimesOpen ? Color.white : Color.grey;
		}
	}

	/// <summary>
	/// Ins it kuang dong.
	/// </summary>
	void InItKuangDong ()
	{
		kdCdTime = tanBaoResp.tongBiCd;
		StopCoroutine ("KuangDongCd");
		if (tanBaoResp.remainFreeTongBiCount > 0 && tanBaoResp.remainFreeTongBiCount <= tanBaoResp.allFreeTongBiCount)
		{
			if (kdCdTime <= 0)
			{
				labelDic["Kd_Des"].text = "本日免费次数" + tanBaoResp.remainFreeTongBiCount + "/" + tanBaoResp.allFreeTongBiCount;
			}
			else
			{
				StartCoroutine ("KuangDongCd");
			}
			labelDic["Kd_Single"].text = kdCdTime > 0 ? TanBaoCost (costDic["KdSingle"]) : "免费";
		}
		else
		{
			labelDic["Kd_Des"].text = "本日免费次数已用完";
			labelDic["Kd_Single"].text = TanBaoCost (costDic["KdSingle"]);
		}
	}

	IEnumerator KuangDongCd ()
	{
		while (kdCdTime > 0)
		{
			kdCdTime --;

			labelDic["Kd_Des"].text = MyColorData.getColorString (5,TimeHelper.GetUniformedTimeString (kdCdTime)) + "后免费";

			yield return new WaitForSeconds (1);

			if (kdCdTime <= 0)
			{
				TanBaoData.Instance.TanBaoInfoReq ();
			}
		}
	}

	/// <summary>
	/// Ins it kuang jing.
	/// </summary>
	void InItKuangJing ()
	{
		kjCdTime = tanBaoResp.yuanBaoCd;
		StopCoroutine ("KuangJingCd");
		if (tanBaoResp.yuanBaoCd > 0)
		{
			labelDic["Kj_Single"].text = TanBaoCost (costDic["KjSingle"]); 
			StartCoroutine ("KuangJingCd");
		}
		else
		{
			labelDic["Kj_Des"].text = "本次探宝免费";
			labelDic["Kj_Single"].text = "免费"; 
		}
	}

	IEnumerator KuangJingCd ()
	{
		while (kjCdTime > 0)
		{
			kjCdTime --;

			labelDic["Kj_Des"].text = MyColorData.getColorString (5,TimeHelper.GetUniformedTimeString (kjCdTime)) + "后免费";

			yield return new WaitForSeconds (1);

			if (kjCdTime <= 0)
			{
				TanBaoData.Instance.TanBaoInfoReq ();
			}
		}
	}

	void IconSampleLoadCallBack (ref WWW p_www, string p_path, Object p_object)
	{
		m_iconSample = p_object as GameObject;

		CreateIconSample (m_kdIconSample,kdTenBtnObj.transform.parent.gameObject,920001);
		CreateIconSample (m_kjIconSample,kjTenBtnObj.transform.parent.gameObject,920002);
	}

	void CreateIconSample (GameObject tempObj,GameObject parentObj,int tempId)
	{
		if (tempObj == null)
		{
			tempObj = Instantiate(m_iconSample) as GameObject;
		}

		tempObj.transform.parent = parentObj.transform;
		tempObj.transform.localPosition = new Vector3 (0,110,0);
		tempObj.transform.localScale = Vector3.one;
		
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (tempId);
		string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		
		IconSampleManager iconSample = tempObj.GetComponent<IconSampleManager> ();
		iconSample.SetIconByID (tempId,"",3);
		iconSample.SetIconPopText(tempId, nameStr, mdesc, 1);
	}

	/// <summary>
	/// Refreshs the tan bao info.
	/// </summary>
	/// <param name="tbType">Tb type.</param>
	/// <param name="tempResp">Temp resp.</param>
	public void RefreshTanBaoInfo (TanBaoData.TanBaoType tbType,ExploreResp tempResp)
	{
		switch (tbType)
		{
		case TanBaoData.TanBaoType.TONGBI_SINGLE:
			
			tanBaoResp.tongBi = tempResp.info.money;
			tanBaoResp.remainFreeTongBiCount = tempResp.info.remainFreeCount;
			tanBaoResp.tongBiCd = tempResp.info.cd;
			InItKuangDong ();
			
			break;
		case TanBaoData.TanBaoType.TONGBI_SPEND:
			
			tanBaoResp.tongBi = tempResp.info.money;
			
			break;
		case TanBaoData.TanBaoType.YUANBAO_SINGLE:
//			Debug.Log ("tempResp.info.cd:" + tempResp.info.cd);
			tanBaoResp.yuanBao = tempResp.info.money;
			tanBaoResp.yuanBaoCd = tempResp.info.cd;
			InItKuangJing ();

			break;
		case TanBaoData.TanBaoType.YUANBAO_SPEND:
			
			tanBaoResp.yuanBao = tempResp.info.money;
			
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Tans the bao cost.
	/// </summary>
	/// <returns>The bao cost.</returns>
	/// <param name="tempCostId">Temp cost identifier.</param>
	private string TanBaoCost (int tempCostId)
	{
		PurchaseTemplate purchase = PurchaseTemplate.GetPurchaseTempById (tempCostId);
		return purchase.price.ToString ();
	}

	void TBBtnHandlerCallBack (GameObject obj)
	{
		UIYindao.m_UIYindao.CloseUI ();
		switch (obj.name)
		{
		case "KDSingleBtn":

			SetJinDuFalse ();
			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SINGLE);

			break;
		case "KDSpendBtn":

			SetJinDuFalse ();
			TBReward.tbReward.BlockController (true,0.1f);
			if (isTenTimesOpen)
			{
				TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SPEND);
			}
			else
			{
				QXComData.CreateBox (1,tenFunctionDes,true,TanBaoData.Instance.TanBaoRespCallBack,true);
			}

			break;
		case "KJSingleBtn":

			SetJinDuFalse ();
			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SINGLE);

			break;
		case "KJSpendBtn":

			SetJinDuFalse ();
			TBReward.tbReward.BlockController (true,0.1f);
			if (isTenTimesOpen)
			{
				TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SPEND);
			}
			else
			{
				QXComData.CreateBox (1,tenFunctionDes,true,TanBaoData.Instance.TanBaoRespCallBack,true);
			}

			break;
		case "CloseBtn":

			sEffectController.OnCloseWindowClick ();
			sEffectController.CloseCompleteDelegate += CloseTanBao;

			break;
		case "RulesBtn":

			TBReward.tbReward.BlockController (true,0.1f);
			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_HELP_DESC),ActiveGuide);

			break;
		default:
			break;
		}
	}

	void SetJinDuFalse ()
	{
		UIShoujiManager.m_UIShoujiManager.m_isPlayShouji = false;
	}

	void ActiveGuide ()
	{
		TBReward.tbReward.BlockController (false);
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.FINISHED_TASK_YINDAO,100160,4);
	}

	/// <summary>
	/// Moneies the number.
	/// </summary>
	/// <returns>The number.</returns>
	/// <param name="tempLabel">Temp label.</param>
	/// <param name="tempStartNum">Temp start number.</param>
	/// <param name="tempEndNum">Temp end number.</param>
	private int MoneyNum (UILabel tempLabel,int tempStartNum,int tempEndNum)
	{
		int startNum = tempStartNum;

		if (startNum <= tempEndNum)
		{
			if(tempLabel.gameObject.transform.localScale != Vector3.one)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;
				
				tempScal.x -= 0.02f;
				tempScal.y -= 0.02f;
				tempScal.z -= 0.02f;
				
				if(tempScal.x < 1)
				{
					tempScal.x = 1f;
					tempScal.y = 1f;
					tempScal.z = 1f;
				}
				
				tempLabel.gameObject.transform.localScale = tempScal;
			}
		}
		else
		{
			if (tempLabel.gameObject.transform.localScale.x < 1.1f)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;
				
				tempScal.x += 0.1f;
				tempScal.y += 0.1f;
				tempScal.z += 0.1f;
				
				if(tempScal.x > 1.1)
				{
					tempScal.x = 1.1f;
					tempScal.y = 1.1f;
					tempScal.z = 1.1f;
				}
				
				tempLabel.gameObject.transform.localScale = tempScal;
			}
			else
			{
				float tempAddNum = (startNum - tempEndNum) / 10;
				
				if (Mathf.Abs(tempAddNum) < 1)
				{
					startNum = tempEndNum;
				}
				else
				{
					startNum = (int)(startNum - tempAddNum);
				}
				
				tempLabel.text = startNum.ToString ();
			}
		}

		return startNum;
	}

	/// <summary>
	/// Checks the TB red.
	/// </summary>
	public void CheckTBRed ()
	{
		bool tongBiRed = tanBaoResp.remainFreeTongBiCount > 0 ? (kdCdTime > 0 ? false : true) : false;
		bool yuanBaoRed = kjCdTime > 0 ? false : true;

		kdRedObj.SetActive (tongBiRed);
		kjRedObj.SetActive (yuanBaoRed);

		PushAndNotificationHelper.SetRedSpotNotification (1101,tongBiRed ? true : false);//1102
		PushAndNotificationHelper.SetRedSpotNotification (1102,yuanBaoRed ? true : false);//1102
	}

	public void CloseTanBao ()
	{
		tbPage = null;
		isOpenFirst = true;
		CheckTBRed ();
		MainCityUI.TryRemoveFromObjectList (gameObject);
		TreasureCityUI.TryRemoveFromObjectList (gameObject);
//		gameObject.SetActive (false);

		//Set TreasureBoxUIBtn's Effect DisActive
		{
			if (TreasureCityUIBR.m_instance != null)
			{
				TreasureCityUIBR.m_instance.OpenWindow = false;
			}
		}

		Destroy (gameObject);
	}
}

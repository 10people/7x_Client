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
	private Dictionary<string,EventHandler> tbBtnDic = new Dictionary<string, EventHandler> ();

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

	private int kdCdTime;
	private int kjCdTime;

	public ScaleEffectController sEffectController;

	public GameObject anchorTopRight;

	private bool isOpenFirst = true;

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

		if (tbBtnDic.Count == 0)
		{
			foreach (EventHandler handler in tbBtnHandlerList)
			{
				tbBtnDic.Add (handler.name,handler);
				handler.m_handler -= TBBtnHandlerCallBack;
				handler.m_handler += TBBtnHandlerCallBack;
			}
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

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100160,1);

		InItKuangDong ();
		InItKuangJing ();

		CheckTBRed ();
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

			labelDic["Kd_Des"].text = QXComData.TimeFormat (kdCdTime) + "后免费";

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
			
			labelDic["Kj_Des"].text = QXComData.TimeFormat (kjCdTime) + "后免费";

			yield return new WaitForSeconds (1);

			if (kjCdTime <= 0)
			{
				TanBaoData.Instance.TanBaoInfoReq ();
			}
		}
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
			Debug.Log ("tempResp.info.cd:" + tempResp.info.cd);
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
		switch (obj.name)
		{
		case "KDSingleBtn":

			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SINGLE);

			break;
		case "KDSpendBtn":

			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SPEND);

			break;
		case "KJSingleBtn":

			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SINGLE);

			break;
		case "KJSpendBtn":

			TBReward.tbReward.BlockController (true,0.1f);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SPEND);

			break;
		case "CloseBtn":

			UIYindao.m_UIYindao.CloseUI ();

			sEffectController.OnCloseWindowClick ();
			sEffectController.CloseCompleteDelegate += CloseTanBao;

			break;
		case "RulesBtn":

			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_HELP_DESC));

			break;
		default:
			break;
		}
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

		PushAndNotificationHelper.SetRedSpotNotification (1101,tongBiRed || yuanBaoRed ? true : false);//1102
	}

	public void CloseTanBao ()
	{
		tbPage = null;
		isOpenFirst = true;
		CheckTBRed ();
		MainCityUI.TryRemoveFromObjectList (gameObject);
//		gameObject.SetActive (false);
		Destroy (gameObject);
	}
}

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

	private ExploreInfoResp tbInfoResp;

	public List<EventHandler> tbBtnHandlerList = new List<EventHandler> ();
	private Dictionary<string,EventHandler> tbBtnDic = new Dictionary<string, EventHandler> ();

	public List<NGUILongPress> btnLongPressList = new List<NGUILongPress> ();

	public List<UILabel> labelList = new List<UILabel> ();
	private Dictionary<string,UILabel> labelDic = new Dictionary<string, UILabel> ();
	private readonly string[] labelName = new string[]
	{
		"TongBi","YuanBao",
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

	public enum RewardType
	{
		KUANGDONG,
		KUANGJING,
	}
	private RewardType rewardType = RewardType.KUANGDONG;

	private int kdCdTime;
	private int kjCdTime;

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		tbPage = this;
	}

	/// <summary>
	/// Gets the TB info resp.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void GetTBInfoResp (ExploreInfoResp tempResp)
	{
		tbInfoResp = tempResp;

		if (tbBtnDic.Count == 0)
		{
			foreach (EventHandler handler in tbBtnHandlerList)
			{
				tbBtnDic.Add (handler.name,handler);
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

		labelDic ["TongBi"].text = tempResp.tongBi.ToString ();
		labelDic ["YuanBao"].text = tempResp.yuanBao.ToString ();
		labelDic ["Kd_Spend"].text = TanBaoCost (costDic ["KdSpend"]).ToString ();
		labelDic ["Kj_Spend"].text = TanBaoCost (costDic ["KjSpend"]).ToString ();

		InItKuangDong ();
		InItKuangJing ();
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

	/// <summary>
	/// Ins it kuang dong.
	/// </summary>
	void InItKuangDong ()
	{
		if (tbInfoResp.remainFreeTongBiCount > 0 && tbInfoResp.remainFreeTongBiCount <= tbInfoResp.allFreeTongBiCount)
		{
			kdCdTime = tbInfoResp.tongBiCd;
			if (kdCdTime > 0)
			{
				StopCoroutine ("KdFreeCd");
				StartCoroutine ("KdFreeCd");
			}
			else
			{
				labelDic["Kd_Des"].text = "本日免费次数" + tbInfoResp.remainFreeTongBiCount + "/" + tbInfoResp.allFreeTongBiCount;
			}
			
			labelDic["Kd_Single"].text = kdCdTime > 0 ? TanBaoCost (costDic["KdSingle"]) : "免费";
		}
		else
		{
			labelDic["Kd_Des"].text = "本日免费次数已用完";
			labelDic["Kd_Single"].text = TanBaoCost (costDic["KdSingle"]);
		}
	}
	IEnumerator KdFreeCd ()
	{
		while (kdCdTime > 0)
		{
			kdCdTime --;
			labelDic["Kd_Des"].text = QXComData.TimeFormat (kdCdTime) + "后免费";
			
			if (kdCdTime <= 0)
			{
				TanBaoData.Instance.TanBaoInfoReq ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// Ins it kuang jing.
	/// </summary>
	void InItKuangJing ()
	{
		if (tbInfoResp.yuanBaoCd > 0)
		{
			kjCdTime = tbInfoResp.yuanBaoCd;
			StopCoroutine ("KjFreeCd");
			StartCoroutine ("KjFreeCd");
			labelDic["Kj_Single"].text = TanBaoCost (costDic["KjSingle"]); 
		}
		else
		{
			labelDic["Kj_Des"].text = "本次探宝免费";
			labelDic["Kj_Single"].text = "免费"; 
		}
	}
	IEnumerator KjFreeCd ()
	{
		while (kjCdTime > 0)
		{
			kjCdTime --;
			labelDic["Kj_Des"].text = QXComData.TimeFormat (kjCdTime) + "后免费";
			
			if (kjCdTime <= 0)
			{
				TanBaoData.Instance.TanBaoInfoReq ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// Refreshs the tan bao info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempInfo">Temp info.</param>
	public void RefreshTanBaoInfo (TanBaoData.TanBaoType tempType,TypeInfo tempInfo)
	{
		switch (tempType)
		{
		case TanBaoData.TanBaoType.TONGBI_SINGLE:

			tbInfoResp.tongBi = tempInfo.money;
			tbInfoResp.remainFreeTongBiCount = tempInfo.remainFreeCount;
			tbInfoResp.tongBiCd = tempInfo.cd;

			labelDic ["TongBi"].text = tbInfoResp.tongBi.ToString ();
			InItKuangDong ();

			break;
		case TanBaoData.TanBaoType.TONGBI_SPEND:

			tbInfoResp.tongBi = tempInfo.money;
			labelDic ["TongBi"].text = tbInfoResp.tongBi.ToString ();

			break;
		case TanBaoData.TanBaoType.YUANBAO_SINGLE:

			tbInfoResp.yuanBao = tempInfo.money;
			tbInfoResp.yuanBaoCd = tempInfo.cd;
			
			labelDic ["YuanBao"].text = tbInfoResp.yuanBao.ToString ();
			InItKuangJing ();

			break;
		case TanBaoData.TanBaoType.YUANBAO_SPEND:
			
			tbInfoResp.yuanBao = tempInfo.money;
			labelDic ["YuanBao"].text = tbInfoResp.yuanBao.ToString ();
			
			break;
		default:
			break;
		}
	}

	void TBBtnHandlerCallBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "KDSingleBtn":

			TanBaoReward.tbReward.BlockController (true);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SINGLE);

			break;
		case "KDSpendBtn":

			TanBaoReward.tbReward.BlockController (true);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.TONGBI_SPEND);

			break;
		case "KJSingleBtn":

			TanBaoReward.tbReward.BlockController (true);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SINGLE);

			break;
		case "KJSpendBtn":

			TanBaoReward.tbReward.BlockController (true);
			TanBaoData.Instance.TBGetRewardReq (TanBaoData.TanBaoType.YUANBAO_SPEND);

			break;
		case "TBAddBtn":
			break;
		case "YBAddBtn":
			break;
		case "CloseBtn":

			sEffectController.OnCloseWindowClick ();
			sEffectController.CloseCompleteDelegate += CloseTanBao;

			break;
		default:
			break;
		}
	}

	public void CloseTanBao ()
	{

		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}

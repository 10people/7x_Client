using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpHighRank : MonoBehaviour {

	public static PvpHighRank highRank;

	public UISprite junXian1;
	public UISprite junXian2;

	public UILabel rank1;
	public UILabel rank2;

	public GameObject rewardBgObj;
	private GameObject iconSamplePrefab;

	private List<RewardData> rewardDataList;

	public EventHandler zheZhaoHandler;

	public GameObject effectObj;

	void Awake ()
	{
		highRank = this;
	}

	void OnDestroy ()
	{
		highRank = null;
	}

	/// <summary>
	/// Ins it high rank page.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	/// <param name="tempData">Temp data.</param>
	public void InItHighRankPage (BaiZhanInfoResp tempResp,List<RewardData> tempDataList)
	{
		rewardDataList = tempDataList;

		BaiZhanTemplate baiZhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (tempResp.pvpInfo.baizhanXMLId);

		rank1.text = tempResp.lastHighestRank.ToString ();
		rank2.text = tempResp.pvpInfo.rank.ToString ();

		Debug.Log ("tempResp.pvpInfo.historyHighRank:" + tempResp.pvpInfo.historyHighRank);
		Debug.Log ("tempResp.pvpInfo.rank:" + tempResp.pvpInfo.rank);

		junXian1.spriteName = "junxian" + BaiZhanTemplate.getBaiZhanJiBieByRank (tempResp.pvpInfo.historyHighRank);
		junXian2.spriteName = "junxian" + baiZhanTemp.jibie;

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}

		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, effectObj, EffectIdTemplate.GetPathByeffectId(620214), null);

		zheZhaoHandler.m_click_handler -= ClickBack;
		zheZhaoHandler.m_click_handler += ClickBack;
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = rewardBgObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3(0,-15,0);
		
		InItIconSample ();
	}
	
	void InItIconSample ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardDataList[0].itemId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (rewardDataList[0].itemId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (rewardDataList[0].itemId,"x" + rewardDataList[0].itemCount,2);
		fuShiIconSample.SetIconPopText(rewardDataList[0].itemId, nameStr, mdesc, 1);
//		iconSamplePrefab.transform.localScale = Vector3.one * 0.6f;
	}

	void ClickBack (GameObject obj)
	{
		BaiZhanPage.baiZhanPage.Yd_GetReward = false;
		BaiZhanPage.baiZhanPage.Yd_Store = false;
		GeneralRewardManager.Instance().CreateReward (rewardDataList);
		Global.m_isOpenBaiZhan = false;
		gameObject.SetActive (false);
	}
}

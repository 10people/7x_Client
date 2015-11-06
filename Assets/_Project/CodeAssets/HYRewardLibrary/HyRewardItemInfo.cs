using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HyRewardItemInfo : MonoBehaviour {

	public RewardItemInfo rewardInfo;
	
	public UISprite itemIcon;
	public UISprite suiPianIcon;
	
	public UISprite pinZhi;
	
	public UILabel numLabel;

	private List<long> junZhuIdList = new List<long> ();
	
	public GameObject selectBoxObj;

	public GameObject applicantListObj;

	private bool isApply = false;
	
	public void GetRewardItemInfo (RewardItemInfo tempInfo,bool isRefresh)
	{
		rewardInfo = tempInfo;

		HuangYeAwardTemplete hyAwardTemp = HuangYeAwardTemplete.getHuangYeAwardTemplateBySiteId (tempInfo.site);
		int itemId = hyAwardTemp.itemId;
		int itemType = hyAwardTemp.itemType;

		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (itemId);
		int iconId = commonTemp.icon;
		int colorId = commonTemp.color;

		switch (itemType)
		{
		case 5:
		{
			colorId = 1;
//			pinZhi.gameObject.SetActive (false);
//			MiBaoSuipianXMltemp miBaoSuiPianTemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (itemId);
//			int iconId = miBaoSuiPianTemp.icon;

			itemIcon.gameObject.SetActive (false);
			suiPianIcon.gameObject.SetActive (true);
			suiPianIcon.spriteName = iconId.ToString ();
			pinZhi.spriteName = "pinzhi" + (colorId - 1);

			break;
		}
		case 6:
		{
//			ItemTemp itemXml = ItemTemp.getItemTempById (itemId);
//			int colorId = itemXml.color;
//			int iconId = int.Parse (itemXml.icon);

//			pinZhi.spriteName = "pinzhi" + (colorId - 1);

			itemIcon.gameObject.SetActive (true);
			suiPianIcon.gameObject.SetActive (false);
			itemIcon.spriteName = iconId.ToString ();
			pinZhi.spriteName = "pinzhi" + (colorId - 1);

			break;
		}

		default:break;
		}

		junZhuIdList.Clear ();
//		Debug.Log ("rewardItem:" + tempInfo.site);
		if (tempInfo.applyerInfo.Count > 0)
		{
			foreach (ApplyerInfo tempPlayer in tempInfo.applyerInfo)
			{
				Debug.Log ("id：" + tempPlayer.junzhuId + "/" + JunZhuData.Instance ().m_junzhuInfo.id);
				junZhuIdList.Add (tempPlayer.junzhuId);
			}
			
			if (junZhuIdList.Contains (JunZhuData.Instance ().m_junzhuInfo.id))
			{
				selectBoxObj.SetActive (true);
			}
			else
			{
				selectBoxObj.SetActive (false);
			}
		}
		
		else
		{
			selectBoxObj.SetActive (false);
		}

		numLabel.text = tempInfo.nums.ToString ();

		if (isRefresh)
		{
			HYApplicantList applicantList = applicantListObj.GetComponent<HYApplicantList> ();
			applicantList.GetRewardItemInfo (tempInfo);
		}
	}

	void OnClick ()
	{
		if (this.transform.FindChild ("ToggleBox") == null)
		{
			HYRewardLibrary.hyReward.CreateToggleBox (this.gameObject);

			HYRewardLibrary.hyReward.RewardLibraryAnim (true);

			HYApplicantList applicantList = applicantListObj.GetComponent<HYApplicantList> ();
			applicantList.GetRewardItemInfo (rewardInfo);
		}
	}

	void Update ()
	{
		if (this.transform.FindChild ("ToggleBox") != null)
		{
			HYRewardBtnsCol.rewardBtnsCol.GetRewardInfo (rewardInfo);

			if (selectBoxObj.activeSelf)
			{
				HYRewardLibrary.hyReward.ChangeState (2);
			}
			else
			{
				HYRewardLibrary.hyReward.ChangeState (1);
			}
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterFuWen : MonoBehaviour {

	void OnClick ()
	{
		RechargeData.Instance.RechargeDataReq ();
		return;
		Carriage.BiaoJuRecordData.Instance.BiaoJuRecordReq (Carriage.BiaoJuRecordData.RecordType.HISTORY);
		return;

//		FuWenData.Instance.OpenFuWen ();
//		ShopData.Instance.OpenShop (ShopData.ShopType.ORDINARY);

		List<RewardData> rewardDataList = new List<RewardData> ();
		for (int i = 0;i < 3;i ++)
		{
			RewardData data = new RewardData (301013,1);//108105,920001,301013
			rewardDataList.Add (data);
		}

//		GeneralRewardManager.Instance().CreateReward (rewardDataList);
		GeneralRewardManager.Instance().CreateSpecialReward (rewardDataList);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterFuWen : MonoBehaviour {

	void OnClick ()
	{
//		RechargeData.Instance.RechargeDataReq ();
//		return;
//		Carriage.BiaoJuRecordData.Instance.BiaoJuRecordReq (Carriage.BiaoJuRecordData.RecordType.HISTORY);
//		return;

//		FuWenData.Instance.OpenFuWen ();
//		ShopData.Instance.OpenShop (ShopData.ShopType.ORDINARY);
		int[] idList = new int[]{920001,920002,920003,920004,920005,921001,921002,921003,921004,921005};
		List<RewardData> rewardDataList = new List<RewardData> ();
		for (int i = 0;i < idList.Length;i ++)
		{
			RewardData data = new RewardData (idList[i],1);//108105,920001,301013
			rewardDataList.Add (data);
		}

		GeneralRewardManager.Instance().CreateReward (rewardDataList);
//		GeneralRewardManager.Instance().CreateSpecialReward (rewardDataList);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterFuWen : MonoBehaviour {

	void OnClick ()
	{
//		FuWenData.Instance.OpenFuWen ();
//		ShopData.Instance.OpenShop (ShopData.ShopType.ORDINARY);

		List<RewardData> rewardDataList = new List<RewardData> ();
		for (int i = 0;i < 3;i ++)
		{
			RewardData data = new RewardData (105001,1);
			rewardDataList.Add (data);
		}

		GeneralRewardManager.Instance ().CreateReward (rewardDataList);
//		GeneralRewardManager.Instance ().CreateSpecialReward (rewardDataList);
	}
}

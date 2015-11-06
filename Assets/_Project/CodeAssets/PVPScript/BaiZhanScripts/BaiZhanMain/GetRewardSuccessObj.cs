using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GetRewardSuccessObj : MonoBehaviour {
	
	public UILabel weiWangLabel;

	public void RewardFly (int weiWang)
	{
		weiWangLabel.text = "+" + weiWang.ToString ();

		Hashtable rewardFly = new Hashtable ();
		rewardFly.Add ("easetype",iTween.EaseType.easeOutQuart);
		rewardFly.Add ("time",1.5f);
		rewardFly.Add ("position",new Vector3(240,65,0));
		rewardFly.Add ("oncomplete","FlyEnd");
		rewardFly.Add ("oncompletetarget",this.gameObject);
		rewardFly.Add ("islocal",true);
		iTween.MoveTo (this.gameObject,rewardFly);
	}

	void FlyEnd ()
	{
		Destroy (this.gameObject);
	}
}

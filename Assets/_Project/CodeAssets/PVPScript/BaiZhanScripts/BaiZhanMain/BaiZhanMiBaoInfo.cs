using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaiZhanMiBaoInfo : MonoBehaviour {

	public UISprite miBaoIcon;

	public UISprite miBaoQuality;

	public GameObject miBaoStar;

	private List<GameObject> starList = new List<GameObject> ();

	//获得秘宝信息
	public void GetMiBaoInfo (int miBaoId,int miBaoStarLevel)
	{
		Debug.Log ("秘宝信息：" + 
		           "秘宝id：" + miBaoId + "秘宝星级：" + miBaoStarLevel);

		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(miBaoId);
		
		int iconId = mMiBaoXmlTemp.icon;

		miBaoIcon.spriteName = iconId.ToString ();

		int quality = mMiBaoXmlTemp.pinzhi;

		miBaoQuality.spriteName = "pinzhi" + quality.ToString();

//		float x = miBaoStar.transform.localPosition.x;
		float y = miBaoStar.transform.localPosition.y;
		float z = miBaoStar.transform.localPosition.z;

		foreach(GameObject starObj in starList)
		{
			Destroy (starObj);
		}

		starList.Clear ();

		for (int i = 0;i < miBaoStarLevel;i ++)
		{
			GameObject star = (GameObject)Instantiate (miBaoStar);

			star.SetActive (true);

			star.name = "Star" + (i + 1);

			star.transform.parent = miBaoStar.transform.parent;

			star.transform.localPosition = new Vector3(i * 20f - (miBaoStarLevel - 1) * 10f,y,z);

			star.transform.localScale = miBaoStar.transform.localScale;

			starList.Add (star);
		}
	}
}

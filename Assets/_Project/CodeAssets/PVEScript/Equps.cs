using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class Equps : MonoBehaviour {

	[HideInInspector]public int Award_id;
	[HideInInspector]public string RewardIcom;
	[HideInInspector]public int Numbers;
	public SaoDangAwardItem mSaoDangAwarditem;
	public UILabel Num;
	public UILabel Name;
	public UISprite UIRewardIcom;
	//public UILabel muilabel;
	void Start () {
	
	}
	
    public void init(){

		Num.text = mSaoDangAwarditem.itemNum.ToString ();
		int id = mSaoDangAwarditem.itemId;
		Debug.Log ("id  = "+id);

		CommonItemTemplate mComItem = CommonItemTemplate.getCommonItemTemplateById (id);

		UIRewardIcom.spriteName = mComItem.icon.ToString();

		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(id);

		Name.text =  mNameIdTemplate.Name;

	}

}

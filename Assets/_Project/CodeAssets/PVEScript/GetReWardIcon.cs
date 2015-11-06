using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;
public class GetReWardIcon : MonoBehaviour {
	[HideInInspector]public int  RewName;
	[HideInInspector]public int  RewIcon;
	[HideInInspector]public int itType ;
	[HideInInspector]public int RewId ;
	[HideInInspector]public int  RewPinzhi;
	GameObject showRewardname = null;
	public GameObject Awardtips;
	public UISprite ico;


	GameObject showInfo;
	public void init(){
		//Debug.Log ("掉落的装备icon的名字。"+RewIcon);

		//Debug.Log ( t_tex.name + " - " + t_tex.width );

//		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId( RewId );

		ItemTemp mItemTemp = ItemTemp.getItemTempById (RewId);

		ico.spriteName = mItemTemp.icon;
	}
	void OnPress()
	{
		
		Debug.Log ("你点击了这个敌人");
		showinfo ();
		
	}

   void showinfo(){
		if(showRewardname == null){
			showRewardname = Instantiate ( Awardtips) as GameObject;
			showRewardname.SetActive(true);
			showRewardname.transform.parent = this.transform;
			showRewardname.transform.localScale = new Vector3 (1,1,1);
			showRewardname.transform.localPosition = new Vector3 (0,105,0);

			NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId (RewId);
			string mdesc = DescIdTemplate.GetDescriptionById(RewId);
			ShowDropinfo mSHow = showRewardname.GetComponent<ShowDropinfo>();
			mSHow.mName = mNameIdTemplate.Name;
			mSHow.mInfo = mdesc;
			mSHow.init();
		}
		else{
			Destroy(showRewardname);
		}
	}

}

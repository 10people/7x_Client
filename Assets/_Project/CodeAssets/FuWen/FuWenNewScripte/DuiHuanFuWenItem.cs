using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class DuiHuanFuWenItem : MonoBehaviour {

	public UIEventListener mEventListener;

	public UISprite FuwenIcon;

	public UISprite FuWenPinZhi;

	public UILabel DuiHuanNeedCost;

	public int AwardId;

	private int Cost;

	[HideInInspector]public int MCost;

	public GameObject ButtonName;

	void Update () {
	
	}
	public void init()
	{
		FuWenDuiHuanTemplate mFuWenDuiHuanTemplate = FuWenDuiHuanTemplate.GetFuWenDuiHuanTemplate_By_Id (AwardId);

		FuWenTemplate mfuwentemp = FuWenTemplate.GetFuWenTemplateByFuWenId (AwardId);

		FuWenPinZhi.spriteName = "pinzhi"+(mfuwentemp.color -1).ToString();

		FuwenIcon.spriteName = mfuwentemp.icon.ToString ();

		DuiHuanNeedCost.text = mFuWenDuiHuanTemplate.cost.ToString ();

		Cost = mFuWenDuiHuanTemplate.cost;
	}

	public void DuiHuan()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.JIAPIANCHSHTIPS ),
		                        FuWenFwDuiHuan );
	}
	GameObject FwDuiHuan;
	void FuWenFwDuiHuan ( ref WWW p_www, string p_path, Object p_object )
	{
		Debug.Log ("符文替换");
		if(FwDuiHuan == null)
		{
			FwDuiHuan = GameObject.Instantiate( p_object ) as GameObject;
			
			FwDuiHuan.name = "FwDuiHuan";
			
			DuiHuanMind mDuiHuanMind = FwDuiHuan.GetComponent<DuiHuanMind>();
			mDuiHuanMind.awardid = AwardId;
			mDuiHuanMind.M_Cost = MCost;
		    mDuiHuanMind.Init();
			//MainCityUI.TryAddToObjectList(NewfuWenObj,false);
		} 
	}

}

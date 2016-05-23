using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class FuWenLanWei : MonoBehaviour {

	[HideInInspector]public FuwenLanwei mFuwenLanwei;

	public UILabel Property;

	public GameObject Arelt;

	public UISprite FuWenIcon;
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void init()
	{
//		Debug.Log ("mFuwenLanwei.itemId = "+mFuwenLanwei.itemId);
//		Debug.Log ("mFuwenLanwei.flag = "+mFuwenLanwei.flag);
		if(mFuwenLanwei.itemId == 0)
		{
			Property.gameObject.SetActive(true);
			FuWenIcon.spriteName = "";
			//List<int> mFuwenOpenList = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiIdList(mpage);
			FuWenOpenTemplate mFuwenOpen = FuWenOpenTemplate.GetFuWenOpenTemplateBy_By_Id(mFuwenLanwei.lanweiId);
			Property.text = mFuwenOpen.background;

		}else if(mFuwenLanwei.itemId > 0)
		{
			Property.gameObject.SetActive(false);
			FuWenTemplate mFuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenLanwei.itemId);
			FuWenIcon.spriteName = mFuwenTemp.icon.ToString();
		}
		Arelt.SetActive (mFuwenLanwei.flag);
	}
}

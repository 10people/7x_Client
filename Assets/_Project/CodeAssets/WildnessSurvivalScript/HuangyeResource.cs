using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;

using qxmobile.protobuf;

public class HuangyeResource : MonoBehaviour {

//	public HuangYeResource mHuangYeResource;//
//
//	//public GameObject Res;
//
//	public UISprite Res_Icon;
//
//	public UILabel Res_Lv;
//
//	public UILabel Res_name;
//
//	public UILabel Alliance_name;
//	void Start () {
//	
//	}
//	
//
//	void Update () {
//	
//	}
//
//	public void init()
//	{
//		int state = mHuangYeResource.status;
//
//		HuangyeTemplate mHY = HuangyeTemplate.getHuangyeTemplate_byid (mHuangYeResource.fileId);
//
//		string mName = NameIdTemplate.GetName_By_NameId (mHY.nameId);
//
//		Res_name.text = mName;
//
//		Res_Lv.text = mHuangYeResource.level.ToString ();
//
//		switch(state)
//		{
//		case 0:   // NPC占领的岛屿
//			Res_Icon.spriteName = "resource_npcOccupy";
//
//			Alliance_name.text = "未占领";
//			break;
//		case 1:     // 被其他玩家占领的岛屿
//			Res_Icon.spriteName = "resource_occupied";
//
//			Alliance_name.text = mHuangYeResource.name;
//			break;
//		case 2:      // 我方占领的岛屿
//			Res_Icon.spriteName = "our_resource";
//
//			Alliance_name.text = "已占领";
//
//			break;
//		default:
//			break;
//		}
//
//	}
//	public void OpenResourceBeach()
//	{
//
//		int state = mHuangYeResource.status;
//
//		ShowRes_NPC();
//	
//	}
//
//	void ShowRes_NPC( )//被NPC占领的岛屿 弹出UI
//	{
//		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HUANGYE_ZHENGROONG), LoadBack);
//	}
//	private GameObject ZhengRongUI;
//
//	private void LoadBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		if(ZhengRongUI != null)
//		{
//			return;
//		}
//		ZhengRongUI = Instantiate(p_object) as GameObject;
//		
//		ZhengRongUI.transform.parent = GameObject.Find("WildnessSurvival(Clone)").transform;
//		
//		ZhengRongUI.transform.localPosition = new Vector3(0,0,0);
//		
//		ZhengRongUI.transform.localScale = new Vector3(1,1,1);
//		
//		HYResoureEnemy mHuangyeZhengRong = ZhengRongUI.GetComponent<HYResoureEnemy>();
//		
//		mHuangyeZhengRong.mHuangYeResource = mHuangYeResource;
//
//		mHuangyeZhengRong.SendMessage_HYResoures();
//
//	}

}




























using UnityEngine;
using System.Collections;

public class LoginItemInfo : MonoBehaviour {

	private Login loginInfo;
	public Login SetLoginInfo
	{
		set{loginInfo = value;}
	}
	public Login GetLoginInfo
	{
		get{return loginInfo;}
	}

	public UILabel serveNameLabel;//区名
	
	public UILabel serveStateLabel;//区状态

	private int index;
	public int SetIndex
	{
		set{index = value;}
	}
	public int GetIndex
	{
		get{return index;}
	}

	public void InItLoginItemInfo ()
	{
//		serveNameLabel.text = loginInfo.serveName + loginInfo.serveId + "区";

		// 2015.10.14 by YuGu, change show
		serveNameLabel.text = loginInfo.serveName;

		AccountRequest.account.ShowServeState (serveStateLabel,loginInfo.serveState);
	}

	void OnPress (bool isPress)
	{
		SelectServeTwo.SetIsLeft = false;
//		Debug.Log ("isLeft:" + SelectServeTwo.GetIsLeft);
	}
}

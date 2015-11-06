using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class LastLogin : MonoBehaviour {

	public JSONNode loginNode;

	public UILabel lastLogin_NameLabel;  //上次登录区名文本框
	public UILabel lastLogin_StateLabel; //上次登录区状态显示文本框

	public GameObject selectServe1;//选区1
	public GameObject selectServe2;//选区2

	public void CreateLastLogin ()
	{
		JSONNode lastLogin = loginNode["lastLogin"];

//		Debug.Log ("lastLogin:" + lastLogin);

		if (lastLogin != null)
		{
			AccountRequest.account.ShowServeState (lastLogin_StateLabel,lastLogin ["state"].AsInt);
			
//			lastLogin_NameLabel.text = lastLogin ["name"].Value + lastLogin ["id"].AsInt + "区";

			// 2015.10.14 by YuGu, change show
			lastLogin_NameLabel.text = lastLogin ["name"].Value;


			SocketTool.SetServerPrefix( lastLogin ["ip"].Value, lastLogin ["port"].AsInt );

			CityGlobalData.m_LastLoginName = lastLogin ["name"].Value;

//			Debug.Log ("lastLoginName:" + lastLogin ["name"].Value);
//			Debug.Log ("lastLoginIp:" + lastLogin ["ip"].Value);
//			Debug.Log ("lastLoginPort:" + lastLogin ["port"].Value);

			EnterGame.enterGame.ServerState = lastLogin ["state"].AsInt;
		}
	}

	//选择分区
	public void SelectServeArea ()
	{
		int serveNum = Login.CreateLoginListByJson (loginNode).Count;

		this.gameObject.SetActive (false);

		if (serveNum <= 40) 
		{
			selectServe1.SetActive (true);

			SelectServeOne serveOne = selectServe1.GetComponent<SelectServeOne> ();
			serveOne.CreateServeItems ();
		}
		
		else 
		{
			selectServe2.SetActive (true);
			SelectServeTwo serveTwo = selectServe2.GetComponent<SelectServeTwo> ();
			serveTwo.CreateServeItems ();
		}
	}
}

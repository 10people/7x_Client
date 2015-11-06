using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Login {

	public int serveId;//服务器Id

	public string serveName;//服务器名称

	public string serveIp;//服务器ip

	public int servePort;//服务器端口号

	public int serveState;//服务器状态

	private static List<Login> loginDataList = new List<Login> ();

	public static List<Login> CreateLoginListByJson (JSONNode json)
	{
		loginDataList.Clear ();

		JSONArray loginList = json ["loginList"].AsArray;

		for (int i = 0;i < loginList.Count;i ++) 
		{	
			Login data = new Login();
			
			data.serveId = loginList [i]["id"].AsInt;
			data.serveName = loginList [i]["name"].Value;
			data.serveIp = loginList [i]["ip"].Value;
			data.servePort = loginList [i]["port"].AsInt;
			data.serveState = loginList [i]["state"].AsInt;
			
			loginDataList.Add (data);
		}

//		Debug.Log ("loginDataList:" + loginDataList.Count);

		return loginDataList;
	}

	public static List<Login> GetLoginData ()
	{
		return loginDataList;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectServeOne : MonoBehaviour {

	/// <summary>
	/// 服务区itemobj
	/// </summary>
	public GameObject loginItemObj;
	/// <summary>
	/// 储存itemobj 的list
	/// </summary>
	private List<GameObject> loginItemList = new List<GameObject> ();
	/// <summary>
	/// loginItem父对象
	/// </summary>
	public GameObject gridObj;
	/// <summary>
	/// 上次登陆按钮sprite
	/// </summary>
	public UISprite lastLoginBtnSprite;
	/// <summary>
	/// 上次登录按钮label
	/// </summary>
	public UILabel lastLoginBtnLabel;

	public GameObject lastLoginObj;

	public TriggerHandler selectBoxTri;

	void Start ()
	{
		selectBoxTri.m_handler += TriggerLoginItem;
	}

	/// <summary>
	/// 初始化选取信息
	/// </summary>
	public void CreateServeItems () 
	{	
		foreach (GameObject m_loginItem in loginItemList)
		{
			Destroy (m_loginItem);
		}
		loginItemList.Clear ();

		List<Login> loginList = Login.GetLoginData ();

		for (int i = 0;i < loginList.Count - 1;i ++) 
		{	
			for (int j = 0;j < loginList.Count - i - 1;j ++) {
				
				if (loginList[j].serveId < loginList[j + 1].serveId) 
				{	
					Login tempLogin = loginList[j];
					
					loginList[j] = loginList[j + 1];
					
					loginList[j + 1] = tempLogin;
				}
			}
		}

		for (int i = 0;i < loginList.Count;i ++)
		{
			GameObject loginItem = (GameObject)Instantiate (loginItemObj);
			
			loginItem.SetActive (true);
			loginItem.name = "LoginItem" + (i + 1);
			
			loginItem.transform.parent = gridObj.transform;
			loginItem.transform.localPosition = new Vector3 (0,-60 * i,0);
			loginItem.transform.localScale = Vector3.one;
			
			loginItemList.Add (loginItem);
			
			LoginItemInfo loginItemInfo = loginItem.GetComponent<LoginItemInfo> ();
			loginItemInfo.SetLoginInfo = loginList[i];
			loginItemInfo.InItLoginItemInfo ();
		}

		gridObj.GetComponent<UICenterOnChild> ().enabled = true;
		if (loginItemList.Count > 5)
		{
			loginItemList[2].GetComponent<UICenterOnClick> ().UICenterMove ();
		}

		AccountRequest.account.ShowLastLoginBtn (lastLoginBtnSprite,lastLoginBtnLabel);
	}

	/// <summary>
	/// 上次登录按钮
	/// </summary>
	public void LastLoginBtn ()
	{
		if (AccountRequest.account.isNewPlayer == 2)
		{
			return;
		}
		lastLoginObj.SetActive (true);
		LastLogin lastLogin = lastLoginObj.GetComponent<LastLogin> ();
		lastLogin.CreateLastLogin ();

		this.gameObject.SetActive (false);
	}

	/// <summary>
	/// 设置ip和端口号
	/// </summary>
	/// <param name="item">Item.</param>
	void TriggerLoginItem (Collider item) 
	{	
		LoginItemInfo loginItemTri = item.transform.parent.GetComponent<LoginItemInfo> ();

		if (loginItemTri) 
		{	
			SocketTool.SetServerPrefix( loginItemTri.GetLoginInfo.serveIp,
			                           loginItemTri.GetLoginInfo.servePort);

			EnterGame.enterGame.ServerState = loginItemTri.GetLoginInfo.serveState;
		}

//		Debug.Log ("ServeName:" + loginItemTri.loginInfo.serveName);
//		Debug.Log ("ServeIp:" + loginItemTri.loginInfo.serveIp);
//		Debug.Log ("servePort:" + loginItemTri.loginInfo.servePort);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectServeTwo : MonoBehaviour {

	public GameObject leftLoginItem;
	public GameObject rightLoginItem;

	public GameObject leftGrid;
	public GameObject rightGrid;

	private List<GameObject> leftItemList = new List<GameObject> ();
	private List<GameObject> rightItemList = new List<GameObject> ();

	public GameObject lastLoginObj;

	public UISprite lastLoginBtnSprite;
	public UILabel lastLoginBtnLabel;

	public TriggerHandler leftTriggerBox;
	public TriggerHandler rightTriggerBox;

	private int fenQuCount;//分区个数
	private int serveNum = 4;//每个分区最多包含的服务器个数

	private static bool isLeft;
	public static bool SetIsLeft
	{
		set{isLeft = value;}
	}
	public static bool GetIsLeft
	{
		get{return isLeft;}
	}

	void Start ()
	{
		leftTriggerBox.m_handler += LeftTrigger;
		rightTriggerBox.m_handler += RightTrigger;
	}

	public void CreateServeItems ()
	{
		DestroyItems (leftItemList);
		DestroyItems (rightItemList);

		List<Login> loginList = Login.GetLoginData ();

		fenQuCount = loginList.Count % serveNum > 0? loginList.Count / serveNum + 1 : loginList.Count / serveNum;

//		Debug.Log ("fenQuCount:" + fenQuCount);

		for (int i = 0;i < fenQuCount;i ++)
		{
			GameObject leftItem = (GameObject)Instantiate (leftLoginItem);

			leftItem.SetActive (true);
			leftItem.name = "PartItem" + (fenQuCount - i - 1);

			leftItem.transform.parent = leftGrid.transform;
			leftItem.transform.localPosition = new Vector3(0,-60 * i,0);
			leftItem.transform.localScale = Vector3.one;

			leftItemList.Add (leftItem);

			LeftPartItem leftPart = leftItem.GetComponent<LeftPartItem> ();
			leftPart.SetIndex = fenQuCount - i - 1;//从大到小
			leftPart.ShowLeft (serveNum);
//			Debug.Log ("leftPart.SetIndex:" + leftPart.GetIndex);
		}

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
			GameObject rightItem = (GameObject)Instantiate (rightLoginItem);

			rightItem.SetActive (true);
			rightItem.name = "LoginItem" + (loginList.Count - i);

			rightItem.transform.parent = rightGrid.transform;
			rightItem.transform.localPosition = new Vector3(0,-60 * i,0);
			rightItem.transform.localScale = Vector3.one;

			rightItemList.Add (rightItem);

			LoginItemInfo loginItemInfo = rightItem.GetComponent<LoginItemInfo> ();
			loginItemInfo.SetLoginInfo = loginList[i];
			loginItemInfo.SetIndex = loginList.Count - i - 1;//从大到小
			loginItemInfo.InItLoginItemInfo ();
//			Debug.Log ("loginItemInfo.SetIndex:" + loginItemInfo.GetIndex);
		}

		leftGrid.GetComponent<UICenterOnChild> ().enabled = true;
		rightGrid.GetComponent<UICenterOnChild> ().enabled = true;
	
		AccountRequest.account.ShowLastLoginBtn (lastLoginBtnSprite,lastLoginBtnLabel);
	}

	void DestroyItems (List<GameObject> tempList)
	{
		foreach (GameObject tempObj in tempList)
		{
			Destroy (tempObj);
		}
		tempList.Clear ();
	}

	//上次登陆按钮
	public void LastLoginBtn ()
	{
		this.gameObject.SetActive (false);
		
		lastLoginObj.SetActive (true);
		LastLogin lastLogin = lastLoginObj.GetComponent<LastLogin> ();
		lastLogin.CreateLastLogin ();
	}

	void LeftTrigger (Collider item)
	{
		if (isLeft)
		{
			LeftPartItem leftPart = item.transform.parent.GetComponent<LeftPartItem> ();
			
			if (rightItemList.Count >= leftPart.GetIndex * serveNum + serveNum / 2)
			{
				UICenterOnClick uiCenter = rightItemList[rightItemList.Count - (leftPart.GetIndex * serveNum + (serveNum / 2 - 1)) - 1].GetComponent<UICenterOnClick> ();
				uiCenter.UICenterMove ();
			}
			else
			{
				UICenterOnClick uiCenter = rightItemList[0].GetComponent<UICenterOnClick> ();
				uiCenter.UICenterMove ();
			}
		}
	}

	void RightTrigger (Collider item)
	{
		LoginItemInfo loginItemTri = item.transform.parent.GetComponent<LoginItemInfo> ();
		
		if (loginItemTri) 
		{	
			SocketTool.SetServerPrefix( loginItemTri.GetLoginInfo.serveIp,
			                           loginItemTri.GetLoginInfo.servePort);

			EnterGame.enterGame.ServerState = loginItemTri.GetLoginInfo.serveState;
		}

		if (!isLeft)
		{
			int curFengQu = (loginItemTri.GetIndex + 1) % serveNum > 0 ? ((loginItemTri.GetIndex + 1) / serveNum) + 1 : (loginItemTri.GetIndex + 1)/serveNum;
			UICenterOnClick uiCenter = leftItemList [leftItemList.Count - curFengQu].GetComponent<UICenterOnClick> ();
			uiCenter.UICenterMove ();
		}
	}
}

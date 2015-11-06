using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SystemEmail : MonoBehaviour {
	
	private List<GameObject> emailItemList = new List<GameObject> ();//邮件itemList

	public GameObject systemGrid;
	public GameObject emailItemObj;//邮件item

	public GameObject noEmailLabel;

	/// <summary>
	/// 获得系统邮件列表
	/// </summary>
	/// <param name="tempList">Temp list.</param>
	public void GetSystemEmailList (List<EmailInfo> tempList)
	{
		Debug.Log ("GetSystemEmailList:" + tempList.Count);

		if (tempList.Count > 0)
		{
			for (int i = 0;i < tempList.Count;i ++)
			{
				GameObject emailItem = (GameObject)Instantiate (emailItemObj);
				
				emailItem.SetActive (true);
				
				emailItem.transform.parent = systemGrid.transform;
				emailItem.transform.localPosition = new Vector3 (0,-133 * i,0);
				emailItem.transform.localScale = emailItemObj.transform.localScale;
				
				emailItemList.Add (emailItem);
			}

			InItSystemEmail (tempList);
		}

		CheckEmailCount (tempList.Count);
	}

	/// <summary>
	/// 初始化系统邮件 0-未读 1-已读
	/// </summary>
	/// <param name="tempEmailList">Temp email list.</param>
	void InItSystemEmail (List<EmailInfo> tempEmailList)
	{
		List<EmailInfo> newEmailList = new List<EmailInfo> ();//未读邮件list
		List<EmailInfo> oldEmailList = new List<EmailInfo> ();//已读邮件list
		List<EmailInfo> allEmailList = new List<EmailInfo> ();//总邮件list

		foreach (EmailInfo email in tempEmailList)
		{
//			Debug.Log ("email.isRead：" + email.isRead);
			if (email.isRead == 0)
			{
				newEmailList.Add (email);
			}

			else if (email.isRead == 1)
			{
				oldEmailList.Add (email);
			}
		}

		//未读邮件排序
		for (int i = 0;i < newEmailList.Count - 1;i ++)
		{
			for (int j = 0;j < newEmailList.Count - i - 1;j ++)
			{
				if (newEmailList[j].time < newEmailList[j + 1].time)
				{
					EmailInfo tempEmail = newEmailList[j];

					newEmailList[j] = newEmailList[j + 1];

					newEmailList[j + 1] = tempEmail;
				}
			}
		}

		//将未读邮件加到总邮件list中
		foreach (EmailInfo newEmail in newEmailList)
		{
			allEmailList.Add (newEmail);
		}

		//已读邮件排序
		for (int i = 0;i < oldEmailList.Count - 1;i ++)
		{
			for (int j = 0;j < oldEmailList.Count - i - 1;j ++)
			{
				if (oldEmailList[j].time < oldEmailList[j + 1].time)
				{
					EmailInfo tempEmail = oldEmailList[j];
					
					oldEmailList[j] = oldEmailList[j + 1];
					
					oldEmailList[j + 1] = tempEmail;
				}
			}
		}

		//将已读邮件加入到总邮件list中
		foreach (EmailInfo oldEmail in oldEmailList)
		{
			allEmailList.Add (oldEmail);
		}

		//初始化邮件item信息
		for (int i = 0;i < emailItemList.Count;i ++)
		{
			EmailItemInfo itemInfo = emailItemList[i].GetComponent<EmailItemInfo> ();
			itemInfo.GetEmailItemInfo (allEmailList[i]);
		}
	}

	/// <summary>
	/// 刷新系统邮件信息
	/// </summary>
	public void RefreshEmailList (List<EmailInfo> tempList)
	{
		Debug.Log ("RefreshTempList:" + tempList.Count);

		if (tempList.Count != emailItemList.Count)
		{
			if (tempList.Count > emailItemList.Count)
			{
				Debug.Log ("添加emailItem");
				int addCount = tempList.Count - emailItemList.Count;
				for (int i = 0;i < addCount;i ++)
				{
					GameObject newEmailItem = (GameObject)Instantiate (emailItemObj);
					
					newEmailItem.SetActive (true);
					
					newEmailItem.transform.parent = systemGrid.transform;
					newEmailItem.transform.localPosition = emailItemObj.transform.localPosition + new Vector3(0,-133 * emailItemList.Count,0);
					newEmailItem.transform.localScale = emailItemObj.transform.localScale;
					
					emailItemList.Add (newEmailItem);
				}
			}

			else if (tempList.Count < emailItemList.Count)
			{
				Debug.Log ("list1:" + emailItemList.Count);
				int destroyCount = emailItemList.Count - tempList.Count;
				Debug.Log ("destroyCount:" + destroyCount);
				for (int i = 0;i < destroyCount;i ++)
				{
					Destroy (emailItemList[0]);
					emailItemList.Remove (emailItemList[i]);
				}
				Debug.Log ("list2:" + emailItemList.Count);
			}
		}

		for (int i = 0;i < emailItemList.Count;i ++)
		{
			emailItemList[i].transform.localPosition = new Vector3(0,-133 * i,0);
		}

		InItSystemEmail (tempList);

		CheckEmailCount (tempList.Count);
	}

	void CheckEmailCount (int count)
	{
		if (count > 0)
		{
			noEmailLabel.SetActive (false);

			if (count < 4)
			{
				systemGrid.gameObject.GetComponent<ItemTopCol> ().enabled = true;
			}
			else
			{
				systemGrid.gameObject.GetComponent<ItemTopCol> ().enabled = false;
			}
		}
		else
		{
			noEmailLabel.SetActive (true);
		}
	}
}

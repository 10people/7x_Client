using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PrivateEmail : MonoBehaviour {
	
	private List<GameObject> letterItemList = new List<GameObject> ();//私信itemList

	public GameObject privateGrid;
	public GameObject letterItemObj;//私信item

	public GameObject noLetterLabel;

	/// <summary>
	/// 获得私信列表
	/// </summary>
	/// <param name="tempList">Temp list.</param>
	public void GetPrivateLetterList (List<EmailInfo> tempList)
	{
		Debug.Log ("GetPrivateEmailList:" + tempList.Count);
	
		if (tempList.Count > 0)
		{
			for (int i = 0;i < tempList.Count;i ++)
			{
				GameObject emailItem = (GameObject)Instantiate (letterItemObj);
				
				emailItem.SetActive (true);
				
				emailItem.transform.parent = privateGrid.transform;
				emailItem.transform.localPosition = new Vector3 (0,-133 * i,0);
				emailItem.transform.localScale = letterItemObj.transform.localScale;
				
				letterItemList.Add (emailItem);
			}

			InItPrivateEmail (tempList);
		}

		CheckEmailCount (tempList.Count);
	}

	/// <summary>
	/// 初始化私信列表 0-未读 1-已读
	/// </summary>
	/// <param name="letterList">Letter list.</param>
	void InItPrivateEmail (List<EmailInfo> letterList)
	{
		List<EmailInfo> newLetterList = new List<EmailInfo> ();//未读私信list
		List<EmailInfo> oldLetterList = new List<EmailInfo> ();//已读私信list
		List<EmailInfo> allLetterList = new List<EmailInfo> ();//总私信list
		
		foreach (EmailInfo letter in letterList)
		{
			if (letter.isRead == 0)
			{
				newLetterList.Add (letter);
			}
			
			else if (letter.isRead == 1)
			{
				oldLetterList.Add (letter);
			}
		}
		
		//未读私信排序
		for (int i = 0;i < newLetterList.Count - 1;i ++)
		{
			for (int j = 0;j < newLetterList.Count - i - 1;j ++)
			{
				if (newLetterList[j].time < newLetterList[j + 1].time)
				{
					EmailInfo tempEmail = newLetterList[j];
					
					newLetterList[j] = newLetterList[j + 1];
					
					newLetterList[j + 1] = tempEmail;
				}
			}
		}
		
		//将未读私信加到总邮件list中
		foreach (EmailInfo newEmail in newLetterList)
		{
			allLetterList.Add (newEmail);
		}
		
		//已读私信排序
		for (int i = 0;i < oldLetterList.Count - 1;i ++)
		{
			for (int j = 0;j < oldLetterList.Count - i - 1;j ++)
			{
				if (oldLetterList[j].time < oldLetterList[j + 1].time)
				{
					EmailInfo tempEmail = oldLetterList[j];
					
					oldLetterList[j] = oldLetterList[j + 1];
					
					oldLetterList[j + 1] = tempEmail;
				}
			}
		}
		
		//将已读邮件加入到总邮件list中
		foreach (EmailInfo oldEmail in oldLetterList)
		{
			allLetterList.Add (oldEmail);
		}

		for (int i = 0;i < allLetterList.Count;i ++)
		{
			EmailItemInfo itemInfo = letterItemList[i].GetComponent<EmailItemInfo> ();
			itemInfo.GetEmailItemInfo (allLetterList[i]);
		}
	}

	/// <summary>
	/// 刷新私信列表
	/// </summary>
	public void RefreshLetterList (List<EmailInfo> tempList)
	{	
		if (tempList.Count != letterItemList.Count)
		{
			if (tempList.Count > letterItemList.Count)
			{
				Debug.Log ("添加letterItem");
				int addCount = tempList.Count - letterItemList.Count;
				for (int i = 0;i < addCount;i ++)
				{
					GameObject newEmailItem = (GameObject)Instantiate (letterItemObj);
					
					newEmailItem.SetActive (true);
					
					newEmailItem.transform.parent = privateGrid.transform;
					newEmailItem.transform.localPosition = letterItemObj.transform.localPosition + new Vector3(0,-133 * letterItemList.Count,0);
					newEmailItem.transform.localScale = letterItemObj.transform.localScale;
					
					letterItemList.Add (newEmailItem);
				}
			}
			
			else if (tempList.Count < letterItemList.Count)
			{
				Debug.Log ("list1:" + letterItemList.Count);
				int destroyCount = letterItemList.Count - tempList.Count;
				Debug.Log ("destroyCount:" + destroyCount);
				for (int i = 0;i < destroyCount;i ++)
				{
					Destroy (letterItemList[0]);
					letterItemList.Remove (letterItemList[i]);
				}
				Debug.Log ("list2:" + letterItemList.Count);
			}
		}

		for (int i = 0;i < letterItemList.Count;i ++)
		{
			letterItemList[i].transform.localPosition = new Vector3(0,-133 * i,0);
		}

		InItPrivateEmail (tempList);

		CheckEmailCount (tempList.Count);
	}

	void CheckEmailCount (int count)
	{
		if (count > 0)
		{
			noLetterLabel.SetActive (false);

			if (count < 4)
			{
				privateGrid.gameObject.GetComponent<ItemTopCol> ().enabled = true;
			}
			else
			{
				privateGrid.gameObject.GetComponent<ItemTopCol> ().enabled = false;
			}
		}
		else
		{
			noLetterLabel.SetActive (true);
		}
	}
}

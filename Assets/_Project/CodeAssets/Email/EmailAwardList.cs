using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailAwardList : MonoBehaviour {

	public GameObject awardItem;

	//获得emailInfo
	public void GetEmailInfo (EmailInfo emailInfo)
	{
		if (emailInfo.goodsList != null)
		{
			for (int i = 0;i < emailInfo.goodsList.Count;i ++)
			{
				GameObject awardObj = (GameObject)Instantiate (awardItem);
				
				awardObj.SetActive (true);
				awardObj.name = "AwardItem";
				
				awardObj.transform.parent = awardItem.transform.parent;
				
				if (emailInfo.goodsList.Count < 7)
				{
					awardObj.transform.localPosition = new Vector3 (i * 115f - (emailInfo.goodsList.Count - 1) * 57.5f + 35f,0,0);
				}
				
				else if (emailInfo.goodsList.Count == 7)
				{
					awardObj.transform.localPosition = new Vector3 (i * 115f - (emailInfo.goodsList.Count - 1) * 57.5f,0,0);
				}
				
				awardObj.transform.localScale = awardItem.transform.localScale;
				
				EmailAwardItem emailAward = awardObj.GetComponent<EmailAwardItem> ();
				emailAward.GetEmailAwardInfo (emailInfo.goodsList[i]);
			}
		}
	}
}

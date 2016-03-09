using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailCheck : MonoBehaviour {

	private EmailInfo emailInfo;

	public enum EmailOperateType
	{
		NO_ACTION = 0,//无操作
		DELATE_AFTER_LOOK = 1,//阅后即删
		DELATE_AFTER_GET = 2,//领取即删
		DELATE_AFTER_OPERATE = 3,//操作即删
		TURN_TO_ALLIANCE = 4//加入联盟后跳转
	}

	private EmailOperateType operateType = EmailOperateType.NO_ACTION;

	private string[][] btnTextLength = new string[][]{new string[]{"确定"},new string[]{"屏蔽","加好友","回复"},new string[]{"拒绝","同意"},
													new string[]{"领取"},new string[]{"跳转"}};

	public List<EventHandler> btnList = new List<EventHandler> ();

	public GameObject systemObj;
	public GameObject privateObj;

	public UILabel privateLabel;
	public UILabel privateSenderLabel;
	public UILabel systemLabel;
	public UILabel systemSenderLabel;

	public GameObject rewardListObj;
	public GameObject rewardItemObj;
	private List<GameObject> rewardItemList = new List<GameObject> ();
	private List<NGUILongPress> longPressList = new List<NGUILongPress> ();
	public UILabel desLabel;

	public UIAtlas equipAtlas;
	public UIAtlas fuWenAtlas;

	public void GetEmailInfo (EmailInfo tempInfo)
	{
		emailInfo = tempInfo;
		EmailTemp emailTemp = EmailTemp.getEmailTempByType(emailInfo.type);
		operateType = (EmailOperateType)Enum.ToObject (typeof (EmailOperateType),emailTemp.operateType);

		if (tempInfo.type == 80000)//私信
		{
			systemObj.SetActive (false);
			privateObj.SetActive (true);

			privateLabel.text = "       " + MyColorData.getColorString (3,emailInfo.content);
			privateSenderLabel.text = MyColorData.getColorString (3,emailInfo.senderName);
			privateSenderLabel.transform.localPosition = new Vector3(795,privateLabel.height >= 370 ? (-360 - (privateLabel.height - 370)) : -320, 0);

			for (int i = 0;i < btnList.Count;i ++)
			{
				btnList[i].gameObject.SetActive (true);
				UILabel btnLabel = btnList[i].GetComponentInChildren<UILabel> ();
				btnLabel.text = btnTextLength[1][i];
			}

			//判断是否是好友，显示加好友按钮
			IsAddFriend (FriendOperationData.Instance.friendIdList.Contains (emailInfo.jzId));
			//判断是否在黑名单里
			bool isShield = BlockedData.Instance().m_BlockedInfoDic.ContainsKey (emailInfo.jzId);
			IsJoinBlackList (isShield);
		}
		else
		{
			systemObj.SetActive (true);
			privateObj.SetActive (false);

			string taiTouStr = MyColorData.getColorString (3,emailInfo.taiTou);
			string contextStr = "\n     " + MyColorData.getColorString (3,emailInfo.content);
			systemLabel.text = taiTouStr + contextStr;
			systemSenderLabel.text = MyColorData.getColorString (3,emailInfo.senderName);
			systemSenderLabel.transform.localPosition = new Vector3(795, systemLabel.height >= 270 ? (-42 - systemLabel.height) : -238, 0);

			bool isShow = operateType == EmailOperateType.DELATE_AFTER_OPERATE ? true : false;

			btnList [0].gameObject.SetActive (isShow);
			btnList [1].gameObject.SetActive (!isShow);
			btnList [2].gameObject.SetActive (isShow);

			rewardListObj.SetActive (operateType == EmailOperateType.DELATE_AFTER_GET ? true : false);
			desLabel.text = operateType == EmailOperateType.DELATE_AFTER_GET ? "" : "无附件";

			switch (operateType)
			{
			case EmailOperateType.NO_ACTION:

				btnList[1].GetComponentInChildren<UILabel> ().text = btnTextLength[0][0];

				break;
			case EmailOperateType.DELATE_AFTER_LOOK:

				btnList[1].GetComponentInChildren<UILabel> ().text = btnTextLength[0][0];

				break;
			case EmailOperateType.DELATE_AFTER_GET:

				btnList[1].GetComponentInChildren<UILabel> ().text = btnTextLength[3][0];
				//创建奖励物品
				CreateRewardList (tempInfo.goodsList);

				break;
			case EmailOperateType.DELATE_AFTER_OPERATE:

				btnList[0].GetComponentInChildren<UILabel> ().text = btnTextLength[2][0];
				btnList[2].GetComponentInChildren<UILabel> ().text = btnTextLength[2][1];

				break;
			case EmailOperateType.TURN_TO_ALLIANCE:

				btnList[1].GetComponentInChildren<UILabel> ().text = btnTextLength[4][0];

				break;
			default:
				break;
			}
		}

		foreach (EventHandler handler in btnList)
		{
			handler.m_click_handler -= BtnHandlerCallBack;
			handler.m_click_handler += BtnHandlerCallBack;
		}
	}

	void CreateRewardList (List<EmailGoods> tempList)
	{
		int createCount = tempList.Count - rewardItemList.Count;
		int exitCount = rewardItemList.Count;

		if (createCount > 0)
		{
			for (int i = 0;i < createCount;i ++)
			{
				GameObject rewardItem = (GameObject)Instantiate (rewardItemObj);

				rewardItem.transform.parent = rewardListObj.transform;
				rewardItem.transform.localPosition = Vector3.zero;
				rewardItem.transform.localScale = Vector3.one;
				rewardItemList.Add (rewardItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (createCount);i ++)
			{
				Destroy (rewardItemList[rewardItemList.Count - 1]);
				rewardItemList.Remove (rewardItemList[rewardItemList.Count - 1]);
			}
		}

		for (int i = 0;i < tempList.Count;i ++)
		{
			rewardItemList[i].SetActive (true);
			rewardItemList[i].transform.localPosition = new Vector3 (i * 115f - (tempList.Count - 1) * 57.5f + (tempList.Count < 7 ? 35f : 0f),0,0);
			rewardItemList[i].name = i.ToString ();
//			Debug.Log ("tempList[i].id:" + tempList[i].id);

			EmailReward reward = rewardItemList[i].GetComponent<EmailReward> ();
			reward.GetRewardInfo (tempList[i]);
		}
	}

	//是否添加了好友
	public void IsAddFriend (bool isAdd)
	{
		btnList[1].gameObject.SetActive (!isAdd);
	}

	//是否加入了黑名单
	public void IsJoinBlackList (bool isJoin)
	{
		BoxCollider box = btnList[0].GetComponent<BoxCollider> ();
		box.enabled = !isJoin;
		UISprite btnSprite = btnList[0].GetComponent<UISprite> ();
		btnSprite.color = isJoin ? Color.gray : Color.white;
	}

	void BtnHandlerCallBack (GameObject obj)
	{
		if (emailInfo.type == 80000)
		{
			switch (obj.name)
			{
			case "Btn1"://屏蔽

				NewEmailData.Instance().EmailOperate (NewEmailData.LetterOperateType.SHIELD,emailInfo);

				break;
			case "Btn2"://加好友

				FriendOperationData.Instance.AddFriends (FriendOperationData.AddFriendType.Email,emailInfo.jzId,emailInfo.senderName);

				break;
			case "Btn3"://回复

				NewEmailData.Instance().SendName = emailInfo.senderName;
				NewEmailData.Instance().SendEmailType = NewEmailData.SendType.REPLY;
				EmailPage.emailPage.ShowEmailPage (EmailPage.EmailShowPage.EMAIL_SEND);

				break;
			default:
				break;
			}
		}
		else
		{
			if (operateType == EmailOperateType.DELATE_AFTER_OPERATE)
			{
				switch (obj.name)
				{
				case "Btn1":
					
					//DELATE_AFTER_OPERATE :拒绝
					NewEmailData.Instance().EmailOperate (NewEmailData.LetterOperateType.REFUSE,emailInfo);
					
					break;
					
				case "Btn3":
					
					//DELATE_AFTER_OPERATE : 同意
					NewEmailData.Instance().EmailOperate (NewEmailData.LetterOperateType.AGREE,emailInfo);
					
					break;
				default:
					break;
				}
				EmailPage.emailPage.BackBtnCallBack ();
			}
			else
			{
				if (obj.name == "Btn2")
				{
					if (operateType == EmailOperateType.TURN_TO_ALLIANCE)
					{
						//跳转到联盟城
						EmailPage.emailPage.CloseEmail ();
					}
					else
					{
						if (operateType == EmailOperateType.DELATE_AFTER_GET)
						{
							//发送领取奖励请求
							NewEmailData.Instance().GetEmailReward (emailInfo.id);
						}
						EmailPage.emailPage.BackBtnCallBack ();
					}
				}
			}
		}
	}
}

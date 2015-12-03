using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class NewEmailData : MonoBehaviour,SocketProcessor {

	private static NewEmailData emailData;

	public static NewEmailData Instance ()
	{
		if (emailData == null)
		{
			GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();
			
			emailData = t_GameObject.AddComponent<NewEmailData>();
		}

		return emailData;
	}

	//邮箱打开方式
	public enum EmailOpenType
	{
		EMAIL_MAIN_PAGE,
		EMAIL_REPLY_PAGE,
	}
	private EmailOpenType emailOpenType = EmailOpenType.EMAIL_MAIN_PAGE;

	private List<EmailInfo> systemList = new List<EmailInfo> ();
	private List<EmailInfo> privateList = new List<EmailInfo> ();

	private GameObject emailObj;
	private bool isEmailReq = false;

	private EmailPage.EmailShowType readType = EmailPage.EmailShowType.SYSTEM;//邮件读取类型

	public enum SendType
	{
		DEFAULT,
		REPLY,
		REPLY_FROM_FRIEND,
	}
	private SendType sendType = SendType.DEFAULT;//发送邮件打开方式
	public SendType SendEmailType
	{
		set{sendType = value;}
		get{return sendType;}
	}

	private string sendName;
	public string SendName
	{
		set{sendName = value;}
		get{return sendName;}
	}
	private string sendContent;
	public string SendContent
	{
		set{sendContent = value;}
		get{return sendContent;}
	}

	private string[] sendFailLength = new string[]{"失败，玩家名空！","失败，内容为空！",
												   "失败，找不到玩家！","失败，有非法字符！","你被对方屏蔽！",
												   "不能给自己发邮件！","间隔时间不到1分钟！","收件人在黑名单中！"};

	private string textStr;
	
	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	public void LoadEmailPrefab ()
	{
		if (emailObj == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EMAIL),
			                        EmailPrefabLoadCallback);
		}
	}
	void EmailPrefabLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		emailObj = Instantiate(p_object) as GameObject;
		emailObj.SetActive (false);
//		DontDestroyOnLoad (emailObj);
	}

	#region OpenEmail
	/// <summary>
	/// Opens the email.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempName">Temp name.</param>
	public void OpenEmail (EmailOpenType tempType,string tempName = null)
	{
		emailOpenType = tempType;

		SendName = tempName != null ? tempName : SendName;

//		Debug.Log ("replyName:" + SendName);

		SendEmailType = tempType == EmailOpenType.EMAIL_REPLY_PAGE ? SendType.REPLY_FROM_FRIEND : SendType.DEFAULT;

		if (emailObj == null)
		{
			Debug.LogError ("EmailPrefab is null!");
			return;
		}

		if (!isEmailReq)
		{
			QXComData.SendQxProtoMessage (ProtoIndexes.C_REQ_MAIL_LIST,ProtoIndexes.S_REQ_MAIL_LIST.ToString ());
//			Debug.Log ("EmailDataReq:" + ProtoIndexes.C_REQ_MAIL_LIST);
		}
		else
		{
			OpenEmailBack ();
		}
	}
	void OpenEmailBack ()
	{
		emailObj.SetActive (true);
		MainCityUI.TryAddToObjectList (emailObj);
		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}

		EmailPage.emailPage.GetEmailResp (emailOpenType,systemList,privateList);
	}
	#endregion

	#region ReadEmail
	/// <summary>
	/// Reads the email.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	public void ReadEmail (long tempId,EmailPage.EmailShowType tempType)
	{
		readType = tempType;

		ReadEmail readReq = new ReadEmail ();
		readReq.id = tempId;

		QXComData.SendQxProtoMessage (readReq,ProtoIndexes.C_READED_EAMIL,ProtoIndexes.S_READED_EAMIL.ToString ());
		Debug.Log ("ReadEmail:" + ProtoIndexes.C_READED_EAMIL);
	}
	#endregion

	#region GetEmailReward
	/// <summary>
	/// Gets the email reward.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	public void GetEmailReward (long tempId)
	{
		GetRewardRequest getAwardReq = new GetRewardRequest();
		getAwardReq.id = tempId;
		QXComData.SendQxProtoMessage (getAwardReq,ProtoIndexes.C_MAIL_GET_REWARD,ProtoIndexes.S_MAIL_GET_REWARD.ToString ());
		Debug.Log("GetAwardEmail:" + ProtoIndexes.C_MAIL_GET_REWARD);
	}
	#endregion

	#region EmailOperate
	public enum LetterOperateType
	{
		AGREE = 1,//同意
		REFUSE = 2,//拒绝
		SHIELD = 5,//屏蔽玩家
	}
	private LetterOperateType letterOperateType = LetterOperateType.AGREE;

	private EmailInfo operateEmail;
	/// <summary>
	/// Emails the operate.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	/// <param name="tempOperateCode">Temp operate code.</param>
	public void EmailOperate (LetterOperateType tempType,EmailInfo tempEmail)
	{
		letterOperateType = tempType;
		operateEmail = tempEmail;

		EmailResponse responseReq = new EmailResponse();
		responseReq.emailId = tempEmail.id;
		responseReq.operCode = (int)tempType;
		QXComData.SendQxProtoMessage (responseReq,ProtoIndexes.C_EMAIL_RESPONSE,ProtoIndexes.S_EMAIL_RESPONSE.ToString ());
		Debug.Log("EmailResponseReq:" + ProtoIndexes.C_EMAIL_RESPONSE);
	}
	#endregion

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_REQ_MAIL_LIST://邮件返回
			{
				EmailListResponse emailDataRes = new EmailListResponse ();
				emailDataRes = QXComData.ReceiveQxProtoMessage (p_message,emailDataRes) as EmailListResponse;
				
				if (emailDataRes != null)
				{
					if (emailDataRes.emailList == null)
					{
						emailDataRes.emailList = new List<EmailInfo>();
					}

					foreach (EmailInfo email in emailDataRes.emailList)
					{	
						if (email.type == 80000)//私信
						{
							privateList.Add (email);
						}
						else //系统邮件
						{
							systemList.Add (email);
						}
					}

					isEmailReq = true;
					OpenEmailBack ();
				}
				
				return true;
			}
			
			case ProtoIndexes.S_READED_EAMIL://读取邮件返回
			{
				Debug.Log ("ReadEmailBack:" + ProtoIndexes.S_READED_EAMIL);
				ReadEmailResp readResp = new ReadEmailResp ();
				readResp = QXComData.ReceiveQxProtoMessage (p_message,readResp) as ReadEmailResp;

				if (readResp != null)
				{
					switch (readType)
					{
					case EmailPage.EmailShowType.SYSTEM:

						systemList = ReadEmailCallBack (systemList,readResp.emailId,readResp.result);
						EmailPage.emailPage.RefreshEmailList (readType,systemList);

						break;
					case EmailPage.EmailShowType.PRIVATE:

						privateList = ReadEmailCallBack (privateList,readResp.emailId,readResp.result);
						EmailPage.emailPage.RefreshEmailList (readType,privateList);

						break;
					default:
						break;
					}
				}

				return true;
			}

			case ProtoIndexes.S_MAIL_NEW://收到一封新邮件
			{
				Debug.Log ("NewEmail:" + ProtoIndexes.S_MAIL_NEW);
				
				NewMailResponse newEmailResp = new NewMailResponse ();
				newEmailResp = QXComData.ReceiveQxProtoMessage (p_message,newEmailResp) as NewMailResponse;
				
				if (newEmailResp != null)
				{
					MainCityUIL.ShowEmailDetail(true,LanguageTemplate.GetText (LanguageTemplate.Text.NEW_EMAIL));

					if (newEmailResp.email.type == 80000)
					{
						privateList.Add (newEmailResp.email);

						if (emailObj != null && emailObj.activeSelf)
						{
							EmailPage.emailPage.RefreshEmailList (EmailPage.EmailShowType.PRIVATE,privateList);
						}
					}
					else
					{
						systemList.Add (newEmailResp.email);

						if (emailObj != null && emailObj.activeSelf)
						{
							EmailPage.emailPage.RefreshEmailList (EmailPage.EmailShowType.SYSTEM,systemList);
						}
					}
					ExistNewEmail ();
				}
				return true;
			}

			case ProtoIndexes.S_MAIL_GET_REWARD://领奖返回
			{
				Debug.Log("GetRewardRes：" + ProtoIndexes.S_MAIL_GET_REWARD);

				GetRewardResponse getAwardResp = new GetRewardResponse ();
				getAwardResp = QXComData.ReceiveQxProtoMessage (p_message,getAwardResp) as GetRewardResponse;

				if (getAwardResp != null)
				{
					//删除邮件
					systemList = GetRewardCallBack (getAwardResp.id,getAwardResp.isSuccess);
					EmailPage.emailPage.ShowEmailPage (EmailPage.EmailShowPage.EMAIL_MAINPGE);
					EmailPage.emailPage.RefreshEmailList (EmailPage.EmailShowType.SYSTEM,systemList);
				}
				
				return true;
			}

			case ProtoIndexes.S_EMAIL_RESPONSE://邮件响应操作返回
			{
				Debug.Log("EmailResponse：" + ProtoIndexes.S_EMAIL_RESPONSE);

				EmailResponseResult responseResp = new EmailResponseResult();
				responseResp = QXComData.ReceiveQxProtoMessage (p_message,responseResp) as EmailResponseResult;
				
				if (responseResp != null)//isSuccess ： 0-成功 1-失败
				{
					if (letterOperateType == LetterOperateType.SHIELD)
					{
						if (responseResp.isSuccess == 0)
						{
							BlackJunzhuInfo tempJunZhu = new BlackJunzhuInfo();
							{
								tempJunZhu.junzhuId = operateEmail.jzId;
							}
							BlockedData.Instance ().m_BlockedInfoDic.Add (tempJunZhu.junzhuId,tempJunZhu);
							//刷新查看邮件界面
							EmailPage.emailPage.RefreshEmailCheck (EmailPage.RefreshType.SHIELD_FRIEND);
							textStr = "您已将" + operateEmail.senderName + "加入黑名单！\n\n您可以在【好友】-【屏蔽名单】功能中解除黑名单设置";
							QXComData.CreateBox (1,textStr,true,null);
						}
						else
						{
							//删除该邮件
							privateList = LetterOperateCallBack (LetterOperateType.SHIELD,1);
							EmailPage.emailPage.ShowEmailPage (EmailPage.EmailShowPage.EMAIL_MAINPGE);
							EmailPage.emailPage.RefreshEmailList (EmailPage.EmailShowType.PRIVATE,privateList);
						}
					}
					else
					{
						systemList = LetterOperateCallBack (LetterOperateType.SHIELD,responseResp.isSuccess);
						EmailPage.emailPage.ShowEmailPage (EmailPage.EmailShowPage.EMAIL_MAINPGE);
						EmailPage.emailPage.RefreshEmailList (EmailPage.EmailShowType.SYSTEM,systemList);
					}
				}
				
				return true;
			}

			case ProtoIndexes.S_SEND_EAMIL://发送邮件返回
			{
				Debug.Log("EmailSendResp：" + ProtoIndexes.S_SEND_EAMIL);
				SendEmailResp sendResp = new SendEmailResp();
				sendResp = QXComData.ReceiveQxProtoMessage (p_message,sendResp) as SendEmailResp;
				
				if (sendResp != null)
				{
					Debug.Log ("发送结果：" + sendResp.result);
					SendEmailCallBack (sendResp.result);
				}
				
				return true;
			}
			default:return false;
			}
		}
		
		return false;
	}
	
	/// <summary>
	/// Reads the email call back.
	/// </summary>
	/// <returns>The email call back.</returns>
	/// <param name="tempList">Temp list.</param>
	/// <param name="tempReadId">Temp read identifier.</param>
	/// <param name="tempResult">Temp result.</param>
	private List<EmailInfo> ReadEmailCallBack (List<EmailInfo> tempList,long tempReadId,int tempResult)
	{
		foreach (EmailInfo email in tempList)
		{
			if (email.id == tempReadId)
			{
				switch (tempResult)
				{
				case 0:

					EmailPage.emailPage.OpenCheckEmail (email);
					EmailTemp emailTemp = EmailTemp.getEmailTempByType(email.type);
					if (emailTemp.operateType == (int)EmailCheck.EmailOperateType.DELATE_AFTER_LOOK)//阅后即删
					{
						tempList.Remove (email);
					}
					else
					{
						email.isRead = 1;
					}
					
					break;
				case 1:
					
					tempList.Remove (email);
					string textStr = "邮件不存在!";
					QXComData.CreateBox (1,textStr,true,null);
					
					break;
				default:
					break;
				}

				break;
			}
		}
		return tempList;
	}

	/// <summary>
	/// Gets the reward call back.
	/// </summary>
	/// <returns>The reward call back.</returns>
	/// <param name="tempId">Temp identifier.</param>
	/// <param name="tempResult">Temp result.</param>
	private List<EmailInfo> GetRewardCallBack (long tempId,int tempResult)
	{
		EmailInfo tempEmail = new EmailInfo();
		foreach (EmailInfo email in systemList)
		{
			if (email.id == tempId)
			{
				tempEmail = email;
				break;
			}
		}

		if (tempResult == 0)
		{
//			textStr = "n恭喜领取奖励成功";
			List<RewardData> dataList = new List<RewardData>();
			for (int m = 0;m < tempEmail.goodsList.Count;m ++)
			{
				RewardData data = new RewardData(tempEmail.goodsList[m].id,tempEmail.goodsList[m].count);
				dataList.Add (data);
			}
			
			GeneralRewardManager.Instance ().CreateReward (dataList);
		}
		else
		{
			switch (tempResult)
			{
			case 1:
				textStr = "没有该邮件！";
				break;
			case 2:
				textStr = "您已经领取过奖励！";
				break;
			default:
				break;
			}
			QXComData.CreateBox (1,textStr,true,null);
		}

		systemList.Remove (tempEmail);

		return systemList;
	}

	/// <summary>
	/// Letters the operate call back.
	/// </summary>
	/// <returns>The operate call back.</returns>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempResult">Temp result.0-成功 1-失败</param>
	private List<EmailInfo> LetterOperateCallBack (LetterOperateType tempType,int tempResult)
	{
		List<EmailInfo> tempEmailList = new List<EmailInfo> ();
		switch (tempType)
		{
		case LetterOperateType.AGREE:

			tempEmailList = systemList;
			textStr = tempResult == 0 ? "您已同意这次房屋交换申请！" : "交换失败！该邮件不存在！";

			break;
		case LetterOperateType.REFUSE:

			tempEmailList = systemList;
			textStr = tempResult == 0 ? "您已拒绝这次房屋交换申请！" : "拒绝交换失败！该邮件不存在！";

			break;
		case LetterOperateType.SHIELD:
			if (tempResult == 1)
			{
				tempEmailList = privateList;
				textStr = "屏蔽失败！该邮件不存在！";
			}
			break;
		default:
			break;
		}

		QXComData.CreateBox (1,textStr,true,null);

		foreach (EmailInfo email in tempEmailList)
		{
			if (email.id == operateEmail.id)
			{
				tempEmailList.Remove (email);
				break;
			}
		}
		return tempEmailList;
	}

	/// <summary>
	/// Sends the email call back.
	/// </summary>
	/// <param name="tempResult">Temp result.</param>
	private void SendEmailCallBack (int tempResult)
	{
		//0-发送成功
		//1-失败，玩家名空
		//2-失败，内容为空
		//3-失败，找不到玩家 
		//4-失败，有非法字符
		//5-你被对方屏蔽
		//6-不能给自己发
		//7-间隔时间不到1分钟
		//8-收件人在黑名单中
		
		if (tempResult == 0)
		{
			textStr = "发送成功！";
			QXComData.CreateBox (1,textStr,true,SendSuccessCallBack);
			//更新发送按钮状态
			EmailPage.emailPage.RefreshEmailSendBtnState (3);
		}
		else
		{
			textStr = sendFailLength[tempResult - 1];
			QXComData.CreateBox (1,textStr,true,null);
			EmailPage.emailPage.RefreshEmailSendBtnState (1);
		}
	}
	void SendSuccessCallBack (int i)
	{
		//关闭发送页面
		if (emailOpenType == EmailOpenType.EMAIL_REPLY_PAGE)
		{
			EmailPage.emailPage.CloseEmail ();
		}
		else
		{
			EmailPage.emailPage.GetSendEmailInfo ();
			EmailPage.emailPage.ShowEmailPage (EmailPage.EmailShowPage.EMAIL_MAINPGE);
			EmailPage.emailPage.CheckUnSendEmail ();
		}
	}

	//判断是否存在未读邮件
	public void ExistNewEmail ()
	{
		bool isExitNewEmail = false;
		foreach (EmailInfo email in systemList)
		{
			if (email.isRead == 0)
			{
				PushAndNotificationHelper.SetRedSpotNotification (41,true);
				isExitNewEmail = true;
				break;
			}
			else
			{
				PushAndNotificationHelper.SetRedSpotNotification (41,false);
			}
		}
		if (!isExitNewEmail)
		{
			foreach (EmailInfo email in privateList)
			{
				if (email.isRead == 0)
				{
					PushAndNotificationHelper.SetRedSpotNotification (10,true);
					isExitNewEmail = true;
					break;
				}
				else
				{
					PushAndNotificationHelper.SetRedSpotNotification (10,false);
				}
			}
		}
		MainCityUIL.SetRedAlert("email",isExitNewEmail);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

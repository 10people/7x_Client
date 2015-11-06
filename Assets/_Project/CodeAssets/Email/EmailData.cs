using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailData : MonoBehaviour,SocketProcessor {

	private static EmailData emailData;

	public List<EmailInfo> emailRespList = new List<EmailInfo>();

	[HideInInspector]public string replyName;
	[HideInInspector]public string content;

	public static EmailData Instance()
	{
		if ( emailData == null )
		{
			GameObject t_GameObject = UtilityTool.GetDontDestroyOnLoadGameObject();
			
			emailData = t_GameObject.AddComponent< EmailData >();
		}
		
		return emailData;
	}

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// 邮箱请求
	/// </summary>
	public void EmailReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_REQ_MAIL_LIST,"25004");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_REQ_MAIL_LIST://邮件返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				EmailListResponse emailInfoResp = new EmailListResponse();
				
				t_qx.Deserialize(t_stream, emailInfoResp, emailInfoResp.GetType());

				if (emailInfoResp != null)
				{
					if (emailInfoResp.emailList != null)
					{
						emailRespList = emailInfoResp.emailList;

//						Debug.Log ("EmailList.count:" + emailInfoResp.emailList.Count);
					}
					else
					{
//						Debug.Log ("emailInfoResp is NULL!");

						emailRespList.Clear ();
					}

					ExistNewEmail (false);
				}

				return true;
			}

			case ProtoIndexes.S_MAIL_NEW://收到一封新邮件
			{
				Debug.Log ("NewEmail:" + ProtoIndexes.S_MAIL_NEW);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				NewMailResponse newEmailResp = new NewMailResponse();
				
				t_qx.Deserialize(t_stream, newEmailResp, newEmailResp.GetType());
				
				if (newEmailResp != null)
				{
//					EmailReq ();
					emailRespList.Add (newEmailResp.email);

					ExistNewEmail (true);

					AddNewEmailIntoList (newEmailResp.email);
				}
				return true;
			}

//			case ProtoIndexes.S_DELETE_MAIL:
//			{
//				Debug.Log ("删除一封邮件：" + ProtoIndexes.S_DELETE_MAIL);
//				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				DeleteEmailResp deleteResp = new DeleteEmailResp();
//				
//				t_qx.Deserialize(t_stream, deleteResp, deleteResp.GetType());
//				
//				if (deleteResp != null)
//				{
//					DeleteEmail (deleteResp.id);
//				}
//				
//				return true;
//			}

			default:return false;
			}
		}

		return false;
	}

	//实例化新邮件
	public void AddNewEmailIntoList (EmailInfo tempInfo)
	{
		GameObject emailObj = GameObject.Find ("Email(Clone)");
		
		if (emailObj != null)
		{
			EmailManager emailMan = emailObj.GetComponent<EmailManager> ();
			emailMan.AddNewEmailIntoList (tempInfo);
		}
	}

	//刷新邮件list
	public void RefreshEmailRespList (int result,long emailId)
	{
		for (int i = 0;i < emailRespList.Count;i ++)
		{
			if (emailId == emailRespList[i].id)
			{
				if (result == 0)
				{	
					EmailTemp emailTemp = EmailTemp.getEmailTempByType(emailRespList[i].type);
					int emailColType = emailTemp.operateType;
					
					if (emailColType == 1)//阅后即删
					{
						emailRespList.Remove (emailRespList[i]);
					}
					else
					{
						emailRespList[i].isRead = 1;
					}
				}
				
				else if (result == 1)
				{
					emailRespList.Remove (emailRespList[i]);
				}
			}
		}
	}

	//判断是否存在未读邮件
	public void ExistNewEmail (bool isNewEmail)
	{
		List<EmailInfo> UnReadEmailList = new List<EmailInfo>();

		for (int i = 0;i < emailRespList.Count;i ++)
		{
			if (emailRespList[i].isRead == 0)
			{
				UnReadEmailList.Add (emailRespList[i]);
			}
		}
//		Debug.Log ("IsExistNewEmail:" + UnReadEmailList.Count);
		if (UnReadEmailList.Count > 0)
		{
			MainCityUIL.SetRedAlert("email",true);
			if (isNewEmail)
			{
				MainCityUIL.ShowEmailDetail(true,LanguageTemplate.GetText (LanguageTemplate.Text.NEW_EMAIL));
			}
		}
		else
		{
			MainCityUIL.SetRedAlert("email",false);
		}
	}

	//回信
	public void ReplyLetter (string name)
	{
		replyName = name;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EMAIL),
		                        EmailLoadCallback);
	}

	void EmailLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;

		EmailManager email = tempObject.GetComponent<EmailManager> ();
		email.GoodFriedReply (replyName);
		
		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

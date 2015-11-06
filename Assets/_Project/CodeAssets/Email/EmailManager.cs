using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailManager : MonoBehaviour,SocketProcessor
{
    public ScaleEffectController m_ScaleEffectController;

	public static EmailManager emailMan;

	public GameObject systemObj;//系统
	public GameObject systemBtn;

	public GameObject privateObj;//私信
	public GameObject privateBtn;

	public GameObject writeObj;//写信
	public GameObject writeBtn;

	private List<EmailInfo> systemEmailList = new List<EmailInfo> ();//系统邮件列表
	private List<EmailInfo> privateEmailList = new List<EmailInfo> ();//私信列表
	
	public GameObject checkEmailObj;//查看邮件页面

	public GameObject systemWarning;
	public GameObject privateWarning;
	public GameObject writeWarning;

	private bool isCheck = false;//是否已点开一封邮件
	public bool IsCheck
	{
		set{isCheck = value;}
		get{return isCheck;}
	}

	void Awake ()
	{
		emailMan = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		GetEmailResp (EmailData.Instance ().emailRespList);

		SystemBtn ();
	}

	//获得邮件返回信息
	void GetEmailResp (List<EmailInfo> emailInfoList)
	{
		systemEmailList.Clear ();
		privateEmailList.Clear ();
		
		foreach (EmailInfo email in emailInfoList)
		{	
			if (email.type == 80000)//私信
			{
				privateEmailList.Add (email);
			}
			
			else //系统邮件
			{
				systemEmailList.Add (email);
			}
		}
		
		PrivateEmail pEmail = privateObj.GetComponent<PrivateEmail> ();
		pEmail.GetPrivateLetterList (privateEmailList);
		
		SystemEmail sEmail = systemObj.GetComponent<SystemEmail> ();
		sEmail.GetSystemEmailList (systemEmailList);

		CheckUnReadEmail (privateEmailList,privateWarning);
		CheckUnReadEmail (systemEmailList,systemWarning);

		if (!string.IsNullOrEmpty (EmailData.Instance ().replyName) || !string.IsNullOrEmpty (EmailData.Instance ().content))
		{
			CheckDraft (true);
		}
		else
		{
			CheckDraft (false);
		}
	}

	//检测未读邮件
	void CheckUnReadEmail (List<EmailInfo> tempList,GameObject warnObj)
	{
		List<EmailInfo> unReadList = new List<EmailInfo> ();

		foreach (EmailInfo email in tempList)
		{
			if (email.type == 80000)
			{
				if (email.isRead == 0)
				{
					unReadList.Add (email);
				}
			}
			
			else
			{
				if (email.isRead == 0)
				{
					unReadList.Add (email);
				}
			}
		}

		if (unReadList.Count > 0)
		{
			warnObj.SetActive (true);
		}
		else
		{
			warnObj.SetActive (false);
		}
	}

	//检测是否有未发邮件
	public void CheckDraft (bool flag)
	{
		writeWarning.SetActive (flag);
	}

	//点击系统按钮
	public void SystemBtn ()
	{
//		Debug.Log ("SystemBtn!");

		systemObj.SetActive (true);
		privateObj.SetActive (false);

		ActiveBtnAnimation (systemBtn,true);
		ActiveBtnAnimation (privateBtn,false);
		ActiveBtnAnimation (writeBtn,false);
	}

	//点击私信按钮
	public void PrivateBtn ()
	{
//		Debug.Log ("PrivateBtn!");

		systemObj.SetActive (false);
		privateObj.SetActive (true);

		ActiveBtnAnimation (systemBtn,false);
		ActiveBtnAnimation (privateBtn,true);
		ActiveBtnAnimation (writeBtn,false);
	}

	//点击写信按钮
	public void WriteBtn ()
	{
//		Debug.Log ("WriteBtn!");

		writeObj.SetActive (true);

		writeObj.transform.FindChild ("InPutLabel_Name").GetComponentInChildren<BoxCollider> ().enabled = true;

		writeObj.GetComponent<EmailSend> ().InItSendBtns ();

		writeObj.GetComponent<EmailSend> ().DefaultName ();

		ActiveBtnAnimation (systemBtn,false);
		ActiveBtnAnimation (privateBtn,false);
		ActiveBtnAnimation (writeBtn,true);
	}

	//放缩当前点击按钮
	void ActiveBtnAnimation (GameObject tempBtn,bool flag)
	{
		float time = 0.1f;
		float y = tempBtn.transform.localPosition.y;
		float z = tempBtn.transform.localPosition.z;
		iTween.EaseType type = iTween.EaseType.linear;

		Hashtable move = new Hashtable ();
		move.Add ("time",time);
		move.Add ("easetype",type);
		move.Add ("islocal",true);
		if (flag)
		{
			move.Add ("position",new Vector3 (-5,y,z));
		}
		else
		{
			move.Add ("position",new Vector3 (0,y,z));
		}
		iTween.MoveTo (tempBtn,move);

		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("easetype",type);
		if (flag)
		{
			scale.Add ("scale",Vector3.one * 1.1f);
		}
		else
		{
			scale.Add ("scale",Vector3.one);
		}
		iTween.ScaleTo (tempBtn,scale);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_READED_EAMIL://读取邮件返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ReadEmailResp readResp = new ReadEmailResp();
				
				t_qx.Deserialize(t_stream, readResp, readResp.GetType());
				
				if (readResp != null)
				{
					Debug.Log ("readResp.emailId:" + readResp.emailId);

					for (int i = 0;i < EmailData.Instance ().emailRespList.Count;i ++)
					{
						if (readResp.emailId == EmailData.Instance ().emailRespList[i].id)
						{
							if (EmailData.Instance ().emailRespList[i].type == 80000)
							{
								HandlePrivateEmail (readResp.result,readResp.emailId);
							}

							else
							{
								HandleSystemEmail (readResp.result,readResp.emailId);
							}
						}
					}

					EmailData.Instance ().RefreshEmailRespList (readResp.result,readResp.emailId);//更新邮件返回list
				}
				return true;
			}

			default:return false;
			}
		}

		return false;
	}

	//私信读取处理
	void HandlePrivateEmail (int result,long emailId)
	{
		Debug.Log ("privateEmailList.Count:" + systemEmailList.Count);
		for (int i = 0;i < privateEmailList.Count;i ++)
		{
			if (emailId == privateEmailList[i].id)
			{
				Debug.Log ("privateEmailList[i].id:" + privateEmailList[i].id);
				if (result == 0)
				{
					privateEmailList[i].isRead = 1;
					CreateCheckEmailObj (privateEmailList[i]);
				}
				else if (result == 1)
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ReadFailLoadBack);
					privateEmailList.Remove (privateEmailList[i]);
				}
				
				break;
			}
		}

		CheckUnReadEmail (privateEmailList,privateWarning);

		PrivateEmail pEmail = privateObj.GetComponent<PrivateEmail> ();
		pEmail.RefreshLetterList (privateEmailList);
	}

	//系统邮件读取处理
	void HandleSystemEmail (int result,long emailId)
	{
		Debug.Log ("systemEmailList.Count:" + systemEmailList.Count);
		for (int i = 0;i < systemEmailList.Count;i ++)
		{
			if (emailId == systemEmailList[i].id)
			{
				Debug.Log ("systemEmailList[i].id:" + systemEmailList[i].id);
				if (result == 0)
				{
					CreateCheckEmailObj (systemEmailList[i]);

					EmailTemp emailTemp = EmailTemp.getEmailTempByType(systemEmailList[i].type);
					int emailColType = emailTemp.operateType;

					if (emailColType == 1)//阅后即删
					{
						systemEmailList.Remove (systemEmailList[i]);
					}
					else
					{
						systemEmailList[i].isRead = 1;
					}
				}
				else if (result == 1)
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ReadFailLoadBack);
					systemEmailList.Remove (systemEmailList[i]);
				}

				break;
			}
		}

		CheckUnReadEmail (systemEmailList,systemWarning);

		SystemEmail sEmail = systemObj.GetComponent<SystemEmail> ();
		sEmail.RefreshEmailList (systemEmailList);
	}

	//系统邮件其它类型的删除操作 1-领取即删 2-操作即删
	public void DeletSystemEmail (long emailId)
	{
		for (int i = 0;i < EmailData.Instance ().emailRespList.Count;i ++)
		{
			if (emailId == EmailData.Instance ().emailRespList[i].id)
			{
				EmailData.Instance ().emailRespList.Remove (EmailData.Instance ().emailRespList[i]);
			}
		}

		for (int i = 0;i < systemEmailList.Count;i ++)
		{
			if (emailId == systemEmailList[i].id)
			{
				systemEmailList.Remove (systemEmailList[i]);
			}
		}

		CheckUnReadEmail (systemEmailList,systemWarning);

		SystemEmail sEmail = systemObj.GetComponent<SystemEmail> ();
		sEmail.RefreshEmailList (systemEmailList);
	}

	//添加进一封新邮件
	public void AddNewEmailIntoList (EmailInfo tempInfo)
	{
		if (tempInfo.type == 80000)
		{
			privateEmailList.Add (tempInfo);

			PrivateEmail pEmail = privateObj.GetComponent<PrivateEmail> ();
			pEmail.RefreshLetterList (privateEmailList);

			CheckUnReadEmail (privateEmailList,privateWarning);
		}
		else
		{
			systemEmailList.Add (tempInfo);

			SystemEmail sEmail = systemObj.GetComponent<SystemEmail> ();
			sEmail.RefreshEmailList (systemEmailList);

			CheckUnReadEmail (systemEmailList,systemWarning);
		}
	}

	void ReadFailLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "查看失败";
		string str = "\n\n邮件不存在!";

		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirm,null,
		             null);
	}

	public void CreateCheckEmailObj (EmailInfo tempInfo)
	{
		GameObject checkObj = (GameObject)Instantiate (checkEmailObj);
		
		checkObj.SetActive (true);
		
		checkObj.transform.parent = checkEmailObj.transform.parent;
		checkObj.transform.localPosition = checkEmailObj.transform.localPosition;
		checkObj.transform.localScale = checkEmailObj.transform.localScale;
		
		CheckEmail checkEmail = checkObj.GetComponent<CheckEmail> ();
		checkEmail.GetEmailInfo (tempInfo);
	}

	public void DestroyEmailRoot ()
	{
		EmailData.Instance().ExistNewEmail(false);
	    m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
	    m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = false;
        }

        MainCityUI.TryRemoveFromObjectList(gameObject);
        Destroy(this.gameObject);
    }

	//好友回复操作
	public void GoodFriedReply (string name)
	{
		writeObj.SetActive (true);

		EmailSend send = writeObj.GetComponent<EmailSend>();
		send.CleanInputLabel (3);
		send.FromGoodfriedReply (name);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

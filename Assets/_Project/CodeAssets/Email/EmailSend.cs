using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailSend : MonoBehaviour,SocketProcessor {

	public GameObject inputNameObj;
	public UIInput nameLabel;

	public GameObject inputContentObj;
	public UIInput contentLabel;

	public GameObject sendBtnText1;//发送前
	public GameObject sendBtnText2;//发送中
	public GameObject sendBtnText3;//发送完成

	private int resultType;//发送结果类型

	public UIScrollBar scrollBar;
	private int line;//label内容行数

	private bool isReply = false;//是否是回信

	public GameObject backBtnObj;

	public UISprite block;//遮罩

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		block.alpha = 0.1f;
		nameLabel.GetComponent<EventHandler> ().m_handler += InputName;
		block.GetComponent<EventHandler> ().m_handler += BlockSubmit;
	}

	//激活屏蔽层
	void InputName (GameObject obj)
	{
		#if UNITY_EDITOR
		Debug.Log ("Unity");
		#else
		Debug.Log ("Android or Ios");
		block.gameObject.SetActive (true);
		#endif
	}

	void BlockSubmit (GameObject obj)
	{
		NameSubmit ();
	}

	//提交名字
	public void NameSubmit ()
	{
		block.gameObject.SetActive (false);
		nameLabel.value = NewSelectRole.TextLengthLimit (NewSelectRole.StrLimitType.CREATE_ROLE_NAME,nameLabel.value);;
	}

	//默认名字
	public void DefaultName ()
	{
		nameLabel.text = EmailData.Instance.replyName;
		contentLabel.text = EmailData.Instance.content;
	}

	//获得好友名字
	public void FromGoodfriedReply (string nameStr)
	{
		nameLabel.text = nameStr;

		nameLabel.gameObject.GetComponent<BoxCollider> ().enabled = false;

		backBtnObj.SetActive (false);

		InItSendBtns ();
	}

	//获得回复人名字
	public void GetNameStr (string nameStr)
	{
		isReply = true;

		nameLabel.text = nameStr;
		nameLabel.gameObject.GetComponent<BoxCollider> ().enabled = false;

		InItSendBtns ();
	}

	//发送邮件请求
	public void SendBtn ()
	{
		if (!TimeHelper.Instance.IsTimeCalcKeyExist ("SendEmail"))
		{
			EmailSendReq ();
			TimeHelper.Instance.AddOneDelegateToTimeCalc ("SendEmail",10);
		}
		else
		{
			if (TimeHelper.Instance.IsCalcTimeOver ("SendEmail"))
			{
				EmailSendReq ();
			}
		}
	}

	void EmailSendReq ()
	{
		SendEmail sendReq = new SendEmail ();
		
		sendReq.receiverName = nameLabel.text;
		sendReq.content = contentLabel.text;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,sendReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_SEND_EAMIL,ref t_protof,"25012");
		Debug.Log ("SendEmail:" + ProtoIndexes.C_SEND_EAMIL);
		
		sendBtnText1.SetActive (false);
		sendBtnText2.SetActive (true);
		sendBtnText3.SetActive (false);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_SEND_EAMIL:

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				SendEmailResp sendResp = new SendEmailResp();
				
				t_qx.Deserialize(t_stream, sendResp, sendResp.GetType());

				if (sendResp != null)
				{
					Debug.Log ("发送结果：" + sendResp.result);

					resultType = sendResp.result;

					ShowTipsBox ();

					if (TimeHelper.Instance.IsTimeCalcKeyExist ("SendEmail"))
					{
						TimeHelper.Instance.RemoveFromTimeCalc ("SendEmail");
					}
				}

				return true;
			}
		}

		return false;
	}

	void ShowTipsBox ()//0-发送成功，1-失败，玩家名空，2-失败，内容为空，3-失败，找不到玩家 、4-失败，有非法字符5-你被对方屏蔽，6-不能给自己发,7-间隔时间不到1分钟，8-收件人在黑名单中
	{
		if (resultType == 0)
		{
			Debug.Log ("发送成功");
			sendBtnText1.SetActive (false);
			sendBtnText2.SetActive (false);
			sendBtnText3.SetActive (true);

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        SendSuccessBack);
		}

		else
		{
			sendBtnText1.SetActive (true);
			sendBtnText2.SetActive (false);
			sendBtnText3.SetActive (false);

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        SendFailBack);
		}
	}

	//发送成功弹窗加载
	void SendSuccessBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "发送成功";
		
		string str = "\n\n邮件发送成功！";
		
		string confirmStr = "确定";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,
		             SendSuccess);
	}
	void SendSuccess (int i)
	{
		CleanInputLabel (3);
		BackToMainPage ();
	}

	//发送失败弹窗加载
	void SendFailBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "发送失败";
		
		string str = "";

		switch (resultType)
		{
		case 1:

			str = "\n邮差不知道要寄给谁...\n赶快添上收件人名字吧~";

			break;

		case 2:

			str = "\n邮差不管送空邮件哦...\n赶快写点什么吧~";

			break;

		case 3:

			str = "\n\n很遗憾，找不到这个玩家...";

			break;

		case 4:

			str = "\n有奇怪的文字混进来了...\n再推敲一下吧！";

			break;

		case 5:

			str = "\n\n您已被该玩家屏蔽！";

			break;

		case 6:

			str = "\n\n无法向自己发送邮件！";

			break;

		case 7:

			str = "\n\n间隔时间不到1分钟，请稍后再发！";

			break;

		case 8:

			str = "\n\n不能对已屏蔽的玩家发送或回复邮件，\n在【好友】-【屏蔽名单】内可解除屏蔽。";

			break;

		default:break;
		}

		string confirmStr = "确定";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,
		             SendFail);
	}
	void SendFail (int i)
	{
		Debug.Log ("发送失败！");
	}

	//清空输入框
	public void CleanInputLabel (int i)
	{
		if (i == 1)
		{
			nameLabel.text = "";
		}

		else if (i == 2)
		{
			contentLabel.text = "";
		}

		else if (i == 3)
		{
			nameLabel.text = "";
			contentLabel.text = "";
		}
	}

	//恢复发送按钮默认状态
	public void InItSendBtns ()
	{
		sendBtnText1.SetActive (true);
		sendBtnText2.SetActive (false);
		sendBtnText3.SetActive (false);
	}

	//返回按钮
	public void BackToMainPage ()
	{
		EmailData.Instance.replyName = nameLabel.text;
		EmailData.Instance.content = contentLabel.text;

		if (!isReply)
		{
			EmailManager.emailMan.SystemBtn ();
		}
		else
		{
			isReply = false;
		}

		if (string.IsNullOrEmpty (nameLabel.text) && string.IsNullOrEmpty (contentLabel.text))
		{
			EmailManager.emailMan.CheckDraft (false);
		}
		else
		{
			EmailManager.emailMan.CheckDraft (true);
		}

		this.gameObject.SetActive (false);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

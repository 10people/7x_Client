using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChatEmailSend : MonoBehaviour,SocketProcessor {

	public UILabel nameLabel;
	
	public GameObject inputContentObj;
	public UILabel contentLabel;
//	private BoxCollider contentBox;
//	private UIInput contentInput;
	
	public GameObject sendBtnText1;//发送前
	public GameObject sendBtnText2;//发送中
	public GameObject sendBtnText3;//发送完成
	
	private int resultType;//发送结果类型
	
	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}
	
//	void Start ()
//	{	
//		contentBox = contentLabel.gameObject.GetComponent<BoxCollider> ();
//		
//		contentInput = contentLabel.gameObject.GetComponent<UIInput> ();
//	}
	
	//获得回复人名字
	public void GetNameStr (string nameStr)
	{
		nameLabel.text = nameStr;
		contentLabel.text = "";

		sendBtnText1.SetActive (true);
		sendBtnText2.SetActive (false);
		sendBtnText3.SetActive (false);
	}
	
	//发送邮件请求
	public void SendBtn ()
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
		
		string str = "邮件发送成功！";
		
		string confirmStr = "确定";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,
		             SendSuccess);
	}
	void SendSuccess (int i)
	{
		DeActiveWindow ();
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
			
			str = "邮差不知道要寄给谁...\n赶快添上收件人名字吧~";
			
			break;
			
		case 2:
			
			str = "邮差不管送空邮件哦...\n赶快写点什么吧~";
			
			break;
			
		case 3:
			
			str = "很遗憾，找不到这个玩家...";
			
			break;
			
		case 4:
			
			str = "有奇怪的文字混进来了...\n再推敲一下吧！";
			
			break;
			
		case 5:
			
			str = "您已被该玩家屏蔽！";
			
			break;
			
		case 6:
			
			str = "无法向自己发送邮件！";
			
			break;
			
		case 7:
			
			str = "间隔时间不到1分钟，请稍后再发！";
			
			break;
			
		case 8:
			
			str = "收件人在黑名单中！无法发送邮件！";
			
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

	public void DeActiveWindow ()
	{
		gameObject.SetActive(false);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

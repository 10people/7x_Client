using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CheckEmail : MonoBehaviour, SocketProcessor
{
    private EmailInfo m_emailInfo;
    private Dictionary<long, BlackJunzhuInfo> blackListDic = new Dictionary<long, BlackJunzhuInfo>();

    private GetRewardResponse getResp;//getRewardRes
    private EmailResponseResult responseResp;//emailResponse

    public UILabel systemTaiTouLabel;
    public UILabel systemContentLabel;
    public UILabel systemSenderLabel;

    public UILabel privateContentLabel;
    public UILabel privateSenderLabel;

    public GameObject systemEmailObj;
    public GameObject privateLetterObj;

    public GameObject awardListObj;//awardObj
    public GameObject awardList_NotHaveObj;//no_award

    public GameObject systemBtnObj;
    public GameObject getAwardBtnObj;
    public GameObject homeBtnsObj;
    public GameObject replyLetterBtnsObj;
    public GameObject reply_shieldBtn;

	public GameObject addFriendBtnObj;

    public GameObject writeObj;//writeobj

    private int homeColType;//home 0-agree 1-refuse

    private int emailType;//emailType

	private int emailColType;//邮件操作类型

    private string cancelStr;
    private string confirmStr;
    private string titleStr;
	private string str;

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
    }

    void Update()
    {
        if (emailType == 80000)
        {
            blackListDic = BlockedData.Instance().m_BlockedInfoDic;

            if (blackListDic.Count > 0)
            {
                foreach (long blackJunZhuId in blackListDic.Keys)
                {
                    //					Debug.Log ("blackJunZhuId:" + blackJunZhuId);
                    if (m_emailInfo.senderName == blackListDic[blackJunZhuId].name)
                    {
                        reply_shieldBtn.SetActive(false);
                    }
                    else
                    {
                        reply_shieldBtn.SetActive(true);
                    }
                }
            }

            else
            {
                reply_shieldBtn.SetActive(true);
            }
        }
    }

    public void GetEmailInfo(EmailInfo emailInfo)
    {
        m_emailInfo = emailInfo;

        emailType = emailInfo.type;

        EmailTemp emailTemp = EmailTemp.getEmailTempByType(emailInfo.type);
        emailColType = emailTemp.operateType;
//		Debug.Log ("type:" + emailInfo.type + "||" + emailColType);
        if (emailColType == 0)
        {
            if (emailInfo.type == 80000)//privateLetter
            {
                privateLetterObj.SetActive(true);

                replyLetterBtnsObj.SetActive(true);

				privateContentLabel.text = "       " + MyColorData.getColorString (3,emailInfo.content);
				privateSenderLabel.text = MyColorData.getColorString (3,emailInfo.senderName);

                if (privateContentLabel.height >= 370)
                {
                    privateSenderLabel.transform.localPosition = new Vector3(795, -360 - (privateContentLabel.height - 370), 0);
                }

				//判断是否是好友，显示加好友按钮
				var friendList = FriendOperationData.Instance.m_FriendListInfo.friends;
				List<long> junZhuIdList = new List<long>();
				for (int i = 0;i < friendList.Count;i ++)
				{
					junZhuIdList.Add (friendList[i].ownerid);
				}

				if (!junZhuIdList.Contains (m_emailInfo.jzId))
				{
					addFriendBtnObj.SetActive (true);
				}
            }

            else//systemEmail
            {
                systemEmailObj.SetActive(true);

                awardList_NotHaveObj.SetActive(true);

                systemBtnObj.SetActive(true);

                systemTaiTouLabel.text = MyColorData.getColorString (3,emailInfo.taiTou);
				systemContentLabel.text = "     " + MyColorData.getColorString (3, emailInfo.content);
				systemSenderLabel.text = MyColorData.getColorString (3,emailInfo.senderName);

                if (systemContentLabel.height >= 246)
                {
					systemSenderLabel.transform.localPosition = new Vector3(795, -42 - systemContentLabel.height, 0);
                }
            }
        }

        else
        {
            systemEmailObj.SetActive(true);

			systemTaiTouLabel.text = MyColorData.getColorString (3,emailInfo.taiTou);
			systemContentLabel.text = "     " + MyColorData.getColorString (3,emailInfo.content);
			systemSenderLabel.text = MyColorData.getColorString (3,emailInfo.senderName);

            if (emailColType == 1)//delete after look
            {
                awardList_NotHaveObj.SetActive(true);

                systemBtnObj.SetActive(true);
            }

            else if (emailColType == 2)//delete after get
            {
                awardListObj.SetActive(true);

                getAwardBtnObj.SetActive(true);

                EmailAwardList awardList = awardListObj.GetComponent<EmailAwardList>();
                awardList.GetEmailInfo(emailInfo);
            }

            else if (emailColType == 3)//delete after operate
            {
                awardList_NotHaveObj.SetActive(true);

                homeBtnsObj.SetActive(true);
            }

            if (systemContentLabel.height >= 246)
            {
				systemSenderLabel.transform.localPosition = new Vector3(795, -42 - systemContentLabel.height, 0);
            }
        }
    }

    //ConfirmBtn
    public void SystemSureBtn()
    {
        DestroyThisGameObj();
    }

    //GetRewardBtn
    public void GetAwardBtn()
    {
        if (!TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
		{
			GetAward();
            TimeHelper.Instance.AddOneDelegateToTimeCalc("CheckEmail", 10);
		}
		else
		{
            if (TimeHelper.Instance.IsCalcTimeOver("CheckEmail"))
			{
				GetAward();
			}
		}
    }

    //Home_refuseBtn
    public void ConfuseBtn()
    {
        homeColType = 1;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                HomeBoxLoadBack);
    }

    //Home_agreeBtn
    public void AgreeBtn()
    {
        homeColType = 0;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                HomeBoxLoadBack);
    }

    //HomeBoxloadback
    void HomeBoxLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        //Home_operate 0-agree 1-refuse
        if (homeColType == 0)
        {
            titleStr = "同意交换";
            str = "\n\n您是否要同意这次房屋交换申请？";

			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null, null, cancelStr, confirmStr,
                         HomeColAsk);
        }

        else if (homeColType == 1)
        {
            titleStr = "拒绝交换";
            str = "\n\n您是否要拒绝这次房屋交换申请？";

			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null, null, cancelStr, confirmStr,
                         HomeColAsk);
        }
    }

    void HomeColAsk(int i)
    {
        if (homeColType == 0)
        {
            if (i == 2)
            {
                if (!TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
				{
					EmailResponseReq(1);
                    TimeHelper.Instance.AddOneDelegateToTimeCalc("CheckEmail", 10);
				}
				else
				{
                    if (TimeHelper.Instance.IsCalcTimeOver("CheckEmail"))
					{
						EmailResponseReq(1);
					}
				}
            }
        }

        else if (homeColType == 1)
        {
            if (i == 2)
            {
                if (!TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
				{
					EmailResponseReq(2);
                    TimeHelper.Instance.AddOneDelegateToTimeCalc("CheckEmail", 10);
				}
				else
				{
                    if (TimeHelper.Instance.IsCalcTimeOver("CheckEmail"))
					{
						EmailResponseReq(2);
					}
				}
            }
        }
    }

    //privateLetter_shieldBtn
    public void ShieldBtn()
    {
//        Debug.Log("从聊天中调用");
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                ShieldLoadBack);
    }

    //ShiledBoxloadback
    void ShieldLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        titleStr = "屏蔽玩家";

		str = "\n\n您是否要将" + m_emailInfo.senderName + "加入黑名单？您将不能和黑名单中的玩家进行交流，也无法看到对方在聊天中的信息。您可以在【好友】-【屏蔽名单】内解除黑名单设置。";


		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null, null, cancelStr, confirmStr,
                     ShieldCol);
    }

    void ShieldCol(int i)
    {
        if (i == 2)
        {
//            Debug.Log("发送屏蔽请求！");
            if (!TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
			{
				EmailResponseReq(5);
                TimeHelper.Instance.AddOneDelegateToTimeCalc("CheckEmail", 10);
			}
			else
			{
                if (TimeHelper.Instance.IsCalcTimeOver("CheckEmail"))
				{
					EmailResponseReq(5);
				}
			}
        }
    }

    //PrivateLetter_replyBtn
    public void ReplyBtn()
    {
        writeObj.SetActive(true);

        EmailSend send = writeObj.GetComponent<EmailSend>();

		if (m_emailInfo.senderName != EmailData.Instance.replyName)
		{
			send.CleanInputLabel(3);
		}
        
        send.GetNameStr(m_emailInfo.senderName);
    }

    //领奖请求
    void GetAward()
    {
        GetRewardRequest getAwardReq = new GetRewardRequest();

        getAwardReq.id = m_emailInfo.id;

        MemoryStream t_stream = new MemoryStream();

        QiXiongSerializer t_serializer = new QiXiongSerializer();

        t_serializer.Serialize(t_stream, getAwardReq);

        byte[] t_protof = t_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MAIL_GET_REWARD, ref t_protof, "25008");
//        Debug.Log("GetAwardEmail:" + ProtoIndexes.C_MAIL_GET_REWARD);
    }

	//邮件响应操作 操作码：1-同意，2-拒绝，5-屏蔽玩家
	void EmailResponseReq(int code)
    {
        EmailResponse responseReq = new EmailResponse();

        responseReq.emailId = m_emailInfo.id;
        responseReq.operCode = code;

        MemoryStream t_stream = new MemoryStream();

        QiXiongSerializer t_serializer = new QiXiongSerializer();

        t_serializer.Serialize(t_stream, responseReq);

        byte[] t_protof = t_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EMAIL_RESPONSE, ref t_protof);
//        Debug.Log("EmailResponseReq:" + ProtoIndexes.C_EMAIL_RESPONSE);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
	        case ProtoIndexes.S_MAIL_GET_REWARD://领奖返回
	        {
//	            Debug.Log("GetRewardRes：" + ProtoIndexes.S_MAIL_GET_REWARD);

	            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

	            QiXiongSerializer t_qx = new QiXiongSerializer();

	            GetRewardResponse getAwardResp = new GetRewardResponse();

	            t_qx.Deserialize(t_stream, getAwardResp, getAwardResp.GetType());

	            if (getAwardResp != null)
	            {
	            	getResp = getAwardResp;
//	            	Debug.Log("Get_isSuccess：" + getAwardResp.isSuccess);

//                    if (getAwardResp.id != null)
//                    {
//                        Debug.Log("GetEmailId：" + getAwardResp.id);
//                    }

					EmailManager.emailMan.DeletSystemEmail (getAwardResp.id);

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                            GetAwardBack);

                    if (TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
					{
                        TimeHelper.Instance.RemoveFromTimeCalc("CheckEmail");
					}
                }

	            return true;
            }

	        case ProtoIndexes.S_EMAIL_RESPONSE://邮件响应操作返回
            {
        	    Debug.Log("EmailResponse：" + ProtoIndexes.S_EMAIL_RESPONSE);

                MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                QiXiongSerializer t_qx = new QiXiongSerializer();

                EmailResponseResult tempResp = new EmailResponseResult();

                t_qx.Deserialize(t_stream, tempResp, tempResp.GetType());

                if (tempResp != null)
                {
                    responseResp = tempResp;

                    if (tempResp.isSuccess == 0)
                    {
                        if (emailType == 80000)
                        {
//                                    Debug.Log("Add in blackList!");
                            BlockedData.Instance().RequestBlockedInfo();
                        }

                        else
                        {
							EmailManager.emailMan.DeletSystemEmail (tempResp.emailId);
						}
					}

                    else if (tempResp.isSuccess == 1)
                    {
						if (emailType == 80000)
						{

						}

						else
						{
							EmailManager.emailMan.DeletSystemEmail (tempResp.emailId);
						}
                    }

                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                            ResponseBack);

                    if (TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
					{
                        TimeHelper.Instance.RemoveFromTimeCalc("CheckEmail");
					}
                }

                return true;
            }

            default: return false;
            }
        }

        return false;
    }

    //GetRewardSuccessBoxloadback
    void GetAwardBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        if (getResp.isSuccess == 0)
        {
            titleStr = "领取成功";

            str = "\n\n恭喜领取奖励成功！";
        }

        else if (getResp.isSuccess == 1)
        {
            titleStr = "领取失败";

            str = "\n\n没有该邮件！";
        }

        else if (getResp.isSuccess == 2)
        {
            titleStr = "领取失败";

            str = "\n\n您已经领取过奖励！";
        }

        uibox.setBox(titleStr,MyColorData.getColorString (1,str),null, null, confirmStr, null,
                     GetAwardFinished);
    }

    void GetAwardFinished(int i)
    {
		if (getResp.isSuccess == 0)
		{
			List<RewardData> dataList = new List<RewardData>();
			for (int m = 0;m < m_emailInfo.goodsList.Count;m ++)
			{
				RewardData data = new RewardData(m_emailInfo.goodsList[m].id,m_emailInfo.goodsList[m].count);
				dataList.Add (data);
			}

			GeneralRewardManager.Instance ().CreateReward (dataList);
		}
        DestroyThisGameObj();
    }

    //EmailResSuccessBoxloadback
    void ResponseBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        if (responseResp.isSuccess == 0)
        {
//            Debug.Log("responseResp success");

            if (emailType == 80000)
            {
                titleStr = "屏蔽玩家";
				str = "\n您已将" + m_emailInfo.senderName + "加入黑名单！\n您可以在【好友】-【屏蔽名单】功能中解除黑名单设置";

                uibox.setBox(titleStr,MyColorData.getColorString (1,str),null, null, confirmStr, null,
                             null);
            }

            else
            {
                if (homeColType == 0)
                {
                    titleStr = "同意交换";
                    str = "\n\n您已同意这次房屋交换申请！";
                }

                else if (homeColType == 1)
                {
                    titleStr = "拒绝交换";
                    str = "\n\n您已拒绝这次房屋交换申请！";
                }

                uibox.setBox(titleStr,MyColorData.getColorString (1,str),null, null, confirmStr, null,
                             ResponseFinishBack);
            }
        }

        else if (responseResp.isSuccess == 1)
        {
//            Debug.Log("responseResp fail");

            if (emailType == 80000)
            {
                titleStr = "屏蔽失败";
            }

            else
            {
                if (homeColType == 0)
                {
                    titleStr = "交换失败";
                }

                else if (homeColType == 1)
                {
                    titleStr = "拒绝失败";
                }
            }
            str = "\n\n没有该邮件！";

			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,
                         ResponseFinishBack);
        }
    }

    void ResponseFinishBack(int i)
    {
        DestroyThisGameObj();
    }

	//添加好友按钮
	public void AddFriendBtn ()
	{
		FriendOperationData.Instance.SetParentObjName = this.name;
		FriendOperationData.Instance.AddFriends (FriendOperationData.AddFriendType.Email,m_emailInfo.jzId,m_emailInfo.senderName);
	}
	
	public void RefreshAddFriendBtn ()
	{
		//刷新邮件好友状态
		addFriendBtnObj.SetActive (false);
	}

    public void DestroyThisGameObj()
    {
		EmailManager.emailMan.IsCheck = false;

        if (TimeHelper.Instance.IsTimeCalcKeyExist("CheckEmail"))
		{
            TimeHelper.Instance.RemoveFromTimeCalc("CheckEmail");
		}
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class FriendOperationData :Singleton<FriendOperationData>, SocketProcessor
{
    public  GetFriendListResp m_FriendListInfo = new GetFriendListResp();
	public List<long> friendIdList = new List<long> ();
    public bool m_FriendInfoGet = false;
    public bool m_FriendInfoRequest = false; // only FriendOperationLayer Can Use
    private string _FriendName = "";
    public bool m_GreetSucess = false;

	//添加好友入口类型
	public enum AddFriendType
	{
		LiaoTian,//聊天
		BaiZhan,//百战
		Email,//邮件
	}
	private AddFriendType addType = AddFriendType.LiaoTian;

	private string parentGameObjName;
	public string SetParentObjName
	{
		set{parentGameObjName = value;}
	}

	#region Mono

    void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}

	void Start ()
    {
 
    }

	void OnDestroy(){
		base.OnDestroy();
	}

	#endregion

    public void RequestData()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetFriendListReq friend = new GetFriendListReq();
        friend.pageNo = 0;
 
        friend.pageSize = 0;
        t_qx.Serialize(t_tream, friend);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FRIEND_REQ, ref t_protof);

    }

	//添加好友
    public void AddFriends(AddFriendType tempType,long FriendId,string friendname)
    {
		addType = tempType;
        _FriendName = friendname;
        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_serializer = new QiXiongSerializer();
        CancelBlack temp = new CancelBlack();
        temp.junzhuId = FriendId;
        t_serializer.Serialize(tempStream, temp);
        byte[] t_protof = tempStream.ToArray();
        t_protof = tempStream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ADDFRIEND_REQ, ref t_protof);
		//Debug.Log ("AddType:" + addType);
		//Debug.Log ("FriendId:" + FriendId);
		//Debug.Log ("friendname:" + friendname);
		//Debug.Log ("AddFriendReq:" + ProtoIndexes.C_ADDFRIEND_REQ);
    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null) {
			
			switch (p_message.m_protocol_index)
			{
                case ProtoIndexes.S_FRIEND_RESP://返回好友信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetFriendListResp ReponseInfo = new GetFriendListResp();

                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                       	
						if (ReponseInfo.friends == null)
						{
							ReponseInfo.friends = new List<FriendJunzhuInfo>();
						}
                        m_FriendListInfo = ReponseInfo;

						friendIdList.Clear ();
						foreach (FriendJunzhuInfo friend in ReponseInfo.friends)
						{
							friendIdList.Add (friend.ownerid);
						}

                        if (m_FriendInfoRequest)
                        {
                            m_FriendInfoRequest = false;
                            m_FriendInfoGet = true;
                        }

                        return true;
                    }
                    break;
                case ProtoIndexes.S_ADDRIEND_RESP://返回添加好友信息
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FriendResp tempResponse = new FriendResp();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
						//Debug.Log ("tempResponse.result:" + tempResponse.result);
                        if (tempResponse.result == 0)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallbackAddFriendSuccess);
                            RequestData();
                            switch (addType)
							{
							case AddFriendType.LiaoTian:
							{
								break;
							}
							case AddFriendType.BaiZhan:
							{
//								BaiZhanPage.baiZhanPage.RefreshOpponentFriendState ();
								break;
							}
							case AddFriendType.Email:
							{
								EmailPage.emailPage.RefreshEmailCheck (EmailPage.RefreshType.ADD_FRIEND);
								break;
							}
							default:
								break;
							}

                            RequestData();
                        }
                        else if (tempResponse.result == 103)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallbackFriendFull);
                        }
                        return true;
                    }
                    break;

                case ProtoIndexes.S_GREET_RESP:// 向某人打招呼返回
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GreetResp greetresp = new GreetResp();

                        t_qx.Deserialize(t_stream, greetresp, greetresp.GetType());
                        m_GreetSucess = true;
                        switch (greetresp.resCode)
                        {
                            case 1:
                                {

                                    // ClientMain.m_UITextManager.createText("打招呼成功！");
                                    RequestData();
                                }
                                break;
                            case 2:
                                {
                                    ClientMain.m_UITextManager.createText("该玩家现在忙，1分钟内无法打招呼！");
                                }
                                break;
                            case 10:
                                {
                                    ClientMain.m_UITextManager.createText("该玩家此前已级被你屏蔽，请解除屏蔽再来加好友吧！");
                                }
                                break;
                            case 20:
                                {
                                    ClientMain.m_UITextManager.createText("自己好友达到上限！");
                                }
                                break;
                            case 30:
                                {
                                    ClientMain.m_UITextManager.createText("该玩家已经是你的好友，你刚刚和他打了个招呼。");
                                }
                                break;
                        }
                        return true;
                    }
            }
            
        }
		return false;
	}

    public void UIBoxLoadCallbackAddFriendSuccess(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_14) + _FriendName + LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_15);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);

    }
    public void UIBoxLoadCallbackFriendFull(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_0);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_1);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null);
    }
}

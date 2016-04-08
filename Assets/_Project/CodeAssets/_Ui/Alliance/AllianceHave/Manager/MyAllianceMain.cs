using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MyAllianceMain : MonoBehaviour, SocketProcessor
{

    public ScaleEffectController m_ScaleEffectController;

	/** required string name = 1;
	required int32 id = 2;
	required int32 level = 3;
	required int32 exp = 4;//当前经验
	required int32 needExp = 5;//升级所需经验
	required int32 build = 6;//建设
	required int32 members = 7;//成员数量
	required int32 memberMax = 8;//最大成员数量
	required int32 contribution = 9;//贡献
	required string notice = 10;//公告
	required int32 icon = 11;
	required int32 identity = 12;//身份0-成员，1-副盟主，2-盟主
	optional int32 isAllow = 13;//是否开启招募 0-关闭，1-开启 ,只有是盟主时才发
	 * *///联盟信息

	public static MyAllianceMain m_allianceMain;
	public AllianceHaveResp g_UnionInfo;

	private AllianceHaveResp m_Union;//本地存储的临时联盟信息

	private UpdateNoticeResp m_noticeResp;//公告修改返回信息
	private ExitAllianceResp m_exitResp;//退出联盟返回
	
	public UISprite allianceIcon;//联盟图标
	public UILabel allianceName;//联盟名字
	public UILabel allianceId;//联盟id
	public UILabel allianceLevel;//联盟等级

	public UILabel exp;//经验
	public UILabel build;//建设
	public UILabel members;//成员
	public UILabel leaderName;//盟主名字
	public UILabel contribution;//贡献
	public UILabel notice;//公告

	private int identity;//成员身份

	public BoxCollider noticeCollider;//公告碰撞框
	private string noticeInfo;//公告内容

	public GameObject editNoticeObj;//公告编辑obj
	public UIInput editBox;//编辑框
	public UILabel editInfo;//编辑的公告内容
	private int charNum;//编辑字符个数
	public UILabel limitNum;//字符限制

	public GameObject confirmEditBtn;//确认编辑按钮

	public GameObject leaderBtn;//盟主操作按钮
	public GameObject applyBtn;//入盟申请按钮
	public GameObject exitAllianceBtn;//退出联盟按钮
	public GameObject lookMembersBtn;//查看成员按钮
	public GameObject donationBtn;//捐献按钮

	public GameObject donationBoxObj;//捐献选择窗口

	private string confirmStr;
	private string cancelStr;

	void Awake()
	{ 
		m_allianceMain = this;
		SocketTool.RegisterMessageProcessor(this);
	}

	void Start () 
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		AllianceData.Instance.RequestData ();
//		g_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
//
//		if(g_UnionInfo != null)
//		{
//			InItAllianceUI();
//		}
	}

	//初始化联盟首页的UI
	void InItAllianceUI()
	{
		identity = g_UnionInfo.identity;//身份

		if(identity == 0)
		{
			noticeCollider.enabled = false;
		}
		Debug.Log ("IconId:" + g_UnionInfo.icon);
		allianceIcon.spriteName = g_UnionInfo.icon.ToString ();//icon 可能读表  暂未设置
		allianceName.text = g_UnionInfo.name;
		allianceId.text = g_UnionInfo.id.ToString ();
		allianceLevel.text = g_UnionInfo.level.ToString ();

		exp.text = g_UnionInfo.exp.ToString()+"/"+g_UnionInfo.needExp.ToString();
		build.text = g_UnionInfo.build.ToString ();
		members.text = g_UnionInfo.members.ToString () + "/" + g_UnionInfo.memberMax.ToString ();
		leaderName.text = g_UnionInfo.mengzhuName;
		contribution.text = g_UnionInfo.contribution.ToString ();

		if (g_UnionInfo.notice == "" || g_UnionInfo.notice == null)
		{

		}
		else
		{
			notice.text = g_UnionInfo.notice;
		}

		donationBtn.SetActive (true);
		switch(identity)
		{
		case 0://普通成员
			
			leaderBtn.SetActive(false);
			applyBtn.SetActive(false);
			exitAllianceBtn.SetActive(true);
			lookMembersBtn.SetActive(true);

			exitAllianceBtn.transform.localPosition = new Vector3 (-260,0,0);
			donationBtn.transform.localPosition = Vector3.zero;
			lookMembersBtn.transform.localPosition = new Vector3 (260,0,0);

			break;

		case 1://副盟主
			
			leaderBtn.SetActive(false);
			applyBtn.SetActive(true);
			exitAllianceBtn.SetActive(true);
			lookMembersBtn.SetActive(true);

			exitAllianceBtn.transform.localPosition = new Vector3 (-330,0,0);
			donationBtn.transform.localPosition = new Vector3(-110,0,0);
			applyBtn.transform.localPosition = new Vector3 (110,0,0);
			lookMembersBtn.transform.localPosition = new Vector3 (330,0,0);

			break;

		case 2://盟主
			
			leaderBtn.SetActive(true);
			applyBtn.SetActive(false);
			exitAllianceBtn.SetActive(false);
			lookMembersBtn.SetActive(true);

			leaderBtn.transform.localPosition = new Vector3 (-260,0,0);
			donationBtn.transform.localPosition = Vector3.zero;
			lookMembersBtn.transform.localPosition = new Vector3 (260,0,0);

			break;

		default:break;
		}
	}

	void Update () 
	{
		if(editNoticeObj.activeSelf)
		{
			if(editInfo.text.Length > 0)
			{
				confirmEditBtn.SetActive(true);
			}
			else
			{
				confirmEditBtn.SetActive(false);
			}

			charNum = editBox.characterLimit - editInfo.text.Length;

			if(charNum >= 0)
			{
				limitNum.text = charNum.ToString();
			}
			else
			{
				limitNum.text = "200";
			}
		}
	}

	public void InItMyAlliance ()
	{
		if(g_UnionInfo != null)
		{
			if(m_Union == null)
			{
				m_Union = g_UnionInfo;
			}

			InItAllianceUI ();

			Debug.Log ("g_UnionInfo.exp:" + g_UnionInfo.exp);
			Debug.Log ("g_UnionInfo.needExp:" + g_UnionInfo.needExp);

			if (g_UnionInfo.exp >= g_UnionInfo.needExp)//升级
			{
				Debug.Log ("联盟可升级");


			}
			if (m_Union.level < g_UnionInfo.level)
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        UpLevelLoadCallback );

				m_Union = g_UnionInfo;
			}
		}
	}

	//联盟升级提示框异步加载回调
	public void UpLevelLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string upLevelStr1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL_TIPSTR1);
		string upLevelStr2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL_TIPSTR2);
		string upLevel = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL);

		string upLevelDes = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL_DES);

		string upLevelTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL_TITLE);

		string str1 = upLevelStr1 + g_UnionInfo.name + upLevelStr2 + (g_UnionInfo.level).ToString () + upLevel;
		string str2 = upLevelDes + (g_UnionInfo.memberMax).ToString ();

		uibox.setBox(upLevelTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,null);
	}

	//打开编辑公告框
	public void OpenEditBox ()
	{
		editNoticeObj.SetActive (true);
	}

	//发送编辑公告请求
	public void EditNoticeReq ()
	{
		UpdateNotice noticeReq = new UpdateNotice ();

		noticeReq.id = g_UnionInfo.id;
		noticeReq.notice = editInfo.text;
		noticeInfo = editInfo.text;

		MemoryStream noticeStream = new MemoryStream ();
		
		QiXiongSerializer noticeQx = new QiXiongSerializer ();

		noticeQx.Serialize (noticeStream,noticeReq);
		
		byte[] t_protof = noticeStream.ToArray();;
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.UPDATE_NOTICE,ref t_protof,"30130");

		editNoticeObj.SetActive (false);
	}

	//关闭公告编辑
	public void CloseEditBox()
	{
		editNoticeObj.SetActive (false);
	}

	//退出联盟
	public void EixtAlliance()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ExitLoadCallback );
	}

	//退出联盟提示框异步加载回调
	public void ExitLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR2);

		string exitTitle = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_TITLE);

		uibox.setBox(exitTitle, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),
		             null,cancelStr,confirmStr,ExitAllianceReq);
	}

	//发送退出联盟的请求
	void ExitAllianceReq (int i)
	{
		if (i == 2)
		{
			ExitAlliance exitReq = new ExitAlliance ();

			exitReq.id = g_UnionInfo.id;

			MemoryStream exitStream = new MemoryStream ();
			
			QiXiongSerializer exitQx = new QiXiongSerializer ();

			exitQx.Serialize (exitStream, exitReq);
			
			byte[] t_protof = exitStream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.EXIT_ALLIANCE, ref t_protof, "30114");
		}
	}

	//接收联盟返回的数据
	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.UPDATE_NOTICE_RESP://修改公告返回
			{
				MemoryStream noticeResp_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer noticeResp_qx = new QiXiongSerializer();

				UpdateNoticeResp noticeResp = new UpdateNoticeResp();
				
				noticeResp_qx.Deserialize (noticeResp_stream, noticeResp, noticeResp.GetType());
			
				if (noticeResp != null)
				{
					m_noticeResp = noticeResp;

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        NoticeLoadCallback );
				}

				return true;
			}

			case ProtoIndexes.EXIT_ALLIANCE_RESP://退出联盟返回
			{
				MemoryStream exit_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer exit_qx = new QiXiongSerializer();

				ExitAllianceResp exitResp = new ExitAllianceResp();
				
				exit_qx.Deserialize(exit_stream, exitResp, exitResp.GetType());

				if(exitResp != null)
				{
					m_exitResp = exitResp;

					if (exitResp.code == 0)
					{
						CityGlobalData.m_isMainScene = true;
					}

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ExitAllianceLoadCallback );
				}
				
				return true;
			}

			default: return false;
			}
		}
		
		return false;
	}

	//联盟公告提示框异步加载回调
	public void NoticeLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string changeTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_GONGGAO_CHANGE_TITLE);

		if (m_noticeResp.code == 0)
		{
			//弹出修改成功的UI提示， 并更新UIlable的显示内容
			Debug.Log("修改成功");
			
			notice.text = noticeInfo;

			string changeSuccessStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_GONGGAO_CHANGE_SUCCESS);

			string str  = changeSuccessStr;
			uibox.setBox(changeTitleStr,null, MyColorData.getColorString (1,str), null,confirmStr,null,null);
		}
		
		else
		{
			Debug.Log("修改失败，，，，，，，，，，");
			
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_GONGGAO_STR_TOOMUCH);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_GONGGAO_CHANGE_FAIL);
			uibox.setBox(changeTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),
			             null,confirmStr,null,null);
		}
	}

	//退出联盟返回提示框异步加载回调
	public void ExitAllianceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string exitTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);

		if(m_exitResp.code == 0)
		{
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);
			string str2 = g_UnionInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES2);

			uibox.setBox(exitTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,DeletUI_i);
		}
		else
		{
			Debug.Log("退出失败");

			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL_REASON);
			uibox.setBox(exitTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,null);
		}
	}

	public void DeletUI_i(int i)//Quite。
	{
        AllianceData.Instance.IsAllianceNotExist = true;
        QXChatUIBox.chatUIBox.SetSituationState();
        m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindowWithEnterMainCity;
        m_ScaleEffectController.OnCloseWindowClick();
	}

	public void DeletUI()//Quite。
	{
        m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
        Destroy(this.gameObject);
    }

    void DoCloseWindowWithEnterMainCity()
    {
        JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
        AllianceData.Instance.RequestData();
        CityGlobalData.m_isAllianceScene = false;
        CityGlobalData.m_isMainScene = true;
        SceneManager.EnterMainCity();
        Destroy(this.gameObject);
    }

	//打开联盟捐献选择窗口
	public void OpenDonationBtn ()
	{
		GameObject donationObj = (GameObject)Instantiate (donationBoxObj);

		donationObj.SetActive (true);
		donationObj.transform.parent = donationBoxObj.transform.parent;
		donationObj.transform.localPosition = donationBoxObj.transform.localPosition;
		donationObj.transform.localScale = donationBoxObj.transform.localScale;

		AllianceDonation donation = donationObj.GetComponent<AllianceDonation> ();
		donation.tempAllianceResp = m_Union;
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}

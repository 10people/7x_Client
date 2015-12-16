using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class MyAllianceInfo : MonoBehaviour,SocketProcessor {

	public AllianceHaveResp m_Alliance;

	public UILabel Alliance_Name;
	
	public UILabel Level;
	
	public UILabel Country;
	
	public UILabel Builds;
	
	public UILabel leader;
	
	public UILabel Exp;
	
	public UILabel Shengwang;
	
	public UILabel Menbers;
	
	public UILabel MyZhiwu;
	
	public UILabel MyGongxianzhi;
	
	public UILabel MyGongJin;
	
	//public UILabel My_notice;//编辑的公告内容
	
	//public GameObject MenberItem;
	
	public UISprite all_Icon;

	public UILabel editInfo;//编辑的公告内容

	public UILabel ShoweditInfo;//编辑的公告内容
	
	private string noticeInfo;//公告内容
	
	//public GameObject editNoticeObj;//公告编辑obj
	
	public UILabel OnLineMenbers;

	public GameObject JuanXianObj;//公告编辑obj

	public int HufuNum = 1;

	public int Maxhufunum;

	public GameObject noticeBtn;//公告编辑按钮

	UpdateNoticeResp m_noticeResp ;

	public List<_AllMember> m_AllMember = new List<_AllMember>();
	public static MyAllianceInfo m_MyAllianceInfo;
	void Awake()
	{
		m_MyAllianceInfo = this;
		SocketTool.RegisterMessageProcessor(this);
		//Debug.Log ( "MiBaoManager.Awake()" );
	}
	void OnDestroy()
	{
	
		SocketTool.UnRegisterMessageProcessor(this);
		//Debug.Log ( "MiBaoManager.OnDestroy()" );
	}

	void Start () {
	
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}
	public void Init()
	{
		InitUI ();
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index){
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
				EditUI.SetActive (false);
				return true;
			}
			case ProtoIndexes.ALLIANCE_HUFU_DONATE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				DonateHuFuResp donateResp = new DonateHuFuResp();
				
				t_qx.Deserialize(t_stream, donateResp, donateResp.GetType());
				
				if (donateResp != null)
				{
					Debug.Log ("捐献结果：" + donateResp.result);
					Debug.Log ("捐献贡献：" + donateResp.gongxian);
					Debug.Log ("捐献建设：" + donateResp.build);
					mdonateResp = donateResp;
				    if (donateResp.result == 1)
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        ResourceLoadCallback2 );
					}
					else
					{
						AllianceData.Instance.RequestData ();
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        ResourceLoadCallback1 );
					}
				}
				JuanXianObj.SetActive (false);
				return true;	
			}
			default: return false;
			}
			
		}
		
		return false;
	}

	DonateHuFuResp mdonateResp;
	public void ResourceLoadCallback1 ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "捐献成功";
		
		string str = "成功捐献"+HufuNum.ToString()+"个虎符，获得"+mdonateResp.gongxian.ToString()+"贡献值" +"\r\n"+"联盟获得"+mdonateResp.build.ToString()+"建设值";
		
		string confirmStr = "确定";
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str), 
		             null,confirmStr,null,null);
	}
	public void ResourceLoadCallback2 ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "捐献失败";
		
		string str = "您的虎符不足！";
		
		string confirmStr = "确定";
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str), 
		             null,confirmStr,null,null);
	}
	void Update () {
	
	}
	public void InitUI()
	{
		Alliance_Name.text = m_Alliance.name+"("+m_Alliance.id.ToString()+")";
		
		Level.text = m_Alliance.level.ToString();

		switch(m_Alliance.country)
		{
		case 1:
			Country.text = "齐";
			break;
		case 2:
			Country.text = "楚";
			break;
		case 3:
			Country.text = "燕";
			break;
		case 4:
			Country.text = "韩";
			break;
		case 5:
			Country.text = "赵";
			break;
		case 6:
			Country.text = "魏";
			break;
		case 7:
			Country.text = "秦";
			break;
		default:
			break;

		}

		Builds.text = m_Alliance.build.ToString ();
		
		leader.text = m_Alliance.mengzhuName;
		
		Exp.text = m_Alliance.exp.ToString ()+"/"+m_Alliance.needExp.ToString();
		
		Shengwang.text = m_Alliance.shengWang.ToString();
		
		Menbers.text = m_Alliance.members.ToString () + "/" + m_Alliance.memberMax.ToString ();
		
		if(m_Alliance.identity == 0)
		{
			MyZhiwu.text = "盟员";
		}
		if(m_Alliance.identity == 1)
		{
			MyZhiwu.text = "副盟主";
		}
		if(m_Alliance.identity == 2)
		{
			MyZhiwu.text = "盟主";
		}
		
		all_Icon.spriteName = m_Alliance.icon.ToString ();
	
		MyGongJin.text = m_Alliance.gongJin.ToString();

		ShowMyGongXianZhi (m_Alliance.contribution);

		ShoweditInfo.text = m_Alliance.notice;
		
		InitMenbers ();

		if(m_Alliance.identity == 1|| m_Alliance.identity == 2)
		{
			noticeBtn.SetActive(true);
		}
		else
		{
			noticeBtn.SetActive(false);
		}
	}

	//显示钱币
	public void ShowMyGongXianZhi(int money)
	{
		m_Alliance.contribution = money;
		MyGongxianzhi.text = money.ToString ();
	}

	private float Dis = 60;
	
	void InitMenbers()
	{
		int memb = 0;

		for(int i = 0; i < m_Alliance.memberInfo.Count; i++)
		{
			if(m_Alliance.memberInfo[i].offlineTime < 0)
			{
				memb++;
			}
		}
		OnLineMenbers.text = memb.ToString()+ "/"+m_Alliance.memberInfo.Count.ToString();
	}

	public UILabel Have_HufuLabel;
	public UILabel HufuLabel;
	public GameObject Level_LeftBtn;
	public GameObject Level_RightBtn;

	public void JuanXianBtn()
	{
		JuanXianObj.SetActive (true);
		GetHuFuNum ();
		if(Maxhufunum >= 1)
		{
			HufuNum = 1;
		}
		else{
			HufuNum = 0;
		}
		initHufuData ();
	
	}
	public void initHufuData()
	{

		Have_HufuLabel.text = "您现在拥有"+Maxhufunum.ToString()+"个虎符";

		HufuLabel.text = HufuNum.ToString ();
		if(Maxhufunum <= 1)
		{
			Level_LeftBtn.SetActive(false);
			
			Level_RightBtn.SetActive(false);

			return;
		}
		if(HufuNum <= 1)
		{
			Level_LeftBtn.SetActive(false);

			Level_RightBtn.SetActive(true);
		}
		else if(HufuNum >= Maxhufunum)
		{
			Level_LeftBtn.SetActive(true);

			Level_RightBtn.SetActive(false);
		}
		else 
		{
			Level_LeftBtn.SetActive(true);
			
			Level_RightBtn.SetActive(true);
		}
	}
	public void ComfirmJuanXian()
	{
		if(HufuNum <= 0)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ResourceLoadCallback2 );
			return ;
		}

		DonationReq (HufuNum);
	}
	public void CloseJuanxianUI()
	{
		JuanXianObj.SetActive (false);
	}
	void GetHuFuNum ()
	{
		Maxhufunum = 0;
		for (int i = 0;i < BagData.Instance().m_bagItemList.Count;i ++)
		{
			if (BagData.Instance().m_bagItemList[i].itemId == 910000 && BagData.Instance().m_bagItemList[i].cnt > 0)
			{
				Maxhufunum += BagData.Instance().m_bagItemList[i].cnt;
			}
		}

		Debug.Log ("Maxhufunum = " +Maxhufunum);
	}
	void DonationReq (int num)
	{
		DonateHuFu donateReq = new DonateHuFu ();
		
		donateReq.count = num;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,donateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.ALLIANCE_HUFU_DONATE,ref t_protof,"30150");

	}

	public GameObject EditUI;
	
	public void  OpenEditnoticeUI()
	{
		EditUI.SetActive (true);
	
		editInfo.text = m_Alliance.notice ;
	}
	public void  CloseEditnoticeUI()
	{
		EditUI.SetActive (false);
		
	}
	//发送编辑公告请求
	public void EditNoticeReq ()
	{
		UpdateNotice noticeReq = new UpdateNotice ();
		
		noticeReq.id = m_Alliance.id;
		
		noticeReq.notice = editInfo.text;
		
		noticeInfo = editInfo.text;
		
		MemoryStream noticeStream = new MemoryStream ();
		
		QiXiongSerializer noticeQx = new QiXiongSerializer ();
		
		noticeQx.Serialize (noticeStream,noticeReq);
		
		byte[] t_protof = noticeStream.ToArray();;
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.UPDATE_NOTICE,ref t_protof,"30130");

	}
	private string confirmStr;
	private string cancelStr;
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
			
			ShoweditInfo.text = noticeInfo;

			m_Alliance.notice = noticeInfo;

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
	public void AllanceBattle()
	{
		AllianceFightData.Instance.OpenAllianceFightMainPage ();
	}

	private bool canOpenShop = true;
	public bool CanOpenShop
	{
		set{canOpenShop = value;}
	}

	//进入商铺
	public void AllanceShop()
	{
		if (canOpenShop)
		{
			canOpenShop = false;
//			GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.ALLANCE,GeneralControl.StoreReqType.FREE);
			ShopData.Instance.OpenShop (ShopData.ShopType.GONGXIAN);
			_MyAllianceManager.Instance().SHow_OR_Close_MyAlliance();
		}
	}
}

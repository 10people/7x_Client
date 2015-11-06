using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ApplyManager : MonoBehaviour ,SocketProcessor {
	private string cancelStr;
	private string confirmStr;

	
	private string titleStr;
	private string str1;
	private string str2;


	LookApplicantsResp m_applicateResp;

	public AllianceHaveResp m_tempInfo;

	public GameObject AppMemberItem;

	private RefuseApplyResp refuseApplyResp;
	private AgreeApplyResp agreeApplyResp;

	public GameObject ReCruitBtn;
	public List<AppMember> m_AppMemberList = new List<AppMember>();
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}
	

	void Update () {
	
	}

	public void Init()
	{
		if(m_tempInfo.identity == 2)
		{
			ReCruitBtn.SetActive(true);
		}
		else
		{
			ReCruitBtn.SetActive(false);
		}
		ApplicateAllianceReq (m_tempInfo);

	}
	GameObject OpenRecruit;
	public void ReCruitSetting()
	{
		if(OpenRecruit)
		{
			return;
		}
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_RECRUIT ),
		                        RecruitLoadCallback );
	}

	//联盟招募异步加载回调
	public void RecruitLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		OpenRecruit = Instantiate( p_object ) as GameObject;
		OpenRecruit.transform.parent = this.transform.parent;
		OpenRecruit.transform.localScale = Vector3.one;
		OpenRecruit.transform.localPosition = Vector3.zero;
		ReCruit mReCruit = OpenRecruit.GetComponent<ReCruit>();
		//mReCruit.Z_UnionInfo = m_tempInfo;

		mReCruit.ChangeNum ();
		mReCruit.initLevel ();
		mReCruit.init ();
	}

	//查看入盟申请成员请求
	void ApplicateAllianceReq (AllianceHaveResp tempInfo)
	{
		LookApplicants applicateReq = new LookApplicants ();
		
		applicateReq.id = tempInfo.id;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,applicateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LOOK_APPLICANTS,ref t_protof,"30124");
		Debug.Log ("ApplicateReq" + ProtoIndexes.LOOK_APPLICANTS);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LOOK_APPLICANTS_RESP://查看申请入盟成员请求返回
			{
				Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				LookApplicantsResp applicateResp = new LookApplicantsResp();
				
				application_qx.Deserialize(application_stream, applicateResp, applicateResp.GetType());
				
				if (applicateResp != null)
				{
					Debug.Log ("申请者信息：" + applicateResp.applicanInfo);

					if (applicateResp.applicanInfo != null)
					{
						m_applicateResp = applicateResp;

						InitApplyMembers();
					}
					else
					{

					}
				}

				return true;
			}
			case ProtoIndexes.REFUSE_APPLY_RESP://拒绝入盟申请请求返回
			{
				Debug.Log ("拒绝返回：" + ProtoIndexes.REFUSE_APPLY_RESP);
				MemoryStream refuse_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer refuse_qx = new QiXiongSerializer();
				
				RefuseApplyResp refuseResp = new RefuseApplyResp();
				
				refuse_qx.Deserialize(refuse_stream, refuseResp, refuseResp.GetType());
				
				if (refuseResp != null)
				{
					refuseApplyResp = refuseResp;
					
					if (refuseApplyResp.result == 0)
					{
						RefreshItemsList (refuseApplyResp.junzhuId, 1);
					}
					
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        RefuseResourceLoadCallback );
					AllianceData.Instance.RequestData();

					AppleNumber -= 1;

					if(AppleNumber <= 0)
					{
						CityGlobalData.AllianceApplyNotice = 0;
					}
				}
				
				return true;
			}
				
			case ProtoIndexes.AGREE_APPLY_RESP://同意入盟申请请求返回
			{
				Debug.Log ("同意返回：" + ProtoIndexes.AGREE_APPLY_RESP);
				MemoryStream agree_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer agree_qx = new QiXiongSerializer();
				
				AgreeApplyResp agreeResp = new AgreeApplyResp();
				
				agree_qx.Deserialize(agree_stream, agreeResp, agreeResp.GetType());
				
				if (agreeResp != null)
				{
					agreeApplyResp = agreeResp;
					
					if (agreeResp.result == 0)
					{
					
						RefreshItemsList (agreeResp.memberInfo.junzhuId ,2);
					}
					
					else if (agreeResp.result == 3)
					{
						RefreshItemsList (agreeResp.junzhuId ,3);
					}
					
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        AgreeResourceLoadCallback );
					AllianceData.Instance.RequestData();
				}
				 AppleNumber --;

				if(AppleNumber <= 0)
				{
					CityGlobalData.AllianceApplyNotice = 0;
				}
				return true;
			}

			default:return false;
			}
		}
		
		return false;
	}
	//刷新申请者列表
	void RefreshItemsList (long junZhuId, int agreeType)
	{
		for (int i = 0;i < m_AppMemberList.Count;i ++)
		{
			if (junZhuId == m_AppMemberList[i].mApplicantInfo.junzhuId)
			{
				backName = m_AppMemberList[i].mApplicantInfo.name;

				m_AppMemberList[i].IsAgree = agreeType;

				m_AppMemberList[i].GetBackData();
			}
		}

	}

	string backName;

	public void RefuseResourceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		titleStr = "拒绝申请";
		
		switch (refuseApplyResp.result)
		{
			
		case 0:
			
			Debug.Log ("Refuse Success!");
			
			str2 = "\r\n"+"\r\n"+"您已拒绝" + backName + "的入盟申请";
			
			break;
			
		case 1:
			
			Debug.Log ("Refuse Fail!");
			
			//str1 = "拒绝失败";
			str2 = "\r\n"+"拒绝失败"+"\r\n"+"\r\n"+"您没有对该申请的拒绝权限";
			
			break;
		}
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str2), null, 
		             null,confirmStr,null,null);
	}

	public void AgreeResourceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		titleStr = "同意申请";
		
		int resultType = agreeApplyResp.result;
		if (resultType == 0)
		{
			Debug.Log ("Agree Success!");
			
			str2 =  "\r\n"+"\r\n"+ "您已同意" + backName + "的入盟申请";
			
			uibox.setBox(titleStr, MyColorData.getColorString (1,str2), null, 
			             null,confirmStr,null,null);
		}
		
		else
		{
			//Debug.Log ("Agree Fail!");
			
			//str1 = "同意入盟申请失败！";
			
			if (resultType == 1)
			{
				Debug.Log ("没有权限");
				str2 = "\r\n"+"同意入盟申请失败！"+"\r\n"+"\r\n"+ "您没有对该申请的同意权限";
			}
			
			else if (resultType == 2)
			{
				Debug.Log ("联盟人数已满");
				str2 = "\r\n"+"同意入盟申请失败！"+"\r\n"+"\r\n"+"联盟人数已满！";
			}
			else if (resultType == 3)
			{
				Debug.Log ("取消申请或加入其他联盟");
				
				//str1 = "同意入盟申请失败";
				
				str2 = "\r\n"+"同意入盟申请失败"+backName +"\r\n"+"\r\n"+ "已取消申请或加入其他联盟";
			}
			uibox.setBox(titleStr, MyColorData.getColorString (1,str2), null, 
			             null,confirmStr,null,null);
		}
	}


	private float Dis = 72;
	private int AppleNumber;
	void InitApplyMembers()
	{
		foreach(AppMember m in m_AppMemberList)
		{
			Destroy( m.gameObject );
		}
		
		m_AppMemberList.Clear ();

		AppleNumber = m_applicateResp.applicanInfo.Count;

		for(int  i = 0; i < m_applicateResp.applicanInfo.Count ; i ++)
		{
			GameObject m_Member = Instantiate(AppMemberItem) as GameObject;
			
			m_Member.SetActive(true);
			
			m_Member.transform.parent = AppMemberItem.transform.parent;
			
			m_Member.transform.localPosition = new Vector3(0,110-i*Dis,0);
			
			m_Member.transform.localScale = Vector3.one;

			AppMember mm__AppMember = m_Member.GetComponent<AppMember>();
			
			mm__AppMember.mApplicantInfo = m_applicateResp.applicanInfo[i];

			mm__AppMember.allianceId = m_tempInfo.id;

			mm__AppMember.m_tAllianceHaveResp = m_tempInfo;

			mm__AppMember.init();
			
			m_AppMemberList.Add(mm__AppMember);
		}
	}
}

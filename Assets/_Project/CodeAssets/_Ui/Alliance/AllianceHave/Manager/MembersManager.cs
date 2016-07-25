using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
using System.Linq;
public class MembersManager : MonoBehaviour , SocketProcessor {

	private LookMembersResp m_membersResp;

	private string titleStr;

	private string str2;

	private string confirmStr;

	public ScaleEffectController m_ScaleEffectController;

	public	AllianceHaveResp m_allianceHaveRes; 

	public GameObject MemberTemp;

	//public GameObject LeaderBtn;

	//public UILabel Label_LeaderBtn;

	//public GameObject FuLeadrBtn;

	//public UILabel Label_FuLeadrBtn;

	private float Dis = 67;

	public long myId;//我的君主id

	private FireMemberResp fireMemberResp;//开除返回
	private UpTitleResp upTitleResp;//升职返回
	private DownTitleResp downTitleResp;//降职返回
	private TransferAllianceResp transAllianceResp;

	private string transName;

	private string backName;

	public List<ShowAllMembers> m_ShowAllMemberList = new List<ShowAllMembers>();

	public List<GameObject> BtnsList = new List<GameObject>();

	//public List<string> Friendsid = new List<string>();
	public GameObject BtnsRoot;
	public static MembersManager mInstance;
	public bool IsDrag = false;
	void Awake()
	{ 
		mInstance = this;

		SocketTool.RegisterMessageProcessor(this);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);

		mInstance = null;
	}

	void Start () {

		jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		closeTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_TITLE);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}
	

	void Update () {
	
		
		if(IsDrag)
		{
			foreach(GameObject btn in BtnsList)
			{
				Destroy(btn);
			}
			BtnsList.Clear();
			IsDrag = false;
		}

	}

	//联盟成员信息请求
	public void AllianceMembersReq (int a_id)
	{
		LookMembers membersReq = new LookMembers ();
		
		membersReq.id = a_id;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,membersReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.LOOK_MEMBERS,ref t_protof,"30116");
	}

	public void Init()
	{
		Transform btns = BtnsRoot.transform.FindChild ("FloatButtons(Clone)");
		if(btns)
		{
			Destroy(btns.gameObject);
		}
		foreach(ShowAllMembers m in m_ShowAllMemberList)
		{
			Destroy( m.gameObject );
		}
		
		m_ShowAllMemberList.Clear ();

		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_GET_FRIEND_IDS);
		AllianceMembersReq (m_allianceHaveRes.id);

	}

	void SortMembers()
	{
		for(int i = 0; i < m_membersResp.memberInfo.Count-1; i++)
		{
			for(int j = i+1; j < m_membersResp.memberInfo.Count; j++)
			{
				if(m_membersResp.memberInfo[i].offlineTime > 0 && m_membersResp.memberInfo[j].offlineTime <= 0)
				{
					var memberinfo = m_membersResp.memberInfo[i];
					
					m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
					
					m_membersResp.memberInfo[j] = memberinfo;
				}
				else if(m_membersResp.memberInfo[i].offlineTime <= 0 && m_membersResp.memberInfo[j].offlineTime <= 0)
				{
					if(m_membersResp.memberInfo[i].identity < m_membersResp.memberInfo[j].identity)
					{
						var memberinfo = m_membersResp.memberInfo[i];
						
						m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
						
						m_membersResp.memberInfo[j] = memberinfo;
					}
					else if(m_membersResp.memberInfo[i].identity == m_membersResp.memberInfo[j].identity)
					{
						if(m_membersResp.memberInfo[i].zhanLi < m_membersResp.memberInfo[j].zhanLi)
						{
							var memberinfo = m_membersResp.memberInfo[i];
							
							m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
							
							m_membersResp.memberInfo[j] = memberinfo;
						}
						else if(m_membersResp.memberInfo[i].zhanLi == m_membersResp.memberInfo[j].zhanLi)
						{
							if(m_membersResp.memberInfo[i].curMonthGongXian < m_membersResp.memberInfo[j].curMonthGongXian)
							{
								var memberinfo = m_membersResp.memberInfo[i];
								
								m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
								
								m_membersResp.memberInfo[j] = memberinfo;
							}
							else if(m_membersResp.memberInfo[i].curMonthGongXian == m_membersResp.memberInfo[j].curMonthGongXian)
							{
								if(m_membersResp.memberInfo[i].contribution < m_membersResp.memberInfo[j].contribution)
								{
									var memberinfo = m_membersResp.memberInfo[i];
									
									m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
									
									m_membersResp.memberInfo[j] = memberinfo;
								}
							}
						}
					}
				}
				else if(m_membersResp.memberInfo[i].offlineTime > 0 && m_membersResp.memberInfo[j].offlineTime > 0)
				{
					if(m_membersResp.memberInfo[i].identity < m_membersResp.memberInfo[j].identity)
					{
						var memberinfo = m_membersResp.memberInfo[i];
						
						m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
						
						m_membersResp.memberInfo[j] = memberinfo;
					}
					else if(m_membersResp.memberInfo[i].identity == m_membersResp.memberInfo[j].identity)
					{
						if(m_membersResp.memberInfo[i].zhanLi < m_membersResp.memberInfo[j].zhanLi)
						{
							var memberinfo = m_membersResp.memberInfo[i];
							
							m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
							
							m_membersResp.memberInfo[j] = memberinfo;
						}
						else if(m_membersResp.memberInfo[i].zhanLi == m_membersResp.memberInfo[j].zhanLi)
						{
							if(m_membersResp.memberInfo[i].curMonthGongXian < m_membersResp.memberInfo[j].curMonthGongXian)
							{
								var memberinfo = m_membersResp.memberInfo[i];
								
								m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
								
								m_membersResp.memberInfo[j] = memberinfo;
							}
							else if(m_membersResp.memberInfo[i].curMonthGongXian == m_membersResp.memberInfo[j].curMonthGongXian)
							{
								if(m_membersResp.memberInfo[i].contribution < m_membersResp.memberInfo[j].contribution)
								{
									var memberinfo = m_membersResp.memberInfo[i];
									
									m_membersResp.memberInfo[i] = m_membersResp.memberInfo[j];
									
									m_membersResp.memberInfo[j] = memberinfo;
								}
							}
						}
					}
				}
			}
		}
		for(int i = 0; i < m_membersResp.memberInfo.Count; i++)
		{
			GameObject m_Member = Instantiate(MemberTemp) as GameObject;
			
			m_Member.SetActive(true);
			
			m_Member.transform.parent = MemberTemp.transform.parent;
			
			m_Member.transform.localPosition = new Vector3(0,120-i*Dis,0);
			
			m_Member.transform.localScale = Vector3.one;
			
			ShowAllMembers mm__ShowAllMembers = m_Member.GetComponent<ShowAllMembers>();
			
			mm__ShowAllMembers.mMemberInfo = m_membersResp.memberInfo[i];
			
			mm__ShowAllMembers.init();
			
			m_ShowAllMemberList.Add(mm__ShowAllMembers);
			
		}
	}

	public void J_inXuanBtn() // 暂时不做 
	{
	
	}

	ExitAllianceResp m_exitResp;

	JunZhuInfo m_JunZhuInfo;
	void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		var temp = Instantiate(p_object) as GameObject;
		var info = temp.GetComponent<KingDetailInfo>();
		
		var tempKingInfo = new KingDetailInfo.KingDetailData()
		{
			RoleID = m_JunZhuInfo.roleId,
			Attack = m_JunZhuInfo.gongji,
			AllianceName = m_JunZhuInfo.lianMeng,
			BattleValue = m_JunZhuInfo.zhanli,
			Junxian = m_JunZhuInfo.junxian,
            JunxianRank = m_JunZhuInfo.junxianRank,
			KingName = m_JunZhuInfo.name,
			Level = m_JunZhuInfo.level,
			Money = m_JunZhuInfo.gongjin,
			Life = m_JunZhuInfo.remainHp,
			Protect = m_JunZhuInfo.fangyu,
            Title = m_JunZhuInfo.chenghao
		};
		foreach(ShowAllMembers menber in m_ShowAllMemberList)
		{
			if(menber.mMemberInfo.junzhuId == m_JunZhuInfo.junZhuId)
			{
				if(menber.mMemberInfo.isVoted == 0)
				{
					var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>()
					{
						//new KingDetailButtonController.KingDetailButtonConfig() {m_ButtonStr = "投TA一票", m_ButtonClick = TouPiao},
						
					};
					//info.SetThis(tempKingInfo, tempConfigList);
				}
				
				if(menber.mMemberInfo.isVoted == 1)
				{
					info.m_VotedSprite.gameObject.SetActive(true);
					info.m_VotedSprite.spriteName = "Apply";
				}
				if(menber.mMemberInfo.isVoted == 2)
				{
					info.m_VotedSprite.gameObject.SetActive(true);
					info.m_VotedSprite.spriteName = "GiveUp";
				}
				info.m_KingDetailEquipInfo.m_BagItemDic = m_JunZhuInfo.equip.items.Where(item => item.buWei > 0).ToDictionary(item => TransferBuwei(item.buWei));

			}
		}

		temp.SetActive(true);
		MainCityUI.TryAddToObjectList (temp);
	}

	public int TransferBuwei(int original)
	{
		switch (original)
		{
		case 1: return 3;
		case 2: return 4;
		case 3: return 5;
		case 11: return 0;
		case 12: return 8;
		case 13: return 1;
		case 14: return 7;
		case 15: return 2;
		case 16: return 6;
		default: Debug.LogError("Error buwei in Transfer in DetailController, Buwei:" + original); return -1;
		}
	}
	public  void TouPiao()
	{
		//FriendOperationLayerManagerment.AddFriends((int)mMemberInfo.junzhuId);
		//DestroyFloatButtons();
	}
	//接收Union求返回的数据
	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LOOK_MEMBERS_RESP://成员信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LookMembersResp membersResp = new LookMembersResp();
				
				t_qx.Deserialize(t_stream, membersResp, membersResp.GetType());
				Debug.Log ("联盟成员信息返回");
				
				m_membersResp = membersResp;
				
				SortMembers();

				return true;
			}
			
			case ProtoIndexes.FIRE_MEMBER_RESP://开除返回
			{
				MemoryStream fire_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer fire_qx = new QiXiongSerializer();
				
				FireMemberResp fireResp = new FireMemberResp();
				
				fire_qx.Deserialize (fire_stream, fireResp, fireResp.GetType());

				if (fireResp != null)
				{
					fireMemberResp = fireResp;
					
					//Debug.Log ("开除：" + fireResp.junzhuId);
					for (int i = 0;i < m_membersResp.memberInfo.Count;i ++)
					{
						if (fireResp.junzhuId == m_membersResp.memberInfo[i].junzhuId)
						{
							backName = m_membersResp.memberInfo[i].name;
							//	Debug.Log ("BackName:" + backName);
							//		Debug.Log ("id:::::" + m_membersResp.memberInfo[i].junzhuId);
							m_membersResp.memberInfo.Remove (m_membersResp.memberInfo[i]);
						}
					}
					AllianceData.Instance.RequestData ();
					Init();
					//开除返回提示弹窗
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        FireResourceLoadCallback );
				}
				return true;

			}
				
			case ProtoIndexes.UP_TITLE_RESP://升职返回
			{
				MemoryStream up_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer up_qx = new QiXiongSerializer();
				
				UpTitleResp upResp = new UpTitleResp();
				
				up_qx.Deserialize (up_stream,upResp,upResp.GetType ());
				if(upResp.code == 3)
				{
					string mstr2 = "当前正在进行联盟战，无法进行升职操作！";
					ClientMain.m_UITextManager.createText(mstr2);
				}
				else{
					if (upResp != null)
					{
						upTitleResp = upResp;
						
						for (int i = 0;i < m_membersResp.memberInfo.Count;i ++)
						{
							if (upResp.junzhuId == m_membersResp.memberInfo[i].junzhuId)
							{
								backName = m_membersResp.memberInfo[i].name;
								//	Debug.Log ("BackName:" + backName);
								//	Debug.Log ("id:::::" + m_membersResp.memberInfo[i].junzhuId);
								
								if (upResp.code == 0)
								{
									//	Debug.Log ("地位：" + upResp.title);
									//	Debug.Log ("升职成功");
									m_membersResp.memberInfo[i].identity = upResp.title;
									
								}
								
								else if (upResp.code == 2)
								{
									m_membersResp.memberInfo.Remove (m_membersResp.memberInfo[i]);
									
								}
							}
						}
						AllianceData.Instance.RequestData ();
						Init();
						
						//升职返回提示弹窗
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        UpResourceLoadCallback );
					}
				}

				
				return true;
			}

			case ProtoIndexes.DOWN_TITLE_RESP://降职返回
			{
				MemoryStream down_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer down_qx = new QiXiongSerializer();
				
				DownTitleResp downResp = new DownTitleResp();
				
				down_qx.Deserialize(down_stream, downResp, downResp.GetType());

				if(downResp.code == 2)
				{
					string mstr2 = "当前正在进行联盟战，无法进行降职操作！";
					ClientMain.m_UITextManager.createText(mstr2);
				}
				else
				{
					if (downResp != null)
					{
						downTitleResp = downResp;
						
						for (int i = 0;i < m_membersResp.memberInfo.Count;i ++)
						{
							if (downResp.junzhuId == m_membersResp.memberInfo[i].junzhuId)
							{
								backName = m_membersResp.memberInfo[i].name;
								//	Debug.Log ("BackName:" + backName);
								//	Debug.Log ("id:::::" + m_membersResp.memberInfo[i].junzhuId);
								if (downResp.code == 0)
								{
									//	Debug.Log ("你被降职了");
									//	Debug.Log ("identy:" + downResp.title);
									m_membersResp.memberInfo[i].identity = downResp.title;
								}
								
								else if (downResp.code == 1)
								{
									//Debug.Log ("该玩家已经不在联盟中");
									m_membersResp.memberInfo.Remove (m_membersResp.memberInfo[i]);
								}
							}
						}
						AllianceData.Instance.RequestData ();
						Init();
						
						//降职返回提示弹窗
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        DownResourceLoadCallback );
					}
				}
			
				
				return true;
			}
			case ProtoIndexes.TRANSFER_ALLIANCE_RESP://转让请求返回
			{
				MemoryStream trans_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer trans_qx = new QiXiongSerializer();
				
				TransferAllianceResp transResp = new TransferAllianceResp();
				
				trans_qx.Deserialize(trans_stream, transResp, transResp.GetType());
				if(transResp.result == 2)
				{
					string mstr2 = "当前正在进行联盟战，无法进行开除操作！";
					ClientMain.m_UITextManager.createText(mstr2);
				}
				else{
					if (transResp != null)
					{
						transAllianceResp = transResp;
						
						if (transResp.result == 0)
						{
							Debug.Log ("转让成功！");
							
							for (int i = 0;i < m_membersResp.memberInfo.Count;i ++)
							{
								
								if (transResp.junzhuId == m_membersResp.memberInfo[i].junzhuId)
								{
									transName = m_membersResp.memberInfo[i].name;
									m_membersResp.memberInfo[i].identity = 2;
									m_allianceHaveRes.identity = 1;
								}
								if(m_membersResp.memberInfo[i].junzhuId == JunZhuData.Instance().m_junzhuInfo.id )
								{
									m_membersResp.memberInfo[i].identity = 1;
								}
							}
						}
						
						else if (transResp.result == 1)
						{
							//Debug.Log ("不在联盟列表里！");
							
							for (int i = 0;i < m_membersResp.memberInfo.Count;i ++)
							{
								if (transResp.junzhuId == m_membersResp.memberInfo[i].junzhuId)
								{
									transName = m_membersResp.memberInfo[i].name;
									
									m_membersResp.memberInfo.Remove (m_membersResp.memberInfo[i]);
								}
							}
						}
						
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        TransAllianceLoadBack);
						AllianceData.Instance.RequestData ();
						Init();
					}
				}
			
				return true;
			}
			default: 
				return false;
			}
		}
		
		return false;
	}

	void TransAllianceLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str2 = "";
		
		string confirmStr = "确定";
		
		if (transAllianceResp.result == 0)
		{
			titleStr = "转让成功";
			str2 = "您已将联盟转让给副盟主" + transName + "！";
			
//			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str2),null,confirmStr,null,
//			             null);
		}
		
		else if (transAllianceResp.result == 1)
		{
			titleStr = "转让失败";
			str2 = transName + "已不在联盟中！";
			
//			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str2),null,confirmStr,null,
//			             null);
		}
		ClientMain.m_UITextManager.createText (str2);
	}
	//开除loadCallBack
	public void FireResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
//		GameObject boxObj = Instantiate( p_object ) as GameObject;
//		UIBox uibox = boxObj.GetComponent<UIBox> ();
//		
		if (fireMemberResp.result == 0)
		{
			//Debug.Log ("开除成功");
			
			titleStr = "开除成功";
			
			str2 = "您已将" + backName + "开除出联盟！";
		}
		else if (fireMemberResp.result == 1)
		{
			titleStr = "开除失效";
			
			str2 = backName + "已不在联盟中！";
		}
		
//		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
//		             null,confirmStr,null,null);
		ClientMain.m_UITextManager.createText (str2);
	}


	//升职loadCallBack
	public void UpResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
//		GameObject boxObj = Instantiate( p_object ) as GameObject;
//		UIBox uibox = boxObj.GetComponent<UIBox> ();
//		
		if (upTitleResp.code == 0)
		{
			titleStr = "升职成功";
			
			str2 = "您已将" + backName + "升职为副盟主!";
		}
		
		else if (upTitleResp.code == 1)
		{
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_SHENGZHI_FAIL);
			
			str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_FU_LEADER_NUM);
		}
		
		else if (upTitleResp.code == 2)
		{
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_SHENGZHI_FAIL);
			
			str2 = backName + "已不在联盟中！";
		}
//		
//		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
//		             null,confirmStr,null,null);

		ClientMain.m_UITextManager.createText (str2);
	}

	
	//降职loadCallBack
	public void DownResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
//		GameObject boxObj = Instantiate( p_object ) as GameObject;
//		UIBox uibox = boxObj.GetComponent<UIBox> ();
//		
		if (downTitleResp.code == 0)
		{
			titleStr = "降职成功";
			
			str2 = "您已将" + backName + "降职为普通成员";
		}
		
		else if (downTitleResp.code == 1)
		{
			titleStr = "降职失效";
			
			str2 = backName + "已不在联盟中！";
		}
		ClientMain.m_UITextManager.createText (str2);
//		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
//		             null,confirmStr,null,null);
	}


	//退出联盟返回提示框异步加载回调
	public void ExitAllianceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string exitTitleStr = "退出联盟";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);
		
		if(m_exitResp.code == 0)
		{
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);
			string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES2);
			
			uibox.setBox(exitTitleStr, str1, str2,null,confirmStr,null,DeletUI_i);
		}
		else
		{
		//	Debug.Log("退出失败");
			
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL_REASON);
			uibox.setBox(exitTitleStr, str1,str2,null,confirmStr,null,null);
		}
	}

	public void DeletUI_i(int i)//Quite。
	{
		JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
		AllianceData.Instance.RequestData();
		//SceneManager.EnterMainCity();
		GameObject uirot = GameObject.Find("_My_Union(Clone)");
		
		if(uirot)
		{
			MainCityUI.TryRemoveFromObjectList(uirot);
			Destroy(uirot);
		}
	}
	void DoCloseWindowWithEnterMainCity()
	{
	
	}
	//联盟解散成功异步加载回调
	public void DisAllianceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
		
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		//PlayerModelController.m_playerModelController.m_isCanUpdatePosition = false;
		//Vector3 vec_pos = new Vector3(-1.0f, 0.4f, -63.0f);
		//PlayerModelController.m_playerModelController.UploadPlayerPosition(vec_pos);
		CityGlobalData.m_isMainScene = true;
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR1);
		string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR2);
		
		uibox.setBox(jieSanTitleStr,str1, str2,null,confirmStr,null,DisAllianceSuccessBack);

	}

	void DisAllianceSuccessBack (int i)
	{
		GameObject uirot = GameObject.Find("_My_Union(Clone)");
		
		if(uirot)
		{
			MainCityUI.TryRemoveFromObjectList(uirot);
			Destroy(uirot);
		}
		JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
        CityGlobalData.m_isAllianceScene = false;
        CityGlobalData.m_isMainScene = true;
        SceneManager.EnterMainCity();
	}
	private string jieSanTitleStr;
	private string closeTitleStr;

	private string cancelStr;


	public void JieSanAlliance() // 解散联盟
	{
		if(m_allianceHaveRes.identity == 2) // 盟主
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        AllianceTransTipsLoadCallback1 );
		}
		else
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ExitLoadCallback );
		}

	}

	//退出联盟提示框异步加载回调
	public void ExitLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR1);
		string str2 = "\n\r"+str1+"\n\r"+LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR2);
		
		string exitTitle = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_TITLE);
		
		uibox.setBox(exitTitle, str2, null,
		             null,cancelStr,confirmStr,ExitAllianceReq);
	}

	//发送退出联盟的请求
	void ExitAllianceReq (int i)
	{
		if (i == 2)
		{
			ExitAlliance exitReq = new ExitAlliance ();
			
			exitReq.id = m_allianceHaveRes.id;
			
			MemoryStream exitStream = new MemoryStream ();
			
			QiXiongSerializer exitQx = new QiXiongSerializer ();
			
			exitQx.Serialize (exitStream, exitReq);
			
			byte[] t_protof = exitStream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.EXIT_ALLIANCE, ref t_protof, "30114");
		}
	}
	

	//解散联盟提示异步加载回调
	public void AllianceTransTipsLoadCallback1 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR2);
		
		uibox.setBox(jieSanTitleStr,str1, str2,null,cancelStr,confirmStr,DisAlliance);
	}
	void DisAlliance (int i)
	{
		if(i == 2)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        AllianceTransTipsLoadCallback2 );
		}
	}
	//解散联盟再次提示异步加载回调
	public void AllianceTransTipsLoadCallback2 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR3);
		string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR4);
		
		string sanSiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SANSI);
		string jieSanStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN);
		
		uibox.setBox(jieSanTitleStr,str1, str2,null,sanSiStr,jieSanStr,DisAllianceReq);
	}
	//发送解散联盟请求
	void DisAllianceReq (int i)
	{
		if(i == 2)
		{
			DismissAlliance disAllianceReq = new DismissAlliance ();
			
			disAllianceReq.id = m_allianceHaveRes.id;
			
			MemoryStream dis_stream = new MemoryStream ();
			
			QiXiongSerializer disQx = new QiXiongSerializer ();
			
			disQx.Serialize (dis_stream,disAllianceReq);
			
			byte[] t_protof = dis_stream.ToArray();;
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.DISMISS_ALLIANCE,ref t_protof,"30132");
		//	Debug.Log ("jiesanReq:" + ProtoIndexes.DISMISS_ALLIANCE);
		}
	}
	public void GetInfo()
	{

	}
	
	public void AddFriend()
	{
		
	}
}

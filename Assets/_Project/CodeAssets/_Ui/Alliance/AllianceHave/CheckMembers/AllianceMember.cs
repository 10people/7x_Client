using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceMember : MonoBehaviour,SocketProcessor {

	public static AllianceMember a_member;

	private AllianceHaveResp m_allianceInfo;//联盟信息
	private LookMembersResp m_membersInfo;//成员信息

	public GameObject memberManagerObj;
	
	public GameObject zheZhao;//遮罩

	public GameObject selectObj;//选中的对勾

	private bool isMemberChange = false;

	private FireMemberResp fireMemberResp;//开除返回
	private UpTitleResp upTitleResp;//升职返回
	private DownTitleResp downTitleResp;//降职返回

	private string titleStr;
//	private string str1;
	private string str2;
//	private string cancelStr;
	private string confirmStr;
	
	private string backName;

	void Awake ()
	{
		a_member = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
//		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}

	//刷新成员
	public void GetMembersInfo (LookMembersResp tempResp,AllianceHaveResp allianceInfo)
	{
		m_allianceInfo = allianceInfo;
		m_membersInfo = tempResp;

		InItMembers memberManager = memberManagerObj.GetComponent<InItMembers>();
		memberManager.GetMembersInfo (tempResp,allianceInfo);
	}

	//成员操作返回
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null) 
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.FIRE_MEMBER_RESP://开除返回
				
				MemoryStream fire_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer fire_qx = new QiXiongSerializer();
				
				FireMemberResp fireResp = new FireMemberResp();
				
				fire_qx.Deserialize (fire_stream, fireResp, fireResp.GetType());
				
				if (fireResp != null)
				{
					fireMemberResp = fireResp;

				//	Debug.Log ("开除：" + fireResp.junzhuId);
					for (int i = 0;i < m_membersInfo.memberInfo.Count;i ++)
					{
						if (fireResp.junzhuId == m_membersInfo.memberInfo[i].junzhuId)
						{
							backName = m_membersInfo.memberInfo[i].name;
							//Debug.Log ("BackName:" + backName);
							//Debug.Log ("id:::::" + m_membersInfo.memberInfo[i].junzhuId);
							m_membersInfo.memberInfo.Remove (m_membersInfo.memberInfo[i]);
						}
					}
					
					InItMembers memberManager = memberManagerObj.GetComponent<InItMembers>();
					memberManager.GetMembersInfo (m_membersInfo,m_allianceInfo);

					isMemberChange = true;

					//开除返回提示弹窗
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        FireResourceLoadCallback );
				}
				
				return true;
				
			case ProtoIndexes.UP_TITLE_RESP://升职返回
				
				MemoryStream up_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer up_qx = new QiXiongSerializer();
				
				UpTitleResp upResp = new UpTitleResp();
				
				up_qx.Deserialize (up_stream,upResp,upResp.GetType ());
				
				if (upResp != null)
				{
					upTitleResp = upResp;

					for (int i = 0;i < m_membersInfo.memberInfo.Count;i ++)
					{
						if (upResp.junzhuId == m_membersInfo.memberInfo[i].junzhuId)
						{
							backName = m_membersInfo.memberInfo[i].name;
							//Debug.Log ("BackName:" + backName);
							//Debug.Log ("id:::::" + m_membersInfo.memberInfo[i].junzhuId);

							if (upResp.code == 0)
							{
							//	Debug.Log ("地位：" + upResp.title);
							//	Debug.Log ("升职成功");
								m_membersInfo.memberInfo[i].identity = upResp.title;
								isMemberChange = true;
							}

							else if (upResp.code == 2)
							{
								m_membersInfo.memberInfo.Remove (m_membersInfo.memberInfo[i]);
								isMemberChange = true;
							}
						}
					}
					InItMembers memberManager = memberManagerObj.GetComponent<InItMembers>();
					memberManager.GetMembersInfo (m_membersInfo,m_allianceInfo);

					isMemberChange = true;

					//升职返回提示弹窗
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        UpResourceLoadCallback );
				}
				
				return true;
				
			case ProtoIndexes.DOWN_TITLE_RESP://降职返回
				
				MemoryStream down_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer down_qx = new QiXiongSerializer();
				
				DownTitleResp downResp = new DownTitleResp();
				
				down_qx.Deserialize(down_stream, downResp, downResp.GetType());
				
				if (downResp != null)
				{
					downTitleResp = downResp;

					for (int i = 0;i < m_membersInfo.memberInfo.Count;i ++)
					{
						if (downResp.junzhuId == m_membersInfo.memberInfo[i].junzhuId)
						{
							backName = m_membersInfo.memberInfo[i].name;
							//Debug.Log ("BackName:" + backName);
						//	Debug.Log ("id:::::" + m_membersInfo.memberInfo[i].junzhuId);
							if (downResp.code == 0)
							{
							//	Debug.Log ("你被降职了");
							//	Debug.Log ("identy:" + downResp.title);
								m_membersInfo.memberInfo[i].identity = downResp.title;
							}
							
							else if (downResp.code == 1)
							{
								//Debug.Log ("该玩家已经不在联盟中");
								m_membersInfo.memberInfo.Remove (m_membersInfo.memberInfo[i]);
							}
						}
					}

					InItMembers memberManager = memberManagerObj.GetComponent<InItMembers>();
					memberManager.GetMembersInfo (m_membersInfo,m_allianceInfo);
					
					isMemberChange = true;

					//降职返回提示弹窗
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        DownResourceLoadCallback );
				}
				
				return true;
			}
		}

		return false;
	}

	//开除loadCallBack
	public void FireResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();

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

		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
		             null,confirmStr,null,WarringSureBtn);
	}

	//升职loadCallBack
	public void UpResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		if (upTitleResp.code == 0)
		{
			titleStr = "升职成功";

			str2 = "您已将" + backName + "升职为副盟主";
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

		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
		             null,confirmStr,null,WarringSureBtn);
	}

	//降职loadCallBack
	public void DownResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
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

		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
		             null,confirmStr,null,WarringSureBtn);
	}

	//控制遮罩的显隐
	public void MakeZheZhao (bool flag)
	{
		if (flag)
		{
			zheZhao.SetActive (true);
		}
		
		else 
		{
			zheZhao.SetActive (false);
		}
	}
	
	void WarringSureBtn (int i)
	{
		MakeZheZhao (false);
	}
	
	//克隆被选择的对勾
	public void Select (GameObject parentObj, Vector3 pos)
	{
		GameObject select = (GameObject)Instantiate (selectObj);
		
		select.SetActive (true);
		select.name = "SelectMe";
		
		select.transform.parent = parentObj.transform;
		
		select.transform.localPosition = pos;
		
		select.transform.localScale = Vector3.one;
	}
	
	//销毁item上的对勾
	public void DestroySelect ()
	{
		if (GameObject.Find ("SelectMe") != null)
		{
			Destroy (GameObject.Find ("SelectMe"));
		}
	}

	//返回
	public void Back ()
	{
		if (isMemberChange)
		{
			AllianceData.Instance.RequestData ();
		}

		Destroy (this.gameObject);
	}
	
	//关闭
	public void CloseAll ()
	{
		if (isMemberChange)
		{
			AllianceData.Instance.RequestData ();
		}

		Destroy (GameObject.Find ("My_Union(Clone)"));
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

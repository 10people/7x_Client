using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TransAlliance : MonoBehaviour,SocketProcessor {

	public static TransAlliance trans;

	private AllianceHaveResp allianceResp;
	private LookMembersResp membersResp;
	private TransferAllianceResp transAllianceResp;

	public long myId;//我的君主id
	
	private List<MemberInfo> m_infoList = new List<MemberInfo>();//用于存放副盟主的列表
	
	public GameObject zheZhao;//遮罩
	
	public GameObject leaderObj;//副盟主obj
	
	private List<GameObject> leaderList = new List<GameObject> ();//用于存放副盟主item的list
	
	public UIGrid leaderGrid;
	
	public GameObject selectObj;//对号

	public UILabel noFuLeaderDes;//没有副盟主时的描述

	private string transName;

	private bool isTrans = false;

	void Awake ()
	{
		trans = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{

	}

	//获得自己联盟信息
	public void GetOwnLianMeng (AllianceHaveResp tempAllianceResp,LookMembersResp tempMemberResp)
	{
		allianceResp = tempAllianceResp;
		membersResp = tempMemberResp;

		CreateMembers (tempAllianceResp,tempMemberResp);
	}

	//创建副盟主列表
	void CreateMembers (AllianceHaveResp m_allianceInfo,LookMembersResp m_MemberResp)
	{
		m_infoList.Clear ();

		//将副盟主添加至list中
		foreach (MemberInfo member in membersResp.memberInfo)
		{
			if (member.identity == 1)
			{
				m_infoList.Add (member);
			}
		}
		
		if (m_infoList.Count != 0)
		{
			noFuLeaderDes.text = "";
			
			//对副盟主按等级排序
			for (int i = 0;i < m_infoList.Count - 1;i ++)
			{
				for (int j = 0;j < m_infoList.Count - i -1;j ++)
				{
					if (m_infoList[i].level < m_infoList[i + 1].level)
					{
						MemberInfo tempInfo = m_infoList[i];
						
						m_infoList[i] = m_infoList[i + 1];
						
						m_infoList[i + 1] = tempInfo;
					}
				}
			}
			
			Debug.Log ("m_infoList:" + m_infoList.Count);
			
			ClearItems ();

			foreach (MemberInfo m_info in m_infoList)
			{
				GameObject leader = (GameObject)Instantiate (leaderObj);
				
				leader.SetActive (true);
				leader.name = "Leader";
				
				leader.transform.parent = leaderGrid.gameObject.transform;
				leader.transform.localPosition = Vector3.zero;
				leader.transform.localScale = Vector3.one;
				
				LeaderItem l_item = leader.GetComponent<LeaderItem> ();
				l_item.ShowLeaderItemInfo (m_info,m_allianceInfo);
				
				leaderList.Add (leader);
			}
			
			leaderGrid.repositionNow = true;
		}

		else
		{
			Debug.Log ("无副盟主");
			ClearItems ();
			noFuLeaderDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NO_FU_LEADER_DES);
		}
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.TRANSFER_ALLIANCE_RESP://转让请求返回
				
				MemoryStream trans_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer trans_qx = new QiXiongSerializer();
				
				TransferAllianceResp transResp = new TransferAllianceResp();
				
				trans_qx.Deserialize(trans_stream, transResp, transResp.GetType());
				
				if (transResp != null)
				{
					transAllianceResp = transResp;

					isTrans = true;

					if (transResp.result == 0)
					{
						Debug.Log ("转让成功！");
						for (int i = 0;i < membersResp.memberInfo.Count;i ++)
						{
							if (transResp.junzhuId == membersResp.memberInfo[i].junzhuId)
							{
								transName = membersResp.memberInfo[i].name;
								membersResp.memberInfo[i].identity = 2;
							}

							else if (myId == membersResp.memberInfo[i].junzhuId)
							{
								membersResp.memberInfo[i].identity = 1;
							}
						}
					}
					
					else if (transResp.result == 1)
					{
						Debug.Log ("不在联盟列表里！");

						for (int i = 0;i < membersResp.memberInfo.Count;i ++)
						{
							if (transResp.junzhuId == membersResp.memberInfo[i].junzhuId)
							{
								transName = membersResp.memberInfo[i].name;
								
								membersResp.memberInfo.Remove (membersResp.memberInfo[i]);
							}
						}
					}

					CreateMembers (allianceResp,membersResp);

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        TransAllianceLoadBack);
				}
				
				return true;
			}
		}
		return false;
	}

	void TransAllianceLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str2 = "";

		string confirmStr = "确定";

		if (transAllianceResp.result == 0)
		{
			titleStr = "转让成功";
			str2 = "您已将联盟转让给副盟主" + transName + "！";

			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str2),null,confirmStr,null,
			             TransSuccessBack);
		}

		else if (transAllianceResp.result == 1)
		{
			titleStr = "转让失败";
			str2 = transName + "已不在联盟中！";
			
			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str2),null,confirmStr,null,
			             TransFailBack);
		}
	}

	void TransSuccessBack (int i)
	{
		AllianceData.Instance.RequestData ();
		Destroy (GameObject.Find ("Leader_Setting(Clone)"));
		Destroy (this.gameObject);
	}

	void TransFailBack (int i)
	{
		MakeZheZhao (false);
	}

	void ClearItems ()
	{
		foreach (GameObject leaderObj in leaderList)
		{
			Destroy (leaderObj);
		}
		leaderList.Clear ();
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
		if (isTrans)
		{
			AllianceData.Instance.RequestData ();
		}

		Destroy (this.gameObject);
	}
	
	//关闭
	public void CloseAll ()
	{
		if (isTrans)
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

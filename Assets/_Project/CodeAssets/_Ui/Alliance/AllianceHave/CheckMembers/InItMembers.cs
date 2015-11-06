using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class InItMembers : MonoBehaviour {

	public GameObject memberItem;//成员item

	public UIGrid memberGrid;

	public List<MemberInfo> memberInfoList = new List<MemberInfo> ();//联盟所有成员列表

	private List<MemberInfo> leaderList = new List<MemberInfo>();//副盟主成员列表
	private List<MemberInfo> generalList = new List<MemberInfo> ();//普通成员列表

	private List<GameObject> memberItemList = new List<GameObject> ();//实例化出的成员item列表

	private AllianceHaveResp m_allianceInfo;//联盟信息

	public GameObject leftTurnBtn;//向左滑动按钮
	public GameObject rightTurnBtn;//向右滑动按钮

	public UIPanel scrollViewPanel;
	private int column;//成员列表的列数

	public int offect;//滑动偏移量

	public GameObject electionInfoBtn;//选举情况按钮
	private LookMembersResp lookResp;//联盟成员信息

	void Start ()
	{
		leftTurnBtn.SetActive (false);
		rightTurnBtn.SetActive (false);
	}

	//获得成员信息
	public void GetMembersInfo (LookMembersResp tempResp,AllianceHaveResp allianceInfo)
	{
		lookResp = tempResp;
		m_allianceInfo = allianceInfo;

		//计算列个数
		if (tempResp.memberInfo.Count % 2 == 0)
		{
			column = tempResp.memberInfo.Count / 2;
		}
		else
		{
			column = tempResp.memberInfo.Count / 2 + 1;
		}

		//清空list
		if (memberInfoList != null || leaderList != null || generalList != null)
		{
			memberInfoList.Clear ();
			leaderList.Clear ();
			generalList.Clear ();
		}

		//对不同职位的成员进行分类
		foreach (MemberInfo tempInfo in tempResp.memberInfo)
		{
			switch (tempInfo.identity)
			{
			case 0:

				generalList.Add (tempInfo);

				break;

			case 1:

				leaderList.Add (tempInfo);

				break;

			case 2:

				memberInfoList.Add (tempInfo);

				break;
			}
		}

		Debug.Log ("memberInfoList1:" + memberInfoList.Count);
		Debug.Log ("leaderList:" + leaderList.Count);
		Debug.Log ("generalList" + generalList.Count);

		//副盟主列表排序
		for (int i = 0;i < leaderList.Count - 1;i ++)
		{
			for (int j = 0;j < leaderList.Count - i - 1;j ++)
			{
				if (leaderList[j].level < leaderList[j + 1].level)
				{
					MemberInfo temp = leaderList[j];
					leaderList[j] = leaderList[j + 1];
					leaderList[j + 1] = temp;
				}
			}
		}
		//将副盟主加入联盟所有成员列表 by level
		foreach (MemberInfo leader in leaderList)
		{
			memberInfoList.Add (leader);
		}

		Debug.Log ("memberInfoList2:" + memberInfoList.Count);

		//普通成员列表排序 by level
		for (int i = 0;i < generalList.Count - 1;i ++)
		{
			for (int j = 0;j < generalList.Count - i -1;j ++)
			{
				if (generalList[j].level < generalList[j + 1].level)
				{
					MemberInfo temp = generalList[j];
					generalList[j] = generalList[j + 1];
					generalList[j + 1] = temp;
				}
			}
		}
		//将普通成员加入联盟所有成员列表
		foreach (MemberInfo general in generalList)
		{
			memberInfoList.Add (general);
		}

		Debug.Log ("memberInfoList3:" + memberInfoList.Count);

		CreateMembers (memberInfoList);

		if (allianceInfo.status == 2 && allianceInfo.isVoted == 1)
		{
			electionInfoBtn.SetActive (true);
		}
	}

	//创建成员列表
	void CreateMembers (List<MemberInfo> m_memberList)
	{
		//清空item表
		foreach (GameObject memberItem in memberItemList)
		{
			Destroy (memberItem);
		}
		
		memberItemList.Clear ();
		Debug.Log ("item列表个数：" + memberItemList.Count);

		foreach (MemberInfo m_info in m_memberList)
		{
			GameObject member = (GameObject)Instantiate (memberItem);
			
			member.SetActive (true);

			member.transform.parent = memberGrid.gameObject.transform;

			member.transform.localPosition = Vector3.zero;

			member.transform.localScale = Vector3.one;
			
			MemberItem m_item = member.GetComponent<MemberItem> ();
			m_item.ShowMemberItemInfo (m_info,m_allianceInfo);

			memberItemList.Add (member);
		}

		memberGrid.repositionNow = true;
	}

	void Update ()
	{
		//列数少于4的时候，整体会回弹到最左边
		if (column <= 4)
		{
			memberGrid.gameObject.GetComponent<ItemTopCol>().enabled = true;
		}
		else
		{
			memberGrid.gameObject.GetComponent<ItemTopCol>().enabled = false;
		}
	}

	void FixedUpdate () 
	{
		scrollViewPanel.clipOffset = new Vector2(-scrollViewPanel.gameObject.transform.localPosition.x, 0);

		int Move_x = column*187 -offect+(int)scrollViewPanel.cachedGameObject.transform.localPosition.x;
		if(column <= 4)
		{
			leftTurnBtn.SetActive(false);
			rightTurnBtn.SetActive(false);
			return;
		}

		if(Move_x <= 5)
		{
			rightTurnBtn.SetActive(false);
			leftTurnBtn.SetActive(true);
		}

		else if(scrollViewPanel.cachedGameObject.transform.localPosition.x >= -10)
		{
			rightTurnBtn.SetActive(true);
			leftTurnBtn.SetActive(false);
		}

		else
		{
			rightTurnBtn.SetActive(true);
			leftTurnBtn.SetActive(true);
		}
	}

	//右移
	public void RightMove()
	{
		StartCoroutine( StartMove (1,column));
	}

	//左移
	public void LeftMove()
	{
		StartCoroutine( StartMove (-1,column));
	}

	IEnumerator StartMove(int i,int j)
	{
		int moveX ;
		if(i == 1)//向右移动
		{
			
			moveX = j*187 -offect+(int)scrollViewPanel.cachedGameObject.transform.localPosition.x;
			
			if(moveX > offect)
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x - offect, -30f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				Debug.Log("moveX" +moveX);
				Debug.Log("scrollor.cacx" +scrollViewPanel.cachedGameObject.transform.localPosition.x);
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x - moveX, -30f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
		else
		{
			moveX = (int)(-scrollViewPanel.cachedGameObject.transform.localPosition.x);
			if(moveX > offect)
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x + offect, -30f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x + moveX, -30f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	//查看选举情况
	public void CheckElectionInfo ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_ELECTION_INFO ),
		                        ResourceLoadCallback );
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{	
		GameObject electionInfo = Instantiate( p_object ) as GameObject;
		
		electionInfo.transform.parent = GameObject.Find ("My_Union(Clone)").transform.FindChild ("Camera").transform;
		electionInfo.transform.localPosition = Vector3.zero;
		electionInfo.transform.localScale = Vector3.one;
		
		ElectionInfoManager electionInfoMan = electionInfo.GetComponent<ElectionInfoManager> ();
		electionInfoMan.GetElectionMember (lookResp,m_allianceInfo);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ElectionInfoManager : MonoBehaviour {

	private List<MemberInfo> electorList = new List<MemberInfo> ();//参选人列表

	private List<GameObject> electorObjList = new List<GameObject> ();//存储electorObj的list

	public GameObject electorObj;//参选人obj

	public UIGrid electionInfoGrid;

	private AllianceHaveResp e_allianceInfo;

	public GameObject leftTurnBtn;//向左滑动按钮
	public GameObject rightTurnBtn;//向右滑动按钮
	
	public UIPanel scrollViewPanel;
	private int column;//成员列表的列数
	
	public int offect;//滑动偏移量

	void Start ()
	{	
		leftTurnBtn.SetActive (false);
		rightTurnBtn.SetActive (false);
	}

	//获得联盟人员信息
	public void GetElectionMember (LookMembersResp membersResp,AllianceHaveResp allianceInfo)
	{
		e_allianceInfo = allianceInfo;

		electorList.Clear ();

		foreach (MemberInfo member in membersResp.memberInfo)
		{
			if (member.isBaoming == 1)
			{
				electorList.Add (member);
			}
		}

		foreach(MemberInfo m in electorList)
		{
			Debug.Log ("叫啥？：" + m.name + "/" + "得票：" + m.voteNum);
		}

		for (int i = 0;i < electorList.Count - 1;i ++)
		{
			for (int j = 0;j < electorList.Count - i - 1;j ++)
			{
				if (electorList[j].voteNum < electorList[j + 1].voteNum)
				{
					MemberInfo tempMember = electorList[j];

					electorList[j] = electorList[j + 1];

					electorList[j + 1] = tempMember;
				}
			}
		}

		foreach(MemberInfo m in electorList)
		{
			Debug.Log ("叫啥？：" + m.name + "/" + "得票：" + m.voteNum);
		}

		//计算列个数
		if (electorList.Count % 2 == 0)
		{
			column = electorList.Count / 2;
		}
		else
		{
			column = electorList.Count / 2 + 1;
		}

		CreateItems (electorList);
	}

	void CreateItems (List<MemberInfo> m_electorList)
	{
		foreach (GameObject tempObj in electorObjList)
		{
			Destroy (tempObj);
		}

		electorObjList.Clear ();

		foreach (MemberInfo electorInfo in m_electorList)
		{
			GameObject elector = (GameObject)Instantiate (electorObj);
			
			elector.SetActive (true);
			
			elector.transform.parent = electionInfoGrid.transform;
			
			elector.transform.localPosition = Vector3.zero;
			
			elector.transform.localScale = Vector3.one;
			
			ElectorInfoItem electorItem = elector.GetComponent<ElectorInfoItem> ();
			electorItem.GetElectorInfo (electorInfo,e_allianceInfo);
		}
	}

	void Update ()
	{
		//列数少于4的时候，整体会回弹到最左边
		if (column <= 4)
		{
			electionInfoGrid.gameObject.GetComponent<ItemTopCol>().enabled = true;
		}
		else
		{
			electionInfoGrid.gameObject.GetComponent<ItemTopCol>().enabled = false;
		}
	}
	
	void FixedUpdate () {
		
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
		}else{
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

	//返回
	public void Back ()
	{
		Destroy (this.gameObject);
	}
	
	//关闭
	public void CloseAll ()
	{
		Destroy (GameObject.Find ("My_Union(Clone)"));
	}
}

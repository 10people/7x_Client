using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceElectionData : MonoBehaviour {

	public static AllianceElectionData electionData;
	
	public List<MemberInfo> memberInfoList = new List<MemberInfo>();//存放成员列表信息的list
	
	public GameObject electionManagerObj;
	
	public GameObject zheZhao;//遮罩
	
	private AllianceHaveResp m_allianceInfo;//返回的自己联盟信息
	
	private int m_allianceId;//自己联盟id
	
	public GameObject selectObj;//选中的对勾

	void Awake ()
	{
		electionData = this;
	}

	//获得自己联盟信息
	public void GetOwnLianMeng (AllianceHaveResp allianceInfo,LookMembersResp membersResp)
	{
		m_allianceInfo = allianceInfo;
		
		m_allianceId = allianceInfo.id;

		Debug.Log ("联盟id：" + m_allianceId );

		if (membersResp != null)
		{
			memberInfoList = membersResp.memberInfo;
			
			ElectionManager electionManager = electionManagerObj.GetComponent<ElectionManager>();
			electionManager.GetElectionInfo (membersResp,m_allianceInfo);
		}
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
		select.name = "SelectIt";
		
		select.transform.parent = parentObj.transform;
		
		select.transform.localPosition = pos;
		
		select.transform.localScale = Vector3.one;
	}

	//销毁item上的对勾
	public void DestroySelect ()
	{
		if (GameObject.Find ("SelectIt") != null)
		{
			Destroy (GameObject.Find ("SelectIt"));
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

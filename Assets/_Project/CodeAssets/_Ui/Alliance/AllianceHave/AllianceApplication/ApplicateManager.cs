using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ApplicateManager : MonoBehaviour {

	public UIGrid applicateGrid;

	public GameObject applicateItem;//入盟申请者item

	private List<GameObject> applicateItemList = new List<GameObject> ();//存放入盟申请者的list

	private List<ApplicantInfo> m_applicateList = new List<ApplicantInfo> ();//入盟申请信息list

	private AllianceHaveResp m_allianceInfo;//联盟信息

	public GameObject leftTurnBtn;//左滑动按钮
	public GameObject rightTurnBtn;//右滑动按钮
	
	public UIPanel scrollViewPanel;
	private int column;//列数
	
	public int offect;//偏移量

	public UILabel onApplyPerson;//没有申请者时显示的描述

	void Start ()
	{
//		Test (10);
	}

	//获得入盟申请信息
	public void GetApplicationInfo( List<ApplicantInfo> tempInfoList,AllianceHaveResp allianceInfo )
	{
		Debug.Log( "m_applicateList: " + m_applicateList.Count );

		m_allianceInfo = allianceInfo;

		if (tempInfoList.Count != 0)
		{
			onApplyPerson.text = "";

			m_applicateList = tempInfoList;

			CreateApplicateItems (tempInfoList);
		}

		else
		{
			ClearItems ();
			onApplyPerson.text = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NO_APPLY_DES);
		}
	}

	//创建入盟申请者list
	void CreateApplicateItems (List<ApplicantInfo> tempInfoList)
	{
		Debug.Log ("入盟申请个数：" + tempInfoList.Count);
		//计算列个数
		if (tempInfoList.Count % 2 == 0)
		{
			column = tempInfoList.Count / 2;
		}
		else
		{
			column = tempInfoList.Count / 2 + 1;
		}

		//按等级排序
		for (int i = 0;i < tempInfoList.Count - 1;i ++)
		{
			for (int j = 0;j < tempInfoList.Count - i - 1;j ++)
			{
				if (tempInfoList[i].level < tempInfoList[i + 1].level)
				{
					ApplicantInfo tempInfo = tempInfoList[i];

					tempInfoList[i] = tempInfoList[i + 1];

					tempInfoList[i + 1] = tempInfo;
				}
			}
		}

		Debug.Log ("itemList1：" + applicateItemList.Count);

		ClearItems ();//清除items

		//创建列表
		foreach (ApplicantInfo itemInfo in tempInfoList)
		{
			GameObject applicate = (GameObject)Instantiate (applicateItem);

			applicate.SetActive (true);
			applicate.name = "ApplicateItem";

			applicate.transform.parent = applicateGrid.gameObject.transform;

			applicate.transform.localPosition = Vector3.zero;

			applicate.transform.localScale = Vector3.one;

			ApplicateItem m_applicate = applicate.GetComponent<ApplicateItem> ();
			m_applicate.GetApplicateItemInfo (itemInfo,m_allianceInfo);

			applicateItemList.Add (applicate);
		}
		Debug.Log ("itemList3：" + applicateItemList.Count);
		applicateGrid.repositionNow = true;
	}

	//清除itemList
	public void ClearItems ()
	{
		foreach (GameObject item in applicateItemList)
		{
			Destroy (item);
		}
		applicateItemList.Clear ();
		Debug.Log ("itemList2：" + applicateItemList.Count);
	}

	void Test (int num)
	{
		if (num % 2 == 0)
		{
			column = num / 2;
		}
		else
		{
			column = num / 2 + 1;
		}
		for (int i = 0;i < num;i ++)
		{
			GameObject applicate = (GameObject)Instantiate (applicateItem);
			
			applicate.SetActive (true);
			applicate.name = "ApplicateItem";
			
			applicate.transform.parent = applicateGrid.gameObject.transform;
			
			applicate.transform.localPosition = Vector3.zero;
			
			applicate.transform.localScale = Vector3.one;
		}
	}

	void Update ()
	{
		//列数少于4的时候，整体会回弹到最左边
		if (column <= 4)
		{
			applicateGrid.gameObject.GetComponent<ItemTopCol>().enabled = true;
		}
		else
		{
			applicateGrid.gameObject.GetComponent<ItemTopCol>().enabled = false;
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
}

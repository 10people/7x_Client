using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TurnBtns : MonoBehaviour {

	public GameObject leftTurnBtn;

	public GameObject rightTurnBtn;

	public List<GameObject> tbObjList = new List<GameObject> ();

	public UIPanel scrollViewPanel;

	public int offect;//滑动偏移量
	public int disTance;//间隔

	private int column;//成员列表的列数

	//获得探宝信息
	public void GetTanBaoInfo (ExploreInfoResp tempInfo)
	{
		leftTurnBtn.SetActive (true);
		rightTurnBtn.SetActive (true);
		column = 5;
	}

	void Update () 
	{
		scrollViewPanel.clipOffset = new Vector2(-scrollViewPanel.gameObject.transform.localPosition.x, 0);

		int Move_x = column*disTance -offect+(int)scrollViewPanel.cachedGameObject.transform.localPosition.x;

		if (column <= 3)
		{
			leftTurnBtn.SetActive (false);
			rightTurnBtn.SetActive (false);
			return;
		}

		if (Move_x <= 5)
		{
			rightTurnBtn.SetActive (false);
			leftTurnBtn.SetActive (true);
		}

		else if (scrollViewPanel.cachedGameObject.transform.localPosition.x >= -10)
		{
			rightTurnBtn.SetActive (true);
			leftTurnBtn.SetActive (false);
		}

		else
		{
			rightTurnBtn.SetActive (true);
			leftTurnBtn.SetActive (true);
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
			
			moveX = j*disTance -offect+(int)scrollViewPanel.cachedGameObject.transform.localPosition.x;
			
			if(moveX > offect)
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x - offect, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
//				Debug.Log("moveX" +moveX);
//				Debug.Log("scrollor.cacx" +scrollViewPanel.cachedGameObject.transform.localPosition.x);
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x - moveX, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
		else
		{
			moveX = (int)(-scrollViewPanel.cachedGameObject.transform.localPosition.x);
			if(moveX > offect)
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x + offect, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				SpringPanel.Begin (scrollViewPanel.cachedGameObject,
				                   new Vector3(scrollViewPanel.cachedGameObject.transform.localPosition.x + moveX, 0f, 0f), 8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
	}
}

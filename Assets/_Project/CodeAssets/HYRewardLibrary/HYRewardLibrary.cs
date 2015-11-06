using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HYRewardLibrary : MonoBehaviour {

	public static HYRewardLibrary hyReward;

	public ReqRewardStoreResp m_rewardResp;//荒野奖励库返回信息
	public AllianceHaveResp m_allianceResp;//联盟返回信息

	private List<GameObject> rewardItemList = new List<GameObject> ();

	public GameObject rewardGridObj;//奖励gameObj父对象
	public GameObject rewardItemObj;//奖励gameObj

	public GameObject desLabel;//首次进入荒野奖励库的描述

	public GameObject btnsObj;
	public GameObject divideBtns;
	public GameObject applyBtns;
	public GameObject xiangQingBtn;//详情按钮

	public GameObject applyBtn;//申请奖励按钮
	public GameObject cancelBtn;//取消申请奖励按钮
	public GameObject divideBtn;//分配奖励按钮

	public GameObject leftObj;
	public UIPanel rightPanel;
	private float alphaNum = 0;//rightPanel初始alpha值
	
	private float time = 0.5f;//alpha时间
	
	public GameObject toggleBoxObj;//点选框

	public GameObject applayerBox;//玩家点选框

	[HideInInspector]public bool isChangerReward;//是否对奖励物品进行过操作

	void Awake ()
	{
		hyReward = this;
	}

	void Start ()
	{
		isChangerReward = false;

		m_rewardResp = HyRewardLibraryData.hyRewardData.rewardResp;
		m_allianceResp = HyRewardLibraryData.hyRewardData.allianceResp;

		InItRewardLibrary ();
		CheckLastReward ();
	}

	//检测是否有已申请物品
	void CheckLastReward ()
	{
		bool flag = true;
		for (int i = 0;i < m_rewardResp.itemInfo.Count && flag;i ++)
		{
			if (m_rewardResp.itemInfo[i].applyerInfo.Count != 0)
			{
				for (int j = 0;j < m_rewardResp.itemInfo[i].applyerInfo.Count && flag;j ++)
				{
					if (JunZhuData.Instance ().m_junzhuInfo.id == m_rewardResp.itemInfo[i].applyerInfo[j].junzhuId)
					{
						HuangYeAwardTemplete hyAwardTemp = HuangYeAwardTemplete.getHuangYeAwardTemplateBySiteId (m_rewardResp.itemInfo[i].site);
						int itemId = hyAwardTemp.itemId;

						HYRewardBtnsCol.rewardBtnsCol.lastRewardName = NameIdTemplate.GetName_By_NameId (itemId);
						
						flag = false;
						
						break;
					}
				}
			}
			
			else
			{
				HYRewardBtnsCol.rewardBtnsCol.lastRewardName = "";
			}
		}
	}

	//初始化荒野奖励库
	public void InItRewardLibrary ()
	{
		if (m_allianceResp.identity == 0 || m_allianceResp.identity == 1)
		{
			divideBtns.SetActive (false);
			
			applyBtns.transform.localPosition = new Vector3(200,0,0);
			xiangQingBtn.transform.localPosition = new Vector3(-200,0,0);
		}
		
		else if (m_allianceResp.identity == 2)
		{
			divideBtns.SetActive (true);
			
			applyBtns.transform.localPosition = new Vector3(290,0,0);
			xiangQingBtn.transform.localPosition = Vector3.zero;
		}
		
		foreach (GameObject item in rewardItemList)
		{
			Destroy (item);
		}
		rewardItemList.Clear ();
		
		for (int i = 0;i < m_rewardResp.itemInfo.Count;i ++)
		{
			GameObject rewardItem = (GameObject)Instantiate (rewardItemObj);
			
			rewardItem.SetActive (true);
			rewardItem.name = "RewardItem" + (i + 1);
			
			rewardItem.transform.parent = rewardGridObj.transform;
			
			rewardItem.transform.localPosition = Vector3.zero;
			
			rewardItem.transform.localScale = rewardItemObj.transform.localScale;
			
			rewardItemList.Add (rewardItem);
			
			HyRewardItemInfo hyReward = rewardItem.GetComponent<HyRewardItemInfo> ();
			hyReward.GetRewardItemInfo (m_rewardResp.itemInfo[i],false);
		}
		rewardGridObj.GetComponent<UIGrid> ().repositionNow = true;
		
		ItemTopCol itemTop = rewardGridObj.GetComponent<ItemTopCol> ();
		if (m_rewardResp.itemInfo.Count < 20)
		{
			itemTop.enabled = true;
		}
		else
		{
			itemTop.enabled = false;
		}
	}

	//申请成功后刷新奖励列表
	public void ApplySuccessRefresh (ApplyRewardResp applyResp)
	{
		if (applyResp.preSite != null)
		{
			for (int i = 0;i < rewardItemList.Count;i ++)
			{
				if (applyResp.preSite == m_rewardResp.itemInfo[i].site)
				{
					//取消上一个物品的申请状态，将申请人信息删除
					for (int j = 0;j < m_rewardResp.itemInfo[i].applyerInfo.Count;j ++)
					{
						if (applyResp.applyerInfo.junzhuId == m_rewardResp.itemInfo[i].applyerInfo[j].junzhuId)
						{
							m_rewardResp.itemInfo[i].applyerInfo.Remove (m_rewardResp.itemInfo[i].applyerInfo[j]);
							
							break;
						}
					}
					
					rewardItemList[i].GetComponent<HyRewardItemInfo> ().GetRewardItemInfo (m_rewardResp.itemInfo[i],false);

					isChangerReward = true;

					break;
				}
			}
		}
		
		if (applyResp.curSite != null)
		{
			for (int i = 0;i < rewardItemList.Count;i ++)
			{
				if (applyResp.curSite == m_rewardResp.itemInfo[i].site)
				{
					//加入申请人信息applyerInfo
					m_rewardResp.itemInfo[i].applyerInfo.Add (applyResp.applyerInfo);
					
					rewardItemList[i].GetComponent<HyRewardItemInfo> ().GetRewardItemInfo (m_rewardResp.itemInfo[i],true);

					CheckLastReward ();

					isChangerReward = true;

					break;
				}
			}
		}
	}
	
	//取消申请成功后刷新奖励列表
	public void CancelApplySuccessRefresh (CancelApplyRewardResp cancelApplyResp)
	{
		for (int i = 0;i < rewardItemList.Count;i ++)
		{
			if (cancelApplyResp.site != null)
			{
				RewardItemInfo rewardInfo = m_rewardResp.itemInfo[i];
				
				if (cancelApplyResp.site == rewardInfo.site)
				{
					for (int j = 0;j < rewardInfo.applyerInfo.Count;j ++)
					{
						if (cancelApplyResp.junzhuId == rewardInfo.applyerInfo[j].junzhuId)
						{
							rewardInfo.applyerInfo.Remove (rewardInfo.applyerInfo[j]);
							
							break;
						}
					}
					
					rewardItemList[i].GetComponent<HyRewardItemInfo> ().GetRewardItemInfo (rewardInfo,true);
					
					CheckLastReward ();

					isChangerReward = true;
					
					break;
				}
			}
		}
	}

	//分配奖励后刷新奖励列表
	public void DivideSuccessRefresh (GiveRewardResp tempGiveResp)
	{
		for (int i = 0;i < rewardItemList.Count;i ++)
		{
//			Debug.Log ("startI:" + i);
			if (tempGiveResp.site == m_rewardResp.itemInfo[i].site)
			{
				m_rewardResp.itemInfo[i].nums -= 1;
				
				for (int j = 0;j < m_rewardResp.itemInfo[i].applyerInfo.Count;j ++)
				{
					if (tempGiveResp.junzhuId == m_rewardResp.itemInfo[i].applyerInfo[j].junzhuId)
					{
						m_rewardResp.itemInfo[i].applyerInfo.Remove (m_rewardResp.itemInfo[i].applyerInfo[j]);
						
						break;
					}
				}
				
				rewardItemList[i].GetComponent<HyRewardItemInfo> ().GetRewardItemInfo (m_rewardResp.itemInfo[i],true);

				CheckLastReward ();

				isChangerReward = true;

//				Debug.Log ("break" + i);
				break;
			}
		}
	}

	//LeftObj move
	public void RewardLibraryAnim (bool isSelect)
	{
		Hashtable leftTrans = new Hashtable ();
		
		leftTrans.Add ("time", time);
		leftTrans.Add ("easetype",iTween.EaseType.easeOutQuart);
		if (!isSelect)
		{
			leftTrans.Add ("position",new Vector3(0,10,0));
			alphaNum = 0;
		}
		else
		{
			leftTrans.Add ("position",new Vector3(-180,10,0));
			alphaNum = 1;
		}
		leftTrans.Add ("islocal",true);
		
		iTween.MoveTo (leftObj,leftTrans);
	}
	
	public void ChangeState (int state)
	{
		switch (state)//1-applyBtn 2-cancelApplyBtn
		{
		case 1:

			cancelBtn.SetActive (false);
			applyBtn.SetActive (true);
			
			break;
			
		case 2:
			
			applyBtn.SetActive (false);
			cancelBtn.SetActive (true);
			
			break;
		}
	}
	
	void Update ()
	{
		if (alphaNum != rightPanel.alpha)
		{
			if (rightPanel.alpha > alphaNum)
			{
				rightPanel.alpha -= 0.1f;
				if (rightPanel.alpha < alphaNum)
				{
					rightPanel.alpha = alphaNum;
				}
			}
			else
			{
				rightPanel.alpha += 0.1f;
				if (rightPanel.alpha > alphaNum)
				{
					rightPanel.alpha = alphaNum;
				}
			}
		}

		if (m_allianceResp.identity == 2)
		{
			if (GameObject.Find ("ApplayerBox") != null && GameObject.Find ("ToggleBox") != null)
			{
				divideBtn.SetActive (true);
			}
			
			else
			{
				divideBtn.SetActive (false);
			}
		}

		if (GameObject.Find ("ToggleBox") != null)
		{
			btnsObj.SetActive (true);
			desLabel.SetActive (false);
		}
	}

	//克隆选择框
	public void CreateToggleBox (GameObject parentObj)
	{
		if (GameObject.Find ("ToggleBox") != null)
		{
			Destroy (GameObject.Find ("ToggleBox"));
		}
		
		GameObject toggleBox = (GameObject)Instantiate (toggleBoxObj);
		
		toggleBox.SetActive (true);
		toggleBox.name = "ToggleBox";
		
		toggleBox.transform.parent = parentObj.transform;
		toggleBox.transform.localPosition = Vector3.zero;
		toggleBox.transform.localScale = Vector3.one;
	}

	//克隆申请人选择框
	public void CreateApplayerBox (GameObject parentObj)
	{
		if (GameObject.Find ("ApplayerBox") != null)
		{
			Destroy (GameObject.Find ("ApplayerBox"));
		}
		
		GameObject playerBox = (GameObject)Instantiate (applayerBox);
		
		playerBox.SetActive (true);
		playerBox.name = "ApplayerBox";
		
		playerBox.transform.parent = parentObj.transform;
		playerBox.transform.localPosition = Vector3.zero;
		playerBox.transform.localScale = Vector3.one;
	}

	public void DestroyRoot ()
	{
		if (isChangerReward)
		{
			HyRewardLibraryData.hyRewardData.HYRewardLibraryReq ();
		}
		
		Destroy (this.gameObject);
	}
}

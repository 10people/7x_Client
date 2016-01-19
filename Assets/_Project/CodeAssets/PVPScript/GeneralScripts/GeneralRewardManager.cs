﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralRewardManager : MonoBehaviour, IUIRootAutoActivator {

	private static GeneralRewardManager m_instance = null;

	public static GeneralRewardManager Instance ()
	{
		//if( m_instance == null )
		//{
		//	string t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.UI_POP_REWARD_ROOT );
			
		//	Global.ResourcesDotLoad( t_ui_path, ResourceLoadCallback );
		//}
		
		return m_instance;
	}

	private static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );
		
		if( t_gb == null )
		{
			Debug.LogError( "Instantiate to null." );
			
			return;
		}
		
		DontDestroyOnLoad( t_gb );
	}

	void Awake()
	{
		m_instance = this;

		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
	}

	void OnDestroy(){
		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}

		m_instance = null;
	}

	public GameObject rewardItemObj;
	private List<RewardData> rewardDataList = new List<RewardData> ();
	private List<GameObject> rewardItemList = new List<GameObject> ();//奖励物品

	public GameObject specialItemObj;
	private List<RewardData> specialRewardList = new List<RewardData> ();//特殊物品展示
	private List<GameObject> specialItemList = new List<GameObject> ();
	
	private int reward_index = 0;//下个奖励物品下标
	public int Reward_Index
	{
		set{reward_index = value;}
		get{return reward_index;}
	}

	private int specialReward_index = 0;//下个特殊物品下标
	public int SpecialReward_Index
	{
		set{specialReward_index = value;}
		get{return specialReward_index;}
	}

	#region GeneralReward
	/// <summary>
	/// Creates one reward.
	/// </summary>
	/// <param name="tempData">Temp data.</param>
	public void CreateReward (RewardData tempData)
	{
		rewardDataList.Add (tempData);
		CheckReward ();
	}

	/// <summary>
	/// Creates the rewardList
	/// </summary>
	/// <param name="tempDataList">Temp data list.</param>
	public void CreateReward (List<RewardData> tempDataList)
	{
		for (int i = 0;i < tempDataList.Count;i ++)
		{
			rewardDataList.Add (tempDataList[i]);
		}

		CheckReward ();
	}

	/// <summary>
	/// Checks the reward.
	/// </summary>
	public void CheckReward ()
	{
		if (reward_index < rewardDataList.Count)
		{
			GameObject rewardObj = GameObject.Find ("RewardItem" + reward_index);
			if(rewardObj == null)
			{
				rewardObj = GameObject.Instantiate ( rewardItemObj );
				rewardObj.name = "RewardItem" + reward_index;

				rewardObj.SetActive (true);
				rewardObj.transform.parent = rewardItemObj.transform.parent;
				rewardObj.transform.localPosition = new Vector3 (0,-80,0);
				rewardObj.transform.localScale = Vector3.one * 0.5f;
				rewardItemList.Add (rewardObj);

				GeneralReward reward = rewardObj.GetComponent<GeneralReward> ();
				reward.InItRewardItem (rewardDataList[reward_index]);

//				Debug.Log ("reward_index:" + reward_index + "|| rewardItemList:" + rewardItemList.Count + "|| rewardDataList:" + rewardDataList.Count);
			}
		}
	}

	/// <summary>
	/// Refreshs the item list.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void RefreshItemList (GameObject obj)
	{
		for (int i = 0;i < rewardItemList.Count;i ++)
		{
			if (rewardItemList[i] == obj)
			{
				Destroy (rewardItemList[i]);
				rewardItemList.RemoveAt (i);
			}
		}
		if (rewardItemList.Count == 0 && specialItemList.Count == 0)
		{
//			Debug.Log ("ClientMain.closePopUp();");

			ClientMain.closePopUp();
		}
	}
	#endregion

	#region SpecialReward
	/// <summary>
	/// Gets the special reward.
	/// </summary>
	/// <param name="tempData">Temp data.</param>
	public void CreateSpecialReward (RewardData tempData)
	{
		specialRewardList.Add (tempData);

		CheckSpecialReward ();
	}
	/// <summary>
	/// Gets the special reward.
	/// </summary>
	/// <param name="tempDataList">Temp data list.</param>
	public void CreateSpecialReward (List<RewardData> tempDataList)
	{
		foreach (RewardData reward in tempDataList)
		{
			specialRewardList.Add (reward);
		}

		CheckSpecialReward ();
	}

	/// <summary>
	/// Checks the special reward.
	/// </summary>
	public void CheckSpecialReward ()
	{
//		Debug.Log ("SpecialReward_Index:" + SpecialReward_Index);
		if (SpecialReward_Index < specialRewardList.Count)
		{
			GameObject rewardObj = GameObject.Find ("SpecialItem" + SpecialReward_Index);

			if(rewardObj == null)
			{
				rewardObj = GameObject.Instantiate ( specialItemObj );
				rewardObj.name = "SpecialItem" + SpecialReward_Index;
				
				rewardObj.SetActive (true);
				rewardObj.transform.parent = specialItemObj.transform.parent;
				rewardObj.transform.localPosition = Vector3.zero;
				rewardObj.transform.localScale = Vector3.one;
				specialItemList.Add (rewardObj);
				GeneralSpecialReward specialReward = rewardObj.GetComponent<GeneralSpecialReward> ();
				specialReward.InItSpecialReward (specialRewardList[SpecialReward_Index]);
			}

			{
				ShowRewardPanel ();
			}
		}
	}

	/// <summary>
	/// Refreshs the special item list.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void RefreshSpecialItemList (GameObject obj)
	{
//		Debug.Log ("obj:" + obj);
		for (int i = 0;i < specialItemList.Count;i ++)
		{
			if (specialItemList[i] == obj)
			{
//				Debug.Log ("specialItemList[i]:" + specialItemList[i]);
				Destroy (specialItemList[i]);
				specialItemList.RemoveAt (i);
			}
		}
		if (rewardItemList.Count == 0 && specialItemList.Count == 0)
		{
			//			Debug.Log ("ClientMain.closePopUp();");
			
			ClientMain.closePopUp();
		}
	}
	#endregion

	/// <summary>
	/// Clears the reward data.
	/// </summary>
	public void ClearRewardData ()
	{
		foreach (GameObject obj in rewardItemList)
		{
			Destroy (obj);
		}
		Reward_Index = 0;
		rewardItemList.Clear ();
		rewardDataList.Clear ();

		foreach (GameObject obj in specialItemList)
		{
			Destroy (obj);
		}
		SpecialReward_Index = 0;
		specialItemList.Clear ();
		specialRewardList.Clear ();
	}

	/// <summary>
	/// 屏蔽其它ui
	/// </summary>
	void ShowRewardPanel ()
	{
		UI2DTool.Instance.AddTopUI( gameObject );

		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI ();
		}
	}

	/// <summary>
	/// Determines whether this instance is exit reward.
	/// </summary>
	/// <returns><c>true</c> if this instance is exit reward; otherwise, <c>false</c>.</returns>
	public bool IsExitReward ()
	{
		return rewardItemList.Count > 0 || specialItemList.Count > 0 ? true : false;
	}
	
	void Update (){

	}

	#region IUIRootAutoActivator
	
	public bool IsNGUIVisible()
	{
		return IsExitReward();
	}
	#endregion
}

/// <summary>
/// Reward data.
/// </summary>
public class RewardData {
	
	public int itemId;//物品id
	public int itemCount;//物品数量

	public int miBaoStar;//秘宝星级

	public delegate void MiBaoClick ();
	public MiBaoClick miBaoClick;
	
	public float moveTime1 = 0.2f;
	public float stopTime = 0.2f;
	public float moveTime2 = 0.1f;
	
	public iTween.EaseType itweenType1 = iTween.EaseType.easeOutBack;
	public iTween.EaseType itweenType2 = iTween.EaseType.linear;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="RewardData"/> class.
	/// Set RewardData's itemId and itemCount
	/// </summary>
	/// <param name="tempItemId">Temp item identifier.</param>
	/// <param name="tempItemCount">Temp item count.</param>
	public RewardData (int tempItemId,int tempItemCount,int tempStar = 0,MiBaoClick tempMiBaoClick = null)
	{
		itemId = tempItemId;
		itemCount = tempItemCount;
		miBaoStar = tempStar;
		if (tempMiBaoClick != null)
		{
			miBaoClick = tempMiBaoClick;
		}
	}
	
	/// <summary>
	/// Sets the reward move time.
	/// </summary>
	/// <param name="tempTime1">first move time</param>
	/// <param name="tempStopTime">stop time.</param>
	/// <param name="tempTime2">second move time</param>
	public void SetRewardMoveTime (float tempTime1,float tempStopTime,float tempTime2)
	{
		moveTime1 = tempTime1;
		stopTime = tempStopTime;
		moveTime2 = tempTime2;
	}
	
	/// <summary>
	/// Sets the type of the reward itween.
	/// </summary>
	/// <param name="tempType1">firse itween.easetype</param>
	/// <param name="tempType2">second itween.easetype</param>
	public void SetRewardItweenType (iTween.EaseType tempType1,iTween.EaseType tempType2)
	{
		itweenType1 = tempType1;
		itweenType2 = tempType2;
	}
}

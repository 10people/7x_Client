using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralRewardManager : MonoBehaviour, IUIRootAutoActivator {

	private static GeneralRewardManager m_instance = null;

	public static GeneralRewardManager Instance ()
	{
		if( m_instance == null )
		{
			string t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.UI_POP_REWARD_ROOT );
			
			Global.ResourcesDotLoad( t_ui_path, ResourceLoadCallback );
		}
		
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

	public GameObject rewardItemObj;
	private List<RewardData> rewardDataList = new List<RewardData> ();
	private List<GameObject> rewardItemList = new List<GameObject> ();

	private int reward_index = 0;//下个奖励物品下标
	public int Reward_Index
	{
		set{reward_index = value;}
		get{return reward_index;}
	}

	private bool isExitReward = false;

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
	}

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
		if (rewardItemList.Count == 0)
		{
//			Debug.Log ("ClientMain.closePopUp();");

			ClientMain.closePopUp();
		}
	}

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
	}

	#region IUIRootAutoActivator
	
	public bool IsNGUIVisible(){
		return IsExitReward();
	}
	
	#endregion

	void Update ()
	{
		isExitReward = rewardItemList.Count > 0 ? true : false;
	}

	/// <summary>
	/// Determines whether this instance is exit reward.
	/// </summary>
	/// <returns><c>true</c> if this instance is exit reward; otherwise, <c>false</c>.</returns>
	public bool IsExitReward ()
	{
		return isExitReward;
	}
}

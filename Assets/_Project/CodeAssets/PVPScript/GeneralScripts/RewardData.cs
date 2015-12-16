using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardData {

	public int itemId;//物品id
	public int itemCount;//物品数量
	public int xingJi;//秘宝星级

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
	public RewardData (int tempItemId,int tempItemCount,int tempXingJi = -1)
	{
		itemId = tempItemId;
		itemCount = tempItemCount;
		xingJi = tempXingJi;
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

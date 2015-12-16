using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWinFlag : MonoBehaviour 
{
	public enum EndType
	{
		Kill_All,
		Kill_Boss,
		Reach_Destination,
		Reach_Time,
		Kill_Gear,
		Kill_Soldier,
		Kill_Hero,
		PROTECT,
	}

	public int endId;

	public bool isWin = true;

	public EndType endType;

	public int killNum;

	public GameObject destinationObject;
	
	public float destinationRadius;

	public int activeNum;

	public List<int> activeList = new List<int> ();

	public bool showOnUI = false;

	public int protectNum;

	public List<int> protectList = new List<int>();

}

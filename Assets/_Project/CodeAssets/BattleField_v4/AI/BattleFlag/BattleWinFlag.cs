using UnityEngine;
using System.Collections;

public class BattleWinFlag : MonoBehaviour 
{
	public enum WinType
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

	public int winId;

	public WinType winType;

	public int killNum;

	public GameObject destinationObject;
	
	public float destinationRadius;

	public bool showOnUI = false;

	public int protectNodeId;

}

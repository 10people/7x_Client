using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWinFlag : MonoBehaviour 
{
	public enum EndType
	{
		Kill_All,			//全歼
		Kill_Boss,			//击杀BOSS
		Reach_Destination,	//到达某地
		Reach_Time,			//到达某时
		Kill_Gear,			//击杀障碍
		Kill_Soldier,		//击杀小兵
		Kill_Hero,			//击杀武将
		PROTECT,			//保护
		Kill_Wave,			//歼敌波数
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

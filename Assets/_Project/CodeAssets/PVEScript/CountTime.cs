using UnityEngine;
using System.Collections;
using System;
using System.Linq;
public class CountTime : MonoBehaviour {
	private int shouldToTime;
//	bool isoverTime;
    int allTime;
	UILabel counttime;
	public GameObject The_sup;
	int  isJingyingGuanQia;
	int guanqiaId;
	void Start () 
	{


	    counttime  = GetComponent<UILabel>();
		guanqiaId = Pve_Level_Info.CurLev;
		isJingyingGuanQia = MapData.mapinstance.Lv [guanqiaId].type;


		if(isJingyingGuanQia == 1)
		{
			this.gameObject.transform.localPosition = new Vector3(-100,45,0);
		}
		else{
			this.gameObject.transform.localPosition = new Vector3(100,45,0);
		}

		InvokeRepeating("StarcountTime",0,1);
	}
	

	void Update () 
	{
	
		if(allTime < 0)
		{
//			isoverTime = true;

			Destroy(The_sup);

			//The_sup.SetActive(false);
		}
	}
	
	void StarcountTime()
	{
		allTime = savaTime.EndTime;
		if(allTime > 60)
		{
			if(((int)(allTime/60)) >= 10&&((int)(allTime%60)) >= 10)
			{
				counttime.text = "00:" +((int)(allTime/60)).ToString()+ ":" +  ((int)(allTime%60)).ToString ();
			}
			else if(((int)(allTime/60)) < 10&&((int)(allTime%60)) >= 10)
			{
				counttime.text = "00:0" +((int)(allTime/60)).ToString()+ ":" +  ((int)(allTime%60)).ToString ();
			}
			else if(((int)(allTime/60)) >= 10&&((int)(allTime%60)) < 10)
			{
				counttime.text = "00:" +((int)(allTime/60)).ToString()+ ":0" +  ((int)(allTime%60)).ToString ();
			}
			else{
				counttime.text = "00:0" +((int)(allTime/60)).ToString()+ ":0" +  ((int)(allTime%60)).ToString ();
			}
		}
		else{

			if(allTime >= 10)
			{
				counttime.text = "00:" + "00:" +  allTime.ToString ();
			}
			else{
				counttime.text = "00:" + "00:0" +  allTime.ToString ();
			}

		}

		savaTime.EndTime -= 1;
	}
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimeLabelHelper : Singleton<TimeLabelHelper>
{
	struct TimeLabelData
	{
		public UILabel m_LabelTime;
		public string m_sEndTimeString;
		public float m_fBTime;
		public int m_iSecondTime;
	}
	private List<TimeLabelData> TimeLabelList = new List<TimeLabelData>();
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i < TimeLabelList.Count; i ++)
		{
			if(TimeLabelList[i].m_LabelTime == null)
			{
				TimeLabelList.RemoveAt(i);
				i --;
			}
			else
			{
				int tempCountdown = TimeLabelList[i].m_iSecondTime - (int)(Time.realtimeSinceStartup - TimeLabelList[i].m_fBTime);
				if(tempCountdown <= 0)
				{
					TimeLabelList[i].m_LabelTime.text = ColorTool.Color_Green_00ff00 + "领取[-]";
				}
				else
				{
					TimeLabelList[i].m_LabelTime.text = ColorTool.Color_Green_00ff00 + TimeHelper.SecondToClockTime(tempCountdown) + "[-]";
				}
			}
		}
	}

	public void setTimeLabel(UILabel label, string endTimeString, int second)
	{
		label.gameObject.SetActive(true);

		for(int i = 0 ; i < TimeLabelList.Count; i ++)
		{
			if(TimeLabelList[i].m_LabelTime == null)
			{
				TimeLabelList.RemoveAt(i);
				i --;
			}
			else if(TimeLabelList[i].m_LabelTime == label)
			{
				TimeLabelData tempTimeLabelData1 = TimeLabelList[i];
				tempTimeLabelData1.m_fBTime = Time.realtimeSinceStartup;
				tempTimeLabelData1.m_iSecondTime = second;
				TimeLabelList[i] = tempTimeLabelData1;
				return;
			}
		}
		TimeLabelData tempTimeLabelData = new TimeLabelData();

		tempTimeLabelData.m_LabelTime = label;
		tempTimeLabelData.m_sEndTimeString = endTimeString;
		tempTimeLabelData.m_fBTime = Time.realtimeSinceStartup;
		tempTimeLabelData.m_iSecondTime = second;
		TimeLabelList.Add(tempTimeLabelData);
	}
}

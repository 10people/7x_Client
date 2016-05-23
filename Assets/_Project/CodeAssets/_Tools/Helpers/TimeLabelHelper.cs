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
		public string m_sAddString;
		public EndTimeDelegate m_FunctionEndTime;
		public bool m_isFunction;
	}
	public delegate void EndTimeDelegate();


	private List<TimeLabelData> TimeLabelList = new List<TimeLabelData>();
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
//		Debug.Log(TimeLabelList.Count);
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
//					Debug.Log("xiaoyu");
//					Debug.Log(TimeLabelList[i].m_isFunction);
					if(!TimeLabelList[i].m_isFunction)
					{
						if(TimeLabelList[i].m_FunctionEndTime != null)
						{
							TimeLabelList[i].m_FunctionEndTime();
							TimeLabelData tempTimeLabel = TimeLabelList[i];
							tempTimeLabel.m_isFunction = true;
							TimeLabelList[i] = tempTimeLabel;
						}
					}
					TimeLabelList[i].m_LabelTime.text = TimeLabelList[i].m_sEndTimeString;
				}
				else
				{
					if(TimeLabelList[i].m_LabelTime.color == Color.white)
					{
						TimeLabelList[i].m_LabelTime.text = ColorTool.Color_Green_00ff00 + TimeHelper.SecondToClockTime(tempCountdown) + TimeLabelList[i].m_sAddString  + "[-]";
					}
					else
					{
						TimeLabelList[i].m_LabelTime.text = TimeHelper.SecondToClockTime(tempCountdown) + TimeLabelList[i].m_sAddString;
					}
				}
			}
		}
	}

	public void setTimeLabel(UILabel label, string endTimeString, int second, EndTimeDelegate endTime =  null, string addString = "")
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

				if(second <= 0)
				{
					if(tempTimeLabelData1.m_FunctionEndTime != null)
					{
						tempTimeLabelData1.m_FunctionEndTime();
						tempTimeLabelData1.m_isFunction = true;
						//				TimeLabelList[i].m_isFunction = true;
					}
				}
				return;
			}
		}
		TimeLabelData tempTimeLabelData = new TimeLabelData();

		tempTimeLabelData.m_LabelTime = label;
		tempTimeLabelData.m_sEndTimeString = endTimeString;
		tempTimeLabelData.m_fBTime = Time.realtimeSinceStartup;
		tempTimeLabelData.m_iSecondTime = second;
		tempTimeLabelData.m_FunctionEndTime = endTime;
//		if(tempTimeLabelData.m_FunctionEndTime != null)
//		{
//			tempTimeLabelData.m_FunctionEndTime();
//		}
		tempTimeLabelData.m_sAddString = addString;
		if(second <= 0)
		{
			if(tempTimeLabelData.m_FunctionEndTime != null)
			{
				tempTimeLabelData.m_FunctionEndTime();
				tempTimeLabelData.m_isFunction = true;
//				TimeLabelList[i].m_isFunction = true;
			}
		}
		TimeLabelList.Add(tempTimeLabelData);
	}
}

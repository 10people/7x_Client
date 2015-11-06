using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDRecordItem : MonoBehaviour {

	private LveBattleItem ldBattleInfo;

	public UISprite headIcon;

	public UISprite nation;

	public UILabel level;

	public UILabel nameLabel;

	public UILabel allianceName;

	public UILabel zhanLiLabel;

	public UILabel attackLabel;

	public UILabel timeLabel;

	public UILabel getAwardLabel;

	public UILabel gongJin;

	public UILabel btnLabel;

	public GameObject mlabel;

	public void GetRecordInfo (LveBattleItem tempInfo)
	{
		ldBattleInfo = tempInfo;
//		Debug.Log ("tempinfo:" + tempInfo.anotherName + "||" + tempInfo.gongJiwin + "||" + tempInfo.time);
		headIcon.spriteName = "PlayerIcon" + tempInfo.anotherRoleId;
		nameLabel.text = tempInfo.anotherName;

		nation.spriteName = "nation_" + tempInfo.anotherGuoJiaId.ToString ();

		if (tempInfo.anotherMengName.Equals (""))
		{
			allianceName.text = "无联盟";
		}
		else
		{
			allianceName.text = "<" + tempInfo.anotherMengName + ">";
		}

		level.text = tempInfo.anotherLevel.ToString ();
		zhanLiLabel.text = tempInfo.anotherZhanli.ToString ();
		timeLabel.text = ShowTime (tempInfo.time);

		if (tempInfo.gongJiId == tempInfo.anotherId)//对方是攻击者
		{
			headIcon.parent.transform.localPosition = new Vector3(-370,0,0);
			attackLabel.transform.localPosition = new Vector3(-50,15,0);
			mlabel.transform.localPosition = new Vector3(70,0,0);
			
			switch (tempInfo.gongJiwin)
			{
			case 1:
				
				gongJin.parent.gameObject.SetActive (true);
				getAwardLabel.transform.localPosition = new Vector3(180,15,0);

				attackLabel.text = "成功掠夺";
				getAwardLabel.text = "[dc0600]掠夺了[-]";
				gongJin.text = tempInfo.lostXiaYi.ToString ();
				btnLabel.text = "复仇";
				
				break;
				
			case 2:
				
				gongJin.parent.gameObject.SetActive (false);
				getAwardLabel.transform.localPosition = new Vector3(180,0,0);

				attackLabel.text = "掠夺";
				getAwardLabel.text = "[00ff00]未成功[-]";
				btnLabel.text = "复仇";
				
				break;
				
			default:
				break;
			}
		}
		else if (tempInfo.gongJiId == JunZhuData.Instance ().m_junzhuInfo.id)//我是攻击者
		{
			headIcon.parent.transform.localPosition = new Vector3(-130,0,0);
			attackLabel.transform.localPosition = new Vector3(-265,15,0);
			mlabel.transform.localPosition = new Vector3(-375,0,0);
			
			switch (tempInfo.gongJiwin)
			{
			case 1://胜利
				
				gongJin.parent.gameObject.SetActive (true);
				getAwardLabel.transform.localPosition = new Vector3(180,15,0);

				attackLabel.text = "成功掠夺";
				getAwardLabel.text = "[dc0600]掠夺了[-]";
				gongJin.text = tempInfo.lostXiaYi.ToString ();
				btnLabel.text = "再战一局";
				
				break;
				
			case 2://失败
				
				gongJin.parent.gameObject.SetActive (false);
				getAwardLabel.transform.localPosition = new Vector3(180,0,0);

				attackLabel.text = "掠夺";
				getAwardLabel.text = "[00ff00]未成功[-]";
				btnLabel.text = "再战一局";
				
				break;
				
			default:
				break;
			}
		}
	}

	public void FightBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			LueDuoData.Instance.JunNationId = ldBattleInfo.anotherGuoJiaId;
			LueDuoData.Instance.LueDuoOpponentReq (ldBattleInfo.anotherId,LueDuoData.WhichOpponent.LUE_DUO);
		}
	}

	/// <summary>
	/// 显示对战时间
	/// </summary>
	/// <returns>The time.</returns>
	/// <param name="time">Time.</param>
	private string ShowTime (long time)
	{
		string timeStr = "很久以前";
		
		string timeStr1 = UTCToTimeString (time,"yyyy-MM-dd");
		string timeStr2 = UTCToTimeString (time,"HH:mm:ss");
//		Debug.Log ("timeStr1:" + timeStr1);
//		Debug.Log ("timeStr2:" + timeStr2);
//		Debug.Log ("NowTime:" + DateTime.Now.ToString ("yyyy-MM-dd"));
		
		List<int> dayList = GetDayList (timeStr1);
		List<int> nowDayList = GetDayList (DateTime.Now.ToString ("yyyy-MM-dd"));
		List<int> timeList = GetTimeList (timeStr2);
		
		if (dayList[0] == nowDayList[0])
		{
			if (dayList[1] == nowDayList[1])
			{
				switch (Mathf.Abs (dayList[2] - nowDayList[2]))
				{
				case 0:
					
					timeStr = "今天" + timeList[0] + "时" + timeList[1] + "分";
					
					break;
					
				case 1:
					
					timeStr = "昨天" + timeList[0] + "时" + timeList[1] + "分";
					
					break;
					
				case 2:
					
					timeStr = "前天" + timeList[0] + "时" + timeList[1] + "分";
					
					break;
					
				default:
					break;
				}
			}
		}
		
		return timeStr;
	}
	
	/// <summary>
	/// 转换时间格式
	/// </summary>
	/// <returns>The to time string.</returns>
	/// <param name="time">Time.</param>
	/// <param name="format">Format.</param>
	private string UTCToTimeString(long time, string format)
	{
		long lTime = time * 10000;
		
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		
		TimeSpan toNow = new TimeSpan(lTime);
		
		DateTime dtResult = dtStart.Add(toNow);
		
		// "yyyy-MM-dd HH:mm:ss"
		return dtResult.ToString(format);
	}
	
	/// <summary>
	/// 获得年月日
	/// </summary>
	/// <returns>The day list.</returns>
	/// <param name="tempStr">Temp string.</param>
	private List<int> GetDayList (string tempStr)
	{
		string[] dayStr = tempStr.Split ('-');
		
		List<int> tempDayList = new List<int> ();
		
		foreach (string s in dayStr)
		{
			//			Debug.Log ("s:" + s);
			tempDayList.Add (int.Parse (s));
		}
		
		return tempDayList;
	}
	
	/// <summary>
	/// 获得小时分钟和秒
	/// </summary>
	/// <returns>The time list.</returns>
	/// <param name="tempStr">Temp string.</param>
	private List<int> GetTimeList (string tempStr)
	{
		string[] timeStr = tempStr.Split (':');
		
		List<int> tempTimeList = new List<int> ();
		
		foreach (string h in timeStr)
		{
			//			Debug.Log ("h:" + h);
			tempTimeList.Add (int.Parse (h));
		}
		
		return tempTimeList;
	}

	/// <summary>
	/// 掠夺记录时间
	/// </summary>
	/// <returns>The time.</returns>
	/// <param name="time">Time.</param>
	private string RecordTime (int time)
	{
		string dateStr = "";

		int hour = time / 3600;
		
		int minute = (time / 60) % 60;

		if (hour >= 64)
		{
			dateStr = "三天以前";
		}
		else
		{
			string whichDay = "";
			if (hour >= 48)
			{
				whichDay = "前天";
			}
			else
			{
				if (hour >= 24)
				{
					whichDay = "昨天";
				}
				else
				{
					whichDay = "今天";
				}
			}

			dateStr = whichDay + hour + "时" + minute + "分";
		}

		return dateStr;
	}
}

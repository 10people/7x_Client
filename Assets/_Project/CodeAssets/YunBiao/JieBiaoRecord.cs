using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JieBiaoRecord : MonoBehaviour {

	/// <summary>
	/// 君主信息 或 马车信息
	/// </summary>
	public GameObject headInfoObj;
	public UISprite headIcon;//君主头像
	public UISprite horseIcon;//马车头像
	public UISprite e_country;//对方国家
	public UISprite pinZhi;//马车品质
	public UILabel level;//等级
	public UILabel junZhuName;//君主名字
	public UILabel allianceName;//联盟名字
	public UILabel zhanLi;//战力
	
	/// <summary>
	/// 攻击方式
	/// </summary>
	public UILabel attackLabel;
	/// <summary>
	/// 时间
	/// </summary>
	public UILabel time;
	
	/// <summary>
	/// 队伍
	/// </summary>
	public UILabel teamLabel;
	
	/// <summary>
	/// 打劫失败
	/// </summary>
	public UILabel fail;
	/// <summary>
	/// 打劫成功奖励
	/// </summary>
	public UILabel award;

	/// <summary>
	/// 获得对战记录信息
	/// </summary>
	public void GetRecordItemInfo (YBHistory tempInfo)
	{
		switch (tempInfo.result)
		{
		case 1://打劫失败

			teamLabel.transform.localPosition = new Vector3(-275f,0,0);
			teamLabel.text = "您的劫镖队伍";

			headInfoObj.transform.localPosition = new Vector3(85f,0,0);
			headIcon.spriteName = "";//不显示
			horseIcon.spriteName = "horseIcon" + tempInfo.type;
			pinZhi.spriteName = "pinzhi" + (tempInfo.type - 1);

			attackLabel.text = "攻击";

			fail.text = "未成功";
			award.gameObject.SetActive (false);

			break;

		case 2://击溃敌人

			teamLabel.transform.localPosition = new Vector3(170f,0,0);
			teamLabel.text = "您的运镖队伍";
			
			headInfoObj.transform.localPosition = new Vector3(-365f,0,0);
			headIcon.spriteName = "PlayerIcon" + tempInfo.type;
			horseIcon.spriteName = "";//不显示
			pinZhi.spriteName = "";//不显示
			
			attackLabel.text = "攻击";
			
			fail.text = "未成功";
			award.gameObject.SetActive (false);

			break;

		case 3://打劫成功

			teamLabel.transform.localPosition = new Vector3(-275f,0,0);
			teamLabel.text = "您的劫镖队伍";
			
			headInfoObj.transform.localPosition = new Vector3(85f,0,0);
			headIcon.spriteName = "";//不显示
			horseIcon.spriteName = "horseIcon" + tempInfo.type;
			pinZhi.spriteName = "pinzhi" + (tempInfo.type - 1);
			
			attackLabel.text = "成功击溃";
			
			fail.text = "";
			award.gameObject.SetActive (true);
			award.text = tempInfo.shouyi.ToString ();

			break;

		case 4://被打劫

			teamLabel.transform.localPosition = new Vector3(170f,0,0);
			teamLabel.text = "您的运镖队伍";
			
			headInfoObj.transform.localPosition = new Vector3(-365f,0,0);
			headIcon.spriteName = "PlayerIcon" + tempInfo.type;
			horseIcon.spriteName = "";//不显示
			pinZhi.spriteName = "";//不显示
			
			attackLabel.text = "成功击溃";
			
			fail.text = "";
			award.gameObject.SetActive (true);
			award.text = tempInfo.shouyi.ToString ();

			break;

		default:break;
		}

		e_country.spriteName = "nation_" + tempInfo.enemyGuojia;

		level.text = "Lv" + tempInfo.enemyLevel.ToString ();
		junZhuName.text = tempInfo.enemyName;

		if (tempInfo.enemylianMengName.Equals (""))
		{
			allianceName.text = "无联盟";
		}
		else
		{
			allianceName.text = "<" + tempInfo.enemylianMengName + ">";
		}

		zhanLi.text = tempInfo.enemyzhanLi.ToString ();

		time.text = ShowTime (tempInfo.time);
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
}

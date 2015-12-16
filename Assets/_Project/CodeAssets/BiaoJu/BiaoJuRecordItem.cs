using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BiaoJuRecordItem : MonoBehaviour {

	private YBHistory recordInfo;

	public UIAtlas playerAtlas;
	public UIAtlas biaoJuAtlas;

	public GameObject headInfoObj;
	public UISprite icon;
	public UISprite border;
	public UISprite nation;
	public UILabel levelLabel;
	public UILabel nameLabel;
	public UILabel zhanLiLabel;

	public UILabel attackLabel;
	public UILabel timeLabel;

	public UILabel teamLabel;
	public UILabel failLabel;
	public UILabel awardLabel;

	private enum JieBiaoType
	{
		JIEBIAO_SUCCESS = 3,
		JIEBIAO_FAIL = 1,
		YUNBIAO_SUCCESS = 2,
		YUNBIAO_FAIL = 4,
	}

	private readonly Dictionary<JieBiaoType,string[]> recordDic = new Dictionary<JieBiaoType, string[]> ()
	{
		{JieBiaoType.JIEBIAO_FAIL,new string[]{"-275,85",
				"您的劫镖队伍",
				"攻击",
				"未成功",
				"horseIcon"}},
		{JieBiaoType.YUNBIAO_SUCCESS,new string[]{"170,-365",
				"您的运镖队伍",
				"攻击",
				"未成功",
				"PlayerIcon"}},
		{JieBiaoType.JIEBIAO_SUCCESS,new string[]{"-275,85",
				"您的劫镖队伍",
				"成功击溃",
				"",
				"horseIcon"}},
		{JieBiaoType.YUNBIAO_FAIL,new string[]{"170,-365",
				"您的运镖队伍",
				"成功击溃",
				"",
				"PlayerIcon"}}
	};

	/// <summary>
	/// Ins it record item.
	/// </summary>
	/// <param name="tempRecordInfo">Temp record info.</param>
	public void InItRecordItem (YBHistory tempRecordInfo)
	{
		recordInfo = tempRecordInfo;

		JieBiaoType type = (JieBiaoType)Enum.ToObject (typeof (JieBiaoType),tempRecordInfo.result);

		string[] posStr = recordDic [type] [0].Split (',');
		teamLabel.transform.localPosition = new Vector3(float.Parse (posStr[0]),0,0);
		teamLabel.text = recordDic [type] [1];

		headInfoObj.transform.localPosition = new Vector3(float.Parse (posStr[1]),0,0);

		icon.atlas = type == JieBiaoType.JIEBIAO_FAIL || type == JieBiaoType.JIEBIAO_SUCCESS ? biaoJuAtlas : playerAtlas;
		icon.spriteName = recordDic [type] [4] + tempRecordInfo.type;

		border.spriteName = type == JieBiaoType.JIEBIAO_FAIL || type == JieBiaoType.JIEBIAO_SUCCESS ? "pinzhi" + (tempRecordInfo.type - 1) : "";

		attackLabel.text = recordDic [type] [2];
		failLabel.text = recordDic [type] [3];

		awardLabel.gameObject.SetActive (type == JieBiaoType.JIEBIAO_FAIL || type == JieBiaoType.YUNBIAO_SUCCESS ? false : true);
		awardLabel.text = tempRecordInfo.shouyi.ToString ();

		nation.spriteName = "nation_" + tempRecordInfo.enemyGuojia;

		levelLabel.text = "Lv" + tempRecordInfo.enemyLevel.ToString ();
		
		if (tempRecordInfo.enemylianMengName.Equals (""))
		{
			nameLabel.text = tempRecordInfo.enemyName + "\n无联盟";
		}
		else
		{
			nameLabel.text = tempRecordInfo.enemyName + "\n<" + tempRecordInfo.enemylianMengName + ">";
		}
		
		zhanLiLabel.text = tempRecordInfo.enemyzhanLi.ToString ();
		
		timeLabel.text = ShowTime (tempRecordInfo.time);
	}

	/// <summary>
	/// 显示对战时间
	/// </summary>
	/// <returns>The time.</returns>
	/// <param name="time">Time.</param>
	private string ShowTime (long time)
	{
		string timeStr = "很久以前";
		
		string timeStr1 = QXComData.UTCToTimeString (time,"yyyy-MM-dd");
		string timeStr2 = QXComData.UTCToTimeString (time,"HH:mm:ss");
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

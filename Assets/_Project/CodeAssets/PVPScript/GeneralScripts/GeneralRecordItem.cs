using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralRecordItem : MonoBehaviour {

	public List<GameObject> m_recordTypeList = new List<GameObject> ();

	private readonly Dictionary<GeneralRecord.RecordType,string> m_recordTypeDic = new Dictionary<GeneralRecord.RecordType, string> ()
	{
		{GeneralRecord.RecordType.SPORT,"Sport"},
	};

	private GeneralRecord.RecordType m_recordType;

	public void InItRecordItem (GeneralRecord.RecordType tempType,object tempInfo)
	{
		m_recordType = tempType;
		SetRecordType ();
		
		switch (tempType)
		{
		case GeneralRecord.RecordType.SPORT:
			m_sportItemResp = tempInfo as ZhandouItem;
			InItSportRecordItem (m_sportItemResp);
			break;
		default:
			break;
		}
	}

	void SetRecordType ()
	{
		foreach (GameObject obj in m_recordTypeList)
		{
			obj.SetActive (obj.name == m_recordTypeDic[m_recordType] ? true : false);
		}
	}

	#region Sport
	public UISprite m_result;
	public UISprite m_compareIcon;
	public UILabel m_compareLabel;
	public UISprite m_head;
	public UISprite m_nation;
	public UILabel m_level;
	public UILabel m_name;
	public UILabel m_time;
	
	private string[] resultLength = new string[]{"attackvictory","attackfail","defensivevictory","defensivefail"};

	private ZhandouItem m_sportItemResp;

	void InItSportRecordItem (ZhandouItem tempInfo)
	{
		m_result.spriteName = resultLength[tempInfo.win - 1];//1-攻击胜利 , 2-攻击失败 , 3-防守胜利 , 4-防守失败
		
		m_nation.spriteName = "nation_" + tempInfo.enemyGuoJiaId;

		QXComData.SetNationSprite (m_nation,tempInfo.enemyGuoJiaId);

		m_head.spriteName = "PlayerIcon" + tempInfo.enemyRoleId;
		
		m_level.text = tempInfo.level.ToString ();
		
		if (tempInfo.enemyId < 0)
		{
			int nameId = int.Parse(tempInfo.enemyName);
			
			string name = NameIdTemplate.GetName_By_NameId (nameId);
			
			m_name.text = name;
		}
		else
		{
			m_name.text = tempInfo.enemyName;
		}
		
		string timeStr = "";
		if (tempInfo.time < 60)
		{
			timeStr = "<1分钟";
		}
		else if (tempInfo.time > 60 && tempInfo.time < 3600)
		{
			timeStr = (tempInfo.time / 60) + "分钟前";
		}
		else if (tempInfo.time / 3600 > 1 && tempInfo.time / 3600 < 24)
		{
			timeStr = (tempInfo.time / 3600) + "小时前";
		}
		else if (tempInfo.time / 3600 >= 24)
		{
			timeStr = (tempInfo.time / (3600 * 24)) + "天前";
		}
		m_time.text = timeStr;
		
		if (tempInfo.junRankChangeV == 0)
		{
			m_compareIcon.spriteName = "";
			m_compareLabel.text = "";
		}
		else
		{
			int colorId = 0;
			if (tempInfo.junRankChangeV > 0)
			{
				colorId = 103;
				m_compareIcon.spriteName = "rankup";
			}
			else if (tempInfo.junRankChangeV < 0)
			{
				colorId = 104;
				m_compareIcon.spriteName = "rankdown";
			}
			
			m_compareLabel.text = MyColorData.getColorString (colorId,tempInfo.junRankChangeV.ToString ());
		}
	}
	#endregion
}

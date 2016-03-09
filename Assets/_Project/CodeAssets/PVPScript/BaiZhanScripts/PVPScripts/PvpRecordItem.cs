using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpRecordItem : MonoBehaviour {

	private ZhandouItem recordInfo;
	
	public UISprite resultIcon;

	public UISprite up_downIcon;
	
	public UILabel up_downLabel;
	
	public UISprite headIcon;
	
	public UISprite country;
	
	public UILabel levelLabel;
	
	public UILabel nameLabel;
	
	public UILabel timeLabel;

	private string[] resultLength = new string[]{"attackvictory","attackfail","defensivevictory","defensivefail"};

	public void InItRecordItemInfo (ZhandouItem tempRecordInfo)
	{
		recordInfo = tempRecordInfo;

		resultIcon.spriteName = resultLength[recordInfo.win - 1];//1-攻击胜利 , 2-攻击失败 , 3-防守胜利 , 4-防守失败
		
//		Debug.Log ("guojiaId:" + recordInfo.enemyGuoJiaId);
		country.spriteName = "nation_" + recordInfo.enemyGuoJiaId;
		
		headIcon.spriteName = "PlayerIcon" + recordInfo.enemyRoleId;
		
		levelLabel.text = recordInfo.level.ToString ();
		
		if (recordInfo.enemyId < 0)
		{
			int nameId = int.Parse(recordInfo.enemyName);
			
			string name = NameIdTemplate.GetName_By_NameId (nameId);
			
			nameLabel.text = MyColorData.getColorString (3,name);
		}
		else
		{
			nameLabel.text = MyColorData.getColorString (3,recordInfo.enemyName);
		}

		string timeStr = "";
		if (recordInfo.time < 60)
		{
			timeStr = "<1分钟";
		}
		else if (recordInfo.time > 60 && recordInfo.time < 3600)
		{
			timeStr = (recordInfo.time / 60) + "分钟前";
		}
		else if (recordInfo.time / 3600 > 1 && recordInfo.time / 3600 < 24)
		{
			timeStr = (recordInfo.time / 3600) + "小时前";
		}
		else if (recordInfo.time / 3600 >= 24)
		{
			timeStr = (recordInfo.time / (3600 * 24)) + "天前";
		}
		timeLabel.text = MyColorData.getColorString (3,timeStr);

		if (recordInfo.junRankChangeV == 0)
		{
			up_downIcon.spriteName = "";
			up_downLabel.text = "";
		}
		else
		{
			if (recordInfo.junRankChangeV > 0)
			{
				up_downIcon.spriteName = "rankup";
			}
			else if (recordInfo.junRankChangeV < 0)
			{
				up_downIcon.spriteName = "rankdown";
			}
			
			up_downLabel.text = MyColorData.getColorString (3,recordInfo.junRankChangeV.ToString ());
		}
	}
}

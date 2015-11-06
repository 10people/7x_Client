using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleRecordItem : MonoBehaviour {

	public ZhandouItem recordInfo;

	public UISprite resultIcon;

	public UISprite up_downIcon;

	public UILabel up_downLabel;

	public UISprite headIcon;

	public UISprite country;

	public UILabel levelLabel;

	public UILabel nameLabel;

	public UILabel timeLabel;

	public void InItRecordItemInfo ()
	{
		Debug.Log ("RecordInfo:" + recordInfo.junRankChangeV + "\n" + recordInfo.receiveWeiWang + "\n" + recordInfo.win);
		switch (recordInfo.win)
		{
		case 1://攻击胜利

			resultIcon.spriteName = "attackvictory";

			break;

		case 2://攻击失败

			resultIcon.spriteName = "attackfail";

			break;

		case 3://防守胜利

			resultIcon.spriteName = "defensivevictory";

			break;

		case 4://防守失败

			resultIcon.spriteName = "defensivefail";

			break;

		default:break;
		}

		Debug.Log ("guojiaId:" + recordInfo.enemyGuoJiaId);
		country.spriteName = "nation_" + recordInfo.enemyGuoJiaId;

		headIcon.spriteName = "PlayerIcon" + recordInfo.enemyRoleId;

		levelLabel.text = recordInfo.level.ToString ();

		if (recordInfo.enemyId < 0)
		{
			int nameId = int.Parse(recordInfo.enemyName);
			
			string name = NameIdTemplate.GetName_By_NameId (nameId);
			
			nameLabel.text = name;
		}
		else
		{
			nameLabel.text = recordInfo.enemyName;
		}

		if (recordInfo.time < 60)
		{
			timeLabel.text = "小于1分钟";
		}
		else if (recordInfo.time > 60 && recordInfo.time < 3600)
		{
			timeLabel.text = (recordInfo.time / 60) + "分钟前";
		}
		else if (recordInfo.time / 3600 > 1 && recordInfo.time / 3600 < 24)
		{
			timeLabel.text = (recordInfo.time / 3600) + "小时前";
		}
		else if (recordInfo.time / 3600 >= 24)
		{
			timeLabel.text = (recordInfo.time / (3600 * 24)) + "天前";
		}

		if (recordInfo.junRankChangeV == 0)
		{
			up_downIcon.gameObject.SetActive (false);
			up_downLabel.gameObject.SetActive (false);
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

			up_downLabel.text = recordInfo.junRankChangeV.ToString ();
		}
	}
}

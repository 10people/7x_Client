using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportEnemyItem : MonoBehaviour {

	public OpponentInfo SportEnemyInfo;

	public UISprite m_headIcon;
	public UILabel m_level;
	public UISprite m_nation;
	public UILabel m_name;
	public UILabel m_rank;
	public UILabel m_zhanLi;
	public UISprite m_newRecord;

	public void InItEnemy ()
	{
		m_headIcon.spriteName = "PlayerIcon" + SportEnemyInfo.roleId;

		m_nation.spriteName = "nation_" + SportEnemyInfo.guojia;
		m_name.text = MyColorData.getColorString (1,SportEnemyInfo.junZhuId < 0 ? NameIdTemplate.GetName_By_NameId (int.Parse(SportEnemyInfo.junZhuName)) : SportEnemyInfo.junZhuName);
		
		m_zhanLi.text = "战力：" + MyColorData.getColorString (SportEnemyInfo.zhanLi > QXComData.JunZhuInfo ().zhanLi ? 5 : 4,SportEnemyInfo.zhanLi.ToString ());
		
		m_level.text = SportEnemyInfo.level.ToString ();

		m_rank.text = "排行：" + SportEnemyInfo.rank.ToString ();

		m_newRecord.gameObject.SetActive (SportEnemyInfo.rank < SportPage.m_instance.SportResp.historyHighRank ? true : false);
	}
}

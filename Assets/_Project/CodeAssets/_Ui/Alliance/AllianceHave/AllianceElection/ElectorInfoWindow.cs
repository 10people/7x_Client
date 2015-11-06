using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ElectorInfoWindow : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UILabel nameLabel;//名字
	private string e_name;
	
	public UILabel levelLabel;//等级
	
	public UILabel gongXianLabel;//贡献
	
	public UILabel junXianLabel;//军衔
	
	public UILabel guanZhiLabel;//官职

	private long e_junZhuId;//君主id

	private int e_allianceId;//联盟id

	public GameObject voteBtnsObj;//确认投票按钮obj

	//获得竞选人信息
	public void GetElectorInfo (MemberInfo e_electorInfo,AllianceHaveResp a_allianceInfo)
	{
		nameLabel.text = e_electorInfo.name;
		e_name = e_electorInfo.name;
		e_allianceId = a_allianceInfo.id;

		headIcon.spriteName = "PlayerIcon" + e_electorInfo.roleId;

		string jiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL);

		levelLabel.text = e_electorInfo.level.ToString () + jiStr;

		gongXianLabel.text = e_electorInfo.contribution.ToString ();

		int junXianId = e_electorInfo.junXian;//军衔id

		BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (junXianId);
		
		junXianLabel.text = NameIdTemplate.GetName_By_NameId (baizhanTemp.templateName);

		int guanZhiId = e_electorInfo.identity;//身份
		
		string guanZhiStr = "";
		
		switch (guanZhiId)
		{
		case 0:

			string chengYuanStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_CHENGYUAN);

			guanZhiStr = chengYuanStr;
			
			break;
			
		case 1:

			string fu_leaderStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_FU_LEADER);

			guanZhiStr = fu_leaderStr;
			
			break;
			
		case 2:

			string leaderStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_LEADER);

			guanZhiStr = leaderStr;
			
			break;
		}
		guanZhiLabel.text = guanZhiStr;

		e_junZhuId = e_electorInfo.junzhuId;
	}

	//取消按钮
	public void DisBtn ()
	{
		VoteManager voteMan = voteBtnsObj.GetComponent<VoteManager> ();
		voteMan.ShowVoteBtn (2);

		AllianceElectionData.electionData.DestroySelect ();

		AllianceElectionData.electionData.MakeZheZhao (false);
		Destroy (this.gameObject);
	}

	//投票确定按钮
	public void VoteBtn ()
	{
		VoteManager voteMan = voteBtnsObj.GetComponent<VoteManager> ();
		voteMan.GetJunZhuInfo (e_junZhuId,e_name,e_allianceId);

		AllianceElectionData.electionData.MakeZheZhao (false);
		Destroy (this.gameObject);
	}
}

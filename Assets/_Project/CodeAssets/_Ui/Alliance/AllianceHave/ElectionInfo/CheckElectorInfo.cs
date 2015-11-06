using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CheckElectorInfo : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UILabel nameLabel;//名字
	
	public UILabel levelLabel;//等级
	
	public UILabel gongXianLabel;//贡献
	
	public UILabel junXianLabel;//军衔
	
	public UILabel guanZhiLabel;//官职

	//显示查看信息
	public void ShowCheckInfo (MemberInfo tempInfo,AllianceHaveResp myAllianceInfo)
	{
		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;

		nameLabel.text = tempInfo.name;

		string jiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_UP_LEVEL);

		levelLabel.text = tempInfo.level.ToString () + jiStr;
		
		gongXianLabel.text = tempInfo.contribution.ToString ();
		
		int junXianId = tempInfo.junXian;//军衔id

		BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (junXianId);
		
		junXianLabel.text = NameIdTemplate.GetName_By_NameId (baizhanTemp.templateName);
		
		int guanZhiId = tempInfo.identity;

		string guanZhiStr = "";

		switch (guanZhiId)
		{
		case 0:
			
			guanZhiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_CHENGYUAN);
			
			break;
			
		case 1:
			
			guanZhiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_FU_LEADER);
			
			break;
			
		case 2:
			
			guanZhiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_MEMBER_LEADER);
			
			break;
		}
		guanZhiLabel.text = guanZhiStr;
	}

	//确定按钮
	public void SureBtn ()
	{
		Destroy (this.gameObject);
	}
}

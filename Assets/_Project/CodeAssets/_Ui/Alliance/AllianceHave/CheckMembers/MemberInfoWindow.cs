using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MemberInfoWindow : MonoBehaviour {

	public UISprite headIcon;//头像icon

	public UILabel nameLabel;//名字

	public UILabel levelLabel;//等级

	public UILabel gongXianLabel;//贡献

	public UILabel junXianLabel;//军衔

	public UILabel guanZhiLabel;//官职

	public GameObject appointBtns;//包含任命官职的btns
	public GameObject kaiChuBtn;//开除btn
	public GameObject shengZhiBtn;//升职btn
	public GameObject jiangZhiBtn;//降职btn

	public GameObject sureBtn;//确定btn

	private int m_identity;//自己在联盟中的职位

	//显示查看信息
	public void ShowCheckInfo (MemberInfo tempInfo,AllianceHaveResp myAllianceInfo)
	{
		m_identity = myAllianceInfo.identity;

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

		ShowBtns (tempInfo.identity);
	}

	//btns显隐
	void ShowBtns (int type)
	{
		sureBtn.SetActive (true);

		if (m_identity == 0 || m_identity == 1)
		{
			appointBtns.SetActive (false);

			sureBtn.transform.localPosition = Vector3.zero;
		}

		else if (m_identity == 2)
		{
			if (type == 0 || type == 1)
			{
				appointBtns.SetActive (true);

				sureBtn.transform.localPosition = new Vector3 (250f,0,0);

				if (type == 0)
				{
					shengZhiBtn.SetActive (true);
					jiangZhiBtn.SetActive (false);
				}
				else 
				{
					shengZhiBtn.SetActive (false);
					jiangZhiBtn.SetActive (true);
				}
			}

			else if (type == 2)
			{
				appointBtns.SetActive (false);

				sureBtn.transform.localPosition = Vector3.zero;
			}
		}
	}
}

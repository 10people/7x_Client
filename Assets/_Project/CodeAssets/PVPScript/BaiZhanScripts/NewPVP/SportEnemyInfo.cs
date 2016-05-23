using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportEnemyInfo : GeneralInstance<SportEnemyInfo> {

	public delegate void EnemyDelegate (int i);

	private EnemyDelegate m_enemyDelegate;

	private OpponentInfo m_sportEnemyInfo;
	
	public UISprite m_junXianIcon;
	public UISprite m_junXianName;
	public UILabel m_rank;
	public UISprite m_nation;
	public UILabel m_name;
	public UILabel m_alliance;
	public UILabel m_zhanLi;
	public UILabel m_zhanLiCompare;

	public UISprite m_skillName;
	public GameObject m_sillInfoObj;
	public UISprite m_skill;
	public GameObject m_lockObj;
	public UISprite m_skillDesBg;
	public UILabel m_skillDes;
	public UILabel m_noSkillDes;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItSportEnemyInfo (OpponentInfo tempInfo,EnemyDelegate tempDelegate)
	{
		//yindao
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);

		m_sportEnemyInfo = tempInfo;
		m_enemyDelegate = tempDelegate;
//		Debug.Log ("junXianLevel:" + tempInfo.junXianLevel);
		int junXianId = tempInfo.junXianLevel;//军衔id
		m_junXianIcon.spriteName = "junxian" + junXianId;
		m_junXianName.spriteName = "JunXian_" + junXianId;
		m_rank.text = MyColorData.getColorString (1,tempInfo.rank.ToString ());
//		Debug.Log ("tempInfo.guojia:" + tempInfo.guojia);
		m_nation.spriteName = QXComData.GetNationSpriteName (tempInfo.guojia);

		m_name.text = tempInfo.junZhuId < 0 ? NameIdTemplate.GetName_By_NameId (int.Parse (tempInfo.junZhuName)) : tempInfo.junZhuName;
		m_alliance.text = tempInfo.lianMengName.Equals ("***") || tempInfo.lianMengName.Equals ("") ? "   " + QXComData.AllianceName (tempInfo.lianMengName) : QXComData.AllianceName (tempInfo.lianMengName);
		m_zhanLi.text = "战力：" + MyColorData.getColorString (QXComData.JunZhuInfo ().zhanLi >= tempInfo.zhanLi ? 4 : 5,tempInfo.zhanLi.ToString ());

		string biJiaoStr = "";
		int colorId = 0;
		int zhanLi = QXComData.JunZhuInfo ().zhanLi - tempInfo.zhanLi;
		if (zhanLi < 0)
		{
			colorId = 5;
			biJiaoStr = "比我高 " + Mathf.Abs(zhanLi);
		}
		else
		{
			colorId = 45;
			biJiaoStr = zhanLi == 0 ? "与我战力相同" : "比我低 " + zhanLi;
		}
		m_zhanLiCompare.text = MyColorData.getColorString (colorId,biJiaoStr);

		m_sillInfoObj.transform.localPosition = new Vector3 (-125,tempInfo.zuheId > 0 ? -15 : 0,0);
		m_lockObj.SetActive (tempInfo.zuheId > 0 ? false : true);

		m_skillDesBg.spriteName = tempInfo.zuheId > 0 ? "0" + tempInfo.zuheId.ToString () : "SkillBg0";

		if (tempInfo.zuheId > 0)
		{
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (tempInfo.zuheId);
			m_skill.spriteName = miBaoSkillTemp.icon.ToString ();
			m_skillName.spriteName = tempInfo.zuheId.ToString ();

			int desId = MiBaoSkillLvTempLate.GetMiBaoSkillComDesIdByZuHeId (tempInfo.zuheId);
			m_skillDes.text = DescIdTemplate.GetDescriptionById (desId);
			m_noSkillDes.text = "";
		}
		else
		{
			m_skill.spriteName = "";
			m_skillName.spriteName = "";
			m_skillDes.text = "";
			m_noSkillDes.text = "未配置秘技";
		}
	}

	public override void MYClick (GameObject ui) 
	{
		switch (ui.name)
		{
		case "ChallengeBtn":
			m_enemyDelegate (1);
			break;
		case "DisBtn":
			m_enemyDelegate (2);
			break;
		case "Zhezhao":
			m_enemyDelegate (3);
			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

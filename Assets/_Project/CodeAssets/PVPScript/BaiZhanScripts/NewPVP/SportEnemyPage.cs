using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportEnemyPage : GeneralInstance<SportEnemyPage> {

	public delegate void EnemyPageDelegate ();
	private EnemyPageDelegate m_enemyPageDelegate;

	private BaiZhanTemplate m_sportTemp;

	private string m_textStr;

	new void Awake ()
	{
		base.Awake ();
	}

	#region PlayerInfo

	public UISprite m_junXianIcon;
	public UISprite m_junXianName;

	public UILabel m_junXian;
	public UILabel m_chanChu;

	public UILabel m_zhanLi;

	public UILabel desLabel;

	public GameObject m_changeBtnObj;

	public GameObject m_sportRewardItem;
	private List<GameObject> m_sportRewardList = new List<GameObject> ();
	private List<string> m_rewardList = new List<string>();

	private bool m_isCanChallenge = false;

	public void InItPlayerInfo ()
	{
		//yindao
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,4);

		m_sportTemp = BaiZhanTemplate.getBaiZhanTemplateById (m_enemyPageInfo.junxianid);

		m_junXianIcon.spriteName = "junxian" + m_sportTemp.jibie;
		m_junXianName.spriteName = "JunXian_" + m_sportTemp.jibie;

		m_junXian.text = MyColorData.getColorString (1,NameIdTemplate.GetName_By_NameId (m_sportTemp.templateName));
		m_chanChu.text = "军衔产出：" + m_sportTemp.produceSpeed + "威望/小时";

		m_zhanLi.text = QXComData.JunZhuInfo ().zhanLi.ToString ();

		string[] m_rewardLen = m_sportTemp.dayAward.Split ('#');
		m_rewardList.Clear ();
		foreach (string str in m_rewardLen)
		{
			m_rewardList.Add (str);
		}

		//结算奖励
		m_sportRewardList = QXComData.CreateGameObjectList (m_sportRewardItem,m_rewardList.Count,m_sportRewardList);
		for (int i = 0;i < m_sportRewardList.Count;i ++)
		{
			m_sportRewardList[i].transform.localPosition = new Vector3(135 + 90 * i,0,0);
			SportRewardItem sportReward = m_sportRewardList[i].GetComponent<SportRewardItem> ();
			sportReward.InItSportRewardItem (m_rewardList[i]);
		}

		m_isCanChallenge = SportPage.m_instance.SportTemp.jibie >= m_sportTemp.jibie - 1 ? true : false;

		desLabel.text = m_isCanChallenge ? "" : "晋升到" + QXComData.GetJunXianName (m_sportTemp.jibie - 1) + "军衔后可以挑战";

		m_changeBtnObj.SetActive (m_isCanChallenge);
	}

	#endregion

	#region EnemyInfo

	private RefreshOtherInfo m_enemyPageInfo;

	public GameObject enemyItemObj;
	private List<GameObject> enemyItemList = new List<GameObject>();

	public void InItSportEnemyPage (RefreshOtherInfo tempInfo,EnemyPageDelegate tempDelegate)
	{
		if (tempInfo.junxianid > 0)
		{
			m_enemyPageInfo = tempInfo;
		}

		m_enemyPageDelegate = tempDelegate;

		InItPlayerInfo ();

		enemyItemList = QXComData.CreateGameObjectList (enemyItemObj,tempInfo.oppoList.Count,enemyItemList);

		for (int i = 0;i < enemyItemList.Count;i ++)
		{
			enemyItemList[i].transform.localPosition = new Vector3(i * 140 - (enemyItemList.Count - 1) * 70,24,0);
			SportEnemyItem sportEnemyItem = enemyItemList[i].GetComponent<SportEnemyItem> ();
			sportEnemyItem.SportEnemyInfo = tempInfo.oppoList[i];
			sportEnemyItem.InItEnemy ();
		}
	}

	#endregion

	public override void MYClick(GameObject ui)
	{
		switch (ui.name)
		{
		case "ChangeBtn":

			SportData.Instance.SportOperateReq (SportData.SportOperate.REFRESH_ENEMYLIST,m_sportTemp.id);
//			m_textStr = SportPage.m_instance.SportResp.huanYiPiYB == 0 ? "首次更换对手免费，确定更换一批对手？" : "确定使用" + SportPage.m_instance.SportResp.huanYiPiYB + "元宝更换一批对手？";
//			QXComData.CreateBoxDiy (m_textStr,false,RefreshPlayer);

			break;
		case "CloseBtn":

			m_enemyPageDelegate ();
			SportPage.m_instance.DailyRewardBtnEffect ();

			break;
		case "ZheZhao":

			m_enemyPageDelegate ();
			SportPage.m_instance.DailyRewardBtnEffect ();

			break;
		default:

			SportEnemyItem sportEnemy = ui.GetComponent<SportEnemyItem> ();
			if (sportEnemy != null)
			{
				if (m_isCanChallenge)
				{
					SportPage.m_instance.OpenSportEnemy (sportEnemy.SportEnemyInfo);
					m_enemyPageDelegate ();
				}
				else
				{
					//军衔等级不够
					m_textStr = "晋升到" + MyColorData.getColorString (5,QXComData.GetJunXianName (m_sportTemp.jibie - 1)) + "军衔后可以挑战";
					ClientMain.m_UITextManager.createText (m_textStr);
				}
			}

			break;
		}
	}

//	void RefreshPlayer (int i)
//	{
//		if (i == 2)
//		{
//			SportData.Instance.SportOperateReq (SportData.SportOperate.REFRESH_ENEMYLIST,m_sportTemp.id);
//		}
//	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
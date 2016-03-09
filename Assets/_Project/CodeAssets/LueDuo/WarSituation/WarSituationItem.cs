using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarSituationItem : MonoBehaviour {

	private HistoryBattleInfo situationInfo;
	private WarSituationData.SituationType situationType;

	public UISprite enemyHead;
	public UILabel enemylevel;
	public UILabel enemyName;
	public UILabel enemyAlliance;
	public UILabel enemyZhanLi;
	public UISprite enemyNation;
	public UILabel enemyProcess;
	public UIScrollBar enemyHpBar;

	public UILabel stateLabel;
	public UILabel fightTimeLabel;

	public GameObject plunderObj;
	public UILabel pNameLabel;
	public UILabel lostCdLabel;

	public GameObject yunBiaoObj;
	public UILabel yNameLabel;
	public UIScrollBar yHpBar;
	public UILabel yHpLabel;
	private GameObject iconSamplePrefab;
		
	public EventHandler situationHandler;

	public UILabel quZhuLabel;

	private int lostCdTime;

	/// <summary>
	/// Ins it situation item.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempInfo">Temp info.</param>
	public void InItSituationItem (WarSituationData.SituationType tempType,HistoryBattleInfo tempInfo)
	{
		situationType = tempType;
		situationInfo = tempInfo;

		enemyHead.spriteName = QXComData.PlayerIcon (situationInfo.enemyRoleId);
		enemylevel.text = situationInfo.enemyLevel.ToString ();
		enemyNation.spriteName = QXComData.Nation (situationInfo.enemyCountryId);
		enemyName.text = MyColorData.getColorString (1,situationInfo.enemyName);
		enemyAlliance.text = MyColorData.getColorString (6,situationInfo.enemyAllianceName.Equals ("***") ? "无联盟" : "<" + situationInfo.enemyAllianceName + ">");
		enemyZhanLi.text = "战力" + situationInfo.enemyZhanLi;
		Debug.Log ("situationInfo.enemyRemainHP:" + situationInfo.enemyRemainHP);
		Debug.Log ("situationInfo.enemyAllHP:" + situationInfo.enemyAllHP);
		int hp = (int)((situationInfo.enemyRemainHP / (float)situationInfo.enemyAllHP) * 100);
		enemyProcess.text = hp + "%";
		QXComData.InItScrollBarValue (enemyHpBar,hp);

		stateLabel.text = tempType == WarSituationData.SituationType.PLUNDER ? "掠夺了" : "正在攻击";

		plunderObj.SetActive (tempType == WarSituationData.SituationType.PLUNDER ? true : false);
		yunBiaoObj.SetActive (tempType == WarSituationData.SituationType.YUNBIAO ? true : false);

		yNameLabel.text = situationInfo.friendName;

		switch (tempType)
		{
		case WarSituationData.SituationType.PLUNDER:

			pNameLabel.text = "";

			situationHandler.gameObject.SetActive (situationInfo.state == 1 ? false : true);

			if (situationInfo.state == 0)
			{
				situationHandler.GetComponentInChildren <UILabel> ().text = "驱逐";
				quZhuLabel.text = "";
			}
			else
			{
				quZhuLabel.text = MyColorData.getColorString (5,"正在被驱逐");
			}

			if (lostCdTime == 0)
			{
				lostCdTime = situationInfo.remainTime;
				StartCoroutine ("LostCd");
			}

			break;
		case WarSituationData.SituationType.YUNBIAO:

			situationHandler.gameObject.SetActive (true);
			situationHandler.GetComponentInChildren <UILabel> ().text = "前往";
			quZhuLabel.text = "";

			StopCoroutine ("LostCd");

			int yhp = (int)((situationInfo.friendRemainHP / (float)situationInfo.friendAllHP) * 100);
			yHpLabel.text = yhp + "%";
			QXComData.InItScrollBarValue (yHpBar,yhp);

			if (iconSamplePrefab == null)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
				                        IconSampleLoadCallBack);
			}
			else
			{
				InItIconSample ();
			}

			break;
		default:
			break;
		}

		situationHandler.m_click_handler -= SituationHandlerClickBack;
		situationHandler.m_click_handler += SituationHandlerClickBack;
	}

	void SituationHandlerClickBack (GameObject obj)
	{
		switch (situationType)
		{
		case WarSituationData.SituationType.PLUNDER:

			//请求可否驱逐状态
			if (situationInfo.state != 1)
			{
				WarSituationPage.situationPage.FightJudgment (situationInfo);
			}

			break;
		case WarSituationData.SituationType.YUNBIAO:

			//跳转到运镖场景
			if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(310))
			{
				PlayerSceneSyncManager.Instance.EnterCarriage();
			}
			else
			{
				FunctionWindowsCreateManagerment.ShowUnopen(310);
			}

			break;
		default:
			break;
		}
	}

	IEnumerator LostCd ()
	{
		while (situationInfo.remainTime > 0)
		{
			situationInfo.remainTime --;

			string timeStr = MyColorData.getColorString (5,TimeHelper.GetUniformedTimeString (situationInfo.remainTime));
			string lostBuildStr = MyColorData.getColorString (5,situationInfo.willLostBuild.ToString ());

			lostCdLabel.text = timeStr + "后联盟将损失" + lostBuildStr + "建设值";

			yield return new WaitForSeconds (1);

			if (situationInfo.remainTime == 0)
			{
				//刷新列表
				WarSituationPage.situationPage.RefreshItemList (situationInfo.itemId,true);
				StopCoroutine ("LostCd");
			}
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = yunBiaoObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3(-40,5,0);
		
		InItIconSample ();
	}
	
	void InItIconSample ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (situationInfo.friendHorseRoleId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (situationInfo.friendHorseRoleId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (situationInfo.friendHorseRoleId,"",1);
		fuShiIconSample.SetIconPopText(situationInfo.friendHorseRoleId, nameStr, mdesc, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
	}
}

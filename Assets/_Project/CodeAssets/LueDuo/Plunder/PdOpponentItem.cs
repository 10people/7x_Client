using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PdOpponentItem : MonoBehaviour {

	private JunZhuInfo junZhuInfo;
	
	public UISprite headIcon;
	public UISprite nation;
	
	public UILabel level;
	public UILabel process;
	public UILabel timeLabel;
	public UILabel nameLabel;
	public UILabel zhanLiLabel;
	public UILabel victory;
	
	public UIScrollBar hpBar;

	public EventHandler plunderHandler;

	private int protectTime;

	/// <summary>
	/// Ins it opponent item.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItOpponentItem (JunZhuInfo tempInfo)
	{
		//Debug.Log ("tempInfo:" + tempInfo.name + "||" + tempInfo.leftProtectTime);
		
		junZhuInfo = tempInfo;
		
		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;
		nation.spriteName = "";

//		if (tempInfo.guojiaId > 0)
//		{
//			nation.SetDimensions (31,42);
//			nation.transform.localScale = Vector3.one;
//		}
//		else
//		{
//			nation.SetDimensions (100,99);
//			nation.transform.localScale = Vector3.one * 0.4f;
//		}

		level.text = tempInfo.level.ToString ();
		nameLabel.text = tempInfo.name;
		
		zhanLiLabel.text = "战力：" + tempInfo.zhanli;
		victory.text = "可掠夺：" + QXComData.MoneyName (QXComData.MoneyType.JIFEN)+ "+" + tempInfo.gongjin;

		//Debug.Log ("HP:" + tempInfo.remainHp + "||" + tempInfo.shengMingMax);
		int jinDu = (int)((tempInfo.remainHp / (float)tempInfo.shengMingMax) * 100);
		jinDu = jinDu < 1 ? 1 : jinDu;
		process.text = jinDu + "%";
		hpBar.barSize = (float)jinDu / 100;

		process.gameObject.SetActive (tempInfo.leftProtectTime > 0 ? false : true);
		timeLabel.gameObject.SetActive (tempInfo.leftProtectTime > 0 ? true : false);

		protectTime = tempInfo.leftProtectTime;
		StopCoroutine ("ProtectTimeShow");
		StartCoroutine ("ProtectTimeShow");
		Debug.Log ("lianmeng:" + junZhuInfo.lianMeng + "||" + AllianceData.Instance.g_UnionInfo.name);
		Debug.Log ("M_CurAllianceId:" + PlunderPage.plunderPage.M_CurAllianceId);
		Debug.Log ("g_UnionInfo.id:" + AllianceData.Instance.g_UnionInfo.id);
		if (PlunderPage.plunderPage.M_CurAllianceId == AllianceData.Instance.g_UnionInfo.id)
		{
			QXComData.SetBtnState (plunderHandler.gameObject,false);
		}
		else
		{
			QXComData.SetBtnState (plunderHandler.gameObject,protectTime <= 0 ? true : false);
		}

		plunderHandler.m_click_handler -= PlunderHandlerClickBack;
		plunderHandler.m_click_handler += PlunderHandlerClickBack;
	}
	
	IEnumerator ProtectTimeShow ()
	{
		string minuteStr = "";
		string secondStr = "";
		
		while (protectTime > 0) 
		{
			protectTime --;
			
			int minute = (protectTime / 60) % 60;
			int second = protectTime % 60;
			
			if (minute < 10)
			{
				minuteStr = "0" + minute;
			}
			else
			{
				minuteStr = minuteStr.ToString ();
			}
			
			if (second < 10) 
			{
				secondStr = "0" + second;
			} 
			else 
			{
				secondStr = second.ToString ();
			}
			
			timeLabel.text = minuteStr + "：" + secondStr;
			
			if (protectTime == 0) 
			{
				PlunderPage.plunderPage.IsOpponentToTop = false;
				PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.JUNZHU,PlunderPage.plunderPage.GetCurAllianceId ());
			}
			yield return new WaitForSeconds(1);
		}
	}

	void PlunderHandlerClickBack (GameObject obj)
	{
		if (PlunderPage.plunderPage.M_CurAllianceId == AllianceData.Instance.g_UnionInfo.id)
		{
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,junZhuInfo.junZhuId == QXComData.JunZhuInfo ().id ? "无法掠夺自己" : "不能掠夺自己的盟友"));
		}
		else
		{
			PlunderData.Instance.PlunderOpponent (PlunderData.Entrance.PLUNDER,junZhuInfo.junZhuId);
		}
	}
}

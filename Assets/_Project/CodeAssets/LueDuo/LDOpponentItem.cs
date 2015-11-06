using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDOpponentItem : MonoBehaviour {

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
	
	public GameObject lueDuoBtn;

	private int protectTime;
	
	public void GetLDOpponentInfo (JunZhuInfo tempInfo)
	{
//		Debug.Log ("tempInfo:" + tempInfo.name + "||" + tempInfo.leftProtectTime);

		junZhuInfo = tempInfo;
		
		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;
		
		nation.spriteName = "nation_" + tempInfo.guojiaId;
		
		level.text = tempInfo.level.ToString ();
		
		nameLabel.text = tempInfo.name;
		
		zhanLiLabel.text = MyColorData.getColorString (3,"战力：" + tempInfo.zhanli);
		
		victory.text = MyColorData.getColorString (4,"可掠夺：贡金+" + tempInfo.gongjin);
//		Debug.Log ("HP:" + tempInfo.remainHp + "||" + tempInfo.shengMingMax);
		int jinDu = (int)((tempInfo.remainHp / (float)tempInfo.shengMingMax) * 100);

		if (jinDu < 1)
		{
			jinDu = 1;
		}

		process.text = jinDu + "%";
		
		hpBar.barSize = (float)jinDu / 100;

		StopCoroutine ("ProtectTimeShow");

		if (tempInfo.leftProtectTime > 0)
		{
			process.gameObject.SetActive (false);
			timeLabel.gameObject.SetActive (true);
			lueDuoBtn.SetActive (false);

			protectTime = tempInfo.leftProtectTime;
			StartCoroutine ("ProtectTimeShow");
		}
		else
		{
			process.gameObject.SetActive (true);
			timeLabel.gameObject.SetActive (false);
			lueDuoBtn.SetActive (true);
		}
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
//				junZhuInfo.leftProtectTime = 0;
//				GetLDOpponentInfo (junZhuInfo);

				LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.JunZhu,
				               LueDuoData.Instance.GetNationId,
				               LueDuoData.Instance.GetAllianceId,
				               LueDuoData.Instance.GetAllianceStartPage,
				               LueDuoData.Direction.Default);
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// 掠夺对手按钮
	/// </summary>
	public void LueDuoBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;
			LueDuoManager.ldManager.ShowChangeSkillEffect (false);
			LueDuoData.Instance.JunNationId = junZhuInfo.guojiaId;
			LueDuoData.Instance.LueDuoOpponentReq (junZhuInfo.junZhuId,LueDuoData.WhichOpponent.LUE_DUO);
		}
	}
}

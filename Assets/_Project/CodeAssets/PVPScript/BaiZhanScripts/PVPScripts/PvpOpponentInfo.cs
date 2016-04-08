﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpOpponentInfo : MonoBehaviour {

	private OpponentInfo opponentInfo;
	
	public UISprite junXianIcon;
	public UISprite junXianName;
	public UILabel rankLabel;
	public UILabel nameLabel;
	public UILabel allianceLabel;
	public UILabel zhanLiLabel;
	public UISprite country;
	public UILabel zhanLiBiJiao;
	
	/// <summary>
	/// 秘宝技能相关
	/// </summary>
	public UISprite miBaoSkillBg;
	
	public UISprite skillIcon;
	public GameObject lockObj;
	
	public UILabel noSkillLabel;
	public GameObject miBaoSkillInfoObj;
	public UILabel skillName;
	public UILabel desLabel;
	
	public ScaleEffectController sEffectControl;
	
	public GameObject infoObj;
	public GameObject addFriendBtn;

	public List<EventHandler> btnHandlerList = new List<EventHandler> ();
	
	private bool isNextState = false;//是否进行下一步
	
	public void ScaleEffect ()
	{
		sEffectControl.OnOpenWindowClick ();
	}
	
	public void InItOpponentWindow (OpponentInfo tempInfo)
	{
		opponentInfo = tempInfo;

		int junXianId = tempInfo.junXianLevel;//军衔id
		junXianIcon.spriteName = "junxian" + junXianId;
		junXianName.spriteName = "JunXian_" + junXianId;
		
		rankLabel.text = MyColorData.getColorString (3,tempInfo.rank.ToString ());

		nameLabel.text = MyColorData.getColorString (3,tempInfo.junZhuId < 0 ? NameIdTemplate.GetName_By_NameId (int.Parse (tempInfo.junZhuName)) : tempInfo.junZhuName);
		allianceLabel.text = MyColorData.getColorString (3, QXComData.AllianceName (tempInfo.lianMengName));
		
		zhanLiLabel.text = MyColorData.getColorString (3,"战力：") + MyColorData.getColorString (BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.zhanLi >= tempInfo.zhanLi ? 4 : 5,tempInfo.zhanLi.ToString ());

		string biJiaoStr = "";
		int zhanLi = BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.zhanLi - tempInfo.zhanLi;

		if (zhanLi < 0)
		{
			biJiaoStr = MyColorData.getColorString (5,"比我高 " + Mathf.Abs(zhanLi));
		}
		else if (zhanLi == 0)
		{
			biJiaoStr = MyColorData.getColorString (4,"与我战力相同");
		}
		else
		{
			biJiaoStr = MyColorData.getColorString (4,"比我低 " + zhanLi);
		}
		zhanLiBiJiao.text = biJiaoStr;

		country.spriteName = "nation_" + tempInfo.guojia;

		lockObj.SetActive (tempInfo.zuheId > 0 ? false : true);
		noSkillLabel.text = tempInfo.zuheId <= 0 ? "未配置秘技" : "";
		miBaoSkillInfoObj.SetActive (tempInfo.zuheId > 0 ? true : false);
		
		if (tempInfo.zuheId > 0)
		{
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (tempInfo.zuheId);
			skillIcon.spriteName = miBaoSkillTemp.icon.ToString ();
			skillName.text = NameIdTemplate.GetName_By_NameId (miBaoSkillTemp.nameId);

			MiBaoSkillLvTempLate miBaoSkillLvTemp = MiBaoSkillLvTempLate.GetMiBaoSkillLvTemplateByIdAndLevel (tempInfo.zuheId,tempInfo.zuHeLevel);
			desLabel.text = DescIdTemplate.GetDescriptionById (miBaoSkillLvTemp.Desc);
		}
		else
		{
			skillIcon.spriteName = "";
			skillName.text = "";
			desLabel.text = "";
		}
		
//		PvpPage.pvpPage.MibaoSkillBgColor (miBaoSkillTemp == null ? 0 : tempInfo.zuheId);
//		Debug.Log ("tempInfo.zuheId:" + tempInfo.zuheId);
		miBaoSkillBg.spriteName = tempInfo.zuheId > 0 ? tempInfo.zuheId.ToString () : "SkillBg0";

		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_click_handler -= BtnHandlerCallBack;
			handler.m_click_handler += BtnHandlerCallBack;
		}
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);
	}

	void BtnHandlerCallBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "FightBtn":

			//请求挑战
			PvpData.Instance.PvpOpponentInfo = opponentInfo;
			PvpData.Instance.PlayerStateCheck (PvpData.PlayerState.STATE_PVP_MAIN_PAGE);

			DisActiveWindow ();

			break;
		case "DisBtn":

			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);
			
			DisActiveWindow ();

			break;
		case "AddFriendBtn":

//			ConfirmManager.confirm.AddFriendName = opponentInfo.junZhuName;
//			ConfirmManager.confirm.AddFriendId = opponentInfo.junZhuId;
//			FriendOperationLayerManagerment.AddFriends ((int)(opponentInfo.junZhuId));
			FriendOperationData.Instance.AddFriends (FriendOperationData.AddFriendType.BaiZhan,
			                                         opponentInfo.junZhuId,opponentInfo.junZhuName);

			break;
		case "Zhezhao":

			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);
			
			DisActiveWindow ();

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 显示好友状态
	/// </summary>
	public void ShowFriendState (bool isShowFriend)
	{
		if (!isShowFriend)
		{
			infoObj.transform.localPosition = new Vector3 (60,10,0);
			addFriendBtn.SetActive (false);
			return;
		}
		else
		{
			bool isFriend = false;
			if (FriendOperationData.Instance.friendIdList.Contains (opponentInfo.junZhuId) || opponentInfo.junZhuId < 0)
			{
				isFriend = true;
			}
			else
			{
				isFriend = false;
			}
			infoObj.transform.localPosition = isFriend ? new Vector3 (60,10,0) : new Vector3 (15,10,0);
			addFriendBtn.SetActive (!isFriend);
		}
	}

	void DisActiveWindow ()
	{
		gameObject.SetActive (false);
	}
}

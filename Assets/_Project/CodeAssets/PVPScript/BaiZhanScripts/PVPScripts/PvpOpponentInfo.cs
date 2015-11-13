using UnityEngine;
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
	public UILabel junXianLabel;
	public UILabel rankLabel;
	public UILabel nameLabel;
	public UILabel allianceLabel;
	public UILabel zhanLiLabel;
	public UISprite country;
	
	/// <summary>
	/// 秘宝技能相关
	/// </summary>
	public UISprite miBaoSkillBg;
	
	public UISprite skillIcon;
	public GameObject lockObj;
	
	public UILabel notActiveLabel;
	public GameObject miBaoSkillInfoObj;
	public UISprite titleSprite;
	public UILabel skillName;
	public UILabel activeNum;
	
	public ScaleEffectController sEffectControl;
	
	public GameObject infoObj;
	public GameObject iconObj;
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
		
		BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (junXianId);
		junXianLabel.text = NameIdTemplate.GetName_By_NameId (baizhanTemp.templateName);
		
		rankLabel.text = tempInfo.rank.ToString ();
		
		if (tempInfo.junZhuId < 0)
		{
			int nameId = int.Parse (tempInfo.junZhuName);
			nameLabel.text = NameIdTemplate.GetName_By_NameId (nameId);
		}
		else
		{
			nameLabel.text = tempInfo.junZhuName;
		}
		
		if (tempInfo.lianMengName.Equals (""))
		{
			allianceLabel.text = "无联盟";
		}
		else
		{
			allianceLabel.text = "<" + tempInfo.lianMengName + ">";
		}
		
		zhanLiLabel.text = tempInfo.zhanLi.ToString ();
		
		country.spriteName = "nation_" + tempInfo.guojia;

		if (tempInfo.activateMiBaoCount >= 2)
		{
			lockObj.SetActive (false);
			skillIcon.gameObject.SetActive (true);
			miBaoSkillInfoObj.SetActive (true);
			notActiveLabel.text = "";

			int miBaoCombId = tempInfo.zuheId;
			miBaoSkillBg.spriteName = PvpPage.pvpPage.MibaoSkillBgColor (miBaoCombId);
			titleSprite.spriteName = miBaoCombId.ToString ();
			
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (miBaoCombId);
			skillIcon.spriteName = miBaoSkillTemp.skill.ToString ();
			
			SkillTemplate skillTemp = SkillTemplate.getSkillTemplateById (miBaoSkillTemp.skill);
			skillName.text = NameIdTemplate.GetName_By_NameId (skillTemp.skillName);
			activeNum.text = NameIdTemplate.GetName_By_NameId (miBaoSkillTemp.nameId) + "(" + tempInfo.activateMiBaoCount + "/3)";
		}
		else
		{
			lockObj.SetActive (true);
			skillIcon.gameObject.SetActive (false);
			miBaoSkillInfoObj.SetActive (false);
			miBaoSkillBg.spriteName = PvpPage.pvpPage.MibaoSkillBgColor (4);
			notActiveLabel.text = "未选择可用的组合技能";
		}
//		Debug.Log ("btnHandlerList:" + btnHandlerList.Count);
		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler += BtnHandlerCallBack;
		}
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

			PvpPage.pvpPage.SkillEffect (true);

			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,3);
			
			DisActiveWindow ();

			break;
		case "AddFriendBtn":

//			ConfirmManager.confirm.AddFriendName = opponentInfo.junZhuName;
//			ConfirmManager.confirm.AddFriendId = opponentInfo.junZhuId;
//			FriendOperationLayerManagerment.AddFriends ((int)(opponentInfo.junZhuId));
			FriendOperationData.Instance.AddFriends (FriendOperationData.AddFriendType.BaiZhan,
			                                         opponentInfo.junZhuId,opponentInfo.junZhuName);

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
			infoObj.transform.localPosition = Vector3.zero;
			iconObj.transform.localPosition = new Vector3 (-125,45,0);
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
			infoObj.transform.localPosition = isFriend ? new Vector3 (55,0,0) : Vector3.zero;
			iconObj.transform.localPosition = isFriend ? new Vector3 (-100,45,0) : new Vector3 (-125,45,0);
			addFriendBtn.SetActive (!isFriend);
		}
	}

	void DisActiveWindow ()
	{
		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler -= BtnHandlerCallBack;
		}
		gameObject.SetActive (false);
	}
}

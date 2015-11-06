using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanOpponent : MonoBehaviour {

	public OpponentInfo opponentInfo;

	public UISprite headIcon;//头像

	public UILabel nameLabel;//名字

	public UILabel zhanLiLabel;//战力

	public UILabel levelLabel;//等级

	public UISprite rankIcon;//排名icon
	public UILabel rankLabel;//排名

	public UISprite country;//国家

	public GameObject opponentWindow;

	private bool isFriend;
	public bool SetFriend
	{
		set{isFriend = value;}
	}

	public void InItOpponentInfo ()
	{
//		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
//		{
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
//			if (tempTaskData.m_iCurIndex == 3)
//			{
//				Debug.Log ("ok");
//				this.gameObject.GetComponent<UIDragScrollView> ().enabled = false;
//			}
//		}

		if (opponentInfo.junZhuId < 0)
		{
			int nameId = int.Parse(opponentInfo.junZhuName);
			
			string name = NameIdTemplate.GetName_By_NameId (nameId);
			
			nameLabel.text = name;
		}
		
		else
		{
			nameLabel.text = opponentInfo.junZhuName;
		}

		zhanLiLabel.text = opponentInfo.zhanLi.ToString ();

		levelLabel.text = opponentInfo.level.ToString ();

		if (opponentInfo.rank < 4)
		{
			rankIcon.gameObject.SetActive (true);

			rankIcon.spriteName = "rank" + opponentInfo.rank;
		}
		rankLabel.text = opponentInfo.rank.ToString ();
//		Debug.Log ("RoleId:" + opponentInfo.roleId);
		headIcon.spriteName = "PlayerIcon" + opponentInfo.roleId;

		country.spriteName = "nation_" + opponentInfo.guojia;
	}

	void OnClick ()
	{
		if (BaiZhanMainPage.baiZhanMianPage.IsYinDaoStop)
		{
			return;
		}

		if (!BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent)
		{
			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = true;
			CloneInfoWindow ();
			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (false);
			if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
			{
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			}
		}
	}

	//克隆显示对手详情的窗口
	void CloneInfoWindow ()
	{
		GameObject window = (GameObject)Instantiate (opponentWindow);
		
		window.SetActive (true);
		window.name = "BaiZhanOpponentInfo";
		
		window.transform.parent = opponentWindow.transform.parent;
		
		window.transform.localPosition = opponentWindow.transform.localPosition;
		
		window.transform.localScale = opponentWindow.transform.localScale;
		
		BaiZhanOpponentInfo baiZhanOpponent = window.GetComponent<BaiZhanOpponentInfo> ();
		baiZhanOpponent.m_opponentInfo = opponentInfo;
		Debug.Log ("isFriend:" + isFriend);
		baiZhanOpponent.IsFriend = isFriend;
		baiZhanOpponent.InItOpponentWindow ();
	}

	void Update ()
	{
		if (FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			if (UIYindao.m_UIYindao.m_isOpenYindao)
			{
				this.gameObject.GetComponent<UIDragScrollView> ().enabled = false;
			}
			else
			{
				this.gameObject.GetComponent<UIDragScrollView> ().enabled = true;
			}
		}
	}
}

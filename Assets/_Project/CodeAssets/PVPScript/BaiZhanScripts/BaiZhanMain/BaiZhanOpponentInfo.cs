using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanOpponentInfo : MonoBehaviour {

	public OpponentInfo m_opponentInfo;

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

	public GameObject windowObj;

	public ScaleEffectController sEffectControl;

	public GameObject infoObj;
	public GameObject iconObj;
	public GameObject addFriendBtn;

	private bool isFriend;
	public bool IsFriend
	{
		set{isFriend = value;}
	}

	private bool isNextState = false;//是否进行下一步

	void Start ()
	{
//		Scale ();
		sEffectControl.OnOpenWindowClick ();
	}

	void Scale () 
	{
		Hashtable scale = new Hashtable ();
		
		scale.Add ("scale",Vector3.one);
		scale.Add ("islocal",true);
		scale.Add ("time",0.6f);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		
		iTween.ScaleTo (windowObj,scale);
	}

	public void InItOpponentWindow ()
	{
		int junXianId = m_opponentInfo.junXianLevel;//军衔id

		junXianIcon.spriteName = "junxian" + junXianId;

		BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (junXianId);
		junXianLabel.text = NameIdTemplate.GetName_By_NameId (baizhanTemp.templateName);
		
		rankLabel.text = m_opponentInfo.rank.ToString ();

		if (isFriend)
		{
			infoObj.transform.localPosition = new Vector3(55,0,0);
			iconObj.transform.localPosition = new Vector3(-100,45,0);
			addFriendBtn.SetActive (false);
		}

		if (m_opponentInfo.junZhuId < 0)
		{
			int nameId = int.Parse (m_opponentInfo.junZhuName);
			
			nameLabel.text = NameIdTemplate.GetName_By_NameId (nameId);
		}
		
		else
		{
			nameLabel.text = m_opponentInfo.junZhuName;
		}

		if (m_opponentInfo.lianMengName.Equals (""))
		{
			allianceLabel.text = "无联盟";
		}
		else
		{
			allianceLabel.text = "<" + m_opponentInfo.lianMengName + ">";
		}

		zhanLiLabel.text = m_opponentInfo.zhanLi.ToString ();

		country.spriteName = "nation_" + m_opponentInfo.guojia;

		int miBaoCombId = m_opponentInfo.zuheId;
		
		if (m_opponentInfo.activateMiBaoCount >= 2)
		{
			lockObj.SetActive (false);
			skillIcon.gameObject.SetActive (true);
			miBaoSkillInfoObj.SetActive (true);
			notActiveLabel.text = "";
			
			MibaoSkillBgColor (miBaoCombId);
			titleSprite.spriteName = miBaoCombId.ToString ();
			
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (miBaoCombId);
			skillIcon.spriteName = miBaoSkillTemp.skill.ToString ();
			
			SkillTemplate skillTemp = SkillTemplate.getSkillTemplateById (miBaoSkillTemp.skill);
			skillName.text = NameIdTemplate.GetName_By_NameId (skillTemp.skillName);
			
			activeNum.text = NameIdTemplate.GetName_By_NameId (miBaoSkillTemp.nameId) +
				"(" + m_opponentInfo.activateMiBaoCount + "/3)";
		}
		else
		{
			lockObj.SetActive (true);
			skillIcon.gameObject.SetActive (false);
			miBaoSkillInfoObj.SetActive (false);
			miBaoSkillBg.spriteName = "greybg";
			notActiveLabel.text = "未选择可用的组合技能";
		}
	}

	/// <summary>
	/// 0-灰色 1-蓝色 2-黄色 3-红色
	/// </summary>
	/// <param name="id">Identifier.</param>
	void MibaoSkillBgColor (int id)
	{
		switch (id)
		{
		case 1:
			
			miBaoSkillBg.spriteName = "bulebg";
			
			break;
			
		case 2:
			
			miBaoSkillBg.spriteName = "redbg";
			
			break;
			
		case 3:
			
			miBaoSkillBg.spriteName = "yellowbg";
			
			break;
			
		case 4:
			
			miBaoSkillBg.spriteName = "greybg";
			
			break;
		}
	}

	//挑战按钮
	public void ChallengeBtn ()
	{
		if (!isNextState)
		{
			isNextState = true;

			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (false);
			//请求挑战
			BaiZhanUnExpected.unExpected.TiaoZhanStateReq (m_opponentInfo,1,null);

			Destroy (this.gameObject);
		}
	}

	//取消按钮
	public void CancelBtn ()
	{
		if (!isNextState)
		{
			isNextState = true;
			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
			if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
			{
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			}
			
			Destroy (this.gameObject);
		}
	}

	//添加好友按钮
	public void AddFriendBtn ()
	{
		ConfirmManager.confirm.AddFriendName = m_opponentInfo.junZhuName;
		ConfirmManager.confirm.AddFriendId = m_opponentInfo.junZhuId;
		FriendOperationLayerManagerment.AddFriends ((int)(m_opponentInfo.junZhuId));
	}
}

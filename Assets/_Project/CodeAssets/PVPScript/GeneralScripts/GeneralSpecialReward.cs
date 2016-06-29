using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class GeneralSpecialReward : MonoBehaviour {

	[HideInInspector]public RewardData rewardData;

	private QXComData.XmlType xmlType;

	public GameObject miBaoObj;
	public GameObject equipObj;

	//将魂信息
	public UITexture mbTexTure;//将魂图标
	public UISprite border;
	public UILabel miBaoName;
	public GameObject starObj;
	private int effectId;

	public GameObject leftObj;
	public UILabel miBaoCount;
	public UILabel unLockNeedNum;
	public UISprite skillIcon;
	public UILabel skillName;
	public UILabel skillDes;

	public GameObject rightObj;
	public UILabel shuxing_attack;
	public UILabel shuxing_deffense;
	public UILabel shuxing_hp;
	public UILabel star_attack;
	public UILabel star_deffense;
	public UILabel star_hp;

	public GetMiBaoInfo getMiBaoInfo;

	private int defaultStar;

	//装备信息
	public UILabel cardLabel;

	public UILabel desLabel;
	private float time = 1f;
	private string m_desStr = "[f2cc84]点击屏幕继续[-]";
	private bool m_start = false;
	private bool m_Count = false;

	public EventHandler rewardHandler;

	private bool isScaleEnd = false;

	public void InItSpecialReward (RewardData tempData)
	{
		rewardData = tempData;
		desLabel.text = "";
		xmlType = (QXComData.XmlType)Enum.ToObject (typeof (QXComData.XmlType),CommonItemTemplate.GetCommonItemTemplateTypeById (tempData.itemId));
//		Debug.Log ("xmlType:" + xmlType);
		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			InItMibaoCardInfo ();
			desLabel.transform.localPosition = new Vector3(0,-270,0);

			break;
		case QXComData.XmlType.EQUIP:

			InItEquipReward ();
			desLabel.transform.localPosition = new Vector3(0,-175,0);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Scales the effect.
	/// </summary>
	/// <param name="obj">Object.</param>
	void ScaleEffect (GameObject obj,float time)
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("scale",Vector3.one);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete","ScaleEffectEnd");
		scale.Add ("oncompletetarget",this.gameObject);
		iTween.ScaleTo (obj,scale);
	}
	void ScaleEffectEnd ()
	{
		//特效
		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			if (!rewardData.m_isCheckOnly)
			{
				MiBaoCardDes ();
			}
			else
			{
				isScaleEnd = true;
			}

//			UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,
//			                                   miBaoObj,
//			                                   EffectIdTemplate.GetPathByeffectId(100157));

			QXComData.InstanceEffect (QXComData.EffectPos.MID,miBaoObj,100157);

			if (QXComData.CheckOpenTask (100160) && !TanBaoData.Instance.M_IsGetCard)
			{
				TanBaoData.Instance.M_IsGetCard = true;
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.FINISHED_TASK_YINDAO,100160,3);
			}

			if (!QXComData.CheckYinDaoOpenState (100160))
			{
				UIYindao.m_UIYindao.setCloseUIEff ();
			}

			break;
		case QXComData.XmlType.EQUIP:

//			UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,
//			                                   equipObj,
//			                                   EffectIdTemplate.GetPathByeffectId(100142 + effectId));
//			UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,
//			                                   equipObj,
//			                                   EffectIdTemplate.GetPathByeffectId(100150 + effectId));
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,equipObj,100142 + effectId);
			QXComData.InstanceEffect (QXComData.EffectPos.MID,equipObj,100150 + effectId);
			isScaleEnd = true;

			break;
		default:
			break;
		}

		rewardHandler.m_click_handler += RewardHandlerClickBack;

		StartCoroutine ("ShowDesLabel1");
//		Debug.Log ("QXComData.CheckYinDaoOpenState (100160)1:" + QXComData.CheckYinDaoOpenState (100160));
	}

	void RewardHandlerClickBack (GameObject obj)
	{
		if (!isScaleEnd)
		{
			return;
		}
		isScaleEnd = false;

		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			UI3DEffectTool.ClearUIFx (miBaoObj);

			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.FINISHED_TASK_YINDAO,100160,4);
			if (!QXComData.CheckYinDaoOpenState (100160))
			{
				UIYindao.m_UIYindao.setOpenUIEff ();
			}

			if (!rewardData.m_isCheckOnly)
			{
				if (!rewardData.m_isNew)
				{
					TBMiBaoSuiPian.m_instance.GetMiBaoSuiPianInfo (rewardData);
					TBMiBaoSuiPian.m_instance.M_SuiPianDelegate = CheckNext;
					
					gameObject.SetActive (false);
				}
				else
				{
					CheckNext ();
					if (rewardData.miBaoClick != null)
					{
						rewardData.miBaoClick ();
					}
				}
			}
			else
			{
				CheckNext ();
				if (rewardData.miBaoClick != null)
				{
					rewardData.miBaoClick ();
				}
			}

			break;
		case QXComData.XmlType.EQUIP:

			UI3DEffectTool.ClearUIFx (equipObj);
			CheckNext ();

			break;
		default:
			break;
		}

//		Debug.Log ("QXComData.CheckYinDaoOpenState (100160)2:" + QXComData.CheckYinDaoOpenState (100160));
	}

	void CheckNext ()
	{
		GeneralRewardManager.Instance().SpecialReward_Index ++;
		GeneralRewardManager.Instance().CheckSpecialReward ();
		GeneralRewardManager.Instance().RefreshSpecialItemList (this.gameObject);
	}

	void MiBaoCardDes ()
	{
		leftObj.SetActive (true);
		rightObj.SetActive (true);

		var miBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
		int miBaoNum = 0;
//		int mibaoLevel = 1;

		for (int i = 0;i < miBaoInfo.miBaoList.Count;i ++)
		{
			if (miBaoInfo.miBaoList[i].level > 0)
			{
				miBaoNum ++;
			}
//			if (miBaoInfo.miBaoList[i].miBaoId == rewardData.itemId)
//			{
//				mibaoLevel = miBaoInfo.miBaoList[i].level;
//			}
		}

		miBaoCount.text = "当前将魂个数：" + MyColorData.getColorString (5, miBaoNum.ToString ()) + "个";

		int nextNeedNum = 0;//解锁下个技能还需将魂个数
		int getSkillId = 0;

		string unLockDesStr = "";//解锁需将魂数

		if (miBaoInfo.skillList == null || miBaoInfo.skillList.Count == 0)
		{
			getSkillId = 1;
		}
		else
		{
			if (miBaoInfo.skillList.Count >= 7)
			{
				getSkillId = 7;
			}
			else
			{
				getSkillId = miBaoInfo.skillList.Count + 1;
			}
		}
//		Debug.Log ("getSkillId:" + getSkillId);
		MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (getSkillId);
		skillIcon.spriteName = mMiBaoskill.icon;

		skillName.text = NameIdTemplate.GetName_By_NameId (mMiBaoskill.nameId);

		string desStr = "";
		string[] desLen = DescIdTemplate.getDescIdTemplateByNameId (mMiBaoskill.briefDesc).description.Split ('#');
		for (int i = 0;i < desLen.Length;i ++)
		{
			desStr += (i < desLen.Length - 1 ? desLen[i] + "\n" : desLen[i]);
		}

		skillDes.text = desStr;

//		Debug.Log ("miBaoNum:" + miBaoNum);
//		Debug.Log ("mMiBaoskill.needNum:" + mMiBaoskill.needNum);

		if (getSkillId < 7)
		{
			if (miBaoNum >= mMiBaoskill.needNum)
			{
				unLockDesStr = "当前无双技能可解锁";
			}
			else
			{
				nextNeedNum = mMiBaoskill.needNum - miBaoNum;
				unLockDesStr = "再收集" + MyColorData.getColorString (5,nextNeedNum.ToString ()) + "个将魂，可解锁";
			}
		}
		else
		{
			unLockDesStr = "无双技能已全部解锁";
		}

		unLockNeedNum.text = unLockDesStr;
//		Debug.Log ("defaultStar:" + defaultStar);
		rewardData.miBaoStar = rewardData.miBaoStar == 0 ? 1 : rewardData.miBaoStar;

		shuxing_attack.text = "基础攻击：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,1,defaultStar,1);
		shuxing_deffense.text = "基础防御：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,1,defaultStar,2);
		shuxing_hp.text = "基础生命：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,1,defaultStar,3);

		star_attack.text = "攻击：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,100,5,1);
		star_deffense.text = "防御：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,100,5,2);
		star_hp.text = "生命：" + getMiBaoInfo.JiSuanZhanLi (rewardData.itemId,100,5,3);

		DesObjMove (leftObj,true);
		DesObjMove (rightObj,false);
	}

	void DesObjMove (GameObject obj,bool left)
	{
		Hashtable move = new Hashtable ();
		move.Add ("position",new Vector3(left ? -325 : 325,0,0));
		move.Add ("time",0.5f);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("islocal",true);
		move.Add ("oncomplete","MoveEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (obj,move);
	}

	void MoveEnd ()
	{
		isScaleEnd = true;
	}

	IEnumerator ShowDesLabel1 ()
	{
		desLabel.text = m_desStr;
		yield return new WaitForSeconds (time);
		m_start = true;
		time = 0.07f;
	}

	void Update ()
	{
		if (!m_start)
		{
			return;
		}
		UIWidget widget = desLabel.GetComponent<UIWidget> ();
		float disCount = 0.03f;
		if (!m_Count)
		{
			if (widget.alpha < 1)
			{
				widget.alpha += disCount;
				if (widget.alpha >= 1)
				{
					widget.alpha = 1;
					m_Count = true;
				}
			}
			else
			{
				m_Count = true;
			}
		}
		else
		{
			if (widget.alpha > 0.4f)
			{
				widget.alpha -= disCount;
				if (widget.alpha <= 0.4f)
				{
					widget.alpha = 0.4f;
					m_Count = false;
				}
			}
			else
			{
				m_Count = false;
			}
		}
	}

	/// <summary>
	/// Ins it mibao card info.
	/// </summary>
	void InItMibaoCardInfo ()
	{
		MiBaoXmlTemp miBaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (rewardData.itemId);
		int iconId = miBaoTemp.icon;
		int nameId = miBaoTemp.nameId;

		defaultStar = rewardData.miBaoStar > 0 ? rewardData.miBaoStar : miBaoTemp.initialStar;

		mbTexTure.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON)
		        
		                                                 + iconId.ToString ());
		int colorId = CommonItemTemplate.getCommonItemTemplateById (rewardData.itemId).color;

		effectId = QXComData.GetEffectColorByXmlColorId (colorId);

//		border.spriteName = "pinzhi" + (colorId - 1);

//		border.width = QXComData.IsDifferent (colorId) ? 350 : 340;
//		border.height = QXComData.IsDifferent (colorId) ? 470 : 460;

//		Debug.Log ("rewardData.xingJi:" + rewardData.xingJi);
		for (int i = 0;i < defaultStar;i ++)
		{
			GameObject star = (GameObject)Instantiate (starObj);

			star.SetActive (true);
			star.transform.parent = starObj.transform.parent;
			star.transform.localPosition = new Vector3(i * 35 - (defaultStar - 1) * 17.5f,0,0);
			star.transform.localScale = Vector3.one;
		}
		
		miBaoName.text = "[b]" + NameIdTemplate.GetName_By_NameId (nameId) + "[-]";
		
		miBaoObj.SetActive (true);
		ScaleEffect (miBaoObj,0.3f);
	}

	/// <summary>
	/// Ins it equip reward.
	/// </summary>
	void InItEquipReward ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
		                        IconSampleLoadCallBack);


	}
	void IconSampleLoadCallBack (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		GameObject iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive (true);
		iconSamplePrefab.transform.parent = equipObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (0, 20, 0);

		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardData.itemId);
		int nameId = commonTemp.nameId;
		effectId = QXComData.GetEffectColorByXmlColorId (commonTemp.color);

		cardLabel.text = NameIdTemplate.GetName_By_NameId (nameId);

		IconSampleManager iconSampleManager = iconSamplePrefab.GetComponent<IconSampleManager>();

		iconSampleManager.SetIconByID (rewardData.itemId);
		iconSampleManager.SetIconBasicDelegate (true,true,RewardHandlerClickBack);///////////////////////////////
		iconSampleManager.BgSprite.gameObject.SetActive (false);

		equipObj.SetActive (true);
		ScaleEffect (equipObj,0.3f);
	}
}

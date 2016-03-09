using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TBRewardCard : MonoBehaviour {
	
	private Award cardInfo;
	private Vector3 targetPos;

	public KmCardFly fly;

	private int rewardIndex;

	public UISprite cardBg;
	public GameObject cardTexObj;

	public UILabel cardLabel;
	
	private GameObject iconSamplePrefab;

	private string[] bgSpriteName = new string[]{"CardBg_back","CardBg"};

	private int pinZhiId;
	private int nameId;

	private float moveTime = 0.7f;

	/// <summary>
	/// Gets the card info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempAward">Temp award.</param>
	/// <param name="tempTargetPos">Temp target position.</param>
	public void GetCardInfo (Award tempAward,Vector3 tempTargetPos,int tempIndex)
	{
		cardInfo = tempAward;
		targetPos = tempTargetPos;
		rewardIndex = tempIndex;

		cardTexObj.SetActive (false);
		cardBg.spriteName = bgSpriteName[0];

		CardMove ();
	}

	void CardMove ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",Vector3.one);
		scale.Add ("time",moveTime);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		iTween.ScaleTo (gameObject,scale);
		
		Hashtable move = new Hashtable ();
		move.Add ("position",targetPos);
		move.Add ("time",moveTime);
		move.Add ("islocal",true);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("oncomplete","CardMoveEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (gameObject,move);
	}

	void CardMoveEnd ()
	{
		fly.CardTrans ();
		if (rewardIndex == 0)
		{
			CardBgRotate ();
		}
	}

	/// <summary>
	/// Cards the rotate.
	/// </summary>
	public void CardBgRotate ()
	{
		UIPlaySound playSound = this.GetComponent<UIPlaySound> ();
		playSound.Play ();

		QXComData.InstanceEffect (cardInfo.itemType == 4 ? QXComData.EffectPos.MID : QXComData.EffectPos.TOP,
		                          cardBg.gameObject,
		                          cardInfo.itemType == 4 ? 100155 : 100141);
		
		TBReward.tbReward.ItweenScale (1,"CardBgRotateEnd",gameObject);
	}

	void CardBgRotateEnd ()
	{
		cardBg.spriteName = bgSpriteName [1];
		cardTexObj.SetActive (true);

		InItCardInfo ();

		TBReward.tbReward.ItweenScale (2,"CardRotateEnd",gameObject);

		if (cardInfo.itemType == 4)
		{
			TBReward.tbReward.IsShowMiBao = true;
		}

		if (!TBReward.tbReward.IsShowMiBao)
		{
			//check card
			TBReward.tbReward.UnTurnIndex ++;
			TBReward.tbReward.CheckUnTurnRotate ();
		}
	}

	void CardRotateEnd ()
	{
		TBReward.tbReward.TurnIndex ++;
		TBReward.tbReward.CardInfoListControl (false,this.GetComponent<TBRewardCard> ());

		if (!TBReward.tbReward.IsShowMiBao)
		{
//			Debug.Log ("1");
			//产生特效
			InstanceEffect ();
			//check card
			TBReward.tbReward.CheckTurnRotate ();
		}
		else
		{
//			Debug.Log ("2");

			TBReward.tbReward.TurnListEffectControl (true);
			if (cardInfo.itemType == 4)
			{
				TBReward.tbReward.ShowMiBaoCard (cardInfo);
			}
		}
	}

	public void InstanceEffect ()
	{
		if (cardInfo.itemType == 4)
		{
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,cardBg.gameObject,100142 + QXComData.GetEffectColorByXmlColorId (pinZhiId));
			QXComData.InstanceEffect (QXComData.EffectPos.MID,cardBg.gameObject,100150 + QXComData.GetEffectColorByXmlColorId (pinZhiId));
		}
	}

	public void ClearEffect ()
	{
		QXComData.ClearEffect (cardBg.gameObject);
	}

	void InItCardInfo ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (cardInfo.itemId);
		nameId = commonTemp.nameId;
		pinZhiId = commonTemp.color;//0 | 1.2 | 3.4.5 | 6.7.8 | 9.10
//		Debug.Log ("pinZhiId:" + pinZhiId);
		if (cardInfo.itemType == 4)
		{
			cardLabel.text = NameIdTemplate.GetName_By_NameId (nameId);
		}
		else
		{
			string nameStr = "";
			if (cardInfo.itemType == 5)
			{
				List<char> nameCharList = new List<char>();
				for (int i = 0;i < NameIdTemplate.GetName_By_NameId (nameId).Length;i ++)
				{
					if (i < 5)
					{
						nameCharList.Add (NameIdTemplate.GetName_By_NameId (nameId)[i]);
					}
				}
				for (int i = 0;i < nameCharList.Count;i ++)
				{
					nameStr += nameCharList[i];
				}
				cardLabel.text = nameStr + "\n碎片x" + cardInfo.itemNumber;
			}
			else
			{
				nameStr = NameIdTemplate.GetName_By_NameId (nameId);
				cardLabel.text = nameStr + "\nx" + cardInfo.itemNumber;
			}
		}
		
		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			CardIconSample ();
		}
	}
	
	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive (true);
		iconSamplePrefab.transform.parent = cardTexObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (0, 20, 0);
		
		CardIconSample ();
	}
	void CardIconSample ()
	{
		//0普通道具;3当铺材料;4秘宝;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
		IconSampleManager iconSampleManager = iconSamplePrefab.GetComponent<IconSampleManager>();

		string itemName = NameIdTemplate.GetName_By_NameId (nameId);
		string mdesc = DescIdTemplate.GetDescriptionById(cardInfo.itemId);

		iconSampleManager.SetIconByID (cardInfo.itemId,"",2);
		iconSampleManager.SetIconBasicDelegate (true,true,null);///////////////////////////////
		iconSampleManager.BgSprite.gameObject.SetActive (false);
		iconSampleManager.SetIconPopText(cardInfo.itemId, itemName, mdesc, 1);
	}
}

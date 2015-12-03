using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TBCardInfo : MonoBehaviour {

	private enum ClickState
	{
		STATE_BEGIN,
		STATE_ROTATE_END,
	}
	private ClickState cState = ClickState.STATE_BEGIN;

	private Award awardInfo;

	public BoxCollider cardBox;
	public EventHandler cardHandler;

	public UISprite cardBg;
	public GameObject cardTexObj;

	public UISprite cardBorder;
	public UILabel cardLabel;

	private GameObject iconSamplePrefab;

	public UIWidget effectWidget;
	private int pinZhiId;

	private readonly Dictionary<int,int> colorDic = new Dictionary<int, int>()//0-0 | 1.2-1 | 3.4.5-2 | 6.7.8-3 | 9.10-4
	{
		{0,0},{1,1},{2,1},{3,2},{4,2},{5,2},{6,3},{7,3},{8,3},{9,4},{10,4}
	};

	private string[] bgSpriteName = new string[]{"CardBg_back","CardBg"};

	private TanBaoData.TanBaoType tbType;
	private Vector3 targetPos;

	private float itweenTime = 1f;

	/// <summary>
	/// Gets the TB card info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempAward">Temp award.</param>
	/// <param name="tempTargetPos">Temp target position.</param>
	public void GetTBCardInfo (TanBaoData.TanBaoType tempType,Award tempAward,Vector3 tempTargetPos)
	{
		ResetCard ();

		tbType = tempType;
		awardInfo = tempAward;
		targetPos = tempTargetPos;

		CardMove ();
	}

	/// <summary>
	/// Resets the card.
	/// </summary>
	void ResetCard ()
	{
		TanBaoReward.tbReward.blockHandler.m_handler -= CardBtnHandlerBack;
		cardTexObj.SetActive (false);
		cardBg.spriteName = bgSpriteName[0];
		cardBox.enabled = false;
		cardHandler.m_handler -= CardBtnHandlerBack;
		cState = ClickState.STATE_BEGIN;
	}

	/// <summary>
	/// Cards the move.
	/// </summary>
	void CardMove ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",Vector3.one);
		scale.Add ("time",itweenTime);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		iTween.ScaleTo (gameObject,scale);

		Hashtable move = new Hashtable ();
		move.Add ("position",targetPos);
		move.Add ("time",itweenTime);
		move.Add ("islocal",true);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("oncomplete","CardMoveEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (gameObject,move);
	}
	/// <summary>
	/// Cards the move end.
	/// </summary>
	void CardMoveEnd ()
	{
		InItCardInfo ();

		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
		{
			CardBtnHandlerBack (gameObject);

			if (awardInfo.itemType != 4)
			{
				cardHandler.m_handler += CardBtnHandlerBack;
			}
		}
		else
		{
			cardBox.enabled = true;
		}
	}
	/// <summary>
	/// Ins it card info.
	/// </summary>
	void InItCardInfo ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (awardInfo.itemId);
		int nameId = commonTemp.nameId;
		pinZhiId = commonTemp.color;//0 | 1.2 | 3.4.5 | 6.7.8 | 9.10
		cardBorder.spriteName = "pinzhi" + (pinZhiId - 1);

		if (awardInfo.itemType == 4)
		{
			cardLabel.text = NameIdTemplate.GetName_By_NameId (nameId);
		}
		else
		{
			string nameStr = "";
			if (awardInfo.itemType == 5)
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
				cardLabel.text = nameStr + "\n碎片x" + awardInfo.itemNumber;
			}
			else
			{
				nameStr = NameIdTemplate.GetName_By_NameId (nameId);
				cardLabel.text = nameStr + "\nx" + awardInfo.itemNumber;
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

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
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
		string itemName = "";
		if (awardInfo.itemType == 4)//秘宝
		{
			MiBaoXmlTemp mibaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (awardInfo.itemId);
			itemName = NameIdTemplate.GetName_By_NameId (mibaoTemp.nameId);
			iconSampleManager.SetIconType (IconSampleManager.IconType.MiBao);
		}
		else if (awardInfo.itemType == 5)//秘宝碎片 MibaoSuiPian表
		{
			MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (awardInfo.itemId);
			itemName = NameIdTemplate.GetName_By_NameId (miBaoSuiPian.m_name);
			iconSampleManager.SetIconType(IconSampleManager.IconType.MiBaoSuiPian);
		}
		else
		{
			ItemTemp item = ItemTemp.getItemTempById (awardInfo.itemId);
			itemName = NameIdTemplate.GetName_By_NameId (item.itemName);
			if (awardInfo.itemType == 0 || awardInfo.itemType == 3 || awardInfo.itemType == 6 || awardInfo.itemType == 9)
			{
				iconSampleManager.SetIconType(IconSampleManager.IconType.equipment);
			}
			else if (awardInfo.itemType == 7 || awardInfo.itemType == 8)
			{
				iconSampleManager.SetIconType(IconSampleManager.IconType.FuWen);
			}
		}
		
		iconSampleManager.SetIconBasic(1,awardInfo.itemId.ToString ());
		
		string mdesc = DescIdTemplate.GetDescriptionById(awardInfo.itemId);
		
		iconSampleManager.SetIconBasicDelegate (true,true,CardBtnHandlerBack);
		iconSampleManager.BgSprite.gameObject.SetActive (false);
		iconSampleManager.SetIconPopText(awardInfo.itemId, itemName, mdesc, 1);
	}

	/// <summary>
	/// Cards the button handler back.
	/// </summary>
	/// <param name="obj">Object.</param>
	void CardBtnHandlerBack (GameObject obj)
	{	
		switch (cState)
		{
		case ClickState.STATE_BEGIN:

			cardBox.enabled = false;
			TanBaoReward.tbReward.ItweenScale (new Vector3(0,1,1),0.15f,iTween.EaseType.linear,"CardBgScaleEnd",gameObject,gameObject);

			if (awardInfo.itemType == 4)
			{
				effectWidget.depth = -1;
				QXComData.InstanceEffect (QXComData.EffectPos.MID,effectWidget.gameObject,100155);

				if (tbType == TanBaoData.TanBaoType.TONGBI_SPEND || tbType == TanBaoData.TanBaoType.YUANBAO_SPEND)
				{
					//加入显示秘宝卡队列
					TanBaoReward.tbReward.AddAwardInToMiBaoList (awardInfo);
				}
			}
			else
			{
				effectWidget.depth = 2;
				QXComData.InstanceEffect (QXComData.EffectPos.TOP,effectWidget.gameObject,100141);
			}

			break;
		case ClickState.STATE_ROTATE_END:

			if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
			{
				TanBaoReward.tbReward.BlockController (false);
				ClearAllEffect ();
			}

			break;
		}
	}

	/// <summary>
	/// Cards the background scale end.
	/// </summary>
	void CardBgScaleEnd ()
	{
		cardBg.spriteName = bgSpriteName [1];
		cardTexObj.SetActive (true);

		TanBaoReward.tbReward.ItweenScale (Vector3.one,0.15f,iTween.EaseType.linear,"CardTexScaleEnd",gameObject,gameObject);
	}
	/// <summary>
	/// Cards the scale end.
	/// </summary>
	void CardTexScaleEnd ()
	{
		if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
		{
			if (awardInfo.itemType == 4)
			{
				//清除其它特效,显示秘宝卡
				ClearAllEffect ();

				TanBaoReward.tbReward.BlockController (false);
				TBMiBaoReward.tbMibaoReward.ShowMibaoReward (awardInfo,tbType);
			}
			else
			{
				cState = ClickState.STATE_ROTATE_END;
				TanBaoReward.tbReward.blockHandler.m_handler += CardBtnHandlerBack;
			}
		}
		else
		{
			TanBaoReward.tbReward.GetCardTurnEndNum ();
			if (awardInfo.itemType == 4)
			{
				QXComData.InstanceEffect (QXComData.EffectPos.TOP,cardBg.gameObject,100142 + colorDic[pinZhiId]);
				QXComData.InstanceEffect (QXComData.EffectPos.MID,cardBg.gameObject,100150 + colorDic[pinZhiId]);

				//开始显示秘宝卡
				TanBaoReward.tbReward.BlockController (false);
				TanBaoReward.tbReward.ShowMibaoCard ();
			}
		}
	}

	public void ClearAllEffect ()
	{
		QXComData.ClearEffect (effectWidget.gameObject);
		QXComData.ClearEffect (cardBg.gameObject);
		gameObject.SetActive (false);
	}
}

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

	private string[] bgSpriteName = new string[]{"CardBg_back","CardBg"};

	private TanBaoData.TanBaoType tbType;
	private Vector3 targetPos;

	private float itweenTime = 1f;
	
	public void GetTBCardInfo (TanBaoData.TanBaoType tempType,Award tempAward,Vector3 tempTargetPos)
	{
		cardTexObj.SetActive (false);
		cardBg.spriteName = bgSpriteName[0];
		cardBox.enabled = false;
		cardHandler.m_handler -= CardBtnHandlerBack;
		cState = ClickState.STATE_BEGIN;

		tbType = tempType;
		awardInfo = tempAward;
		targetPos = tempTargetPos;

		CardMoveItween ();
	}
	
	void CardMoveItween ()
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
		move.Add ("oncomplete","CardMoveItweenEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (gameObject,move);
	}

	void CardMoveItweenEnd ()
	{
		InItCardInfo ();
		cardBox.enabled = true;
		cardHandler.m_handler += CardBtnHandlerBack;
	}

	void InItCardInfo ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (awardInfo.itemId);
		int nameId = commonTemp.nameId;
		int pinZhiId = commonTemp.color;//0 | 1.2 | 3.4.5 | 6.7.8 | 9.10
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

	void CardBtnHandlerBack (GameObject obj)
	{	
		switch (cState)
		{
		case ClickState.STATE_BEGIN:

			cardBox.enabled = false;
			CardBgRotateTween ();

			if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
			{

			}
			else
			{

			}

			break;
		case ClickState.STATE_ROTATE_END:

			if (tbType == TanBaoData.TanBaoType.TONGBI_SINGLE || tbType == TanBaoData.TanBaoType.YUANBAO_SINGLE)
			{
				TanBaoReward.tbReward.BlockController (false);
				gameObject.SetActive (false);
			}
			else
			{
				
			}

			break;
		}
	}

	void CardBgRotateTween ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",new Vector3 (0,1,1));
		scale.Add ("time",0.15f);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete","CardBgRotateTweenEnd");
		scale.Add ("oncompletetarget",gameObject);
		iTween.ScaleTo (gameObject,scale);
	}
	void CardBgRotateTweenEnd ()
	{
		cardBg.spriteName = bgSpriteName [1];
		cardTexObj.SetActive (true);

		Hashtable scale = new Hashtable ();
		scale.Add ("scale",new Vector3 (1,1,1));
		scale.Add ("time",0.1f);
		scale.Add ("easetype",iTween.EaseType.easeInQuart);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete","CardRotateTweenEnd");
		scale.Add ("oncompletetarget",gameObject);
		iTween.ScaleTo (gameObject,scale);
	}
	void CardRotateTweenEnd ()
	{
		cState = ClickState.STATE_ROTATE_END;
		cardBox.enabled = true;

		TanBaoReward.tbReward.GetCardTurnEndNum ();
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GoodItem : MonoBehaviour {

	private DuiHuanInfo duiHuanInfo;
	private ShopData.ShopType shopType;

	private ShopGoodInfo goodInfo = new ShopGoodInfo();

	public UISprite moneyIcon;
	public UILabel moneyLabel;
	public UILabel desLabel;

	private bool isAllianceCanBuy;
	private bool isSparkEffect;

	private GameObject iconSamplePrefab;

	public GameObject saleOutObj;
	public GameObject soldOutSprite;
	public UILabel vipLabel;
	public EventHandler goodHandler;

	private int jieSuoVip;

	public GameObject tuiJianObj;

	/// <summary>
	/// Gets the dui huan info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempInfo">Temp info.</param>
	public void GetDuiHuanInfo (ShopData.ShopType tempType,DuiHuanInfo tempInfo)
	{
		shopType = tempType;
		duiHuanInfo = tempInfo;
//		Debug.Log ("tempInfo.isChange:" + tempInfo.isChange + "||" + tempInfo.site);
		goodInfo.xmlId = tempInfo.id;

		desLabel.text = "";
		moneyIcon.transform.parent.gameObject.SetActive (true);

		jieSuoVip = 0;

		bool tuiJian = false;

		switch (tempType)
		{
		case ShopData.ShopType.ORDINARY:
		{
			DangpuItemCommonTemplate template = DangpuItemCommonTemplate.getDangpuItemCommonById (tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = (QXComData.MoneyType)Enum.ToObject (typeof (QXComData.MoneyType),template.needType);
			goodInfo.countBuyTime = tempInfo.remainCount;

			jieSuoVip = template.VIP;
			tuiJian = template.ifRecomanded == 0 ? false : true;

			break;
		}
		case ShopData.ShopType.MYSTRERT:
		{
			DangPuTemplate template = DangPuTemplate.getDangpuTemplateById(tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = (QXComData.MoneyType)Enum.ToObject (typeof (QXComData.MoneyType),template.needType);

			jieSuoVip = template.VIP;
			tuiJian = template.ifRecomanded == 0 ? false : true;

			break;
		}
		case ShopData.ShopType.WEIWANG:
		{
			DuiHuanTemplete template = DuiHuanTemplete.getDuiHuanTemplateById (tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = ShopData.Instance.MoneyType (tempType);

			tuiJian = template.ifRecomanded == 0 ? false : true;

			break;
		}
		case ShopData.ShopType.GONGXIAN:
		{
			LMDuiHuanTemplate template = LMDuiHuanTemplate.getLMDuiHuanTemplateById (tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = ShopData.Instance.MoneyType (tempType);
			goodInfo.site = template.site;
			goodInfo.countBuyTime = tempInfo.remainCount;

			isAllianceCanBuy = ShopData.Instance.GetAllianceLevel () >= template.needLv ? true : false;
//			Debug.Log ("goodInfo.itemId:" + goodInfo.itemId);
			desLabel.text = isAllianceCanBuy ? "" : MyColorData.getColorString (5,"商铺升至" + template.needLv + "级可购买");
			moneyIcon.transform.parent.gameObject.SetActive (isAllianceCanBuy);

			tuiJian = template.ifRecomanded == 0 ? false : true;

			break;
		}
		case ShopData.ShopType.GONGXUN:
		{
			LMFightDuiHuanTemplate template = LMFightDuiHuanTemplate.getLMFightDuiHuanTemplateById (tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = ShopData.Instance.MoneyType (tempType);

			break;
		}
		case ShopData.ShopType.HUANGYE:
		{
			HuangYeDuiHuanTemplate template = HuangYeDuiHuanTemplate.getHuangYeDuiHuanTemplateById (tempInfo.id);
			goodInfo.itemId = template.itemId;
			goodInfo.itemType = template.itemType;
			goodInfo.itemName = NameIdTemplate.GetName_By_NameId (template.itemId);
			goodInfo.itemNum = template.itemNum;
			goodInfo.needMoney = template.needNum;
			goodInfo.moneyType = ShopData.Instance.MoneyType (tempType);

			tuiJian = template.ifRecomanded == 0 ? false : true;

			break;
		}
		default:
			break;
		}

		tuiJianObj.SetActive (shopType == ShopData.ShopType.GONGXIAN ? (isAllianceCanBuy ? tuiJian : false) : tuiJian);

		goodInfo.site = tempInfo.site;

		moneyIcon = QXComData.MoneySprite (goodInfo.moneyType,moneyIcon);
		moneyLabel.text = goodInfo.needMoney.ToString ();
		saleOutObj.SetActive ((!tempInfo.isChange || JunZhuData.Instance().m_junzhuInfo.vipLv < jieSuoVip) ? true : false);

		soldOutSprite.SetActive (!tempInfo.isChange);

		isSparkEffect = (!tempInfo.isChange || JunZhuData.Instance ().m_junzhuInfo.vipLv < jieSuoVip) ? 
			false : (shopType == ShopData.ShopType.GONGXIAN ? isAllianceCanBuy : true);

		vipLabel.gameObject.SetActive (JunZhuData.Instance().m_junzhuInfo.vipLv < jieSuoVip ? true : false);
		vipLabel.text = MyColorData.getColorString (1,JunZhuData.Instance().m_junzhuInfo.vipLv < jieSuoVip ? "V" + jieSuoVip + "解锁" : "");

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        LoadIconSamplePrefab);
		}
		else
		{
			InItIconSamplePrefab ();
		}

		goodHandler.m_click_handler -= ClickItem;
		goodHandler.m_click_handler += ClickItem;
	}

	void LoadIconSamplePrefab (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = this.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (0,25,0);

		InItIconSamplePrefab ();
	}
	void InItIconSamplePrefab ()
	{
		//0普通道具;3当铺材料;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
		IconSampleManager iconSample = iconSamplePrefab.GetComponent<IconSampleManager>();

		iconSample.SetIconByID (goodInfo.itemId,"x" + goodInfo.itemNum,1,false,isSparkEffect);

		string mdesc = DescIdTemplate.GetDescriptionById(goodInfo.itemId);
		
		iconSample.SetIconBasicDelegate (false,true,ClickItem);
//		iconSample.SetIconPopText(goodInfo.itemId, goodInfo.itemName, mdesc, 1);
//		iconSamplePrefab.transform.localScale = Vector3.one * 0.9f;

		//if shopType == GongXian : SetBgColor to grey
		UISprite[] sprites = this.GetComponentsInChildren<UISprite> ();
		foreach (UISprite sprite in sprites)
		{
			sprite.color = shopType == ShopData.ShopType.GONGXIAN ? (isAllianceCanBuy ? Color.white : Color.black) : Color.white;
		}
	}
	
	void ClickItem (GameObject obj)
	{
		if (ShopPage.shopPage.clickTime > 0)
		{
			return;
		}
		if (duiHuanInfo.isChange && JunZhuData.Instance().m_junzhuInfo.vipLv >= jieSuoVip)
		{
			if (shopType == ShopData.ShopType.GONGXIAN)
			{
				if (isAllianceCanBuy)
				{
					ShopPage.shopPage.OpenShopBuyWindow (goodInfo);
				}
			}
			else
			{
				ShopPage.shopPage.OpenShopBuyWindow (goodInfo);
			}
		}
	}
}

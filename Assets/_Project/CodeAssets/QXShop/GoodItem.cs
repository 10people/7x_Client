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

	private GameObject iconSamplePrefab;

	public GameObject saleOutObj;
	public EventHandler goodHandler;

	/// <summary>
	/// Gets the dui huan info.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempInfo">Temp info.</param>
	public void GetDuiHuanInfo (ShopData.ShopType tempType,DuiHuanInfo tempInfo)
	{
		shopType = tempType;
		duiHuanInfo = tempInfo;

		goodInfo.xmlId = tempInfo.id;

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

			break;
		}
		default:
			break;
		}

		moneyIcon = QXComData.MoneySprite (goodInfo.moneyType,moneyIcon);
		moneyLabel.text = goodInfo.needMoney.ToString ();
		saleOutObj.SetActive (tempInfo.isChange ? false : true);

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        LoadIconSamplePrefab);
		}
		else
		{
			InItIconSamplePrefab ();
		}

		goodHandler.m_handler -= ClickItem;
		goodHandler.m_handler += ClickItem;
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

		if (goodInfo.itemType == 5)//秘宝碎片 MibaoSuiPian表
		{
			MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (goodInfo.itemId);
			iconSample.SetIconType(IconSampleManager.IconType.MiBaoSuiPian);
		}
		else
		{
			ItemTemp item = ItemTemp.getItemTempById (goodInfo.itemId);
			if (goodInfo.itemType == 0 || goodInfo.itemType == 3 || goodInfo.itemType == 6 || goodInfo.itemType == 9)
			{
				iconSample.SetIconType(IconSampleManager.IconType.equipment);
			}
			else if (goodInfo.itemType == 7 || goodInfo.itemType == 8)
			{
				iconSample.SetIconType(IconSampleManager.IconType.FuWen);
			}
		}

		iconSample.SetIconBasic(0,goodInfo.itemId.ToString (),"x" + goodInfo.itemNum.ToString ());
		
		string mdesc = DescIdTemplate.GetDescriptionById(goodInfo.itemId);
		
		iconSample.SetIconBasicDelegate (true,true,ClickItem);
		iconSample.SetIconPopText(goodInfo.itemId, goodInfo.itemName, mdesc, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.9f;
	}
	
	void ClickItem (GameObject obj)
	{
		if (duiHuanInfo.isChange)
		{
			ShopPage.shopPage.OpenShopBuyWindow (goodInfo);
		}
	}
}

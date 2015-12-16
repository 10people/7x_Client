using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopBuyWindow : MonoBehaviour {

	public static ShopBuyWindow shopBuyWindow;

	private ShopGoodInfo goodInfo;

	public GameObject windowObj;
	
	public UISprite moneyIcon;
	public UILabel nameLabel;
	public UILabel desLabel;
	public UILabel numLabel;
	public UILabel needMoney;

	private string desText;

	public List<EventHandler> buyHandlerList = new List<EventHandler> ();

	private GameObject iconSamplePrefab;

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		shopBuyWindow = this;
	}

	/// <summary>
	/// Gets the buy good info.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void GetBuyGoodInfo (ShopGoodInfo tempInfo)
	{
		sEffectController.OnOpenWindowClick ();

		goodInfo = tempInfo;

		moneyIcon = QXComData.MoneySprite (tempInfo.moneyType,moneyIcon);
		numLabel.text = "购买" + tempInfo.itemNum + "件";
		needMoney.text = tempInfo.needMoney.ToString ();
		nameLabel.text = tempInfo.itemName;

		desText = DescIdTemplate.GetDescriptionById(goodInfo.itemId);
		desLabel.text = desText;

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        LoadIconSamplePrefab);
		}
		else
		{
			InItIconSamplePrefab ();
		}

		foreach (EventHandler handler in buyHandlerList)
		{
			handler.m_handler -= BuyHandlerClickBack;
			handler.m_handler += BuyHandlerClickBack;
		}
	}

	void LoadIconSamplePrefab (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = nameLabel.transform.parent;
		iconSamplePrefab.transform.localPosition = new Vector3 (-190,10,0);
		
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
		
		iconSample.SetIconBasic(3,goodInfo.itemId.ToString ());

		iconSample.SetIconPopText(goodInfo.itemId, goodInfo.itemName, desText, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.85f;
	}

	void BuyHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "CancelBtn":
			break;
		case "SureBtn":

			ShopData.Instance.ShopGoodsBuyReq (goodInfo);

			break;
		default:
			break;
		}
		gameObject.SetActive (false);
	}
}

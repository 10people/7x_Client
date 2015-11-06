using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralBuyWindow : MonoBehaviour {

	public GameObject generalStoreObj;

	public GameObject iconSampleParent;
	private GameObject iconSamplePrefab;
	private int itemType;//物品类型
	private int itemId;//物品id

	public UILabel numLabel;

	private string itemName;//物品名字
	public UILabel nameLabel;

	private string mdesc;
	public UILabel desLabel;

	public UILabel needMoneyLabel;
	public UISprite moneyIcon;

	public List<EventHandler> btnHandlerList = new List<EventHandler> ();

	public ScaleEffectController m_ScaleEffectController;

	/// <summary>
	/// Gets the goods info.
	/// </summary>
	/// <param name="tempStoreType">商铺类型.</param>
	/// <param name="tempItemType">物品类型.</param>
	/// <param name="tempItemId">物品id.</param>
	/// <param name="tempNum">物品数量.</param>
	/// <param name="tempItemName">物品名字.</param>
	/// <param name="tempNeedMoney">T购买物品需要的钱币数量.</param>
	public void GetGoodsInfo (GeneralControl.StoreType tempStoreType,int tempItemType,int tempItemId,int tempNum,string tempItemName,int tempNeedMoney)
	{
		m_ScaleEffectController.OnOpenWindowClick ();

		itemType = tempItemType;
		itemId = tempItemId;
		itemName = tempItemName;

		mdesc = DescIdTemplate.GetDescriptionById(itemId);
		desLabel.text = mdesc;

		nameLabel.text = tempItemName;
		numLabel.text = MyColorData.getColorString (1,"购买" + tempNum + "件");
		needMoneyLabel.text = MyColorData.getColorString (1,tempNeedMoney.ToString ());

		GeneralStore store = generalStoreObj.GetComponent<GeneralStore> ();
		moneyIcon.spriteName = store.IconName (tempStoreType);

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack(ref tempWww, null, iconSamplePrefab);
		}

		btnHandlerList [0].m_handler += Back;
		btnHandlerList [1].m_handler += Close;
	}

	void Back (GameObject obj)
	{
		GeneralStore store = generalStoreObj.GetComponent<GeneralStore> ();
		store.DuiHuanReqBack (1);
		Destroy (gameObject);
	}

	void Close (GameObject obj)
	{
		GeneralStore store = generalStoreObj.GetComponent<GeneralStore> ();
		store.DuiHuanReqBack (2);
		Destroy (gameObject);
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (iconSamplePrefab == null)
		{
			iconSamplePrefab = p_object as GameObject;
		}
		
		GameObject iconSample = (GameObject)Instantiate (iconSamplePrefab);
		
		iconSample.SetActive(true);
		iconSample.transform.parent = iconSampleParent.transform;
		iconSample.transform.localPosition = new Vector3 (-190,10,0);

		//0普通道具;3当铺材料;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
		IconSampleManager fuShiIconSample = iconSample.GetComponent<IconSampleManager>();
		if (itemType == 5)//秘宝碎片 MibaoSuiPian表
		{
			MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (itemId);
			itemName = NameIdTemplate.GetName_By_NameId (miBaoSuiPian.m_name);
			fuShiIconSample.SetIconType(IconSampleManager.IconType.MiBaoSuiPian);
		}
		else
		{
			ItemTemp item = ItemTemp.getItemTempById (itemId);
			itemName = NameIdTemplate.GetName_By_NameId (item.itemName);
			if (itemType == 0 || itemType == 3 || itemType == 6 || itemType == 9)
			{
				fuShiIconSample.SetIconType(IconSampleManager.IconType.equipment);
			}
			else if (itemType == 7 || itemType == 8)
			{
				fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
			}
		}
		
		fuShiIconSample.SetIconBasic(3,itemId.ToString ());

		fuShiIconSample.SetIconPopText(itemId, itemName, mdesc, 1);
		iconSample.transform.localScale = Vector3.one * 0.85f;
	}
}

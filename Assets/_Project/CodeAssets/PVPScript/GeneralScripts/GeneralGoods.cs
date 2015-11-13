using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralGoods : MonoBehaviour {

	private DuiHuanInfo duiHuanInfo;

	private GameObject iconSamplePrefab;
	public GameObject saleEnd;
	private int itemType;//物品类型
	private int itemId;//物品id
	private int num;//物品数量

	public UILabel needMoneyLabel;
	private int needMoney;//需要钱币数量
	public UISprite moneyIcon;
	
	private string itemName;//物品名字

	private GeneralControl.StoreType storeType;//商铺类型

	private List<GameObject> iconSampleList = new List<GameObject> ();

	public GameObject generalStoreObj;

	/// <summary>
	/// Ins it goods info.
	/// </summary>
	/// <param name="tempType">商铺类型</param>
	/// <param name="tempInfo">物品信息</param>
	public void InItGoodsInfo (GeneralControl.StoreType tempStoreType,DuiHuanInfo tempInfo)
	{
		duiHuanInfo = tempInfo;

		storeType = tempStoreType;

		switch (tempStoreType)
		{
		case GeneralControl.StoreType.PVP:
		{
			DuiHuanTemplete duiHuanTemp = DuiHuanTemplete.getDuiHuanTemplateById (tempInfo.id);
			itemType = duiHuanTemp.itemType;//物品类型
			itemId = duiHuanTemp.itemId;//物品id
			needMoney = duiHuanTemp.needNum;
			needMoneyLabel.text = duiHuanTemp.needNum.ToString ();
			num = duiHuanTemp.itemNum;

			break;
		}
		case GeneralControl.StoreType.HUANGYE:
		{
			HuangYeDuiHuanTemplate duiHuanTemp = HuangYeDuiHuanTemplate.getHuangYeDuiHuanTemplateById (tempInfo.id);
			itemType = duiHuanTemp.itemType;//物品类型
			itemId = duiHuanTemp.itemId;//物品id
			needMoney = duiHuanTemp.needNum;
			needMoneyLabel.text = duiHuanTemp.needNum.ToString ();
			num = duiHuanTemp.itemNum;

			break;
		}
		case GeneralControl.StoreType.ALLANCE:
		{
			LMDuiHuanTemplate duiHuanTemp = LMDuiHuanTemplate.getLMDuiHuanTemplateById (tempInfo.id);
			itemType = duiHuanTemp.itemType;//物品类型
			itemId = duiHuanTemp.itemId;//物品id
			needMoney = duiHuanTemp.needNum;
			needMoneyLabel.text = duiHuanTemp.needNum.ToString ();
			num = duiHuanTemp.itemNum;

			break;
		}
		case GeneralControl.StoreType.ALLIANCE_FIGHT:
		{
			LMFightDuiHuanTemplate duiHuanTemp = LMFightDuiHuanTemplate.getLMFightDuiHuanTemplateById (tempInfo.id);
			itemType = duiHuanTemp.itemType;//物品类型
			itemId = duiHuanTemp.itemId;//物品id
			needMoney = duiHuanTemp.needNum;
			needMoneyLabel.text = duiHuanTemp.needNum.ToString ();
			num = duiHuanTemp.itemNum;

			break;
		}
		default:
			break;
		}

		GeneralStore store = generalStoreObj.GetComponent<GeneralStore> ();

		moneyIcon.spriteName = store.IconName (tempStoreType);

//		Debug.Log ("isChange:" + tempInfo.isChange);
		saleEnd.SetActive (tempInfo.isChange ? false : true);
		this.gameObject.GetComponent<BoxCollider> ().enabled = tempInfo.isChange ? true : false;

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

		this.gameObject.GetComponent<UIDragScrollView> ().enabled = !QXComData.CheckYinDaoOpenState (100200);
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (iconSamplePrefab == null)
		{
			iconSamplePrefab = p_object as GameObject;
		}
		foreach (GameObject obj in iconSampleList)
		{
			Destroy (obj);
		}
		iconSampleList.Clear ();

		GameObject iconSample = (GameObject)Instantiate (iconSamplePrefab);
		
		iconSample.SetActive(true);
		iconSample.transform.parent = this.transform;
		iconSample.transform.localPosition = new Vector3 (0,15,0);
		iconSampleList.Add (iconSample);
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

		fuShiIconSample.SetIconBasic(4,itemId.ToString (),"x" + num.ToString ());

		string mdesc = DescIdTemplate.GetDescriptionById(itemId);

		fuShiIconSample.SetIconBasicDelegate (true,true,ClickItem);
		fuShiIconSample.SetIconPopText(itemId, itemName, mdesc, 1);
		iconSample.transform.localScale = Vector3.one * 0.8f;

		iconSample.GetComponent<UIDragScrollView> ().enabled = !QXComData.CheckYinDaoOpenState (100200);
	}

	void OnClick ()
	{
		ClickItem (gameObject);
	}

	void ClickItem (GameObject obj)
	{
		GeneralStore store = generalStoreObj.GetComponent<GeneralStore> ();
		Debug.Log ("store.IsDuiHuan:" + store.IsDuiHuan);
		if (duiHuanInfo.isChange)
		{
			if (!store.IsDuiHuan)
			{
				store.IsDuiHuan = true;
				store.DuiHuanReq (duiHuanInfo,itemId,needMoney,itemName,itemType,num);
				
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,4);
			}
		}
	}
}

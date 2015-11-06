using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuShiOperate : MonoBehaviour {

	private Fuwen fuWenInfo;
	private int iconId;
	private int lanWeiId;

	public enum OperateType
	{
		XIANGQIAN,
		HECHENG,
	}

	private GameObject iconSamplePrefab;

	public GameObject fuShiInfoObj;

	public UILabel nameLabel;
	public UILabel numLabel;
	public UILabel desLabel;
	public UILabel shuXingLabel;

	public UISprite lockSprite;//锁定icon

	public GameObject xiangQianBtnsObj;
	public GameObject heChengBtnsObj;

	public List<EventHandler> btnHandlerList = new List<EventHandler> ();

	/// <summary>
	/// 获得符石信息
	/// </summary>
	/// <param name="tempType">操作类型（镶嵌页，合成页）</param>
	/// <param name="tempFuWenInfo">符石信息</param>
	/// <param name="tempLanWeiInfo">栏位信息</param>
	public void GetOperateInfo (OperateType tempType,int tempItemId,int tempLanWeiId)
	{
		lanWeiId = tempLanWeiId;

		List<Fuwen> fuWenList = FuWenData.Instance.fuWenDataResp.fuwens;
		for (int i = 0;i < fuWenList.Count;i ++)
		{
			if (fuWenList[i].itemId == tempItemId)
			{
				fuWenInfo = fuWenList[i];
				break;
			}
		}

		if (fuWenInfo == null)
		{
			fuWenInfo = new Fuwen();
			fuWenInfo.itemId = tempItemId;
			fuWenInfo.cnt = 1;
			fuWenInfo.isLock = 2;
		}

		lockSprite.gameObject.SetActive (fuWenInfo.isLock == 1 ? true : false);

		xiangQianBtnsObj.SetActive (tempType == OperateType.XIANGQIAN ? true : false);
		heChengBtnsObj.SetActive (tempType == OperateType.HECHENG ? true : false);
		
		btnHandlerList[5].gameObject.SetActive (tempType == OperateType.XIANGQIAN ? true : false);//关闭按钮

		numLabel.gameObject.SetActive (tempType == OperateType.HECHENG ? true : false);

		switch (tempType)
		{
		case OperateType.XIANGQIAN:
		{
			BoxCollider heBtnBox = btnHandlerList[0].GetComponent<BoxCollider> ();
			heBtnBox.enabled = fuWenInfo.cnt >= 4 && fuWenInfo.isLock == 2 ? true : false;
			UISprite heBtnSprite = btnHandlerList[0].GetComponent<UISprite> ();//合成按钮
			heBtnSprite.color = fuWenInfo.cnt >= 4 && fuWenInfo.isLock == 2 ? Color.white : Color.gray;
			UILabel heBtnLabel = btnHandlerList[0].GetComponentInChildren<UILabel> ();
			heBtnLabel.color = fuWenInfo.cnt >= 4 && fuWenInfo.isLock == 2 ? Color.white : Color.gray;

			break;
		}
		case OperateType.HECHENG:
		{
			btnHandlerList[3].gameObject.SetActive (fuWenInfo.isLock == 2 ? true : false);//锁定按钮 2-未锁定状态
			btnHandlerList[4].gameObject.SetActive (fuWenInfo.isLock == 1 ? true : false);//解锁按钮 1-锁定状态

			break;
		}
		default:
			break;
		}

		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler += HandlerCallBack;
		}

		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (fuWenInfo.itemId);
		nameLabel.text = NameIdTemplate.GetName_By_NameId (fuWenTemp.name);
		desLabel.text = NameIdTemplate.GetName_By_NameId (fuWenTemp.shuXingName);
		
		numLabel.text = "x" + fuWenInfo.cnt;
		shuXingLabel.text = "+" + fuWenTemp.shuxingValue;
		
		iconId = fuWenTemp.icon;

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
			fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
			fuShiIconSample.SetIconBasic(4,iconId.ToString ());

			iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);

		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = fuShiInfoObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (-145,0,0);

		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
		fuShiIconSample.SetIconBasic(4,iconId.ToString ());

		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
	}

	void HandlerCallBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "HeChengBtn":
			//发送合成符石请求
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.GENERAL_HECHENG,fuWenInfo.itemId,lanWeiId);
			DestroyOperateWindow ();
			break;
		case "RemoveBtn":
			//发送卸下符石请求
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.REMOVE_FUWEN,fuWenInfo.itemId,lanWeiId);
			DestroyOperateWindow ();
			break;
		case "UnLockBtn":
			//发送解锁符石请求
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.UNLOCK,fuWenInfo.itemId,lanWeiId);
			DestroyOperateWindow ();
			break;
		case "LockBtn":
			//发送锁定符石请求
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.LOCK,fuWenInfo.itemId,lanWeiId);
			DestroyOperateWindow ();
			break;
		case "CancelBtn":
			FuWenMainPage.fuWenMainPage.IsBtnClick = false;
			DestroyOperateWindow ();
			break;
		case "CloseBtn":
			FuWenMainPage.fuWenMainPage.IsBtnClick = false;
			DestroyOperateWindow ();
			break;
		default:
			break;
		}
	}

	void DestroyOperateWindow ()
	{
//		Destroy (this.gameObject);
		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler -= HandlerCallBack;
		}
		fuWenInfo = null;
		gameObject.SetActive (false);
	}
}

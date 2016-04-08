using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenSelectItem : MonoBehaviour {

	private Fuwen fuWenInfo;
	private FuwenLanwei lanWeiInfo;
	private FuWenSelect.SelectType selectType;

	public UISprite itemBg;
	public UILabel nameLabel;
	public UILabel numLabel;
	public UILabel desLabel;
	public UILabel shuXingLabel;
	private GameObject iconSamplePrefab;

	public List<EventHandler> operateHandlerList = new List<EventHandler> ();

	public UILabel selectBtnLabel;

	private string nameStr;
	private bool isCurFuWen = false;

	private int fuWenCnt;

	//获得符文信息
	public void GetFuWenInfo (FuWenSelect.SelectType tempType,Fuwen tempInfo,FuwenLanwei tempLanWeiInfo)
	{
		fuWenInfo = tempInfo;
		lanWeiInfo = tempLanWeiInfo;
		selectType = tempType;

		fuWenCnt = tempInfo.cnt + (tempLanWeiInfo.itemId == tempInfo.itemId ? 1 : 0);

		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (tempInfo.itemId);

		nameStr = NameIdTemplate.GetName_By_NameId (fuWenTemp.name);
		nameLabel.text = MyColorData.getColorString (3,nameStr);
		desLabel.text = FuWenData.Instance.colorCode + NameIdTemplate.GetName_By_NameId (fuWenTemp.shuXingName) + "[-]";
		
		numLabel.text = MyColorData.getColorString (3,"x" + fuWenCnt);
		shuXingLabel.text = FuWenData.Instance.colorCode + "+" + fuWenTemp.shuxingValue  + "[-]";

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}

		isCurFuWen = fuWenInfo.itemId == lanWeiInfo.itemId ? true : false;
		bool isCanMix = fuWenCnt >= 4 && fuWenInfo.isLock == 2;

		itemBg.color = isCurFuWen ? QXComData.lightColor : Color.white;
		//选择框
		operateHandlerList [0].gameObject.SetActive (tempType == FuWenSelect.SelectType.HECHENG ? true : false);
		selectBtnLabel.text = tempType == FuWenSelect.SelectType.HECHENG ? (isCurFuWen ? "当前选择" : "选择") : "";
		//卸下按钮
		operateHandlerList [2].gameObject.SetActive (tempType == FuWenSelect.SelectType.XIANGQIAN ? (isCurFuWen ? (fuWenTemp.fuwenLevel < 11 ? (isCanMix ? false : true) : true) : false) : false);
		//合成按钮
		operateHandlerList [1].gameObject.SetActive (tempType == FuWenSelect.SelectType.XIANGQIAN ? (isCurFuWen ? (fuWenTemp.fuwenLevel < 11 ? (isCanMix ? true : false) : false) : false) : false);
		//镶嵌按钮
		operateHandlerList [3].gameObject.SetActive (tempType == FuWenSelect.SelectType.XIANGQIAN ? (isCurFuWen ? false : true) : false);

		foreach (EventHandler handler in operateHandlerList)
		{
			handler.m_click_handler -= OperateHandlerClickBack;
			handler.m_click_handler += OperateHandlerClickBack;
		}

		this.gameObject.GetComponent<UIDragScrollView> ().enabled = QXComData.CheckYinDaoOpenState (100470) ? false : true;
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);

		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = this.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (-175,0,0);

		InItIconSample ();
	}

	void InItIconSample ()
	{
		string mdesc = DescIdTemplate.GetDescriptionById (fuWenInfo.itemId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (fuWenInfo.itemId,"",3);
		fuShiIconSample.SetIconPopText(fuWenInfo.itemId, nameStr, mdesc, 1);
		
		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
	}

	void OperateHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "MixBtn":

			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.GENERAL_HECHENG,fuWenInfo.itemId,lanWeiInfo.lanweiId);

			break;
		case "RemoveBtn":

			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.REMOVE_FUWEN,fuWenInfo.itemId,lanWeiInfo.lanweiId);

			break;
		case "SelectBtn":

			FuWenMainPage.fuWenMainPage.CurHeChengItemId = fuWenInfo.itemId;
			FuWenMainPage.fuWenMainPage.ShowMixBtns ();
			FuWenMainPage.fuWenMainPage.FxController (FuWenMixBtn.FxType.OPEN);
			FuWenMainPage.fuWenMainPage.EffectPanel (true);

			break;
		case "EquipBtn":

			if (QXComData.CheckYinDaoOpenState (100470))
			{
				UIYindao.m_UIYindao.CloseUI ();
			}

			//发送镶嵌符石请求
			if (lanWeiInfo.itemId > 0)
			{
				FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.REMOVE_FUWEN,fuWenInfo.itemId,lanWeiInfo.lanweiId);
			}
			FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.EQUIP_FUWEN,fuWenInfo.itemId,lanWeiInfo.lanweiId);

			break;
		default:
			break;
		}

		FuWenSelect.fuWenSelect.CloseBtn (gameObject);
	}
}

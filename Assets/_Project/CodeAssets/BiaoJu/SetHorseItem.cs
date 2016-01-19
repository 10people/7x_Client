using UnityEngine;
using System.Collections;

public class SetHorseItem : MonoBehaviour {

	private BiaoJuHorseInfo horseInfo;

	public UISprite horseIcon;
	public UISprite border;

	public GameObject point;
	public GameObject line;
	public GameObject desLabel;

	public UILabel shouYiLabel;
	public UILabel costLabel;
	public EventHandler buyBtn;
	public UILabel vipLevel;

	private int costYuanBao;

	public GameObject selectBox;

	private string textStr;

	public void InItSetHorseItem (BiaoJuHorseInfo tempInfo,int curId)
	{
		horseInfo = tempInfo;

		horseIcon.spriteName = BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,2);
		border.spriteName = BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,3);

		selectBox.SetActive (tempInfo.horseId == curId ? true : false);

		line.SetActive (tempInfo.horseId == 2 ? false : true);

		point.SetActive (tempInfo.horseId <= curId ? true : false);
		point.transform.localPosition = new Vector3 (float.Parse (BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,4)),0,0);

		desLabel.SetActive (tempInfo.horseId == curId ? true : false);
		costLabel.gameObject.SetActive (tempInfo.horseId > curId ? true : false);

		shouYiLabel.text = tempInfo.shouYi.ToString ();

		CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (BiaoJuPage.bjPage.CurHorseLevel);
		costYuanBao = tempInfo.upNeedMoney - cartTemp.ShengjiCost;
		costLabel.text = costYuanBao.ToString ();

		if (tempInfo.horseId > curId)
		{
//			UISprite btnSprite = buyBtn.GetComponent<UISprite> ();
//			btnSprite.color = tempInfo.needVipLevel < JunZhuData.Instance ().m_junzhuInfo.vipLv ? Color.white : Color.gray;
//			UIWidget btnLabel = buyBtn.GetComponentInChildren<UIWidget> ();
//			btnLabel.color = tempInfo.needVipLevel < JunZhuData.Instance ().m_junzhuInfo.vipLv ? Color.white : Color.gray;

			vipLevel.text = tempInfo.needVipLevel <= JunZhuData.Instance ().m_junzhuInfo.vipLv ? "" : "VIP" + tempInfo.needVipLevel + "可购买";
		}

		buyBtn.m_handler -= BuyBtnHandlerClickBack;
		buyBtn.m_handler += BuyBtnHandlerClickBack;
	}

	void BuyBtnHandlerClickBack (GameObject obj)
	{
		if (horseInfo.needVipLevel <= JunZhuData.Instance ().m_junzhuInfo.vipLv)
		{
			if (costYuanBao > JunZhuData.Instance ().m_junzhuInfo.yuanBao)
			{
//				textStr = "元宝不足！是否跳转到充值？";
//				QXComData.CreateBox (1,textStr,false,null);
				SetHorseWindow.setHorse.CloseSetHorseWindow (gameObject);
				BiaoJuData.Instance.TurnToVip ();
			}
			else
			{
				BiaoJuData.Instance.UpHorseReq (horseInfo.horseId);
			}
		}
		else
		{
            EquipSuoData.TopUpLayerTip(null, false, 0, "VIP等级不足！是否跳转到充值？");
			//textStr = "VIP等级不足！是否跳转到充值？";
			//QXComData.CreateBox (1,textStr,false,LackVipLevel);
			//SetHorseWindow.setHorse.CloseSetHorseWindow (gameObject);
		}
	}

	void LackVipLevel (int i)
	{
		if (i == 2)
		{
			BiaoJuPage.bjPage.CloseBiaoJu ();
			TopUpLoadManagerment.LoadPrefab();
		}
	}
}

﻿using UnityEngine;
using System.Collections;

namespace Carriage
{
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
//			Debug.Log ("BiaoJuPage.bjPage.CurHorseLevel:" + BiaoJuPage.bjPage.CurHorseLevel);
			CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (BiaoJuPage.bjPage.CurHorseLevel);
//			Debug.Log ("tempInfo.upNeedMoney:" + tempInfo.upNeedMoney);
//			Debug.Log ("cartTemp.ShengjiCost:" + cartTemp.ShengjiCost);
			costYuanBao = tempInfo.upNeedMoney - cartTemp.ShengjiCost;
			costLabel.text = costYuanBao.ToString ();

			if (tempInfo.horseId > curId)
			{
	 			vipLevel.text = tempInfo.needVipLevel <= JunZhuData.Instance().m_junzhuInfo.vipLv ? "" : "VIP" + tempInfo.needVipLevel + "可购买";
			}

			buyBtn.m_click_handler -= BuyBtnHandlerClickBack;
			buyBtn.m_click_handler += BuyBtnHandlerClickBack;
		}

		void BuyBtnHandlerClickBack (GameObject obj)
		{
			BiaoJuData.Instance.UpHorseReq (horseInfo.horseId);
//			if (horseInfo.needVipLevel <= JunZhuData.Instance().m_junzhuInfo.vipLv)
//			{
//				if (costYuanBao > JunZhuData.Instance().m_junzhuInfo.yuanBao)
//				{
//					SetHorseWindow.setHorse.CloseSetHorseWindow (gameObject);
////					textStr = "元宝不足！是否前往充值？";
////					QXComData.CreateBox (1,textStr,false,BiaoJuPage.bjPage.TurnToVip);
//					BiaoJuPage.bjPage.LackYuanbao ();
//				}
//				else
//				{
//
//				}
//			}
//			else
//			{
//	            EquipSuoData.TopUpLayerTip(null, false, 0, "VIP等级不足！是否跳转到充值？");
//				//textStr = "VIP等级不足！是否跳转到充值？";
//				//QXComData.CreateBox (1,textStr,false,LackVipLevel);
//				//SetHorseWindow.setHorse.CloseSetHorseWindow (gameObject);
//			}
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
}

using UnityEngine;
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
		public UISprite m_vipIcon;

		private int costYuanBao;

		public GameObject selectBox;

		private string textStr;

		public void InItSetHorseItem (BiaoJuHorseInfo tempInfo,int curId)
		{
			horseInfo = tempInfo;

			horseIcon.spriteName = BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,2);
			border.spriteName = BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,3);

			selectBox.SetActive (tempInfo.horseId == curId ? true : false);

			line.SetActive (tempInfo.horseId == 2 ? false : true);

			point.SetActive (tempInfo.horseId <= curId ? true : false);
			point.transform.localPosition = new Vector3 (float.Parse (BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,4)),0,0);

			desLabel.SetActive (tempInfo.horseId == curId ? true : false);
			costLabel.gameObject.SetActive (tempInfo.horseId > curId ? true : false);

			shouYiLabel.text = tempInfo.shouYi.ToString ();
//			Debug.Log ("BiaoJuPage.m_instance.CurHorseLevel:" + BiaoJuPage.m_instance.CurHorseLevel);
			CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (BiaoJuPage.m_instance.CurHorseLevel);
//			Debug.Log ("tempInfo.upNeedMoney:" + tempInfo.upNeedMoney);
//			Debug.Log ("cartTemp.ShengjiCost:" + cartTemp.ShengjiCost);
			costYuanBao = tempInfo.upNeedMoney - cartTemp.ShengjiCost;
			costLabel.text = costYuanBao.ToString ();
			Debug.Log ("tempInfo.needVipLevel:" + tempInfo.needVipLevel);
//			if (tempInfo.horseId > curId)
//			{
//	 			vipLevel.text = tempInfo.needVipLevel <= QXComData.JunZhuInfo ().vipLv ? "" : "特权";
//				m_vipIcon.spriteName = tempInfo.needVipLevel <= QXComData.JunZhuInfo ().vipLv ? "" : "v" + tempInfo.needVipLevel;
//			}

			vipLevel.text = tempInfo.needVipLevel > 0 ? "特权" : "";
			m_vipIcon.spriteName = tempInfo.needVipLevel > 0 ? "v" + tempInfo.needVipLevel : "";

			buyBtn.m_click_handler -= BuyBtnHandlerClickBack;
			buyBtn.m_click_handler += BuyBtnHandlerClickBack;
		}

		void BuyBtnHandlerClickBack (GameObject obj)
		{
//			BiaoJuData.Instance.UpHorseReq (horseInfo.horseId);
			Debug.Log ("horseInfo.needVipLevel:" + horseInfo.needVipLevel);
			Debug.Log ("QXComData.JunZhuInfo ().vipLv:" + QXComData.JunZhuInfo ().vipLv);
			if (QXComData.CheckYinDaoOpenState (100370))
			{
				BiaoJuData.Instance.UpHorseReq (horseInfo.horseId);
			}
			else
			{
				if (horseInfo.needVipLevel > QXComData.JunZhuInfo ().vipLv)
				{
					Global.CreateFunctionIcon (1901);
				}
				else
				{
					BiaoJuData.Instance.UpHorseReq (horseInfo.horseId);
				}
			}
		}
	}
}

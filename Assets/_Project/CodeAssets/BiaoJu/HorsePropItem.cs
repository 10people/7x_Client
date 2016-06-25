using UnityEngine;
using System.Collections;

namespace Carriage
{
	public class HorsePropItem : MonoBehaviour {

		private HorsePropInfo propInfo;

		public UISprite propIcon;

		public UILabel desLabel;
		public UILabel costLabel;

		public EventHandler buyBtn;

		private string textStr;

		public void InItHorsePropItem (HorsePropInfo tempInfo)
		{
			propInfo = tempInfo;
	//		Debug.Log ("propInfo.isBuy:" + propInfo.isBuy);
			propIcon.spriteName = tempInfo.iconId.ToString ();

			desLabel.text = tempInfo.desc;

			costLabel.text = tempInfo.cost.ToString ();

			QXComData.SetBtnState (buyBtn.gameObject,BiaoJuPage.m_instance.HavePropCount () >= 3 || tempInfo.isBuy ? false : true);

			buyBtn.m_click_handler -= BuyBtnHandlerClickBack;
			buyBtn.m_click_handler += BuyBtnHandlerClickBack;
		}

		void BuyBtnHandlerClickBack (GameObject obj)
		{
			if (BiaoJuPage.m_instance.HavePropCount () < 3)
			{
				if (!propInfo.isBuy)
				{
					BiaoJuData.Instance.BuyHorsePropReq (propInfo.id);
				}
				else
				{
					textStr = "已购买此道具！";
					ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,textStr));
				}
			}
			else
			{
				//道具栏已满
				textStr = "道具栏已满";
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,textStr));
			}
		}
	}
}

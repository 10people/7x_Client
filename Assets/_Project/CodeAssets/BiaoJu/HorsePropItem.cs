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

			desLabel.text = MyColorData.getColorString (3, tempInfo.desc);

			costLabel.text = tempInfo.cost.ToString ();

			UISprite btnSprite = buyBtn.GetComponent<UISprite> ();
			btnSprite.color = BiaoJuPage.bjPage.HavePropCount () >= 3 ? Color.gray : (tempInfo.isBuy ? Color.gray : Color.white);
			UIWidget btnWidget = buyBtn.transform.FindChild ("BtnLabel").gameObject.GetComponent<UIWidget> ();
			btnWidget.color = BiaoJuPage.bjPage.HavePropCount () >= 3 ? Color.gray : (tempInfo.isBuy ? Color.gray : Color.white);

			buyBtn.m_click_handler -= BuyBtnHandlerClickBack;
			buyBtn.m_click_handler += BuyBtnHandlerClickBack;
		}

		void BuyBtnHandlerClickBack (GameObject obj)
		{
			if (!propInfo.isBuy)
			{
				if (BiaoJuPage.bjPage.HavePropCount () < 3)
				{
					BiaoJuData.Instance.BuyHorsePropReq (propInfo.id);
//					//发送购买请求
//					if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= propInfo.cost)
//					{
//						//					QXComData.CreateBox (1,"元宝不足！",true,null);
//						//					return;
//						BiaoJuData.Instance.BuyHorsePropReq (propInfo.id);
//					}
//					else
//					{
//						HorsePropWindow.propWindow.CloseBtnHandlerClickBack (gameObject);
//						//元宝不足
////						textStr = "元宝不足！是否前往充值？";
////						QXComData.CreateBox (1,textStr,false,BiaoJuPage.bjPage.TurnToVip);
//						BiaoJuPage.bjPage.LackYuanbao ();
//					}
				}
				else
				{
					//道具栏已满
					textStr = "道具栏已满";
					QXComData.CreateBox (1,textStr,true,null);
				}
			}
		}
	}
}

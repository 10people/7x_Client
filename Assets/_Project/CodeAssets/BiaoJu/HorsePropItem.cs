using UnityEngine;
using System.Collections;

public class HorsePropItem : MonoBehaviour {

	private HorsePropInfo propInfo;

	public UISprite propIcon;

	public UILabel desLabel;
	public UILabel costLabel;

	public EventHandler buyBtn;

	public void InItHorsePropItem (HorsePropInfo tempInfo)
	{
		propInfo = tempInfo;

		propIcon.spriteName = tempInfo.iconId.ToString ();

		desLabel.text = tempInfo.desc;

		costLabel.text = tempInfo.cost.ToString ();

		UISprite btnSprite = buyBtn.GetComponent<UISprite> ();
		btnSprite.color = BiaoJuPage.bjPage.HavePropCount () >= 3 ? Color.gray : (tempInfo.isBuy ? Color.gray : Color.white);
		UIWidget btnWidget = buyBtn.transform.FindChild ("BtnLabel").gameObject.GetComponent<UIWidget> ();
		btnWidget.color = BiaoJuPage.bjPage.HavePropCount () >= 3 ? Color.gray : (tempInfo.isBuy ? Color.gray : Color.white);

		buyBtn.m_handler -= BuyBtnHandlerClickBack;
		buyBtn.m_handler += BuyBtnHandlerClickBack;
	}

	void BuyBtnHandlerClickBack (GameObject obj)
	{
		if (BiaoJuPage.bjPage.HavePropCount () < 3)
		{
			if (!propInfo.isBuy)
			{
				//发送购买请求
				if (JunZhuData.Instance ().m_junzhuInfo.yuanBao >= propInfo.cost)
				{
					BiaoJuData.Instance.BuyHorsePropReq (propInfo.id);
				}
				else
				{
					//元宝不足
				}
			}
		}
		else
		{
			//道具栏已满
		}
	}
}

using UnityEngine;
using System.Collections;

public class BattleResultGeneralIcon : MonoBehaviour 
{
	public UISprite spriteIcon;

	public UILabel labelNum;


	public void refreshData(Enums.Currency currency, int num)
	{
//		TongBi,//铜币
//		JingYan,//经验
//		YuanBao,//元宝
//		GongXian,//联盟贡献
//		JianShe,//联盟建设
//		WeiWang,//威望（百战）
//		GongJin,//贡金（掠夺）
//		GongXun,//功勋（联盟战）
//		HuangYe//荒野币（荒野）

		string spriteName = "";

		if(currency == Enums.Currency.TongBi) spriteName = "coinicon";

		else if(currency == Enums.Currency.JingYan) spriteName = "expicon";

		else if(currency == Enums.Currency.YuanBao) spriteName = "ingot_30_21";

		else if(currency == Enums.Currency.GongXian) spriteName = "GongXian";

		else if(currency == Enums.Currency.JianShe) spriteName = "buildIcon";

		else if(currency == Enums.Currency.WeiWang) spriteName = "weiwangIcon";

		else if(currency == Enums.Currency.GongJin) spriteName = "gongjin";

		else if(currency == Enums.Currency.GongXun) spriteName = "GongXun";

		else if(currency == Enums.Currency.HuangYe) spriteName = "HuangYe";

		spriteIcon.spriteName = spriteName;

		labelNum.text = "" + num;

	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TeamItem : MonoBehaviour {

	/// <summary>
	/// 马车头像
	/// </summary>
	public UISprite horseIcon;
	/// <summary>
	/// 马车品质
	/// </summary>
	public UISprite pinzhi;
	/// <summary>
	/// 国家
	/// </summary>
	public UISprite country;
	/// <summary>
	/// 君主名字
	/// </summary>
	public UILabel junZhuName;
	/// <summary>
	/// 联盟名字
	/// </summary>
	public UILabel allianceName;
	/// <summary>
	/// 等级
	/// </summary>
	public UILabel levelLabel;
	/// <summary>
	/// 血量
	/// </summary>
	public UILabel hpLabel;
	/// <summary>
	/// 进度
	/// </summary>
	public UILabel jinDuLabel;
	/// <summary>
	/// 收益
	/// </summary>
	public UILabel awardLabel;
	/// <summary>
	/// 显示进度条
	/// </summary>
	public UIScrollBar jinDuBar;
	/// <summary>
	/// 血量进度条
	/// </summary>
	public UIScrollBar hpBar;
	/// <summary>
	/// 战力
	/// </summary>
	public UILabel zhanLi;
	/// <summary>
	/// 数值加成
	/// </summary>
	public UILabel addLabel;

	/// <summary>
	/// 获得押镖君主信息
	/// </summary>
	/// <param name="tempYbJzInfo">Temp yb jz info.</param>
	public void GetTeamItemInfo (YabiaoJunZhuInfo tempYbJzInfo)
	{
		horseIcon.spriteName = "horseIcon" + tempYbJzInfo.horseType;

		pinzhi.spriteName = "pinzhi" + (tempYbJzInfo.horseType - 1); 

		country.spriteName = "nation_" + tempYbJzInfo.junzhuGuojia.ToString ();

		junZhuName.text = tempYbJzInfo.junZhuName;

		addLabel.text = MyColorData.getColorString (6, "+" + tempYbJzInfo.huDun) + "%";

		if (tempYbJzInfo.lianMengName.Equals (""))
		{
			allianceName.text = "无联盟";
		}
		else
		{
			allianceName.text = "<" + tempYbJzInfo.lianMengName + ">";
		}

		levelLabel.text = "Lv" + tempYbJzInfo.level.ToString ();

		awardLabel.text = tempYbJzInfo.worth.ToString ();

		zhanLi.text = tempYbJzInfo.zhanLi.ToString ();

		int jinDu = (int)((tempYbJzInfo.usedTime / (float)tempYbJzInfo.totalTime) * 100);
		jinDuLabel.text = "进度" + jinDu.ToString () + "%";

		YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (jinDuBar,jinDu);

		int hp = (int)((tempYbJzInfo.hp / (float)tempYbJzInfo.maxHp) * 100);
		hpLabel.text = tempYbJzInfo.hp.ToString () + "/" + tempYbJzInfo.maxHp.ToString ();

		YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (hpBar,hp);
	}
}

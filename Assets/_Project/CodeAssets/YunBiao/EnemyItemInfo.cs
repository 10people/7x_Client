using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnemyItemInfo : MonoBehaviour {

	public EnemiesInfo enemyInfo;
	/// <summary>
	/// 国家
	/// </summary>
	public UISprite country;
	/// <summary>
	/// 马车头像
	/// </summary>
	public UISprite horseIcon;
	/// <summary>
	/// 马车品质
	/// </summary>
	public UISprite pinZhi;
	/// <summary>
	/// 人物头像
	/// </summary>
	public UISprite headIcon;
	/// <summary>
	/// 等级
	/// </summary>
	public UILabel levelLabel;
	/// <summary>
	/// 君主名字
	/// </summary>
	public UILabel junZhuName;
	/// <summary>
	/// 联盟名字
	/// </summary>
	public UILabel allianceName;
	/// <summary>
	/// 血条
	/// </summary>
	public UIScrollBar hpBar;
	/// <summary>
	/// 血条label
	/// </summary>
	public UILabel hp;
	/// <summary>
	/// 没有运镖时的描述
	/// </summary>
	public UILabel desLabel;
	/// <summary>
	/// 包含进度等信息的obj
	/// </summary>
	public GameObject processObj;
	/// <summary>
	/// 战力
	/// </summary>
	public UILabel zhanLi;
	/// <summary>
	/// 进度条
	/// </summary>
	public UIScrollBar jinDuBar;
	/// <summary>
	/// 进度label
	/// </summary>
	public UILabel jindu;
	/// <summary>
	/// 收益
	/// </summary>
	public UILabel award;
	/// <summary>
	/// 选择框
	/// </summary>
	public GameObject selectBoxObj;
	/// <summary>
	/// 护盾加成
	/// </summary>
	public UILabel addLabel;

	/// <summary>
	/// 获得仇人信息
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void GetEnemyInfo (EnemiesInfo tempInfo,long junZhuId)
	{
		enemyInfo = tempInfo;
//		Debug.Log ("RoomId:" + tempInfo.eRoomId);
		pinZhi.spriteName = "pinzhi" + (tempInfo.horseType - 1);

		levelLabel.text = "Lv" + tempInfo.jzLevel.ToString ();

		junZhuName.text = tempInfo.junZhuName;

		if (tempInfo.lianMengName.Equals (""))
		{
			allianceName.text = "无联盟";
		}
		else
		{
			allianceName.text = "<" + tempInfo.lianMengName + ">";
		}

		zhanLi.text = tempInfo.zhanLi.ToString ();

		country.spriteName = "nation_" + tempInfo.guojia;

		if (tempInfo.junZhuId == junZhuId)
		{
			selectBoxObj.SetActive (true);
		}

		switch (tempInfo.state)
		{
		case 10://押镖中

			headIcon.spriteName = "";//不显示
			horseIcon.spriteName = "horseIcon" + tempInfo.horseType;
			pinZhi.spriteName = "pinzhi" + (tempInfo.horseType - 1);

			junZhuName.transform.localPosition = new Vector3(-80,25,0);
			allianceName.transform.localPosition = new Vector3(-80,0,0);
			zhanLi.transform.parent.transform.localPosition = new Vector3(165,30,0);

			addLabel.text = MyColorData.getColorString (6, "+" + tempInfo.hudun) + "%";

			desLabel.text = "";

			processObj.SetActive (true);

			int jinDu = (int)((tempInfo.usedTime / (float)tempInfo.totalTime) * 100);
			jindu.text = "进度" + jinDu.ToString () + "%";
			
			YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (jinDuBar,jinDu);
//			Debug.Log ("tempInfo.hp:" + tempInfo.hp + "\ntempInfo.maxHp:" + tempInfo.maxHp);
			int hpNum = (int)((tempInfo.hp / (float)tempInfo.maxHp) * 100);
			hp.text = tempInfo.hp.ToString () + "/" + tempInfo.maxHp.ToString ();

			award.text = YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (tempInfo.horseType).ToString ();

			YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (hpBar,hpNum);

			break;

		case 20://未押镖

			headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;
			horseIcon.spriteName = "";//不显示
			pinZhi.spriteName = "";//不显示

			junZhuName.transform.localPosition = new Vector3(-80,10,0);
			allianceName.transform.localPosition = new Vector3(-80,-15,0);
			zhanLi.transform.parent.transform.localPosition = new Vector3(165,5,0);

			desLabel.text = "未在运镖";

			processObj.SetActive (false);

			break;
		}
	}

	void OnClick ()
	{
		JieBiaoEnemyList.jbEnemy.GetSelectEnemyInfo (enemyInfo);
	}
}

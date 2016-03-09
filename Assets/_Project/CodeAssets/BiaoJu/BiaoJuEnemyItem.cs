using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

namespace Carriage
{
	public class BiaoJuEnemyItem : MonoBehaviour {

		private EnemiesInfo enemyInfo;

		public UIAtlas playerAtlas;
		public UIAtlas biaoJuAtlas;

		public UISprite iconBgSprite;
		public UISprite iconSprite;
		public UISprite border;
		public UISprite nation;
		public UILabel levelLabel;
		public UILabel nameLabel;
		public UILabel allianceLabel;

		public UIScrollBar hpBar;
		public UILabel hpLabel;

		public UIScrollBar jinDuBar;
		public UILabel jinDuLabel;

		public GameObject desObj;
		public UILabel zhanLiLabel;

		public UILabel awardLabel;

		public GameObject processObj;

		public void InItEnemyItem (EnemiesInfo tempInfo)
		{
			enemyInfo = tempInfo;

			iconBgSprite.spriteName = tempInfo.state == 10 ? "item_bg" : "yuankuang";
			iconSprite.atlas = tempInfo.state == 10 ? biaoJuAtlas : playerAtlas;
			iconSprite.spriteName = tempInfo.state == 10 ? "horseIcon" + tempInfo.horseType : "PlayerIcon" + tempInfo.roleId;

			border.spriteName = tempInfo.state == 10 ? "pinzhi" + QXComData.HorsePinZhiId (tempInfo.horseType) : "";
			levelLabel.text = "Lv" + tempInfo.jzLevel.ToString ();

			nameLabel.text = tempInfo.junZhuName;

			allianceLabel.text = MyColorData.getColorString (6,tempInfo.lianMengName.Equals ("") ? "无联盟" : "<" + tempInfo.lianMengName + ">");

			zhanLiLabel.text = "战力" + tempInfo.zhanLi.ToString ();

			nation.spriteName = "nation_" + tempInfo.guojia;

			desObj.SetActive (tempInfo.state == 10 ? false : true);

			processObj.SetActive (tempInfo.state == 10 ? true : false);

			if (tempInfo.state == 10)
			{
				int jinDu = (int)((tempInfo.usedTime / (float)tempInfo.totalTime) * 100);
				jinDuLabel.text = "进度" + jinDu.ToString () + "%";
				QXComData.InItScrollBarValue (jinDuBar,jinDu);

				int hpNum = (int)((tempInfo.hp / (float)tempInfo.maxHp) * 100);
				hpLabel.text = tempInfo.hp.ToString () + "/" + tempInfo.maxHp.ToString ();
				QXComData.InItScrollBarValue (hpBar,hpNum);

				awardLabel.text = BiaoJuPage.bjPage.GetHorseAwardNum (tempInfo.horseType).ToString ();
			}

			this.GetComponent<EventHandler> ().m_click_handler -= ClickBack;
			this.GetComponent<EventHandler> ().m_click_handler += ClickBack;
		}

		void ClickBack (GameObject obj)
		{
			//关闭记录弹窗，自动寻路
			if (enemyInfo.state == 10)
			{
				RootManager.Instance.m_CarriageMain.NavigateToCarriage ((int)enemyInfo.junZhuId);
				BiaoJuRecordPage.bjRecordPage.CloseRecord ();
			}
		}
	}
}

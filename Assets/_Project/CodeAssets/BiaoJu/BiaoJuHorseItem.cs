using UnityEngine;
using System.Collections;

namespace Carriage
{
	public class BiaoJuHorseItem : MonoBehaviour {

		private BiaoJuHorseInfo horseInfo;

		public UISprite horseIcon;
		public UISprite border;
		public UILabel nameLabel;

		public NGUILongPress horsePress;

		public void InItHorseItem (BiaoJuHorseInfo tempInfo)
		{
			horseInfo = tempInfo;

			horseIcon.spriteName = BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,1);
			border.spriteName = BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,3);

			nameLabel.text = BiaoJuPage.m_instance.HorseStringInfo (tempInfo.horseId,0);

			horsePress.OnLongPress -= ActiveTips;
			horsePress.OnLongPress += ActiveTips;
		}

		void ActiveTips (GameObject go)
		{
			ShowTip.showTip (horseInfo.horseItemId);
		}
	}
}

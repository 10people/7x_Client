using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carriage
{
	public class RewardTipsWindow : MonoBehaviour {

		public static RewardTipsWindow rewardTips;

		public List<EventHandler> closeBtnList = new List<EventHandler>();

		public List<UILabel> tipLableList = new List<UILabel>();

		public delegate void RewardTipsDelegate (int i);
		private RewardTipsDelegate tipsDelegate;

		public ScaleEffectController sEffectController;

		void Awake ()
		{
			rewardTips = this;
		}

		void OnDestroy ()
		{
			rewardTips = null;
		}

		public void InItRewardTips (RewardTipsDelegate tempDelegate)
		{
			sEffectController.OnOpenWindowClick ();

			tipLableList [0].text = "运镖总收益=" 
				+ MyColorData.getColorString (16,"基础收益+") 
					+ MyColorData.getColorString (4,"品质加成+")
					+ MyColorData.getColorString (15,"马具增益");

			int startReward = BiaoJuPage.bjPage.GetHorseAwardNum (1);
			int pinZhiReward = BiaoJuPage.bjPage.GetHorseAwardNum (BiaoJuPage.bjPage.CurHorseLevel) - BiaoJuPage.bjPage.GetHorseAwardNum (1);
			int propReward = BiaoJuPage.bjPage.PropReward;
			int totleReward = startReward + pinZhiReward + propReward;

			tipLableList [1].text = "=" + MyColorData.getColorString (16, startReward.ToString () + (totleReward == startReward ? "" : "+")) 
				+ MyColorData.getColorString (4,(pinZhiReward == 0 ? "" : pinZhiReward.ToString ()) + (pinZhiReward == 0 || propReward == 0 ? "" : "+"))
					+ MyColorData.getColorString (15, propReward == 0 ? "" : propReward.ToString ())
					+ (totleReward == startReward ? "" : "=" + MyColorData.getColorString (5,totleReward.ToString ()));
			
			tipLableList [2].text = "确定消耗一个运镖次数派出镖马？";

			tipsDelegate = tempDelegate;

			foreach (EventHandler handler in closeBtnList)
			{
				handler.m_click_handler -= CloseRewardTips;
				handler.m_click_handler += CloseRewardTips;
			}

			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,11);
		}

		void CloseRewardTips (GameObject obj)
		{
			switch (obj.name)
			{
			case "Zhezhao":
				tipsDelegate (1);
				break;
			case "CloseBtn":
				tipsDelegate (1);
				break;
			case "SureBtn":
				tipsDelegate (2);
				break;
			default:
				break;
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carriage
{
	public class HorsePropWindow : MonoBehaviour {

		public static HorsePropWindow propWindow;

		public UIScrollView propSc;
		public UIScrollBar propSb;

		public UIGrid propGrid;

		public GameObject propItemObj;
		private List<GameObject> propItemList = new List<GameObject> ();

		public UILabel yuanBaoLabel;

		public List<EventHandler> closeHandlerList = new List<EventHandler>();

		public ScaleEffectController sEffectController;

		private bool isOpenFirst = true;

		void Awake ()
		{
			propWindow = this;
		}

		void OnDestroy ()
		{
			propWindow = null;
		}

		public void InItHorsePropWindow (List<HorsePropInfo> tempTotleList)
		{
			if (isOpenFirst)
			{
				isOpenFirst = false;
				sEffectController.OnOpenWindowClick ();
			}

			propItemList = QXComData.CreateGameObjectList (propItemObj,propGrid,tempTotleList.Count,propItemList);

			for (int i = 0;i < tempTotleList.Count;i ++)
			{
				HorsePropItem horseProp = propItemList[i].GetComponent<HorsePropItem> ();
				horseProp.InItHorsePropItem (tempTotleList[i]);

				propGrid.repositionNow = true;
			}

			propSc.enabled = tempTotleList.Count > 3 ? true : false;
			propSb.gameObject.SetActive (tempTotleList.Count > 3 ? true : false);

			yuanBaoLabel.text = "您拥有" + MyColorData.getColorString (1,JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString ()) + "元宝";

			foreach (EventHandler handler in closeHandlerList)
			{
				handler.m_click_handler -= CloseBtnHandlerClickBack;
				handler.m_click_handler += CloseBtnHandlerClickBack;
			}

			if (BiaoJuPage.bjPage.CheckGaoJiMaBian ())
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,9);
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,8);
			}
		}

		public void CloseBtnHandlerClickBack (GameObject obj)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,10);
			isOpenFirst = true;
			gameObject.SetActive (false);
		}
	}
}

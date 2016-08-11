using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carriage
{
	public class HorsePropWindow : GeneralInstance<HorsePropWindow> {

		public UIScrollView propSc;
		public UIScrollBar propSb;

		public GameObject propItemObj;
		private List<GameObject> propItemList = new List<GameObject> ();

		public UILabel yuanBaoLabel;

		public ScaleEffectController sEffectController;

		private bool isOpenFirst = true;

		new void Awake ()
		{
			base.Awake ();
		}

		new void OnDestroy ()
		{
			base.OnDestroy ();
		}

		public void InItHorsePropWindow (List<HorsePropInfo> tempTotleList)
		{
			if (isOpenFirst)
			{
				isOpenFirst = false;
				sEffectController.OnOpenWindowClick ();
			}

			propItemList = QXComData.CreateGameObjectList (propItemObj,tempTotleList.Count,propItemList);

			for (int i = 0;i < tempTotleList.Count;i ++)
			{
				propItemList[i].transform.localPosition = new Vector3(0,-i * 90,0);
				HorsePropItem horseProp = propItemList[i].GetComponent<HorsePropItem> ();
				horseProp.InItHorsePropItem (tempTotleList[i]);
			}

			propSc.UpdateScrollbars (true);

			propSc.enabled = tempTotleList.Count > 3 ? true : false;
			propSb.gameObject.SetActive (tempTotleList.Count > 3 ? true : false);

			yuanBaoLabel.text = MyColorData.getColorString (108,JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString ());

			if (BiaoJuPage.m_instance.CheckGaoJiMaBian ())
			{
//				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,11);
				if (QXComData.CheckYinDaoOpenState (100370))
				{
					MYClick (gameObject);
				}
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,10);
			}
		}

		public override void MYClick (GameObject ui)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,12);
			isOpenFirst = true;
			gameObject.SetActive (false);
		}
	}
}

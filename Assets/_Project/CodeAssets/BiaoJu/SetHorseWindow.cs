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
	public class SetHorseWindow : MonoBehaviour {

		public static SetHorseWindow setHorse;

		public GameObject horseItemObj;
		private List<GameObject> horseItemList = new List<GameObject> ();

		public List<EventHandler> closeHandlerList = new List<EventHandler>();

		public UILabel totleShouYiDes;
		public UILabel totleShouYiNum;

		private bool isOpenFirst = true;

		public ScaleEffectController sEffectController;

		void Awake ()
		{
			setHorse = this;
		}

		void OnDestroy ()
		{
			setHorse = null;
		}

		public void InItSetHorseWindow (List<BiaoJuHorseInfo> tempList,int tempType)
		{
			if (isOpenFirst)
			{
				isOpenFirst = false;
				sEffectController.OnOpenWindowClick ();
			}

			if (horseItemList.Count == 0)
			{
				horseItemList.Add (horseItemObj);
				for (int i = 0;i < 3;i ++)
				{
					GameObject horseItem = (GameObject)Instantiate (horseItemObj);

					horseItem.transform.parent = horseItemObj.transform.parent;
					horseItem.transform.localPosition = horseItemObj.transform.localPosition + new Vector3(100 * (i + 1),0,0);
					horseItem.transform.localScale = Vector3.one;
					horseItemList.Add (horseItem);
				}
			}

			for (int i = 0;i < horseItemList.Count;i ++)
			{
				SetHorseItem setHorse = horseItemList[i].GetComponent<SetHorseItem> ();
				setHorse.InItSetHorseItem (tempList[i + 1],tempType);
			}

			totleShouYiDes.text = "您的总收益=" + MyColorData.getColorString (1,"基础收益") + MyColorData.getColorString (4,"+加成收益");
			totleShouYiNum.text = "=" + MyColorData.getColorString (1, BiaoJuPage.bjPage.GetHorseAwardNum (1).ToString ()) 
				+ MyColorData.getColorString (4,"+" + (BiaoJuPage.bjPage.GetHorseAwardNum (tempType) - BiaoJuPage.bjPage.GetHorseAwardNum (1)).ToString ());

			foreach (EventHandler handler in closeHandlerList)
			{
				handler.m_click_handler -= CloseSetHorseWindow;
				handler.m_click_handler += CloseSetHorseWindow;
			}

			if (tempType == 5)
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,6);
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,5);
			}
		}

		public void CloseSetHorseWindow (GameObject obj)
		{
			//判断是否有高级马鞭
			if (BiaoJuPage.bjPage.CheckGaoJiMaBian ())
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,10);
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,7);
			}
			isOpenFirst = true;
			gameObject.SetActive (false);
		}
	}
}

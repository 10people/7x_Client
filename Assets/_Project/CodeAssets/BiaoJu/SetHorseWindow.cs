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
	public class SetHorseWindow : GeneralInstance<SetHorseWindow> {

		public GameObject horseItemObj;
		private List<GameObject> horseItemList = new List<GameObject> ();

		public UILabel totleShouYiDes;
		public UILabel totleShouYiNum;

		private bool isOpenFirst = true;

		public ScaleEffectController sEffectController;

		new void Awake ()
		{
			base.Awake ();
		}

		new void OnDestroy ()
		{
			base.OnDestroy ();
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
					horseItem.transform.localPosition = horseItemObj.transform.localPosition + new Vector3(120 * (i + 1),0,0);
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
			totleShouYiNum.text = "=" + MyColorData.getColorString (1, BiaoJuPage.m_instance.GetHorseAwardNum (1).ToString ()) 
				+ MyColorData.getColorString (4,"+" + (BiaoJuPage.m_instance.GetHorseAwardNum (tempType) - BiaoJuPage.m_instance.GetHorseAwardNum (1)).ToString ());

			if (tempType == 5)
			{
//				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,8);
				if (QXComData.CheckYinDaoOpenState (100370))
				{
					MYClick (gameObject);
				}
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,7);
			}
		}

		public override void MYClick (GameObject ui)
		{
			//判断是否有高级马鞭
			if (BiaoJuPage.m_instance.CheckGaoJiMaBian ())
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,12);
			}
			else
			{
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,9);
			}
			isOpenFirst = true;
			gameObject.SetActive (false);
		}
	}
}

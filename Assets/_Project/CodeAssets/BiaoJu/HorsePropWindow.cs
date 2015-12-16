using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HorsePropWindow : MonoBehaviour {

	public UIScrollView propSc;
	public UIScrollBar propSb;

	public UIGrid propGrid;

	public GameObject propItemObj;
	private List<GameObject> propItemList = new List<GameObject> ();

	public UILabel yuanBaoLabel;

	public EventHandler clostBtn;

	public ScaleEffectController sEffectController;

	private bool isOpenFirst = true;

	public void InItHorsePropWindow (List<HorsePropInfo> tempTotleList)
	{
		if (isOpenFirst)
		{
			isOpenFirst = false;
			sEffectController.OnOpenWindowClick ();
		}

		if (propItemList.Count == 0)
		{
			for (int i = 0;i < tempTotleList.Count;i ++)
			{
				GameObject propItem = (GameObject)Instantiate (propItemObj);

				propItem.SetActive (true);
				propItem.transform.parent = propGrid.transform;
				propItem.transform.localPosition = Vector3.zero;
				propItem.transform.localScale = propItemObj.transform.localScale;

				propItemList.Add (propItem);
			}
		}

		for (int i = 0;i < tempTotleList.Count;i ++)
		{
			HorsePropItem horseProp = propItemList[i].GetComponent<HorsePropItem> ();
			horseProp.InItHorsePropItem (tempTotleList[i]);

			propGrid.repositionNow = true;
		}

		yuanBaoLabel.text = "您拥有" + MyColorData.getColorString (1,JunZhuData.Instance ().m_junzhuInfo.yuanBao.ToString ()) + "元宝";

		clostBtn.m_handler += CloseBtnHandlerClickBack;
	}

	void CloseBtnHandlerClickBack (GameObject obj)
	{
		isOpenFirst = true;
		clostBtn.m_handler -= CloseBtnHandlerClickBack;
		gameObject.SetActive (false);
	}
}

using UnityEngine;
using System.Collections;

public class PawnshopRefreshHint : MonoBehaviour
{
	public PawnshopUIControllor controllor;

	public UILabel labelNum;


	[HideInInspector] public int refreshCost;


	public void refreshData()
	{
		labelNum.text = refreshCost + "";
	}

	public void refresh()
	{
		controllor.sendRefreshData();
	}

	public void CloseLayer()
	{
		gameObject.SetActive (false);
	}

}

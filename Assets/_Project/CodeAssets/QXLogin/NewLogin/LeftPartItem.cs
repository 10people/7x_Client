using UnityEngine;
using System.Collections;

public class LeftPartItem : MonoBehaviour {

	public UILabel leftLabel;

	private int index;
	public int SetIndex
	{
		set{index = value;}
	}
	public int GetIndex
	{
		get{return index;}
	}

	public void ShowLeft (int serveNum)
	{
		leftLabel.text = (serveNum * index + 1) + "-" + (serveNum * (index + 1)) + "区";
	}

	void OnPress (bool isPress)
	{
		SelectServeTwo.SetIsLeft = true;
//		Debug.Log ("isLeft:" + SelectServeTwo.GetIsLeft);
	}
}

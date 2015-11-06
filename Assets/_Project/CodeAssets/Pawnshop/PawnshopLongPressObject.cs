using UnityEngine;
using System.Collections;

public class PawnshopLongPressObject : MonoBehaviour 
{
	public int comonItemId;


	void OnPress(bool pressed)
	{
		if(pressed == true) ShowTip.showTip (comonItemId);
	}

}

using UnityEngine;
using System.Collections;

public class PawnshopDelNumButton : MonoBehaviour
{

	private bool pressing = false;


	void OnPress(bool pressed)
	{
		pressing = pressed;

		if(pressed == true)
		{
			StartCoroutine(MyUpdate());
		}
	}

	public void OnClick()
	{
		delCount ();
	}

	IEnumerator MyUpdate ()
	{
		yield return new WaitForSeconds(0.5f);
		
		for(;pressing == true;)
		{
			yield return new WaitForEndOfFrame();

			delCount();
		}
	}

	private void delCount()
	{
		transform.parent.SendMessage("delNum");
	}

}

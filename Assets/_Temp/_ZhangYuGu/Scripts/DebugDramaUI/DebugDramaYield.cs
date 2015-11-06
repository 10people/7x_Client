using UnityEngine;
using System.Collections;

public class DebugDramaYield : MonoBehaviour 
{

	public void action()
	{
		StartCoroutine (_action());
	}

	IEnumerator _action()
	{
		Time.timeScale = 1;

		float time = 1f;

		for(;time > 0;)
		{
			time -= Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}
	}

	void Update()
	{
		//Debug.Log ("UUUUUUUUUUUUUUUUUUUU    " + Time.timeScale + ", " + Time.deltaTime);
	}

}

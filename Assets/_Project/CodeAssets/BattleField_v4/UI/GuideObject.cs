using UnityEngine;
using System.Collections;

public class GuideObject : MonoBehaviour
{
	public DramaControllor controllor;


	public void over(GameObject _tempGc)
	{
		_tempGc.SetActive (true);

		controllor.onPressInGuide ();
	}

}

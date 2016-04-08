using UnityEngine;
using System.Collections;

public class TCityPlayerAuto : MonoBehaviour {

	private GameObject autoObj;

	public void AddAutoObj (GameObject tempObj)
	{
//		Debug.Log ("Add:" + tempObj.name);
		autoObj = tempObj;
		ModelAutoActivator.RegisterAutoActivator (tempObj);
	}

	public void RemoveAutoObj ()
	{
//		Debug.Log ("Remove:" + autoObj.name);
		ModelAutoActivator.UnregisterAutoActivator (autoObj);
	}
}

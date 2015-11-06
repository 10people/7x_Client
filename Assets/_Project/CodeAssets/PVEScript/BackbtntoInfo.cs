using UnityEngine;
using System.Collections;

public class BackbtntoInfo : MonoBehaviour {

	public GameObject selectMibaoObj;
	private GameObject pveUIObj;

	void Start ()
	{

		pveUIObj = GameObject.Find ("PveUI(Clone)") as GameObject;
	}

	void OnClick()
	{

		Destroy (selectMibaoObj);
	}

	public void DestroyPveUI ()
	{
		MapData.mapinstance.GuidLevel = 0;
		MapData.mapinstance.ShowPVEGuid ();
		Destroy (pveUIObj);
	}
}

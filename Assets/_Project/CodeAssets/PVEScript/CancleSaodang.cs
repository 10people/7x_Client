using UnityEngine;
using System.Collections;

public class CancleSaodang : MonoBehaviour {

	public GameObject supObj;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnClick()
	{

		Destroy (supObj);
	}
}

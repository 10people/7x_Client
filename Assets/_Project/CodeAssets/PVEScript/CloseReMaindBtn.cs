using UnityEngine;
using System.Collections;

public class CloseReMaindBtn : MonoBehaviour {
	public GameObject supgbj;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnClick()
	{

		Destroy (supgbj);

	}
}

using UnityEngine;
using System.Collections;

public class UnionUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

		this.transform.Translate (Vector3.up*Time.deltaTime*0.5f);

		if(this.transform.localPosition.y> 180)
		{
			Destroy(this.gameObject);
		}
	}
}

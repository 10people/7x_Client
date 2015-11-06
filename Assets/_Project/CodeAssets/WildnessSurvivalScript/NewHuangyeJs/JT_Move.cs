using UnityEngine;
using System.Collections;

public class JT_Move : MonoBehaviour {

	int Dir = -1;
	float Speed = 0.03f;
	void Start () {
	
	}
	

	void Update () {
	
		if(this.gameObject.transform.localPosition.y < -2 )
		{

			Dir = 1;

		}
		if(this.gameObject.transform.localPosition.y > 2 )
		{
			Dir = -1;
		}
		this.gameObject.transform.Translate(new Vector3 (0,Speed*Time.deltaTime*Dir,0));
	}
}

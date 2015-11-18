using UnityEngine;
using System.Collections;

public class BladeEffect : MonoBehaviour {

	void Awake(){
		if( GetComponent<Renderer>() == null ){
			Debug.LogError( "Error, No Renderer Found." );

			return;
		}

		if( GetComponent<Renderer>().sharedMaterial == null ){
			Debug.LogError( "Error, No Material Found." );

			return;
		}

		Quality_Blade.UpdateBladeEffect( gameObject );
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

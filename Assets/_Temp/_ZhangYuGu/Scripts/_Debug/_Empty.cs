using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Empty : MonoBehaviour {
	
	#region Mono

	void Awake(){
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Awake() " + gameObject.name );
	}

	public GameObject m_root_game_object = null;

	// Use this for initialization
	void Start () {
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Start() " + gameObject.name );

		if( m_root_game_object != null ){
			ModelAutoActivator.RegisterAutoActivator( m_root_game_object );
		}
	}

	void OnEnable(){
//		Debug.Log( "_Empty.OnEnable()" );

	}

	void OnDestroy(){
//		Debug.Log( "_Empty.OnDestroy()" );

		if( m_root_game_object != null ){
			ModelAutoActivator.UnregisterAutoActivator( m_root_game_object );
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

	}

	#endregion



	#region Utilities

	public int GetFrameCount(){
		return Time.frameCount;
	}

	#endregion
}
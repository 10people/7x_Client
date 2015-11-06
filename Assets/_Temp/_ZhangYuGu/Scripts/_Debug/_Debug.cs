using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//#if UNITY_EDITOR 
//using UnityEditor;
//using UnityEditorInternal;
//#endif

public class _Debug : MonoBehaviour {

	public Animator m_animator = null;

	private static _Debug m_instance = null;

	#region Mono

	void Awake(){
		Debug.Log ( "_Debug.Awake()" );

		m_instance = this;

		DontDestroyOnLoad( gameObject );
	}

	// Use this for initialization
	void Start () {
		Debug.Log ( "_Debug.Start()" );

	}

	void OnGUI(){

	}

	void Update () {
//		Debug.Log ( "Update.TimeScale: " + Time.timeScale );

	}

	void LateUpdate() {

	}

	void OnDestroy(){
		Debug.Log ( "_Debug.OnDestroy()" );

		m_instance = null;
	}

	#endregion



	#region Utilities


	#endregion
}

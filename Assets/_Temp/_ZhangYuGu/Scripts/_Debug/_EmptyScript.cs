using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _EmptyScript : ScriptableObject {

	private static _EmptyScript m_instance_script = null;

	public static _EmptyScript Instance(){
		if( m_instance_script == null ){
			m_instance_script = ScriptableObject.CreateInstance<_EmptyScript>();
		}

		return m_instance_script;
	}

	void OnEnable() {
		Debug.Log("_EmptyScript OnEnable");
    }
	
	void OnDisable() {
		Debug.Log("_EmptyScript OnDisable");
    }
	
	void OnDestroy() {
		Debug.Log("_EmptyScript OnDestroy");
    }
}
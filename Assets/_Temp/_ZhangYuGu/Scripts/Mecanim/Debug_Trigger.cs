using UnityEngine;
using System.Collections;

public class Debug_Trigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log( gameObject + ".OnTriggerEnter( " + other.gameObject + " )" );
	}

	void OnTriggerStay(Collider other) {
		//Debug.Log( gameObject + ".OnTriggerStay( " + other.gameObject + " )" );
	}

	void OnTriggerExit(Collider other) {
		Debug.Log( gameObject + ".OnTriggerExit( " + other.gameObject + " )" );
	}
}

using UnityEngine;
using System.Collections;

public class Debug_Trigger : MonoBehaviour {

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

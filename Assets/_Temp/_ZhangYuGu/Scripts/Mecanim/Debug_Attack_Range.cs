using UnityEngine;
using System.Collections;

public class Debug_Attack_Range : MonoBehaviour {

	public string m_act_name;



	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Attack

	void OnTriggerEnter(Collider other) {
		//Debug.Log( gameObject + ".OnTriggerEnter( " + other.gameObject + " )" );

		other.gameObject.SendMessage( "OnAttacked", this, SendMessageOptions.DontRequireReceiver );
	}
	
	void OnTriggerStay(Collider other) {
		//Debug.Log( gameObject + ".OnTriggerStay( " + other.gameObject + " )" );
	}
	
	void OnTriggerExit(Collider other) {
		//Debug.Log( gameObject + ".OnTriggerExit( " + other.gameObject + " )" );
	}

	#endregion
}

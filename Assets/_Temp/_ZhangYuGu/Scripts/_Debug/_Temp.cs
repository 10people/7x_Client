


using UnityEngine;
using System.Collections;



public class _Temp : MonoBehaviour {
	
	#region Mono

//	private CharacterController m_controller;

	public GameObject m_gb;

	// Use this for initialization
	void Start () {
//		m_controller = (CharacterController)ComponentHelper.AddIfNotExist( gameObject, typeof(CharacterController) );
		InvokeRepeating( "ShowHitted", 1.0f, 3.0f );

	}

	private void ShowHitted(){
//		Debug.Log( "ShowHitted()" );

		EffectTool.Instance.SetHittedEffect( m_gb );
	}

//	void OnAnimatorMove(){
//		Animator animator = GetComponent<Animator>();
//       
//		if( animator != null ){
//			Vector3 newPosition = transform.position;
//
//			if( animator.deltaPosition.sqrMagnitude != 0 ){
//				m_controller.Move( animator.deltaPosition );
//			}
//        }
//	}

	#endregion



	#region Guide

	#endregion



	#region Utilities

	#endregion

}

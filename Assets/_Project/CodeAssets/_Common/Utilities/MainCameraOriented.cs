using UnityEngine;
using System.Collections;

public class MainCameraOriented : MonoBehaviour {

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		UpdateCameraFace();
	}

	#endregion



	#region Utilities

	private Camera m_cached_camera = null;
	
	private void UpdateCameraFace(){
		m_cached_camera = Camera.main;
		
		if ( m_cached_camera == null ){
			return;
		}
		
		if ( m_cached_camera.gameObject.activeInHierarchy == false ){ 
			return;
		}
		
		transform.forward = m_cached_camera.transform.forward;
	}

	#endregion
}

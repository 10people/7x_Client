//#define DEBUG_SCENE_CAMERA_FX



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.4.x
 * @since:		Unity 5.1.3
 * Function:	Use this camera and fx to replace origin battlefield v4's camera.
 * 
 * Notes:
 * 
 */ 
public class Quality_SceneCameraFx : MonoBehaviour{

	#region Mono

	private static Quality_SceneCameraFx m_instance = null;

	void Awake(){
		m_instance = this;

		if( Camera.main != null ){
			InitSceneCameraAndCameraFx();
		}
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Utilities

	private bool m_fx_open = true;

	private List<GameObject> m_fx_gb_list = new List<GameObject>();

	public static void SwitchSceneCameraFx(){
		m_instance.m_fx_open = !m_instance.m_fx_open;

		for( int i = 0; i < m_instance.m_fx_gb_list.Count; i++ ){
			m_instance.m_fx_gb_list[ i ].SetActive( m_instance.m_fx_open );
		}
	}

	public static void InitSceneCameraAndCameraFx(){
		GameObject t_gb = KingCamera.getChildCamera();

		if( t_gb == null ){
			#if DEBUG_SCENE_CAMERA_FX
			Debug.Log( "child camera is null." );
			#endif

			return;
		}

		if( m_instance == null ){
			#if DEBUG_SCENE_CAMERA_FX
			Debug.Log( "Scene Camera Fx Not Exist." );
			#endif

			return;
		}

		{
			AudioListener t_listener = m_instance.gameObject.GetComponent<AudioListener>();

			if( t_listener != null ){
				#if DEBUG_SCENE_CAMERA_FX
				Debug.Log( "Scene Camera Remove additional AudioListener." );
				#endif

				Destroy( t_listener );
			}
			else{
				#if DEBUG_SCENE_CAMERA_FX
				Debug.Log( "Listener not found." );
				#endif
			}
		}

		// origin game camera
		Camera t_origin_cam = t_gb.GetComponent<Camera>();

		GameObject t_cam_root = t_gb.transform.parent.gameObject;

		// art configured camera
		Camera t_scene_cam = GetSceneConfiggedCamera();

		{
			{
				m_instance.m_fx_open = true;

				m_instance.m_fx_gb_list.Clear();
			}

			int t_count = t_scene_cam.gameObject.transform.childCount;

			for( int i = 0; i < t_count; i++ ){
				m_instance.m_fx_gb_list.Add( t_scene_cam.gameObject.transform.GetChild( i ).gameObject );
			}
		}

		{
			m_instance.transform.parent = t_cam_root.transform;

			if( t_origin_cam != null ){
				CameraHelper.CopyCameraComponentParam( t_origin_cam, t_scene_cam );	
			}
			else{
				TransformHelper.ResetLocalPosAndLocalRotAndLocalScale( m_instance.gameObject );	
			}
		}

		if( t_origin_cam != null ){
			if( t_origin_cam.fieldOfView != 45.0f ){
				Debug.LogError( "Error, Fov is not 45.0f." );

				t_origin_cam.gameObject.SetActive( false );
			}
			else{
				t_origin_cam.gameObject.SetActive( false );

				Destroy( t_origin_cam );	
			}
		}

		if( Quality_IEStatus.IsStatusNone() ){
			Vignetting m_vignetting = t_scene_cam.gameObject.GetComponent<Vignetting>();

			if( m_vignetting != null ){
				#if DEBUG_SCENE_CAMERA_FX
				Debug.Log( "Remove vignetting." );
				#endif
				m_vignetting.enabled = false;
			}
			else{
				#if DEBUG_SCENE_CAMERA_FX
				Debug.Log( "vignetting None." );
				#endif
			}
		}
	}

	public static Camera GetSceneConfiggedCamera(){
		if( m_instance == null ){
			return null;
		}

		Camera t_scene_cam = m_instance.gameObject.GetComponentInChildren<Camera>();

		return t_scene_cam;
	}

	#endregion


}

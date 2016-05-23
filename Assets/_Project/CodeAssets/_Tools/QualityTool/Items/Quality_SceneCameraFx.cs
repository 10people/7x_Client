//#define DEBUG_SCENE_CAMERA_FX



using UnityEngine;
using System.Collections;



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

		// origin game camera
		Camera t_origin_cam = t_gb.GetComponent<Camera>();

		GameObject t_cam_root = t_gb.transform.parent.gameObject;

		// art configured camera
		Camera t_scene_cam = m_instance.gameObject.GetComponentInChildren<Camera>();

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
			t_origin_cam.gameObject.SetActive( false );

			Destroy( t_origin_cam );
		}
	}

	#endregion


}

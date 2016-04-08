//#define DEBUG_CAMERA_HELPER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraHelper {

	#region Main Camera
	private static List<Camera> m_cached_main_cams = new List<Camera>();

	public static void SetMainCamera( bool p_enable ){
		#if DEBUG_CAMERA_HELPER
		Debug.Log( "SetMainCamera( " + p_enable + " )" );
		#endif

		{
			Camera[] t_cams = GameObject.FindObjectsOfType<Camera>();

			int t_main_count = 0;

			for( int i = 0; i < t_cams.Length; i++ ){
				if( IsMainCamera( t_cams[ i ] ) ){
					t_main_count++;

					if( !m_cached_main_cams.Contains( t_cams[ i ] ) ){
						m_cached_main_cams.Add( t_cams[ i ] );
					}
				}
			}

			if( t_main_count > 1 ){
				Debug.LogError( "Error, more than one main cam." );
			}
		}

		for( int i = m_cached_main_cams.Count - 1; i >= 0; i-- ){
			if( m_cached_main_cams[ i ] == null ){
				m_cached_main_cams.RemoveAt( i );

				continue;
			}

			m_cached_main_cams[ i ].enabled = p_enable;
		}

		if( m_cached_main_cams.Count > 1 ){
			Debug.LogError( "Error, more than one main cam." );
		}
	}

	public static bool IsMainCamera( Camera p_cam ){
		if( p_cam == null ){
			return false;
		}

		if( p_cam.tag == "MainCamera" ){
			return true;
		}
		else{
			return false;
		}
	}
	
	#endregion



	#region Log

	public static void ShowCameraInfo( GameObject p_gb ){
		if( p_gb == null ){
			Debug.Log( "GameObject is null." );

			return;
		}

		Camera[] t_cams = p_gb.GetComponentsInChildren<Camera>();

		for( int i = 0; i < t_cams.Length; i++ ){
			Camera t_cam = t_cams[ i ];

			LogCamera( t_cam, i + "" );
		}
	}

	public static void LogCamera( Camera p_cam, string p_prefix ){
		if( !string.IsNullOrEmpty( p_prefix ) ){
			Debug.Log( "----------------------- LogCamera: " + p_prefix );
		}

		if( p_cam == null ){
			Debug.Log( "Camera is null: " + p_cam );

			return;
		}

		GameObjectHelper.LogGameObjectHierarchy( p_cam.gameObject );

		Debug.Log( "GameObject.activeInHierarchy: " + p_cam.gameObject.activeInHierarchy );

		Debug.Log( "Camera.enabled: " + p_cam.enabled );

		Debug.Log( "Cam.Depth: " + p_cam.depth );

		Debug.Log( "Cam.Culling: " + p_cam.cullingMask  );
	}

	#endregion

}
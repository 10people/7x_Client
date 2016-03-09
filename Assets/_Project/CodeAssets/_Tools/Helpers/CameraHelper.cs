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
			GameObject t_gb = GameObject.FindGameObjectWithTag( "MainCamera" );

			if( t_gb != null ){
				Camera t_main_c = t_gb.GetComponent<Camera>();

				if( t_main_c != null ){
					if( !m_cached_main_cams.Contains( t_main_c ) ){
						m_cached_main_cams.Add( t_main_c );
					}	
				}
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
	
	#endregion
}
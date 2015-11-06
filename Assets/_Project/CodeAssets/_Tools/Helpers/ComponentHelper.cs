//#define DEBUG_MONO_HELPER

using UnityEngine;
using System.Collections;

public class ComponentHelper{

	#region Global Class Update

	/// Non-Mono class update could be here, invoked in UtilityTool.
	public static void GlobalClassUpdate(){
		UIRootAutoActivator.Instance().Update();
	}

	#endregion



	#region Find Camera

	public static Camera GetCameraInSelfOrChildren( GameObject p_gb ){
		if( p_gb == null ){
			return null;
		}
		
		Camera t_com = p_gb.GetComponent<Camera>();
		
		if( t_com != null ){
			return t_com;
		}

		Camera[] t_cams = p_gb.GetComponentsInChildren<Camera>();

		if( t_cams.Length == 0 ){
			return null;
		}

		if( t_cams.Length > 1 ){
			Debug.Log( "Many Cameras here." );

			for( int i = 0; i < t_cams.Length; i++ ){
				GameObjectHelper.LogGameObjectHierarchy( t_cams[ i ].gameObject,  i +" : " );
			}
		}

		return t_cams[ 0 ];
	}

	public static Camera GetCameraInSelfOrParent( GameObject p_gb ){
		if( p_gb == null ){
			return null;
		}

		Camera t_com = p_gb.GetComponent<Camera>();

		if( t_com != null ){
			return t_com;
		}

		t_com = p_gb.GetComponentInParent<Camera>();

		return t_com;
	}

	#endregion



	#region Add & Destroy

	/// <summary>
	/// Destroy mono.
	/// 
	/// Notes:
	/// 1.if in PlayMode, destroy it;
	/// 2.if in EditMode, destroyimmediately, since destroy is notusable in EditMode;
	/// </summary>
	public static void Destroy( Component p_target ){
		if( p_target == null ){
			#if DEBUG_MONO_HELPER
			Debug.Log( "HideAndDestroy( " + p_target.name + " )" );
			#endif
			
			return;
		}

		if( Application.isPlaying ){
			GameObject.Destroy( p_target );
		}
		else{
			GameObject.DestroyImmediate( p_target );
		}
	}

	/// <summary>
	/// Destroies mono immediately.
	/// </summary>
	/// <param name="p_target">P_target.</param>
	public static void DestroyImmediate( Component p_target ){
		GameObject.DestroyImmediate (p_target);
	}

	/// Add type to a gameobject, if not exist on it.
	public static void AddIfNotExist( GameObject p_gb, System.Type p_type ){
		Component t_com = p_gb.GetComponent( p_type );
		
		if (t_com == null) {
			p_gb.AddComponent( p_type );
		}
	}

	#endregion



	#region Log Component

	public static void LogUISprite( UISprite p_sprite ){
		Debug.Log( p_sprite.width + ", " + p_sprite.height + ": " + p_sprite.atlas.name + " - " + p_sprite.spriteName );
	}

	public static void LogUITexutre( UITexture p_tex ){
		Debug.Log( p_tex.width + ", " + p_tex.height + ": " + p_tex.mainTexture.name );
	}

	public static void LogUILabel( UILabel p_label ){
		Debug.Log( p_label.width + ", " + p_label.height + ": " + p_label.text );
	}

	public static void LogCamera( Camera p_cam ){
		Debug.Log( p_cam.depth + ", " + p_cam.cullingMask );
	}

	public static void LogParticleSystem( ParticleSystem p_ps ){
		if( p_ps == null ){
			return;
		}

		Debug.Log( "Alive: " + p_ps.IsAlive() );

		Debug.Log( "isPlaying: " + p_ps.isPlaying );

		Debug.Log( "isPaused: " + p_ps.isPaused );

		Debug.Log( "isStopped: " + p_ps.isStopped );

		Debug.Log( "particleCount: " + p_ps.particleCount );

		Debug.Log( "startColor: " + p_ps.startColor );
	}

	#endregion
}

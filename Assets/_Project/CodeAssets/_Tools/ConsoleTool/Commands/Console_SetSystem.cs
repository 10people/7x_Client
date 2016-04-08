using UnityEngine;
using System.Collections;
using System;

public class Console_SetSystem {

	#region GC
	
	public static void GC( string[] p_params ){
		UtilityTool.UnloadUnusedAssets();
	}

	public static void LogScreen( string[] p_params ){
		ScreenHelper.LogScreen();
	}

	public static void Vibrate( string[] p_params ){
#if UNITY_ANDROID || UNITY_IOS
		Debug.Log( "Vibrate()" );

		Handheld.Vibrate();
#endif
	}
	
	#endregion



	#region Load & Unload

	public static void LoadRes( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );

			return;
		}

		string t_param_1_path = p_params[ 1 ];

		Debug.Log( "Load Res: " + t_param_1_path );

		Resources.Load( t_param_1_path );
	}

	public static void CreateRes( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );

			return;
		}

		string t_param_1_path = p_params[ 1 ];

		Debug.Log( "Load Res: " + t_param_1_path );

		{
			UnityEngine.Object t_ob = Resources.Load( t_param_1_path );

			if( t_ob == null ){
				Debug.Log( "Object is null: " + t_param_1_path );

				return;
			}

			GameObject t_gb = (GameObject)GameObject.Instantiate( t_ob );

//			{
//				t_gb.SetActive( false );
//
//				GameObject.Destroy( t_gb );
//
//				UtilityTool.UnloadUnusedAssets();
//			}
		}

//		{
//			Global.ResourcesDotLoad( t_param_1_path, CreateResCallback );
//		}
	}

	public static void CreateResCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		if (p_object == null) {
			Debug.LogError ("Asset Not Exist: " + p_path);

			return;
		}

		GameObject.Instantiate( p_object );
	}

	public static void UnloadRes( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );

			return;
		}

		string t_param_1_path = p_params[ 1 ];

		Debug.Log( "Unload Res: " + t_param_1_path );

		UnityEngine.Object t_ob = Resources.Load( t_param_1_path );

		if( t_ob == null ){
			Debug.Log( "Object is null: " + t_param_1_path );

			return;
		}

		Resources.UnloadAsset( t_ob );
	}

	#endregion
}

using UnityEngine;
using System.Collections;
using System;

public class Console_Effect : MonoBehaviour {

	#region Effect

	///	CloseStrEffect Node_1
	public static void CloseStrEffect( string[] p_params ){
		if( p_params.Length < 2 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_target = "";
		
		try{
			t_param_1_target = p_params[ 1 ];
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		Debug.Log( "str effect active: " + t_param_1_target );
		
		GameObject t_gb = GameObject.Find( t_param_1_target );
		
		if( t_gb == null ){
			Debug.LogError( "Error, gb not found: " + t_param_1_target );
			
			return;
		}
		
		StrEffectItem.CloseEffect( t_gb );
	}

	/// /StrEff Node_1 3002 2.0 2.0
	public static void StrEffect( string[] p_params ){
		if( p_params.Length < 5 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_target = "";
		
		string t_param_2_id = "";

		float t_param_3_offset = 0.0f;

		float t_param_4_scale = 0.0f;
		
		try{
			t_param_1_target = p_params[ 1 ];

			t_param_2_id = p_params[ 2 ];

			t_param_3_offset = float.Parse( p_params[ 3 ] );

			t_param_4_scale = float.Parse( p_params[ 4 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		Debug.Log( "str effect active: " + t_param_1_target + " , " + t_param_2_id );

		GameObject t_gb = GameObject.Find( t_param_1_target );

		if( t_gb == null ){
			Debug.LogError( "Error, gb not found: " + t_param_1_target );

			return;
		}

		StrEffectItem.OpenEffect_Console( t_gb, t_param_2_id, t_param_3_offset, t_param_4_scale );
	}

	/// /uibgef true
	public static void UIBackgroundEffect( string[] p_params ){
		if( p_params.Length < 2 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}

		bool t_param_1_active = false;

		float t_param_2_size_p = 0.5f;
		
		try{
			t_param_1_active = bool.Parse( p_params[ 1 ] );

			if( p_params.Length == 3 ){
				t_param_2_size_p = float.Parse( p_params[ 2 ] );
			}
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}

		Debug.Log( "ui effect active: " + t_param_1_active );

		SetUIBackground( t_param_1_active, t_param_2_size_p );
	}

	public static void SetUIBackground( bool p_active, float p_coef = 0.25f ){
		Camera[] t_cams = GameObject.FindObjectsOfType<Camera>();
		
		int t_main_count = 0;
		
		for( int i = 0; i < t_cams.Length; i++ ){
			if( t_cams[ i ] == Camera.main ){
				t_main_count++;

				if( p_active ){
					UIBackgroundEffect t_ef = (UIBackgroundEffect)ComponentHelper.AddIfNotExist( t_cams[ i ].gameObject, typeof(UIBackgroundEffect) );

					if( p_coef > 0 ){
						t_ef.SetRtSize( p_coef );
					}
				}
				else{
					ComponentHelper.RemoveIfExist( t_cams[ i ].gameObject, typeof(UIBackgroundEffect) );
				}
			}
		}
		
		if( t_main_count > 1 ){
			Debug.LogError( "Error, more than one main cam." );
		}
	}

	#endregion



}

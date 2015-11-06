using UnityEngine;
using System.Collections;
using System;

public class Console_RedSpot {

	#region Red Spot
	
	public static void OnLogRedSpotData( string[] p_params ){
		PushAndNotificationHelper.LogRedSpotNotification ();
	}
	
	public static void OnRedSpotCountDown( string[] p_params ){
		if( p_params.Length <= 2 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		string t_param_1_type_str = "";
		
		PushAndNotificationHelper.CountDownType t_param_1_type = PushAndNotificationHelper.CountDownType.FetchServerRedSpot;
		
		int t_param_2_function_id = -1;
		
		float t_param_3_time = 0.0f;
		
		try{
			t_param_1_type_str = p_params[ 1 ];
			
			if( t_param_1_type_str.ToLowerInvariant() == "local" ){
				t_param_1_type = PushAndNotificationHelper.CountDownType.SetLocalRedSpot;
			}
			else if( t_param_1_type_str.ToLowerInvariant() == "server" ){
				t_param_1_type = PushAndNotificationHelper.CountDownType.FetchServerRedSpot;
			}
			else{
				Debug.LogError( "type error: " + t_param_1_type_str );
			}
			
			t_param_2_function_id = int.Parse( p_params[ 2 ] );
			
			t_param_3_time = float.Parse( p_params[ 3 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		PushAndNotificationHelper.AddCountDownRedSpot( t_param_1_type, t_param_2_function_id, t_param_3_time );
	}
	
	public static void OnSetRedSpotData( string[] p_params ){
		if( p_params.Length <= 2 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		int t_param_1_function_id = -1;
		
		bool t_param_2_show = false;
		
		try{
			t_param_1_function_id = int.Parse( p_params[ 1 ] );
			
			t_param_2_show = bool.Parse( p_params[ 2 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		PushAndNotificationHelper.SetRedSpotNotification( t_param_1_function_id, t_param_2_show );
	}
	
	public static void OnGetRedSpotChild( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		int t_param_1_function_id = -1;
		
		try{
			t_param_1_function_id = int.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		int t_child_count = FunctionOpenTemp.GetRedSpotChildCount ( t_param_1_function_id );
		
		Debug.Log ( t_param_1_function_id + " have " + t_child_count + " child." );
	}
	
	#endregion
}

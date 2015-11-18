using UnityEngine;
using System.Collections;
using System;

public class Console_SetQuality {

	#region Set FPS
	
	public static void SetFPS( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		int t_param_1_fps = 0;
		
		try{
			t_param_1_fps = int.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}

		Application.targetFrameRate = t_param_1_fps;
			
		Debug.Log( "Fps: " + Application.targetFrameRate );
	}
	
	#endregion



	#region Set Sync

	public static void SetSync( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		int t_param_1_sync = 0;
		
		try{
			t_param_1_sync = int.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}

		if( t_param_1_sync == 2 ){
			QualitySettings.vSyncCount = 2;
		}
		else if( t_param_1_sync == 1 ){
			QualitySettings.vSyncCount = 1;
		}
		else if( t_param_1_sync == 0 ){
			QualitySettings.vSyncCount = 0;
		}
		else{
			Debug.Log( "Sync Error: " + t_param_1_sync );
		}

		Debug.Log( "Sync: " + QualitySettings.vSyncCount );
	}

	#endregion



	#region SetWeight
	
	public static void SetWeight( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		int t_param_1_weight = 0;
		
		try{
			t_param_1_weight = int.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		{
			if( t_param_1_weight == 1 ){
				QualitySettings.blendWeights = BlendWeights.OneBone;
			}
			else if( t_param_1_weight == 2 ){
				QualitySettings.blendWeights = BlendWeights.TwoBones;
			}
			else if( t_param_1_weight == 4 ){
				QualitySettings.blendWeights = BlendWeights.FourBones;
			}
			
			Debug.Log( "Weight: " + QualitySettings.blendWeights );
		}
	}
	
	#endregion



	#region Set Bloom
	
	public static void SetBloom( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		{
			Quality_Common.ConfigBloom( t_param_1_show );
			
			QualityTool.m_quality_dict[ QualityTool.CONST_BLOOM ].AutoSet( p_params[ 1 ] );
		}
	}
	
	#endregion

	
	
	#region Set Light
	
	public static void SetLight( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		bool t_param_1_show = false;
		
		try{
			t_param_1_show = bool.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		{
			Quality_Shadow.ConfigLights( t_param_1_show );
			
			QualityTool.m_quality_dict[ QualityTool.CONST_IN_CITY_SHADOW ].AutoSet( p_params[ 1 ] );
			
			QualityTool.m_quality_dict[ QualityTool.CONST_BATTLE_FIELD_SHADOW ].AutoSet( p_params[ 1 ] );
		}
	}
	
	#endregion
	
	
	
	#region Log Quality
	
	public static void LogQuality( string[] p_params ){
		QualityTool.LogQualityItems();
	}
	
	#endregion
	
	
	
	#region LogRootAutoRelease
	
	public static void LogRootAutoRelease( string[] p_params ){
		UIRootAutoActivator.Log();
	}
	
	#endregion
}

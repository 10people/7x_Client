using UnityEngine;
using System.Collections;
using System;

public class Console_SetWeight {

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
}

using UnityEngine;
using System.Collections;
using System;

public class Console_SetBloom {

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
			QualityTool.ConfigBloom( t_param_1_show );
			
			QualityTool.m_quality_dict[ QualityTool.CONST_BLOOM ].AutoSet( p_params[ 1 ] );
		}
	}
	
	#endregion
}

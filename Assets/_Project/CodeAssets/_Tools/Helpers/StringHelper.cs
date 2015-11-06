using UnityEngine;
using System.Collections;

public class StringHelper {

	public static bool IsLowerEqual( string p_param_0, string p_param_1 ){
		return p_param_0.ToLowerInvariant() == p_param_1.ToLowerInvariant();
	}

	public static void LogStringArray( string[] p_params, string p_surfix = "" ){
		if( p_params == null ){
			return;
		}

		for( int i = 0; i < p_params.Length; i++ ){
			Debug.Log( p_surfix + " String Array: " + i + " -> " + p_params[ i ] );
		}
	}
}

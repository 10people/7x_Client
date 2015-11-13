using UnityEngine;
using System.Collections;

public class StringHelper {

	#region Common

	public static bool IsContain( string p_source_str, string p_target_str ){
		if( p_source_str.IndexOf( p_target_str ) > 0 ){
			return true;
		} 

		return false;
	}

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

	public static string AddPrefix( string p_string, string p_prefix ){
		if (!p_string.StartsWith(p_prefix))
		{
			return p_prefix + p_string;
		}
		
		return p_string;
	}
	
	public static string RemovePrefix( string p_string, string p_prefix ){
		if ( p_string.StartsWith( p_prefix ) ){
			p_string = p_string.Substring(p_prefix.Length);
		}
		
		return p_string;
	}
	
	public static string RemoveSurfix( string p_string, string p_surfix ){
		if ( p_string.Contains( p_surfix ) ){
			int t_count = p_string.Length - p_surfix.Length;
			
			if (t_count == p_string.LastIndexOf(p_surfix)){
				p_string = p_string.Substring(0, t_count);
			}
			else{
				Debug.LogError("Error, " + p_string + " not end with " + p_surfix);
			}
		}
		
		return p_string;
	}
	
	/** Desc:
     * 
     * Params:
     * 1.p_surfixes:	.jpg#.prefab#.png;
     */
	public static bool IsEndWith( string p_file_name, string p_surfixes ){
		char[] t_splitters = { '#' };
		
		string[] t_surfixes = p_surfixes.Split(t_splitters);
		
		for ( int i = 0; i < t_surfixes.Length; i++ ){
			if ( p_file_name.ToLowerInvariant().EndsWith(t_surfixes[i].ToLowerInvariant()) ){
				return true;
			}
		}
		
		return false;
	}

	#endregion
}

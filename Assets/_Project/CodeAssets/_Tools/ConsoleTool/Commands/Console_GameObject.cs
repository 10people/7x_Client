using UnityEngine;
using System.Collections;
using System;

public class Console_GameObject : MonoBehaviour {

	#region Find GameObject
	
	public static void FindGameObject( string[] p_params ){
		FindGameObjectAndReturn( p_params );
	}
	
	public static GameObject FindGameObjectAndReturn( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return null;
		}
		
		string t_param_1_name = "";
		
		try{
			t_param_1_name = p_params[ 1 ];
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return null;
		}
		
		GameObject t_gb = GameObject.Find( t_param_1_name );
		
		if( t_gb != null ){
			GameObjectHelper.LogGameObjectHierarchy( t_gb );
		}
		else{
			Debug.LogError( "GameObject not found: " + t_param_1_name );
		}
		
		return t_gb;
	}
	
	#endregion
	
	
	
	#region Destroy GameObject
	
	public static void DestroyGameObject( string[] p_params ){
		GameObject t_obj = FindGameObjectAndReturn( p_params );
		
		if( t_obj == null ){
			return;
		}
		
		Destroy( t_obj );
		
		{
			GameObjectHelper.LogGameObjectHierarchy( t_obj, "Destroy first found " );
			
		}
	}
	
	#endregion

}

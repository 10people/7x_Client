#define DEBUG_RESOURCES_HELPER

using UnityEngine;
using System.Collections;

public class ResourcesHelper {

	public static GameObject LoadAssetAndSetParent( string p_asset_path, GameObject p_target_parent ){
		#if DEBUG_RESOURCES_HELPER
		Debug.Log( "ResourcesHelper.LoadAssetAndSetParent( " + p_asset_path + ", " + p_target_parent.name + " )" );
		#endif

		GameObject t_gb = (GameObject)GameObject.Instantiate( Resources.Load( p_asset_path ) );

		if( p_target_parent == null ){
			Debug.Log( "Target Parent = null." );

			return t_gb;
		}
		else{
			t_gb.transform.parent = p_target_parent.transform;
		}

		return t_gb;
	}
}
//#define DEBUG_GAME_OBJECT_HELPER

using UnityEngine;
using System.Collections;

public class GameObjectHelper {

	#region Register Global Component
	
	/// <summary>
	/// Register global components which never destroyed here.
	/// </summary>
	public static void RegisterGlobalComponents(){
		#if DEBUG_GAME_OBJECT_HELPER
		Debug.Log( "RegisterGlobalComponents()" );
		#endif

		GameObject t_dont_destroy = UtilityTool.GetDontDestroyOnLoadGameObject();

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(UtilityTool) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(PushAndNotificationHelper) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(ConsoleTool) );
	}

	#endregion



	#region Hierarchy

	public static void LogGameObjectHierarchy( GameObject p_gb, string p_prefex = "" ){
		if( p_gb == null ){
			Debug.Log( "Object is null." );
			
			return;
		}
		
		string t_origin_name = p_gb.name;
		
		Debug.Log( p_prefex + " " + t_origin_name + ": " + GetGameObjectHierarchy( p_gb ) );
	}
	
	public static string GetGameObjectHierarchy( GameObject p_gb ){
		string t_name = p_gb.name;
		
		while( p_gb.transform.parent != null ){
			t_name = p_gb.transform.parent.name + "/" + t_name;
			
			p_gb = p_gb.transform.parent.gameObject;
		}
		
		return t_name;
	}

	#endregion



	#region GameObject Helper

	/// Get 1st ancestor
	public static GameObject GetRootGameObject( GameObject p_gb ){
		if( p_gb == null ){
			return null;
		}

		while( p_gb.transform.parent != null ){
			p_gb = p_gb.transform.parent.gameObject;
		}

		return p_gb;
	}

	/// <summary>
	/// Hides the and destroy gameobject.
	/// 
	/// Notes:
	/// 1.if in PlayMode, destroy it;
	/// 2.if in EditMode, destroyimmediately, since destroy is notusable in EditMode;
	/// </summary>
	public static void HideAndDestroy( GameObject p_target ){
		if( p_target == null ){
			#if DEBUG_GAME_OBJECT_HELPER
			Debug.Log( "HideAndDestroy( " + p_target.name + " )" );
			#endif

			return;
		}

		p_target.SetActive( false );

		if( Application.isPlaying ){
			GameObject.Destroy( p_target );
		}
		else{
			GameObject.DestroyImmediate( p_target );
		}
	}

	public static void LogGameObjectInfo( GameObject p_gb ){
		Debug.Log( "-------- LogGameObjectInfo: " + p_gb + "------------" );

		Debug.Log( "IsNull: " + ( p_gb == null ) );

		if( p_gb == null ){
			return;
		}

		Debug.Log( "GameObject.Name: " + p_gb.name );
	}

	public static void LogGameObjectTransform( GameObject p_gb, string p_prefex = "" ){
		if( p_gb == null ){
			Debug.Log( "Object is null." );
			
			return;
		}

		Transform t_tran = p_gb.transform;

		Debug.Log( p_prefex + ": " + GetGameObjectHierarchy( p_gb ) );

		// global
		{
			Debug.Log( "Scale: " + t_tran.lossyScale );
			
			Debug.Log( "Pos: " + t_tran.position );
			
			Debug.Log( "Rot: " + t_tran.rotation );
		}

		// local
		{
			Debug.Log( "Local.Scale: " + t_tran.localScale );

			Debug.Log( "Local.Pos: " + t_tran.localPosition );

			Debug.Log( "Local.Rot: " + t_tran.localRotation );
		}
	}

	#endregion
}
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

		GameObject t_dont_destroy = GameObjectHelper.GetDontDestroyOnLoadGameObject();

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(UtilityTool) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(PushAndNotificationHelper) );

		ComponentHelper.AddIfNotExist( t_dont_destroy, typeof(ConsoleTool) );
	}

	#endregion



	#region Dont Destroy On Load
	
	public static GameObject m_dont_destroy_on_load_gb = null;

	private static GameObject m_temp_gbs_root_gb = null;
	
	private static string CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME = "_Dont_Destroy_On_Load";
	
	private static string CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME = "_Temps_GB_Root";

	public static string GetDontDestroyGameObjectName(){
		return CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;
	}
	
	public static GameObject GetDontDestroyOnLoadGameObject(){
		if ( m_dont_destroy_on_load_gb == null ){
			m_dont_destroy_on_load_gb = new GameObject();
			
			m_dont_destroy_on_load_gb.name = CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;

			GameObject.DontDestroyOnLoad( m_dont_destroy_on_load_gb );
		}
		
		return m_dont_destroy_on_load_gb;
	}
	
	public static GameObject GetTempGameObjectsRootGameObject(){
		if( m_temp_gbs_root_gb == null ){
			m_temp_gbs_root_gb = new GameObject();
			
			m_temp_gbs_root_gb.name = CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME;
			
			GameObject.DontDestroyOnLoad(m_temp_gbs_root_gb);
		}
		
		return m_temp_gbs_root_gb;
	}
	
	public static void ClearDontDestroyGameObject()
	{
		//		Debug.Log( "UtilityTool.ClearDontDestroyGameObject()" );
		
		if (m_dont_destroy_on_load_gb != null)
		{
			
			MonoBehaviour[] t_items = m_dont_destroy_on_load_gb.GetComponents<MonoBehaviour>();
			
			for (int i = 0; i < t_items.Length; i++)
			{
				if (t_items[i].GetType() == typeof(UtilityTool))
				{
					//					Debug.Log( "Skip UtilityTool." );
					
					continue;
				}
				
				if (t_items[i].GetType() == typeof(ConfigTool))
				{
					//					Debug.Log( "Skip ConfigTool." );
					
					continue;
				}
				
				if (t_items[i].GetType() == typeof(Bundle_Loader))
				{
					//					Debug.Log( "Skip Bundle_Loader." );
					
					//					Bundle_Loader.CleanData();
					
					continue;
				}
				
				if (t_items[i].GetType() == typeof(ThirdPlatform))
				{
					continue;
				}
				
				
				t_items[i].enabled = false;

				GameObject.Destroy( t_items[i] );
			}
			
			//			m_dont_destroy_on_load_gb.SetActive( false );
			//
			//			Destroy( m_dont_destroy_on_load_gb );
			//
			//			m_dont_destroy_on_load_gb = null;
		}
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

	
	
	#region GameObject

	/// instantiate p_prefab and add it to p_parent's child.
	/// 
	/// Notes:
	/// 1.will also change it's layer to p_parent's.
	public static GameObject AddChild( GameObject p_parent, GameObject p_prefab ){
		GameObject t_gb = (GameObject)GameObject.Instantiate(p_prefab);
		
		if (t_gb != null && p_parent != null)
		{
			Transform t = t_gb.transform;
			
			t.parent = p_parent.transform;
			
			t.localPosition = Vector3.zero;
			
			t.localRotation = Quaternion.identity;
			
			t.localScale = Vector3.one;
			
			t_gb.layer = p_parent.layer;
		}
		
		return t_gb;
	}

	public static void RemoveAllChildrenDeeply( GameObject p_gb, bool p_remove_self = false ){
		if ( p_gb == null ){
			Debug.LogWarning("Error in RemoveAllChildrenDeeply, p_gb = null.");
			
			return;
		}
		
		int t_child_count = p_gb.transform.childCount;
		
		{
			for ( int i = 0; i < t_child_count; i++ ){
				Transform t_child = p_gb.transform.GetChild(i);
				
				RemoveAllChildrenDeeply(t_child.gameObject, true);
			}
			
			if (p_remove_self){
				p_gb.SetActive(false);

				GameObject.Destroy( p_gb );
			}
		}
	}
	
	#endregion


	
	#region Layer
	
	public static void SetGameObjectLayer(GameObject p_target_layer_gb, GameObject p_gameobject)
	{
		if( p_gameobject == null ){
			Debug.LogError( "p_gameobject = null." );
			
			return;
		}
		
		if(p_target_layer_gb == null ){
			Debug.LogError( "p_target_layer_gb = null." );
			
			return;
		}
		
		int t_child_count = p_gameobject.transform.childCount;
		
		{
			for (int i = 0; i < t_child_count; i++)
			{
				Transform t_child = p_gameobject.transform.GetChild(i);
				
				t_child.gameObject.layer = p_target_layer_gb.layer;
				
				SetGameObjectLayer(p_target_layer_gb, t_child.gameObject);
			}
			
			p_gameobject.layer = p_target_layer_gb.layer;
		}
	}
	
	#endregion
}
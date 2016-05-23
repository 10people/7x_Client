//#define DEBUG_GAME_OBJECT_HELPER

using UnityEngine;
using System.Collections;

public class GameObjectHelper {
	
	#region Temp GameObject

	private static GameObject m_temp_gbs_root_gb = null;

	private static string CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME = "_Temps_GB_Root";

	public static GameObject GetTempGameObjectsRoot(){
		if( m_temp_gbs_root_gb == null ){
			m_temp_gbs_root_gb = new GameObject();
			
			m_temp_gbs_root_gb.name = CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME;
		}
		
		return m_temp_gbs_root_gb;
	}

	private static GameObject m_temp_fx_root_gb = null;

	private static string CONST_TEMP_GAMEOBJECT_FX_ROOT_GAME_OBJECT_NAME	= "_Temp_Fx_GB_Root";

	public static GameObject GetTempFxGameObjectRoot(){
		if( m_temp_fx_root_gb == null ){
			m_temp_fx_root_gb = new GameObject();

			m_temp_fx_root_gb.transform.parent = GetTempGameObjectsRoot().transform;

			m_temp_fx_root_gb.name = CONST_TEMP_GAMEOBJECT_FX_ROOT_GAME_OBJECT_NAME;
		}

		return m_temp_fx_root_gb;
	}

	#endregion



	#region Dont Destroy On Load
	
	public static GameObject m_dont_destroy_on_load_gb = null;

	private static string CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME = "_Dont_Destroy_On_Load";
	
	public static string GetDontDestroyGameObjectName(){
		return CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;
	}
	
	public static GameObject GetDontDestroyOnLoadGameObject(){
		if ( m_dont_destroy_on_load_gb == null ){
			m_dont_destroy_on_load_gb = new GameObject();
			
			m_dont_destroy_on_load_gb.name = CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;

			DontDestroyGameObject( m_dont_destroy_on_load_gb );
		}
		
		return m_dont_destroy_on_load_gb;
	}
	
	public static void ClearDontDestroyGameObject(){
		//		Debug.Log( "UtilityTool.ClearDontDestroyGameObject()" );
		
		if ( m_dont_destroy_on_load_gb != null ){
			
			MonoBehaviour[] t_items = m_dont_destroy_on_load_gb.GetComponents<MonoBehaviour>();
			
			for (int i = 0; i < t_items.Length; i++){
				if( t_items[i].GetType() == typeof(UtilityTool) ||
				    t_items[i].GetType() == typeof(ConfigTool) ||
				   	t_items[i].GetType() == typeof(Bundle_Loader) ||
				   	t_items[i].GetType() == typeof(BundleHelper) ||
				   	t_items[i].GetType() == typeof(ThirdPlatform) ||
				   	t_items[i].GetType() == typeof(UIRootAutoActivator) ){
					continue;
				}

				{
					t_items[i].enabled = false;
					
					GameObject.Destroy( t_items[i] );
				}
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
		if( p_gb == null ){
			return "<Null GameObject>";
		}

		string t_name = p_gb.name;
		
		while( p_gb.transform.parent != null ){
			t_name = p_gb.transform.parent.name + "/" + t_name;
			
			p_gb = p_gb.transform.parent.gameObject;
		}
		
		return t_name;
	}

	public static string GetGameObjectHierarchy( Component p_com ){
		if( p_com == null ){
			return "<Null Component>";
		}

		GameObject p_gb = p_com.gameObject;

		if( p_gb == null ){
			return "<Null GameObject>";
		}

		string t_name = p_gb.name;

		while( p_gb.transform.parent != null ){
			t_name = p_gb.transform.parent.name + "/" + t_name;

			p_gb = p_gb.transform.parent.gameObject;
		}

		return t_name;
	}

	#endregion



	#region GameObject Helper

	public static GameObject GetChild( GameObject p_gb, string p_child_name ){
//	public static GameObject GetChild( GameObject p_gb, string p_child_name, int p_level = 0 ){
		if( p_gb == null ){
			Debug.LogError( "Error, GameObject is null." );

			return null;
		}

		int t_count = p_gb.transform.childCount;

		for( int i = 0; i < t_count; i++ ){
			Transform t_tran = p_gb.transform.GetChild( i );

			if( t_tran == null ){
				continue;
			}

			if( t_tran.name == p_child_name ){
				return t_tran.gameObject;
			}
			else{
				GameObject t_gb = GetChild( t_tran.gameObject, p_child_name );
//				GameObject t_gb = GetChild( t_tran.gameObject, p_child_name, p_level + 1 );

				if( t_gb != null ){
//					if( p_level == 0 ){
//						LogGameObjectHierarchy( t_gb, "Child Found." );
//					}

					return t_gb;
				}
			}
		}

		return null;
	}

	/// Get 1st ancestor
	public static GameObject GetRootGameObject( GameObject p_gb ){
		if( p_gb == null ){
			Debug.LogError( "Error, Root GameObject is null." );

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

	#endregion



	#region Logs

	public static void LogGameObjectComponents( GameObject p_gb ){
		ComponentHelper.LogGameObjectComponents( p_gb );
	}

	public static void LogGameObjectInfo( GameObject p_gb ){
		Debug.Log( "-------- LogGameObjectInfo: " + p_gb + "------------" );

		Debug.Log( "IsNull: " + ( p_gb == null ) );

		if( p_gb == null ){
			return;
		}

		Debug.Log( "GameObject.Name: " + p_gb.name );

		LogGameObjectHierarchy( p_gb );
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

			GameObjectHelper.SetGameObjectLayer( t_gb, p_parent.layer );
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

	public static void SetGameObjectLayer( GameObject p_target_gb, int p_layer ){
		if( p_target_gb == null ){
			Debug.LogError( "p_target_gb = null." );

			return;
		}

//		Debug.Log( "SetGameObjectLayer( " + p_target_gb + " - " + p_layer + " )" );

		p_target_gb.layer = p_layer;
	}

	public static void SetGameObjectLayerRecursive( GameObject p_game_object, int p_target_layer ){
		if( p_game_object == null ){
			Debug.LogError( "p_gameobject = null." );
			
			return;
		}
		
		int t_child_count = p_game_object.transform.childCount;
		
		{
			for (int i = 0; i < t_child_count; i++)
			{
				Transform t_child = p_game_object.transform.GetChild( i );
				
				t_child.gameObject.layer = p_target_layer;
				
				SetGameObjectLayerRecursive( t_child.gameObject, p_target_layer );
			}

			SetGameObjectLayer( p_game_object, p_target_layer );
		}
	}
	
	#endregion



	#region Mono

	public static bool HaveMissingMono( GameObject t_gb ){
		MonoBehaviour[] t_monos = t_gb.GetComponents<MonoBehaviour>();

		for( int i = 0; i < t_monos.Length; i++ ){
			if( t_monos[ i ] == null ){
				return true;
			}
		}

		return false;
	}

	public static void LogComponents( GameObject p_gb ){
		MonoBehaviour[] t_monos = p_gb.GetComponentsInChildren<MonoBehaviour>();

		for( int i = 0; i < t_monos.Length; i++ ){
			MonoBehaviour t_mono = t_monos[ i ];

			if( t_mono != null ){
				Debug.Log( i + " : " + t_monos[ i ] + " - " + t_mono.GetType() );
			}
			else{
				Debug.Log( i + " : " + t_monos[ i ] + " is null." );
			}
		}
	}

	#endregion



	#region DontDestroy

	public static void DontDestroyGameObject( GameObject p_gb ){
		if( p_gb == null ){
			Debug.Log( "GameObject is null." );

			return;
		}

		GameObject.DontDestroyOnLoad( p_gb );
	}

	#endregion
}
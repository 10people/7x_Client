//#define DEBUG_OBJECT



using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;



public class ObjectHelper : MonoBehaviour{

	#region Instance

	private static ObjectHelper m_instance;

	public static ObjectHelper Instance(){
		if( m_instance == null )
		{
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

			m_instance = t_gameObject.AddComponent( typeof( ObjectHelper ) ) as ObjectHelper;
		}

		return m_instance;
	}

	#endregion



	#region Mono

	void OnLevelWasLoaded(){
		CleanObjects();
	}

	#endregion



	#region Log

	public static void LogObject( UnityEngine.Object p_object, string p_prefix = "" ){
		if( p_object == null ){
			Debug.Log( "Object is null." );
			
			return;
		}
		
		Debug.Log( p_prefix + " " + p_object.name );
		
		#if UNITY_EDITOR
		Debug.Log( "Path: " + AssetDatabase.GetAssetPath( p_object ) );
		#endif
	}

	#endregion

	#region Trace

	public static void OnUpdate(){
//		#if DEBUG_OBJECT
//		Debug.Log( "ObjectHelper.OnUpdate()" );
//		#endif

		CleanRefs();
	}

	public static void AddGameObjectTrace( GameObject p_gb ){
		#if DEBUG_OBJECT
		Debug.Log( "AddGameObjectTrace( " + p_gb + " )" );
		#endif

		WeakReference t_ref = new WeakReference( p_gb );

		m_refs.Add( t_ref );
	}

	public static void AddComponentTrace( Component p_com ){
		#if DEBUG_OBJECT
		Debug.Log( "AddComponentTrace( " + p_com + " )" );
		#endif

		WeakReference t_ref = new WeakReference( p_com );
		
		m_refs.Add( t_ref );
	}

	public static void AddObjectTrace( UnityEngine.Object p_ob ){
		#if DEBUG_OBJECT
		Debug.Log( "AddObjectTrace( " + p_ob + " )" );
		#endif

		WeakReference t_ref = new WeakReference( p_ob );
		
		m_refs.Add( t_ref );
	}

	#endregion



	#region Load Cache

	private static Dictionary<string, UnityEngine.Object> m_object_dict = new Dictionary<string, UnityEngine.Object>();

	private void CleanObjects(){
		foreach( KeyValuePair<string, UnityEngine.Object> t_kv in m_object_dict ){
			if( t_kv.Value == null ){
				m_object_dict.Remove( t_kv.Key );
			}
		}
	}

	public static void LoadObject( string p_object_path ){
		Global.ResourcesDotLoad( p_object_path, ObjectHelper.Instance().PreloadAnimLoadedCallback );
	}

	public static UnityEngine.Object GetLoadedObject( string p_object_path ){
		UnityEngine.Object t_object = null;

		if( m_object_dict.ContainsKey( p_object_path ) ){
			t_object = m_object_dict[ p_object_path ];
		}

		if( t_object == null ){
			Debug.Log( "Error, object is null: " + p_object_path );
		}

		return t_object;
	}

	public void PreloadAnimLoadedCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "AnimLoadedCallback.path: " + p_path );

		Debug.Log( "AnimLoadedCallback.ob: " + p_object );
		#endif

		if( p_object == null ){
			Debug.Log( "Object is null." );

			return;
		}

		UnityEngine.Object t_ob = null;

		if( m_object_dict.ContainsKey( p_path ) ){
			t_ob = m_object_dict[ p_path ];
		}

		if( t_ob == null ){
			m_object_dict[ p_path ] = p_object;
		}
	}

	#endregion



	#region Utilities

	private static List<System.WeakReference> m_refs = new List<System.WeakReference>();

	private static void CleanRefs(){
		for( int i = m_refs.Count - 1; i >= 0; i-- ){
			if( m_refs[ i ].Target == null ){
				#if DEBUG_OBJECT
				Debug.Log( "Remve Ref( " + i + " )" );
				#endif

				m_refs.RemoveAt( i );
			}
		}
	}

	private static void LogRef( WeakReference p_ref, string p_prefix = "" ){
		if( p_ref == null ){
			Debug.Log( "Ref is null." );
			
			return;
		}

		Debug.Log( p_prefix + " " + p_ref + " - " + p_ref.ToString() + 
		          " .Alive: " + p_ref.IsAlive +
		          " .Target: " + p_ref.Target );
	}

	public static void LogRefs(){
		Debug.Log( "RefCount: " + m_refs.Count );

		for( int i = 0; i < m_refs.Count; i++ ){
			LogRef( m_refs[ i ], i + "" );
		}
	}

	#endregion
}

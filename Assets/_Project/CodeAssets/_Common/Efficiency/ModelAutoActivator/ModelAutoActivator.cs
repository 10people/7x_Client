//#define DEBUG_MODEL

//#define TEMP_CLOSE_FOR_LOCK

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using System.IO;

public class ModelAutoActivator : MonoBehaviour{

	#region Instance

	private static ModelAutoActivator m_instance = null;

	private static Camera m_main_cam = null;

	public static ModelAutoActivator Instance(){
		if( m_instance == null ){
			m_instance = GameObjectHelper.GetDontDestroyOnLoadGameObject().AddComponent<ModelAutoActivator>();
		}

		return m_instance;
	}

	public static ModelAutoActivator GetInstanceWithOutCreate(){
		return m_instance;
	}

	#endregion



	#region Mono

	void Awake(){
		UpdateMainCam();
	}

	void Start(){
		
	}

	void OnLevelWasLoaded(){
		UpdateMainCam();
	}

	public void OnDestroy(){
		m_instance = null;

		Clear();
	}

	#endregion



	#region Functions

	private static float m_last_update_time = 0.0f;

	private void UpdateMainCam(){
		m_main_cam = Camera.main;
	}

	public void ManualUpdate(){
		if( m_main_cam == null ){
			return;
		}

		#if TEMP_CLOSE_FOR_LOCK
		if( Time.realtimeSinceStartup - m_last_update_time < 1.0f ){
		#else
		if( Time.realtimeSinceStartup - m_last_update_time < ConfigTool.GetFloat( ConfigTool.CONST_CHAR_UPDATE_INTERVAL, 1.0f ) ){
		#endif
			return;
		}
		else{
			#if DEBUG_MODEL
			Debug.Log( Time.realtimeSinceStartup + " --------------------------- Update Model Activators Now ---------------------------" );
			#endif

			m_last_update_time = Time.realtimeSinceStartup;
		}

		for( int i = m_activator_list.Count - 1; i >= 0; i-- ){
			ActivatorContainer t_container = m_activator_list[ i ];

			t_container.Update();
		}
	}

	public static void Clear(){
		m_activator_list.Clear();

		m_active_activator_list.Clear();
	}

	public static void LogModelAuto( string[] p_params ){
		Log();
	}

	public static void Log(){
		Debug.Log( "Count: " + m_activator_list.Count );

		List<ActivatorContainer> t_list = GetActiveActivator();

		for( int i = t_list.Count - 1; i >= 0; i-- ){
			ModelAutoActivator.ActivatorContainer t_container = t_list[ i ];

			Debug.Log( i + 
				" visible: " + t_container.IsVisible() + 
				" Hierarchy: " + GameObjectHelper.GetGameObjectHierarchy( t_container.GetRootGameObject() ) );
		}

//		for( int i = m_activator_list.Count - 1; i >= 0; i-- ){
//			ActivatorContainer t_container = m_activator_list[ i ];
//			
//			t_container.Log();
//		}
	}

	public static bool IsVisible( GameObject p_gb ){
		if( p_gb == null ){
			Debug.LogError( "GameObject is null." );

			return false;
		}

		ActivatorContainer t_container = GetActivatorContainer( p_gb );

		if( t_container == null ){
			Debug.LogError( "Error GameObject not registered." );

			return false;
		}

		return t_container.IsVisible();
	}

	private static bool IsInScreen( Vector3 p_pos ){
		if( m_main_cam == null ){
			Debug.LogError( "Error, Main Cam is null." );

			return false;
		}

		Vector3 t_vec = m_main_cam.WorldToViewportPoint( p_pos );

		if( t_vec.x < -SCREEN_OFFSET || t_vec.x > 1 + SCREEN_OFFSET ){
			return false;
		}

		if( t_vec.y < -SCREEN_OFFSET || t_vec.y > 1 + SCREEN_OFFSET ){
			return false;
		}

		return true;
	}

	private static bool IsReachLimitation(){
		#if TEMP_CLOSE_FOR_LOCK
		if( m_active_activator_list.Count >= 30 ){
			return true;
		}
		#else
		if (SceneManager.IsInCarriageScene ()) {
			if( m_active_activator_list.Count >= ConfigTool.GetInt( ConfigTool.CONST_MAX_CHAR_COUNT, 30 ) ){
				return true;
			}
		} else if (SceneManager.IsInTreasureCityScene ()) {
			if( m_active_activator_list.Count >= ConfigTool.GetInt( ConfigTool.CONST_MAX_CHAR_IN_TAN_BAO_COUNT, 30 ) ){
				return true;
			}
		} else {
			Debug.Log( "Error, not in right scene." );
		}
		#endif

		return false;
	}

	private static bool IsActive( ActivatorContainer p_container ){
		if( m_active_activator_list.Contains( p_container ) ){
			return true;
		}

		return false;
	}

	private static void Deactivate( ActivatorContainer p_containter ){
		if( p_containter == null ){
			Debug.LogError( "Container is null." );

			return;
		}

		#if DEBUG_MODEL
		Debug.Log( "Deactivate( " + GameObjectHelper.GetGameObjectHierarchy( p_containter.GetRootGameObject() ) + " )" );
		#endif

		if( m_active_activator_list.Contains( p_containter ) ){
			m_active_activator_list.Remove( p_containter );	
		}
		else{
			#if DEBUG_MODEL
			Debug.Log( "Error, not contained." );
			#endif
		}

		p_containter.SetVisible( false );
	}

	private static void Activate( ActivatorContainer p_containter ){
		if( p_containter == null ){
			Debug.LogError( "Container is null." );

			return;
		}

		#if DEBUG_MODEL
		Debug.Log( "Activate( " + GameObjectHelper.GetGameObjectHierarchy( p_containter.GetRootGameObject() ) + " )" );
		#endif

		if( !m_active_activator_list.Contains( p_containter ) ){
			p_containter.SetVisible( true );

			m_active_activator_list.Add( p_containter );
		}
		else{
			#if DEBUG_MODEL
			Debug.Log( "Error, already contained." );
			#endif
		}
	}

	#endregion



	#region Listener

	private static List<ActivatorContainer> m_activator_list = new List<ActivatorContainer>();

	private static List<ActivatorContainer> m_active_activator_list = new List<ActivatorContainer>();

	private const float SCREEN_OFFSET		= 0.2f;


	public static List<ActivatorContainer> GetActiveActivator(){
		return m_active_activator_list;
	}

	/// Add UIRoot Auto activate.
	public static void RegisterAutoActivator( GameObject p_gb, IModelAutoActivateListener p_listener = null ){
		if( p_gb == null ){
			Debug.LogError( "Error p_gb is null." );

			return;
		}

		{
			ActivatorContainer t_container = GetActivatorContainer( p_gb );

			if( t_container != null ){
				Debug.LogError( "GameObject already contained." );

				return;
			}
		}

		{
			ActivatorContainer t_activator = new ActivatorContainer( p_gb, p_listener );

			m_activator_list.Add( t_activator );

			#if DEBUG_MODEL
			Debug.Log( "RegisterAutoActivator( " + GameObjectHelper.GetGameObjectHierarchy( p_gb ) + " )" );
			#endif

			{
				t_activator.Update();

				if( t_activator.IsVisible() ){
					Activate( t_activator );
				}
				else{
					Deactivate( t_activator );
				}
			}
		}
	}

	public static bool IsModelVisible( GameObject p_gb ){
		ActivatorContainer t_container = GetActivatorContainer( p_gb );

		if( t_container != null ){
			return t_container.IsVisible();
		}
		else{
			Debug.Log( "Model Is not under control." );

			return true;
		}
	}

	// Remove UIRoot Auto activate.
	public static void UnregisterAutoActivator( GameObject p_gb ){
		{
			ActivatorContainer t_container = GetActivatorContainer( p_gb );

			if( t_container != null ){
				m_activator_list.Remove( t_container );

				#if DEBUG_MODEL
				Debug.Log( "UnregisterAutoActivator( " + GameObjectHelper.GetGameObjectHierarchy( p_gb ) + " )" );
				#endif
			}
		}

		{
			ActivatorContainer t_container = GetActiveActivatorContainer( p_gb );

			if( t_container != null ){
				m_active_activator_list.Remove( t_container );

				#if DEBUG_MODEL
				Debug.Log( "UnregisterAutoActivator( " + GameObjectHelper.GetGameObjectHierarchy( p_gb ) + " )" );
				#endif
			}
		}
	}

	private static ActivatorContainer GetActivatorContainer( GameObject p_gb ){
		for( int i = 0; i < m_activator_list.Count; i++ ){
			ActivatorContainer t_container = m_activator_list[ i ];

			if( t_container.GetRootGameObject() == p_gb ){
				return t_container;
			}
		}

		return null;
	}

	private static ActivatorContainer GetActiveActivatorContainer( GameObject p_gb ){
		for( int i = 0; i < m_active_activator_list.Count; i++ ){
			ActivatorContainer t_container = m_active_activator_list[ i ];

			if( t_container.GetRootGameObject() == p_gb ){
				return t_container;
			}
		}

		return null;
	}

	#endregion

	public class ActivatorContainer{
		private GameObject m_root_gameobject = null;

		private IModelAutoActivateListener m_listener = null;

		private MeshRenderer[] m_renderers = null;

		private SkinnedMeshRenderer[] m_skin_renderers = null;

		// position
		private bool m_is_in_screen = false;

		private bool m_is_visible = false;

		public ActivatorContainer( GameObject p_gb, IModelAutoActivateListener p_listener ){
			if( p_gb == null ){
				Debug.LogError( "GameObject is Null." );

				return;
			}

			m_root_gameobject = p_gb;

			m_listener = p_listener;

			Init();
		}

		private void Init(){
			m_renderers = m_root_gameobject.GetComponentsInChildren<MeshRenderer>();

			m_skin_renderers = m_root_gameobject.GetComponentsInChildren<SkinnedMeshRenderer>();

			m_is_in_screen = false;
		}

		public GameObject GetRootGameObject(){
			return m_root_gameobject;
		}

		public void Update(){
			if( ShouldClear() ){
				Clear();

				return;
			}

			m_is_visible = UpdateVisibility();

			#if DEBUG_MODEL
			Debug.Log( "Visibility: " + m_is_visible );
			#endif
		}

		public bool IsVisible(){
			return m_is_visible;
		}

		private bool UpdateVisibility(){
			if( IsActive( this ) ){
				if( !ModelAutoActivator.IsInScreen( m_root_gameobject.transform.position ) ){
					#if DEBUG_MODEL
					Debug.Log( "Active Before, OutOfScreen Now." );
					#endif

					m_is_in_screen = false;

					ModelAutoActivator.Deactivate( this );

					return false;
				}
				else{
					#if DEBUG_MODEL
					Debug.Log( "Always Active." );
					#endif

					return true;
				}
			}
			else{
				if( ModelAutoActivator.IsReachLimitation() ){
					#if DEBUG_MODEL
					Debug.Log( "InActive & ReachLimitation." );
					#endif

					return false;
				}	
				else{
					if( !m_is_in_screen && ModelAutoActivator.IsInScreen( m_root_gameobject.transform.position ) ){
						#if DEBUG_MODEL
						Debug.Log( "NotInScreen Before, InScreen Now." );
						#endif

						m_is_in_screen = true;

						ModelAutoActivator.Activate( this );

						return true;
					}
					else{
						#if DEBUG_MODEL
						Debug.Log( "Always not visible." );
						#endif

						return false;
					}
				}
			}
		}

		public void SetVisible( bool p_is_visible ){
			for( int i = 0; i < m_renderers.Length; i++ ){
				MeshRenderer t_renderer = m_renderers[ i ];

				if( t_renderer == null ){
					continue;
				}

				t_renderer.enabled = p_is_visible;
			}

			for( int i = 0; i < m_skin_renderers.Length; i++ ){
				SkinnedMeshRenderer t_renderer = m_skin_renderers[ i ];

				if( t_renderer == null ){
					continue;
				}

				t_renderer.enabled = p_is_visible;
			}

			{
				TellListener( p_is_visible );
			}
		}

		private void TellListener( bool p_is_visible ){
			if( m_listener != null ){
				m_listener.SetModelActive( p_is_visible );
			}
		}

		public static void Log(){
		
		}

		private bool ShouldClear(){
			if( m_root_gameobject == null ){
				return true;
			}

			return false;
		}

		private void Clear(){
			UnregisterAutoActivator( m_root_gameobject );

			m_root_gameobject = null;
		}
	}

	public interface IModelAutoActivateListener {

		void SetModelActive( bool p_active );
	}
}

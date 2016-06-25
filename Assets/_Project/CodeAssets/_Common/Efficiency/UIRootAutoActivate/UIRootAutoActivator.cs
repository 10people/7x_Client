//#define DEBUG_ACTIVATOR



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



/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.10.27
 * @since:		Unity 5.1.3
 * Function:	Help to improve UI' efficiency.
 * 
 * Notes:
 * 1.Implement IUIRootAutoActivator, then it will be auto activate or deactivate.
 */ 
public class UIRootAutoActivator : MonoBehaviour{

	#region Instance

	private static UIRootAutoActivator m_instance = null;

	public static UIRootAutoActivator Instance(){
		if( m_instance == null ){

			m_instance = GameObjectHelper.GetDontDestroyOnLoadGameObject().AddComponent<UIRootAutoActivator>();
		}

		return m_instance;
	}

	#endregion



	#region Mono

	public void OnDestroy(){
		m_instance = null;

		m_activator_list.Clear();
	}

	#endregion



	#region Functions

	public void ManualUpdate(){
		for( int i = m_activator_list.Count - 1; i >= 0; i-- ){
			ActivatorContainer t_container = m_activator_list[ i ];

			t_container.Update();
		}

		if( m_log ){
			m_log = false;

			Log();
		}
	}

	public bool m_log = false;

	public static void Log(){
		Debug.Log( "Count: " + m_activator_list.Count );

		for( int i = m_activator_list.Count - 1; i >= 0; i-- ){
			ActivatorContainer t_container = m_activator_list[ i ];
			
			t_container.Log( i + "" );
		}
	}

	#endregion



	#region Listener

	private static List<ActivatorContainer> m_activator_list = new List<ActivatorContainer>();

	/// Add UIRoot Auto activate.
	public static void RegisterAutoActivator( IUIRootAutoActivator p_auto_activate ){
		try{
			MonoBehaviour t_mono = (MonoBehaviour)p_auto_activate;
		}
		catch( Exception e ){
			Debug.LogError( "cast error: " + e );

			return;
		}

		{
			ActivatorContainer t_activator = new ActivatorContainer( p_auto_activate );

			m_activator_list.Add( t_activator );
		}
	}

	// Remove UIRoot Auto activate.
	public static void UnregisterAutoActivator( IUIRootAutoActivator p_auto_activate ){
		ActivatorContainer t_container = GetActivatorContainer( p_auto_activate );

		if( t_container != null ){
			m_activator_list.Remove( t_container );
		}
	}

	/// Fetch target container or null.
	private static ActivatorContainer GetActivatorContainer( IUIRootAutoActivator p_activate ){
		for( int i = 0; i < m_activator_list.Count; i++ ){
			ActivatorContainer t_activator = m_activator_list[ i ];

			if( t_activator.GetUIRootActivator() == p_activate ){
				return t_activator;
			}
		}

		return null;
	}

	#endregion

	private class ActivatorContainer{
		private IUIRootAutoActivator m_target_ui_root_activator = null;

		private GameObject m_root_gameobject = null;

		private Camera[] m_target_cameras = null;

		private List<UICamera> m_target_ui_camera_list = new List<UICamera>();

		public ActivatorContainer( IUIRootAutoActivator p_ui_root ){
			m_target_ui_root_activator = p_ui_root;

			{
				m_target_cameras = ComponentHelper.GetCamerasInSelfOrParent( ((MonoBehaviour)m_target_ui_root_activator).gameObject );
				
				if( m_target_cameras == null || m_target_cameras.Length == 0 ){
					m_target_cameras = ComponentHelper.GetCamerasInSelfOrChildren( ((MonoBehaviour)m_target_ui_root_activator).gameObject );
				}

				if( m_target_cameras.Length > 0 ){
					for( int i = 0; i < m_target_cameras.Length; i++ ){
						UICamera t_cam = m_target_cameras[ i ].gameObject.GetComponent<UICamera>();

						if( t_cam != null ){
							m_target_ui_camera_list.Add( t_cam );
						}
					}
				}

				if( m_target_cameras.Length == 0 ){
					Debug.Log( "Error, no cam exist: " + GameObjectHelper.GetGameObjectHierarchy( ((MonoBehaviour)m_target_ui_root_activator).gameObject ) );
				}
			}

			m_root_gameobject = GameObjectHelper.GetRootGameObject( ((MonoBehaviour)m_target_ui_root_activator).gameObject );
		}

		public IUIRootAutoActivator GetUIRootActivator(){
			return m_target_ui_root_activator;
		}

		public void Update(){
			if( ShouldClear() ){
				Clear();

				return;
			}

			UpdateVisibility();
		}

		private void UpdateVisibility(){
			if( m_target_cameras != null ){
				for( int i = 0; i < m_target_cameras.Length; i++ ){
					Camera t_cam = m_target_cameras[ i ];

					if( t_cam == null ){
						continue;
					}

					t_cam.enabled = m_target_ui_root_activator.IsNGUIVisible();
				}

				for( int i = 0;  i < m_target_ui_camera_list.Count; i++ ){
					UICamera t_ui_cam = m_target_ui_camera_list[ i ];

					if( t_ui_cam == null ){
						continue;
					}

					Camera t_cam = t_ui_cam.gameObject.GetComponent<Camera>();

					if( t_cam == null ){
						continue;
					}

					t_ui_cam.enabled = t_cam.enabled;
				}

				return;
			}

			if( m_root_gameobject.activeSelf != m_target_ui_root_activator.IsNGUIVisible() ){
				m_root_gameobject.SetActive( m_target_ui_root_activator.IsNGUIVisible() );
			}
		}

		public void Log( string p_prefix = "" ){
			Debug.Log( "------------ " + p_prefix + "-----------------" );

			Debug.Log( "m_target_ui_root_activator: " + GameObjectHelper.GetGameObjectHierarchy( ((MonoBehaviour)m_target_ui_root_activator).gameObject ) );

			Debug.Log( "m_root_gameobject: " + m_root_gameobject );

			for( int i = 0; i < m_target_cameras.Length; i++ ){
				Camera t_cam = m_target_cameras[ i ];

				Debug.Log( "Target Camera " + i + " : " + GameObjectHelper.GetGameObjectHierarchy( t_cam.gameObject ) );
			}

			for( int i = 0; i < m_target_ui_camera_list.Count; i++ ){
				UICamera t_ui_cam = m_target_ui_camera_list[ i ];

				Debug.Log( "Target UICamera " + i + " : " + GameObjectHelper.GetGameObjectHierarchy( t_ui_cam.gameObject ) );
			}
		}

		private bool ShouldClear(){
			if( m_target_ui_root_activator == null ){
				return true;
			}

			if( m_root_gameobject == null ){
				return true;
			}

			return false;
		}

		private void Clear(){
			m_target_ui_root_activator = null;

			m_root_gameobject = null;

			m_activator_list.Remove( this );
		}
	}
}

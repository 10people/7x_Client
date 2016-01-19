
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
	}

	public static void Log(){
		Debug.Log( "Count: " + m_activator_list.Count );

		for( int i = m_activator_list.Count - 1; i >= 0; i-- ){
			ActivatorContainer t_container = m_activator_list[ i ];
			
			t_container.Log();
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

		private Camera m_target_camera = null;

		private UICamera m_target_ui_camera = null;

		public ActivatorContainer( IUIRootAutoActivator p_ui_root ){
			m_target_ui_root_activator = p_ui_root;

			{
				m_target_camera = ComponentHelper.GetCameraInSelfOrParent( ((MonoBehaviour)m_target_ui_root_activator).gameObject );
				
				if( m_target_camera == null ){
					m_target_camera = ComponentHelper.GetCameraInSelfOrChildren( ((MonoBehaviour)m_target_ui_root_activator).gameObject );
				}

				if( m_target_camera != null ){
					m_target_ui_camera = m_target_camera.gameObject.GetComponent<UICamera>();
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
			if( m_target_camera != null ){
				m_target_camera.enabled = m_target_ui_root_activator.IsNGUIVisible();

				if( m_target_ui_camera != null ){
					m_target_ui_camera.enabled = m_target_camera.enabled;
				}

				return;
			}

			if( m_root_gameobject.activeSelf != m_target_ui_root_activator.IsNGUIVisible() ){
				m_root_gameobject.SetActive( m_target_ui_root_activator.IsNGUIVisible() );
			}
		}

		public void Log(){
			Debug.Log( "m_target_ui_root_activator: " + GameObjectHelper.GetGameObjectHierarchy( ((MonoBehaviour)m_target_ui_root_activator).gameObject ) );

			Debug.Log( "m_root_gameobject: " + m_root_gameobject );

			Debug.Log( "m_target_behaviour: " + m_target_camera );
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

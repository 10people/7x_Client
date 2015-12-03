using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Empty : MonoBehaviour {

	#region Mono

	public int m_ui_version 	= 0;

	public GameObject m_gb_ui	= null;

	private BundleHelper m_bundle_helper = null;

	void Awake(){
		Debug.Log( "Awake()" );
	}

	// Use this for initialization
	void Start () {
		Debug.Log( "Start()" );

		m_bundle_helper = (BundleHelper)ComponentHelper.AddIfNotExist( gameObject, typeof(BundleHelper) );
	}

	void OnEnable(){
		Debug.Log( "OnEnable()" );

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		{
			GUIHelper.GUILayoutVerticalSpace( 0.1f );
		}

		GUILayout.BeginVertical();

		{
			if( GUILayout.Button( "Clean" ) ){
				BundleHelper.CleanCache();
			}

			if( GUILayout.Button( "Load Bundle" ) ){
				string t_url = PathHelper.GetLocalBundleWWWPath( "3d/scene/_debug" );

				m_bundle_helper.LoadBundle( t_url, 0 );
			}

			if( GUILayout.Button( "Load Level" ) ){
				Application.LoadLevel( "_Empty" );
			}
		}

		GUILayout.EndVertical();
	}

	#endregion



}

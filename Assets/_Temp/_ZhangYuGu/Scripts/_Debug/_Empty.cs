using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Empty : MonoBehaviour {

	#region Mono

	public Vector3 m_vec		= new Vector3( 0, 0, 0);

	public UITexture m_tex;

	public Material m_mat;

	public Shader m_shader;

	void Awake(){
		Debug.Log( "Awake()" );

		m_tex = gameObject.GetComponent<UITexture>();

		if( m_tex == null ){
			Debug.LogError( "m_tex = null." );

			return;
		}

		m_tex.material = new Material( m_tex.material );

		m_mat = m_tex.material;

		m_shader = m_mat.shader;
	}

	// Use this for initialization
	void Start () {
		Debug.Log( "Start()" );
	}

	void OnEnable(){
		Debug.Log( "OnEnable()" );

	}
	
	// Update is called once per frame
	void Update () {
		if( m_mat == null ){
			Debug.LogError( "m_mat = null." );

			return;
		}

		m_mat.SetFloat( "_x", m_vec.x );

		m_mat.SetFloat( "_y", m_vec.y );

		m_mat.SetFloat( "_z", m_vec.z );
	}

	void OnGUI(){
		{
			GUIHelper.GUILayoutVerticalSpace( 0.1f );
		}

		GUILayout.BeginVertical();

//		{
//			if( GUILayout.Button( "Clean" ) ){
//				BundleHelper.CleanCache();
//			}
//
//			if( GUILayout.Button( "Load Bundle" ) ){
//				string t_url = PathHelper.GetLocalBundleWWWPath( "3d/scene/_empty" );
//				StartCoroutine( StartLoadBundle( t_url, 0 ) );
//			}
//
//			if( GUILayout.Button( "Load Level" ) ){
//				Application.LoadLevel( "_Empty" );
//			}
//		}

		GUILayout.EndVertical();
	}

	#endregion

}

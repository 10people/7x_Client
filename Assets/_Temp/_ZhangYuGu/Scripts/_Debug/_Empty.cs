using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _Empty : MonoBehaviour {

	public Vector3 m_vec		= new Vector3( 0, 0, 0);

	public UITexture m_tex;

	public Material m_mat;

	public Shader m_shader;

	public static _Empty m_instance = null;

	private _EmptyScript m_empty_script = null;

	#region Mono

	void Awake(){
		Debug.Log( "_Empty.Awake()" );

		m_instance = this;

		_EmptyScript.Instance();

//		m_tex = gameObject.GetComponent<UITexture>();
//
//		if( m_tex == null ){
//			Debug.LogError( "m_tex = null." );
//
//			return;
//		}
//
//		m_tex.material = new Material( m_tex.material );
//
//		m_mat = m_tex.material;
//
//		m_shader = m_mat.shader;
	}

	// Use this for initialization
	void Start () {
		Debug.Log( "_Empty.Start()" );
	}

	void OnEnable(){
		Debug.Log( "_Empty.OnEnable()" );

	}

	void OnDestroy(){
		Debug.Log( "_Empty.OnDestroy()" );

		m_empty_script = null;
	}

	// Update is called once per frame
	void Update () {

	}

	void OnGUI(){
//		{
//			GUIHelper.GUILayoutVerticalSpace( 0.1f );
//		}
//
//		GUILayout.BeginVertical();
//
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
//
//		GUILayout.EndVertical();
	}

	#endregion



	#region Utilities

	public int GetFrameCount(){
		return Time.frameCount;
	}

	#endregion
}
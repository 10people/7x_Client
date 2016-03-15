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

	public int m_ui_source_effect_id = 223;

	public int m_ui_mirror_effect_id = 224;

	public int m_ui_fx_id = 225;

	#region Mono

	void Awake(){
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Awake() " + gameObject.name );

		m_instance = this;

//		_EmptyScript.Instance();

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
//		Debug.Log( Time.realtimeSinceStartup + " _Empty.Start() " + gameObject.name );

		EffectTool.OpenMultiUIEffect_ById( gameObject, m_ui_source_effect_id, m_ui_mirror_effect_id, m_ui_fx_id );

//		UISprite t_sprite = GetComponent<UISprite>();
//
//		if( t_sprite == null ){
//			return;
//		}
//
//		ComponentHelper.LogUISprite( t_sprite, "UI.Sprite" );
//
//		UISpriteData t_sprite_data = t_sprite.atlas.GetSprite( t_sprite.spriteName );
//
//		ComponentHelper.LogUISpriteData( t_sprite_data, "UI.Sprite.Data" );
	}

	void OnEnable(){
//		Debug.Log( "_Empty.OnEnable()" );

	}

	void OnDestroy(){
//		Debug.Log( "_Empty.OnDestroy()" );

//		m_empty_script = null;

		EffectTool.CloseMultiUIEffect_ById( gameObject, m_ui_source_effect_id, m_ui_mirror_effect_id, m_ui_fx_id );
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
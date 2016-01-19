//#define DEBUG_EFFECT


using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class SparkleEffectItem : MonoBehaviour {

	/** Desc:
	 * 	Common_Icon		---> common icon
	 *  Confirm_Button	---> a little red
	 *  Cancel_Button	---> a little orange
	 */
	public enum MenuItemStyle{
		None,				// default use
		Common_Icon,		// common icon
		Confirm_Button,		// a little red
		Cancel_Button,		// a little orange
	}

	protected MenuItemStyle m_style =  MenuItemStyle.None;

	public Color m_color = new Color( 1f, 1f, 1f, 1f );

	public static Color[] DefinedColors = {
		new Color( 1f, 1f, 1f, 1f ),								// Default
		new Color( 255f / 255, 243f / 255, 193f / 255, 1.0f ),		// Common_Icon
		new Color( 255f / 255, 209f / 255, 195f / 255, 1.0f ),		// Confirm_Button
		new Color( 255f / 255, 236f / 255, 201f / 255, 1.0f ),		// Cancel_Button
	};

	private float m_start_time 	= 0.0f;

	#region Mono

	void Start(){
		m_start_time = Time.realtimeSinceStartup;

		{
			m_color = DefinedColors[ (int)m_style ];
		}

		{
			m_sprite = GetComponent<UISprite>();
			
			if( m_sprite == null ){
				Debug.Log( "No UISprite Attached." );

				return;
			}

			{
				m_tex = m_sprite.mainTexture;
				
				m_sprite_data = m_sprite.atlas.GetSprite( m_sprite.spriteName );

				if( m_sprite_data == null ){
					Debug.Log( "No Sprite data setted." );

					return;
				}
			}
		}

		{
			m_ui_tex = GetComponent<UITexture>();

			if( m_ui_tex != null ){
				Debug.Log( "Error, uitexture already exist." );

				return;
			}

			m_ui_tex = gameObject.AddComponent<UITexture>();

			m_ui_tex.depth = m_sprite.depth;

			m_sprite.enabled = false;

			{
				m_ui_tex.width = m_sprite.width;
				
				m_ui_tex.height = m_sprite.height;

				Vector3 t_vec3 = m_sprite.gameObject.transform.localPosition;

				m_ui_tex.pivot = m_sprite.pivot;

				m_ui_tex.gameObject.transform.localPosition = t_vec3;

				m_ui_tex.material = new Material( Shader.Find( "Custom/Effects/Sparkle Effect" ) );

				m_ui_tex.material.mainTexture = m_tex;

				Vector4 t_vec_4 = GetVec4();

				m_ui_tex.material.SetVector( "_Tex_ST", t_vec_4 );
			}
		}
	}

	void Update(){
		if( m_ui_tex != null ){
			if( m_ui_tex.material != null ){
				UpdateSparkle();

				m_ui_tex.material.SetColor( "_TintColor", m_color );

				m_ui_tex.material.SetFloat( "_Coef", m_coef );

				m_ui_tex.material.SetFloat( "_T", m_t );

				m_ui_tex.CustomReset();
			}
		}
	}

	void OnDestroy(){
		m_sprite.enabled = true;

		Destroy( m_ui_tex );
	}

	#endregion



	#region Use

	public static void CloseSparkle( GameObject p_ui_sprite_gb ){
		if( p_ui_sprite_gb == null ){
			Debug.Log( "No gameobject passed." );
			
			return;
		}

		SparkleEffectItem t_sparkle = p_ui_sprite_gb.GetComponent<SparkleEffectItem>();
		
		if( t_sparkle == null ){
			Debug.Log( "No Sprite found for: " + p_ui_sprite_gb );
			
			return;
		}

		Destroy( t_sparkle );
	}

	/// Pass gameobject with target UISprite here
	public static void OpenSparkle( GameObject p_ui_sprite_gb, MenuItemStyle p_style ){
		if( p_ui_sprite_gb == null ){
			Debug.Log( "No gameobject passed." );

			return;
		}

		UISprite t_sprite = p_ui_sprite_gb.GetComponent<UISprite>();

		if( t_sprite == null ){
			Debug.Log( "No Sprite found for: " + p_ui_sprite_gb );

			return;
		}

		ComponentHelper.RemoveIfExist( p_ui_sprite_gb, typeof(SparkleEffectItem) );

		SparkleEffectItem t_sparkle = p_ui_sprite_gb.AddComponent<SparkleEffectItem>();

		t_sparkle.m_style = p_style;
	}

	#endregion


	
	#region Utilities

	private void UpdateSparkle(){
		if( Time.realtimeSinceStartup - m_start_time - m_duration - m_interval > 0 ){
			m_start_time = Time.realtimeSinceStartup;
		}

		{
			m_t = Mathf.Lerp( 1, m_min_t, ( Time.realtimeSinceStartup - m_start_time ) / m_duration );
		}
	}

	private bool IsActive(){
		if( Time.realtimeSinceStartup - m_start_time < m_duration ){
			return true;
		}

		return false;
	}

	#endregion



	#region Clean

	private void LogInfo(){
		Debug.Log( "LogInfo()" );

		ComponentHelper.LogTexture( m_tex );

		if( m_sprite_data != null ){
			m_sprite_data.Log();
		}
	}

	private Vector4 GetVec4(){
		return new Vector4( 
		                   1.0f * ( m_sprite_data.width + m_sprite_data.paddingLeft + m_sprite_data.paddingRight ) / m_sprite.atlas.AtlasWidth,
		                   1.0f * ( m_sprite_data.height + m_sprite_data.paddingTop + m_sprite_data.paddingBottom ) / m_sprite.atlas.AtlasHeight,
		                   1.0f * ( m_sprite_data.x - m_sprite_data.paddingLeft ) / m_sprite.atlas.AtlasWidth,
		                   1 - 1.0f * ( m_sprite_data.y + m_sprite_data.height + m_sprite_data.paddingBottom ) / m_sprite.atlas.AtlasHeight );
	}

	#endregion



	#region Callbacks

	#endregion



	#region Utilities

	private float m_duration	= 0.3f;
	
	private float m_interval	= 2.5f;
	
	private float m_min_t 		= -2f;
	
	private float m_coef 		= 1.0f;
	
	private float m_t 			= -1;
	
	private UITexture m_ui_tex 	= null;
	
	private Texture m_tex 		= null;
	
	private UISprite m_sprite 	= null;
	
	private UISpriteData m_sprite_data 	= null;

	#endregion

}
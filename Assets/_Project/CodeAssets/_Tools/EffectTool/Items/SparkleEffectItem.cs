//#define DEBUG_EFFECT



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;



public class SparkleEffectItem : MonoBehaviour {

	#if DEBUG_EFFECT
	public Vector4 m_vec4 = Vector4.zero;
	#endif

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

	// target sparkle circle count, -1 means loop forever
	private int m_sparkle_count = -1;

	public static Color[] DefinedColors = {
		new Color( 1f, 1f, 1f, 1f ),								// Default
		new Color( 255f / 255, 243f / 255, 193f / 255, 1.0f ),		// Common_Icon
		new Color( 255f / 255, 209f / 255, 195f / 255, 1.0f ),		// Confirm_Button
		new Color( 255f / 255, 236f / 255, 201f / 255, 1.0f ),		// Cancel_Button
	};

	private Color m_color = new Color( 1f, 1f, 1f, 1f );


	private enum SparkleType{
		Sprite,
		Texture,
	}

	private SparkleType m_sparkle_type = SparkleType.Sprite;

	#region Mono

	void Start(){
		#if DEBUG_EFFECT
		Debug.Log( "SparkleEffectItem.Start( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		m_start_time = Time.realtimeSinceStartup;

		{
			UpdateSparkleParam();
		}
	}

	void OnEnable(){
		#if DEBUG_EFFECT
		Debug.Log( "SparkleEffectItem.OnEnable( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		InitSparkle();
	}

	void Update(){
		if( string.Compare( m_sprite_name, m_sprite.spriteName ) != 0 ||
			m_tex != m_sprite.mainTexture ){
			#if DEBUG_EFFECT
			Debug.Log( "Sprite Change, Reset." );
			#endif

			Clean();

			InitSparkle();
		}

		if (m_ui_tex != null) {
			if (m_ui_tex.material != null) {
				UpdateSparkle ();
			}
		}

		if( m_ui_tex != null ){
			if( m_ui_tex.material != null ){
				m_ui_tex.material.SetColor( "_TintColor", m_color );

				m_ui_tex.material.SetFloat( "_Coef", m_coef );

				m_ui_tex.material.SetFloat( "_T", m_t );

				if( m_sparkle_type == SparkleType.Sprite ){
					m_ui_tex.CustomReset();	
				}
				else if( m_sparkle_type == SparkleType.Texture ){
					m_ui_tex.CustomReset();	
				}
			}
		}
	}

	void OnDisable(){
		#if DEBUG_EFFECT
		Debug.Log( "SparkleEffectItem.OnDisable( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		Clean();
	}

	void OnDestroy(){
		#if DEBUG_EFFECT
		Debug.Log( "SparkleEffectItem.OnDestroy( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		Clean();
	}

	#endregion



	#region Use

	private void InitSparkle(){
		{
			m_sprite = GetComponent<UISprite>();

			if( m_sprite == null ){
				m_ui_tex = GetComponent<UITexture>();

				if( m_ui_tex == null ){
					Debug.Log( "No UISprite Attached." );

					Destroy( this );

					return;	
				}
				else{
					m_sparkle_type = SparkleType.Texture;
				}
			}
			else{
				m_sparkle_type = SparkleType.Sprite;

				{
					m_tex = m_sprite.mainTexture;

					m_sprite_data = m_sprite.atlas.GetSprite( m_sprite.spriteName );

					m_sprite_name = m_sprite.spriteName;

					if( m_sprite_data == null ){
						#if UNITY_EDITOR
						Debug.Log( "No Sprite data setted." );
						#endif

						return;
					}

					#if DEBUG_EFFECT
					LogInfo();
					#endif
				}
			}
		}

		if( m_sparkle_type == SparkleType.Sprite ){
			m_ui_tex = gameObject.AddComponent<UITexture>();

			m_ui_tex.panel = m_sprite.panel;

			m_ui_tex.depth = m_sprite.depth;

			m_sprite.enabled = false;

			{
				Vector4 t_dd = m_sprite.drawingDimensions;

				m_ui_tex.width = (int)( t_dd.z - t_dd.x );

				m_ui_tex.height = (int)( t_dd.w - t_dd.y );

				Vector3 t_vec3 = m_sprite.gameObject.transform.localPosition;

				m_ui_tex.pivot = m_sprite.pivot;

				m_ui_tex.gameObject.transform.localPosition = t_vec3;

				m_ui_tex.material = new Material( Shader.Find( "Custom/Effects/Sparkle Effect" ) );

				m_ui_tex.material.mainTexture = m_tex;

				Vector4 t_vec_4 = GetVec4();

				#if DEBUG_EFFECT
				m_vec4 = t_vec_4;
				#endif

				m_ui_tex.material.SetVector( "_Tex_ST", t_vec_4 );
			}
		}
		else if( m_sparkle_type == SparkleType.Texture ){
			m_origin_sh = m_ui_tex.shader;

			m_tex = m_ui_tex.mainTexture;

			m_ui_tex.material = new Material( Shader.Find( "Custom/Effects/Sparkle Effect" ) );

			m_ui_tex.material.mainTexture = m_tex;
		}
	}

	public void Clean(){
//		Debug.Log( "Sparkle.Clean()" );

		if( m_sparkle_type == SparkleType.Sprite ){
			if( m_sprite != null ){
				m_sprite.enabled = true;	

				m_sprite = null;
			}

			if( m_ui_tex != null ){
				m_ui_tex.enabled = false;

//				DestroyImmediate( m_ui_tex );

				Destroy( m_ui_tex );

				m_ui_tex = null;
			}
		}
		else if( m_sparkle_type == SparkleType.Texture ){
			if( m_ui_tex != null ){
				m_ui_tex.material = null;

				m_ui_tex.shader = m_origin_sh;

				m_origin_sh = null;

				m_ui_tex = null;
			}
		}
	}

	public static void CloseSparkle( GameObject p_ui_sprite_gb ){
		if( p_ui_sprite_gb == null ){
			Debug.Log( "No gameobject passed." );
			
			return;
		}

		SparkleEffectItem t_sparkle = p_ui_sprite_gb.GetComponent<SparkleEffectItem>();
		
		if( t_sparkle == null ){
//			Debug.Log( "No Sparkle found for: " + p_ui_sprite_gb );

//			GameObjectHelper.LogGameObjectHierarchy( p_ui_sprite_gb );
			
			return;
		}

		t_sparkle.Clean();

		Destroy( t_sparkle );
	}

	/** Desc:
	 * Pass gameobject with target UISprite here.
	 * 
	 * Params:
	 * p_sparkle_count: -1 means always sparkling, 1-int.max means sparkle only defined times.
	 */ 
	public static void OpenSparkle( GameObject p_ui_sprite_gb, MenuItemStyle p_style, int p_sparkle_count = -1 ){
		if( p_ui_sprite_gb == null ){
			Debug.Log( "No gameobject passed." );

			return;
		}

		{
			UISprite t_sprite = p_ui_sprite_gb.GetComponent<UISprite>();

			UITexture t_tex = p_ui_sprite_gb.GetComponent<UITexture>();

			if( t_sprite == null ){
				if( t_tex == null ){
					Debug.Log( "No sprite or texture found for: " + p_ui_sprite_gb );

					return;
				}
			}
		}

		{
			SparkleEffectItem t_effect = (SparkleEffectItem)ComponentHelper.RemoveIfExist( p_ui_sprite_gb, typeof(SparkleEffectItem) );

			if( t_effect != null ){
				t_effect.Clean();
			}
		}

		SparkleEffectItem t_sparkle = p_ui_sprite_gb.AddComponent<SparkleEffectItem>();

		t_sparkle.SetSparkleItemType( p_style );

		t_sparkle.SetSparkleCount( p_sparkle_count );
	}

	#endregion



	#region Set

	public void SetSparkleCount( int p_sparkle_count = -1 ){
		m_sparkle_count = p_sparkle_count;
	}

	private void SetSparkleItemType( MenuItemStyle p_style ){
		m_style = p_style;

		UpdateSparkleParam();
	}

	private void UpdateSparkleParam(){
		m_color = DefinedColors[ (int)m_style ];
	}

	#endregion


	
	#region Utilities

	private int m_done_circle_count = 0;

	private void UpdateSparkle(){
		if( Time.realtimeSinceStartup - m_start_time - m_duration - m_interval > 0 ){
			m_start_time = Time.realtimeSinceStartup;

			m_done_circle_count++;
		}

		{
			m_t = Mathf.Lerp( 1, m_min_t, ( Time.realtimeSinceStartup - m_start_time ) / m_duration );
		}

		if( m_sparkle_count > 0 ){
			if( m_done_circle_count < 0 ){
				m_done_circle_count = 0;
			}

			if( m_done_circle_count >= m_sparkle_count ){
//				Debug.Log( m_done_circle_count + " / " + m_sparkle_count );

				Destroy( this );
			}
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
		if( m_sparkle_type == SparkleType.Sprite ){
			return new Vector4( 
				1.0f * ( m_sprite_data.width ) / m_sprite.atlas.AtlasWidth,
				1.0f * ( m_sprite_data.height ) / m_sprite.atlas.AtlasHeight,
				1.0f * ( m_sprite_data.x ) / m_sprite.atlas.AtlasWidth,
				1 - 1.0f * ( m_sprite_data.y + m_sprite_data.height ) / m_sprite.atlas.AtlasHeight );	
		}
		else if( m_sparkle_type == SparkleType.Texture ){
			return new Vector4( 
				1.0f,
				1.0f,
				0.0f,
				0.0f );
		}
		else{
			return new Vector4( 0, 0, 1, 1 );
		}


//		return new Vector4( 
//		                   1.0f * ( m_sprite_data.width + m_sprite_data.paddingLeft + m_sprite_data.paddingRight ) / m_sprite.atlas.AtlasWidth,
//		                   1.0f * ( m_sprite_data.height + m_sprite_data.paddingTop + m_sprite_data.paddingBottom ) / m_sprite.atlas.AtlasHeight,
//		                   1.0f * ( m_sprite_data.x - m_sprite_data.paddingLeft ) / m_sprite.atlas.AtlasWidth,
//		                   1 - 1.0f * ( m_sprite_data.y + m_sprite_data.height + m_sprite_data.paddingBottom ) / m_sprite.atlas.AtlasHeight );
	}

	#endregion



	#region Callbacks

	#endregion



	#region Utilities

	private float m_start_time 	= 0.0f;

	private float m_duration	= 0.3f;
	
	private float m_interval	= 2.5f;
	
	private float m_min_t 		= -2f;
	
	private float m_coef 		= 1.0f;
	
	private float m_t 			= -1;

	// self or mirror
	private UITexture m_ui_tex 	= null;

	private Texture m_tex 		= null;

	private UISprite m_sprite 	= null;
	
	private UISpriteData m_sprite_data 	= null;

	private string m_sprite_name = "";

	private Shader m_origin_sh	= null;

	#endregion

}
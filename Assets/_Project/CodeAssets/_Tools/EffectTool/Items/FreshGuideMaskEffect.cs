//#define DEBUG_EFFECT



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;



public class FreshGuideMaskEffect : MonoBehaviour {
	
	#region Mono

	void Awake(){
		
	}

	void Start(){
		#if DEBUG_EFFECT
		Debug.Log( "FreshGuideMaskEffect.Start( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif
	}

	void OnEnable(){
		#if DEBUG_EFFECT
		Debug.Log( "FreshGuideMaskEffect.OnEnable( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		InitSparkle();
	}

	void Update(){
		if (m_ui_tex != null) {
			if (m_ui_tex.material != null) {
				UpdateSparkle();
			}
		}
	}

	void OnDisable(){
		#if DEBUG_EFFECT
		Debug.Log( "FreshGuideMaskEffect.OnDisable( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif
	}

	void OnDestroy(){
		#if DEBUG_EFFECT
		Debug.Log( "FreshGuideMaskEffect.OnDestroy( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif
	}

	#endregion



	#region Use

	private void InitSparkle(){
		{
			m_sprite = GetComponent<UISprite>();

			{
				m_tex = m_sprite.mainTexture;

				m_sprite_data = m_sprite.atlas.GetSprite( m_sprite.spriteName );

				m_sprite_name = m_sprite.spriteName;

				if( m_sprite_data == null ){
//					#if UNITY_EDITOR
//					Debug.Log( "No Sprite data setted." );
//					#endif

					return;
				}

				#if DEBUG_EFFECT
				LogInfo();
				#endif
			}
		}

		{
			m_ui_tex = (UITexture)ComponentHelper.AddIfNotExist( gameObject, typeof(UITexture) );

			m_ui_tex.panel = m_sprite.panel;

			m_ui_tex.depth = m_sprite.depth;

			m_sprite.enabled = false;

			m_ui_tex.hideFlags = HideFlags.HideInInspector;

			{
				Vector4 t_dd = m_sprite.drawingDimensions;

				m_ui_tex.width = (int)( t_dd.z - t_dd.x );

				m_ui_tex.height = (int)( t_dd.w - t_dd.y );

				Vector3 t_vec3 = m_sprite.gameObject.transform.localPosition;

				m_ui_tex.pivot = m_sprite.pivot;

				m_ui_tex.gameObject.transform.localPosition = t_vec3;

				m_ui_tex.material = ComponentHelper.NewMaterial( "Custom/Effects/Fresh Guide" );

				m_ui_tex.material.mainTexture = m_tex;

				m_ui_tex.material.SetVector( "_Tex_ST", GetVec4() );
			}
		}
	}

	#endregion



	#region Set

	#endregion


	
	#region Utilities

	private void UpdateSparkle(){
		Vector4 t_dd = m_sprite.drawingDimensions;

		m_ui_tex.width = (int)( t_dd.z - t_dd.x );

		m_ui_tex.height = (int)( t_dd.w - t_dd.y );
	}

	#endregion



	#region Clean

	private void LogInfo(){
		ComponentHelper.LogTexture( m_tex );

		if( m_sprite_data != null ){
			m_sprite_data.Log();
		}
	}

	#endregion



	#region Callbacks

	#endregion



	#region Utilities

	private Vector4 GetVec4(){
		return new Vector4( 
			1.0f * ( m_sprite_data.width ) / m_sprite.atlas.AtlasWidth,
			1.0f * ( m_sprite_data.height ) / m_sprite.atlas.AtlasHeight,
			1.0f * ( m_sprite_data.x ) / m_sprite.atlas.AtlasWidth,
			1 - 1.0f * ( m_sprite_data.y + m_sprite_data.height ) / m_sprite.atlas.AtlasHeight );	
	}

	// self or mirror
	private UITexture m_ui_tex 	= null;

	private Texture m_tex 		= null;

	private UISprite m_sprite 	= null;
	
	private UISpriteData m_sprite_data 	= null;

	private string m_sprite_name = "";

	private Shader m_origin_sh	= null;

	#endregion

}
#define FX_MADE_BOSS



//#define DEBUG_OCCLUSION
//
//#define DEBUG_DEAD
//
//#define DEBUG_BOSS
//
//#define DEBUG_HITTED



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;



public class EffectTool : Singleton<EffectTool> {


	#region Mono

	void OnDestroy(){
		ClearBoss();

		base.OnDestroy();
	}

	#endregion



	#region Occlusion City

	public static void DisableCityOcclusion( GameObject p_object ){
		if( p_object == null ){
			return;
		}

		#if DEBUG_OCCLUSION
		Debug.Log( "DisableCityOcclusion( " + p_object + " )" );

		GameObjectHelper.LogGameObjectInfo( p_object );
		#endif

//		{
//			FindCharHL();
//
//			FindCharOC();
//		}

		{
			ShaderHelper.Replace<SkinnedMeshRenderer>( p_object, m_sl_char_oc, m_sl_char_hl );
			
			ShaderHelper.Replace<MeshRenderer>( p_object, m_sl_char_oc, m_sl_char_hl );
		}
	}

	private static Shader m_sl_char_oc			= null;

	private static Shader m_sl_char_hl			= null;

	private static void FindCharOC(){
		if( true ){
			return;
		}

		if( m_sl_char_oc == null ){
			m_sl_char_oc = Shader.Find( "Custom/Characters/Occlusion Colored" );
		}
	}

	private static void FindCharHL(){
		if( true ){
			return;
		}

		if( m_sl_char_hl == null ){
			m_sl_char_hl = Shader.Find( "Custom/Characters/Main Texture High Light" );
		}
	}

	#endregion



	#region Occlusion BattleField

	public static void DisableBattleOcclusion( GameObject p_object ){
		if( true ){
			return;
		}

		#if DEBUG_OCCLUSION
		Debug.Log( "DisableOcclusion( " + p_object + " )" );
		#endif

		if( p_object == null ){
			return;
		}

		{
			FindFxOC();
		}
		
		{
			DisableBattleOcclusionRenderers<SkinnedMeshRenderer>( p_object, m_sl_effect_oc );
		
			DisableBattleOcclusionRenderers<MeshRenderer>( p_object, m_sl_effect_oc );
		}

		#if DEBUG_OCCLUSION
		Debug.Log( "DisableOcclusion.Done." );
		#endif
	}

	/// Only Should be used in BattleField Occlusion.
	private static void DisableBattleOcclusionRenderers<T>( GameObject p_object, Shader p_shader ) where T : Renderer{
		if( true ){
			return;
		}

		T[] t_renderers = p_object.GetComponentsInChildren<T>();
		
		for ( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}
				
				if ( t_mat.shader == p_shader ){
					StoreMaterialInfo( p_object, t_mats[j] );
					
					t_mats[j] = null;
					
					Material[] t_new_mats = new Material[ t_mats.Length - 1 ];
					
					{
						int t_index = 0;
						
						for( int k = 0; k < t_mats.Length; k++ ){
							if( t_mats[ k ] != null ){
								t_new_mats[ t_index++ ] = t_mats[ k ];
							}
						}
					}
					
					t_renderer.materials = t_new_mats;
					
					break;
				}
			}
		}
	}

	/// Only Should be used in BattleField Occlusion.
	private static void StoreMaterialInfo( GameObject p_object, Material p_mat ){
		if( true ){
			return;
		}

		StoreGameObjectData t_data = p_object.GetComponent<StoreGameObjectData>();
		
		if ( t_data == null ){
			t_data = p_object.AddComponent<StoreGameObjectData>();
		}
		
		t_data.m_shared_mat = p_mat;
	}
	
	public static void RestoreBattleOcclusion( GameObject p_object ){
		if( true ){
			return;
		}

		#if DEBUG_OCCLUSION
		Debug.Log( "RestoreOcclusion( " + p_object + " )" );
		#endif

		StoreGameObjectData t_data = p_object.GetComponent<StoreGameObjectData>();
		
		if ( t_data == null ){
			// Debug.LogError("StoreGameObjectData == null.");
			
			return;
		}
		
		{
			RestoreBattleOcclusionRenderer<SkinnedMeshRenderer>(p_object, t_data);
			
			RestoreBattleOcclusionRenderer<MeshRenderer>(p_object, t_data);
		}

		#if DEBUG_OCCLUSION
		Debug.Log( "RestoreOcclusion.Done." );
		#endif
	}

	/// Only Should be used in BattleField Occlusion.
	private static void RestoreBattleOcclusionRenderer<T>( GameObject p_object, StoreGameObjectData p_data ) where T : Renderer{
		if( true ){
			return;
		}

		T[] t_renderers = p_object.GetComponentsInChildren<T>();
		
		for (int i = 0; i < t_renderers.Length; i++)
		{
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			bool t_found = false;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[j];
				
				if( t_mat.shader == p_data.m_shared_mat.shader ){
					t_found = true;
				}
			}
			
			if( !t_found ){
				Material[] t_new_mats = new Material[ t_mats.Length + 1 ];
				
				{
					for( int k = 0; k < t_mats.Length; k++ ){
						t_new_mats[ k ] = t_mats[ k ];
					}
				}
				
				t_new_mats[ t_mats.Length ] = p_data.m_shared_mat;
				
				t_renderer.materials = t_new_mats;
			}
		}
	}

	private static Shader m_sl_effect_oc 		= null;
	
	private static void FindFxOC(){
		if( true ){
			return;
		}

		if( m_sl_effect_oc == null ){
			m_sl_effect_oc = Shader.Find( "Custom/Effects/Occlusion Colored" );
		}
	}

	#endregion


	
	#region Dead
	
	public static void DisableDeadEffect( GameObject p_gb ){
		#if DEBUG_DEAD
		Debug.Log( "DisableDeadEffect( " + p_gb + " )" );
		#endif

		if ( p_gb == null ){
			Debug.Log("Error, p_gb = null.");
			
			return;
		}
		
		{
			DisableDeadEffect<SkinnedMeshRenderer>( p_gb );
			
			DisableDeadEffect<MeshRenderer>( p_gb );
		}

		#if DEBUG_DEAD
		Debug.Log( "DisableDeadEffect.Done." );
		#endif
	}
	
	private static void DisableDeadEffect<T>( GameObject p_gb ) where T : Renderer{
		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		if ( t_renderers == null || t_renderers.Length == 0 ){
			return;
		}
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			Renderer t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			if( t_mats.Length == 0 ){
				continue;
			}
			
			DissolveEffect t_script = t_renderer.gameObject.GetComponent<DissolveEffect>();
			
			if( t_script != null ){
				t_script.Restore();
			}
		}
	}
	
	
	public static void SetDeadEffect( GameObject p_gb ){
		#if DEBUG_DEAD
		Debug.Log( "SetDeadEffect( " + p_gb + " )" );
		#endif

		if ( p_gb == null ){
			Debug.Log("Error, p_gb = null.");
			
			return;
		}
		
		{
			SetDeadEffect<SkinnedMeshRenderer>( p_gb );
			
			SetDeadEffect<MeshRenderer>( p_gb );
		}

		#if DEBUG_DEAD
		Debug.Log( "SetDeadEffect.Done." );
		#endif
	}
	
	private static void SetDeadEffect<T>( GameObject p_gb ) where T : Renderer{
		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		if ( t_renderers == null || t_renderers.Length == 0 ){
			return;
		}
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			Renderer t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			Material t_main_mat = t_mats[0];
			
			DissolveEffect t_script = t_renderer.gameObject.GetComponent<DissolveEffect>();
			
			if( t_script == null ){
				t_script = t_renderer.gameObject.AddComponent<DissolveEffect>();
				
				t_script.m_mat = t_main_mat;
			}
		}
	}

	#endregion



	#region Boss Crushed Effect

	public static void SetBossEffectNormal( GameObject p_gb ){
		if( p_gb == null ){
			Debug.LogError( "Error, gb is null." );

			return;
		}

		BossEffect[] t_effects = p_gb.GetComponentsInChildren<BossEffect>();

		for( int i = 0; i < t_effects.Length; i++ ){
			t_effects[ i ].UseNormalMat();
		}
	}

	public static void SetBossEffectCrushed( GameObject p_gb, float p_time ){
		if( p_gb == null ){
			Debug.LogError( "Error, gb is null." );

			return;
		}

		BossEffect[] t_effects = p_gb.GetComponentsInChildren<BossEffect>();

		#if UNITY_EDITOR
		if( t_effects.Length == 0 ){
			Debug.Log( "Boss has no effect: " + p_gb );
		}
		#endif

		for( int i = 0; i < t_effects.Length; i++ ){
			t_effects[ i ].UseCrushMat( p_time );
		}
	}

	#endregion



	#region Enemy Player Effect

	// PVE's enemy player effect
	public static void SetEnemyPlayerEffect( GameObject p_gb ){
		EnemyPlayerEffect[] t_effects = p_gb.GetComponentsInChildren<EnemyPlayerEffect>();

		#if UNITY_EDITOR
		if( t_effects.Length == 0 ){
			Debug.Log( "Enemy Player has no effect." );
		}
		#endif

		for( int i = 0; i < t_effects.Length; i++ ){
			t_effects[ i ].UseEnemyEffect();
		}
	}

	#endregion



	#region Old Boss Effect
	
	private static Shader m_sl_boss_effect = null;
	
	private static List<GameObject> m_boss_target_list = new List<GameObject> ();
	
	private static Material m_boss_mat = null;

	private static void ClearBoss(){
		m_boss_target_list.Clear();

		m_sl_boss_effect = null;

		m_boss_mat = null;
	}

	public static void ClearBossEffect( GameObject p_gb ){
		if ( p_gb == null ){
			Debug.Log("Error, p_gb = null.");

			return;
		}

		{
			Shader t_origin = Shader.Find( "Custom/Characters/Main Texture Hight Light Rim" );

			Shader t_new = Shader.Find( "Custom/Characters/Main Texture High Light" );

			{
				Material[] t_mats = ComponentHelper.GetMaterialsWithShader<SkinnedMeshRenderer>( p_gb, t_origin );

				for( int i = 0; i < t_mats.Length; i++ ){
					Material t_mat = t_mats[ i ];

					if( t_mat != null ){
						t_mat.SetFloat( "_RimWidth", 8.0f );
					}
				}
			}

			{
				Material[] t_mats = ComponentHelper.GetMaterialsWithShader<MeshRenderer>( p_gb, t_origin );

				for( int i = 0; i < t_mats.Length; i++ ){
					Material t_mat = t_mats[ i ];

					if( t_mat != null ){
						t_mat.SetFloat( "_RimWidth", 8.0f );
					}
				}
			}

//			{
//				Material t_mat = ShaderHelper.Replace<SkinnedMeshRenderer>( p_gb, t_origin, t_new );
//			}
//
//			{
//				Material t_mat = ShaderHelper.Replace<MeshRenderer>( p_gb, t_origin, t_new );
//			}
		}
	}

	public static void SetBossEffect( GameObject p_gb, string p_color_str = "", float p_coef = 1.39f ){
		if ( p_gb == null ){
			Debug.Log("Error, p_gb = null.");
			
			return;
		}

		if (true) {
			return;
		}
		
		if ( !QualityTool.GetBool(QualityTool.CONST_BOSS_EFFECT) ){
			return;
		}

//		#if DEBUG_BOSS
//		Debug.Log( "SetBossEffect( " + p_gb + " )" );
//		#endif
				
//		{
//			m_boss_target_list.Add ( p_gb );
//			
//			string t_path = EffectIdTemplate.GetPathByeffectId ( 51021 );
//			
//			if (string.IsNullOrEmpty (t_path)) {
//				return;
//			}
//			
//			Global.ResourcesDotLoad( t_path, BossMatLoadCallback );
//		}

		{
			Shader t_origin = Shader.Find( "Custom/Characters/Main Texture High Light" );

			Shader t_new = Shader.Find( "Custom/Characters/Main Texture Hight Light Rim" );

			{
				Material t_mat = ShaderHelper.Replace<SkinnedMeshRenderer>( p_gb, t_origin, t_new );
			}
			
			{
				Material t_mat = ShaderHelper.Replace<MeshRenderer>( p_gb, t_origin, t_new );
			}

			{
				Color p_color = Color.red;

				MathHelper.ParseHexString( p_color_str, out p_color, Color.red );

				{
					Material[] t_mats = ComponentHelper.GetMaterialsWithShader<SkinnedMeshRenderer>( p_gb, t_new );

					for( int i = 0; i < t_mats.Length; i++ ){
						Material t_mat = t_mats[ i ];

						if( t_mat != null ){
//						{
//							t_mat.SetFloat( "_Coef", p_coef );
//
//							t_mat.SetColor( "_SKColor", p_color );
//						}

							SetBossMatParam( t_mat, p_color, p_coef );
						}
					}
				}

				{
					Material[] t_mats = ComponentHelper.GetMaterialsWithShader<MeshRenderer>( p_gb, t_new );

					for( int i = 0; i < t_mats.Length; i++ ){
						Material t_mat = t_mats[ i ];

						if( t_mat != null ){
//						{
//							t_mat.SetFloat( "_Coef", p_coef );
//
//							t_mat.SetColor( "_SKColor", p_color );
//						}

							SetBossMatParam( t_mat, p_color, p_coef );
						}
					}
				}
			}
		}

		#if DEBUG_BOSS
		Debug.Log( "SetBossEffect.Done." );
		#endif
	}

	private static void SetBossMatParam( Material p_mat, Color p_color, float p_coef ){
		if( p_mat == null ){
			return;
		}

		p_mat.SetColor( "_RimColor", p_color );

		p_mat.SetFloat( "_RimWidth", 2.0f );

		p_mat.SetFloat( "_RimWeight", 1.0f );
//		p_mat.SetColor( "_RimWeight", p_color );
	}
	
	private static void BossMatLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		m_boss_mat = (Material)p_object;
		
		#if DEBUG_BOSS
		Debug.Log( "BossMat.Loaded( " + m_boss_mat + " )" );
		#endif
		
		for( int i = 0; i < m_boss_target_list.Count; i++ ){
			GameObject t_gb = m_boss_target_list[ i ];
			
			{
				SetDetailBossEffect<SkinnedMeshRenderer>(t_gb);
				
				SetDetailBossEffect<MeshRenderer>(t_gb);
			}
		}
		
		m_boss_target_list.Clear();
	}
	
	private static void SetDetailBossEffect<T>( GameObject p_gb ) where T : Renderer{
		T[] t_renderers = p_gb.GetComponentsInChildren<T>();
		
		if ( t_renderers == null || t_renderers.Length == 0 ){
			return;
		}
		
		#if FX_MADE_BOSS
		#else
		if (m_boss_effect == null)
		{
			m_boss_effect = Shader.Find("Custom/Effects/Boss Effect");
			
			if (m_boss_effect == null)
			{
				Debug.LogError("Error, Boss Effect not found.");
				
				return;
			}
		}
		#endif
		
		for ( int i = 0; i < t_renderers.Length; i++ ){
			Renderer t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			#if FX_MADE_BOSS
			Material[] t_new_mats = new Material[ t_mats.Length + 1 ];
			
			for( int j = 0; j < t_mats.Length; j++ ){
				t_new_mats[ j ] = t_mats[ j ];
			}
			
			t_new_mats[ t_mats.Length ] = m_boss_mat;
			
			t_renderer.materials = t_new_mats;
			#else
			Material t_main_mat = t_mats[0];
			
			Texture t_tex = t_main_mat.mainTexture;
			
			t_main_mat.shader = m_boss_effect;
			
			t_main_mat.mainTexture = t_tex;
			
			DynamicGlow t_script = t_renderer.gameObject.GetComponent<DynamicGlow>();
			
			if (t_script == null)
			{
				t_script = t_renderer.gameObject.AddComponent<DynamicGlow>();
				
				t_script.m_mat = t_main_mat;
			}
			#endif
		}
	}

	#endregion



	#region Hitted
	
	public void SetHittedEffect( GameObject p_gb, bool p_is_soldier_hitted = true ){
//		if( true ){
//			return;
//		}

		#if DEBUG_HITTED
		Debug.Log( "SetHittedEffect( " + p_gb + " )" );
		#endif

		HittedEffect t_hitted = p_gb.GetComponent<HittedEffect>();

		Shader t_origin = Shader.Find( "Custom/Characters/Main Texture High Light" );

		Shader t_new = Shader.Find( "Custom/Characters/Main Texture Hight Light Rim" );

		if ( t_hitted == null ){
			t_hitted = p_gb.AddComponent<HittedEffect>();

			// enemies but boss
			{
//				Material t_mat_skin = ShaderHelper.Replace<SkinnedMeshRenderer>( p_gb, t_origin, t_new );
//
//				Material t_mat = ShaderHelper.Replace<MeshRenderer>( p_gb, t_origin, t_new );

//				if( t_mat_skin == null && t_mat == null ){
//					t_hitted.SaveBossEffect( t_new );
//				}
			}
		}
		else{
			t_hitted.InitAnim();
		}

		t_hitted.SetIsSoldierHitted( p_is_soldier_hitted );

		#if DEBUG_HITTED
		Debug.Log( "SetHittedEffect.Done." );
		#endif
	}
	
	#endregion



	#region UI Effect

	/** Desc:
	 * UISprite effect for scaling and alphaing and fx particles
	 * 
	 * Params:
	 * p_gb: target gameobject
	 * p_source_effect_id: param 0 from ZhongZhenWei
	 * p_mirror_effect_id: param 1 from ZhongZhenWei
	 * p_fx_id: param 1 from ZhongZhenWei
	 */
	public static void OpenMultiUIEffect_ById( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id ){
		UIMultiAnimEffect.OpenUIEffect( p_gb, p_source_effect_id, p_mirror_effect_id, p_fx_id );	
	}

	public static void CloseMultiUIEffect_ById( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id  ){
		UIMultiAnimEffect.CloseUIEffect( p_gb, p_source_effect_id, p_mirror_effect_id, p_fx_id );
	}

	public static void OpenUIEffect_ById( GameObject p_gb, int p_source_effect_id ){
		UIAnimEffect.OpenUIEffect( p_gb, p_source_effect_id );	
	}

	public static void CloseUIEffect_ById( GameObject p_gb, int p_source_effect_id  ){
		UIAnimEffect.CloseUIEffect( p_gb, p_source_effect_id );
	}

	#endregion



	#region UI Background Effect

	/// Desc:
	/// Open or Close Camera's effect.
	/// 
	/// Param:
	/// p_gb: give camera gb here.
	public static UIBackgroundEffect SetUIBackgroundEffect( GameObject p_gb, bool p_enable ){
		return UIBackgroundEffect.SetUIBackgroundEffect( p_gb, p_enable );
	}

	#endregion



	#region Story Vignet

	public static void StoryVignet( GameObject p_cam_gb ){
		if( Quality_IEStatus.IsStatusNone() ){
			return;
		}

		if( p_cam_gb == null ){
			Debug.LogError( "Error, cam gb is null." );

			return;
		}


		Camera t_cam = p_cam_gb.GetComponent<Camera>();

		if( t_cam == null ){
			Debug.LogError( "Cam not exist." );

			return;
		}

		ComponentHelper.AddIfNotExist( p_cam_gb, typeof(Vignetting) );
	}

	#endregion
}

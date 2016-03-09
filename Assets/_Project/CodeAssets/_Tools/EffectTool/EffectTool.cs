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

	void Update(){
	
	}

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

		{
			FindCharHL();

			FindCharOC();
		}

		{
			ShaderHelper.Replace<SkinnedMeshRenderer>( p_object, m_sl_char_oc, m_sl_char_hl );
			
			ShaderHelper.Replace<MeshRenderer>( p_object, m_sl_char_oc, m_sl_char_hl );
		}
	}

	private static Shader m_sl_char_oc			= null;

	private static Shader m_sl_char_hl			= null;

	private static void FindCharOC(){
		if( m_sl_char_oc == null ){
			m_sl_char_oc = Shader.Find( "Custom/Characters/Occlusion Colored" );
		}
	}

	private static void FindCharHL(){
		if( m_sl_char_hl == null ){
			m_sl_char_hl = Shader.Find( "Custom/Characters/Main Texture High Light" );
		}
	}

	#endregion



	#region Occlusion BattleField

	public static void DisableBattleOcclusion( GameObject p_object ){
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
		StoreGameObjectData t_data = p_object.GetComponent<StoreGameObjectData>();
		
		if ( t_data == null ){
			t_data = p_object.AddComponent<StoreGameObjectData>();
		}
		
		t_data.m_shared_mat = p_mat;
	}
	
	public static void RestoreBattleOcclusion( GameObject p_object ){
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



	#region Boss
	
	private static Shader m_sl_boss_effect = null;
	
	private static List<GameObject> m_boss_target_list = new List<GameObject> ();
	
	private static Material m_boss_mat = null;

	private static void ClearBoss(){
		m_boss_target_list.Clear();

		m_sl_boss_effect = null;

		m_boss_mat = null;
	}

	public static void SetBossEffect( GameObject p_gb, string p_color_str = "", float p_coef = 1.39f ){
		if ( p_gb == null ){
			Debug.Log("Error, p_gb = null.");
			
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

			Shader t_new = Shader.Find( "Custom/Characters/Stroke High Light" );

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
					Material t_mat = ComponentHelper.GetMaterialWithShader<SkinnedMeshRenderer>( p_gb, t_new );
					
					if( t_mat != null ){
						t_mat.SetFloat( "_Coef", 1.39f );
					
						t_mat.SetColor( "_SKColor", p_color );
					}
				}

				{
					Material t_mat = ComponentHelper.GetMaterialWithShader<MeshRenderer>( p_gb, t_new );
					
					if( t_mat != null ){
						t_mat.SetFloat( "_Coef", 1.39f );

						t_mat.SetColor( "_SKColor", p_color );
					}
				}
			}
		}

		#if DEBUG_BOSS
		Debug.Log( "SetBossEffect.Done." );
		#endif
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
		
		m_boss_target_list.Clear ();
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
	
	public void SetHittedEffect( GameObject p_gb ){
		#if DEBUG_HITTED
		Debug.Log( "SetHittedEffect( " + p_gb + " )" );
		#endif

		HittedEffect t_hitted = p_gb.GetComponent<HittedEffect>();
		
		if ( t_hitted == null ){
			t_hitted = p_gb.AddComponent<HittedEffect>();
		}
		
		t_hitted.Init();

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
	public static void OpenUIEffect_ById( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id ){
		UIAniEffectItem.OpenUIEffect( p_gb, p_source_effect_id, p_mirror_effect_id, p_fx_id );	
	}

	public static void CloseUIEffect_ById( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id  ){
		UIAniEffectItem.CloseUIEffect( p_gb, p_source_effect_id, p_mirror_effect_id, p_fx_id );
	}

	#endregion



	#region UI Background Effect

	/// Desc:
	/// Open or Close Camera's effect.
	public static UIBackgroundEffect SetUIBackgroundEffect( GameObject p_gb, bool p_enable ){
		return UIBackgroundEffect.SetUIBackgroundEffect( p_gb, p_enable );
	}

	#endregion
}

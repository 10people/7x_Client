//#define DEBUG_EFFECT


using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.2.2
 * @since:		Unity 5.1.3
 * Function:	QuanMinWuSHuang's XiaHou Skill, a large sprite on his head.
 * 
 * Notes:
 * None.
 */ 
public class StrEffectItem : MonoBehaviour {

	private const float m_scale_time = 0.2f;

	private static List<StrEffectContainer> m_str_load_list = new List<StrEffectContainer>();

	private static List<StrEffectContainer> m_str_list = new List<StrEffectContainer>();

//	private static StrEffectItem m_instance = null;
//
//	#region Instance
//
//	public static StrEffectItem Instance(){
//		if( m_instance == null ){
//			m_instance = (StrEffectItem)ComponentHelper.AddIfNotExist( GameObjectHelper.GetTempGameObjectsRoot(), typeof(StrEffectItem) );
//		}
//
//		return m_instance;
//	}
//
//	#endregion



	#region Mono

	private float m_start_time = 0.0f;

	private StrEffectContainer m_target_eff = null;

	private List<Renderer> m_renderers = new List<Renderer>();

	void Start(){
		m_start_time = Time.realtimeSinceStartup;

		{
			SkinnedMeshRenderer[] t_renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

			for( int i = 0; i < t_renderers.Length; i++ ){
				m_renderers.Add( t_renderers[ i ] );
			}
		}

		{
			MeshRenderer[] t_renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
			
			for( int i = 0; i < t_renderers.Length; i++ ){
				m_renderers.Add( t_renderers[ i ] );
			}
		}
	}

	void Update(){
		if( m_target_eff == null ){
			Debug.Log( "target is null." );

			return;
		}

		if( m_target_eff.m_target_gb == null ){
			Debug.Log( "target is null." );
			
			return;
		}

		bool t_visible = false;

		if( ( Time.realtimeSinceStartup - m_start_time ) > m_target_eff.m_time_offset &&
		   ( Time.realtimeSinceStartup - m_start_time ) < ( m_target_eff.m_time_offset + m_target_eff.m_time_len ) ){
			t_visible = true;
		}

		SetVisible( t_visible );

		if( !t_visible ){
			return;
		}

		transform.position = m_target_eff.m_target_gb.transform.position + new Vector3( 0, m_target_eff.m_offset_y, 0 );

		transform.rotation = m_target_eff.m_target_gb.transform.rotation;

		m_target_eff.m_anim.Play( m_target_eff.m_target_anim.GetCurrentAnimatorStateInfo( 0 ).shortNameHash, -1,  m_target_eff.m_target_anim.GetCurrentAnimatorStateInfo( 0 ).normalizedTime );

		{
			float t_p = Mathf.Clamp01( ( Time.realtimeSinceStartup - m_start_time - m_target_eff.m_time_offset ) / m_scale_time );

			float t_s = Mathf.Lerp( 0, m_target_eff.m_scale, t_p );

			transform.localScale = new Vector3( t_s, t_s, t_s );
		}
	}

	void OnDestroy(){
		{
			m_target_eff.m_target_gb = null;

			Clean();
		}
	}

	#endregion


	#region Use

	public static void CloseEffect( GameObject p_target ){
		#if DEBUG_EFFECT
		Debug.Log( "CloseEffect( " + p_target + " )" );
		#endif

		StrEffectContainer[] t_containers = GetTargetContainer( p_target );

		if( t_containers.Length == 0 ){
//			#if DEBUG_EFFECT
//			Debug.LogError( "Error, no effect found." );
//			#endif

			return;
		}

		for( int i = 0; i < t_containers.Length; i++ ){
			Destroy( t_containers[ i ].m_gb );
		}
	}

	/// ModelId#Scale#offsetY#Color#Coef#TimeOffset#TimeLen
	/// 3002#2.0#2.0#ff0000#0.74#0.0#10.0
	public static void OpenEffect( GameObject p_target, string p_fx_3d, int p_self_model_id ){
		if( string.IsNullOrEmpty( p_fx_3d ) ){
			return;
		}

		#if DEBUG_EFFECT
		Debug.Log( "OpenEffect( " + p_target + " , " + p_fx_3d + " )" );

		GameObjectHelper.LogGameObjectHierarchy( p_target );
		#endif

		string[] t_items = p_fx_3d.Split( '#' );

		if( t_items.Length < 7 ){
			Debug.LogError( "len not enough." );

			return;
		}

		#if DEBUG_EFFECT
		for( int i = 0; i < t_items.Length; i++ ){
			Debug.Log( i + " : " + t_items[ i ] );
		}
		#endif

		int t_model_id = 3002;

		string t_model_path = "";

		float t_scale = 2.0f;

		float t_offset_y = 2.0f;

		Color t_color = Color.red;

		float t_coef = 0.74f;

		float t_time_offset = 0.0f;

		float t_time_len = 10.0f;

		try{
			t_model_id = int.Parse( t_items[ 0 ] );

			if( t_model_id < 0 ){
				#if DEBUG_EFFECT
				Debug.Log( "Use Self Model." );
				#endif

//				t_model_id = p_self_model_id;
			}
			else{
				t_model_path = ModelTemplate.GetResPathByModelId( t_model_id );
			}

			t_scale = float.Parse( t_items[ 1 ] );
			
			t_offset_y = float.Parse( t_items[ 2 ] );

			MathHelper.ParseHexString( t_items[ 3 ], out t_color, Color.red );

			t_coef = float.Parse( t_items[ 4 ] );

			t_time_offset = float.Parse( t_items[ 5 ] );

			t_time_len = float.Parse( t_items[ 6 ] );
		}catch( Exception e ){
			Debug.LogError( "Exception: " + e );

			return;
		}

		#if DEBUG_EFFECT
		Debug.Log( "model id: " + t_model_id );

		Debug.Log( "model path: " + t_model_path );

		Debug.Log( "scale: " + t_scale );

		Debug.Log( "offset y: " + t_offset_y );

		Debug.Log( "color: " + t_color );

		Debug.Log( "coef: " + t_coef );

		Debug.Log( "Time Offset: " + t_time_offset );

		Debug.Log( "Time Len: " + t_time_len );
		#endif

		{
			StrEffectContainer t_container = new StrEffectContainer( p_target, t_model_path,
			                                               			t_color,
			                                                        t_offset_y, t_scale, t_coef,
			                                                        t_time_offset, t_time_len );

			m_str_load_list.Add( t_container );
		}
		
		Global.ResourcesDotLoad( t_model_path,
		                        ResourceLoadCallback );
	}

	public static void OpenEffect_Console( GameObject p_target, string p_effect, float p_offset  = 1.0f, float p_scale = 2.0f ){
		#if DEBUG_EFFECT
		Debug.Log( "OpenEffect_Console( " + p_target + " , " + p_effect + " )" );
		#endif

		string t_ef_path = "_3D/Models/BattleField/Monster" + p_effect;

		#if DEBUG_EFFECT
		Debug.Log( "ef path: " + t_ef_path );
		#endif

		{
			StrEffectContainer t_container = new StrEffectContainer( p_target, t_ef_path,
			                                                        new Color( 0.3647f, 0.8353f, 0.866f, 1 ),
			                                                        p_offset, p_scale, 0.74f,
			                                                        0.0f,
			                                                        10.0f );

			m_str_load_list.Add( t_container );
		}

		Global.ResourcesDotLoad( t_ef_path,
		                        ResourceLoadCallback );
	}

	public static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		if( p_object == null && !string.IsNullOrEmpty( p_path ) ){
			Debug.LogError( "object to null." );
			
			return;
		}

		{
			Clean();
		}

		StrEffectContainer[] t_target = GetTargetContainerInLoading( p_path );
		
		if( t_target.Length == 0 ){
			Debug.LogError( "Target is null: " + p_path );

			LogInfo();

			return;
		}

		for( int i = 0; i < t_target.Length; i++ ){
			GameObject t_ef_gb = null;

			BaseAI t_self_base_ai = null;

			if( string.IsNullOrEmpty( p_path ) ){
				t_self_base_ai = t_target[ i ].m_target_gb.GetComponentInChildren<BaseAI>();

				if( t_self_base_ai == null ){
					Debug.LogError( "Self have no BaseAI." );

					continue;
				}

				if( t_self_base_ai.body == null ){
					Debug.LogError( "Self's BaseAI have no Body." );
					
					continue;
				}

				t_ef_gb = (GameObject)GameObject.Instantiate( t_self_base_ai.body );
			}
			else{
				t_ef_gb = (GameObject)GameObject.Instantiate( p_object );
			}

			{
				t_target[ i ].m_gb = t_ef_gb;
			}

			{
				ComponentHelper.ClearColliders( t_ef_gb );
				
				ComponentHelper.ClearMonos( t_ef_gb );
				
				ComponentHelper.ClearComponents<NavMeshAgent>( t_ef_gb );
				
				ComponentHelper.ClearComponents<CharacterController>( t_ef_gb );

				ComponentHelper.ClearComponents<AudioListener>( t_ef_gb );

				ComponentHelper.ClearComponents<AudioSource>( t_ef_gb );
			}

			Animator t_target_anim = t_target[ i ].m_target_gb.GetComponentInChildren<Animator>();

			{
				t_ef_gb.transform.parent = GameObjectHelper.GetTempGameObjectsRoot().transform;
				
				t_ef_gb.AddComponent<DevelopAnimationCallback>();
			}

			StrEffectItem t_eff = t_ef_gb.AddComponent<StrEffectItem>();

			{
				t_eff.m_target_eff = t_target[ i ];
			}

			{
				Animator t_anim = t_ef_gb.GetComponentInChildren<Animator>();

				{
					t_eff.m_target_eff.m_anim = t_anim;

					t_eff.m_target_eff.m_target_anim = t_target_anim;
				}

				t_anim.runtimeAnimatorController = t_target_anim.runtimeAnimatorController;

				t_anim.Play( t_target_anim.GetCurrentAnimatorStateInfo( 0 ).shortNameHash, -1, t_target_anim.GetCurrentAnimatorStateInfo( 0 ).normalizedTime );

				Shader t_old_0 = Shader.Find( "Custom/Characters/Stroke High Light" );
				
				Shader t_old_1 = Shader.Find( "Custom/Characters/Main Texture High Light" );

				Shader t_new = Shader.Find( "Custom/Effects/StrEffect" );

				ShaderHelper.ReplaceAll<SkinnedMeshRenderer>( t_ef_gb, t_old_0, t_new );
				
				ShaderHelper.ReplaceAll<SkinnedMeshRenderer>( t_ef_gb, t_old_1, t_new );

				{
					Material[] t_mats = ComponentHelper.GetMaterialsWithShader<SkinnedMeshRenderer>( t_ef_gb, t_new );

					for( int j = 0; j < t_mats.Length; j++ ){
						Material t_mat = t_mats[ j ];

						if( t_mat != null ){
							t_mat.SetFloat( "_Coef", t_target[ i ].m_coef );
							
							t_mat.SetColor( "_SKColor", t_target[ i ].m_color );
						}
						else{
							#if DEBUG_EFFECT
							Debug.LogError( "Error, param not setted." );
							#endif
						}
					}
				}

				ShaderHelper.ReplaceAll<MeshRenderer>( t_ef_gb, t_old_0, t_new );
				
				ShaderHelper.ReplaceAll<MeshRenderer>( t_ef_gb, t_old_1, t_new );

				{
					Material[] t_mats = ComponentHelper.GetMaterialsWithShader<MeshRenderer>( t_ef_gb, t_new );
					
					for( int j = 0; j < t_mats.Length; j++ ){
						Material t_mat = t_mats[ j ];
						
						if( t_mat != null ){
							t_mat.SetFloat( "_Coef", t_target[ i ].m_coef );
							
							t_mat.SetColor( "_SKColor", t_target[ i ].m_color );
						}
						else{
							#if DEBUG_EFFECT
							Debug.LogError( "Error, param not setted." );
							#endif
						}
					}
				}
			}

			m_str_load_list.Remove( t_target[ i ] );

			m_str_list.Add( t_target[ i ] );
		}
	}

	#endregion



	#region Clean

	public static void Clean(){
		for( int i = m_str_load_list.Count - 1; i >= 0; i-- ){
			StrEffectContainer t_container = m_str_load_list[ i ];

			if( t_container.m_target_gb == null ){
				m_str_load_list.Remove( t_container );
			}
		}

		for( int i = m_str_list.Count - 1; i >= 0; i-- ){
			StrEffectContainer t_container = m_str_list[ i ];
			
			if( t_container.m_target_gb == null ){
				m_str_list.Remove( t_container );
			}
		}
	}

	private static StrEffectContainer[] GetTargetContainerInLoading( string p_path ){
		List<StrEffectContainer> t_gb_list = new List<StrEffectContainer>();

		for( int i = 0; i < m_str_load_list.Count; i++ ){
			StrEffectContainer t_container = m_str_load_list[ i ];

			if( t_container.m_eff_path == p_path ){
				t_gb_list.Add( t_container );
			}
		}

		return t_gb_list.ToArray();
	}

	private static StrEffectContainer[] GetTargetContainer( GameObject p_gb ){
		List<StrEffectContainer> t_gb_list = new List<StrEffectContainer>();
		
		for( int i = 0; i < m_str_list.Count; i++ ){
			StrEffectContainer t_container = m_str_list[ i ];
			
			if( t_container.m_target_gb == p_gb ){
				t_gb_list.Add( t_container );
			}
		}
		
		return t_gb_list.ToArray();
	}

	private static void LogInfo(){
		Debug.Log( "LogInfo()" );

		for( int i = 0; i < m_str_load_list.Count; i++ ){
			StrEffectContainer t_container = m_str_load_list[ i ];
			
			Debug.Log( t_container.m_target_gb + " : " + t_container.m_eff_path );
		}
	}

	#endregion



	#region Callbacks

	public void PlaySound( string p_path ){

	}

	public static void PlaySound( int p_sound_id, GameObject p_gb ){
	
	}

	#endregion



	#region Utilities

	private void SetVisible( bool p_visible ){
		if( m_renderers == null ){
			#if DEBUG_EFFECT
			Debug.LogError( "No renderers exist." );
			#endif

			return;
		}

//		#if DEBUG_EFFECT
//		Debug.Log( "SetVisible: " + p_visible );
//		#endif
		
		for( int i = 0; i < m_renderers.Count; i++ ){
			m_renderers[ i ].enabled = p_visible;
		}
	}

	#endregion

	private class StrEffectContainer{
		public GameObject m_gb = null;

		public GameObject m_target_gb = null;

		public Animator m_anim = null;

		public Animator m_target_anim = null;

		public string m_eff_path = "";

		public float m_offset_y = 1.0f;

		public float m_scale = 2.0f;

		public Color m_color = new Color( 0.3647f, 0.8353f, 0.866f, 1 );

		public float m_coef = 0.74f;

		public float m_time_offset = 0.0f;

		public float m_time_len = 10.0f;

		public StrEffectContainer( GameObject p_target, string p_effect, Color p_color, 
		                          float p_offset  = 1.0f, float p_scale = 2.0f, float p_coef = 0.74f,
		                          float p_time_offset = 0.0f, float p_time_len = 10.0f ){
			m_target_gb = p_target;

			m_eff_path = p_effect;

			m_offset_y = p_offset;

			m_scale = p_scale;

			m_color = p_color;

			m_coef = p_coef;

			m_time_offset = p_time_offset;

			m_time_len = p_time_len;
		}
	}
}
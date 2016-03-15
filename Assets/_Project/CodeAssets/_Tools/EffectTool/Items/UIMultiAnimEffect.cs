//#define DEBUG_EFFECT



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UIMultiAnimEffect : MonoBehaviour {
	// mirror
	private GameObject m_mirror_gb = null;

	// fx
	private GameObject m_fx_gb = null;

	// self
	private Animator m_source_anim = null;



	private int m_source_effect_id = -1;

	private int m_mirror_effect_id = -1;

	private int m_fx_effect_id = -1;



	private const int D_OFFSET	= 10;

	#region Mono

	void Start(){
		Init();
	}

	void Update(){

	}

	void OnDestroy(){
		if( m_mirror_gb != null ){
			m_mirror_gb.SetActive( false );

			Destroy( m_mirror_gb );
		}

		if( m_source_anim != null ){
			#if DEBUG_EFFECT
			Debug.Log( "source try rewind." );
			#endif

			AnimatorHelper.RewindToEnd( m_source_anim );

			m_source_anim.enabled = false;

			Destroy( m_source_anim );
		}

		if( m_fx_gb != null ){
			m_fx_gb.SetActive( false );

			Destroy( m_fx_gb );
		}
	}

	#endregion



	#region Use

	public static void CloseUIEffect( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id ){
		#if DEBUG_EFFECT
		Debug.Log( "CloseUIEffect( " + p_gb + " )" );
		#endif

		if( p_gb == null ){
			Debug.Log( "No gameobject passed." );
			
			return;
		}

		UIMultiAnimEffect t_effect = p_gb.GetComponent<UIMultiAnimEffect>();
		
		if( t_effect == null ){
//			Debug.Log( "No Ani found for: " + p_gb );

//			GameObjectHelper.LogGameObjectHierarchy( p_gb );
			
			return;
		}

		Destroy( t_effect );
	}

	/// Pass gameobject with target UISprite here
	public static void OpenUIEffect( GameObject p_gb, int p_source_effect_id, int p_mirror_effect_id, int p_fx_id ){
		#if DEBUG_EFFECT
		Debug.Log( "OpenUIEffect( " + p_gb + " - " + p_source_effect_id + " )" );
		#endif

		if( p_gb == null ){
			Debug.Log( "No gameobject passed." );

			return;
		}

		UIMultiAnimEffect t_effect = p_gb.GetComponent<UIMultiAnimEffect>();

		if( t_effect != null ){
			Debug.Log( "UI Effect already exist, return now.");

			return;
		}

		Animator t_anim = p_gb.GetComponent<Animator>();

		if( t_anim != null ){
			Debug.Log( "Animator already exist, return now." );

			return;
		}

		GameObject t_gb = GameObject.Instantiate( p_gb );

		t_effect = p_gb.AddComponent<UIMultiAnimEffect>();

		{
			#if DEBUG_EFFECT
			Debug.Log( "p_source_effect_id = " +p_source_effect_id);
			Debug.Log( "m_mirror_effect_id = " +p_mirror_effect_id);
			Debug.Log( "m_fx_effect_id = " +p_fx_id);
			#endif

			t_effect.m_mirror_gb = t_gb;

			t_effect.m_source_effect_id = p_source_effect_id;

			t_effect.m_mirror_effect_id = p_mirror_effect_id;

			t_effect.m_fx_effect_id = p_fx_id;
		}

		CleanGameObject( t_effect );
	}

	#endregion


	
	#region Clean

	private void LogInfo(){
		Debug.Log( "LogInfo()" );


	}
	
	#endregion



	#region Callbacks

	#endregion



	#region Utilities

	private static void CleanGameObject( UIMultiAnimEffect p_anim ){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.CleanGameObject()" );
		#endif

		ComponentHelper.ClearColliders( p_anim.m_mirror_gb );

		ComponentHelper.ClearMonosWithoutNGUI( p_anim.m_mirror_gb );

		p_anim.m_mirror_gb.transform.parent = p_anim.gameObject.transform.parent;

		TransformHelper.CopyTransform( p_anim.gameObject, p_anim.m_mirror_gb );

		ComponentHelper.ShiftWidgetDepth( p_anim.gameObject, D_OFFSET );
	}

	private void Init(){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.Init()" );
		#endif

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( m_source_effect_id ), LoadSourceEffectCallback );

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( m_mirror_effect_id ), LoadMirrorEffectCallback );

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( m_fx_effect_id ), LoadFxCallback );
	}

	public void LoadSourceEffectCallback( ref WWW p_www,string p_path, UnityEngine.Object p_object ){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.LoadSourceEffectCallback()" );
		#endif

		if( p_object == null ){
			Debug.LogError ( "Effect Object is null: " + m_source_effect_id );

			return;
		}

		m_source_anim = gameObject.AddComponent<Animator>();

		m_source_anim.runtimeAnimatorController = (RuntimeAnimatorController)p_object;
	}

	public void LoadMirrorEffectCallback( ref WWW p_www,string p_path, UnityEngine.Object p_object ){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.LoadMirrorEffectCallback()" );
		#endif

		if( p_object == null ){
			Debug.LogError ( "Effect Object is null: " + m_source_effect_id );

			return;
		}

		{
			Animator t_anim = m_mirror_gb.AddComponent<Animator>();

			t_anim.runtimeAnimatorController = (RuntimeAnimatorController)p_object;
		}

		{
			int t_max = ComponentHelper.GetNGUIMaxDepth( gameObject );

			int t_min = ComponentHelper.GetNGUIMinDepth( m_mirror_gb );

			#if DEBUG_EFFECT
			Debug.Log( "self.max: " + t_max );

			Debug.Log( "mirror.min: " + t_min );
			#endif

			ComponentHelper.ShiftWidgetDepth( m_mirror_gb, - t_min + t_max + 1 );
		}
	}

	public void LoadFxCallback( ref WWW p_www,string p_path, UnityEngine.Object p_object ){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.LoadFxCallback()" );
		#endif

		if( p_object == null ){
			Debug.LogError ( "Effect Object is null: " + m_source_effect_id );

			return;
		}

		m_fx_gb = (GameObject)Instantiate( p_object );

		{
			TransformHelper.StoreTransform( m_fx_gb );

			m_fx_gb.transform.parent = gameObject.transform.parent;

			TransformHelper.RestoreTransform( m_fx_gb );

			m_fx_gb.transform.localPosition = gameObject.transform.localPosition;
		}

		{
			int t_max = ComponentHelper.GetNGUIMaxDepth( m_mirror_gb );

			int t_min = ComponentHelper.GetNGUIMinDepth( m_fx_gb );

			#if DEBUG_EFFECT
			Debug.Log( "mirror.max: " + t_max );

			Debug.Log( "fx.min: " + t_min );
			#endif

			ComponentHelper.ShiftWidgetDepth( m_fx_gb, - t_min + t_max + 1 );
		}
	}

	#endregion
}
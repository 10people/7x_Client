//#define DEBUG_EFFECT



using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UIAnimEffect : MonoBehaviour {
	// self
	private Animator m_source_anim = null;

	private int m_source_effect_id = -1;

	#region Mono

	void Start(){
		Init();
	}

	void OnDestroy(){
		if( m_source_anim != null ){
			#if DEBUG_EFFECT
			Debug.Log( "source try rewind." );
			#endif

			AnimatorHelper.RewindToEnd( m_source_anim );

			m_source_anim.enabled = false;

			Destroy( m_source_anim );
		}
	}

	#endregion



	#region Use

	public static void CloseUIEffect( GameObject p_gb, int p_source_effect_id ){
		#if DEBUG_EFFECT
		Debug.Log( "CloseUIEffect( " + p_gb + " )" );
		#endif

		if( p_gb == null ){
			Debug.Log( "No gameobject passed." );
			
			return;
		}

		UIAnimEffect t_effect = p_gb.GetComponent<UIAnimEffect>();
		
		if( t_effect == null ){
//			Debug.Log( "No Ani found for: " + p_gb );

//			GameObjectHelper.LogGameObjectHierarchy( p_gb );
			
			return;
		}

		Destroy( t_effect );
	}

	/// Pass gameobject with target UISprite here
	public static void OpenUIEffect( GameObject p_gb, int p_source_effect_id ){
		#if DEBUG_EFFECT
		Debug.Log( "OpenUIEffect( " + p_gb + " - " + p_source_effect_id + " )" );
		#endif

		if( p_gb == null ){
			Debug.Log( "No gameobject passed." );

			return;
		}

		UIAnimEffect t_effect = p_gb.GetComponent<UIAnimEffect>();

		if( t_effect != null ){
			Debug.Log( "UI Effect already exist, return now.");

			return;
		}

		Animator t_anim = p_gb.GetComponent<Animator>();

		if( t_anim != null ){
			Debug.Log( "Animator already exist, return now." );

			return;
		}

		t_effect = p_gb.AddComponent<UIAnimEffect>();

		{
			#if DEBUG_EFFECT
			Debug.Log( "p_source_effect_id = " +p_source_effect_id);
			#endif

			t_effect.m_source_effect_id = p_source_effect_id;
		}
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

	private void Init(){
		#if DEBUG_EFFECT
		Debug.Log( "AniEffect.Init()" );
		#endif

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( m_source_effect_id ), LoadSourceEffectCallback );
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

	#endregion
}
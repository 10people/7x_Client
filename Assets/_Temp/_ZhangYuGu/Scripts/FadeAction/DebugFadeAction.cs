using UnityEngine;
using System.Collections;

public class DebugFadeAction : MonoBehaviour {

	public float m_fade_time = 1.0f;


	private int m_animation_index = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log( GetCurClip().wrapMode + " Playing: " + GetComponent<Animation>().isPlaying + " - " + 
		          GetCurClip().name + " - " + 
		          GetComponent<Animation>()[ GetCurClip().name ].normalizedTime );

		if( !isAnimationPlaying() ){
			AnimationClip t_clip = GetAnimation( 0 );
			
			//animation.Play( t_clip.name );

			//animation.CrossFade( t_clip.name );

			Debug.Log( "Fading Action: " + t_clip.name + " - " + m_fade_time );

			GetComponent<Animation>().CrossFade( t_clip.name, m_fade_time );
		}
	}

	public void FadeAnimation(){
		m_animation_index++;
		
		AnimationClip t_clip = GetAnimation( m_animation_index );
		
		Debug.Log( "CrossFade Action: " + t_clip.name + " - " + m_fade_time );
		
		GetComponent<Animation>().CrossFade( t_clip.name, m_fade_time );
		
		NGUIDebug.ClearLogs();
		
		NGUIDebug.Log( m_animation_index + ": " + t_clip.name );
	}

	private AnimationClip GetAnimation( int p_index ){
		int t_i = 0;
		
		AnimationClip t_clip_0 = null;
		
		foreach( AnimationState t_clip in GetComponent<Animation>() ){
			if( t_clip_0 == null ){
				t_clip_0 = t_clip.clip;
			}
			
			if( t_i == p_index ){
				return t_clip.clip;   
			}
			
			t_i++;   
		}

		m_animation_index = 0;
		
		return t_clip_0;
	} 

	private bool isAnimationPlaying(){
		AnimationState t_state = GetCurClip();

		return ( GetComponent<Animation>().isPlaying && t_state.wrapMode != WrapMode.ClampForever ) || 
			( ( t_state.wrapMode == WrapMode.ClampForever ) && ( t_state.normalizedTime <= 1.0f ) );
	}

	public AnimationState GetCurClip(){
		foreach( AnimationState anim in GetComponent<Animation>() ){
			if( GetComponent<Animation>().IsPlaying( anim.name ) ){
				return anim;
			}
		}

		return null;
	}


}
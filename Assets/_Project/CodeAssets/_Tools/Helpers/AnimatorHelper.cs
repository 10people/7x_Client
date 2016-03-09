using UnityEngine;
using System.Collections;

public class AnimatorHelper 
{
	#region Parameter

	public static bool HaveParameter( Animator p_anim, string p_param ){
		if( p_anim == null ){
			return false;
		}

		AnimatorControllerParameter[] t_params = p_anim.parameters;

		for( int i = 0; i < t_params.Length - 1; i++ ){
			if( t_params[ i ].name == p_param ){
				return true;
			}
		}

		return false;
	}

	#endregion



	#region Play

	public static void RewindToEnd( Animator p_anim ){
		p_anim.Play( p_anim.GetCurrentAnimatorStateInfo( 0 ).shortNameHash, 
			-1, 
			1.0f );
		
		p_anim.Update( 0.0f );
	}

	#endregion



	#region Info

	public static string GetCurrentPlayingClipName( Animator p_anim ){
		if( p_anim == null ){
			return "";
		}

		AnimatorClipInfo[] t_states = p_anim.GetCurrentAnimatorClipInfo( 0 );

		for( int i = 0; i < t_states.Length; /*i++*/ ){
			AnimatorClipInfo t_item = t_states[ i ];

			return t_item.clip.name;
		}

		return "";
	}

	#endregion
}

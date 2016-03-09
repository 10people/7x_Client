using UnityEngine;
using System.Collections;

public class AnimationHelper 
{
    public static int GetAnimatorPlayingHash(Animator p_animator)
    {
        return p_animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    }

	public static string IsPlaying(Animator mAnim)
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetCurrentAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}
	
	public static string nextPlaying(Animator mAnim)
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetNextAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}
}

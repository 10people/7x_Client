using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NpcAnimationManagerment : MonoBehaviour
{
    string[] animationMame = new string[] { "Stand0", "Animation1", "Animation2", "Animation3"};

	public Animator m_Animator;
    private int m_iRandomNum;
    private int m_iCurNum;

    void Start()
    {
		m_Animator = transform.GetComponent<Animator>();
        
		m_iRandomNum = Global.getRandom(20) + 300;
//        InvokeRepeating("NpcAnimation", m_iRandomNum, 2.0f);
    }

	void OnDestroy(){
		m_Animator = null;
	}

//    void NpcAnimation()
//    {
//		Debug.Log("=====================1");
//        m_iRandomNum = Global.getRandom(50) + 300;
//        m_iCurNum = 0;
//        m_Animator.SetTrigger(animationMame[Global.getRandom(2) + 2]);
//    }

	public bool IsPlaying( string p_action_name )
	{
		if (m_Animator == null) return false;
		
		AnimatorClipInfo[] t_states = m_Animator.GetCurrentAnimatorClipInfo( 0 );
		
		if( t_states.Length > 1 ){
			//return false;
		}
		else if( t_states.Length == 0 ){
			//return false;
		}
		
		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];
			
			if( t_item.clip.name.Equals(p_action_name) )
			{
				return true;
			}
		}
		return false;
	}


    void Update()
    {
        m_iCurNum++;
        if (m_iCurNum >= m_iRandomNum)
        {
//			Debug.Log(m_iRandomNum);
//			Debug.Log(m_iCurNum);
			//Debug.Log(m_iRandomNum);
			//Debug.Log(m_iCurNum);
            m_iRandomNum = Global.getRandom(50) + 300;
            m_iCurNum = 0;
			m_Animator.SetTrigger(animationMame[Global.getRandom(2) + 2]);
        }
    }

    public void setDialogAnimation()
    {
		if(!IsPlaying(animationMame[1]))
		{
			m_Animator.SetTrigger(animationMame[1]);
		}
    }
}

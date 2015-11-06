using UnityEngine;
using System.Collections;

public class PlayerAutoNavManagerment : MonoBehaviour
{
    public Animator m_NavAnim;

    public void AnimController()
    {
        m_NavAnim.enabled = false;
        if (m_NavAnim.name.Equals("Label7"))
        {
            PlayeratuoMoveManagerMent.m_AnimOver = true;
        }
    }
 
}

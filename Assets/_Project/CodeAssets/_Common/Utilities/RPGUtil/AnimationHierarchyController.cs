using UnityEngine;
using System.Collections;

public class AnimationHierarchyController : MonoBehaviour
{
    public Animator m_Animator;

    /// <summary>
    /// Not used, use this for debug now.
    /// </summary>
    public string LatestSetAnimationName;

    public bool IsCloseAnimator;

    public void Play(string animationName)
    {
        LatestSetAnimationName = animationName;

        m_Animator.Play(animationName, -1, 0);
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
}
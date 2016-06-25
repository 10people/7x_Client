using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class AnimationHierarchyPlayer
{
    public PlayerManager m_PlayerManager;
    public SinglePlayerController m_SinglePlayerController;
    public Dictionary<int, int> m_PiorityDic = new Dictionary<int, int>();

    public AnimationHierarchyController TryGetController(int p_uid)
    {
        if (p_uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            if (m_SinglePlayerController == null)
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
                {
                    Debug.LogWarning("self player not exist");
                }
                return null;
            }
            return m_SinglePlayerController.GetComponent<AnimationHierarchyController>();
        }
        else
        {
            if (!m_PlayerManager.m_PlayerDic.ContainsKey(p_uid))
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
                {
                    Debug.LogWarning("other player not exist");
                }
                return null;
            }
            return m_PlayerManager.m_PlayerDic[p_uid].GetComponent<AnimationHierarchyController>();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="p_uid"></param>
    /// <param name="animationName">play animation using this name</param>
    /// <param name="isStrictlyPlay">donot consider hierarchy if true</param>
    /// <returns></returns>
    public void TryPlayAnimation(int p_uid, string animationName, bool isStrictlyPlay = false)
    {
        AnimationHierarchyController controller = TryGetController(p_uid);

        if (!IsCanPlayAnimation(p_uid, animationName, isStrictlyPlay))
        {
            return;
        }

        if (controller == null)
        {
            Debug.LogWarning("Cannot play animation cause cannot find player, " + p_uid);
            return;
        }

        //if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
        //{
        //    Debug.LogWarning("===========Play " + animationName + " in " + p_uid);
        //}

        controller.Play(animationName);

        if (animationName == "Dead")
        {
            controller.IsCloseAnimator = true;
        }
    }

    public bool IsCanPlayAnimation(int p_uid, string animationName, bool isStrictlyPlay = false)
    {
        AnimationHierarchyController controller = TryGetController(p_uid);

        if (controller == null)
        {
            Debug.LogWarning("Cannot play animation cause cannot find player, " + p_uid);
            return false;
        }

        if (isStrictlyPlay)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
            {
                Debug.LogWarning("Cancel check hierarchy in strictly playing mode.");
            }
        }
        else
        {
            if (m_PiorityDic[Animator.StringToHash(animationName)] < 0 || m_PiorityDic[AnimationHelper.GetAnimatorPlayingHash(controller)] < 0)
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
                {
                    Debug.LogWarning("Cannot play animation in: " + p_uid + " cause animation/ current playing animation not exist in hierarchy.");
                }
                return false;
            }
            if (m_PiorityDic[Animator.StringToHash(animationName)] > m_PiorityDic[AnimationHelper.GetAnimatorPlayingHash(controller)])
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
                {
                    Debug.LogWarning("Cannot play animation: " + animationName + " in: " + p_uid + " cause animation hierarchy block, " + m_PiorityDic[Animator.StringToHash(animationName)] + ">" + m_PiorityDic[AnimationHelper.GetAnimatorPlayingHash(controller)]);
                }
                return false;
            }

            if (controller.IsCloseAnimator)
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
                {
                    Debug.LogWarning("Cannot play animation in: " + p_uid + "cause close manually.");
                }
                return false;
            }
            //else
            //{
            //    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ANIMATION_HIERARCHY))
            //    {
            //        Debug.LogWarning("Can play animation: " + animationName + " in: " + p_uid + ", " + m_PiorityDic[Animator.StringToHash(animationName)] + "<=" + m_PiorityDic[AnimationHelper.GetAnimatorPlayingHash(controller)]);
            //    }
            //}
        }

        return true;
    }
}

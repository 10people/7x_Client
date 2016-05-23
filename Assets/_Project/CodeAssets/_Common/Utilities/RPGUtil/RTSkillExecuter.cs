using UnityEngine;
using System.Collections;
using System.Linq;

public class RTSkillExecuter
{
    public AnimationHierarchyPlayer m_AnimationHierarchyPlayer;
    public SinglePlayerController m_SelfPlayerController;
    public RPGBaseCultureController m_SelfPlayerCultureController;
    public PlayerManager m_PlayerManager;

    public HeadIconSetter m_SelfIconSetter;
    public HeadIconSetter m_TargetIconSetter;

    public GameObject m_LogicMain;

    public void ExecuteAttack(int p_attackUID, int p_beenAttackUID, int p_skillID)
    {
        if (p_attackUID == PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            //mine skill
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
            {
                //turn rotation
                if (m_PlayerManager.m_PlayerDic.ContainsKey(p_beenAttackUID))
                {
                    m_SelfPlayerController.transform.forward = m_PlayerManager.m_PlayerDic[p_beenAttackUID].transform.position - m_SelfPlayerController.transform.position;
                    m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_SelfPlayerController.transform.localEulerAngles.y, 0);
                }

                m_SelfPlayerController.DeactiveMove();

                m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

                if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                {
                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), m_SelfPlayerController.gameObject, null, Vector3.zero, m_SelfPlayerController.transform.forward);
                }
            }
        }
        else
        {
            var temp = m_PlayerManager.m_PlayerDic.Where(item => item.Key == p_attackUID).ToList();
            if (temp.Any())
            {
                //other player skill.
                if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
                {
                    //turn rotation.
                    if (m_PlayerManager.m_PlayerDic.ContainsKey(p_beenAttackUID))
                    {
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.forward = m_PlayerManager.m_PlayerDic[p_beenAttackUID].transform.position - m_PlayerManager.m_PlayerDic[p_attackUID].transform.position;
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles = new Vector3(0, m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles.y, 0);

                        if (EffectNumController.Instance.IsCanPlayEffect())
                        {
                            EffectNumController.Instance.NotifyPlayingEffect();

                            if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                            {
                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                            }
                        }

                    }
                    else if (p_beenAttackUID == PlayerSceneSyncManager.Instance.m_MyselfUid && m_SelfPlayerController != null)
                    {
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.forward = m_SelfPlayerController.transform.position - m_PlayerManager.m_PlayerDic[p_attackUID].transform.position;
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles = new Vector3(0, m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles.y, 0);

                        if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                        {
                            FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                        }
                    }

                    temp.First().Value.DeactiveMove();

                    m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);
                }
            }
        }
    }

    public void ExecuteBeenAttack(int p_attackUID, int p_beenAttackUID, long p_damage, float p_remaining, int p_targetId, int p_skillID)
    {
        if (p_beenAttackUID == PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            //mine been attack
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit))
            {
                m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit);

                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(p_skillID).CsOnHit), m_SelfPlayerController.gameObject, null, Vector3.zero, m_SelfPlayerController.transform.forward);

                EffectTool.Instance.SetHittedEffect(m_SelfPlayerController.gameObject);
            }

            m_SelfPlayerCultureController.OnDamage(p_damage, p_remaining, p_skillID == 111);
            m_SelfIconSetter.UpdateBar(p_remaining);
        }
        else
        {
            var temp = m_PlayerManager.m_PlayerDic.Where(item => item.Key == p_beenAttackUID).ToList();
            if (temp.Any())
            {
                //other player been attack.
                if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit))
                {
                    m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit);

                    if (EffectNumController.Instance.IsCanPlayEffect())
                    {
                        EffectNumController.Instance.NotifyPlayingEffect();

                        FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(p_skillID).CsOnHit), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                    }

                    EffectTool.Instance.SetHittedEffect(temp.First().Value.gameObject);
                }

                var temp2 = temp.First().Value.GetComponent<RPGBaseCultureController>();
                if (temp2 != null)
                {
                    temp2.OnDamage(p_damage, p_remaining, p_skillID == 111);
                    if (p_targetId == p_beenAttackUID)
                    {
                        m_TargetIconSetter.UpdateBar(p_remaining);
                    }
                }
            }
        }
    }

    public void ExecuteRecover(int p_uID, int p_skillID)
    {
        if (p_uID == PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            //mine skill
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
            {
                m_SelfPlayerController.DeactiveMove();

                m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

                if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                {
                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), m_SelfPlayerController.gameObject, null, Vector3.zero, m_SelfPlayerController.transform.forward);
                }
            }

            //Update blood num.
            m_LogicMain.SendMessage("UpdateWithUse1BloodNum");
        }
        else
        {
            var temp = m_PlayerManager.m_PlayerDic.Where(item => item.Key == p_uID).ToList();
            if (temp != null && temp.Count() > 0)
            {
                if (!temp.First().Value.IsCarriage)
                {
                    //other player skill, carriage not included.
                    if (m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
                    {
                        temp.First().Value.DeactiveMove();

                        m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

                        if (EffectNumController.Instance.IsCanPlayEffect())
                        {
                            EffectNumController.Instance.NotifyPlayingEffect();

                            if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                            {
                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                            }
                        }
                    }
                }
            }
        }
    }
}

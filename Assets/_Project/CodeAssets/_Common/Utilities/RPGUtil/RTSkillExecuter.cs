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
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
            {
                //turn rotation
                if (m_PlayerManager.m_PlayerDic.ContainsKey(p_beenAttackUID))
                {
                    m_SelfPlayerController.transform.forward = m_PlayerManager.m_PlayerDic[p_beenAttackUID].transform.position - m_SelfPlayerController.transform.position;
                    m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_SelfPlayerController.transform.localEulerAngles.y, 0);
                }

                m_SelfPlayerController.DeactiveMove();

                m_AnimationHierarchyPlayer.TryPlayAnimation(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

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
                if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
                {
                    //turn rotation.
                    if (m_PlayerManager.m_PlayerDic.ContainsKey(p_beenAttackUID))
                    {
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.forward = m_PlayerManager.m_PlayerDic[p_beenAttackUID].transform.position - m_PlayerManager.m_PlayerDic[p_attackUID].transform.position;
                        m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles = new Vector3(0, m_PlayerManager.m_PlayerDic[p_attackUID].transform.localEulerAngles.y, 0);

                        if (EffectNumController.Instance.IsCanPlayEffect(false))
                        {
                            EffectNumController.Instance.NotifyPlayingEffect(false);

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

                        if (EffectNumController.Instance.IsCanPlayEffect(true))
                        {
                            EffectNumController.Instance.NotifyPlayingEffect(true);

                            if (RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot > 0)
                            {
                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(p_skillID).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                            }
                        }
                    }

                    temp.First().Value.DeactiveMove();

                    m_AnimationHierarchyPlayer.TryPlayAnimation(p_attackUID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_attackUID"></param>
    /// <param name="p_beenAttackUID"></param>
    /// <param name="p_damage"></param>
    /// <param name="p_remaining"></param>
    /// <param name="p_selectTargetId">current select uid, use this to update head icon</param>
    /// <param name="p_skillID"></param>
    public void ExecuteBeenAttack(int p_attackUID, int p_beenAttackUID, long p_damage, float p_remaining, int p_selectTargetId, int p_skillID, bool isCriticalDamage = false)
    {
        if (p_beenAttackUID == PlayerSceneSyncManager.Instance.m_MyselfUid)
        {
            //mine been attack
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit))
            {
                m_AnimationHierarchyPlayer.TryPlayAnimation(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit);
            }

            //Been attack effect not controlled by animation.
            FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(p_skillID).CsOnHit), m_SelfPlayerController.gameObject, null, Vector3.zero, m_SelfPlayerController.transform.forward);

            EffectTool.Instance.SetHittedEffect(m_SelfPlayerController.gameObject);

            m_SelfPlayerCultureController.OnDamage(p_damage, p_remaining, isCriticalDamage);
            m_SelfIconSetter.UpdateBar(p_remaining);
        }
        else
        {
            var list = m_PlayerManager.m_PlayerDic.Where(item => item.Key == p_beenAttackUID).ToList();
            if (list.Any())
            {
                var cultureController = list.First().Value.GetComponent<RPGBaseCultureController>();

                //other player been attack.
                if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit))
                {
                    //Cancel play been attack animation on special target.
                    if (!cultureController.IsCarriage)
                    {
                        m_AnimationHierarchyPlayer.TryPlayAnimation(p_beenAttackUID, RTActionTemplate.GetTemplateByID(p_skillID).CeOnHit);
                    }
                }

                //Been attack effect not controlled by animation.
                if (EffectNumController.Instance.IsCanPlayEffect(false))
                {
                    EffectNumController.Instance.NotifyPlayingEffect(false);

                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(p_skillID).CsOnHit), list.First().Value.gameObject, null, Vector3.zero, list.First().Value.transform.forward);
                }

                EffectTool.Instance.SetHittedEffect(list.First().Value.gameObject);

                if (cultureController != null)
                {
                    cultureController.OnDamage(p_damage, p_remaining, isCriticalDamage);
                    if (p_selectTargetId == p_beenAttackUID)
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
            if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
            {
                m_SelfPlayerController.DeactiveMove();

                m_AnimationHierarchyPlayer.TryPlayAnimation(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

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
                    if (m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot))
                    {
                        temp.First().Value.DeactiveMove();

                        m_AnimationHierarchyPlayer.TryPlayAnimation(p_uID, RTSkillTemplate.GetTemplateByID(p_skillID).CsOnShot);

                        if (EffectNumController.Instance.IsCanPlayEffect(false))
                        {
                            EffectNumController.Instance.NotifyPlayingEffect(false);

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

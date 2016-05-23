using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace AllianceBattle
{
    public class MultiAttackSkillExecuter : MonoBehaviour
    {
        private GameObject m_targetObject;
        private GameObject m_attackObject;
        public int m_AttackUID;
        public int m_TargetUID;

        private Random m_random = new Random();
        private float m_slideDistance = 8f;
        private float m_slideDuration = 0.2f;
        private int m_slideTimes = 11;

        private List<string> m_animationList = new List<string>() { "ShuangdaoSkill1", "ShuangdaoSkill2", "ShuangdaoSkill3", "ShuangdaoSkill4" };

        private int m_remainingSlideTimes;
        private Vector3 m_storedInitialPosition = new Vector3();
        private bool isUpdateCameraManually = false;

        public static void Execute(int p_attackUID, int p_targetUID)
        {
            var s_attackObject = RootManager.GetPlayerObjectByUID(p_attackUID);
            var tempSkill = s_attackObject.GetComponent<MultiAttackSkillExecuter>() ?? s_attackObject.AddComponent<MultiAttackSkillExecuter>();

            tempSkill.ExecuteThis(p_attackUID, p_targetUID);
        }

        public void ExecuteThis(int p_attackUID, int p_targetUID)
        {
            m_AttackUID = p_attackUID;
            m_TargetUID = p_targetUID;

            m_attackObject = RootManager.GetPlayerObjectByUID(p_attackUID);
            m_targetObject = RootManager.GetPlayerObjectByUID(p_targetUID);

            m_remainingSlideTimes = m_slideTimes;
            m_storedInitialPosition = m_attackObject.transform.position;

            isAttackSelf = m_AttackUID == PlayerSceneSyncManager.Instance.m_MyselfUid;
            isTargetSelf = m_TargetUID == PlayerSceneSyncManager.Instance.m_MyselfUid;
            if (isAttackSelf)
            {
                m_singlePlayerController = m_attackObject.GetComponent<SinglePlayerController>();

                m_singlePlayerController.DeactiveMove();
                m_singlePlayerController.IsUploadPlayerPosition = false;
                m_singlePlayerController.TrackCamera = null;

                isUpdateCameraManually = true;
            }
            else
            {
                m_attackObject.GetComponent<OtherPlayerController>().DeactiveMove();
            }

            Execute1Attack();
        }

        private bool isAttackSelf;
        private bool isTargetSelf;
        private SinglePlayerController m_singlePlayerController;

        private void Execute1Attack()
        {
            m_remainingSlideTimes--;

            var degree = m_random.Next(0, 359) * Math.PI / 180;
            var animationIndex = m_random.Next(0, 3);

            var startPosition = new Vector3(m_targetObject.transform.position.x + (float)Math.Cos(degree) * m_slideDistance / 2, m_targetObject.transform.position.y, m_targetObject.transform.position.z + (float)Math.Sin(degree) * m_slideDistance / 2);
            var endPosition = 2 * m_targetObject.transform.position - startPosition;
            var angleTemp = (float)(Math.Atan2(endPosition.x - startPosition.x, endPosition.z - startPosition.z) / Math.PI * 180);

            m_attackObject.transform.position = startPosition;
            m_attackObject.transform.eulerAngles = new Vector3(0, angleTemp, 0);
            iTween.MoveTo(m_attackObject, iTween.Hash("position", endPosition, "time", m_slideDuration, "easeType", "linear", "islocal", false, "oncomplete", "OnExecute1AttackFinish", "oncompletetarget", gameObject));

            if (isAttackSelf)
            {
                //mine skill
                if (RootManager.Instance.m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(m_AttackUID, m_animationList[animationIndex]))
                {
                    RootManager.Instance.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(m_AttackUID, m_animationList[animationIndex]);

                    if (RTSkillTemplate.GetTemplateByID(181).EsOnShot > 0)
                    {
                        if (EffectNumController.Instance.IsCanPlayEffect())
                        {
                            EffectNumController.Instance.NotifyPlayingEffect();

                            FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(181).EsOnShot), m_attackObject, null, Vector3.zero, m_attackObject.transform.forward);
                        }
                    }
                }
            }
            else
            {
                //other player skill.
                if (RootManager.Instance.m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(m_AttackUID, m_animationList[animationIndex]))
                {
                    RootManager.Instance.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(m_AttackUID, m_animationList[animationIndex]);

                    if (isTargetSelf)
                    {
                        if (RTSkillTemplate.GetTemplateByID(181).EsOnShot > 0)
                        {
                            if (EffectNumController.Instance.IsCanPlayEffect())
                            {
                                EffectNumController.Instance.NotifyPlayingEffect();

                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(181).EsOnShot), m_attackObject, null, Vector3.zero, m_attackObject.transform.forward);
                            }
                        }
                    }
                    else
                    {
                        if (EffectNumController.Instance.IsCanPlayEffect())
                        {
                            EffectNumController.Instance.NotifyPlayingEffect();

                            if (RTSkillTemplate.GetTemplateByID(181).EsOnShot > 0)
                            {
                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(181).EsOnShot), m_attackObject, null, Vector3.zero, m_attackObject.transform.forward);
                            }
                        }
                    }
                }
            }
        }

        public void OnExecute1AttackFinish()
        {
            if (m_remainingSlideTimes > 0)
            {
                Execute1Attack();
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            //Active move.
            if (isAttackSelf)
            {
                isUpdateCameraManually = false;

                //m_attackObject.transform.position = m_storedInitialPosition;
                m_singlePlayerController.ActiveMove();
                m_singlePlayerController.IsUploadPlayerPosition = true;
                m_singlePlayerController.TrackCamera = RootManager.Instance.TrackCamera;

                RootManager.Instance.m_AllianceBattleMain.RestoreSkill();
            }
            else
            {
                m_attackObject.GetComponent<OtherPlayerController>().ActiveMove();
            }

            RootManager.Instance.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(m_AttackUID, "Stand", true);
        }

        void LateUpdate()
        {
            if (isUpdateCameraManually && isAttackSelf && m_targetObject != null)
            {
                RootManager.Instance.TrackCamera.transform.localPosition = m_singlePlayerController.TrackCameraPosition + new Vector3(m_targetObject.transform.localPosition.x, 0, m_targetObject.transform.localPosition.z);
                RootManager.Instance.TrackCamera.transform.localEulerAngles = m_singlePlayerController.TrackCameraRotation;
            }
        }
    }
}
//#define UNIT_TEST

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllianceBattle;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace AllianceBattle
{
    public class AllianceBattleMain : MonoBehaviour
    {
        public Joystick m_Joystick;

        public RootManager m_RootManager;

        public HeadIconSetter SelfIconSetter;
        public HeadIconSetter TargetIconSetter;

        public List<RTSkillController> m_SkillControllers = new List<RTSkillController>();

        public List<RTSkillController> m_CanActivedSkillControllers
        {
            get
            {
                return m_SkillControllers.Where(item => item.m_Template.SkillTarget != 0).ToList();
            }
        }

        public UIBackgroundEffect m_MainUIVagueEffect;
        public UIBackgroundEffect m_Top1UIVagueEffect;
        public UIBackgroundEffect m_Top2UIVagueEffect;

        #region Navigate with tracking

        [HideInInspector]
        public Transform m_TargetItemTransform;

        private const float m_TrackingNavigateRange = 1.5f;

        public void NavigateToItem()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_TargetItemTransform == null)
            {
                return;
            }

            //Make navigate move.
            if (Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_TargetItemTransform.position) > m_TrackingNavigateRange)
            {
                m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = NavigateToItem;
                m_RootManager.m_SelfPlayerController.StartNavigation(m_TargetItemTransform.position);
            }
        }

        #endregion

        #region Map Controller

        public MapController m_MapController;

        public void ExecuteAfterOpenMap()
        {
            m_MainUIVagueEffect.enabled = true;
            m_Joystick.m_Box.enabled = false;
        }

        public void ExecuteAfterCloseMap()
        {
            m_MainUIVagueEffect.enabled = false;
            m_Joystick.m_Box.enabled = true;
        }

        #endregion

        #region UI Click Event

        public void OnReturnClick()
        {
            CityGlobalData.m_isJieBiaoScene = false;
            PlayerSceneSyncManager.Instance.ExitAB();
        }

        public void OnSwitchClick()
        {
            var allItemList = GetPossibleActiveTargetList();

            if (allItemList != null && allItemList.Any())
            {
                //Get only one target
                m_TargetIndex++;
                if (m_TargetIndex >= allItemList.Count)
                {
                    m_TargetIndex = 0;
                }
                ActiveTarget(allItemList[m_TargetIndex]);
            }
        }

        private int SkillIndexAfterNavi;

        private void OnSkillAfterNavi()
        {
            OnSkillClick(SkillIndexAfterNavi);
        }

        private bool isCloseSkillManually = false;

        public void OnSkillClick(int index)
        {
            if (isCloseSkillManually)
            {
                return;
            }

            if (index >= 0)
            {
                var tempController = m_SkillControllers.Where(item => item.m_Index == index).First();
                var template = tempController.m_Template;

                //Check CD.
                if (tempController.IsInCD)
                {
                    ClientMain.m_UITextManager.createText("技能正在冷却中");
                    return;
                }

                //Update target ,skill target check, return when no target.
                if (template.SkillTarget == 1 && (m_TargetId < 0 || !m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId)))
                {
                    ClientMain.m_UITextManager.createText("需要选定一个目标");
                    DeactiveTarget();
                    return;
                }

                //skill target check, return when 1. cannot operated to player/other player, 2. cannot operated to AllianceBattle.
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 1 && ((m_TargetId < 0) || (m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId))))
                {
                    ClientMain.m_UITextManager.createText("不能对玩家使用");
                    return;
                }

                //skill target relationship check, return when 1. cannot operated to friend, 2. cannot operated to enemy.
                if (template.SkillTarget == 1 && template.CRRejectU == 1 && m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<ABPlayerCultureController>().IsEnemy)
                {
                    ClientMain.m_UITextManager.createText("不能对敌方使用");
                    return;
                }
                if (template.SkillTarget == 1 && template.CRRejectU == 2 && (m_TargetId < 0 || (m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<ABPlayerCultureController>().IsEnemy)))
                {
                    ClientMain.m_UITextManager.createText("不能对友方使用");
                    return;
                }

                //Make navigate move.
                if (template.SkillTarget == 1 && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position) > template.Range_Max)
                {
                    SkillIndexAfterNavi = index;
                    m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = OnSkillAfterNavi;
                    m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position, template.Range_Max);
                    return;
                }

                //Rotate to target.
                if (template.SkillTarget == 1)
                {
                    m_RootManager.m_SelfPlayerController.transform.forward = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position - m_RootManager.m_SelfPlayerController.transform.position;
                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y, 0);
                }

                //skill distance check, return when skill operated to others beyond distance.
                if (template.SkillTarget == 1)
                {
                    var distance = Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position);

                    if (distance > template.Range_Max || distance < template.Range_Min)
                    {
                        ClientMain.m_UITextManager.createText("目标不在技能范围内");
                        return;
                    }
                }

                //Special check for blood recover.
                if (template.SkillId == 121 && m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null && m_RootManager.m_SelfPlayerCultureController.RemainingBlood >= m_RootManager.m_SelfPlayerCultureController.TotalBlood)
                {
                    ClientMain.m_UITextManager.createText("您为满血状态");
                    return;
                }

                //Special skill for summon.
                if (template.SkillId == 200)
                {
                    OnSummonClick();
                }
                else
                {
                    FightAttackReq tempInfo = new FightAttackReq()
                    {
                        targetUid = template.SkillTarget == 0 ? PlayerSceneSyncManager.Instance.m_MyselfUid : m_TargetId,
                        skillId = index
                    };
                    MemoryStream tempStream = new MemoryStream();
                    QiXiongSerializer tempSer = new QiXiongSerializer();
                    tempSer.Serialize(tempStream, tempInfo);
                    byte[] t_protof;
                    t_protof = tempStream.ToArray();
                    SocketTool.Instance().SendSocketMessage(template.SkillId == 151 ? ProtoIndexes.SKILL_PREPARE : ProtoIndexes.C_FIGHT_ATTACK_REQ, ref t_protof);
                }
            }
        }

        #endregion

        #region Target Controller

        public int m_TargetIndex = -1;
        public int m_TargetId = -1;

        public float SelectDistance
        {
            get
            {
                return selectDistance > 0 ? selectDistance : (selectDistance = (float)CanshuTemplate.GetValueByKey("ATTACK_LOCKENEMY_RANGE"));
            }
        }

        private float selectDistance;

        private List<KeyValuePair<int, OtherPlayerController>> GetAllItemsWithinDistance()
        {
            if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic == null || m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Count == 0)
            {
                return null;
            }

            //In distance
            var allItemList = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Where(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position) < SelectDistance).ToList();
            if (!allItemList.Any())
            {
                return null;
            }

            return allItemList;
        }

        /// <summary>
        /// Get active target list, auto deactive target if list is null.
        /// </summary>
        /// <returns></returns>
        private List<int> GetPossibleActiveTargetList()
        {
            //In distance
            var allItemList = GetAllItemsWithinDistance();
            if (allItemList == null)
            {
                DeactiveTarget();
                return null;
            }

            return allItemList.OrderBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).Select(item => item.Key).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if active successfully.</returns>
        public bool AutoActiveTarget()
        {
            if (m_TargetId < 0)
            {
                var allItemList = GetPossibleActiveTargetList();

                if (allItemList != null && allItemList.Any())
                {
                    //Get only one target
                    ActiveTarget(allItemList.First());

                    return true;
                }
            }

            return false;
        }

        public void AutoDeactiveTarget()
        {
            if (m_TargetId > 0 && (!m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) || Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position) > SelectDistance))
            {
                DeactiveTarget();
            }
        }

        public void ActiveTarget(int p_targetID)
        {
            m_TargetId = p_targetID;

            //active attack and skills operated to others.
            if (m_CanActivedSkillControllers.Any())
            {
                m_CanActivedSkillControllers.ForEach(item => item.m_SkillButton.isEnabled = true);
                m_CanActivedSkillControllers.ForEach(item => item.m_SkillSprite.color = Color.white);
            }

            //clear selected.
            m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ToList().ForEach(item => item.Value.GetComponent<ABPlayerCultureController>().OnDeSelected());

            var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<ABPlayerCultureController>();
            if (temp != null)
            {
                //active selected.
                temp.OnSelected();

                //set target ui info.
                TargetIconSetter.SetPlayer(temp.RoleID, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue, 0);
                TargetIconSetter.gameObject.SetActive(true);

                //Try show clamped buttons.
            }
        }

        public void DeactiveTarget()
        {
            if (m_CanActivedSkillControllers.Any())
            {
                //deactive attack and skills operated to others.
                m_CanActivedSkillControllers.ForEach(item => item.m_SkillButton.isEnabled = false);
                m_CanActivedSkillControllers.ForEach(item => item.m_SkillSprite.color = Color.grey);
            }

            //deactive target prefab.
            TargetIconSetter.gameObject.SetActive(false);

            //deactive selected.
            if (m_TargetId > 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Keys.Contains(m_TargetId))
            {
                var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<ABPlayerCultureController>();
                if (temp != null)
                {
                    temp.OnDeSelected();
                }
            }

            m_TargetId = -1;
        }

        #endregion

        #region Skill/Buff Executer

        public RTSkillExecuter m_RTSkillExecuter = new RTSkillExecuter();

        [HideInInspector]
        public GameObject m_LongSkillEffectPrefab;

        public void ExecuteSkill(FightAttackResp tempInfo)
        {
            switch (tempInfo.skillId)
            {
                //normal attack
                case 101:
                    {
                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);
                        m_RTSkillExecuter.ExecuteBeenAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.damage, tempInfo.remainLife, m_TargetId, tempInfo.skillId);

                        break;
                    }
                //Recover skill
                case 121:
                    {
                        m_RTSkillExecuter.ExecuteRecover(tempInfo.attackUid, tempInfo.skillId);

                        break;
                    }
                //Aoe skill
                case 151:
                    {
                        var temp = RootManager.GetPlayerObjectByUID(tempInfo.attackUid);
                        if (temp != null)
                        {
                            temp.GetComponent<WeaponSwitcher>().SwitchWeapon(WeaponSwitcher.WeaponType.Heavy);
                        }
                        else
                        {
                            Debug.LogWarning("Switch weapon fail cause cannot find player, " + tempInfo.attackUid);
                        }

                        if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            isCloseSkillManually = true;
                        }

                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);

                        break;
                    }
                //Long skill
                case 171:
                    {
                        var playerObject = RootManager.GetPlayerObjectByUID(tempInfo.attackUid);
                        var targetObject = RootManager.GetPlayerObjectByUID(tempInfo.targetUid);
                        if (playerObject != null)
                        {
                            playerObject.GetComponent<WeaponSwitcher>().SwitchWeapon(WeaponSwitcher.WeaponType.Range);

                            //Use track skill executer to simulate effect.
                            var trackSkill = playerObject.GetComponent<TrackSkillExecuter>() ?? playerObject.AddComponent<TrackSkillExecuter>();
                            trackSkill.SkillPrefab = m_LongSkillEffectPrefab;
                            trackSkill.SourceObject = playerObject;
                            trackSkill.TargetObject = targetObject;

                            //playerObject.GetComponent<RPGBaseCultureController>().m_ExecuteAfterSkillFinish = trackSkill.Execute;
                            trackSkill.Execute();
                        }
                        else
                        {
                            Debug.LogWarning("Switch weapon fail cause cannot find player, " + tempInfo.attackUid);
                        }

                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);
                        m_RTSkillExecuter.ExecuteBeenAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.damage, tempInfo.remainLife, m_TargetId, tempInfo.skillId);

                        break;
                    }
                //Multi attack skill
                case 181:
                    {
                        if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            isCloseSkillManually = true;
                        }

                        MultiAttackSkillExecuter.Execute(tempInfo.attackUid, tempInfo.targetUid);

                        break;
                    }
            }

            //Set cds.
            if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                m_SkillControllers.ForEach(item => item.TryStartSharedCD());
                var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == tempInfo.skillId).ToList();
                if (triggeredSkill.Any())
                {
                    triggeredSkill.First().TryStartSelfCD();
                }
            }
        }

        public void RestoreSkill()
        {
            isCloseSkillManually = false;
        }

        public void StopAOESkill(ErrorMessage p_msg)
        {
            var temp = AnimationHelper.GetAnimatorPlayingHash(m_RootManager.m_AnimationHierarchyPlayer.TryGetAnimator(p_msg.errorCode));
            if (temp == Animator.StringToHash("DadaoSkill"))
            {
                if (m_RootManager.m_AnimationHierarchyPlayer.IsCanPlayAnimationInAnimator(p_msg.errorCode, "EndDadaoSkill"))
                {
                    if (p_msg.errorCode == PlayerSceneSyncManager.Instance.m_MyselfUid)
                    {
                        if (m_RootManager.m_SelfPlayerController != null)
                        {
                            m_RootManager.m_SelfPlayerCultureController.m_ExecuteAfterSkillFinish = RestoreSkill;
                            m_RootManager.m_SelfPlayerController.DeactiveMove();
                        }
                        else
                        {
                            Debug.LogWarning("play EndDadaoSkill in uid: " + p_msg.errorCode + "fail cause self not exist.");
                        }
                    }
                    else
                    {
                        var otherPlayer = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Where(item => item.Key == p_msg.errorCode).ToList();
                        if (otherPlayer.Any())
                        {
                            otherPlayer.First().Value.DeactiveMove();
                        }
                        else
                        {
                            Debug.LogWarning("play EndDadaoSkill in uid: " + p_msg.errorCode + "fail cause target not exist.");
                        }
                    }

                    m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimationInAnimator(p_msg.errorCode, "EndDadaoSkill");
                }
                else
                {
                    Debug.LogWarning("play EndDadaoSkill in uid: " + p_msg.errorCode + "fail in hierarchy");
                }
            }
            else
            {
                Debug.LogWarning("Cancel play stop aoe skill in uid: " + p_msg.errorCode + " cause current playing animation is not aoe.");
            }
        }

        [Obsolete]
        public void ExecuteSkill(AOESkill tempInfo)
        {
            m_RTSkillExecuter.ExecuteAttack(tempInfo.srcUid, -1, 151);

            for (int i = 0; i <= tempInfo.affectedUids.Count; i++)
            {
                m_RTSkillExecuter.ExecuteBeenAttack(tempInfo.srcUid, tempInfo.affectedUids[i], tempInfo.damages[i], 0, m_TargetId, 151);
            }
        }

        public void ExecuteBuff(BufferInfo tempInfo)
        {
            switch (tempInfo.bufferId)
            {
                //recover buff
                case 121:
                    {
                        //mine buff
                        if (tempInfo.targetId == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null)
                            {
                                return;
                            }

                            //Change blood bar.
                            m_RootManager.m_SelfPlayerCultureController.OnRecover(tempInfo.value, tempInfo.remainLife);
                            SelfIconSetter.UpdateBar(tempInfo.remainLife);
                        }
                        //other player buff
                        else
                        {
                            if (!m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(tempInfo.targetId))
                            {
                                return;
                            }

                            var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[tempInfo.targetId].GetComponent<ABPlayerCultureController>();
                            if (temp != null)
                            {
                                //Change blood bar.
                                temp.OnRecover(tempInfo.value, tempInfo.remainLife);
                                if (m_TargetId == tempInfo.targetId)
                                {
                                    TargetIconSetter.UpdateBar(tempInfo.remainLife);
                                }
                            }
                        }

                        break;
                    }
                //damage buff
                case 151:
                    {
                        m_RTSkillExecuter.ExecuteBeenAttack(-1, tempInfo.targetId, tempInfo.value, tempInfo.remainLife, m_TargetId, 161);

                        break;
                    }
                case 161:
                    {
                        m_RTSkillExecuter.ExecuteBeenAttack(-1, tempInfo.targetId, tempInfo.value, tempInfo.remainLife, m_TargetId, 191);

                        break;
                    }
                case 171:
                    {
                        m_RTSkillExecuter.ExecuteBeenAttack(-1, tempInfo.targetId, tempInfo.value, tempInfo.remainLife, m_TargetId, 201);

                        break;
                    }
            }
        }

        public void ExecuteBloodChange(FightAttackResp info)
        {
            if (info.targetUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                if (m_RootManager.m_SelfPlayerCultureController != null)
                {
                    m_RootManager.m_SelfPlayerCultureController.TotalBlood = info.damage;
                    m_RootManager.m_SelfPlayerCultureController.UpdateBloodBar(info.remainLife);
                }
                else
                {
                    Debug.LogError("Cannot update total blood cause self not exist.");
                    return;
                }
            }
            else
            {
                if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(info.targetUid))
                {
                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().TotalBlood = info.damage;
                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().UpdateBloodBar(info.remainLife);
                }
                else
                {
                    Debug.LogError("Cannot update total blood cause target: " + info.targetUid + " not exist.");
                    return;
                }
            }
        }

        #endregion

        #region Dead and Rebrith[FIX]

        public void ExecuteDead(int p_uID)
        {
            if (p_uID == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                Destroy(m_RootManager.m_SelfPlayerController.gameObject);

                m_RootManager.m_SelfPlayerCultureController = null;
                m_RootManager.m_SelfPlayerController = null;
                DeactiveTarget();

                //Show dead dimmer.
                ShowDeadWindow(m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.autoReviveRemainTime, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.remainAllLifeTimes / 1000, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.remainAllLifeTimes % 1000, 0, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.onSiteReviveCost);
            }
            else
            {
                //Remove from mesh controller.
                if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
                {
                    ModelAutoActivator.UnregisterAutoActivator(m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].gameObject);
                }

                m_RootManager.m_AbPlayerSyncManager.DestroyPlayer(p_uID);
            }
        }

        public GameObject DeadWindow;

        public UILabel ShowRebirthInfoLabel;

        public GameObject FreeQuickRebirthObject;
        public GameObject StoneQuickRebirthObject;
        public GameObject IngotQuickRebirthObject;

        public UILabel FreeQuickRebirthLabel;
        public UILabel StoneQuickRebirthTimesLabel;
        public UILabel StoneQuickRebirthTotalLabel;
        public UILabel IngotQuickRebirthTimesLabel;
        public UILabel IngotQuickRebirthTotalLabel;

        private int m_storedSlowRebirthTimes;
        private int m_quickRebirthType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slowRebirthTime"></param>
        /// <param name="remainQuickRebirthTimes"></param>
        /// <param name="totalQuickRebirthTimes"></param>
        /// <param name="quickRebirthStone"></param>
        /// <param name="quickRebirthCost"></param>
        public void ShowDeadWindow(int slowRebirthTime, int remainQuickRebirthTimes, int totalQuickRebirthTimes, int quickRebirthStone, int quickRebirthCost)
        {
            DeadWindow.SetActive(true);

            //Set rebirth time calc.
            m_storedSlowRebirthTimes = slowRebirthTime;
            if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
            }
            TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleRebirth", slowRebirthTime, SetRebirthTime);

            FreeQuickRebirthObject.SetActive(false);
            StoneQuickRebirthObject.SetActive(false);
            IngotQuickRebirthObject.SetActive(false);
            //Which to show.
            if (remainQuickRebirthTimes > 0)
            {
                FreeQuickRebirthLabel.text = "今日剩余免费快速复活次数" + remainQuickRebirthTimes + "/" + totalQuickRebirthTimes;
                FreeQuickRebirthObject.SetActive(true);
                m_quickRebirthType = 20;
            }
            else if (quickRebirthStone > 0)
            {
                StoneQuickRebirthTimesLabel.text = "x1";
                var bagList = BagData.Instance().m_bagItemList;
                var temp = bagList.Where(item => item.itemId == 910010).ToList();
                StoneQuickRebirthTotalLabel.text = "拥有：" + (temp.Any() ? temp.First().cnt : 0) + "个";
                StoneQuickRebirthObject.SetActive(true);
                m_quickRebirthType = 30;
            }
            else
            {
                IngotQuickRebirthTimesLabel.text = "x" + quickRebirthCost;
                IngotQuickRebirthTotalLabel.text = "拥有：" + JunZhuData.Instance().m_junzhuInfo.yuanBao;
                IngotQuickRebirthObject.SetActive(true);
                m_quickRebirthType = 40;
            }

            m_Joystick.m_Box.enabled = false;
            m_MainUIVagueEffect.enabled = true;
            m_Top2UIVagueEffect.enabled = true;
        }

        public void HideDeadWindows()
        {
            DeadWindow.SetActive(false);

            if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
            }

            if (!m_RootManager.m_Top1Hierarchy.IsNGUIVisible())
            {
                m_Joystick.m_Box.enabled = true;

                m_MainUIVagueEffect.enabled = false;
                m_Top2UIVagueEffect.enabled = false;
            }
        }

        private void SetRebirthTime(int time)
        {
            if (ShowRebirthInfoLabel.gameObject.activeInHierarchy)
            {
                ShowRebirthInfoLabel.text = "距离复活时间还有" + ColorTool.Color_Red_c40000 + (m_storedSlowRebirthTimes - time) + "[-]" + "秒";
            }

            if (m_storedSlowRebirthTimes - time <= 0)
            {
                OnSlowRebirthClick();

                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
                }
            }
        }

        public void OnSlowRebirthClick()
        {
            PlayerReviveRequest tempInfo = new PlayerReviveRequest()
            {
                type = 10
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, tempInfo);
            byte[] t_protof;
            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_FuHuo, ref t_protof);
        }

        public void OnQuickRebirthClick()
        {
            PlayerReviveRequest tempInfo = new PlayerReviveRequest()
            {
                type = m_quickRebirthType
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, tempInfo);
            byte[] t_protof;
            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_FuHuo, ref t_protof);
        }

        #endregion

        #region Remaining Blood

        public UILabel m_RemainingBloodNumLabel;
        public int m_RemainBloodNum;

        public GameObject m_AddBloodTimesBTN;
        public GameObject m_RecoverBTN;

        public void SetRemainingBloodNum(int remainingNum)
        {
            m_RemainBloodNum = remainingNum;

            m_RemainingBloodNumLabel.text = "x" + remainingNum;

            var temp = m_SkillControllers.Where(item => item.m_Index == 121);
            if (temp != null && temp.Any())
            {
                if (remainingNum <= 0)
                {
                    temp.First().m_SkillButton.isEnabled = false;
                    temp.First().m_SkillSprite.color = Color.grey;

                    //Show add button
                    m_AddBloodTimesBTN.SetActive(true);
                    m_RecoverBTN.SetActive(false);
                }
                else
                {
                    temp.First().m_SkillButton.isEnabled = true;
                    temp.First().m_SkillSprite.color = Color.white;

                    //Show recover button
                    m_AddBloodTimesBTN.SetActive(false);
                    m_RecoverBTN.SetActive(true);
                }
            }
        }

        public void UpdateWithUse1BloodNum()
        {

        }

        #endregion

        #region TP

        public TPController m_TpController;

        private float m_tpDuration;

        public void TpToPosition(Vector2 p_position)
        {
            if (m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null)
            {
                m_TpController.m_ExecuteAfterTP = ExecuteAfterTp;
                m_TpController.TPToPosition(p_position, m_tpDuration);
            }
        }

        private void ExecuteAfterTp(Vector2 p_position)
        {
            m_RootManager.m_SelfPlayerController.transform.localPosition = new Vector3(p_position.x, RootManager.BasicYPosition, p_position.y);
        }

        #endregion

        #region Navigation Animation

        public void ShowNavigationAnim()
        {
            AnimationController.ShowAnimation(Res2DTemplate.GetResPath(Res2DTemplate.Res.AUTO_NAV));
        }

        public void StopNavigationAnim()
        {
            AnimationController.StopAnimation();
        }

        public CharacterAnimationController AnimationController;

        #endregion

        #region Alert Info Effect

        public BannerEffectController m_BannerEffectController;

        public void ExecuteAfterEffectClick()
        {
            if (m_CurrenTongzhiData != null)
            {
                Global.m_listAllTheData.Remove(m_CurrenTongzhiData);
                Global.upDataTongzhiData(null);

                if (m_CurrenTongzhiData.m_ButtonIndexList != null && m_CurrenTongzhiData.m_ButtonIndexList.Any())
                {
                    PromptActionReq req = new PromptActionReq();
                    req.reqType = m_CurrenTongzhiData.m_ButtonIndexList.First();
                    req.suBaoId = m_CurrenTongzhiData.m_SuBaoMSG.subaoId;
                    MemoryStream tempStream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    t_qx.Serialize(tempStream, req);
                    byte[] t_protof;
                    t_protof = tempStream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Prompt_Action_Req, ref t_protof);
                }
            }

            m_MainCityUiTongzhi.upDataShow();
        }

        public void ExecuteAfterEffectEnd()
        {
            if (m_CurrenTongzhiData != null)
            {
                ExecuteReportData();
            }
        }

        #endregion

        #region Report Info

        public GameObject m_ReportObject;
        public MainCityUITongzhi m_MainCityUiTongzhi;

        public TongzhiData m_CurrenTongzhiData;

        private void DoOpenReportWindow(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>is executed</returns>
        public bool ExecuteReportData()
        {
            //Update report icon.
            if (Global.m_listJiebiaoData != null && Global.m_listJiebiaoData.Any())
            {
                m_ReportObject.SetActive(true);
            }
            else
            {
                m_ReportObject.SetActive(false);
            }

            if (m_BannerEffectController.isAlertInfoEffectShowing)
            {
                Debug.LogWarning("Cancel execute report data cause alert effect showing");
                return false;
            }

            foreach (TongzhiData data in Global.m_listJiebiaoData)
            {
                if (data.IsEffectShowType())
                {
                    m_CurrenTongzhiData = data;

                    List<int> awardIdList = new List<int>();
                    List<int> awardNumList = new List<int>();

                    var subLabelText = data.m_SuBaoMSG.subao;
                    var labelText = data.m_ReportTemplate.m_sReportTitle;
                    var awards = data.m_SuBaoMSG.award.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (awards.Any())
                    {
                        awards.ForEach(item =>
                        {
                            var splited = item.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            awardIdList.Add(int.Parse(splited[1]));
                            awardNumList.Add(int.Parse(splited[2]));
                        });
                    }

                    m_BannerEffectController.ShowAlertInfo(labelText, subLabelText, "", awardIdList, awardNumList);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Score Window

        public void OnScoreClick()
        {
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_SCORE_LIST);
        }

        public void OnCloseScoreClick()
        {
            HideScoreWindow();
        }

        public GameObject ScoreWindowObject;
        public UIScrollView ScoreScrollView;
        public UIGrid ScoreScrollGrid;
        public GameObject ScoreDetailPrefab;

        public UILabel MyRankLabel;

        public List<ABScoreItemController.ScoreData> m_ScoreDataList = new List<ABScoreItemController.ScoreData>();

        public void ShowScoreWindow()
        {
            ScoreWindowObject.SetActive(true);

            m_Joystick.m_Box.enabled = false;
            m_MainUIVagueEffect.enabled = true;
            m_Top2UIVagueEffect.enabled = true;

            RefreshScoreWindow();
        }

        public void HideScoreWindow()
        {
            ScoreWindowObject.SetActive(false);

            if (!m_RootManager.m_Top1Hierarchy.IsNGUIVisible())
            {
                m_Joystick.m_Box.enabled = true;
                m_MainUIVagueEffect.enabled = false;
                m_Top2UIVagueEffect.enabled = false;
            }
        }

        public void RefreshScoreWindow()
        {
            while (ScoreScrollGrid.transform.childCount > 0)
            {
                var child = ScoreScrollGrid.transform.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }

            m_ScoreDataList = m_ScoreDataList.OrderBy(item => item.Rank).ToList();
            m_ScoreDataList.ForEach(item =>
            {
                var controller = NGUITools.AddChild(ScoreScrollGrid.gameObject, ScoreDetailPrefab).GetComponent<ABScoreItemController>();
                controller.m_ScoreData = item;
                controller.SetThis();

                controller.gameObject.name = item.Rank + "_" + controller.gameObject.name;
                controller.gameObject.SetActive(true);
            });

            ScoreScrollGrid.Reposition();

            var temp = m_ScoreDataList.Where(item => item.Name == JunZhuData.Instance().m_junzhuInfo.name).ToList();
            if (temp.Any())
            {
                MyRankLabel.text = temp.First().Rank.ToString();
            }
        }

        #endregion

        #region Command

        public GameObject CommandBTN;

        public GameObject CommandWindow;
        public UIGrid CommandGrid;
        public GameObject CommandItemPrefab;
        public UISprite CommandWindowBG;

        public List<ABCmdItemController.CmdItemData> m_CmdItemDataList = new List<ABCmdItemController.CmdItemData>();

        public void OnCommandClick()
        {
            CommandWindow.SetActive(!CommandWindow.activeInHierarchy);

            if (CommandWindow.activeInHierarchy && CommandGrid.transform.childCount == 0)
            {
                //while (CommandGrid.transform.childCount > 0)
                //{
                //    var child = CommandGrid.transform.GetChild(0);
                //    Destroy(child.gameObject);
                //    child.parent = null;
                //}

                for (int i = 0; i < m_CmdItemDataList.Count; i++)
                {
                    var controller = NGUITools.AddChild(CommandGrid.gameObject, CommandItemPrefab).GetComponent<ABCmdItemController>();
                    controller.m_CmdItemData = m_CmdItemDataList[i];
                    controller.SetThis();

                    controller.gameObject.name = i + "_" + controller.gameObject.name;
                    controller.gameObject.SetActive(true);
                }

                ScoreScrollGrid.Reposition();
            }
        }

        #endregion

        #region Summon

        private int m_summonUID;

        public UILabel m_RemainingSummonNumLabel;
        public int m_RemainSummonNum;

        public GameObject m_AddSummonTimesBTN;
        public GameObject m_SummonBTN;

        public void OnSummonClick()
        {
            SocketHelper.SendQXMessage(ProtoIndexes.LMZ_ZhaoHuan);
        }

        public void ExecuteSummon(int p_uID)
        {
            if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
            {
                m_summonUID = p_uID;

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ShowSummonWindow);
            }

            if (PlayerSceneSyncManager.Instance.m_MyselfUid == p_uID)
            {
                //Set cds.
                m_SkillControllers.ForEach(item => item.TryStartSharedCD());
                var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == 200).ToList();
                if (triggeredSkill.Any())
                {
                    triggeredSkill.First().TryStartSelfCD();
                }
            }
        }

        private void ShowSummonWindow(ref WWW p_www, string p_path, Object p_object)
        {
            if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_summonUID))
            {
                UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
                uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                     null, "是否传送至" + m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_summonUID].GetComponent<ABPlayerCultureController>().KingName + "处?",
                     null,
                     LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                     ClickSummonWindow);
            }
        }

        private void ClickSummonWindow(int i)
        {
            switch (i)
            {
                case 1:
                    break;
                case 2:
                    if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_summonUID))
                    {
                        var pos = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_summonUID].transform.localPosition;
                        TpToPosition(new Vector2(pos.x, pos.z));
                    }
                    break;
            }
        }

        public void SetRemainingSummonNum(int remainingNum)
        {
            m_RemainSummonNum = remainingNum;

            m_RemainingSummonNumLabel.text = "x" + remainingNum;

            var temp = m_SkillControllers.Where(item => item.m_Index == 200);
            if (temp != null && temp.Any())
            {
                if (remainingNum <= 0)
                {
                    temp.First().m_SkillButton.isEnabled = false;
                    temp.First().m_SkillSprite.color = Color.grey;

                    //Show add button
                    m_AddSummonTimesBTN.SetActive(true);
                    m_SummonBTN.SetActive(false);
                }
                else
                {
                    temp.First().m_SkillButton.isEnabled = true;
                    temp.First().m_SkillSprite.color = Color.white;

                    //Show recover button
                    m_AddSummonTimesBTN.SetActive(false);
                    m_SummonBTN.SetActive(true);
                }
            }
        }

        #endregion

        #region Battle Stat

        public UILabel BattleTimeLabel;
        public UILabel BattleScoreLabel;
        public UILabel ComboKillLabel;

        public void UpdateBattleTime(int remainingTime, int state)
        {
            switch (state)
            {
                case -1:
                    {
                        BattleTimeLabel.text = "比赛将于" + ColorTool.Color_Red_c40000 + TimeHelper.SecondToClockTime(remainingTime) + "[-]后开始";
                        break;
                    }
                case 0:
                    {
                        BattleTimeLabel.text = "比赛将于" + ColorTool.Color_Red_c40000 + TimeHelper.SecondToClockTime(remainingTime) + "[-]后结束";
                        break;
                    }
                case 1:
                    {
                        BattleTimeLabel.text = "比赛结束, 守方获胜！";
                        break;
                    }
                case 2:
                    {
                        BattleTimeLabel.text = "比赛结束， 攻方获胜！";
                        break;
                    }
            }
        }

        public void UpdateBattleStat(int scoreNum, int comboKill)
        {
            BattleScoreLabel.text = "个人积分：" + scoreNum;
            ComboKillLabel.text = "连杀：" + comboKill;
        }

        #endregion

        #region Battle Result

        public GameObject BattleResultWindow;

        public UISprite ResultFlagSprite;
        private string resultFlagWin = "result_flag";
        private string resultFlagLose = "result_lose";

        public UISprite ResultSprite;
        private string resultWin = "result_win_3";
        private string resultLose = "result_lose_0";

        public UILabel ResultScoreLabel;
        public UILabel ResultRankLabel;
        public UILabel ResultKillLabel;
        public UILabel ResultDescLabel;

        public UIGrid GainItemGrid;
        public GameObject GainItemPrefab;
        private string gainItemStr;

        public void ShowBattleResult(bool isSucceed, int personalScore, int rank, int killNum, int allianceResult, string gainItemStr)
        {
            ResultFlagSprite.spriteName = isSucceed ? resultFlagWin : resultFlagLose;
            ResultSprite.spriteName = isSucceed ? resultWin : resultLose;

            ResultScoreLabel.text = "个人积分：" + personalScore;
            ResultRankLabel.text = "排名：" + rank;
            ResultKillLabel.text = "杀敌：" + killNum;

            if (allianceResult == 200)
            {
                ResultDescLabel.text = "攻陷敌方基地";
            }
            else if (allianceResult == -200)
            {
                ResultDescLabel.text = "被敌方攻陷基地";
            }
            else if (allianceResult >= 20000)
            {
                ResultDescLabel.text = "攻陷了" + (allianceResult - 20000) + "座营地";
            }
            else if (allianceResult >= 10000)
            {
                ResultDescLabel.text = "守住了" + (allianceResult - 10000) + "座营地";
            }
            else
            {
                Debug.LogError("AB result alliance stat error, " + allianceResult);
            }

            //this.gainItemStr = gainItemStr;
            //while (GainItemGrid.transform.childCount != 0)
            //{
            //    var child = GainItemGrid.transform.GetChild(0);
            //    Destroy(child.gameObject);
            //    child.parent = null;
            //}
            //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadedCallBack);

            BattleResultWindow.SetActive(true);

            TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleResult", m_ResultTotalTime, OnUpdateABResultTime);

            m_Joystick.m_Box.enabled = false;
            m_MainUIVagueEffect.enabled = true;
            m_Top2UIVagueEffect.enabled = true;
        }

        public UILabel m_ResultRemainingLabel;
        private const int m_ResultTotalTime = 30;

        private void OnUpdateABResultTime(int p_time)
        {
            if (m_ResultTotalTime - p_time > 0)
            {
                m_ResultRemainingLabel.text = (m_ResultTotalTime - p_time) + "s后将自动退出战场";
            }
            else
            {
                OnReturnClick();
            }
        }

        private void OnIconSampleLoadedCallBack(ref WWW www, string path, Object loadedObject)
        {
            gainItemStr.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(item =>
           {
               var splited = item.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(item2 => int.Parse(item2)).ToList();

               var iconContainer = Instantiate(GainItemPrefab);
               var iconSample = Instantiate(loadedObject) as GameObject;
               var iconSampleManager = iconSample.GetComponent<IconSampleManager>();
               iconSampleManager.SetIconByID(splited[1], "", 10);
               TransformHelper.ActiveWithStandardize(iconContainer.transform, iconSample.transform);

               var numLabel = TransformHelper.FindChild(iconContainer.transform, "IconNumLabel").GetComponent<UILabel>();
               numLabel.text = splited[2].ToString();

               TransformHelper.ActiveWithStandardize(GainItemGrid.transform, iconContainer.transform, 0.5f);
           });

            GainItemGrid.Reposition();
        }

        #endregion

        #region Occupy Progress Bar

        private const string BarToLeftSpriteName = "ArrowLeft";
        private const string BarToRightSpriteName = "ArrowRight";

        public GameObject ProgressBarObject;
        public UIWidget ProgressBarBG;
        public UISprite ProgressCursor;
        public UIProgressBar ProgressBar;
        public UILabel ProgressPercentLabel;

        public void ShowOccupyBar(bool isShow, float totalValue)
        {
            if (ProgressBarObject.activeInHierarchy != isShow)
            {
                ProgressBarObject.SetActive(isShow);

                if (isShow)
                {
                    ProgressPercentLabel.text = totalValue.ToString();

                    m_RootManager.m_AbHoldPointManager.TryUpdateHoldPointUI();
                }
            }
        }

        public void UpdateOccupyBar(float value, bool isToLeft)
        {
            if (ProgressBarObject.activeInHierarchy)
            {
                ProgressCursor.spriteName = isToLeft ? BarToLeftSpriteName : BarToRightSpriteName;
                ProgressCursor.transform.localPosition = new Vector3(-ProgressBarBG.width / 2.0f + ProgressBarBG.width * value, 0, 0);

                ProgressBar.value = value;
            }
        }

        #endregion

        #region Buy Item Window

        public ABBuyItemWindowManager m_AbBuyItemWindowManager;

        public void ShowBuyItemWindow(ABBuyItemWindowManager.Type p_type, int p_costNum, int p_totalNum)
        {
            m_AbBuyItemWindowManager.gameObject.SetActive(true);
            m_AbBuyItemWindowManager.SetThis(p_type, p_costNum, p_totalNum);
        }

        [HideInInspector]
        public bool IsCanClickBuyBloodTimes = true;

        public void OnBuyBloodTimesClick()
        {
            if (IsCanClickBuyBloodTimes)
            {
                IsCanClickBuyBloodTimes = false;

                BuyXuePingReq temp = new BuyXuePingReq()
                {
                    code = 2
                };
                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer tempSer = new QiXiongSerializer();
                tempSer.Serialize(tempStream, temp);
                byte[] t_protof;
                t_protof = tempStream.ToArray();

                SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_BUY_XueP, ref t_protof);
            }
            else
            {
                Debug.LogWarning("Cancel click buy blood times.");
            }
        }

        [HideInInspector]
        public bool IsCanClickBuySummonTimes = true;

        public void OnBuySummonTimesClick()
        {
            if (IsCanClickBuySummonTimes)
            {
                IsCanClickBuySummonTimes = false;

                BuyXuePingReq temp = new BuyXuePingReq()
                {
                    code = 2
                };
                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer tempSer = new QiXiongSerializer();
                tempSer.Serialize(tempStream, temp);
                byte[] t_protof;
                t_protof = tempStream.ToArray();

                SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_BUY_Summon, ref t_protof);
            }
            else
            {
                Debug.LogWarning("Cancel click buy summon times.");
            }
        }

        #endregion

        #region Big Buff

        public Transform BigBuffParent;

        public void AddBigBuff(int uID)
        {
            RPGBaseCultureController controller = null;

            if (uID == PlayerSceneSyncManager.Instance.m_MyselfUid && m_RootManager.m_SelfPlayerCultureController != null)
            {
                controller = m_RootManager.m_SelfPlayerCultureController;

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconLoadCallBack);
            }
            else if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(uID))
            {
                controller = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[uID].GetComponent<RPGBaseCultureController>();
            }

            if (controller != null)
            {
                ClientMain.m_UITextManager.createText(controller.KingName + "获得称号特效！");
            }
        }

        private void OnIconLoadCallBack(ref WWW www, string path, Object prefab)
        {
            var ins = Instantiate(prefab) as GameObject;
            TransformHelper.ActiveWithStandardize(BigBuffParent, ins.transform);

            var manager = ins.GetComponent<IconSampleManager>();
            manager.SetIconByID(4301);
            manager.SetIconPopText();
        }

        #endregion

        [HideInInspector]
        public int RecommandedScale = -1;
        [HideInInspector]
        public int RecommandedNum = -1;

        private float checkTime1;

        public Camera NGUICamera
        {
            get { return m_nguiCamera ?? (m_nguiCamera = GetComponentInChildren<Camera>()); }
        }

        private Camera m_nguiCamera;

        void Update()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) return;

            #region Update small map

            m_MapController.UpdateGizmosPosition(PlayerSceneSyncManager.Instance.m_MyselfUid, m_RootManager.m_SelfPlayerController.transform.localPosition, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y);

            #endregion

            if (Time.realtimeSinceStartup - checkTime1 > 1.0f)
            {
                //auto active/deactive target.
                AutoActiveTarget();
                AutoDeactiveTarget();

                //Switch NGUI UI state for perfermance.
                m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Select(item => item.Value.GetComponent<ABPlayerCultureController>()).ToList().ForEach(item =>
                {
                    if (UtilityTool.IsInScreen(item.m_UIParentObject.transform.position, m_RootManager.TrackCamera) && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.transform.position) <= SelectDistance * 2)
                    {
                        item.SetUIParentObject(true);
                    }
                    else
                    {
                        item.SetUIParentObject(false);
                    }
                });

                checkTime1 = Time.realtimeSinceStartup;
            }
        }

        void Start()
        {
            ////Execute remaining data.
            //ExecuteReportData();

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_Main");
        }

        void Awake()
        {
            //Load configs.
            RecommandedScale = int.Parse(YunBiaoTemplate.GetValueByKey("rec_zhanli_scale"));
            RecommandedNum = int.Parse(YunBiaoTemplate.GetValueByKey("rec_cartNum"));

            m_tpDuration = 0;

            m_MapController.m_ExecuteAfterOpenMap = ExecuteAfterOpenMap;
            m_MapController.m_ExecuteAfterCloseMap = ExecuteAfterCloseMap;

            //Set variables.
            m_RTSkillExecuter = new RTSkillExecuter
            {
                m_AnimationHierarchyPlayer = m_RootManager.m_AnimationHierarchyPlayer,
                m_PlayerManager = m_RootManager.m_AbPlayerSyncManager,
                m_LogicMain = m_RootManager.m_AllianceBattleMain.gameObject,
                m_SelfIconSetter = SelfIconSetter,
                m_TargetIconSetter = TargetIconSetter,
                m_SelfPlayerController = m_RootManager.m_SelfPlayerController,
                m_SelfPlayerCultureController = m_RootManager.m_SelfPlayerCultureController
            };

            //Bind delegate.
            m_BannerEffectController.m_ExecuteAfterClick = ExecuteAfterEffectClick;
            m_BannerEffectController.m_ExecuteAfterEnd = ExecuteAfterEffectEnd;
        }

#if UNIT_TEST
        void OnGUI()
        {
            if (GUILayout.Button("Test TP"))
            {
                TpToPosition(new Vector2(202, 30));
            }
            if (GUILayout.Button("Test result"))
            {
                ShowBattleResult(true);
            }
        }
#endif
    }
}
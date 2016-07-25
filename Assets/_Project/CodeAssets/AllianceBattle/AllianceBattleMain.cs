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
                //return m_SkillControllers.Where(item => item.m_Template != null && item.m_Template.SkillTarget != 0).ToList();
                return new List<RTSkillController>();
            }
        }

        public UIBackgroundEffect m_MainUIVagueEffect;
        public UIBackgroundEffect m_Top1UIVagueEffect;
        public UIBackgroundEffect m_Top2UIVagueEffect;

        public ABScoreWindowController m_AbScoreWindowController;

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

        #region Battle Result

        public ABResultManager m_ABResultManager;

        #endregion

        #region Wow like pop up Alert

        public UILabel WowEffectAlertLabel;
        private const float WowEffectDuration = 3.5f;

        /// <summary>
        /// Used for hold under attack, hold destroyed.
        /// </summary>
        /// <param name="info"></param>
        public void ShowWowAlert(string info)
        {
            WowEffectAlertLabel.text = info;
            WowEffectAlertLabel.gameObject.SetActive(false);
            WowEffectAlertLabel.gameObject.SetActive(true);

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABWowAlertLabel"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABWowAlertLabel");
            }
            TimeHelper.Instance.AddOneDelegateToTimeCalc("ABWowAlertLabel", WowEffectDuration, EndWowAlert);
        }

        private void EndWowAlert()
        {
            TimeHelper.Instance.RemoveFromTimeCalc("ABWowAlertLabel");

            WowEffectAlertLabel.gameObject.SetActive(false);
        }

        #endregion

        #region Big buff pop up Alert

        public UILabel BigBuffAlertLabel1;
        public UILabel BigBuffAlertLabel2;
        private const float BigBuffDuration = 3.5f;

        /// <summary>
        /// Used for hold under attack, hold destroyed.
        /// </summary>
        /// <param name="info"></param>
        public void ShowBigBuffAlert(string info)
        {
            BigBuffAlertLabel1.text = info;
            BigBuffAlertLabel1.gameObject.SetActive(false);
            BigBuffAlertLabel1.gameObject.SetActive(true);

            BigBuffAlertLabel2.text = info;
            BigBuffAlertLabel2.gameObject.SetActive(false);
            BigBuffAlertLabel2.gameObject.SetActive(true);

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABBigBuffAlertLabel"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABBigBuffAlertLabel");
            }
            TimeHelper.Instance.AddOneDelegateToTimeCalc("ABBigBuffAlertLabel", BigBuffDuration, EndBigBuffAlert);
        }

        private void EndBigBuffAlert()
        {
            TimeHelper.Instance.RemoveFromTimeCalc("ABBigBuffAlertLabel");

            BigBuffAlertLabel1.gameObject.SetActive(false);
            BigBuffAlertLabel2.gameObject.SetActive(false);
        }

        #endregion

        #region UI Click Event

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

                //Check self player not exist, cannot use skill expect summon.
                if ((m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) && template.SkillId != 200)
                {
                    TryCancelChaseToAttack();
                    return;
                }


                //Special check for blood recover.
                if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
                {

                }
                else
                {
                    if (template.SkillId == 121 && battleStatState < 0)
                    {
                        ClientMain.m_UITextManager.createText("备战阶段不可以使用血瓶");
                        return;
                    }
                }

                //Check CD.
                if (tempController.IsInCD)
                {
                    ClientMain.m_UITextManager.createText("技能正在冷却中");
                    return;
                }

                //Update target ,skill target check, return when no target.
                if (template.SkillTarget == 1 && (m_TargetId < 0 || !m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId)))
                {
                    //Play fake animation and effect.
                    ExecuteSelfFakeSkill(template.SkillId);

                    //ClientMain.m_UITextManager.createText("需要选定一个目标");
                    //Cancel chase attack.
                    TryCancelChaseToAttack();
                    DeactiveTarget();
                    return;
                }

                //skill target check, return when 1. cannot operated to player/other player, 2. cannot operated to AllianceBattle.
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 1 && ((m_TargetId < 0) || (m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId))))
                {
                    ClientMain.m_UITextManager.createText("不能对玩家使用");
                    //Cancel chase attack.
                    TryCancelChaseToAttack();
                    return;
                }

                //skill target relationship check, return when 1. cannot operated to friend, 2. cannot operated to enemy.
                if (template.SkillTarget == 1 && template.CRRejectU == 1 && m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<RPGBaseCultureController>().IsEnemy)
                {
                    ClientMain.m_UITextManager.createText("不能对敌方使用");
                    return;
                }
                if (template.SkillTarget == 1 && template.CRRejectU == 2 && (m_TargetId < 0 || (m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<RPGBaseCultureController>().IsEnemy)))
                {
                    ClientMain.m_UITextManager.createText("不能对友方使用");
                    //Cancel chase attack.
                    TryCancelChaseToAttack();
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
                else if (template.SkillId == 300)
                {
                    OnCreateCloneClick();
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

        public void OnReportClick()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TONGZHI), DoOpenReportWindow);
        }

        public void OnCreateCloneClick()
        {
            if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(30))
            {
                ClientMain.m_UITextManager.createText("召唤分身需要V特权达到" + VipFuncOpenTemplate.GetNeedLevelByKey(30) + "级!");
                return;
            }

            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null)
            {
                return;
            }

            if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnCreateCloneBoxLoadCallBack);
            }
            else
            {
                if (battleStatState >= 0)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnCreateCloneBoxLoadCallBack);
                }
                else
                {
                    ClientMain.m_UITextManager.createText("备战阶段不可以召唤分身");
                }
            }
        }

        private void OnCreateCloneBoxLoadCallBack(ref WWW www, string path, Object prefab)
        {
            UIBox uiBox = Instantiate<GameObject>(prefab as GameObject).GetComponent<UIBox>();
            uiBox.setBox("召唤分身",
                LanguageTemplate.GetText(4930).Replace("***", JCZTemplate.GetJCZTemplateByKey("buyfenshen_price").value), null,
                null,
                LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                OnCloneBoxClick,
                null, null, null, false, true, true, false, 100, 0, VipFuncOpenTemplate.GetNeedLevelByKey(30));
        }

        private void OnCloneBoxClick(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.yuanBao < int.Parse(JCZTemplate.GetJCZTemplateByKey("buyfenshen_price").value))
                        {
                            CommonBuy.Instance.ShowIngot();
                            return;
                        }

                        var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Values.Where(item => Vector3.Distance(item.CultureController.transform.position, m_RootManager.m_SelfPlayerController.transform.position) < m_RootManager.m_AbHoldPointManager.HoldPointRange).ToList();

                        if (temp.Any() && !temp.First().IsDestroyed)
                        {
                            ErrorMessage temp2 = new ErrorMessage()
                            {
                                errorCode = temp.First().ID,
                                cmd = temp.First().UID
                            };

                            SocketHelper.SendQXMessage(temp2, ProtoIndexes.LMZ_fenShen);
                        }
                        else
                        {
                            ClientMain.m_UITextManager.createText("需要在战旗附近才能召唤分身!");
                            return;
                        }

                        break;
                    }
            }
        }

        public void OnGuideVideoClick()
        {
            //Play guide video.
            VideoHelper.PlayDramaVideo(EffectIdTemplate.GetPathByeffectId(700003), true);
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

            return allItemList.Where(item => item.Value.GetComponent<RPGBaseCultureController>().IsEnemy).OrderBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).Select(item => item.Key).ToList();
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
            m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ToList().ForEach(item => item.Value.GetComponent<RPGBaseCultureController>().OnDeSelected());

            var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<RPGBaseCultureController>();
            if (temp != null)
            {
                //active selected.
                temp.OnSelected();

                //set target ui info.
                if (temp.RoleID >= 100000)
                {
                    var temp2 = (temp as ABHoldCultureController).m_OccupyPoint;

                    string holdName = (temp2.Type == 4 ? "Ad" : "Nor") + (temp2.Side == 1 ? "Blue" : "Red");

                    TargetIconSetter.SetPlayer(temp.RoleID, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue, 0, holdName);
                }
                else
                {
                    TargetIconSetter.SetPlayer(temp.RoleID, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue, 0);
                }
                TargetIconSetter.gameObject.SetActive(true);
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
                var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].GetComponent<RPGBaseCultureController>();
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
                //sp attack
                case 101:
                case 111:
                    {
                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);
                        m_RTSkillExecuter.ExecuteBeenAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.damage, tempInfo.remainLife, m_TargetId, tempInfo.skillId, tempInfo.isBaoJi);

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
                            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
                            {
                                Debug.LogWarning("===============Close skill mauanlly.");
                            }

                            isCloseSkillManually = true;
                        }

                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);

                        if (TimeHelper.Instance.IsTimeCalcKeyExist("ABStopAoeSkill" + tempInfo.attackUid))
                        {
                            TimeHelper.Instance.RemoveFromTimeCalc("ABStopAoeSkill" + tempInfo.attackUid);
                        }
                        TimeHelper.Instance.AddOneDelegateToTimeCalc("ABStopAoeSkill" + tempInfo.attackUid, RTActionTemplate.GetTemplateByID(151).Param2 / 1000f + 0.5f, StopAOESkill);

                        break;
                    }
                //Long skill
                case 171:
                    {
                        var playerObject = RootManager.GetPlayerObjectByUID(tempInfo.attackUid);
                        var targetObject = RootManager.GetPlayerObjectByUID(tempInfo.targetUid);
                        if (playerObject != null)
                        {
                            playerObject.GetComponent<WeaponSwitcher>().SwitchRangeWeapon(playerObject.GetComponent<RPGBaseCultureController>().RoleID);

                            //Use track skill executer to simulate effect.
                            var trackSkill = playerObject.GetComponent<TrackSkillExecuter>() ?? playerObject.AddComponent<TrackSkillExecuter>();
                            trackSkill.SkillPrefab = m_LongSkillEffectPrefab;
                            trackSkill.SourceObject = playerObject;
                            trackSkill.TargetObject = targetObject;
                            trackSkill.AttackerUID = tempInfo.attackUid;

                            playerObject.GetComponent<ABPlayerCultureController>().m_ExecuteAfterLongSkillShot = trackSkill.Execute;
                            //trackSkill.Execute();
                        }
                        else
                        {
                            Debug.LogWarning("Switch weapon fail cause cannot find player, " + tempInfo.attackUid);
                        }

                        m_RTSkillExecuter.ExecuteAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.skillId);
                        m_RTSkillExecuter.ExecuteBeenAttack(tempInfo.attackUid, tempInfo.targetUid, tempInfo.damage, tempInfo.remainLife, m_TargetId, tempInfo.skillId, tempInfo.isBaoJi);

                        break;
                    }
                //Multi attack skill
                case 181:
                    {
                        if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
                            {
                                Debug.LogWarning("=============Close skill mauanlly.");
                            }

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

        public void ExecuteSelfFakeSkill(int skillId)
        {
            switch (skillId)
            {
                //normal attack
                //sp attack
                case 101:
                case 111:
                    {
                        m_RTSkillExecuter.ExecuteAttack(PlayerSceneSyncManager.Instance.m_MyselfUid, -1, skillId);

                        break;
                    }
                //Recover skill
                case 121:
                    {
                        m_RTSkillExecuter.ExecuteRecover(PlayerSceneSyncManager.Instance.m_MyselfUid, skillId);

                        break;
                    }
                ////Aoe skill
                //case 151:
                //    {
                //        var temp = RootManager.GetPlayerObjectByUID(PlayerSceneSyncManager.Instance.m_MyselfUid);
                //        if (temp != null)
                //        {
                //            temp.GetComponent<WeaponSwitcher>().SwitchWeapon(WeaponSwitcher.WeaponType.Heavy);
                //        }
                //        else
                //        {
                //            Debug.LogWarning("Switch weapon fail cause cannot find player, " + PlayerSceneSyncManager.Instance.m_MyselfUid);
                //        }

                //        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME))
                //        {
                //            Debug.LogWarning("Close skill mauanlly.");
                //        }

                //        isCloseSkillManually = true;

                //        m_RTSkillExecuter.ExecuteAttack(PlayerSceneSyncManager.Instance.m_MyselfUid, -1, skillId);

                //        break;
                //    }
                //Long skill
                case 171:
                    {
                        var playerObject = RootManager.GetPlayerObjectByUID(PlayerSceneSyncManager.Instance.m_MyselfUid);
                        if (playerObject != null)
                        {
                            playerObject.GetComponent<WeaponSwitcher>().SwitchRangeWeapon(playerObject.GetComponent<RPGBaseCultureController>().RoleID);

                            //Use track skill executer to simulate effect.
                            var trackSkill = playerObject.GetComponent<TrackSkillExecuter>() ?? playerObject.AddComponent<TrackSkillExecuter>();
                            trackSkill.SkillPrefab = m_LongSkillEffectPrefab;
                            trackSkill.SourceObject = playerObject;
                            trackSkill.FakeTargetPosition = TransformHelper.Get2DTrackPosition(playerObject.transform.position, playerObject.transform.eulerAngles, SelectDistance);
                            trackSkill.AttackerUID = PlayerSceneSyncManager.Instance.m_MyselfUid;

                            playerObject.GetComponent<ABPlayerCultureController>().m_ExecuteAfterLongSkillShot = trackSkill.ExecuteFake;
                            //trackSkill.ExecuteFake();
                        }
                        else
                        {
                            Debug.LogWarning("Switch weapon fail cause cannot find player, " + PlayerSceneSyncManager.Instance.m_MyselfUid);
                        }

                        m_RTSkillExecuter.ExecuteAttack(PlayerSceneSyncManager.Instance.m_MyselfUid, -1, skillId);
                        break;
                    }
                    ////Multi attack skill
                    //case 181:
                    //    {
                    //        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME))
                    //        {
                    //            Debug.LogWarning("Close skill mauanlly.");
                    //        }

                    //        isCloseSkillManually = true;

                    //        MultiAttackSkillExecuter.Execute(PlayerSceneSyncManager.Instance.m_MyselfUid, -1);

                    //        break;
                    //    }
            }

            //Set cds.
            m_SkillControllers.ForEach(item => item.TryStartSharedCD());
            var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == skillId).ToList();
            if (triggeredSkill.Any())
            {
                triggeredSkill.First().TryStartSelfCD();
            }
        }

        public void RestoreSkill()
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
            {
                Debug.LogWarning("+++++++++++++++Open skill mauanlly.");
            }

            isCloseSkillManually = false;
        }

        public void StopAOESkill(string str)
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist(str))
            {
                TimeHelper.Instance.RemoveFromTimeCalc(str);
            }

            StopAOESkill(int.Parse(str.Replace("ABStopAoeSkill", "")));
        }

        public void StopAOESkill(int p_uid)
        {
            if (m_RootManager.m_AnimationHierarchyPlayer.IsCanPlayAnimation(p_uid, "EndDadaoSkill"))
            {
                if (p_uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                {
                    if (m_RootManager.m_SelfPlayerController != null)
                    {
                        m_RootManager.m_SelfPlayerCultureController.m_ExecuteAfterSkillFinish = RestoreSkill;
                        m_RootManager.m_SelfPlayerController.DeactiveMove();
                    }
                    else
                    {
                        Debug.LogWarning("play EndDadaoSkill in uid: " + p_uid + "fail cause self not exist.");
                    }
                }
                else
                {
                    var otherPlayer = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Where(item => item.Key == p_uid).ToList();
                    if (otherPlayer.Any())
                    {
                        otherPlayer.First().Value.DeactiveMove();
                    }
                    else
                    {
                        Debug.LogWarning("play EndDadaoSkill in uid: " + p_uid + "fail cause target not exist.");
                    }
                }

                m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(p_uid, "EndDadaoSkill");
            }
            else
            {
                Debug.LogWarning("play EndDadaoSkill in uid: " + p_uid + "fail in hierarchy");
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

                            var temp = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[tempInfo.targetId].GetComponent<RPGBaseCultureController>();
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
                    float minors = info.remainLife - m_RootManager.m_SelfPlayerCultureController.RemainingBlood;
                    if (Math.Abs(minors) > float.MinValue)
                    {
                        m_RootManager.m_SelfPlayerCultureController.ShowBloodChange((long)Math.Abs(minors), minors < 0, false);
                    }

                    m_RootManager.m_SelfPlayerCultureController.TotalBlood = info.damage;
                    m_RootManager.m_SelfPlayerCultureController.UpdateBloodBar(info.remainLife);

                    SelfIconSetter.UpdateBar(info.remainLife);
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
                    float minors = info.remainLife - m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().RemainingBlood;
                    if (Math.Abs(minors) > float.MinValue)
                    {
                        m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().ShowBloodChange((long)Math.Abs(minors), minors < 0, false);
                    }

                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().TotalBlood = info.damage;
                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[info.targetUid].GetComponent<RPGBaseCultureController>().UpdateBloodBar(info.remainLife);

                    if (m_TargetId == info.targetUid)
                    {
                        TargetIconSetter.UpdateBar(info.remainLife);
                    }
                }
                else
                {
                    Debug.LogError("Cannot update total blood cause target: " + info.targetUid + " not exist.");
                    return;
                }
            }
        }

        #endregion

        #region Chase Attack

        [HideInInspector]
        public bool IsChaseAttack
        {
            get { return isChaseAttack; }
            set
            {
                if (value && !isChaseAttack)
                {
                    AnimationController.ShowAnimation(Res2DTemplate.GetResPath(Res2DTemplate.Res.CHASE_ATTACK_NAV));
                }
                else if (!value && isChaseAttack)
                {
                    AnimationController.StopAnimation();
                }

                isChaseAttack = value;
            }
        }

        private bool isChaseAttack = false;

        private int chaseTargetID = -1;

        public void OnChaseAttackClick()
        {
            if (m_TargetId < 0)
            {
                ClientMain.m_UITextManager.createText("请在选择一个敌方目标后使用");
                return;
            }

            if (IsChaseAttack)
            {
                ClientMain.m_UITextManager.createText("自动追击中...");
                return;
            }

            IsChaseAttack = true;
            chaseTargetID = m_TargetId;
        }

        public UISprite ChaseToAttackSprite;

        public void TryCancelChaseToAttack()
        {
            IsChaseAttack = false;
        }

        public void UpdateChaseState()
        {
            if (IsChaseAttack && (m_RootManager.m_SelfPlayerController.m_RealJoystickOffset != Vector3.zero || m_TargetId != chaseTargetID || !m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) || m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null))
            {
                IsChaseAttack = false;
            }
        }

        public void UpdateChaseBTNColor()
        {
            if (!IsChaseAttack && m_TargetId >= 0 && m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null)
            {
                if (ChaseToAttackSprite.color != Color.white)
                {
                    SetChaseBTNColor(true);
                }
            }
            else
            {
                if (ChaseToAttackSprite.color != Color.grey)
                {
                    SetChaseBTNColor(false);
                }
            }
        }

        public void SetChaseBTNColor(bool isHighlight)
        {
            ChaseToAttackSprite.color = isHighlight ? Color.white : Color.grey;

            if (isHighlight)
            {
                SparkleEffectItem.OpenSparkle(ChaseToAttackSprite.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
            }
            else
            {
                SparkleEffectItem.CloseSparkle(ChaseToAttackSprite.gameObject);
            }
        }

        #endregion

        #region Dead and Rebrith[FIX]

        public void ExecuteDead(int p_uID)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
            {
                Debug.LogWarning("==================Player dead execute: " + p_uID);
            }

            if (p_uID == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                Destroy(m_RootManager.m_SelfPlayerController.gameObject);

                m_RootManager.m_SelfPlayerCultureController = null;
                m_RootManager.m_SelfPlayerController = null;
                DeactiveTarget();

                //Show dead dimmer.
                ShowDeadWindow(m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.autoReviveRemainTime, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.remainAllLifeTimes / 1000, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.remainAllLifeTimes % 1000, 0, m_RootManager.m_AbPlayerSyncManager.m_StoredPlayerDeadNotify.onSiteReviveCost);

                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_ALLIANCE_BATTLE))
                {
                    Debug.LogWarning("+++++++++++++++Open skill mauanlly.");
                }

                //Reset skill close state.
                isCloseSkillManually = false;
            }
            else if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
            {
                //Is hold point.
                if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].IsHold)
                {
                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].GetComponent<ABHoldCultureController>().SetHoldPointState(false);

                    m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Remove(p_uID);
                }
                else
                {
                    //Remove from mesh controller.
                    if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].GetComponent<RPGBaseCultureController>().KingName != JunZhuData.Instance().m_junzhuInfo.name)
                    {
                        ModelAutoActivator.UnregisterAutoActivator(m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].gameObject);
                    }

                    m_RootManager.m_AbPlayerSyncManager.DestroyPlayer(p_uID);
                }
            }
        }

        public GameObject DeadWindow;

        public UILabel ShowRebirthInfoLabel;

        public GameObject FreeQuickRebirthObject;
        public GameObject StoneQuickRebirthObject;
        public GameObject IngotQuickRebirthObject;

        public GameObject QuickRebirthButton;
        public GameObject CannotQuickRebirthButton;

        public UILabel FreeQuickRebirthLabel;
        public UILabel StoneQuickRebirthTimesLabel;
        public UILabel StoneQuickRebirthTotalLabel;
        public UILabel IngotQuickRebirthTimesLabel;
        public UILabel IngotQuickRebirthTotalLabel;

        public GameObject VipObject;
        public UISprite VipSprite;

        private int m_storedSlowRebirthTimes;
        private int m_quickRebirthType;

        private int m_remainingQuickRebirthTimes;

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

            m_remainingQuickRebirthTimes = remainQuickRebirthTimes;

            FreeQuickRebirthObject.SetActive(false);
            StoneQuickRebirthObject.SetActive(false);
            IngotQuickRebirthObject.SetActive(false);
            //Which to show.
            //if (remainQuickRebirthTimes > 0)
            //{
            FreeQuickRebirthLabel.text = "今日剩余快速复活次数" + remainQuickRebirthTimes + "/" + VipTemplate.GetVipABRebirthInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv);
            FreeQuickRebirthObject.SetActive(true);
            //m_quickRebirthType = 20;
            //}
            //else if (quickRebirthStone > 0)
            //{
            //    StoneQuickRebirthTimesLabel.text = "x1";
            //    var bagList = BagData.Instance().m_bagItemList;
            //    var temp = bagList.Where(item => item.itemId == 910010).ToList();
            //    StoneQuickRebirthTotalLabel.text = "拥有：" + (temp.Any() ? temp.First().cnt : 0) + "个";
            //    StoneQuickRebirthObject.SetActive(true);
            //    m_quickRebirthType = 30;
            //}
            //else
            //{
            IngotQuickRebirthTimesLabel.text = "x" + quickRebirthCost;
            IngotQuickRebirthTotalLabel.text = "拥有：" + JunZhuData.Instance().m_junzhuInfo.yuanBao;
            IngotQuickRebirthObject.SetActive(true);
            m_quickRebirthType = 40;
            //}

            //Set quick rebirth btn.
            if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipTemplate.GetLevelOfABQuickRebirthStart() && remainQuickRebirthTimes == 0)
            {
                QuickRebirthButton.SetActive(false);
                CannotQuickRebirthButton.SetActive(true);
            }
            else
            {
                QuickRebirthButton.SetActive(true);
                CannotQuickRebirthButton.SetActive(false);
            }

            //Set vip sign.
            int vip = VipTemplate.templates.Where(item => item.ABRebirth > 0).OrderBy(item => item.lv).First().lv;

            VipObject.SetActive(true);
            VipSprite.spriteName = "v" + vip;

            m_Joystick.m_Box.enabled = false;
        }

        public void TryRefreshVIPInDeadWindow(int vipID)
        {
            if (DeadWindow.activeInHierarchy)
            {
                FreeQuickRebirthLabel.text = "今日剩余快速复活次数" + m_remainingQuickRebirthTimes + "/" + VipTemplate.GetVipABRebirthInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv);
                FreeQuickRebirthObject.SetActive(true);

                //Set quick rebirth btn.
                if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipTemplate.GetLevelOfABQuickRebirthStart() && m_remainingQuickRebirthTimes == 0)
                {
                    QuickRebirthButton.SetActive(false);
                    CannotQuickRebirthButton.SetActive(true);
                }
                else
                {
                    QuickRebirthButton.SetActive(true);
                    CannotQuickRebirthButton.SetActive(false);
                }
            }
        }

        public void HideDeadWindows()
        {
            DeadWindow.SetActive(false);

            if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
            }

            m_Joystick.m_Box.enabled = true;
        }

        private void SetRebirthTime(int time)
        {
            if (ShowRebirthInfoLabel != null && ShowRebirthInfoLabel.gameObject.activeInHierarchy)
            {
                ShowRebirthInfoLabel.text = "距离复活时间还有" + (m_storedSlowRebirthTimes - time) + "秒";
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
            if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipTemplate.GetLevelOfABQuickRebirthStart())
            {
                CommonBuy.Instance.ShowVIP();
                return;
            }
            else if (m_remainingQuickRebirthTimes == 0)
            {
                ClientMain.m_UITextManager.createText("今日快速复活次数已用尽");
                return;
            }

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

        public void OnCannotQuickRebirthClick()
        {
            if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipTemplate.GetLevelOfABQuickRebirthStart())
            {
                CommonBuy.Instance.ShowVIP();
                return;
            }
            else if (m_remainingQuickRebirthTimes == 0)
            {
                ClientMain.m_UITextManager.createText("今日快速复活次数已用尽");
                return;
            }
        }

        [Obsolete]
        private void CannotQuickRebirthCallBack(ref WWW www, string path, Object prefab)
        {
            UIBox uibox = (Instantiate(prefab) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox("快速复活",
                 "V特权到达" + VipTemplate.GetLevelOfABQuickRebirthStart() + "级可以购买快速复活", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
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
            float yPos;
            Vector3 tempPos = new Vector3(p_position.x, RootManager.BasicYPosition, p_position.y);
            if (TransformHelper.RayCastXToFirstCollider(tempPos, out yPos))
            {
                tempPos = new Vector3(p_position.x, yPos, p_position.y);
            }

            m_RootManager.m_SelfPlayerController.transform.localPosition = tempPos;
        }

        #endregion

        #region ChaseToAttack/Navigation Animation

        public void ShowNavigationAnim()
        {
            if (!IsChaseAttack)
            {
                AnimationController.ShowAnimation(Res2DTemplate.GetResPath(Res2DTemplate.Res.AUTO_NAV));
            }
        }

        public void StopNavigationAnim()
        {
            if (!IsChaseAttack)
            {
                AnimationController.StopAnimation();
            }
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
            if (Global.m_listJunchengData != null && Global.m_listJunchengData.Any())
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

            foreach (TongzhiData data in Global.m_listJunchengData)
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

        #region Simple Report

        public ABTongzhi m_AbTongzhi;

        public void ShowSimpleReport(int uID, string info)
        {
            m_AbTongzhi.TargetUID = uID;
            m_AbTongzhi.TargetPosition = Vector3.zero;

            m_AbTongzhi.ShowUI(info);
        }

        public void ShowSimpleReport(Vector3 position, string info)
        {
            m_AbTongzhi.TargetUID = -1;
            m_AbTongzhi.TargetPosition = position;

            m_AbTongzhi.ShowUI(info);
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
                    controller.m_AllianceBattleMain = this;
                    controller.SetThis();

                    controller.gameObject.name = i + "_" + controller.gameObject.name;
                    controller.gameObject.SetActive(true);
                }

                CommandGrid.Reposition();
            }
        }

        public void ShowCommandMapEffect(int index)
        {
            switch (index % 10)
            {
                case 1:
                    {
                        var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Where(item => (item.Key == 201 || item.Key == 301 || item.Key == 401) && !item.Value.IsDestroyed).OrderBy(item => item.Key).ToList();
                        if (temp.Any())
                        {
                            m_MapController.m_MapEffectController.BlinkBeenAttackEffect(temp.First().Value.UID, m_MapController.m_ItemGizmosDic[temp.First().Value.UID].localPosition);
                        }

                        break;
                    }
                case 2:
                    {
                        var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Where(item => (item.Key == 202 || item.Key == 302 || item.Key == 401) && !item.Value.IsDestroyed).OrderBy(item => item.Key).ToList();
                        if (temp.Any())
                        {
                            m_MapController.m_MapEffectController.BlinkBeenAttackEffect(temp.First().Value.UID, m_MapController.m_ItemGizmosDic[temp.First().Value.UID].localPosition);
                        }

                        break;
                    }
                case 3:
                    {
                        var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Where(item => (item.Key == 203 || item.Key == 303 || item.Key == 401) && !item.Value.IsDestroyed).OrderBy(item => item.Key).ToList();
                        if (temp.Any())
                        {
                            m_MapController.m_MapEffectController.BlinkBeenAttackEffect(temp.First().Value.UID, m_MapController.m_ItemGizmosDic[temp.First().Value.UID].localPosition);
                        }

                        break;
                    }
                case 4:
                    {
                        //Attacker
                        if (m_RootManager.MyPart == 1)
                        {
                            var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Where(item => (item.Key == 401) && !item.Value.IsDestroyed).OrderBy(item => item.Key).ToList();

                            if (temp.Any())
                            {
                                m_MapController.m_MapEffectController.BlinkBeenAttackEffect(temp.First().Value.UID, m_MapController.m_ItemGizmosDic[temp.First().Value.UID].localPosition);
                            }
                        }
                        //Protecter
                        else if (m_RootManager.MyPart == 2)
                        {
                            var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Where(item => (item.Key == 402) && !item.Value.IsDestroyed).OrderBy(item => item.Key).ToList();

                            if (temp.Any())
                            {
                                m_MapController.m_MapEffectController.BlinkBeenAttackEffect(temp.First().Value.UID, m_MapController.m_ItemGizmosDic[temp.First().Value.UID].localPosition);
                            }
                        }

                        break;
                    }
                case 5:
                    {
                        break;
                    }
            }
        }

        public int RemainingCmdCD = -1;
        public int StoredTotalCmdCD;

        public void StartCommandCDCalc(int duration = 10)
        {
            StoredTotalCmdCD = duration;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABCmdCD"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABCmdCD");
            }

            TimeHelper.Instance.AddEveryDelegateToTimeCalc("ABCmdCD", duration, UpdateCommandCD);
        }

        private void UpdateCommandCD(int cd)
        {
            RemainingCmdCD = StoredTotalCmdCD - cd;

            if (RemainingCmdCD <= 0)
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABCmdCD");
            }
        }

        #endregion

        #region Summon

        private Vector3 m_storedSummonTargetPosition;
        private string m_storedSummonTargetName;
        private bool isSummonWindowShowed;
        private UIBox m_storedSummonWindow;

        public UILabel m_RemainingSummonNumLabel;
        public int m_RemainSummonNum;

        public GameObject m_AddSummonTimesBTN;
        public GameObject m_SummonBTN;

        public void OnSummonClick()
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
            {
                SocketHelper.SendQXMessage(ProtoIndexes.LMZ_ZhaoHuan);
            }
            else
            {
                if (battleStatState >= 0)
                {
                    SocketHelper.SendQXMessage(ProtoIndexes.LMZ_ZhaoHuan);
                }
                else
                {
                    ClientMain.m_UITextManager.createText("备战阶段不可以进行召唤");
                }
            }
        }

        public void ExecuteSummon(int p_uID, string info)
        {
            if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
            {
                ShowSimpleReport(p_uID, info);
            }
            else if (m_RootManager.m_AbPlayerSyncManager.m_DeadPlayerDic.ContainsKey(p_uID) && PlayerSceneSyncManager.Instance.m_MyselfUid != p_uID)
            {
                ShowSimpleReport(m_RootManager.m_AbPlayerSyncManager.m_DeadPlayerDic[p_uID].m_PositionOnDead, info);
            }
            else if (PlayerSceneSyncManager.Instance.m_MyselfUid == p_uID)
            {
                //Show summon response.
                ClientMain.m_UITextManager.createText(ColorTool.GetColorString("31ff3b", LanguageTemplate.GetText(4801)));

                //Set cds.
                m_SkillControllers.ForEach(item => item.TryStartSharedCD());
                var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == 200).ToList();
                if (triggeredSkill.Any())
                {
                    triggeredSkill.First().TryStartSelfCD(false);
                }
            }
        }

        [Obsolete]
        public void ExecuteSummon(int p_uID)
        {
            if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
            {
                //Show summon window.
                if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(p_uID))
                {
                    m_storedSummonTargetPosition = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].transform.localPosition;
                    m_storedSummonTargetName = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[p_uID].GetComponent<RPGBaseCultureController>().KingName;
                }

                if (isSummonWindowShowed && m_storedSummonWindow != null)
                {
                    Destroy(m_storedSummonWindow);
                    m_storedSummonWindow = null;
                }

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ShowSummonWindow);
            }

            if (PlayerSceneSyncManager.Instance.m_MyselfUid == p_uID)
            {
                //Show summon response.
                ClientMain.m_UITextManager.createText(ColorTool.GetColorString("31ff3b", LanguageTemplate.GetText(4801)));

                //Set cds.
                m_SkillControllers.ForEach(item => item.TryStartSharedCD());
                var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == 200).ToList();
                if (triggeredSkill.Any())
                {
                    triggeredSkill.First().TryStartSelfCD(false);
                }
            }
        }

        [Obsolete]
        private void ShowSummonWindow(ref WWW p_www, string p_path, Object p_object)
        {
            isSummonWindowShowed = true;

            m_storedSummonWindow = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            m_storedSummonWindow.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "是否传送至" + m_storedSummonTargetName + "处?", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                 ClickSummonWindow);
        }

        [Obsolete]
        private void ClickSummonWindow(int i)
        {
            isSummonWindowShowed = false;

            switch (i)
            {
                case 1:
                    break;
                case 2:
                    TpToPosition(new Vector2(m_storedSummonTargetPosition.x, m_storedSummonTargetPosition.z));
                    break;
            }
        }

        public void SetRemainingSummonNum(int remainingNum)
        {
            if (AllianceData.Instance.g_UnionInfo.identity > 0)
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
            else
            {
                Debug.LogWarning("Canont set remaining summon num cause normal identity.");
            }
        }

        #endregion

        #region Battle Stat

        public UILabel BattleTimeLabel;
        public UILabel BattleScoreLabel;
        public UILabel ComboKillLabel;

        /// <summary>
        /// -1 for not started, 0 for started, 1 for protecter wins, 2 for attacker wins.
        /// </summary>
        [HideInInspector]
        public int battleStatState = -100;

        public void UpdateBattleTime(int remainingTime, int state)
        {
            battleStatState = state;

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

        #region Occupy Progress Bar

        public GameObject ProgressBarObject;
        public UIProgressBar ProgressBar;
        public UISprite ProgressBarFore;
        public UILabel ProgressPercentLabel;
        public UIProgressBar OriginalHoldPointBar;

        private EventDelegate UpdateBar;

        private const string OccupyBarGreen = "BloodGreen";
        private const string OccupyBarBlue = "BloodBlue";
        private const string OccupyBarRed = "BloodRed";

        public void ShowOccupyBar(bool isShow, int holdID)
        {
            if (ProgressBarObject.activeInHierarchy != isShow)
            {
                ProgressBarObject.SetActive(isShow);

                if (isShow)
                {
                    ProgressPercentLabel.text = m_RootManager.m_AbHoldPointManager.HoldPointDic[holdID].CultureController.TotalBlood.ToString();
                    ProgressBarFore.spriteName = m_RootManager.m_AbHoldPointManager.HoldPointDic[holdID].CultureController.IsEnemy ? OccupyBarRed : OccupyBarGreen;

                    OriginalHoldPointBar = m_RootManager.m_AbHoldPointManager.HoldPointDic[holdID].CultureController.ProgressBar;
                    if (OriginalHoldPointBar != null && !OriginalHoldPointBar.onChange.Contains(UpdateBar))
                    {
                        ProgressBar.value = OriginalHoldPointBar.value;
                        ProgressBarFore.gameObject.SetActive(ProgressBar.value > 0.01f);

                        OriginalHoldPointBar.onChange.Add(UpdateBar);
                    }
                }
                else
                {
                    if (OriginalHoldPointBar != null && OriginalHoldPointBar.onChange.Contains(UpdateBar))
                    {
                        OriginalHoldPointBar.onChange.Remove(UpdateBar);
                    }
                    OriginalHoldPointBar = null;
                }
            }
        }

        public void UpdateOccupyBar()
        {
            UpdateOccupyBar(OriginalHoldPointBar.value);
        }

        public void UpdateOccupyBar(float value)
        {
            if (ProgressBarObject.activeInHierarchy)
            {
                ProgressBar.value = value;

                ProgressBarFore.gameObject.SetActive(ProgressBar.value > 0.01f);
            }
        }

        #endregion

        #region Buy Item Window

        public ABBuyItemWindowManager m_AbBuyItemWindowManager;

        public void ShowBuyItemWindow(ABBuyItemWindowManager.Type p_type, int p_costNum, int p_remainingTimes)
        {
            m_AbBuyItemWindowManager.gameObject.SetActive(true);
            m_AbBuyItemWindowManager.SetThis(p_type, p_costNum, p_remainingTimes);
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

        public UIGrid SelfBuffsGrid;
        public GameObject BigBuffObject;
        private int bigBuffID;
        //4301 4302 4303 4304 4305
        private const int BigBuffStartID = 4301;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="buffID"></param>
        /// <param name="isAdd"></param>
        /// <param name="isShowNotification"></param>
        public void SetBigBuff(bool isAdd, int uID, int buffID = -1, bool isShowNotification = true)
        {
            bigBuffID = buffID;

            ABPlayerCultureController controller = null;

            //Get controller.
            if (uID == PlayerSceneSyncManager.Instance.m_MyselfUid && m_RootManager.m_SelfPlayerCultureController != null)
            {
                controller = m_RootManager.m_SelfPlayerCultureController;
            }
            else if (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.ContainsKey(uID))
            {
                controller = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[uID].GetComponent<ABPlayerCultureController>();
            }

            if (controller != null)
            {
                //Add icon if self
                if (uID == PlayerSceneSyncManager.Instance.m_MyselfUid)
                {
                    if (isAdd)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnBigBuffLoadCallBack);
                    }
                    else
                    {
                        if (BigBuffObject != null)
                        {
                            Destroy(BigBuffObject);
                            BigBuffObject = null;
                        }
                    }
                }

                //Play effect.
                controller.SetBigEffect(buffID - BigBuffStartID, isAdd);

                //Notification.
                if (isAdd && isShowNotification)
                {
                    ShowBigBuffAlert(LanguageTemplate.GetText(4920 + buffID - BigBuffStartID).Replace("玩家名字七个字", controller.KingName));
                }
            }
        }

        private void OnBigBuffLoadCallBack(ref WWW www, string path, Object prefab)
        {
            if (BigBuffObject == null)
            {
                BigBuffObject = Instantiate(prefab) as GameObject;
            }

            TransformHelper.ActiveWithStandardize(SelfBuffsGrid.transform, BigBuffObject.transform);
            var manager = BigBuffObject.GetComponent<IconSampleManager>();
            manager.SetIconByID(bigBuffID);
            manager.SetIconPopText();

            SelfBuffsGrid.Reposition();
        }

        #endregion

        #region Attacker gained buff

        public int AttackerGainBuffPercent = 0;
        public GameObject AttackerGainBuffObject;

        public void RefreshAttackerGainedBuff()
        {
            if (m_RootManager.MyPart != 1)
            {
                return;
            }

            AttackerGainBuffPercent = 0;
            if (m_RootManager.m_AbHoldPointManager.HoldPointDic.Values.Any(item => item.IsDestroyed && item.Type == 2 && item.Side == 1))
            {
                AttackerGainBuffPercent = 3;
            }
            if (m_RootManager.m_AbHoldPointManager.HoldPointDic.Values.Any(item => item.IsDestroyed && item.Type == 3 && item.Side == 1))
            {
                AttackerGainBuffPercent = 5;
            }

            if (AttackerGainBuffPercent > 0)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnAttackerGainBuffLoadCallBack);
            }
            else
            {
                if (AttackerGainBuffObject != null)
                {
                    Destroy(AttackerGainBuffObject);
                    AttackerGainBuffObject = null;
                }
            }
        }

        private void OnAttackerGainBuffLoadCallBack(ref WWW www, string path, Object prefab)
        {
            if (AttackerGainBuffObject == null)
            {
                AttackerGainBuffObject = Instantiate(prefab) as GameObject;
            }

            TransformHelper.ActiveWithStandardize(SelfBuffsGrid.transform, AttackerGainBuffObject.transform);
            var manager = AttackerGainBuffObject.GetComponent<IconSampleManager>();
            manager.SetIconByID(4401, "x" + AttackerGainBuffPercent, 5);
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
                m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Select(item => item.Value.GetComponent<RPGBaseCultureController>()).ToList().ForEach(item =>
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

                //Set attacker base attack effect.
                var temp = m_RootManager.m_AbHoldPointManager.HoldPointDic.Values.Where(item => item.Type == 4 && item.Side == 2).ToList();
                if (temp.Any())
                {
                    if (!temp.First().IsDestroyed && (m_RootManager.m_AbPlayerSyncManager.m_PlayerDic.Any(item => !item.Value.IsHold && Vector3.Distance(temp.First().CultureController.transform.position, item.Value.transform.position) < m_RootManager.m_AbHoldPointManager.HoldPointRange && !m_RootManager.IsAttacker(item.Value.GetComponent<RPGBaseCultureController>().AllianceName)) || (m_RootManager.m_SelfPlayerController != null && Vector3.Distance(temp.First().CultureController.transform.position, m_RootManager.m_SelfPlayerController.transform.position) < m_RootManager.m_AbHoldPointManager.HoldPointRange && m_RootManager.MyPart == 2)))
                    {
                        temp.First().CultureController.AttackEffect.SetActive(true);
                    }
                    else
                    {
                        temp.First().CultureController.AttackEffect.SetActive(false);
                    }
                }

                checkTime1 = Time.realtimeSinceStartup;
            }

            //Execute chase attack.
            UpdateChaseState();
            UpdateChaseBTNColor();

            if (IsChaseAttack && m_TargetId >= 0)
            {
                var tempController = m_SkillControllers.Where(item => item.m_Index == 101).First();
                var template = tempController.m_Template;

                if (!tempController.IsInCD)
                {
                    OnSkillClick(101);
                }
                else if (!m_RootManager.m_SelfPlayerController.IsInNavigate || Time.realtimeSinceStartup - checkTime1 > 1.0f)
                {
                    if (Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform.position) > template.Range_Max)
                    {
                        m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = null;
                        m_TargetItemTransform = m_RootManager.m_AbPlayerSyncManager.m_PlayerDic[m_TargetId].transform;
                        NavigateToItem();
                    }
                }
            }
        }

        void Start()
        {
            ////Execute remaining data.
            //ExecuteReportData();

            m_SummonBTN.SetActive(AllianceData.Instance.g_UnionInfo.identity > 0);
            m_AddSummonTimesBTN.SetActive(AllianceData.Instance.g_UnionInfo.identity > 0);

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
            UpdateBar = new EventDelegate(UpdateOccupyBar);
        }

        void OnGUI()
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_UNIT_TEST))
            {
                if (GUILayout.Button("Test hold dead"))
                {
                    //Play dead animation.
                    if (m_RootManager.m_AnimationHierarchyPlayer.IsCanPlayAnimation(m_TargetId, "Dead"))
                    {
                        m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(m_TargetId, "Dead");
                    }
                }
                if (GUILayout.Button("Test2"))
                {
                    m_RootManager.m_AllianceBattleMain.m_MapController.m_MapEffectController.PlayUIEffect(m_RootManager.m_AllianceBattleMain.m_MapController.m_ItemGizmosDic[m_TargetId].gameObject, 620242);
                }
                if (GUILayout.Button("Test1"))
                {
                    m_RootManager.m_AllianceBattleMain.m_MapController.m_MapEffectController.PlayUIEffect(m_RootManager.m_AllianceBattleMain.m_MapController.m_ItemGizmosDic[m_TargetId].gameObject, 620241);
                }
                if (GUILayout.Button("Test wow alert"))
                {
                    ShowWowAlert("杀了杀了都傻了");
                }
                if (GUILayout.Button("Test bb alert"))
                {
                    ShowBigBuffAlert("Test bb alert");
                }
                if (GUILayout.Button("Test big buff"))
                {
                    SetBigBuff(true, PlayerSceneSyncManager.Instance.m_MyselfUid, 4301);
                }
                if (GUILayout.Button("Test big buff"))
                {
                    SetBigBuff(true, PlayerSceneSyncManager.Instance.m_MyselfUid, 4304);
                }
                if (GUILayout.Button("Test result"))
                {
                    m_RootManager.m_AllianceBattleMain.m_ABResultManager.ShowBattleResult(false, 999, 888, 777, 666);
                }
                if (GUILayout.Button("Test animation bug"))
                {
                    m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(PlayerSceneSyncManager.Instance.m_MyselfUid, "Dead");
                    m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(PlayerSceneSyncManager.Instance.m_MyselfUid, "DadaoSkill");
                }
                if (GUILayout.Button("Test animation bug2"))
                {
                    StopAllCoroutines();
                    StartCoroutine("testcoroutine");
                }
                if (GUILayout.Button("Test specific cd."))
                {
                    m_SkillControllers.First().TryStartAmountOfSelfCD(15);
                }
                if (GUILayout.Button("Test monster click."))
                {
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                    OnSkillClick(121);
                }
                if (GUILayout.Button("Test monster click."))
                {
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                    OnSkillClick(101);
                }
            }
        }

        IEnumerator testcoroutine()
        {
            m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(PlayerSceneSyncManager.Instance.m_MyselfUid, "Dead");

            yield return new WaitForEndOfFrame();

            m_RootManager.m_AnimationHierarchyPlayer.TryPlayAnimation(PlayerSceneSyncManager.Instance.m_MyselfUid, "DadaoSkill");
        }
    }
}
//#define DEBUG_CARRIAGE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Carriage
{
    public class CarriageMain : MonoBehaviour
    {
        public Joystick m_Joystick;
        public UISprite m_StartCarriageBTN;
        [HideInInspector]
        public bool isCanStartCarriage = true;
        [HideInInspector]
        public int RemainingStartCarriageTimes = 0;
        [HideInInspector]
        public int RemainingRobCarriageTimes = 0;
        [HideInInspector]
        public int RemainingAdditionalStartTimes = 0;

        public List<long> m_IHelpOtherJunzhuIdList = new List<long>();

        public RootManager m_RootManager;

        public HeadIconSetter SelfIconSetter;
        public HeadIconSetter TargetIconSetter;

        public UIBackgroundEffect m_MainUIVagueEffect;
        public UIBackgroundEffect m_Top1UIVagueEffect;
        public UIBackgroundEffect m_Top2UIVagueEffect;

        public List<RTSkillController> m_SkillControllers = new List<RTSkillController>();

        public List<RTSkillController> m_CanActivedSkillControllers
        {
            get
            {
                return m_SkillControllers.Where(item => item.m_Template.SkillTarget != 0).ToList();
            }
        }

        #region Map Controller

        public MapController m_MapController;

        public void ExecuteAfterOpenMap()
        {
            //open vague
            m_MainUIVagueEffect.enabled = true;
            m_Joystick.m_Box.enabled = false;
        }

        public void ExecuteAfterCloseMap()
        {
            //close vague
            m_MainUIVagueEffect.enabled = false;
            m_Joystick.m_Box.enabled = true;
        }

        #endregion

        #region Start Carriage

        public void DoStartCarriage()
        {
            BiaoJuData.Instance.OpenBiaoJu();
        }

        #endregion

        #region UI Click Event

        public void OnReturnClick()
        {
            CityGlobalData.m_isJieBiaoScene = false;
            PlayerSceneSyncManager.Instance.ExitCarriage();
        }

        public void OnStartCarriageClick()
        {
            if (!isCanStartCarriage)
            {
                ClientMain.m_UITextManager.createText("正在运镖中...");
                return;
            }

            if (m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null)
            {
                //Cancel chase.
                RootManager.Instance.m_CarriageMain.TryCancelChaseToAttack();

                m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = m_RootManager.m_CarriageMain.DoStartCarriage;
                m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_CarriageSafeArea.m_CarriageNPCList.OrderBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.transform.position)).First().transform.position);
            }
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

        public void OnSkillClick(int index)
        {
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
                if (template.SkillTarget == 1 && (m_TargetId < 0 || !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId)))
                {
                    ClientMain.m_UITextManager.createText("需要选定一个目标");
                    TryCancelChaseToAttack();
                    DeactiveTarget();
                    return;
                }

                //skill target check, return when 1. cannot operated to player/other player, 2. cannot operated to carriage.
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 1 && ((m_TargetId < 0) || (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage)))
                {
                    ClientMain.m_UITextManager.createText("不能对玩家使用");
                    TryCancelChaseToAttack();
                    return;
                }
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 2 && (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage))
                {
                    ClientMain.m_UITextManager.createText("不能对马车使用");
                    TryCancelChaseToAttack();
                    return;
                }

                //skill target relationship check, return when 1. cannot operated to friend, 2. cannot operated to enemy.
                if (template.SkillTarget == 1 && template.CRRejectU == 1 && m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>().IsEnemy)
                {
                    ClientMain.m_UITextManager.createText("不能对敌方使用");
                    TryCancelChaseToAttack();
                    return;
                }
                if (template.SkillTarget == 1 && template.CRRejectU == 2 && (m_TargetId < 0 || (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>().IsEnemy)))
                {
                    ClientMain.m_UITextManager.createText("不能对友方使用");
                    TryCancelChaseToAttack();
                    return;
                }

                //Make navigate move.
                if (template.SkillTarget == 1 && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position) > template.Range_Max)
                {
                    SkillIndexAfterNavi = index;
                    m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = OnSkillAfterNavi;
                    m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position, template.Range_Max);
                    return;
                }

                //Rotate to target.
                if (template.SkillTarget == 1)
                {
                    m_RootManager.m_SelfPlayerController.transform.forward = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position - m_RootManager.m_SelfPlayerController.transform.position;
                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y, 0);
                }

                //skill distance check, return when skill operated to others beyond distance.
                if (template.SkillTarget == 1)
                {
                    var distance = Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position);

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
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FIGHT_ATTACK_REQ, ref t_protof);
            }
        }

        public void OnReportClick()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TONGZHI), DoOpenReportWindow);
        }

        public void OnNavigateToMyCarriageClick()
        {
            if (RootManager.Instance.m_SelfPlayerController != null && RootManager.Instance.m_SelfPlayerCultureController != null)
            {
                var temp = m_TotalCarriageListController.m_StoredCarriageControllerList.Where(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name).ToList();

                if (temp.Any() && RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(temp.First().UID))
                {
                    //Cancel chase.
                    RootManager.Instance.m_CarriageMain.TryCancelChaseToAttack();

                    RootManager.Instance.m_SelfPlayerController.StartNavigation(RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic[temp.First().UID].transform.position);

                    //m_TotalCarriageListController.OnCloseWindowClick();
                    //RootManager.Instance.m_CarriageMain.OnCloseBigMap();
                }
            }
        }

        public void OnReceiveAdditionalStartTimesClick()
        {
            BuyCountsReq temp = new BuyCountsReq()
            {
                type = 30
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.C_YABIAO_BUY_RSQ);
        }

        public void OnRecordClick()
        {
            BiaoJuRecordData.Instance.BiaoJuRecordReq(BiaoJuRecordData.RecordType.HISTORY);
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
                ClientMain.m_UITextManager.createText("请走到敌人或敌马附近");
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
            if (IsChaseAttack && (m_RootManager.m_SelfPlayerController.m_RealJoystickOffset != Vector3.zero || m_TargetId != chaseTargetID || !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) || m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null || m_RootManager.m_CarriageSafeArea.m_SafeAreaList.Any(item => Vector2.Distance(item.AreaPos, new Vector2(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position.x, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position.z)) < item.AreaRadius)))
            {
                IsChaseAttack = false;
            }
        }

        public void UpdateChaseBTNColor()
        {
            if (!IsChaseAttack && m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null && !m_RootManager.m_CarriageSafeArea.m_SafeAreaList.Any(item => Vector2.Distance(item.AreaPos, new Vector2(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position.x, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position.z)) < item.AreaRadius))
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

        #region Buy Rebirth Full Time

        public void OnBuyRebirthFullTimesClick()
        {
            BuyAllLifeReviveTimesReq temp = new BuyAllLifeReviveTimesReq()
            {
                code = 0
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, temp);
            byte[] t_protof;
            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_FULL_REBIRTH_TIME_REQ, ref t_protof);
        }

        public void DoBuyRebirthFullTime()
        {
            BuyAllLifeReviveTimesReq temp = new BuyAllLifeReviveTimesReq()
            {
                code = 1
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, temp);
            byte[] t_protof;
            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_FULL_REBIRTH_TIME_REQ, ref t_protof);
        }

        #endregion

        #region Buy Blood Time

        public void OnBuyBloodTimesClick()
        {
            BuyXuePingReq temp = new BuyXuePingReq()
            {
                code = 2
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, temp);
            byte[] t_protof;
            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_XUEPING_REQ, ref t_protof);
        }

        public void DoBuyBloodTimes()
        {
            BuyXuePingReq temp = new BuyXuePingReq()
            {
                code = 1
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, temp);
            byte[] t_protof;
            t_protof = tempStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_XUEPING_REQ, ref t_protof);
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
            if (m_RootManager.m_CarriageItemSyncManager.m_PlayerDic == null || m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Count == 0)
            {
                return null;
            }

            //In distance
            var allItemList = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position) < SelectDistance).ToList();
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

            //Select carriage.
            var carriageList = allItemList.Where(item => item.Value.IsCarriage).ToList();
            //Select player.
            var playerList = allItemList.Where(item => !item.Value.IsCarriage).ToList();

            //Order.
            carriageList = carriageList.OrderBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).ToList();
            playerList = playerList.OrderBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).ToList();

            allItemList = carriageList.Concat(playerList).ToList();

            return allItemList.Select(item => item.Key).ToList();
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
            if (m_TargetId > 0 && (!m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) || Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position) > SelectDistance))
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
            m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ToList().ForEach(item => item.Value.GetComponent<CarriageBaseCultureController>().OnDeSelected());

            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();
            if (temp != null)
            {
                //active selected.
                temp.OnSelected();

                //set target ui info.
                if (temp.RoleID >= 50000)
                {
                    TargetIconSetter.SetHorse(temp.HorseLevel, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip);
                }
                else
                {
                    TargetIconSetter.SetPlayer(temp.RoleID, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue);
                }
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
            if (m_TargetId > 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Keys.Contains(m_TargetId))
            {
                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();
                if (temp != null)
                {
                    temp.OnDeSelected();
                }
            }

            m_TargetId = -1;
        }

        #endregion

        #region Skill/Buff Executer

        public void ExecuteSkill(FightAttackResp tempInfo)
        {
            switch (tempInfo.skillId)
            {
                //normal attack
                case 101:
                //sp attack
                case 111:
                    {
                        if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            //mine skill
                            if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                            {
                                //turn rotation
                                if (m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(tempInfo.targetUid))
                                {
                                    m_RootManager.m_SelfPlayerController.transform.forward = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.targetUid].transform.position - m_RootManager.m_SelfPlayerController.transform.position;
                                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y, 0);
                                }

                                m_RootManager.m_SelfPlayerController.DeactiveMove();

                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);
                            }
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackUid).ToList();
                            if (temp != null && temp.Any())
                            {
                                if (!temp.First().Value.IsCarriage)
                                {
                                    //other player skill, carriage not included.
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        //turn rotation.
                                        if (m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(tempInfo.targetUid))
                                        {
                                            m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.forward = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.targetUid].transform.position - m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.position;
                                            m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.localEulerAngles = new Vector3(0, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.localEulerAngles.y, 0);
                                        }
                                        else if (tempInfo.targetUid == PlayerSceneSyncManager.Instance.m_MyselfUid && m_RootManager.m_SelfPlayerController != null)
                                        {
                                            m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.forward = m_RootManager.m_SelfPlayerController.transform.position - m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.position;
                                            m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.localEulerAngles = new Vector3(0, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.attackUid].transform.localEulerAngles.y, 0);
                                        }

                                        temp.First().Value.DeactiveMove();

                                        FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                                    }
                                }
                                else
                                {
                                    //carriage skill.
                                }
                            }
                        }

                        if (tempInfo.targetUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            //mine been attack
                            if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.targetUid, RTActionTemplate.GetTemplateByID(tempInfo.skillId).CeOnHit))
                            {
                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(tempInfo.skillId).CsOnHit), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);

                                EffectTool.Instance.SetHittedEffect(m_RootManager.m_SelfPlayerController.gameObject);
                            }

                            m_RootManager.m_SelfPlayerCultureController.OnDamage(tempInfo.damage, tempInfo.remainLife, tempInfo.skillId == 111);
                            SelfIconSetter.UpdateBar(tempInfo.remainLife);
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.targetUid).ToList();
                            if (temp.Any())
                            {
                                if (!temp.First().Value.IsCarriage)
                                {
                                    //other player been attack, carriage not included.
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.targetUid, RTActionTemplate.GetTemplateByID(tempInfo.skillId).CeOnHit))
                                    {
                                        FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(tempInfo.skillId).CsOnHit), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);

                                        EffectTool.Instance.SetHittedEffect(temp.First().Value.gameObject);
                                    }
                                }
                                else
                                {
                                    //carriage been attack, use this to play sound.
                                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(67), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);

                                    //carriage been attack effect.
                                    EffectTool.Instance.SetHittedEffect(temp.First().Value.gameObject);
                                }

                                var temp2 = temp.First().Value.GetComponent<CarriageBaseCultureController>();
                                if (temp2 != null)
                                {
                                    temp2.OnDamage(tempInfo.damage, tempInfo.remainLife, tempInfo.skillId == 111);
                                    if (m_TargetId == tempInfo.targetUid)
                                    {
                                        TargetIconSetter.UpdateBar(tempInfo.remainLife);
                                    }

                                    //Play map effect.
                                    if (temp.First().Value.IsCarriage)
                                    {
                                        if ((!string.IsNullOrEmpty(temp2.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp2.AllianceName == AllianceData.Instance.g_UnionInfo.name)) || (temp2.KingName == JunZhuData.Instance().m_junzhuInfo.name))
                                        {
                                            m_MapController.ShowBeenAttackEffect(tempInfo.targetUid);
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                //Recover skill
                case 121:
                    {
                        if (tempInfo.attackUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            //mine skill
                            if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                            {
                                m_RootManager.m_SelfPlayerController.DeactiveMove();

                                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);
                            }

                            //Update blood num.
                            SetRemainingBloodNum(m_RemainBloodNum - 1);
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackUid).ToList();
                            if (temp != null && temp.Count() > 0)
                            {
                                if (!temp.First().Value.IsCarriage)
                                {
                                    //other player skill, carriage not included.
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        temp.First().Value.DeactiveMove();

                                        FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                                    }
                                }
                            }
                        }

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
                            if (!m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(tempInfo.targetId))
                            {
                                return;
                            }

                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.targetId].GetComponent<CarriageBaseCultureController>();
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
            }
        }

        public void ExecuteRecover(SafeAreaBloodReturn tempInfo)
        {
            //mine
            if (tempInfo.uid == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null)
                {
                    return;
                }

                //Change blood bar.
                m_RootManager.m_SelfPlayerCultureController.OnRecover(tempInfo.returnValue, tempInfo.remainLife);
                SelfIconSetter.UpdateBar(tempInfo.remainLife);
            }
            //other player
            else
            {
                if (!m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(tempInfo.uid))
                {
                    return;
                }

                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[tempInfo.uid].GetComponent<CarriageBaseCultureController>();
                if (temp != null)
                {
                    //Change blood bar.
                    temp.OnRecover(tempInfo.returnValue, tempInfo.remainLife);
                    if (m_TargetId == tempInfo.uid)
                    {
                        TargetIconSetter.UpdateBar(tempInfo.remainLife);
                    }
                }
            }
        }

        #endregion

        #region Dead and Rebrith

        public GameObject DeadWindow;
        public UILabel FailInfoLabel;
        public UILabel FullRebirthTimesLabel;
        public UILabel QuickRebirthLabel;
        public UILabel SlowRebirthLabel;

        public UIButton QuickRebirthButton;
        public GameObject BuyFullRebirthTimeObject;

        public int SlowRebirthTime;

        public void ShowDeadWindow(int killerUID, int remainFullRebirthTimes, int slowRebirthTime, int quickRebirthCost)
        {
            //Set this.
            string killerName = "";
            if (m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(killerUID))
            {
                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[killerUID].GetComponent<CarriageBaseCultureController>();

                if (temp != null)
                {
                    killerName = temp.KingName;
                }
            }
            else if (m_RootManager.m_CarriageItemSyncManager.m_DeadPlayerDic.ContainsKey(killerUID))
            {
                killerName = m_RootManager.m_CarriageItemSyncManager.m_DeadPlayerDic[killerUID].m_KingName;
            }

            FailInfoLabel.text = "您已被" + ColorTool.Color_Red_c40000 + (string.IsNullOrEmpty(killerName) ? "" : killerName) + "[-]" + "击溃";
            FullRebirthTimesLabel.text = "今日剩余满血复活次数" + ColorTool.Color_Red_c40000 + remainFullRebirthTimes + "[-]";
            SlowRebirthLabel.text = ColorTool.Color_Red_c40000 + slowRebirthTime + "[-]" + "秒后自动安全复活";
            QuickRebirthLabel.text = "消耗" + ColorTool.Color_Blue_016bc5 + quickRebirthCost + "[-]" + "元宝立刻满血原地复活";

            QuickRebirthButton.isEnabled = remainFullRebirthTimes > 0;
            //QuickRebirthButton.UpdateColor(QuickRebirthButton.isEnabled, true);
            BuyFullRebirthTimeObject.SetActive(remainFullRebirthTimes <= 0);

            DeadWindow.SetActive(true);

            //Set rebirth time calc.
            SlowRebirthTime = slowRebirthTime;
            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
            }
            TimeHelper.Instance.AddEveryDelegateToTimeCalc("CarriageRebirth", slowRebirthTime, SetRebirthTime);

            //vague
            m_MainUIVagueEffect.enabled = true;
            m_Top2UIVagueEffect.enabled = true;
            m_Joystick.m_Box.enabled = false;
        }

        public void HideDeadWindows()
        {
            DeadWindow.SetActive(false);

            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
            }

            //vague
            m_MainUIVagueEffect.enabled = false;
            m_Top2UIVagueEffect.enabled = false;
            m_Joystick.m_Box.enabled = true;
        }

        private void SetRebirthTime(int time)
        {
            if (SlowRebirthLabel.gameObject.activeInHierarchy)
            {
                SlowRebirthLabel.text = ColorTool.Color_Red_c40000 + (SlowRebirthTime - time) + "[-]" + "秒后自动安全复活";
            }

            if (SlowRebirthTime - time <= 0)
            {
                OnSlowRebirthClick();

                if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
                }
            }
        }

        public void OnSlowRebirthClick()
        {
            PlayerReviveRequest tempInfo = new PlayerReviveRequest()
            {
                type = 0
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, tempInfo);
            byte[] t_protof;
            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.PLAYER_REVIVE_REQUEST, ref t_protof);
        }

        public void OnQuickRebirthClick()
        {
            PlayerReviveRequest tempInfo = new PlayerReviveRequest()
            {
                type = 1
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, tempInfo);
            byte[] t_protof;
            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.PLAYER_REVIVE_REQUEST, ref t_protof);
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

        #endregion

        #region Safe Area

        public UILabel m_SafeAresRecoveringInfoLabel;
        public List<GameObject> m_SafeAresRecoveringInfoLabelList;

        public bool m_IsInSafeArea
        {
            get { return m_isInSafeArea; }
            set
            {
                if (m_isInSafeArea != value)
                {
                    ClientMain.m_UITextManager.createText(value ? "您已进入安全区,禁止互相攻击。" : "离开安全区,当心敌人。");

                    if (value && m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null && m_RootManager.m_SelfPlayerCultureController.RemainingBlood < m_RootManager.m_SelfPlayerCultureController.TotalBlood)
                    {
                        m_SafeAresRecoveringInfoLabel.gameObject.SetActive(true);
                        LoadingTween.StartLoadingTween(m_SafeAresRecoveringInfoLabel.gameObject, m_SafeAresRecoveringInfoLabelList, -1, -1, 0.1f, 0.5f, null);

                        SetRecoveringIcon(true);
                    }
                    else
                    {
                        m_SafeAresRecoveringInfoLabel.gameObject.SetActive(false);
                        LoadingTween.StopLoadingTween(m_SafeAresRecoveringInfoLabel.gameObject);

                        SetRecoveringIcon(false);
                    }
                }

                m_isInSafeArea = value;
            }
        }

        private bool m_isInSafeArea;

        public void SetSafeAreaFlag(Vector2 p_position)
        {
            //Set safe area flag.
            bool temp = false;
            for (int i = 0; i < m_RootManager.m_CarriageSafeArea.m_SafeAreaList.Count; i++)
            {
                if (Vector2.Distance(m_RootManager.m_CarriageSafeArea.m_SafeAreaList[i].AreaPos, p_position) < m_RootManager.m_CarriageSafeArea.m_SafeAreaList[i].AreaRadius)
                {
                    temp = true;
                    break;
                }
            }

            m_IsInSafeArea = temp;
        }

        #endregion

        #region Alliance Tech Icon

        public GameObject m_SafeAresRecoveringParent;
        public int m_SafeAresRecoveringLevel = -1;

        private void SetRecoveringIcon(bool isShow)
        {
            if (isShow)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), SetRecoveringIcon);
            }
            else
            {
                while (m_SafeAresRecoveringParent.transform.childCount > 0)
                {
                    var child = m_SafeAresRecoveringParent.transform.GetChild(0);
                    Destroy(child.gameObject);
                    child.parent = null;
                }
            }
        }

        private void SetRecoveringIcon(ref WWW p_www, string p_path, UnityEngine.Object p_object)
        {
            while (m_SafeAresRecoveringParent.transform.childCount > 0)
            {
                var child = m_SafeAresRecoveringParent.transform.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }

            if (!AllianceData.Instance.IsAllianceNotExist && m_SafeAresRecoveringLevel > 0)
            {
                var temp = Instantiate(p_object) as GameObject;
                var manager = temp.GetComponent<IconSampleManager>();
                TransformHelper.ActiveWithStandardize(m_SafeAresRecoveringParent.transform, temp.transform);

                var id = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level(204, m_SafeAresRecoveringLevel).id;
                manager.SetIconByID(id, "", 10);
                manager.SetIconPopText(id);
            }
        }

        public GameObject CarriageBuffParent;
        public int m_CarriageBuffLevel = -1;

        private void SetCarriageBuffIcon(bool isShow)
        {
            if (isShow)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), SetCarriageBuffIcon);
            }
            else
            {
                while (CarriageBuffParent.transform.childCount > 0)
                {
                    var child = CarriageBuffParent.transform.GetChild(0);
                    Destroy(child.gameObject);
                    child.parent = null;
                }
            }
        }

        private void SetCarriageBuffIcon(ref WWW p_www, string p_path, UnityEngine.Object p_object)
        {
            while (CarriageBuffParent.transform.childCount > 0)
            {
                var child = CarriageBuffParent.transform.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }

            if (!AllianceData.Instance.IsAllianceNotExist && m_CarriageBuffLevel > 0)
            {
                var temp = Instantiate(p_object) as GameObject;
                var manager = temp.GetComponent<IconSampleManager>();
                TransformHelper.ActiveWithStandardize(CarriageBuffParent.transform, temp.transform);

                var id = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level(205, m_CarriageBuffLevel).id;
                manager.SetIconByID(id, "", 10);
                manager.SetIconPopText(id);
            }
        }
        #endregion

        #region Whip, Aid CallBack/Clamp Buttons

        public Transform LeftTopTransform;
        public Transform RightBottomTransform;

        public GameObject ClampedObject;
        public GameObject WhipObject;
        public GameObject AidObject;

        private bool isCanWhip
        {
            get { return isCanWhipValue; }
            set
            {
                if (value != isCanWhipValue)
                {
                    if (UI3DEffectTool.HaveAnyFx(WhipObject))
                    {
                        UI3DEffectTool.ClearUIFx(WhipObject);
                    }

                    if (value)
                    {
                        UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, WhipObject, EffectTemplate.getEffectTemplateByEffectId(100009).path);

                        WhipObject.GetComponent<UISprite>().color = Color.white;
                        WhipObject.GetComponent<UIButton>().enabled = true;
                    }
                    else
                    {
                        WhipObject.GetComponent<UIButton>().enabled = false;
                        WhipObject.GetComponent<UISprite>().color = Color.grey;
                    }
                }

                isCanWhipValue = value;
            }
        }
        private bool isCanWhipValue;

        private bool isCanAid
        {
            get { return isCanAidValue; }
            set
            {
                if (value != isCanAidValue)
                {
                    if (UI3DEffectTool.HaveAnyFx(AidObject))
                    {
                        UI3DEffectTool.ClearUIFx(AidObject);
                    }

                    if (value)
                    {
                        UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, AidObject, EffectTemplate.getEffectTemplateByEffectId(100009).path);

                        AidObject.GetComponent<UISprite>().color = Color.white;
                        AidObject.GetComponent<UIButton>().enabled = true;
                    }
                    else
                    {
                        AidObject.GetComponent<UIButton>().enabled = false;
                        AidObject.GetComponent<UISprite>().color = Color.grey;
                    }
                }

                isCanAidValue = value;
            }
        }
        private bool isCanAidValue;

        public BaseSkillController WhipSkillController;

        public void UpdateWhipCD(bool isAdvancedWhip)
        {
            WhipSkillController.SelfCD = isAdvancedWhip ? MaJuTemplate.GetMaJuTemplateById(910008).value3 : MaJuTemplate.GetMaJuTemplateById(910007).value3;
        }

        public void OnWhipClick()
        {
            if (!isCanWhip)
            {
                ClientMain.m_UITextManager.createText("请走到自己或盟友的镖马附近");
                return;
            }

            var allItems = GetAllItemsWithinDistance();
            if (allItems != null)
            {
                allItems = allItems.Where(item => item.Value.IsCarriage).ToList();

                if (allItems.Any())
                {
                    var allController = allItems.Select(item => item.Value.GetComponent<CarriageBaseCultureController>()).ToList();

                    allController.Where(item => (item.KingName == JunZhuData.Instance().m_junzhuInfo.name) || (!string.IsNullOrEmpty(item.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (item.AllianceName == AllianceData.Instance.g_UnionInfo.name))).ToList().ForEach(item =>
                    {
                        JiaSuReq tempMsg = new JiaSuReq
                        {
                            ybUid = item.UID
                        };

                        MemoryStream t_tream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        t_qx.Serialize(t_tream, tempMsg);

                        byte[] t_protof;
                        t_protof = t_tream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_CARTJIASU_REQ, ref t_protof, false);
                    });
                }
            }
        }

        public void OnAidClick()
        {
            if (!isCanAid)
            {
                ClientMain.m_UITextManager.createText("请走到盟友的镖马附近");
                return;
            }

            var allItems = GetAllItemsWithinDistance();
            if (allItems != null)
            {
                allItems = allItems.Where(item => item.Value.IsCarriage).ToList();

                if (allItems.Any())
                {
                    var allController = allItems.Select(item => item.Value.GetComponent<CarriageBaseCultureController>()).ToList();

                    allController.Where(item => item.KingName != JunZhuData.Instance().m_junzhuInfo.name && !m_IHelpOtherJunzhuIdList.Contains(item.JunzhuID) && !string.IsNullOrEmpty(item.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (item.AllianceName == AllianceData.Instance.g_UnionInfo.name)).ToList().ForEach(item =>
                      {
                          AnswerYaBiaoHelpReq tempMsg = new AnswerYaBiaoHelpReq
                          {
                              ybUid = item.UID,
                              code = 10
                          };

                          MemoryStream t_tream = new MemoryStream();
                          QiXiongSerializer t_qx = new QiXiongSerializer();
                          t_qx.Serialize(t_tream, tempMsg);

                          byte[] t_protof;
                          t_protof = t_tream.ToArray();
                          SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ANSWER_YBHELP_RSQ, ref t_protof, false);
                      });
                }
            }
        }

        public void ShowClampedButtons()
        {
            ClampedObject.SetActive(true);
        }

        public void HideClampedButtons()
        {
            ClampedObject.SetActive(false);
        }

        public void ClampButtons()
        {
            ClampedObject.transform.position = new Vector3(Mathf.Clamp(ClampedObject.transform.position.x, LeftTopTransform.position.x, RightBottomTransform.position.x), Mathf.Clamp(ClampedObject.transform.position.y, RightBottomTransform.position.y, LeftTopTransform.position.y));
        }

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

                    m_BannerEffectController.ShowAlertInfo(labelText, subLabelText, data.m_ReportTemplate.m_iEvent == 114 || data.m_ReportTemplate.m_iEvent == 115 ? ("今日还剩" + ColorTool.Color_Red_c40000 + RemainingRobCarriageTimes + "[-]" + "次可获劫镖收益") : "", awardIdList, awardNumList);
                    return true;
                }
            }

            return false;
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

        #region All carriage list and My helper list

        public TotalCarriageListController m_TotalCarriageListController;
        public MyHelperListController m_MyHelperListController;

        public UISprite OpenTotalCarriageListButton;

        [HideInInspector]
        public int RecommandedScale = -1;
        [HideInInspector]
        public int RecommandedNum = -1;

        #endregion

        #region Help Window

        public GameObject HelpWindowObject;
        public ScaleEffectController m_ScaleEffectController;

        public UIScrollBar m_HelpWindowScrollBar;
        public UIScrollView m_HelpWindowScrollView;

        public UILabel HelpInfoLabel;

        public void OnOpenHelpWindowClick()
        {
            HelpWindowObject.SetActive(true);
            m_ScaleEffectController.OpenCompleteDelegate = OnOpenHelpWindowComplete;
            m_ScaleEffectController.OnOpenWindowClick();
        }

        private void OnOpenHelpWindowComplete()
        {
            SetHelpWindow();
        }

        public void OnCloseHelpWindowClick()
        {
            m_ScaleEffectController.CloseCompleteDelegate = OnCloseHelpWindowComplete;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        private void OnCloseHelpWindowComplete()
        {
            HelpWindowObject.SetActive(false);
        }

        public void SetHelpWindow()
        {
            HelpInfoLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.CARRIAGE_HELP_DESC);
            m_HelpWindowScrollView.UpdateScrollbars(true);
            m_HelpWindowScrollBar.value = 0f;
            m_HelpWindowScrollBar.ForceUpdate();
        }

        #endregion

        #region My Carriage

        public GameObject MyCarriageGameObject;
        public UISprite MyCarriageHorseLevelSprite;
        public UISprite MyCarriageQualitySprite;
        public UIProgressBar MyCarriageProgressBar;
        public UILabel MyCarriageInfoLabel;

        public void ShowMyCarriageLogo(CarriageCultureController controller)
        {
            //Show carriage buff icon if possible.
            if (!MyCarriageGameObject.activeInHierarchy)
            {
                SetCarriageBuffIcon(true);
            }

            MyCarriageGameObject.SetActive(true);

            SetMyCarriageLogo(controller);
        }

        public void HideMyCarriageLogo()
        {
            //Hide carriage buff icon if possible.
            if (MyCarriageGameObject.activeInHierarchy)
            {
                SetCarriageBuffIcon(false);
            }

            MyCarriageGameObject.SetActive(false);
        }

        public void SetMyCarriageLogo(CarriageCultureController controller)
        {
            if (controller.TotalBlood > 0 || controller.RemainingBlood <= controller.TotalBlood)
            {
                MyCarriageProgressBar.value = controller.RemainingBlood / controller.TotalBlood;
            }

            MyCarriageHorseLevelSprite.spriteName = "horseIcon" + controller.HorseLevel;
            MyCarriageQualitySprite.spriteName = "pinzhi" + HeadIconSetter.horseIconToQualityTransferDic[controller.HorseLevel];
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

        #region Additional Start Times

        public GameObject AdditionalStartGameObject;

        public bool IsShowAdditionalStartTimes
        {
            get { return m_isShowAdditionalStartTimes; }
            set
            {
                if (m_isShowAdditionalStartTimes != value)
                {
                    if (UI3DEffectTool.HaveAnyFx(AdditionalStartGameObject))
                    {
                        UI3DEffectTool.ClearUIFx(AdditionalStartGameObject);
                    }

                    if (value)
                    {
                        AdditionalStartGameObject.SetActive(true);
                        UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, AdditionalStartGameObject, EffectTemplate.getEffectTemplateByEffectId(600154).path);
                    }
                    else
                    {
                        AdditionalStartGameObject.SetActive(false);
                    }
                }
                m_isShowAdditionalStartTimes = value;
            }
        }

        private bool m_isShowAdditionalStartTimes;

        #endregion

        #region Bubble Controller

        public BubbleController m_BubbleController;

        #endregion

        #region Outter Call

        public void NavigateToCarriage(int kingID)
        {
            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Select(item => item.Value.GetComponent<CarriageBaseCultureController>()).Where(item => item.JunzhuID == kingID).ToList();

            if (temp.Any())
            {
                //Cancel chase.
                RootManager.Instance.m_CarriageMain.TryCancelChaseToAttack();

                m_RootManager.m_SelfPlayerController.StartNavigation(temp.First().transform.position);
            }
        }

        #endregion

        private float checkTime1;
        private float checkTime2;

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

                //check safe area.
                SetSafeAreaFlag(new Vector2(m_RootManager.m_SelfPlayerController.transform.localPosition.x, m_RootManager.m_SelfPlayerController.transform.localPosition.z));

                //update total carriage list
                m_TotalCarriageListController.m_StoredCarriageControllerList = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Value.IsCarriage).Select(item => item.Value.GetComponent<CarriageCultureController>()).ToList();

                if (m_TotalCarriageListController.m_StoredCarriageControllerList.Any(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name))
                {
                    //update 2 lists state
                    m_MyHelperListController.gameObject.SetActive(true);
                    m_TotalCarriageListController.gameObject.SetActive(false);

                    //update start carriage btn
                    if (UI3DEffectTool.HaveAnyFx(m_StartCarriageBTN.gameObject))
                    {
                        UI3DEffectTool.ClearUIFx(m_StartCarriageBTN.gameObject);
                    }

                    m_StartCarriageBTN.color = Color.grey;
                    m_StartCarriageBTN.GetComponent<UIButton>().enabled = false;
                    isCanStartCarriage = false;

                    //Show my carriage logo.
                    ShowMyCarriageLogo(m_TotalCarriageListController.m_StoredCarriageControllerList.Where(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name).First());
                }
                else
                {
                    //update 2 lists state
                    m_MyHelperListController.gameObject.SetActive(false);
                    m_TotalCarriageListController.gameObject.SetActive(true);

                    //Clear my helper list.
                    if (m_MyHelperListController.m_StoredXieZhuJunZhuResp == null)
                    {
                        m_MyHelperListController.m_StoredXieZhuJunZhuResp = new XieZhuJunZhuResp();
                    }
                    if (m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz == null)
                    {
                        m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz = new List<XieZhuJunZhu>();
                    }
                    m_MyHelperListController.m_StoredXieZhuJunZhuResp.xiezhuJz.Clear();

                    //update start carriage btn.
                    if (RemainingStartCarriageTimes > 0)
                    {
                        if (!UI3DEffectTool.HaveAnyFx(m_StartCarriageBTN.gameObject))
                        {
                            UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, m_StartCarriageBTN.gameObject, EffectTemplate.getEffectTemplateByEffectId(600154).path);
                        }
                    }
                    else
                    {
                        if (UI3DEffectTool.HaveAnyFx(m_StartCarriageBTN.gameObject))
                        {
                            UI3DEffectTool.ClearUIFx(m_StartCarriageBTN.gameObject);
                        }
                    }

                    m_StartCarriageBTN.color = Color.white;
                    m_StartCarriageBTN.GetComponent<UIButton>().enabled = true;
                    isCanStartCarriage = true;

                    //Hide my carriage logo.
                    HideMyCarriageLogo();
                }

                checkTime1 = Time.realtimeSinceStartup;
            }

            if (Time.realtimeSinceStartup - checkTime2 > m_BubbleController.bubbleGapTime)
            {
                if (m_TotalCarriageListController.m_StoredCarriageControllerList.Any(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name) && m_RootManager.m_SelfPlayerController != null)
                {
                    var myCarriage = m_TotalCarriageListController.m_StoredCarriageControllerList.Where(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name).First();

                    if (Vector3.Distance(myCarriage.transform.position, m_RootManager.m_SelfPlayerController.transform.position) > m_BubbleController.bubbleDistance)
                    {
                        m_BubbleController.ShowBubble();
                    }
                }

                checkTime2 = Time.realtimeSinceStartup;
            }

            //Set Whip, Aid buttons.
            var allItems = GetAllItemsWithinDistance();
            if (allItems != null)
            {
                allItems = allItems.Where(item => item.Value.IsCarriage).ToList();

                if (allItems.Any())
                {
                    var allController = allItems.Select(item => item.Value.GetComponent<CarriageBaseCultureController>()).ToList();

                    isCanWhip = allController.Any(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name) || allController.Any(item => (!string.IsNullOrEmpty(item.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (item.AllianceName == AllianceData.Instance.g_UnionInfo.name)));
                    isCanAid = allController.Any(item => item.KingName != JunZhuData.Instance().m_junzhuInfo.name && (!string.IsNullOrEmpty(item.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (item.AllianceName == AllianceData.Instance.g_UnionInfo.name) && !m_IHelpOtherJunzhuIdList.Contains(item.JunzhuID)));
                }
                else
                {
                    isCanWhip = false;
                    isCanAid = false;
                }
            }
            else
            {
                isCanWhip = false;
                isCanAid = false;
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
                    if (Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position) > template.Range_Max)
                    {
                        m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position);
                    }
                }
            }

            //var screen = m_RootManager.TrackCamera.WorldToScreenPoint(temp.transform.position);
            //var viewport = NGUICamera.ScreenToViewportPoint(screen);
            //ClampedObject.transform.localPosition = new Vector3((viewport.x - 0.5f) * ClientMain.m_TotalWidthInCoordinate, (viewport.y - 0.5f) * ClientMain.m_TotalWidthInCoordinate, 0);

            //ClampButtons();

            //Set additional rob times info.
            IsShowAdditionalStartTimes = FunctionOpenTemp.m_EnableFuncIDList.Contains(311);
        }

        void Start()
        {
            //Execute remaining data.
            ExecuteReportData();

            //Init whip/aid button color.
            isCanAid = true;
            isCanWhip = true;

            //Open spark effect for button.
            SparkleEffectItem.OpenSparkle(OpenTotalCarriageListButton.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
        }

        void Awake()
        {
            //Load configs.
            m_BubbleController.bubbleGapTime = float.Parse(YunBiaoTemplate.GetValueByKey("bubble_interval"));
            m_BubbleController.bubbleExistTime = float.Parse(YunBiaoTemplate.GetValueByKey("bubble_duration"));
            m_BubbleController.bubbleDistance = float.Parse(YunBiaoTemplate.GetValueByKey("bubble_distance"));

            m_BubbleController.bubbleStrList = new List<string>() { LanguageTemplate.GetText(1532), LanguageTemplate.GetText(1533), LanguageTemplate.GetText(1534) };

            RecommandedScale = int.Parse(YunBiaoTemplate.GetValueByKey("rec_zhanli_scale"));
            RecommandedNum = int.Parse(YunBiaoTemplate.GetValueByKey("rec_cartNum"));

            m_tpDuration = float.Parse(YunBiaoTemplate.GetValueByKey("TP_duration"));

            //Bind delegate.
            m_BannerEffectController.m_ExecuteAfterClick = ExecuteAfterEffectClick;
            m_BannerEffectController.m_ExecuteAfterEnd = ExecuteAfterEffectEnd;

            WhipSkillController.BaseSkillClickDelegate = OnWhipClick;

            m_MapController.m_ExecuteAfterOpenMap = ExecuteAfterOpenMap;
            m_MapController.m_ExecuteAfterCloseMap = ExecuteAfterCloseMap;
        }

#if DEBUG_CARRIAGE

        void OnGUI()
        {
            if (GUILayout.Button("Test TP"))
            {
                TpToPosition(new Vector2(202, 30));
            }
            if (GUILayout.Button("Test buy"))
            {
                CommonRecharge.Instance.ShowBuy(10000, 5, doBuy);
            }
            if (GUILayout.Button("Test vip"))
            {
                CommonRecharge.Instance.ShowVIP(5);
            }
            if (GUILayout.Button("Test effect"))
            {
                m_BannerEffectController.ShowAlertInfo("testing", "testing2", "", new List<int>() { 900010, 900011 }, new List<int>() { 1, 2 });
            }
        }

        void doBuy()
        {
            m_BannerEffectController.ShowAlertInfo("testing", "testing2", "", new List<int>() { 900010, 900011 }, new List<int>() { 1, 2 });
        }
#endif

    }
}
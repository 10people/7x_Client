//#define DEBUG_CARRIAGE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

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

        public SmallMapController m_SmallMapController;
        public List<GameObject> m_ItemGizmosPrefabList = new List<GameObject>();
        public Dictionary<int, Transform> m_ItemGizmosDic = new Dictionary<int, Transform>();

        private const float m_MapTransDuration = 0.5f;
        public float m_MapTransTime;
        public Transform m_MapSmallModeTransform;
        private float m_MapSmallModeA = 1.0f;
        public Transform m_MapBigModeTransform;
        private float m_MapBigModeA = 1.0f;

        public GameObject m_BigMapDeco;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_uID"></param>
        /// <param name="p_type">from 0 - 5</param>
        /// <returns></returns>
        public bool AddGizmos(int p_uID, int p_type)
        {
            if (m_ItemGizmosDic.ContainsKey(p_uID))
            {
                Debug.LogWarning("Cannot add duplicated gizmos to small map, id:" + p_uID);
                return false;
            }

            var gizmos = Instantiate(m_ItemGizmosPrefabList[p_type]);
            gizmos.name += "_" + p_uID;
            TransformHelper.ActiveWithStandardize(m_SmallMapController.transform, gizmos.transform);
            m_ItemGizmosDic.Add(p_uID, gizmos.transform);

            return true;
        }

        public bool RemoveGizmos(int p_uID)
        {
            if (!m_ItemGizmosDic.ContainsKey(p_uID))
            {
                Debug.LogWarning("Cannot remove non-existed gizmos from small map, id:" + p_uID);
                return false;
            }

            Destroy(m_ItemGizmosDic[p_uID].gameObject);
            m_ItemGizmosDic.Remove(p_uID);

            return true;
        }

        public void UpdateGizmosPosition(int p_uID, Vector3 p_position, float p_rotation)
        {
            if (!m_ItemGizmosDic.ContainsKey(p_uID))
            {
                Debug.LogWarning("Cannot update non-existed gizmos in small map, id:" + p_uID);
                return;
            }

            m_SmallMapController.SetPositionInSmallMap(m_ItemGizmosDic[p_uID], p_position, p_rotation);
        }

        #endregion

        #region Map Effect Controller

        public MapEffectController m_MapEffectController;

        public void ShowMapEffect(int p_uid)
        {
            if (m_ItemGizmosDic.ContainsKey(p_uid))
            {
                m_MapEffectController.BlinkEffect(p_uid, m_ItemGizmosDic[p_uid].localPosition);
            }
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
                    DeactiveTarget();
                    return;
                }

                //skill target check, return when 1. cannot operated to player/other player, 2. cannot operated to carriage.
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 1 && ((m_TargetId < 0) || (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage)))
                {
                    ClientMain.m_UITextManager.createText("技能不能对玩家使用");
                    return;
                }
                if (template.SkillTarget == 1 && template.ST_TypeRejectU == 2 && (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage))
                {
                    ClientMain.m_UITextManager.createText("技能不能对马车使用");
                    return;
                }

                //skill target relationship check, return when 1. cannot operated to friend, 2. cannot operated to enemy.
                if (template.SkillTarget == 1 && template.CRRejectU == 1 && m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>().IsEnemy)
                {
                    ClientMain.m_UITextManager.createText("技能不能对敌方使用");
                    return;
                }
                if (template.SkillTarget == 1 && template.CRRejectU == 2 && (m_TargetId < 0 || (m_TargetId >= 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>().IsEnemy)))
                {
                    ClientMain.m_UITextManager.createText("技能不能对友方使用");
                    return;
                }

                //Make navigate move.
                if (template.SkillTarget == 1 && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position) > 2f)
                {
                    SkillIndexAfterNavi = index;
                    m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = OnSkillAfterNavi;
                    m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position);
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

        public void OnOpenBigMap()
        {
            if (!m_SmallMapController.m_IsMapInSmallMode) return;

            m_MapTransTime = Time.realtimeSinceStartup;
            m_SmallMapController.m_IsMapInSmallMode = false;

            m_SmallMapController.MapBG.mainTexture = m_SmallMapController.BigMapTexture;
            //m_SmallMapController.MapBG.width = m_SmallMapController.MapBG.height = 200;

            var color = m_SmallMapController.GetComponent<UITexture>().color;
            m_SmallMapController.GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapBigModeA);
            m_BigMapDeco.SetActive(true);

            //vague
            m_MainUIVagueEffect.enabled = true;
            m_Joystick.m_Box.enabled = false;
        }

        public void OnCloseBigMap()
        {
            if (m_SmallMapController.m_IsMapInSmallMode) return;

            m_MapTransTime = Time.realtimeSinceStartup;
            m_SmallMapController.m_IsMapInSmallMode = true;

            m_SmallMapController.MapBG.mainTexture = m_SmallMapController.SmallMapTexture;
            //m_SmallMapController.MapBG.width = m_SmallMapController.MapBG.height = 200;

            var color = m_SmallMapController.GetComponent<UITexture>().color;
            m_SmallMapController.GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapSmallModeA);
            m_BigMapDeco.SetActive(false);

            //vague
            m_MainUIVagueEffect.enabled = false;
            m_Joystick.m_Box.enabled = true;
        }

        public void OnAidClick()
        {
            if (m_TargetId > 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage)
            {
                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();

                if (!string.IsNullOrEmpty(temp.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp.AllianceName == AllianceData.Instance.g_UnionInfo.name))
                {
                    AnswerYaBiaoHelpReq tempMsg = new AnswerYaBiaoHelpReq
                    {
                        ybUid = temp.UID,
                        code = 10
                    };

                    MemoryStream t_tream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    t_qx.Serialize(t_tream, tempMsg);

                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ANSWER_YBHELP_RSQ, ref t_protof, false);
                }
            }
        }

        public void OnAlertEffectClick()
        {
            DenableAlertEffectClick();

            HideAlertEffect();

            //Show awards.
            if (m_RewardIDs != null && m_RewardIDs.Any() && m_RewardNums != null && m_RewardNums.Any() && m_RewardIDs.Count == m_RewardNums.Count)
            {
                GeneralRewardManager.Instance().CreateReward(m_RewardIDs.Select((t, i) => new RewardData(t, m_RewardNums[i])).ToList());
            }

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

        public void OnReportClick()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TONGZHI), DoOpenReportWindow);
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

        /// <summary>
        /// Get active target list, auto deactive target if list is null.
        /// </summary>
        /// <returns></returns>
        private List<int> GetPossibleActiveTargetList()
        {
            if (m_RootManager.m_CarriageItemSyncManager.m_PlayerDic == null || m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Count == 0)
            {
                DeactiveTarget();
                return null;
            }

            //In distance
            var allItemList = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position) < SelectDistance).ToList();
            if (!allItemList.Any())
            {
                DeactiveTarget();
                return null;
            }

            //Select carriage.
            var carriageList = allItemList.Where(item => item.Value.IsCarriage).ToList();
            //Select player.
            var playerList = allItemList.Where(item => !item.Value.IsCarriage).ToList();

            //Order.
            carriageList = carriageList.OrderBy(item => item.Value.GetComponent<CarriageBaseCultureController>().RemainingBlood).ThenBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).ToList();
            playerList = playerList.OrderBy(item => item.Value.GetComponent<CarriageBaseCultureController>().RemainingBlood).ThenBy(item => Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.transform.position)).ToList();

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
                    TargetIconSetter.SetHorse(temp.HorseLevel, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue);
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
                            m_RootManager.m_SelfPlayerController.DeactiveMove();
                            if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                            {
                                FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);
                            }
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackUid).ToList();
                            if (temp != null && temp.Count() > 0)
                            {
                                if (!temp.First().Value.IsCarriage)
                                {
                                    //other player skill, carriage not included.
                                    temp.First().Value.DeactiveMove();
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
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
                                FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(tempInfo.skillId).CsOnHit), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);
                            }

                            m_RootManager.m_SelfPlayerCultureController.OnDamage(tempInfo.damage, tempInfo.remainLife);
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
                                        FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTActionTemplate.GetTemplateByID(tempInfo.skillId).CsOnHit), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                                    }
                                }
                                else
                                {
                                    //carriage been attack, use this to play sound.
                                    FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(67), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
                                }

                                var temp2 = temp.First().Value.GetComponent<CarriageBaseCultureController>();
                                if (temp2 != null)
                                {
                                    temp2.OnDamage(tempInfo.damage, tempInfo.remainLife);
                                    if (m_TargetId == tempInfo.targetUid)
                                    {
                                        TargetIconSetter.UpdateBar(tempInfo.remainLife);
                                    }

                                    //Play map effect.
                                    if (temp.First().Value.IsCarriage)
                                    {
                                        if ((!string.IsNullOrEmpty(temp2.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp2.AllianceName == AllianceData.Instance.g_UnionInfo.name)) || (temp2.KingName == JunZhuData.Instance().m_junzhuInfo.name))
                                        {
                                            ShowMapEffect(tempInfo.targetUid);
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
                            m_RootManager.m_SelfPlayerController.DeactiveMove();
                            if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                            {
                                FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject, null, Vector3.zero, m_RootManager.m_SelfPlayerController.transform.forward);
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
                                    temp.First().Value.DeactiveMove();
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject, null, Vector3.zero, temp.First().Value.transform.forward);
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

        public bool m_IsInSafeArea
        {
            get { return m_isInSafeArea; }
            set
            {
                if (m_isInSafeArea != value)
                {
                    ClientMain.m_UITextManager.createText(value ? "您已进入安全区,禁止互相攻击。" : "离开安全区,当心敌人。");
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

        #region Clamp Buttons

        public Transform LeftTopTransform;
        public Transform RightBottomTransform;

        public GameObject ClampedObject;
        public GameObject WhipObject;
        public GameObject AidObject;

        public BaseSkillController WhipSkillController;

        public void UpdateWhipCD(bool isAdvancedWhip)
        {
            WhipSkillController.SelfCD = isAdvancedWhip ? MaJuTemplate.GetMaJuTemplateById(910008).value3 : MaJuTemplate.GetMaJuTemplateById(910007).value3;
        }

        public void OnWhipClick()
        {
            if (m_TargetId > 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage)
            {
                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();

                if ((!string.IsNullOrEmpty(temp.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp.AllianceName == AllianceData.Instance.g_UnionInfo.name)) || (temp.KingName == JunZhuData.Instance().m_junzhuInfo.name))
                {
                    JiaSuReq tempMsg = new JiaSuReq
                    {
                        ybUid = temp.UID
                    };

                    MemoryStream t_tream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    t_qx.Serialize(t_tream, tempMsg);

                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_CARTJIASU_REQ, ref t_protof, false);
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

        public GameObject AlertEffectMainObject;
        public GameObject AlertEffectBGObject;
        public GameObject AlertEffectInfoObject;
        public UILabel AlertEffectLabel;
        public UILabel AlertEffectSubLabel;
        public UIGrid AlertEffectRewardGrid;

        public BoxCollider AlertEffectCollider;

        public List<int> m_RewardIDs = new List<int>();
        public List<int> m_RewardNums = new List<int>();

        private const float BGTurnDuration = 0.3f;
        private const float LabelMoveDuration = 0.4f;

        public void ShowAlertInfo(string p_mainStr, string p_subStr = "", List<int> p_rewardIDs = null, List<int> p_rewardNums = null)
        {
            AlertEffectLabel.text = p_mainStr;
            AlertEffectSubLabel.text = p_subStr;

            if (p_rewardIDs != null && p_rewardIDs.Any() && p_rewardNums != null && p_rewardNums.Any() && p_rewardIDs.Count == p_rewardNums.Count)
            {
                m_RewardIDs = p_rewardIDs;
                m_RewardNums = p_rewardNums;

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
            else
            {
                //Clear all awards.

                m_RewardIDs.Clear();
                m_RewardNums.Clear();

                while (AlertEffectRewardGrid.transform.childCount > 0)
                {
                    var child = AlertEffectRewardGrid.transform.GetChild(0);
                    child.parent = null;
                    Destroy(child.gameObject);
                }

                ShowAlertEffect();
            }
        }

        private void OnIconSampleLoadCallBack(ref WWW www, string path, UnityEngine.Object p_object)
        {
            while (AlertEffectRewardGrid.transform.childCount > 0)
            {
                var child = AlertEffectRewardGrid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }

            for (int i = 0; i < m_RewardIDs.Count; i++)
            {
                var temp = (Instantiate(p_object) as GameObject).GetComponent<IconSampleManager>();
                TransformHelper.ActiveWithStandardize(AlertEffectRewardGrid.transform, temp.transform);

                temp.SetIconByID(m_RewardIDs[i], "x" + m_RewardNums[i], 10);
            }

            float deflection = AlertEffectRewardGrid.transform.localScale.x * AlertEffectRewardGrid.cellWidth * (m_RewardIDs.Count - 1) / 2f;
            AlertEffectRewardGrid.transform.localPosition = new Vector3(-deflection, AlertEffectRewardGrid.transform.localPosition.y, 0);
            AlertEffectRewardGrid.Reposition();

            ShowAlertEffect();
        }

        public void ShowAlertEffect()
        {
            AlertEffectMainObject.SetActive(true);

            ShowAlertEffectBG();
        }

        private void ShowAlertEffectBG()
        {
            AlertEffectBGObject.transform.localScale = new Vector3(1, 0, 1);
            AlertEffectBGObject.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", BGTurnDuration, "easetype", "easeOutBack", "onupdate", "SetAlertEffectBGScale", "oncomplete", "ShowAlertEffectInfo"));
        }

        private void ShowAlertEffectInfo()
        {
            AlertEffectInfoObject.transform.localPosition = new Vector3(-ClientMain.m_TotalWidthInCoordinate, AlertEffectInfoObject.transform.localPosition.y, 0);
            AlertEffectInfoObject.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", -ClientMain.m_TotalWidthInCoordinate, "to", 0, "time", LabelMoveDuration, "easetype", "easeOutBack", "onupdate", "SetAlertEffectInfoPos", "oncomplete", "EnableAlertEffectClick"));
        }

        public void HideAlertEffect()
        {
            HideAlertEffectInfo();
        }

        private void HideAlertEffectInfo()
        {
            AlertEffectInfoObject.transform.localPosition = new Vector3(0, AlertEffectInfoObject.transform.localPosition.y, 0);
            AlertEffectInfoObject.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", ClientMain.m_TotalWidthInCoordinate, "time", LabelMoveDuration, "easetype", "easeInBack", "onupdate", "SetAlertEffectInfoPos", "oncomplete", "HideAlertEffectBG"));
        }

        private void HideAlertEffectBG()
        {
            AlertEffectInfoObject.SetActive(false);

            AlertEffectBGObject.transform.localScale = new Vector3(1, 1, 1);
            AlertEffectBGObject.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", BGTurnDuration, "easetype", "easeInBack", "onupdate", "SetAlertEffectBGScale", "oncomplete", "EndAlertEffect"));
        }

        public void EndAlertEffect()
        {
            AlertEffectBGObject.SetActive(false);
            AlertEffectMainObject.SetActive(false);

            if (m_CurrenTongzhiData != null)
            {
                ExecuteReportData();
            }
        }

        public void SetAlertEffectBGScale(float value)
        {
            AlertEffectBGObject.transform.localScale = new Vector3(1, value, 1);
        }

        public void SetAlertEffectInfoPos(float value)
        {
            AlertEffectInfoObject.transform.localPosition = new Vector3(value, AlertEffectInfoObject.transform.localPosition.y, 0);
        }

        public void EnableAlertEffectClick()
        {
            AlertEffectCollider.enabled = true;
        }

        public void DenableAlertEffectClick()
        {
            AlertEffectCollider.enabled = false;
        }

        #endregion

        #region Report Info

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

                    ShowAlertInfo(labelText, subLabelText, awardIdList, awardNumList);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region TP

        public GameObject m_TpObject;
        public UIProgressBar m_TpBar;
        public UILabel m_TpTimeLabel;

        public float m_TPDuration
        {
            get
            {
                if (m_tpDuration <= 0)
                {
                    string l_value = YunBiaoTemplate.GetValueByKey("TP_duration");
                    return m_tpDuration = !string.IsNullOrEmpty(l_value) ? float.Parse(l_value) : 3;
                }
                else
                {
                    return m_tpDuration;
                }
            }
        }

        private float m_tpDuration = -1;

        private Vector2 m_TpToPos;

        public void TPToPosition(Vector2 targetPos)
        {
            if (m_RootManager.m_SelfPlayerController != null && m_RootManager.m_SelfPlayerCultureController != null)
            {
                m_TpToPos = targetPos;

                m_TpObject.SetActive(true);
                StartSetTpBar();
            }
        }

        private void StartSetTpBar()
        {
            m_TpBar.value = 0;
            m_TpTimeLabel.text = "传送中" + 0.0 + "秒";
            //m_TpTimeLabel.text = ColorTool.Color_White_ffffff + "传送中[-]" + ColorTool.Color_Red_c40000 + 0.0 + "[-]" + ColorTool.Color_White_ffffff + "秒[-]";
            iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", m_TPDuration, "easetype", "linear", "onupdate", "UpdateTpInfo", "oncomplete", "DoTpToPos"));
        }

        private void UpdateTpInfo(float value)
        {
            m_TpBar.value = value;
            m_TpTimeLabel.text = "传送中" + float.Parse((m_TPDuration * value).ToString("0.0")) + "秒";
            //m_TpTimeLabel.text = ColorTool.Color_White_ffffff + "传送中[-]" + ColorTool.Color_Red_c40000 + float.Parse((m_TPDuration * value).ToString("0.0")) + "[-]" + ColorTool.Color_White_ffffff + "秒[-]";
        }

        private void DoTpToPos()
        {
            m_TpObject.SetActive(false);

            m_RootManager.m_SelfPlayerController.transform.localPosition = new Vector3(m_TpToPos.x, RootManager.BasicYPosition, m_TpToPos.y);
        }

        #endregion

        #region All carriage list and My helper list

        public TotalCarriageListController m_TotalCarriageListController;
        public MyHelperListController m_MyHelperListController;

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

        #region Navigation Animation

        private GameObject NavAnimObject;
        public GameObject NavAnimParent;

        public void ShowNavAnimation()
        {
            if (NavAnimObject == null)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.AUTO_NAV),
                                       NavAnimLoadCallback);
            }
        }

        private void NavAnimLoadCallback(ref WWW p_www, string p_path, Object p_object)
        {
            NavAnimObject = (GameObject)Instantiate(p_object);
            TransformHelper.ActiveWithStandardize(NavAnimParent.transform, NavAnimObject.transform);
        }

        public void StopNavAnimation()
        {
            Destroy(NavAnimObject);
        }

        #endregion

        private float checkTime;

        public Camera NGUICamera
        {
            get { return m_nguiCamera ?? (m_nguiCamera = GetComponentInChildren<Camera>()); }
        }

        private Camera m_nguiCamera;

        void Update()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) return;

            //Map trans animation
            if (Time.realtimeSinceStartup - m_MapTransTime <= m_MapTransDuration)
            {
                if (!m_SmallMapController.m_IsMapInSmallMode)
                {
                    m_SmallMapController.transform.position = Vector3.Lerp(m_MapSmallModeTransform.position, m_MapBigModeTransform.position, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                    m_SmallMapController.transform.localScale = Vector3.Lerp(m_MapSmallModeTransform.localScale, m_MapBigModeTransform.localScale, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                }
                else
                {
                    m_SmallMapController.transform.position = Vector3.Lerp(m_MapBigModeTransform.position, m_MapSmallModeTransform.position, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                    m_SmallMapController.transform.localScale = Vector3.Lerp(m_MapBigModeTransform.localScale, m_MapSmallModeTransform.localScale, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                }
            }

            //Update small map
            {
                UpdateGizmosPosition(PlayerSceneSyncManager.Instance.m_MyselfUid, m_RootManager.m_SelfPlayerController.transform.localPosition, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y);
            }

            if (Time.realtimeSinceStartup - checkTime > 1.0f)
            {
                //auto active/deactive target.
                AutoActiveTarget();
                AutoDeactiveTarget();

                //check safe area.
                SetSafeAreaFlag(new Vector2(m_RootManager.m_SelfPlayerController.transform.localPosition.x, m_RootManager.m_SelfPlayerController.transform.localPosition.z));

                //update total carriage list
                m_TotalCarriageListController.m_StoredCarriageControllerList = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Value.IsCarriage).Select(item => item.Value.GetComponent<CarriageCultureController>()).ToList();
                //update 2 lists state, update start carriage btn, show effect if necessary.
                if (m_TotalCarriageListController.m_StoredCarriageControllerList.Any(item => item.KingName == JunZhuData.Instance().m_junzhuInfo.name))
                {
                    m_MyHelperListController.gameObject.SetActive(true);
                    m_TotalCarriageListController.gameObject.SetActive(false);

                    if (UI3DEffectTool.Instance().HaveAnyFx(m_StartCarriageBTN.gameObject))
                    {
                        UI3DEffectTool.Instance().ClearUIFx(m_StartCarriageBTN.gameObject);
                    }

                    m_StartCarriageBTN.color = Color.grey;
                    m_StartCarriageBTN.GetComponent<UIButton>().enabled = false;
                    isCanStartCarriage = false;
                }
                else
                {
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

                    if (RemainingStartCarriageTimes > 0)
                    {
                        if (!UI3DEffectTool.Instance().HaveAnyFx(m_StartCarriageBTN.gameObject))
                        {
                            UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, m_StartCarriageBTN.gameObject, EffectTemplate.getEffectTemplateByEffectId(600154).path);
                        }
                    }
                    else
                    {
                        if (UI3DEffectTool.Instance().HaveAnyFx(m_StartCarriageBTN.gameObject))
                        {
                            UI3DEffectTool.Instance().ClearUIFx(m_StartCarriageBTN.gameObject);
                        }
                    }

                    m_StartCarriageBTN.color = Color.white;
                    m_StartCarriageBTN.GetComponent<UIButton>().enabled = true;
                    isCanStartCarriage = true;
                }

                checkTime = Time.realtimeSinceStartup;
            }

            //clamp buttons.
            if (m_TargetId > 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId) && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].IsCarriage)
            {
                var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();

                if (temp.KingName == JunZhuData.Instance().m_junzhuInfo.name)
                {
                    WhipObject.SetActive(true);
                    AidObject.SetActive(false);

                    if (!ClampedObject.activeInHierarchy)
                    {
                        ShowClampedButtons();

                        if (UI3DEffectTool.Instance().HaveAnyFx(WhipObject))
                        {
                            UI3DEffectTool.Instance().ClearUIFx(WhipObject);
                        }
                        UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, WhipObject, EffectTemplate.getEffectTemplateByEffectId(100009).path);
                    }
                    //var screen = m_RootManager.TrackCamera.WorldToScreenPoint(temp.transform.position);
                    //var viewport = NGUICamera.ScreenToViewportPoint(screen);
                    //ClampedObject.transform.localPosition = new Vector3((viewport.x - 0.5f) * ClientMain.m_TotalWidthInCoordinate, (viewport.y - 0.5f) * ClientMain.m_TotalWidthInCoordinate, 0);

                    //ClampButtons();
                }
                else if ((!string.IsNullOrEmpty(temp.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp.AllianceName == AllianceData.Instance.g_UnionInfo.name)))
                {
                    WhipObject.SetActive(true);

                    AidObject.SetActive(!m_IHelpOtherJunzhuIdList.Contains(temp.JunzhuID));

                    if (!ClampedObject.activeInHierarchy)
                    {
                        ShowClampedButtons();

                        if (UI3DEffectTool.Instance().HaveAnyFx(WhipObject))
                        {
                            UI3DEffectTool.Instance().ClearUIFx(WhipObject);
                        }
                        UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, WhipObject, EffectTemplate.getEffectTemplateByEffectId(100009).path);

                        if (AidObject.activeInHierarchy)
                        {
                            if (UI3DEffectTool.Instance().HaveAnyFx(AidObject))
                            {
                                UI3DEffectTool.Instance().ClearUIFx(AidObject);
                            }
                            UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, AidObject, EffectTemplate.getEffectTemplateByEffectId(100009).path);
                        }
                    }
                    //var screen = m_RootManager.TrackCamera.WorldToScreenPoint(temp.transform.position);
                    //var viewport = NGUICamera.ScreenToViewportPoint(screen);
                    //ClampedObject.transform.localPosition = new Vector3((viewport.x - 0.5f) * ClientMain.m_TotalWidthInCoordinate, (viewport.y - 0.5f) * ClientMain.m_TotalWidthInCoordinate, 0);

                    //ClampButtons();
                }
                else
                {
                    WhipObject.SetActive(false);
                    AidObject.SetActive(false);

                    if (ClampedObject.activeInHierarchy)
                    {
                        HideClampedButtons();

                        if (UI3DEffectTool.Instance().HaveAnyFx(WhipObject))
                        {
                            UI3DEffectTool.Instance().ClearUIFx(WhipObject);
                        }

                        if (UI3DEffectTool.Instance().HaveAnyFx(AidObject))
                        {
                            UI3DEffectTool.Instance().ClearUIFx(AidObject);
                        }
                    }
                }
            }
            else
            {
                WhipObject.SetActive(false);
                AidObject.SetActive(false);

                HideClampedButtons();
            }
        }

        void Start()
        {
            AlertEffectBGObject.GetComponent<UISprite>().width = (int)(ClientMain.m_TotalWidthInCoordinate);

            //Execute remaining data.
            ExecuteReportData();

            WhipSkillController.BaseSkillClickDelegate = OnWhipClick;
        }

#if DEBUG_CARRIAGE

        void OnGUI()
        {
            if (GUILayout.Button("Test TP"))
            {
                TPToPosition(new Vector2(202, 30));
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
                ShowAlertInfo("testing", "testing2", new List<int>() { 900010, 900011 }, new List<int>() { 1, 2 });
            }
        }

        void doBuy()
        {
            ShowAlertInfo("testing", "testing2", new List<int>() { 900010, 900011 }, new List<int>() { 1, 2 });
        }
#endif

    }
}
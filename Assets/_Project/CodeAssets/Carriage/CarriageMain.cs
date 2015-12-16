using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageMain : MonoBehaviour
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

        #region Map Controller

        public SmallMapController m_SmallMapController;
        public List<GameObject> m_ItemGizmosPrefabList = new List<GameObject>();
        public Dictionary<int, Transform> m_ItemGizmosDic = new Dictionary<int, Transform>();

        public bool m_IsMapInSmallMode = true;
        private const float m_MapTransDuration = 0.5f;
        public float m_MapTransTime;
        public Transform m_MapSmallModeTransform;
        public float m_MapSmallModeA = 1.0f;
        public Transform m_MapBigModeTransform;
        public float m_MapBigModeA = 0.5f;

        public GameObject m_CloseBigMapButton;

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

        #region UI Click Event

        public void OnReturnClick()
        {
            CityGlobalData.m_isJieBiaoScene = false;
            PlayerSceneSyncManager.Instance.ExitCarriage();
        }

        public void OnStartCarriageClick()
        {
            BiaoJuData.Instance.OpenBiaoJu();
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
                if (tempController.IsInCD) return;

                //Check target update.
                if (template.SkillTarget != 0 && !m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId))
                {
                    DeactiveTarget();
                    return;
                }

                //Make navigate move.
                if (template.SkillTarget != 0 && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position) > 2f)
                {
                    SkillIndexAfterNavi = index;
                    m_RootManager.m_SelfPlayerController.m_CompleteNavDelegate = OnSkillAfterNavi;
                    m_RootManager.m_SelfPlayerController.StartNavigation(m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position);
                    return;
                }

                //Rotate to target.
                if (template.SkillTarget != 0)
                {
                    m_RootManager.m_SelfPlayerController.transform.forward = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].transform.position - m_RootManager.m_SelfPlayerController.transform.position;
                    m_RootManager.m_SelfPlayerController.transform.localEulerAngles = new Vector3(0, m_RootManager.m_SelfPlayerController.transform.localEulerAngles.y, 0);
                }

                //TODO: set distance check per skill.

                ////skill target check, return when no target.
                //if (template.SkillTarget == 1 && m_TargetId < 0) return;

                ////skill target check, return when 1. cannot operated to players/others 2. cannot operated to carriage
                //if (m_TargetId < 0 || (template.EffectTarget == 1 && m_RootManager.m_CarriageItemManager.m_PlayerDic[m_TargetId].m_RoleID < 50000) || (template.EffectTarget == 2 && m_RootManager.m_CarriageItemManager.m_PlayerDic[m_TargetId].m_RoleID >= 50000)) return;

                ////skill target check, return when cannot operated to player/others.
                //if (template.ST_TypeRejectU == 1) return;

                ////skill distance check, return when skill operated to others cannot reach distance.
                //if (template.SkillTarget == 1)
                //{
                //    var distance = Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, m_RootManager.m_CarriageItemManager.m_PlayerDic[m_TargetId].transform.position);

                //    if (distance > template.Range_Max || distance < template.Range_Min) return;
                //}

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
            if (!m_IsMapInSmallMode) return;

            m_MapTransTime = Time.realtimeSinceStartup;
            m_IsMapInSmallMode = false;

            var color = m_SmallMapController.GetComponent<UITexture>().color;
            m_SmallMapController.GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapBigModeA);
            m_CloseBigMapButton.SetActive(true);
        }

        public void OnCloseBigMap()
        {
            if (m_IsMapInSmallMode) return;

            m_MapTransTime = Time.realtimeSinceStartup;
            m_IsMapInSmallMode = true;

            var color = m_SmallMapController.GetComponent<UITexture>().color;
            m_SmallMapController.GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapSmallModeA);
            m_CloseBigMapButton.SetActive(false);
        }

        public void OnWhipClick()
        {

        }

        public void OnProtectClick()
        {

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
            var carriageList = allItemList.Where(item => item.Value.m_RoleID >= 50000).ToList();
            //Select player.
            var playerList = allItemList.Where(item => item.Value.m_RoleID < 50000).ToList();

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
                TargetIconSetter.SetThis(temp.RoleID, true, temp.Level, temp.KingName, temp.AllianceName, temp.TotalBlood, temp.RemainingBlood, temp.NationID, temp.Vip, temp.BattleValue);
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
                                FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject);
                            }
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackUid).ToList();
                            if (temp != null && temp.Count() > 0)
                            {
                                if (temp.First().Value.m_RoleID < 50000)
                                {
                                    //other player skill, carriage not included.
                                    temp.First().Value.DeactiveMove();
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject);
                                    }
                                }
                            }
                        }

                        if (tempInfo.targetUid == PlayerSceneSyncManager.Instance.m_MyselfUid)
                        {
                            //mine been attack
                            m_RootManager.TryPlayAnimationInAnimator(tempInfo.targetUid, "BATC");

                            m_RootManager.m_SelfPlayerCultureController.OnDamage(tempInfo.damage, tempInfo.remainLife);
                            SelfIconSetter.UpdateBar(tempInfo.remainLife);
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.targetUid).ToList();
                            if (temp.Any())
                            {
                                //other player been attack, carriage not included.
                                if (temp.First().Value.m_RoleID < 50000)
                                {
                                    m_RootManager.TryPlayAnimationInAnimator(tempInfo.targetUid, "BATC");
                                }

                                var temp2 = temp.First().Value.GetComponent<CarriageBaseCultureController>();
                                if (temp2 != null)
                                {
                                    temp2.OnDamage(tempInfo.damage, tempInfo.remainLife);
                                    if (m_TargetId == tempInfo.targetUid)
                                    {
                                        TargetIconSetter.UpdateBar(tempInfo.remainLife);
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
                                FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), m_RootManager.m_SelfPlayerController.gameObject);
                            }

                            //Update blood num.
                            SetRemainingBloodNum(m_RemainBloodNum - 1);
                        }
                        else
                        {
                            var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackUid).ToList();
                            if (temp != null && temp.Count() > 0)
                            {
                                if (temp.First().Value.m_RoleID < 50000)
                                {
                                    //other player skill, carriage not included.
                                    temp.First().Value.DeactiveMove();
                                    if (m_RootManager.TryPlayAnimationInAnimator(tempInfo.attackUid, RTSkillTemplate.GetTemplateByID(tempInfo.skillId).CsOnShot))
                                    {
                                        FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTSkillTemplate.GetTemplateByID(tempInfo.skillId).EsOnShot), temp.First().Value.gameObject);
                                    }
                                }
                            }
                        }

                        break;
                    }
            }

            //Set cds.
            m_SkillControllers.ForEach(item => item.TryStartSharedCD());
            var triggeredSkill = m_SkillControllers.Where(item => item.m_Index == tempInfo.skillId).ToList();
            if (triggeredSkill.Any())
            {
                triggeredSkill.First().TryStartSelfCD();
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

        #endregion

        #region Dead and Rebrith

        public GameObject DeadWindow;
        public UILabel FailInfoLabel;
        public UILabel QuickRebirthTimesLabel;
        public UILabel QuickRebirthLabel;
        public UILabel SlowRebirthLabel;

        public int SlowRebirthTime;

        public void ShowDeadWindow(int killerUID, int remainQuickRebirthTimes, int slowRebirthTime, int quickRebirthCost)
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
            QuickRebirthTimesLabel.text = "今日剩余满血复活次数" + ColorTool.Color_Red_c40000 + remainQuickRebirthTimes + "[-]";
            SlowRebirthLabel.text = ColorTool.Color_Red_c40000 + slowRebirthTime + "[-]" + "秒后自动安全复活";
            QuickRebirthLabel.text = "消耗" + ColorTool.Color_Blue_016bc5 + quickRebirthCost + "[-]" + "元宝立刻满血原地复活";

            DeadWindow.SetActive(true);

            //Set rebirth time calc.
            SlowRebirthTime = slowRebirthTime;
            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
            }
            TimeHelper.Instance.AddEveryDelegateToTimeCalc("CarriageRebirth", slowRebirthTime, SetRebirthTime);
        }

        public void HideDeadWindows()
        {
            DeadWindow.SetActive(false);
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

            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
            }
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

            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageRebirth"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageRebirth");
            }
        }

        #endregion

        #region Remaining Blood

        public UILabel m_RemainingBloodNumLabel;
        public int m_RemainBloodNum;

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
                }
                else
                {
                    temp.First().m_SkillButton.isEnabled = true;
                    temp.First().m_SkillSprite.color = Color.white;
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
                    ClientMain.m_UITextManager.createText(value ? "您已进入安全区,在这个区域内不能攻击其他玩家。" : "您已离开安全区,请注意安全,小心敌人的攻击。");
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
        public GameObject ProtectObject;

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
            ClampedObject.transform.position = new Vector3(Mathf.Clamp(ClampedObject.transform.position.x, LeftTopTransform.position.x, RightBottomTransform.position.x), Mathf.Clamp(ClampedObject.transform.position.y, LeftTopTransform.position.y, RightBottomTransform.position.y));
        }

        #endregion

        private float checkTime;

        public Camera NGUICamera
        {
            get { return m_nguiCamera ?? (m_nguiCamera = GetComponentInParent<Camera>()); }
        }

        private Camera m_nguiCamera;

        void Update()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) return;

            //Map trans animation
            if (Time.realtimeSinceStartup - m_MapTransTime <= m_MapTransDuration)
            {
                if (!m_IsMapInSmallMode)
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

            //Check active target with clamped buttons, check safe area.
            if (Time.realtimeSinceStartup - checkTime > 1.0f)
            {
                //auto active/deactive target.
                AutoActiveTarget();
                AutoDeactiveTarget();

                //clamp buttons.
                if (m_TargetId > 0 && m_RootManager.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_TargetId))
                {
                    var temp = m_RootManager.m_CarriageItemSyncManager.m_PlayerDic[m_TargetId].GetComponent<CarriageBaseCultureController>();

                    if ((!string.IsNullOrEmpty(temp.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (temp.AllianceName == AllianceData.Instance.g_UnionInfo.name)))
                    {
                        WhipObject.SetActive(true);
                        ProtectObject.SetActive(true);
                    }
                    if (temp.KingName == JunZhuData.Instance().m_junzhuInfo.name)
                    {
                        WhipObject.SetActive(true);
                        ProtectObject.SetActive(false);
                    }

                    ClampedObject.transform.position = NGUICamera.ScreenToViewportPoint(m_RootManager.TrackCamera.WorldToScreenPoint(temp.transform.position));

                    ShowClampedButtons();
                    ClampButtons();
                }
                else
                {
                    HideClampedButtons();
                }

                //check safe area.
                SetSafeAreaFlag(new Vector2(m_RootManager.m_SelfPlayerController.transform.localPosition.x, m_RootManager.m_SelfPlayerController.transform.localPosition.z));

                checkTime = Time.realtimeSinceStartup;
            }
        }
    }
}
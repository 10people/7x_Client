using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace AllianceBattle
{
    public class AllianceBattleUI : MonoBehaviour, SocketListener
    {
        [HideInInspector]
        public float AttackDistance;
        [HideInInspector]
        public float AttackDegree;

        public RootManager m_RootManager;

        #region LeftTop UI

        public UISprite PlayerSprite;
        public UISprite NationSprite;
        public UILabel LevelLabel;
        public UILabel KingNameLabel;
        public UILabel AllianceNameLabel;

        public void SetLeftTopUI()
        {
            PlayerSprite.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
            KingNameLabel.text = MyColorData.getColorString(9, "[b]" + JunZhuData.Instance().m_junzhuInfo.name + "[/b]");
            LevelLabel.text = JunZhuData.Instance().m_junzhuInfo.level.ToString();
            NationSprite.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();

            if (!AllianceData.Instance.IsAllianceNotExist)
            {
                //try set alliance name
                if (AllianceData.Instance.g_UnionInfo != null && AllianceData.Instance.g_UnionInfo.name != null)
                {
                    AllianceNameLabel.text = "<" + AllianceData.Instance.g_UnionInfo.name + ">";
                }
            }
            else
            {
                Debug.LogError("Cannot enter alliance battle if no alliance.");
                AllianceNameLabel.text = "无联盟";
            }

        }

        #endregion

        #region Top UI

        public int RemainTime = -1;
        public string AllianceName1;
        public string AllianceName2;
        public int TotalScore = -1;
        public int Score1 = -1;
        public int Score2 = -1;
        public int HoldPoint1 = -1;
        public int HoldPoint2 = -1;
        public bool IsBlueMine;

        public UILabel RemainTimeLabel;
        public UILabel AllianceName1Label;
        public UILabel AllianceName2Label;
        public UILabel Score1Label;
        public UILabel Score2Label;
        public UILabel HoldPoint1Label;
        public UISprite HoldPoint1Sprite;
        public UILabel HoldPoint2Label;
        public UISprite HoldPoint2Sprite;

        private const string RedPoint = "RedPoint";
        private const string BluePoint = "BluePoint";

        private int totalTime;

        private void SetTime(int data)
        {
            if (totalTime - data < 0)
            {
                TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRemainTime");
            }
            else
            {
                RemainTimeLabel.text = TimeHelper.SecondToClockTime(totalTime - data).ToString();
            }
        }

        public void SetTopUI()
        {
            if (RemainTime > 0)
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRemainTime"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRemainTime");
                }
                totalTime = RemainTime;
                TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleRemainTime", RemainTime, SetTime);
            }

            if (!string.IsNullOrEmpty(AllianceName1))
            {
                AllianceName1Label.text = AllianceName1;
            }
            if (!string.IsNullOrEmpty(AllianceName2))
            {
                AllianceName2Label.text = AllianceName2;
            }

            if (TotalScore >= 0)
            {
                if (Score1 >= 0)
                {
                    Score1Label.text = Score1 + "/" + TotalScore;
                }
                if (Score2 >= 0)
                {
                    Score2Label.text = Score2 + "/" + TotalScore;
                }
            }

            if (HoldPoint1 >= 0)
            {
                HoldPoint1Label.text = HoldPoint1.ToString();
            }
            if (HoldPoint2 >= 0)
            {
                HoldPoint2Label.text = HoldPoint2.ToString();
            }

            HoldPoint1Sprite.spriteName = IsBlueMine ? BluePoint : RedPoint;
            HoldPoint2Sprite.spriteName = IsBlueMine ? RedPoint : BluePoint;
        }

        public void ShowPopupInfo(string info)
        {
            ClientMain.m_UITextManager.createText(info);
        }

        #endregion

        #region Small Map

        public UIWidget MapBG;
        public UISprite PlayerPointsSprite;

        private readonly Vector4 MapBorderRange = new Vector4(-100, 100, -100, 100);

        /// <summary>
        /// Transfer position in world space to small map space.
        /// </summary>
        /// <param name="originalPosition">aixs y not considered</param>
        /// <returns>small map position</returns>
        public Vector3 SmallMapPositionTransfer(Vector3 originalPosition)
        {
            var percentVector2 = new Vector2((originalPosition.x - MapBorderRange.x) / (MapBorderRange.y - MapBorderRange.x), (originalPosition.z - MapBorderRange.z) / (MapBorderRange.w - MapBorderRange.z));

            return new Vector3(-MapBG.width / 2.0f + MapBG.width * percentVector2.x, -MapBG.height / 2.0f + MapBG.height * percentVector2.y, 0);
        }

        public void SetPlayerPositionInSmallMap()
        {
            if (m_RootManager.m_AlliancePlayerController == null || m_RootManager.m_AllianceBasicPlayerController == null)
            {
                return;
            }

            PlayerPointsSprite.transform.localPosition = SmallMapPositionTransfer(m_RootManager.m_AlliancePlayerController.transform.localPosition);
        }

        #endregion

        #region Occupy Progress Bar

        private const string BarToLeftSpriteName = "ArrowLeft";
        private const string BarToRightSpriteName = "ArrowRight";

        public float LimitOccupyBarValue;
        public float CriticalOccupyBarValue;
        [HideInInspector]
        public float CriticalValueInProgressBar1;
        [HideInInspector]
        public float CriticalValueInProgressBar2;

        public GameObject ProgressBarObject;

        public UIWidget ProgressBarBG;
        public UISprite ProgressBarFG;
        public UISprite ProgressWhiteSprite;

        private float _previousBarValue = -1;

        public void InitOccupyBar()
        {
            ProgressWhiteSprite.width = (int)(ProgressBarBG.width / LimitOccupyBarValue * CriticalOccupyBarValue);
        }

        public void ShowOccupyBar(bool isShow)
        {
            if ((ProgressBarObject.activeInHierarchy && !isShow) || (!ProgressBarObject.activeInHierarchy && isShow))
            {
                ProgressBarObject.SetActive(isShow);
            }
        }

        public void SetOccupyBar(float value)
        {
            value = Mathf.Clamp(value, 0, 1);

            if (value != _previousBarValue)
            {
                ProgressBarFG.spriteName = value > _previousBarValue ? BarToRightSpriteName : BarToLeftSpriteName;
                _previousBarValue = value;

                ProgressBarFG.transform.localPosition = new Vector3(-ProgressBarBG.width / 2.0f + ProgressBarBG.width * value, 0, 0);
            }
        }

        #endregion

        #region Dimmer

        public GameObject DimmerObject;
        public UILabel RebirthLabel;

        public int RebirthDuration;

        public void ShowDimmer(bool isShow)
        {
            DimmerObject.SetActive(isShow);

            if (isShow)
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
                }
                TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleRebirth", RebirthDuration, SetRebirthTime);
            }
        }

        private void SetRebirthTime(int time)
        {
            if (RebirthLabel.gameObject.activeInHierarchy)
            {
                RebirthLabel.text = "复活倒计时:" + (RebirthDuration - time) + "s";
            }

            if (RebirthDuration - time == 0)
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleRebirth"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleRebirth");
                }
            }
        }

        #endregion

        #region Chat

        public ChatABWindow m_ChatABWindow;

        public UILabel LastChatLabel;
        public GameObject ChatInputObject;
        public GameObject LastChatObject;

        public bool IsChatOpened = false;

        private readonly Vector3 ChatLogOpenPosition = new Vector3(0, 78.45f, 0);
        private readonly Vector3 ChatLogClosePosition = new Vector3(0, -200f, 0);
        private const float ChatObjectMoveDuration = 0.5f;

        public UIButton ChatLogButton;
        public GameObject ChatLogObject;

        public void OnChatLogClick()
        {
            ChatLogButton.isEnabled = false;

            if (!IsChatOpened)
            {
                iTween.MoveTo(ChatLogObject, iTween.Hash(
                    "position", ChatLogOpenPosition,
                    "time", ChatObjectMoveDuration,
                    "easeType", "easeOutBack",
                    "islocal", true,
                    "oncomplete", "OnOpenComplete",
                    "oncompletetarget", gameObject));
            }
            else
            {
                iTween.MoveTo(ChatLogObject, iTween.Hash(
                    "position", ChatLogClosePosition,
                    "time", ChatObjectMoveDuration,
                    "easeType", "easeInBack",
                    "islocal", true,
                    "oncomplete", "OnCloseComplete",
                    "oncompletetarget", gameObject));
            }
        }

        public void OnOpenComplete()
        {
            ChatLogButton.isEnabled = true;
            IsChatOpened = true;
            ShowChatLog(true);
            SwitchChat(true);
        }

        public void OnCloseComplete()
        {
            ChatLogButton.isEnabled = true;
            IsChatOpened = false;
            ShowChatLog(false);
            SwitchChat(false);
        }

        private void ShowChatLog(bool isOpen)
        {
            if (!isOpen)
            {
                m_ChatABWindow.ClearObject();
            }
            else
            {
                m_ChatABWindow.GetChannelFrame(m_ChatABWindow.CurrentChannel).m_ChatBaseDataHandler.Refresh(2);
            }
        }

        public void SwitchChat(bool isToInput)
        {
            ChatInputObject.SetActive(isToInput);
            LastChatObject.SetActive(!isToInput);
        }

        public void SetLastChat(string text)
        {
            LastChatLabel.text = text;
        }

        #endregion

        #region UI

        public BattlefieldInfoResp m_BattlefieldInfoResp;

        public void Refresh()
        {
            //Set top ui.
            RemainTime = m_BattlefieldInfoResp.endRemainTime;
            AllianceName1 = AllianceData.Instance.g_UnionInfo.name;

            var temp = m_BattlefieldInfoResp.battleDatas.Where(item => item.allianceId == AllianceData.Instance.g_UnionInfo.id);
            if (temp == null || temp.Count() != 1)
            {
                Debug.LogError("Error in finding player alliance data when refresh UI.");
                return;
            }
            var myBattleData = temp.First();
            TotalScore = myBattleData.scoreMax;
            Score1 = myBattleData.score;
            HoldPoint1 = myBattleData.holdNum;
            IsBlueMine = myBattleData.team == 2;

            temp = m_BattlefieldInfoResp.battleDatas.Where(item => item.allianceId != AllianceData.Instance.g_UnionInfo.id);
            if (temp == null || temp.Count() != 1)
            {
                Debug.LogError("Error in finding other player alliance data when refresh UI.");
                return;
            }
            var otherBattleData = temp.First();
            AllianceName2 = otherBattleData.allianceName;
            Score2 = otherBattleData.score;
            HoldPoint2 = otherBattleData.holdNum;

            SetTopUI();

            //Set occupy bar.
            m_BattlefieldInfoResp.campInfos.ForEach(item =>
            {
                var tempHoldPoint = AllianceBattleHoldPointManager.HoldPointList.Where(item2 => item2.id == item.id);
                if (tempHoldPoint != null && tempHoldPoint.Count() == 1)
                {
                    //Check red or blue.
                    tempHoldPoint.First().OccupyValue = item.cursorPos == 1 ? (item.curHoldValue + LimitOccupyBarValue) / (2 * LimitOccupyBarValue) : (LimitOccupyBarValue - item.curHoldValue) / (2 * LimitOccupyBarValue);
                    tempHoldPoint.First().MovingSpeed = (item.cursorPos == 1 ? 1 : -1) * item.perSecondsHoldValue / (2 * LimitOccupyBarValue);
                }
            });

            m_RootManager.m_AllianceBattleHoldPointManager.SetConfig();
        }

        public void OnReturnClick()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnReturnClickLoadCallBack);
        }

        private void OnReturnClickLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox("退出战斗",
                null, "是否离开当前联盟战？",
                null,
                LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                OnGoToReturn);
        }

        private void OnGoToReturn(int i)
        {
            switch (i)
            {
                case 1:
                    break;
                case 2:
                    {
                        OnReturnToMainCity();

                        break;
                    }
            }
        }

        private void OnReturnToMainCity()
        {
            ExitScene temp = new ExitScene { uid = 0 };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.EXIT_FIGHT_SCENE);

            //GoToReturn.
            if (AllianceData.Instance.IsAllianceNotExist)
            {
                SceneManager.EnterMainCity();
            }
            else
            {
                SceneManager.EnterAllianceCity();
            }
        }

        #endregion

        #region Attack

        public UIButton m_AttackButton;
        public UISprite m_AttackSprite;
        public long m_ToAttackId = -1;

        public UILabel ToAttackLabel;

        void Update()
        {
            if (m_RootManager.m_AlliancePlayerManager == null || m_RootManager.m_AllianceBasicPlayerController == null) return;

            //Update small map
            {
                SetPlayerPositionInSmallMap();
            }

            //Show select effect.
            {
                if (m_RootManager.m_AlliancePlayerManager.m_PlayerDic == null || m_RootManager.m_AlliancePlayerManager.m_PlayerDic.Count == 0)
                {
                    //[ALERT]Donot modify rank of call.
                    DeactiveAttack();
                    m_ToAttackId = -1;
                    return;
                }

                //In distance
                var temp = m_RootManager.m_AlliancePlayerManager.m_PlayerDic.Where(item => Vector3.Distance(m_RootManager.m_AlliancePlayerController.transform.position, item.Value.transform.position) < AttackDistance);
                if (!temp.Any())
                {
                    DeactiveAttack();
                    m_ToAttackId = -1;
                    return;
                }

                //In angle
                temp = temp.Where(item => Vector3.Angle(m_RootManager.m_AlliancePlayerController.transform.forward, item.Value.transform.position - m_RootManager.m_AlliancePlayerController.transform.position) < AttackDegree / 2.0f);
                if (!temp.Any())
                {
                    DeactiveAttack();
                    m_ToAttackId = -1;
                    return;
                }

                //Select ememy
                temp = temp.Where(item => item.Value.GetComponent<AllianceBasicPlayerController>().AllianceName != AllianceData.Instance.g_UnionInfo.name);
                if (!temp.Any())
                {
                    DeactiveAttack();
                    m_ToAttackId = -1;
                    return;
                }

                //Get only one target
                //[ALERT]Donot modify rank of call.
                var temp2 = temp.ToList().OrderBy(item => Vector3.Distance(m_RootManager.m_AlliancePlayerController.transform.position, item.Value.transform.position)).First();
                m_ToAttackId = temp2.Key;
                ActiveAttack(temp2.Value.GetComponent<AllianceBasicPlayerController>().KingName);
            }
        }

        public void ActiveAttack(string toAttackName)
        {
            m_AttackButton.isEnabled = true;
            m_AttackSprite.color = Color.white;
            ToAttackLabel.text = "目标:" + toAttackName;

            m_RootManager.m_AlliancePlayerManager.m_PlayerDic[m_ToAttackId].GetComponent<AllianceBasicPlayerController>().OnSelected();
        }

        public void DeactiveAttack()
        {
            m_AttackButton.isEnabled = false;
            m_AttackSprite.color = Color.grey;
            ToAttackLabel.text = "目标:无";

            if (m_ToAttackId > 0 && m_RootManager.m_AlliancePlayerManager.m_PlayerDic.Keys.Contains(m_ToAttackId))
            {
                m_RootManager.m_AlliancePlayerManager.m_PlayerDic[m_ToAttackId].GetComponent<AllianceBasicPlayerController>().OnDeSelected();
            }
        }

        public void OnAttackClick()
        {
            if (m_ToAttackId < 0) return;

            FightAttackReq tempInfo = new FightAttackReq()
            {
                targetId = m_ToAttackId
            };
            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer tempSer = new QiXiongSerializer();
            tempSer.Serialize(tempStream, tempInfo);
            byte[] t_protof;
            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FIGHT_ATTACK_REQ, ref t_protof);

            //Play animation
            m_RootManager.m_AlliancePlayerController.m_Animator.SetTrigger("Attack");
            m_RootManager.m_AlliancePlayerManager.m_PlayerDic[m_ToAttackId].GetComponent<Animator>().SetTrigger("BATC");
        }

        #endregion

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    case ProtoIndexes.S_FIGHT_ATTACK_RESP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            FightAttackResp tempInfo = new FightAttackResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            bool isMineAttack = false;

                            var temp = m_RootManager.m_AlliancePlayerManager.m_PlayerDic.Where(item => item.Key == tempInfo.attackId).ToList();
                            if (temp != null && temp.Count() > 0)
                            {
                                //other player attack.
                                temp.First().Value.GetComponent<Animator>().SetTrigger("Attack");
                            }
                            else
                            {
                                //mine attack.
                                isMineAttack = true;
                            }

                            var temp2 = m_RootManager.m_AlliancePlayerManager.m_PlayerDic.Where(item => item.Key == tempInfo.targetId).ToList();
                            if (temp2 != null && temp2.Count() > 0)
                            {
                                //other player been attack.
                                //Cancel play animation when mine attack.
                                if (!isMineAttack)
                                {
                                    temp2.First().Value.GetComponent<Animator>().SetTrigger("BATC");
                                }
                                temp2.First().Value.GetComponent<AllianceBasicPlayerController>().OnDamage(tempInfo.damage, tempInfo.remainLife);
                            }
                            else if (m_RootManager.m_AllianceBasicPlayerController != null && m_RootManager.m_AlliancePlayerController != null)
                            {
                                //mine been attack.
                                //Cancel play animation when mine attack.
                                if (!isMineAttack)
                                {
                                    m_RootManager.m_AlliancePlayerController.GetComponent<Animator>().SetTrigger("BATC");
                                }
                                m_RootManager.m_AllianceBasicPlayerController.OnDamage(tempInfo.damage, tempInfo.remainLife);
                            }

                            return true;
                        }
                    //Set UI and player info.
                    case ProtoIndexes.ALLIANCE_BATTLE_FIELD_RESP:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattlefieldInfoResp tempInfo = new BattlefieldInfoResp();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_BattlefieldInfoResp = tempInfo;

                            Refresh();

                            //Set character blood.
                            if (m_RootManager.m_AllianceBasicPlayerController != null)
                            {
                                m_RootManager.m_AllianceBasicPlayerController.TotalBlood = m_BattlefieldInfoResp.totalLife;
                                m_RootManager.m_AllianceBasicPlayerController.RemainingBlood = m_BattlefieldInfoResp.remainLife;
                            }
                            m_RootManager.m_AllianceBasicPlayerController.SetThis();
                            //Set character position.
                            m_RootManager.m_AlliancePlayerController.transform.localPosition = new Vector3(m_BattlefieldInfoResp.posX, m_RootManager.m_AlliancePlayerController.transform.localPosition.y, m_BattlefieldInfoResp.posZ);

                            return true;
                        }
                    //Refresh info.
                    case ProtoIndexes.ALLIANCE_BATTLE_FIELD_NOTIFY:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattlefieldInfoNotify tempInfo = new BattlefieldInfoNotify();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            m_BattlefieldInfoResp.endRemainTime = tempInfo.endRemainTime;
                            m_BattlefieldInfoResp.battleDatas = tempInfo.battleDatas;
                            m_BattlefieldInfoResp.campInfos = tempInfo.campInfos;

                            Refresh();

                            return true;
                        }
                    //End battle msg.
                    case ProtoIndexes.ALLIANCE_BATTLE_RESULT:
                        {
                            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            BattleResultAllianceFight tempInfo = new BattleResultAllianceFight();
                            t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                            BattleControlor.BattleResult result = tempInfo.result ? BattleControlor.BattleResult.RESULT_WIN : BattleControlor.BattleResult.RESULT_LOSE;
                            List<Enums.Currency> currencyList = new List<Enums.Currency>();
                            List<int> numList = new List<int>();
                            tempInfo.awardItems.ForEach(item =>
                            {
                                currencyList.Add(Enums.Currency.GongXian);
                                numList.Add(item.awardNum);
                            });
                            int second = tempInfo.costTime;

                            EnterBattleResult.showBattleResult(result, currencyList, numList, second, second, OnReturnToMainCity);

                            return true;
                        }
                }
            }

            return false;
        }

#if UNITY_EDITOR
        void OnGUI()
        {

            if (GUILayout.Button("do"))
            {
                EnterBattleResult.showBattleResult(BattleControlor.BattleResult.RESULT_WIN, new List<Enums.Currency>() { Enums.Currency.GongXian }, new List<int>() { 100 }, 55, 60, OnReturnToMainCity);
            }
        }
#endif

        void Start()
        {
            SetLeftTopUI();
            SocketHelper.SendQXMessage(ProtoIndexes.ALLIANCE_BATTLE_FIELD_REQ);

            InitOccupyBar();
        }

        void Awake()
        {
            SocketTool.RegisterSocketListener(this);

            //Read data from xmls.
            AttackDistance = LianmengzhanTemplate.Templates[0].ScreeningDistance;
            AttackDegree = LianmengzhanTemplate.Templates[0].ScreenAngle;
            RebirthDuration = LianmengzhanTemplate.Templates[0].ReviveTime;
            TotalScore = (int)LianmengzhanTemplate.Templates[0].ScoreMax;
            CriticalOccupyBarValue = LMZBuildingTemplate.GetTemplatesBySide(0).First().CriticalValue;
            LimitOccupyBarValue = LMZBuildingTemplate.GetTemplatesBySide(0).First().ZhanlingzhiMax;
            CriticalValueInProgressBar1 = 0.5f - 0.5f / LimitOccupyBarValue * CriticalOccupyBarValue;
            CriticalValueInProgressBar2 = 0.5f + 0.5f / LimitOccupyBarValue * CriticalOccupyBarValue;
        }

        void OnDestroy()
        {
            SocketTool.UnRegisterSocketListener(this);
        }
    }
}

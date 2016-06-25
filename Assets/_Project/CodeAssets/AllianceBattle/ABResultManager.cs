using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllianceBattle
{
    public class ABResultManager : MonoBehaviour
    {
        public AllianceBattleMain m_AllianceBattleMain;

        private bool isResultWin;
        public UISprite ResultFlagSprite;
        private string resultFlagWin = "result_flag";
        private string resultFlagLose = "result_lose";

        public UISprite ResultSprite;
        private string resultWin = "result_win_3";
        private string resultLose = "result_lose_0";

        private int resultScoreNum;
        private int resultRankNum;
        private int resultKillNum;
        private int resultMyGainNum;
        public UILabel ResultScoreLabel;
        public UILabel ResultRankLabel;
        public UILabel ResultKillLabel;
        public UILabel ResultMyGainLabel;

        public List<UISprite> NorHoldNoSpriteList = new List<UISprite>();
        public List<UISprite> AdHoldNoSpriteList = new List<UISprite>();
        public UISprite ProtecterBaseNoSprite;
        public UISprite AttackerBaseNoSprite;

        public UILabel NorHoldDescLabel;
        public UILabel AdHoldDescLabel;
        public UILabel ProtecterBaseDescLabel;
        public UILabel AttackerBaseDescLabel;

        public UILabel ResultAllianceGainLabel;

        public List<ABResultHeadSetter> m_AbResultHeadSetterList = new List<ABResultHeadSetter>();

        public UIGrid m_BestPlayerGrid;
        public GameObject m_BestPlayerPrefab;

        public GameObject ScoreListButtonObject;

        public void ShowBattleResult(bool isSucceed, int personalScore, int rank, int killNum, int allianceGain, bool isCanClickScoreBTN = true)
        {
            //Request score list.
            m_AllianceBattleMain.m_RootManager.m_AllianceBattleMsgManager.ExecuteAfterScoreList = m_AllianceBattleMain.m_RootManager.m_AllianceBattleMain.m_ABResultManager.SetScoreListData;
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_SCORE_LIST);

            //flag
            isResultWin = isSucceed;
            ResultFlagSprite.spriteName = isSucceed ? resultFlagWin : resultFlagLose;
            ResultSprite.spriteName = isSucceed ? resultWin : resultLose;

            //result stat
            resultScoreNum = personalScore;
            ResultScoreLabel.text = "积分：";
            resultRankNum = rank;
            ResultRankLabel.text = "个人排名：";
            resultKillNum = killNum;
            ResultKillLabel.text = "杀敌：";
            ResultAllianceGainLabel.text = "联盟获得功勋：" + allianceGain + "\n由全体成员平分";

            //holds stat
            int norDestroyedNum = 0;
            int adDestroyedNum = 0;
            bool isAttackerBaseDestroyed = false;
            bool isProtecterBaseDestroyed = false;

            foreach (var occupyPoint in m_AllianceBattleMain.m_RootManager.m_AbHoldPointManager.HoldPointDic.Values)
            {
                if (occupyPoint.Type == 2)
                {
                    if (occupyPoint.IsDestroyed)
                    {
                        norDestroyedNum++;
                    }
                }
                else if (occupyPoint.Type == 3)
                {
                    if (occupyPoint.IsDestroyed)
                    {
                        adDestroyedNum++;
                    }
                }
                else if (occupyPoint.Type == 4)
                {
                    if (occupyPoint.Side == 1)
                    {
                        isProtecterBaseDestroyed = occupyPoint.IsDestroyed;
                    }
                    else if (occupyPoint.Side == 2)
                    {
                        isAttackerBaseDestroyed = occupyPoint.IsDestroyed;
                    }
                }
            }

            NorHoldNoSpriteList.ForEach(item => item.gameObject.SetActive(false));
            NorHoldNoSpriteList.Take(norDestroyedNum).ToList().ForEach(item => item.gameObject.SetActive(true));
            //Attacker
            if (m_AllianceBattleMain.m_RootManager.MyPart == 1)
            {
                if (norDestroyedNum > 0)
                {
                    NorHoldDescLabel.text = "成功占领" + norDestroyedNum + "座前哨";
                }
                else
                {
                    NorHoldDescLabel.text = "未能占领前哨";
                }
            }
            //Protecter
            else if (m_AllianceBattleMain.m_RootManager.MyPart == 2)
            {
                if (norDestroyedNum > 0)
                {
                    NorHoldDescLabel.text = "我方" + norDestroyedNum + "座前哨失守";
                }
                else
                {
                    NorHoldDescLabel.text = "成功守护全部前哨";
                }
            }

            AdHoldNoSpriteList.ForEach(item => item.gameObject.SetActive(false));
            AdHoldNoSpriteList.Take(adDestroyedNum).ToList().ForEach(item => item.gameObject.SetActive(true));
            //Attacker
            if (m_AllianceBattleMain.m_RootManager.MyPart == 1)
            {
                if (adDestroyedNum > 0)
                {
                    AdHoldDescLabel.text = "成功占领" + adDestroyedNum + "座行营";
                }
                else
                {
                    AdHoldDescLabel.text = "未能占领行营";
                }
            }
            //Protecter
            else if (m_AllianceBattleMain.m_RootManager.MyPart == 2)
            {
                if (adDestroyedNum > 0)
                {
                    AdHoldDescLabel.text = "我方" + adDestroyedNum + "座行营失守";
                }
                else
                {
                    AdHoldDescLabel.text = "成功守护全部行营";
                }
            }

            ProtecterBaseNoSprite.gameObject.SetActive(isProtecterBaseDestroyed);
            //Attacker
            if (m_AllianceBattleMain.m_RootManager.MyPart == 1)
            {
                if (isProtecterBaseDestroyed)
                {
                    ProtecterBaseDescLabel.text = ColorTool.Color_Green_00ff00 + "成功占领守方城池" + "[-]";
                }
                else
                {
                    ProtecterBaseDescLabel.text = "未能占领守方城池";
                }
            }
            //Protecter
            else if (m_AllianceBattleMain.m_RootManager.MyPart == 2)
            {
                if (isProtecterBaseDestroyed)
                {
                    ProtecterBaseDescLabel.text = ColorTool.Color_Red_c40000 + "我方城池被攻占" + "[-]";
                }
                else
                {
                    ProtecterBaseDescLabel.text = ColorTool.Color_Green_00ff00 + "成功守护我方城池" + "[-]";
                }
            }

            AttackerBaseNoSprite.gameObject.SetActive(isAttackerBaseDestroyed);
            //Attacker
            if (m_AllianceBattleMain.m_RootManager.MyPart == 1)
            {
                if (isAttackerBaseDestroyed)
                {
                    AttackerBaseDescLabel.text = ColorTool.Color_Red_c40000 + "我方帅旗被摧毁" + "[-]";
                }
                else
                {
                    AttackerBaseDescLabel.text = "我方帅旗未被摧毁";
                }
            }
            //Protecter
            else if (m_AllianceBattleMain.m_RootManager.MyPart == 2)
            {
                if (isAttackerBaseDestroyed)
                {
                    AttackerBaseDescLabel.text = ColorTool.Color_Green_00ff00 + "成功摧毁敌方帅旗" + "[-]";
                }
                else
                {
                    AttackerBaseDescLabel.text = "未摧毁敌方帅旗";
                }
            }

            ScoreListButtonObject.GetComponent<UISprite>().color = isCanClickScoreBTN ? new Color(1f, 1f, 1f, 1f) : new Color(.5f, .5f, .5f, 1f);
            ScoreListButtonObject.GetComponent<BoxCollider>().enabled = isCanClickScoreBTN;
        }

        private float gridBasePositionX = -159.76f;

        public void SetScoreListData()
        {
            var myScore = m_AllianceBattleMain.m_AbScoreWindowController.m_ScoreDataList.Where(item => item.ID == JunZhuData.Instance().m_junzhuInfo.id);
            resultMyGainNum = myScore.Any() ? myScore.First().GongXun : 0;
            ResultMyGainLabel.text = "个人功勋：";

            //best player
            while (m_BestPlayerGrid.transform.childCount != 0)
            {
                var child = m_BestPlayerGrid.transform.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }

            var top3Score = m_AllianceBattleMain.m_AbScoreWindowController.m_ScoreDataList.OrderByDescending(item => item.Score).Take(3).ToList();
            for (int i = 0; i < top3Score.Count; i++)
            {
                var bestPlayer = Instantiate<GameObject>(m_BestPlayerPrefab);
                TransformHelper.ActiveWithStandardize(m_BestPlayerGrid.transform, bestPlayer.transform);
                var bestPlayerController = bestPlayer.GetComponent<ABResultHeadSetter>();
                bestPlayerController.SetThis(top3Score[i].RoleID, top3Score[i].Name, top3Score[i].AllianceName, i + 1);
            }

            if (top3Score.Count == 3)
            {
                m_BestPlayerGrid.transform.localPosition = new Vector3(gridBasePositionX, m_BestPlayerGrid.transform.localPosition.y, m_BestPlayerGrid.transform.localPosition.z);
            }
            else if (top3Score.Count == 2)
            {
                m_BestPlayerGrid.transform.localPosition = new Vector3(gridBasePositionX + m_BestPlayerGrid.cellWidth * 2 / 3, m_BestPlayerGrid.transform.localPosition.y, m_BestPlayerGrid.transform.localPosition.z);
            }
            else if (top3Score.Count == 1)
            {
                m_BestPlayerGrid.transform.localPosition = new Vector3(gridBasePositionX + m_BestPlayerGrid.cellWidth, m_BestPlayerGrid.transform.localPosition.y, m_BestPlayerGrid.transform.localPosition.z);
            }
            m_BestPlayerGrid.Reposition();

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultDelay"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultDelay");
            }
            TimeHelper.Instance.AddOneDelegateToTimeCalc("ABResultDelay", m_ResultDelayTime, ActiveWindow);
        }

        private void ActiveWindow()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleResult"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleResult");
            }
            TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleResult", m_ResultTotalTime, OnUpdateABResultTime);

            m_AllianceBattleMain.m_Joystick.m_Box.enabled = false;
            m_AllianceBattleMain.m_MainUIVagueEffect.enabled = true;
            m_AllianceBattleMain.m_Top2UIVagueEffect.enabled = true;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultDelay"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultDelay");
            }

            ProcessResultAnimation();
        }

        #region Animations

        public GameObject StatListObject;
        public GameObject PersonalAndBestObject;
        public GameObject QuitAndInfosObject;

        private void ProcessResultAnimation()
        {
            ResultFlagSprite.gameObject.SetActive(false);
            StatListObject.SetActive(false);
            PersonalAndBestObject.SetActive(false);
            QuitAndInfosObject.SetActive(false);

            gameObject.SetActive(true);

            ShowFlag();
        }

        private void ShowFlag()
        {
            //flag
            ResultFlagSprite.transform.localPosition = Vector3.zero;
            ResultFlagSprite.gameObject.SetActive(true);

            if (isResultWin)
            {
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, ResultFlagSprite.gameObject, EffectIdTemplate.GetPathByeffectId(100102));
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, ResultFlagSprite.gameObject, EffectIdTemplate.GetPathByeffectId(100105));
            }

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShow"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShow");
            }
            TimeHelper.Instance.AddOneDelegateToTimeCalc("ABResultFlagShow", 2.1f, ShadeFlagOut);
        }

        private Vector3 FlagEndPos = new Vector3(-191.23f, 157.37f, 0);

        private void ShadeFlagOut()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShow"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShow");
            }

            totalCalcTime = 0.3f;
            parentCalcObject = ResultFlagSprite.gameObject;
            isDecending = true;
            ExecuteAfterSets = ShadeFlagIn;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShadeOut"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShadeOut");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultFlagShadeOut", .3f, SetAInAllWidgets);
        }

        private void ShadeFlagIn()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShadeOut"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShadeOut");
            }

            ResultFlagSprite.gameObject.SetActive(false);
            ResultFlagSprite.transform.localPosition = FlagEndPos;
            ResultFlagSprite.color = new Color(ResultFlagSprite.color.r, ResultFlagSprite.color.g, ResultFlagSprite.color.b, 0);
            ResultFlagSprite.gameObject.SetActive(true);

            totalCalcTime = 0.3f;
            parentCalcObject = ResultFlagSprite.gameObject;
            isDecending = false;
            ExecuteAfterSets = ShadeStatList;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShadeIn"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShadeIn");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultFlagShadeIn", .3f, SetAInAllWidgets);
        }

        private void ShadeStatList()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultFlagShadeIn"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultFlagShadeIn");
            }

            StatListObject.GetComponentsInChildren<UIWidget>(true).ToList().ForEach(item => item.color = new Color(item.color.r, item.color.g, item.color.b, 0));
            StatListObject.gameObject.SetActive(true);

            totalCalcTime = 0.3f;
            parentCalcObject = StatListObject;
            isDecending = false;
            ExecuteAfterSets = ShadePersonalAndBestList;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultStatShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultStatShade");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultStatShade", .3f, SetAInAllWidgets);
        }

        private void ShadePersonalAndBestList()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultStatShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultStatShade");
            }

            PersonalAndBestObject.GetComponentsInChildren<UIWidget>(true).ToList().ForEach(item => item.color = new Color(item.color.r, item.color.g, item.color.b, 0));
            PersonalAndBestObject.gameObject.SetActive(true);

            totalCalcTime = 0.3f;
            parentCalcObject = PersonalAndBestObject;
            isDecending = false;
            ExecuteAfterSets = IncreaseInfoNums;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultPersonalAndBestShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultPersonalAndBestShade");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultPersonalAndBestShade", .3f, SetAInAllWidgets);
        }

        private void IncreaseInfoNums()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultPersonalAndBestShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultPersonalAndBestShade");
            }

            ResultScoreLabel.text = "积分：" + 0;
            ResultRankLabel.text = "个人排名：" + 0;
            ResultKillLabel.text = "杀敌：" + 0;
            ResultMyGainLabel.text = "个人功勋：" + 0;

            totalCalcTime = 2.5f;
            ExecuteAfterSets = ShadQuitAndInfosList;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultNumIncrease"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultNumIncrease");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultNumIncrease", 2.5f, SetNumsInAllLabels);
        }

        private void ShadQuitAndInfosList()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultNumIncrease"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultNumIncrease");
            }

            QuitAndInfosObject.GetComponentsInChildren<UIWidget>(true).ToList().ForEach(item => item.color = new Color(item.color.r, item.color.g, item.color.b, 0));
            QuitAndInfosObject.gameObject.SetActive(true);

            totalCalcTime = 0.3f;
            parentCalcObject = QuitAndInfosObject;
            isDecending = false;
            ExecuteAfterSets = EndResultAnimation;

            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultQuitAndInfosShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultQuitAndInfosShade");
            }
            TimeHelper.Instance.AddFrameDelegateToTimeCalc("ABResultQuitAndInfosShade", .3f, SetAInAllWidgets);
        }

        private void EndResultAnimation()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("ABResultQuitAndInfosShade"))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("ABResultQuitAndInfosShade");
            }
        }

        private float totalCalcTime;
        private GameObject parentCalcObject;
        private DelegateHelper.VoidDelegate ExecuteAfterSets;
        private bool isDecending = false;

        private void SetAInAllWidgets(float elapsedTime)
        {
            float percent = elapsedTime / totalCalcTime;
            if (percent > 0 && percent < 1)
            {
                parentCalcObject.GetComponentsInChildren<UIWidget>(true).ToList().ForEach(item => item.color = new Color(item.color.r, item.color.g, item.color.b, isDecending ? (1 - percent) : percent));
            }
            else if (percent >= 1)
            {
                parentCalcObject.GetComponentsInChildren<UIWidget>(true).ToList().ForEach(item => item.color = new Color(item.color.r, item.color.g, item.color.b, isDecending ? 0 : 1));

                if (ExecuteAfterSets != null)
                {
                    ExecuteAfterSets();
                }
            }
        }

        private void SetNumsInAllLabels(float elapsedTime)
        {
            float percent = elapsedTime / totalCalcTime;

            if (percent > 0 && percent < 1)
            {
                ResultScoreLabel.text = "积分：" + (int)(resultScoreNum * percent);
                ResultRankLabel.text = "个人排名：" + (int)(resultRankNum * percent);
                ResultKillLabel.text = "杀敌：" + (int)(resultKillNum * percent);
                ResultMyGainLabel.text = "个人功勋：" + (int)(resultMyGainNum * percent);
            }
            else if (percent >= 1)
            {
                ResultScoreLabel.text = "积分：" + resultScoreNum;
                ResultRankLabel.text = "个人排名：" + resultRankNum;
                ResultKillLabel.text = "杀敌：" + resultKillNum;
                ResultMyGainLabel.text = "个人功勋：" + resultMyGainNum;

                if (ExecuteAfterSets != null)
                {
                    ExecuteAfterSets();
                }
            }
        }

        #endregion

        public UILabel m_ResultRemainingLabel;
        private const int m_ResultTotalTime = 300;
        private const int m_ResultDelayTime = 3;

        private void OnUpdateABResultTime(int p_time)
        {
            if (m_ResultTotalTime - p_time > 0)
            {
                m_ResultRemainingLabel.text = (m_ResultTotalTime - p_time) + "秒后将自动退出战场";
            }
            else
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleResult"))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleResult");
                }
                ClickReturnWindow(2);
            }
        }

        public void OnReturnClick()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnReturnCallBack);
        }

        private void OnReturnCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            var returnConfirm = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            returnConfirm.setBox("退出战场",
                 "是否退出当前战场回到大地图?", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
                 ClickReturnWindow);
        }

        public void ClickReturnWindow(int i)
        {
            switch (i)
            {
                case 1:
                    break;
                case 2:
                    if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleResult"))
                    {
                        TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleResult");
                    }

                    Global.m_isCityWarOpen = (AllianceData.Instance.g_UnionInfo != null);
                    PlayerSceneSyncManager.Instance.ExitAB();
                    break;
            }
        }
    }
}
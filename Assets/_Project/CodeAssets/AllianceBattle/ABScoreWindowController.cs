using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABScoreWindowController : MonoBehaviour, SocketListener
    {
        public AllianceBattleMain m_AllianceBattleMain;
        public GameObject FloatButtonPrefab;

        public void OnScoreClick()
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
            {
                m_AllianceBattleMain.m_RootManager.m_AllianceBattleMsgManager.ExecuteAfterScoreList = m_AllianceBattleMain.m_RootManager.m_AllianceBattleMain.m_AbScoreWindowController.ShowScoreWindow;

                SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_SCORE_LIST);
            }
            else
            {
                if (m_AllianceBattleMain.battleStatState >= 0)
                {
                    m_AllianceBattleMain.m_RootManager.m_AllianceBattleMsgManager.ExecuteAfterScoreList = m_AllianceBattleMain.m_RootManager.m_AllianceBattleMain.m_AbScoreWindowController.ShowScoreWindow;

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.LMZ_SCORE_LIST);
                }
                else
                {
                    ClientMain.m_UITextManager.createText("开战后可以查看积分战况");
                }
            }
        }

        public void OnCloseScoreClick()
        {
            HideScoreWindow();
        }

        public UIScrollView ScoreScrollView;
        public UIScrollBar ScoreScrollBar;
        public UIGrid ScoreScrollGrid;
        public GameObject ScoreDetailPrefab;

        public UILabel MyRankLabel;
        public UILabel NoDataLabel;

        public List<ABScoreItemController.ScoreData> m_ScoreDataList = new List<ABScoreItemController.ScoreData>();
        public List<ABScoreItemController> m_ScoreItemList = new List<ABScoreItemController>();

        public GameObject AttackerSelectObject;
        public GameObject DefenderSelectObject;

        public GameObject AttackerBTN;
        public GameObject DefenderBTN;
        public GameObject CannotClickDefenderBTN;

        public void OnAttackerScoreClick()
        {
            RefreshScoreWindow(true);
        }

        public void OnDefenderScoreClick()
        {
            RefreshScoreWindow(false);
        }

        public void ShowScoreWindow()
        {
            gameObject.SetActive(true);

            m_AllianceBattleMain.m_Joystick.m_Box.enabled = false;
            m_AllianceBattleMain.m_MainUIVagueEffect.enabled = true;
            m_AllianceBattleMain.m_Top2UIVagueEffect.enabled = true;
            RefreshScoreWindow(m_AllianceBattleMain.m_RootManager.MyPart == 1);
        }

        public void HideScoreWindow()
        {
            gameObject.SetActive(false);

            if (!m_AllianceBattleMain.m_RootManager.m_Top1Hierarchy.IsNGUIVisible())
            {
                m_AllianceBattleMain.m_Joystick.m_Box.enabled = true;
                m_AllianceBattleMain.m_MainUIVagueEffect.enabled = false;
                m_AllianceBattleMain.m_Top2UIVagueEffect.enabled = false;
            }
        }

        public void RefreshScoreWindow(bool isAttacker)
        {
            AttackerSelectObject.SetActive(isAttacker);
            DefenderSelectObject.SetActive(!isAttacker);

            while (ScoreScrollGrid.transform.childCount > 0)
            {
                var child = ScoreScrollGrid.transform.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }
            m_ScoreItemList.Clear();

            var tempScoreList = m_ScoreDataList.Where(item => (isAttacker && ((m_AllianceBattleMain.m_RootManager.MyPart == 1 && item.AllianceName == AllianceData.Instance.g_UnionInfo.name) || (m_AllianceBattleMain.m_RootManager.MyPart == 2 && item.AllianceName != AllianceData.Instance.g_UnionInfo.name))) || (!isAttacker && ((m_AllianceBattleMain.m_RootManager.MyPart == 1 && item.AllianceName != AllianceData.Instance.g_UnionInfo.name) || (m_AllianceBattleMain.m_RootManager.MyPart == 2 && item.AllianceName == AllianceData.Instance.g_UnionInfo.name)))).OrderBy(item => item.Rank).ToList();
            tempScoreList.ForEach(item =>
            {
                var controller = NGUITools.AddChild(ScoreScrollGrid.gameObject, ScoreDetailPrefab).GetComponent<ABScoreItemController>();
                controller.m_ScoreData = item;
                controller.SetThis();

                controller.gameObject.name = item.Rank + "_" + controller.gameObject.name;
                controller.gameObject.SetActive(true);

                m_ScoreItemList.Add(controller);
            });

            ScoreScrollGrid.Reposition();

            if (!tempScoreList.Any())
            {
                NoDataLabel.text = LanguageTemplate.GetText(isAttacker ? 4723 : 4724);
                NoDataLabel.gameObject.SetActive(true);
            }
            else
            {
                NoDataLabel.gameObject.SetActive(false);
            }

            var temp = m_ScoreDataList.Where(item => item.Name == JunZhuData.Instance().m_junzhuInfo.name).ToList();
            if (temp.Any())
            {
                MyRankLabel.text = temp.First().Rank.ToString();
            }
        }

        public void ClampScrollView()
        {
            //clamp scroll bar value.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            StartCoroutine(DoClampScrollView());
        }

        IEnumerator DoClampScrollView()
        {
            yield return new WaitForEndOfFrame();

            ScoreScrollView.UpdateScrollbars(true);
            float scrollValue = ScoreScrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) ScoreScrollBar.value = 0.99f;
            if (scrollValue <= 0) ScoreScrollBar.value = 0.01f;
        }

        public void OnDragAreaPress()
        {
            m_ScoreItemList.ForEach(item => item.DestroyFloatButtons());
        }

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    //Black list.
                    case ProtoIndexes.S_Join_BlackList_Resp:
                        {
                            object joinToBlackListRespObject = new BlacklistResp();
                            if (SocketHelper.ReceiveQXMessage(ref joinToBlackListRespObject, p_message,
                                ProtoIndexes.S_Join_BlackList_Resp))
                            {
                                BlacklistResp tempJoinBlacklistResp = joinToBlackListRespObject as BlacklistResp;

                                //shield if return 0.
                                if (tempJoinBlacklistResp.result == 0)
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ShieldSucceedCallBack);

                                    //Refresh black list after shield succeed.
                                    BlockedData.Instance().RequestBlockedInfo();
                                }
                                else if (tempJoinBlacklistResp.result == 1)
                                {
                                    Debug.LogWarning("Trying to add to black list fail.");
                                }
                                return true;
                            }
                            return false;
                        }
                }
            }
            return false;
        }

        public string ShieldName = "";

        public void ShieldSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "您已成功添加" + ShieldName + "到屏蔽列表", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        void Awake()
        {
            SocketTool.RegisterSocketListener(this);

            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), OnFloatButtonLoadCallBack);
        }

        private void OnFloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            FloatButtonPrefab = p_object as GameObject;
        }

        void OnDestroy()
        {
            SocketTool.RegisterSocketListener(this);
        }
    }
}
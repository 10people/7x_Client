using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class BattleController : ModuleController, SocketListener
    {
        public override void OnMyDataClick(GameObject go)
        {
            RequestOne(JunZhuData.Instance().m_junzhuInfo.name);
        }

        public void Refresh(List<GuoGuanInfo> list)
        {
            //Find nothing in one mode.
            if ((list == null || list.Count == 0) && IsOneMode)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), m_RootController.FindNoKingCallBack);
                return;
            }

            while (m_Grid.transform.childCount != 0)
            {
                var child = m_Grid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
            m_DetailControllerList.Clear();

            if (list != null && list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var temp = Instantiate(m_Prefab) as GameObject;
                    TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);
                    var controller = temp.GetComponent<BattleDetailController>();

                    temp.name += "_" + UtilityTool.FullNumWithZeroDigit(i, list.Count.ToString().Length);

                    controller.m_ModuleController = this;
                    controller.m_BattleInfo = list[i];
                    controller.SetThis();

                    m_DetailControllerList.Add(controller);

                    m_Grid.Reposition();

                    //Reset scroll view.
                    m_ScrollView.UpdateScrollbars(m_ScrollBar);
                    m_ScrollBar.value = IsRefreshToTop ? 0.0f : 1.0f;
                }
                NoDataLabel.gameObject.SetActive(false);
            }
            else
            {
                NoDataLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.PAI_HANG_BANG_01);
                NoDataLabel.gameObject.SetActive(true);
            }

            //High light item.
            if (IsOneMode)
            {
                var detailControllerList = m_DetailControllerList.Select(item => item as BattleDetailController).Where(item => item.m_BattleInfo.name == m_OneModeNameStr).ToList();
                if (detailControllerList.Count == 1)
                {
                    UISprite widget = detailControllerList[0].GetComponent<UISprite>();
                    widget.color = new Color(244, 154, 0);

                    float widgetValue = m_ScrollView.GetWidgetValueRelativeToScrollView(widget).y;
                    if (widgetValue < 0 || widgetValue > 1)
                    {
                        m_ScrollView.SetWidgetValueRelativeToScrollView(widget, 0);

                        //clamp scroll bar value.
                        //donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
                        //set 0.99 and 0.01 cause same bar value not taken in execute.
                        float scrollValue = m_ScrollView.GetSingleScrollViewValue();
                        if (scrollValue >= 1) m_ScrollBar.value = 0.99f;
                        if (scrollValue <= 0) m_ScrollBar.value = 0.01f;
                    }
                }
            }
        }

        public override void GetMyRank(int type, int nationID)
        {
            GetRankReq temp = new GetRankReq { rankType = type, guojiaId = nationID, id = (int)JunZhuData.Instance().m_junzhuInfo.id };

            SocketHelper.SendQXMessage(temp, ProtoIndexes.RANKING_MY_RANK_REQ);
        }

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    case ProtoIndexes.RANKING_RESP:
                        {
                            MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            RankingResp tempResp = new RankingResp();
                            t_qx.Deserialize(t_tream, tempResp, tempResp.GetType());

                            if (tempResp.rankType != 4) return false;

                            //Set page.
                            CurrentPageIndex = tempResp.pageNo;
                            TotalPageIndex = tempResp.pageCount;

                            Refresh(tempResp.guoguanList);

                            return true;
                        }
                    case ProtoIndexes.RANKING_MY_RANK_RESP:
                        {
                            MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                            QiXiongSerializer t_qx = new QiXiongSerializer();
                            GetRankResp tempResp = new GetRankResp();
                            t_qx.Deserialize(t_tream, tempResp, tempResp.GetType());

                            //Set rank label.
                            if (tempResp.rank <= 0)
                            {
                                MiddleLabel.text = "无";
                                MyDataHandler.GetComponent<UIButton>().isEnabled = false;
                            }
                            else
                            {
                                MiddleLabel.text = tempResp.rank.ToString();
                                MyDataHandler.GetComponent<UIButton>().isEnabled = true;
                            }

                            return true;
                        }
                }
            }
            return false;
        }

        new void Update()
        {
            base.Update();
        }

        new void Awake()
        {
            base.Awake();
            SocketTool.RegisterSocketListener(this);
        }

        new void OnDestroy()
        {
            base.OnDestroy();
            SocketTool.UnRegisterSocketListener(this);
        }
    }
}

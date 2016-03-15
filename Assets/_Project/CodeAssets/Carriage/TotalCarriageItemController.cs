using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

namespace Carriage
{
    public class TotalCarriageItemController : MonoBehaviour
    {
        public TotalCarriageListController m_TotalCarriageListController;

        public CarriageCultureController m_StoredCarriageInfo;

        public UISprite HeadDecoSprite;
        public UISprite HeadSprite;
        public UISprite HeadQualitySprite;
        public UILabel LevelLabel;
        public UILabel KingNameLabel;
        public UILabel AllianceNameLabel;
        public UIProgressBar Bar;
        public UILabel BarNumLabel;
        public UILabel ProgressLabel;
        public UILabel MoneyLabel;
        public UILabel RecommandedLabel;
        public GameObject HelpBTNObject;

        public void SetThis(CarriageCultureController info)
        {
            m_StoredCarriageInfo = info;

            SetThis();
        }

        private void SetThis()
        {
            HeadSprite.spriteName = "horseIcon" + m_StoredCarriageInfo.HorseLevel;
            HeadQualitySprite.spriteName = "pinzhi" + HeadIconSetter.horseIconToQualityTransferDic[m_StoredCarriageInfo.HorseLevel];
            if (m_StoredCarriageInfo.IsChouRen)
            {
                HeadDecoSprite.spriteName = "Enemy";
                HeadDecoSprite.gameObject.SetActive(true);

                KingNameLabel.color = Color.red;
            }
            else if (m_StoredCarriageInfo.KingName == JunZhuData.Instance().m_junzhuInfo.name)
            {
                HeadDecoSprite.spriteName = "Self";
                HeadDecoSprite.gameObject.SetActive(true);

                KingNameLabel.color = Color.black;
            }
            else
            {
                HeadDecoSprite.gameObject.SetActive(false);

                KingNameLabel.color = Color.black;
            }
            if (!string.IsNullOrEmpty(m_StoredCarriageInfo.AllianceName) && !AllianceData.Instance.IsAllianceNotExist && m_StoredCarriageInfo.AllianceName == AllianceData.Instance.g_UnionInfo.name && m_StoredCarriageInfo.KingName != JunZhuData.Instance().m_junzhuInfo.name)
            {
                if (RootManager.Instance.m_CarriageMain.m_IHelpOtherJunzhuIdList.Contains(m_StoredCarriageInfo.JunzhuID))
                {
                    HelpBTNObject.GetComponent<UISprite>().color = Color.grey;
                }
                else
                {
                    HelpBTNObject.GetComponent<UISprite>().color = Color.white;
                }
                HelpBTNObject.SetActive(true);
            }
            else
            {
                HelpBTNObject.SetActive(false);
            }
            KingNameLabel.text = string.IsNullOrEmpty(m_StoredCarriageInfo.KingName) ? "" : m_StoredCarriageInfo.KingName;
            AllianceNameLabel.text = (string.IsNullOrEmpty(m_StoredCarriageInfo.AllianceName) || m_StoredCarriageInfo.AllianceName == "***") ? LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT) : ("<" + m_StoredCarriageInfo.AllianceName + ">");
            LevelLabel.text = "Lv" + m_StoredCarriageInfo.Level;
            Bar.value = m_StoredCarriageInfo.RemainingBlood / m_StoredCarriageInfo.TotalBlood;
            BarNumLabel.text = m_StoredCarriageInfo.RemainingBlood + "/" + m_StoredCarriageInfo.TotalBlood;
            ProgressLabel.text = "进度" + m_StoredCarriageInfo.ProgressPercent + "%";
            MoneyLabel.text = CarriageValueCalctor.GetRealValueOfCarriage(m_StoredCarriageInfo.Money, m_StoredCarriageInfo.Level, m_StoredCarriageInfo.BattleValue, m_StoredCarriageInfo.HorseLevel, m_StoredCarriageInfo.IsChouRen).ToString();

            //Set recommanded.
            if (m_StoredCarriageInfo.IsRecommandedOne)
            {
                RecommandedLabel.text = "建议攻打";
                RecommandedLabel.gameObject.SetActive(true);
            }
            else
            {
                RecommandedLabel.gameObject.SetActive(false);
            }
        }

        public void OnNavigateClick()
        {
            if (RootManager.Instance.m_SelfPlayerController != null && RootManager.Instance.m_SelfPlayerCultureController != null && RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(m_StoredCarriageInfo.UID))
            {
                //Cancel chase.
                RootManager.Instance.m_CarriageMain.TryCancelChaseToAttack();

                RootManager.Instance.m_SelfPlayerController.StartNavigation(RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic[m_StoredCarriageInfo.UID].transform.position);

                m_TotalCarriageListController.OnCloseWindowClick();
                RootManager.Instance.m_CarriageMain.m_MapController.OnCloseBigMap();
            }
        }

        public void OnHelpClick()
        {
            if (RootManager.Instance.m_CarriageMain.m_IHelpOtherJunzhuIdList.Contains(m_StoredCarriageInfo.JunzhuID))
            {
                ClientMain.m_UITextManager.createText("您已在该镖马的护送列表内");
                return;
            }

            AnswerYaBiaoHelpReq tempMsg = new AnswerYaBiaoHelpReq
            {
                ybUid = m_StoredCarriageInfo.UID,
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

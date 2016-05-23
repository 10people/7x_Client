using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class BaizhanDetailController : DetailController
    {
        public BaiZhanInfo m_BaizhanInfo;

        public UISprite NationSprite;
        public UISprite Top3Sprite;
        public UILabel RankLabel;

        public UILabel KingLabel;
        public UILabel AllianceLabel;

        public UILabel WinsNumLabel;

        public UILabel ShenwangLabel;

        public UISprite JunxianSprite;
        public UILabel JunxianLabel;
        public UISprite JunxianLabelSprite;
        public UILabel NoJunxianLabel;

        public void SetThis()
        {
            KingLabel.text = m_BaizhanInfo.name;
            AllianceLabel.text = (string.IsNullOrEmpty(m_BaizhanInfo.lianmeng) || (m_BaizhanInfo.lianmeng == "无")) ? "无联盟" : ("<" + m_BaizhanInfo.lianmeng + ">");

            WinsNumLabel.text = m_BaizhanInfo.winCount.ToString();

            ShenwangLabel.text = m_BaizhanInfo.weiwang.ToString();

            NationSprite.spriteName = "nation_" + m_BaizhanInfo.guojiaId;
            if (m_BaizhanInfo.rank <= 3)
            {
                Top3Sprite.gameObject.SetActive(true);
                RankLabel.gameObject.SetActive(false);
                Top3Sprite.spriteName = "rank" + m_BaizhanInfo.rank;
            }
            else
            {
                Top3Sprite.gameObject.SetActive(false);
                RankLabel.gameObject.SetActive(true);
                RankLabel.text = m_BaizhanInfo.rank.ToString();
            }

            //Check junxian empty
            if (m_BaizhanInfo.junxianRank < 0)
            {
                JunxianSprite.gameObject.SetActive(false);
                JunxianLabelSprite.gameObject.SetActive(false);
                NoJunxianLabel.gameObject.SetActive(true);

                NoJunxianLabel.text = "无军衔";
            }
            else
            {
                JunxianSprite.gameObject.SetActive(true);
                JunxianLabelSprite.gameObject.SetActive(true);
                NoJunxianLabel.gameObject.SetActive(false);

                JunxianLabel.text = m_BaizhanInfo.junxian + " " + m_BaizhanInfo.junxianRank;
                JunxianSprite.spriteName = "junxian" + m_BaizhanInfo.junxianLevel;
            }
        }

        new void OnClick()
        {
            base.OnClick();

            if (m_ModuleController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }
            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_ModuleController.m_RootController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            List<FloatButtonsController.ButtonInfo> tempList = new List<FloatButtonsController.ButtonInfo>();
            tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "查看信息", m_VoidDelegate = GetInfo });
            if (m_BaizhanInfo.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_BaizhanInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
                }
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_BaizhanInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽玩家", m_VoidDelegate = Shield });
                }
                if (m_BaizhanInfo.lianmeng != "无")
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "掠夺", m_VoidDelegate = Rob });
                }
            }

            FloatButtonsController.Initialize(tempList, true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }

        public override void GetInfo()
        {
            KingDetailInfoController.Instance.ShowKingDetailWindow(m_BaizhanInfo.junZhuId);

            m_ModuleController.ClampScrollView();
        }

        public override void AddFriend()
        {
            if (FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_BaizhanInfo.junZhuId))
            {
                ClientMain.m_UITextManager.createText("该玩家已经是您的好友！");
            }
            else
            {
                FriendOperationLayerManagerment.AddFriends((int)m_BaizhanInfo.junZhuId);
                m_ModuleController.ClampScrollView();
            }
        }

        public override void Shield()
        {
            m_ModuleController.m_RootController.ShieldName = m_BaizhanInfo.name;

            JoinToBlacklist tempMsg = new JoinToBlacklist
            {
                junzhuId = m_BaizhanInfo.junZhuId
            };
            SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
            m_ModuleController.ClampScrollView();
        }

        public override void Rob()
        {
            PlunderData.Instance.PlunderOpponent(PlunderData.Entrance.RANKLIST, m_BaizhanInfo.junZhuId);
            m_ModuleController.ClampScrollView();
        }
    }
}

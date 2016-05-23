using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class ChongLouDetailController : DetailController
    {
        public ChongLouInfo m_ChongLouInfo;

        public UISprite NationSprite;
        public UISprite Top3Sprite;
        public UILabel RankLabel;

        public UILabel KingLabel;
        public UILabel AllianceLabel;

        public UILabel LevelLabel;
        public UILabel MaxLabel;
        public UILabel TimeLabel;

        public void SetThis()
        {
            KingLabel.text = m_ChongLouInfo.name;
            AllianceLabel.text = (string.IsNullOrEmpty(m_ChongLouInfo.lianMeng) || (m_ChongLouInfo.lianMeng == "无")) ? "无联盟" : ("<" + m_ChongLouInfo.lianMeng + ">");

            LevelLabel.text = "Lv" + m_ChongLouInfo.level;
            MaxLabel.text = m_ChongLouInfo.layer + "层";
            TimeLabel.text = m_ChongLouInfo.time;

            NationSprite.spriteName = "nation_" + m_ChongLouInfo.guojiaId;
            if (m_ChongLouInfo.rank <= 3)
            {
                Top3Sprite.gameObject.SetActive(true);
                RankLabel.gameObject.SetActive(false);
                Top3Sprite.spriteName = "rank" + m_ChongLouInfo.rank;
            }
            else
            {
                Top3Sprite.gameObject.SetActive(false);
                RankLabel.gameObject.SetActive(true);
                RankLabel.text = m_ChongLouInfo.rank.ToString();
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
            if (m_ChongLouInfo.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_ChongLouInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
                }
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_ChongLouInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽玩家", m_VoidDelegate = Shield });
                }

                if (m_ChongLouInfo.lianMeng != "无")
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
            KingDetailInfoController.Instance.ShowKingDetailWindow(m_ChongLouInfo.junZhuId);

            m_ModuleController.ClampScrollView();
        }

        public override void AddFriend()
        {
            if (FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_ChongLouInfo.junZhuId))
            {
                ClientMain.m_UITextManager.createText("该玩家已经是您的好友！");
            }
            else
            {
                FriendOperationLayerManagerment.AddFriends((int)m_ChongLouInfo.junZhuId);
                m_ModuleController.ClampScrollView();
            }
        }

        public override void Shield()
        {
            m_ModuleController.m_RootController.ShieldName = m_ChongLouInfo.name;

            JoinToBlacklist tempMsg = new JoinToBlacklist
            {
                junzhuId = m_ChongLouInfo.junZhuId
            };
            SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
            m_ModuleController.ClampScrollView();
        }

        public override void Rob()
        {
            PlunderData.Instance.PlunderOpponent(PlunderData.Entrance.RANKLIST, m_ChongLouInfo.junZhuId);
            m_ModuleController.ClampScrollView();
        }
    }
}

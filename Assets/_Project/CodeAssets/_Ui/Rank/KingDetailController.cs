using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class KingDetailController : DetailController
    {
        public JunZhuInfo m_JunZhuInfo;

        public UISprite NationSprite;
        public UISprite Top3Sprite;
        public UILabel RankLabel;

        public UILabel KingLabel;
        public UILabel AllianceLabel;

        public UILabel LevelLabel;

        public UILabel BattleNumLabel;

        public UISprite JunxianSprite;
        public UILabel JunxianLabel;
        public UISprite JunxianLabelSprite;
        public UILabel NoJunxianLabel;

        public void SetThis()
        {
            KingLabel.text = m_JunZhuInfo.name;
            AllianceLabel.text = (m_JunZhuInfo.lianMeng == "无") ? "无联盟" : ("<" + m_JunZhuInfo.lianMeng + ">");

            LevelLabel.text = "Lv" + m_JunZhuInfo.level;

            BattleNumLabel.text = m_JunZhuInfo.zhanli.ToString();

            NationSprite.spriteName = "nation_" + m_JunZhuInfo.guojiaId;
            if (m_JunZhuInfo.rank <= 3)
            {
                Top3Sprite.gameObject.SetActive(true);
                RankLabel.gameObject.SetActive(false);
                Top3Sprite.spriteName = "rank" + m_JunZhuInfo.rank;
            }
            else
            {
                Top3Sprite.gameObject.SetActive(false);
                RankLabel.gameObject.SetActive(true);
                RankLabel.text = m_JunZhuInfo.rank.ToString();
            }

            //Check junxian empty
            if (m_JunZhuInfo.junxianRank < 0)
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

                JunxianLabel.text = m_JunZhuInfo.junxian + " " + m_JunZhuInfo.junxianRank;
                JunxianSprite.spriteName = "junxian" + m_JunZhuInfo.junxianLevel;
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
            if (m_JunZhuInfo.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_JunZhuInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
                }
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_JunZhuInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽玩家", m_VoidDelegate = Shield });
                }

                if (m_JunZhuInfo.lianMeng != "无")
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
            JunZhuInfoSpecifyReq temp = new JunZhuInfoSpecifyReq()
            {
                junzhuId = m_JunZhuInfo.junZhuId
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ);
            m_ModuleController.ClampScrollView();
        }

        public override void AddFriend()
        {
            m_ModuleController.m_RootController.AddFriendName = m_JunZhuInfo.name;

            FriendOperationLayerManagerment.AddFriends((int)m_JunZhuInfo.junZhuId);
            m_ModuleController.ClampScrollView();
        }

        public override void Shield()
        {
            m_ModuleController.m_RootController.ShieldName = m_JunZhuInfo.name;

            JoinToBlacklist tempMsg = new JoinToBlacklist
            {
                junzhuId = m_JunZhuInfo.junZhuId
            };
            SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
            m_ModuleController.ClampScrollView();
        }

        public override void Rob()
        {
            LueDuoData.Instance.LueDuoOpponentReq(m_JunZhuInfo.junZhuId, LueDuoData.WhichOpponent.RANKLIST);
            m_ModuleController.ClampScrollView();
        }
    }
}

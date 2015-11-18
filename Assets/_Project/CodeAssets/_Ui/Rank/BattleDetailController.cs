using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class BattleDetailController : DetailController
    {
        public GuoGuanInfo m_BattleInfo;

        public UISprite NationSprite;
        public UISprite Top3Sprite;
        public UILabel RankLabel;

        public UILabel KingLabel;
        public UILabel AllianceLabel;

        public UILabel NormalLabel;

        public UILabel ChuanqiLabel;

        public UILabel StarLabel;

        public void SetThis()
        {
            KingLabel.text = m_BattleInfo.name;
            AllianceLabel.text = (m_BattleInfo.lianmeng == "无") ? "无联盟" : ("<" + m_BattleInfo.lianmeng + ">");

            //Remove first 0 char in battle.
            var putongStr = m_BattleInfo.putong;
            while (!string.IsNullOrEmpty(putongStr) && putongStr[0] == '0')
            {
                putongStr = putongStr.Remove(0, 1);
            }
            NormalLabel.text = putongStr;

            var chuanqiStr = m_BattleInfo.chuanqi;
            while (!string.IsNullOrEmpty(chuanqiStr) && chuanqiStr[0] == '0')
            {
                chuanqiStr = chuanqiStr.Remove(0, 1);
            }
            //Check empty string.
            ChuanqiLabel.text = string.IsNullOrEmpty(chuanqiStr) ? "无" : chuanqiStr;

            NationSprite.spriteName = "nation_" + m_BattleInfo.guojiaId;
            if (m_BattleInfo.rank <= 3)
            {
                Top3Sprite.gameObject.SetActive(true);
                RankLabel.gameObject.SetActive(false);
                Top3Sprite.spriteName = "rank" + m_BattleInfo.rank;
            }
            else
            {
                Top3Sprite.gameObject.SetActive(false);
                RankLabel.gameObject.SetActive(true);
                RankLabel.text = m_BattleInfo.rank.ToString();
            }

            StarLabel.text = m_BattleInfo.starCount.ToString();
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

            if (m_BattleInfo.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_BattleInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
                }
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_BattleInfo.junZhuId))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽玩家", m_VoidDelegate = Shield });
                }
                if (m_BattleInfo.lianmeng != "无")
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
                junzhuId = m_BattleInfo.junZhuId
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ);
            m_ModuleController.ClampScrollView();
        }

        public override void AddFriend()
        {
            m_ModuleController.m_RootController.AddFriendName = m_BattleInfo.name;

            FriendOperationLayerManagerment.AddFriends((int)m_BattleInfo.junZhuId);
            m_ModuleController.ClampScrollView();
        }

        public override void Shield()
        {
            m_ModuleController.m_RootController.ShieldName = m_BattleInfo.name;

            JoinToBlacklist tempMsg = new JoinToBlacklist
            {
                junzhuId = m_BattleInfo.junZhuId
            };
            SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
            m_ModuleController.ClampScrollView();
        }

        public override void Rob()
        {
            LueDuoData.Instance.LueDuoOpponentReq(m_BattleInfo.junZhuId, LueDuoData.WhichOpponent.RANKLIST);
            m_ModuleController.ClampScrollView();
        }
    }
}

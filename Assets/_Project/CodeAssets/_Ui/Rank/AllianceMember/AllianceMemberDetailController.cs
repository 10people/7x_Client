using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class AllianceMemberDetailController : DetailController
    {
        public AllianceMemberController m_AllianceMemberController;

        public JunZhuInfo m_JunZhuInfo;

        public UILabel KingLabel;
        public UILabel JobLabel;
        public UILabel LevelLabel;
        public UILabel GongxianLabel;
        public UILabel BattlevalueLabel;
        public UILabel GongjinLabel;

        public void SetThis()
        {
            KingLabel.text = m_JunZhuInfo.name;

            switch (m_JunZhuInfo.job)
            {
                case 0:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_CHENGYUAN);
                    break;
                case 1:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_FU_LEADER);
                    break;
                case 2:
                    JobLabel.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_MEMBER_LEADER);
                    break;
                default:
                    Debug.LogError("Error job index in AllianceMemberDetailController.");
                    break;
            }

            LevelLabel.text = "Lv" + m_JunZhuInfo.level;
            GongxianLabel.text = m_JunZhuInfo.gongxian.ToString();
            BattlevalueLabel.text = m_JunZhuInfo.zhanli.ToString();
            GongjinLabel.text = m_JunZhuInfo.gongjin.ToString();
        }

        new void OnClick()
        {
            base.OnClick();
            if (m_AllianceMemberController != null)
            {
                m_AllianceMemberController.m_AllianceMemberDetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }

            if (m_AllianceMemberController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_AllianceMemberController.m_RootController.FloatButtonPrefab);
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

        public new IEnumerator AdjustFloatButton()
        {
            yield return new WaitForEndOfFrame();

            //Cancel adjust cause multi touch may destroy this float buttons gameobject.
            if (FloatButtonsController == null || FloatButtonsController.gameObject == null)
            {
                yield break;
            }

			NGUIHelper.AdaptWidgetInScrollView(m_AllianceMemberController.m_ScrollView, m_AllianceMemberController.m_ScrollBar, FloatButtonsController.m_BGLeft.GetComponent<UIWidget>());
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
            m_AllianceMemberController.m_RootController.AddFriendName = m_JunZhuInfo.name;

            FriendOperationLayerManagerment.AddFriends((int)m_JunZhuInfo.junZhuId);
            m_ModuleController.ClampScrollView();
        }

        public override void Shield()
        {
            m_AllianceMemberController.m_RootController.ShieldName = m_JunZhuInfo.name;

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

        /// <summary>
        /// use new destroy all float buttons.
        /// </summary>
        /// <param name="isPress"></param>
        new void OnPress(bool isPress)
        {
            base.OnPress(isPress);

            if (m_AllianceMemberController != null)
            {
                m_AllianceMemberController.m_AllianceMemberDetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }
        }
    }
}

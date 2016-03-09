using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

namespace Rank
{
    public class AllianceDetailController : DetailController
    {
        public LianMengInfo m_AllianceInfo;

        public UISprite NationSprite;
        public UISprite Top3Sprite;
        public UILabel RankLabel;

        public UILabel AllianceLabel;

        public UILabel LevelLabel;

        public UILabel ShenwangLabel;

        public UILabel NumberLabel;

        public void SetThis()
        {
            AllianceLabel.text = "<" + m_AllianceInfo.mengName + ">";

            LevelLabel.text = "Lv" + m_AllianceInfo.level;

            ShenwangLabel.text = m_AllianceInfo.shengWang.ToString();

            NationSprite.spriteName = "nation_" + m_AllianceInfo.guoJiaId;
            if (m_AllianceInfo.rank <= 3)
            {
                Top3Sprite.gameObject.SetActive(true);
                RankLabel.gameObject.SetActive(false);
                Top3Sprite.spriteName = "rank" + m_AllianceInfo.rank;
            }
            else
            {
                Top3Sprite.gameObject.SetActive(false);
                RankLabel.gameObject.SetActive(true);
                RankLabel.text = m_AllianceInfo.rank.ToString();
            }

            NumberLabel.text = m_AllianceInfo.member + "/" + m_AllianceInfo.allMember;
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

            FloatButtonsController.Initialize(tempList, true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }

        public override void GetInfo()
        {
            AllianceMemberWindowManager.Instance.OpenAllianceMemberWindowInRank(m_AllianceInfo.mengId, m_AllianceInfo.mengName, m_ModuleController.m_RootController, "UseInRank");
        }

        public override void AddFriend()
        {

        }

        public override void Shield()
        {

        }

        public override void Rob()
        {

        }
    }
}

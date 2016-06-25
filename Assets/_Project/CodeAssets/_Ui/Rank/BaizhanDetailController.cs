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

            SetBG(false);
        }

        new void OnClick()
        {
            base.OnClick();

            if (m_ModuleController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }

            m_ModuleController.m_RootController.ShieldName = m_BaizhanInfo.name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_ModuleController.m_RootController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_BaizhanInfo.junZhuId, m_BaizhanInfo.name, m_BaizhanInfo.lianmeng, new List<GameObject>() { m_ModuleController.m_RootController.gameObject }, m_ModuleController.ClampScrollView), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }
    }
}

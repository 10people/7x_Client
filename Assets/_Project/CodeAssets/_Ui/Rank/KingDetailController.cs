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
            AllianceLabel.text = (string.IsNullOrEmpty(m_JunZhuInfo.lianMeng) || (m_JunZhuInfo.lianMeng == "无")) ? "无联盟" : ("<" + m_JunZhuInfo.lianMeng + ">");

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
            if (m_JunZhuInfo.junxianRank <= 0)
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

            SetBG(false);
        }

        new void OnClick()
        {
            base.OnClick();

            if (m_ModuleController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }

            m_ModuleController.m_RootController.ShieldName = m_JunZhuInfo.name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_ModuleController.m_RootController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_JunZhuInfo.junZhuId, m_JunZhuInfo.name, m_JunZhuInfo.lianMeng, new List<GameObject>() { m_ModuleController.m_RootController.gameObject }, m_ModuleController.ClampScrollView), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }
    }
}

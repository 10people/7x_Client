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

            SetBG(false);
        }

        new void OnClick()
        {
            base.OnClick();

            if (m_ModuleController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }

            m_ModuleController.m_RootController.ShieldName = m_ChongLouInfo.name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_ModuleController.m_RootController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_ChongLouInfo.junZhuId, m_ChongLouInfo.name, m_ChongLouInfo.lianMeng, new List<GameObject>() { m_ModuleController.m_RootController.gameObject }, m_ModuleController.ClampScrollView), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }
    }
}

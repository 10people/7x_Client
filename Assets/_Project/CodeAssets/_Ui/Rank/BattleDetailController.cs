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
            AllianceLabel.text = (string.IsNullOrEmpty(m_BattleInfo.lianmeng) || (m_BattleInfo.lianmeng == "无")) ? "无联盟" : ("<" + m_BattleInfo.lianmeng + ">");

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

            SetBG(false);
        }

        new void OnClick()
        {
            base.OnClick();

            if (m_ModuleController.m_RootController.FloatButtonPrefab == null)
            {
                return;
            }

            m_ModuleController.m_RootController.ShieldName = m_BattleInfo.name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_ModuleController.m_RootController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_BattleInfo.junZhuId, m_BattleInfo.name, m_BattleInfo.lianmeng, new List<GameObject>() { m_ModuleController.m_RootController.gameObject }, m_ModuleController.ClampScrollView), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }
    }
}

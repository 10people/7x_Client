using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABScoreItemController : MonoBehaviour
    {
        public ABScoreWindowController m_AbScoreWindowController;

        public struct ScoreData
        {
            public int Rank;
            public string Name;
            public string AllianceName;
            public int Score;
            public int TotalKill;
            public int ComboKill;
            public long ID;
            public int RoleID;
            public int GongXun;
        }

        public ScoreData m_ScoreData;

        public UILabel RankLabel;
        public UILabel NameLabel;
        public UILabel ScoreLabel;
        public UILabel TotalKilLabel;
        public UILabel ComboLabel;
        public UILabel GongxunLabel;

        public void SetThis()
        {
            RankLabel.text = m_ScoreData.Rank.ToString();
            if (JunZhuData.Instance().m_junzhuInfo.name == m_ScoreData.Name)
            {
                NameLabel.text = ColorTool.Color_Green_00ff00 + m_ScoreData.Name + "[-]";
            }
            else
            {
                NameLabel.text = m_ScoreData.Name;
            }
            ScoreLabel.text = m_ScoreData.Score.ToString();
            TotalKilLabel.text = m_ScoreData.TotalKill.ToString();
            ComboLabel.text = m_ScoreData.ComboKill.ToString();
            GongxunLabel.text = m_ScoreData.GongXun.ToString();
        }

        public void OnClick()
        {
            if (m_AbScoreWindowController != null)
            {
                m_AbScoreWindowController.m_ScoreItemList.ForEach(item => item.DestroyFloatButtons());
            }

            if (m_AbScoreWindowController == null || m_AbScoreWindowController.FloatButtonPrefab == null)
            {
                return;
            }

            m_AbScoreWindowController.ShieldName = m_ScoreData.Name;

            //Create object and set.
            GameObject tempObject = (GameObject)Instantiate(m_AbScoreWindowController.FloatButtonPrefab);
            FloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

            FloatButtonsController.Initialize(FloatButtonsConfig.GetConfig(m_ScoreData.ID, m_ScoreData.Name, m_ScoreData.AllianceName, new List<GameObject>() { m_AbScoreWindowController.gameObject }, m_AbScoreWindowController.ClampScrollView, true, true, true, false, true, false), true);

            TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, tempObject.transform);

            StartCoroutine(AdjustFloatButton());
        }

        public GameObject FloatButtonsRoot;
        public FloatButtonsController FloatButtonsController;

        public void DestroyFloatButtons()
        {
            if (FloatButtonsController != null)
            {
                Destroy(FloatButtonsController.gameObject);
                FloatButtonsController = null;
            }
        }

        public IEnumerator AdjustFloatButton()
        {
            yield return new WaitForEndOfFrame();

            //Cancel adjust cause multi touch may destroy this float buttons gameobject.
            if (FloatButtonsController == null || FloatButtonsController.gameObject == null)
            {
                yield break;
            }

            NGUIHelper.AdaptWidgetInScrollView(m_AbScoreWindowController.ScoreScrollView, m_AbScoreWindowController.ScoreScrollBar, FloatButtonsController.m_BG.GetComponent<UIWidget>());
        }
    }
}

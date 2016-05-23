using UnityEngine;
using System.Collections;

public class ABScoreItemController : MonoBehaviour
{
    public struct ScoreData
    {
        public int Rank;
        public string Name;
        public int Score;
        public int TotalKill;
        public int ComboKill;
    }

    public ScoreData m_ScoreData;

    public UILabel RankLabel;
    public UILabel NameLabel;
    public UILabel ScoreLabel;
    public UILabel TotalKilLabel;
    public UILabel ComboLabel;

    public void SetThis()
    {
        RankLabel.text = m_ScoreData.Rank.ToString();
        NameLabel.text = m_ScoreData.Name;
        ScoreLabel.text = m_ScoreData.Score.ToString();
        TotalKilLabel.text = m_ScoreData.TotalKill.ToString();
        ComboLabel.text = m_ScoreData.ComboKill.ToString();
    }
}

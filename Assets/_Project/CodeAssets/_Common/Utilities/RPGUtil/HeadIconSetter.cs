using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadIconSetter : MonoBehaviour
{
    public UISprite PlayerHeadSprite;
    public UISprite HorseHeadSprite;
    public UISprite HourseQualitySprite;
    public UISprite HpOrExpSprite;
    public UILabel LevelLabel;
    public UILabel NameLabel;
    public UIProgressBar Bar;
    public UISprite NationSprite;
    public UILabel AllianceLabel;
    public GameObject VipObject;
    public UISprite VipSprite;
    public GameObject BattleValueObject;
    public UILabel BattleValueLabel;
    public UILabel BarPrecentLabel;

    private string playerIconPrefix = "PlayerIcon";
    private string horseIconPrefix = "horseIcon";
    private string horseQualityPrefix = "pinzhi";

    public static readonly Dictionary<int, int> horseIconToQualityTransferDic = new Dictionary<int, int>()
    {
        {1, 0}, {2, 1}, {3, 3}, {4, 6}, {5, 9},
    };

    private string hpLogo = "HP";
    private string expLogo = "exp";
    private string nationLogoPrefix = "nation_";

    private string vipPrefix = "v";
    private string levelPrefix = "Lv";

    public void SetPlayer(int roleID, bool isHP, int level, string name, string allianceName, float totalValue, float currentValue, int nationID, int vipID, int battleValue, int horseLevel)
    {
        if (roleID >= 50000)
        {
            if (PlayerHeadSprite != null)
            {
                PlayerHeadSprite.gameObject.SetActive(false);
            }

            if (HorseHeadSprite != null && horseLevel > 0)
            {
                HorseHeadSprite.gameObject.SetActive(true);
                HorseHeadSprite.spriteName = horseIconPrefix + horseLevel;
            }

            if (HourseQualitySprite != null)
            {
                if (horseIconToQualityTransferDic.ContainsKey(horseLevel))
                {
                    HourseQualitySprite.gameObject.SetActive(true);
                    HourseQualitySprite.spriteName = horseQualityPrefix + horseIconToQualityTransferDic[horseLevel];
                }
                else
                {
                    HourseQualitySprite.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (PlayerHeadSprite != null)
            {
                PlayerHeadSprite.gameObject.SetActive(true);
                PlayerHeadSprite.spriteName = playerIconPrefix + roleID;
            }

            if (HorseHeadSprite != null)
            {
                HorseHeadSprite.gameObject.SetActive(false);
            }
        }

        if (HpOrExpSprite != null)
        {
            HpOrExpSprite.spriteName = isHP ? hpLogo : expLogo;
        }

        if (NameLabel != null)
        {
            NameLabel.text = name;
        }

        if (LevelLabel != null)
        {
            LevelLabel.text = levelPrefix + level;
        }

        TotalValue = totalValue;
        UpdateBar(currentValue);

        if (NationSprite != null)
        {
            NationSprite.spriteName = nationLogoPrefix + nationID;
        }

        if (AllianceLabel != null)
        {
            AllianceLabel.text = (string.IsNullOrEmpty(allianceName) || allianceName == "***") ? "无联盟" : allianceName;
        }

        if (VipObject != null)
        {
            if (vipID > 0 && VipSprite != null)
            {
                VipSprite.spriteName = vipPrefix + vipID;
                VipObject.SetActive(true);
            }
            else
            {
                VipObject.SetActive(false);
            }
        }

        if (BattleValueLabel != null && BattleValueObject != null)
        {
            if (battleValue > 0)
            {
                BattleValueObject.SetActive(true);
                BattleValueLabel.text = battleValue.ToString();
            }
            else
            {
                BattleValueObject.SetActive(false);
            }
        }
    }

    public float TotalValue = -1;
    public float CurrentValue = -1;

    public void UpdateBar(float val)
    {
        CurrentValue = val;

        if (CurrentValue / TotalValue >= 0 && CurrentValue / TotalValue <= 1)
        {
            if (Bar != null)
            {
                Bar.value = CurrentValue / TotalValue;
                Bar.ForceUpdate();
            }

            if (BarPrecentLabel != null)
            {
                BarPrecentLabel.text = (CurrentValue / TotalValue * 100).ToString("n1") + "%";
            }
        }
    }
}

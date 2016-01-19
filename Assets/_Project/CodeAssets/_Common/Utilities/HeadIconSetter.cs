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
    public UILabel VipLabel;
    public UILabel BattleValueLabel;

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

    private string vipPrefix = "VIP";
    private string levelPrefix = "Lv";

    public void SetPlayer(int roleID, bool isHP, int level, string name, string allianceName, float totalValue, float currentValue, int nationID, int vipID, int battleValue)
    {
        PlayerHeadSprite.gameObject.SetActive(true);
        HorseHeadSprite.gameObject.SetActive(false);
        PlayerHeadSprite.spriteName = playerIconPrefix + roleID;
        HpOrExpSprite.spriteName = isHP ? hpLogo : expLogo;
        NameLabel.text = name;
        LevelLabel.text = levelPrefix + level;

        TotalValue = totalValue;
        UpdateBar(currentValue);

        NationSprite.spriteName = nationLogoPrefix + nationID;
        AllianceLabel.text = (string.IsNullOrEmpty(allianceName) || allianceName == "***") ? "无联盟" : allianceName;
        VipLabel.text = vipPrefix + vipID;
        BattleValueLabel.text = battleValue.ToString();
    }

    public void SetHorse(int horseLevel, bool isHP, int level, string name, string allianceName, float totalValue, float currentValue, int nationID, int vipID, int battleValue)
    {
        PlayerHeadSprite.gameObject.SetActive(false);
        HorseHeadSprite.gameObject.SetActive(true);
        HorseHeadSprite.spriteName = horseIconPrefix + horseLevel;
        HourseQualitySprite.spriteName = horseQualityPrefix + horseIconToQualityTransferDic[horseLevel];
        HpOrExpSprite.spriteName = isHP ? hpLogo : expLogo;
        NameLabel.text = name;
        LevelLabel.text = levelPrefix + level;

        TotalValue = totalValue;
        UpdateBar(currentValue);

        NationSprite.spriteName = nationLogoPrefix + nationID;
        AllianceLabel.text = (string.IsNullOrEmpty(allianceName) || allianceName == "***") ? "无联盟" : allianceName;
        VipLabel.text = vipPrefix + vipID;
        BattleValueLabel.text = battleValue.ToString();
    }

    public float TotalValue = -1;
    public float CurrentValue = -1;

    public void UpdateBar(float val)
    {
        CurrentValue = val;

        if (CurrentValue / TotalValue >= 0 && CurrentValue / TotalValue <= 1)
        {
            Bar.value = CurrentValue / TotalValue;
            Bar.ForceUpdate();
        }
    }
}

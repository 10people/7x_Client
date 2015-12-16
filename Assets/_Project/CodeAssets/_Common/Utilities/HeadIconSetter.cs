using UnityEngine;
using System.Collections;

public class HeadIconSetter : MonoBehaviour
{
    public UISprite HeadSprite;
    public UISprite HpOrExpSprite;
    public UILabel LevelLabel;
    public UILabel NameLabel;
    public UIProgressBar Bar;
    public UISprite NationSprite;
    public UILabel AllianceLabel;
    public UILabel VipLabel;
    public UILabel BattleValueLabel;

    private string playerIconPrefix = "PlayerIcon";
    private string hpLogo = "HP";
    private string expLogo = "exp";
    private string nationLogoPrefix = "nation_";

    private string vipPrefix = "VIP";
    private string levelPrefix = "Lv";

    public void SetThis(int roleID, bool isHP, int level, string name, string allianceName, float totalValue, float currentValue, int nationID, int vipID, int battleValue)
    {
        HeadSprite.spriteName = playerIconPrefix + roleID;
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
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadIconSetter : MonoBehaviour
{
    public UISprite PlayerHeadSprite;
    public UISprite HorseHeadSprite;
    public UISprite HoldPointHeadSprite;
    public UISprite HorseQualitySprite;
    public UISprite HpOrExpSprite;
    public UILabel LevelLabel;
    public UILabel NameLabel;
    public UIProgressBar Bar;
    public UISprite BarSprite;
    public UISprite NationSprite;
    public UILabel AllianceLabel;
    public GameObject VipObject;
    public UISprite VipSprite;
    public GameObject BattleValueObject;
    public UILabel BattleValueLabel;
    public UILabel BarPrecentLabel;

    private string playerIconPrefix = "PlayerIcon";
    private string horseIconPrefix = "horseIcon";
    private string holdPointIconPrefix = "HoldIcon";
    private string horseQualityPrefix = "pinzhi";

    public static readonly Dictionary<int, int> horseIconToQualityTransferDic = new Dictionary<int, int>()
    {
        {1, 0}, {2, 1}, {3, 3}, {4, 6}, {5, 9},
    };

    private string hpLogo = "HP";
    private string expLogo = "exp";
    private string hpBarSprite = "Blood";
    private string expBarSprite = "Exp";
    private string nationLogoPrefix = "nation_";

    private string vipPrefix = "v";
    public string levelPrefix = "";

    public void SetPlayer(int roleID, bool isHP, int level, string name, string allianceName, float totalValue, float currentValue, int nationID, int vipID, int battleValue, int horseLevel, string holdPointName = null)
    {
        if (roleID >= 100000)
        {
            if (PlayerHeadSprite != null)
            {
                PlayerHeadSprite.gameObject.SetActive(false);
            }

            if (HorseHeadSprite != null)
            {
                HorseHeadSprite.gameObject.SetActive(false);
            }

            if (HoldPointHeadSprite != null && !string.IsNullOrEmpty(holdPointName))
            {
                HoldPointHeadSprite.gameObject.SetActive(true);
                HoldPointHeadSprite.spriteName = holdPointIconPrefix + holdPointName;
            }
        }
        else if (roleID >= 50000)
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

            if (HorseQualitySprite != null)
            {
                if (horseIconToQualityTransferDic.ContainsKey(horseLevel))
                {
                    HorseQualitySprite.gameObject.SetActive(true);
                    HorseQualitySprite.spriteName = horseQualityPrefix + horseIconToQualityTransferDic[horseLevel];
                }
                else
                {
                    HorseQualitySprite.gameObject.SetActive(false);
                }
            }

            if (HoldPointHeadSprite != null)
            {
                HoldPointHeadSprite.gameObject.SetActive(false);
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

            if (HoldPointHeadSprite != null)
            {
                HoldPointHeadSprite.gameObject.SetActive(false);
            }
        }

        if (HpOrExpSprite != null)
        {
            HpOrExpSprite.spriteName = isHP ? hpLogo : expLogo;
        }

        if (BarSprite != null)
        {
            BarSprite.spriteName = isHP ? hpBarSprite : expBarSprite;
        }

        if (NameLabel != null)
        {
            NameLabel.text = name;
        }

        if (LevelLabel != null && level > 0)
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

        RefreshVIP(vipID);

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

    public void RefreshVIP(int vipID)
    {
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
                BarPrecentLabel.text = ((CurrentValue / TotalValue * 100 > 0 && CurrentValue / TotalValue * 100 < 1) ? 1 : (CurrentValue / TotalValue * 100)).ToString("n0") + "%";
            }

            if (BarSprite != null)
            {
                BarSprite.fillAmount = CurrentValue / TotalValue;
            }
        }
    }
}

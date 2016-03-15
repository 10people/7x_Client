using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageCultureController : CarriageBaseCultureController
    {
        public UILabel MoneyLabel;
        public UILabel ProgressLabel;
        public UILabel StateLabel1;
        public UILabel StateLabel2;
        public UISprite HorseLevelSprite;
        public UISprite QualitySprite;

        public int ProgressPercent;
        public int ProtectStateRemaining;
        public int SpeedStateRemaining;

        public bool IsRecommandedOne = false;

        public void UpdateInfo()
        {
            //ProgressLabel.text = "进度" + ProgressPercent + "%";
            //HorseLevelSprite.spriteName = "horseIcon" + HorseLevel;
            //QualitySprite.spriteName = "pinzhi" + HeadIconSetter.horseIconToQualityTransferDic[HorseLevel];

            StateLabel1.gameObject.SetActive(false);
            StateLabel2.gameObject.SetActive(false);

            if (SpeedStateRemaining > 0)
            {
                StateLabel1.text = ColorTool.Color_Gold_ffb12a + "加速" + "[-]" + ColorTool.Color_Red_c40000 + SpeedStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                StateLabel1.gameObject.SetActive(true);
            }

            if (ProtectStateRemaining > 0)
            {
                var temp = !StateLabel1.gameObject.activeInHierarchy ? StateLabel1 : StateLabel2;
                temp.text = ColorTool.Color_Gold_ffb12a + "保护" + "[-]" + ColorTool.Color_Red_c40000 + ProtectStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                temp.gameObject.SetActive(true);
            }
        }

        public static readonly Dictionary<int, string> HorseLevelToColorDic = new Dictionary<int, string>()
        {
            {1, ColorTool.Color_White_ffffff},
            {2, ColorTool.Color_Green_00ff00},
            {3, ColorTool.Color_Blue_016bc5},
            {4, ColorTool.Color_Purple_cb02d8},
            {5, ColorTool.Color_Orange_ff7f00},
        };

        public override void SetThis()
        {
            base.SetThis();

            NameLabel.text = (string.IsNullOrEmpty(KingName) ? "" : MyColorData.getColorString(9, "[b]" + KingName + "的" + "[/b]")) + (HorseLevelToColorDic.ContainsKey(HorseLevel) ? (HorseLevelToColorDic[HorseLevel] + "[b]" + "镖马" + "[/b][-]") : "");
            LevelLabel.text = Level.ToString();
            MoneyLabel.text = "+" + CarriageValueCalctor.GetRealValueOfCarriage(Money, Level, BattleValue, HorseLevel, IsChouRen);

            //Play walking particle only when horse level major to 1.
            if (HorseLevel > 1)
            {
                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(600216 + HorseLevel), gameObject, null);
            }
        }
    }
}

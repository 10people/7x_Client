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
        public UILabel StateLabel;
        public UISprite HorseLevelSprite;

        public int ProgressPercent;
        public int ProtectStateRemaining;
        public int SpeedStateRemaining;

        public void UpdateInfo()
        {
            ProgressLabel.text = "进度" + ProgressPercent + "%";
            HorseLevelSprite.spriteName = "horseIcon" + HorseLevel;

            if (ProtectStateRemaining > 0)
            {
                StateLabel.text = ColorTool.Color_Gold_ffb12a + "保护" + "[-]" + ColorTool.Color_Red_c40000 + ProtectStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                StateLabel.gameObject.SetActive(true);
            }
            else if (SpeedStateRemaining > 0)
            {
                StateLabel.text = ColorTool.Color_Gold_ffb12a + "加速" + "[-]" + ColorTool.Color_Red_c40000 + SpeedStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                StateLabel.gameObject.SetActive(true);
            }
            else
            {
                StateLabel.gameObject.SetActive(false);
            }
        }

        public override void SetThis()
        {
            base.SetThis();

            LevelLabel.text = "Lv" + Level;
            MoneyLabel.text = "+" + Money;
        }
    }
}

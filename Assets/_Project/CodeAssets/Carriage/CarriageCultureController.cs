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

        public void UpdateInfo()
        {
            ProgressLabel.text = "进度" + ProgressPercent + "%";
            HorseLevelSprite.spriteName = "horseIcon" + HorseLevel;
            QualitySprite.spriteName = "pinzhi" + HeadIconSetter.horseIconToQualityTransferDic[HorseLevel];

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

        public override void SetThis()
        {
            base.SetThis();

            LevelLabel.text = "Lv" + Level;
            MoneyLabel.text = "+" + Money;
        }
    }
}

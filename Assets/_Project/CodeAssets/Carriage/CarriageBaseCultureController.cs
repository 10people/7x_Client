using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageBaseCultureController : RPGBaseCultureController
    {
        public UILabel NameLabel;
        public UILabel AllianceLabel;
        public UISprite VIPSprite;
        public UILabel LevelLabel;

        public int Money;
        public int HorseLevel;
        public bool IsChouRen;

        private const string redBarName = "progressred";
        private const string greenBarName = "progressford";

        public bool IsCarriage
        {
            get { return RoleID >= 50000; }
        }

        public override void SetThis()
        {
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : greenBarName;

            base.SetThis();

            AllianceLabel.text = (string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)) : (MyColorData.getColorString(12, "<" + AllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost));
            VIPSprite.spriteName = "vip" + Vip;
        }
    }
}

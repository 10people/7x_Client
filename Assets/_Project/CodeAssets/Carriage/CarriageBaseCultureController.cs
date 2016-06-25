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
        public GameObject VIPObject;
        public UILabel LevelLabel;

        public int Money;
        public int HorseLevel;
        public bool IsChouRen;

        private const string redBarName = "progressred";
        private const string greenBarName = "progressford";

        public override void SetThis()
        {
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : greenBarName;

            base.SetThis();

            AllianceLabel.text = (string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)) : (MyColorData.getColorString(12, "<" + AllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost));

            if (Vip > 0)
            {
                VIPSprite.spriteName = "v" + Vip;
                VIPObject.SetActive(true);

                //Move name label and vip sprite to center.
                NameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
                NameLabel.alignment = NGUIText.Alignment.Center;
                NameLabel.transform.localPosition = new Vector3(VIPSprite.width, NameLabel.transform.localPosition.y, NameLabel.transform.localPosition.z);
                NameLabel.gameObject.SetActive(false);
                NameLabel.gameObject.SetActive(true);

                VIPObject.transform.localPosition = new Vector3(NameLabel.transform.localPosition.x - NameLabel.width * NameLabel.transform.localScale.x / 2f - 1.5f * VIPSprite.width, NameLabel.transform.localPosition.y, NameLabel.transform.localPosition.z);
            }
            else
            {
                VIPObject.SetActive(false);

                //Move name label to center.
                NameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
                NameLabel.alignment = NGUIText.Alignment.Center;
                NameLabel.transform.localPosition = new Vector3(0, NameLabel.transform.localPosition.y, NameLabel.transform.localPosition.z);
            }
        }
    }
}

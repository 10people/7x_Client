using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AllianceBattle;

namespace AllianceBattle
{
    public class ABPlayerCultureController : RPGBaseCultureController
    {
        public UILabel NameLabel;
        public UILabel AllianceLabel;
        public UISprite VIPSprite;
        public UILabel LevelLabel;

        private const string redBarName = "BloodRed";
        private const string greenBarName = "BloodGreen";
        private const string blueBarName = "BloodBlue";

        public void OnStartDadaoSkillFinish()
        {
            EnableMove();
        }

        public void OnDadaoSkillStart()
        {
            if (RTBuffTemplate.GetTemplateByID(151).BuffDisplay > 0)
            {
                if (UID == PlayerSceneSyncManager.Instance.m_MyselfUid)
                {
                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTBuffTemplate.GetTemplateByID(151).BuffDisplay), gameObject, null, Vector3.zero, transform.forward);
                }
                else
                {
                    if (EffectNumController.Instance.IsCanPlayEffect())
                    {
                        EffectNumController.Instance.NotifyPlayingEffect();

                        FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(RTBuffTemplate.GetTemplateByID(151).BuffDisplay), gameObject, null, Vector3.zero, transform.forward);
                    }
                }
            }
        }

        public override void SetThis()
        {
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : (IsSelf ? greenBarName : blueBarName);

            base.SetThis();

            if (string.IsNullOrEmpty(KingName))
            {
                Debug.LogError("King name null");
            }
            else
            {
                NameLabel.text = ("[b]" + (IsSelf ? ColorTool.Color_Green_00ff00 : (IsEnemy ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_016bc5)) + KingName + "[-][/b]");
            }

            LevelLabel.text = Level.ToString();

            AllianceLabel.text = (string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)) : (MyColorData.getColorString(12, "<" + AllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost));
            VIPSprite.spriteName = "vip" + Vip;
        }
    }
}


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
        public GameObject VIPObject;
        public UILabel LevelLabel;

        private const string redBarName = "BloodRed";
        private const string greenBarName = "BloodGreen";
        private const string blueBarName = "BloodBlue";

        public void OnStartDadaoSkillFinish()
        {
            EnableMove();
        }

        public DelegateHelper.VoidDelegate m_ExecuteAfterLongSkillShot;

        public void OnLongSkillShot()
        {
            if (m_ExecuteAfterLongSkillShot != null)
            {
                m_ExecuteAfterLongSkillShot();
                m_ExecuteAfterLongSkillShot = null;
            }
        }

        public bool IsCanPlayAOESkill = false;
        public float LastCheckEffectNumTime;

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
                    if (Time.realtimeSinceStartup - LastCheckEffectNumTime > 5f)
                    {
                        IsCanPlayAOESkill = EffectNumController.Instance.IsCanPlayEffect(false);

                        LastCheckEffectNumTime = Time.realtimeSinceStartup;
                    }

                    if (IsCanPlayAOESkill)
                    {
                        EffectNumController.Instance.NotifyPlayingEffect(false, 10);

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

            LevelLabel.text = (IsSelf ? ColorTool.Color_Green_00ff00 : (IsEnemy ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_016bc5)) + Level + "[-]";

            AllianceLabel.text = (IsSelf ? ColorTool.Color_Green_00ff00 : (IsEnemy ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_016bc5)) + ((string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT) : ("<" + AllianceName + ">" + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost))) + "[-]";

            //Move name label to center.
            NameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
            NameLabel.alignment = NGUIText.Alignment.Center;
            NameLabel.transform.localPosition = new Vector3(0, NameLabel.transform.localPosition.y, NameLabel.transform.localPosition.z);

            if (Vip > 0)
            {
                VIPSprite.spriteName = "v" + Vip;
                VIPObject.transform.localPosition = new Vector3(NameLabel.transform.localPosition.x - NameLabel.width * NameLabel.transform.localScale.x / 2f - 1.5f * VIPSprite.width, NameLabel.transform.localPosition.y, NameLabel.transform.localPosition.z);

                VIPObject.SetActive(true);
            }
            else
            {
                VIPObject.SetActive(false);
            }
        }

        public Transform EffectParentObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectIndex">to be at 0, 1, 2, 3, 4</param>
        public void SetBigEffect(int effectIndex, bool isAdd)
        {
            while (EffectParentObject.childCount != 0)
            {
                var child = EffectParentObject.GetChild(0);
                Destroy(child.gameObject);
                child.parent = null;
            }

            if (isAdd)
            {
                FxHelper.PlayLocalFx(EffectIdTemplate.GetPathByeffectId(620234 + effectIndex), EffectParentObject.gameObject);
            }
        }
    }
}


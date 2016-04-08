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

            if (SpeedStateRemaining > 0)
            {
                StateLabel1.text = ColorTool.Color_Gold_ffb12a + "加速" + "[-]" + ColorTool.Color_Red_c40000 + SpeedStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                StateLabel1.gameObject.SetActive(true);

                if (ProtectStateRemaining > 0)
                {
                    StateLabel2.text = ColorTool.Color_Gold_ffb12a + "保护" + "[-]" + ColorTool.Color_Red_c40000 + ProtectStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                    StateLabel2.gameObject.SetActive(true);
                }
                else
                {
                    StateLabel2.gameObject.SetActive(false);
                }
            }
            else
            {
                StateLabel2.gameObject.SetActive(false);

                if (ProtectStateRemaining > 0)
                {
                    StateLabel1.text = ColorTool.Color_Gold_ffb12a + "保护" + "[-]" + ColorTool.Color_Red_c40000 + ProtectStateRemaining + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
                    StateLabel1.gameObject.SetActive(true);
                }
                else
                {
                    StateLabel1.gameObject.SetActive(false);
                }
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

            if (string.IsNullOrEmpty(KingName) || !HorseLevelToColorDic.ContainsKey(HorseLevel))
            {
                Debug.LogError("King name null or carriage quality not identified");
            }
            else
            {
                NameLabel.text = ("[b]" + (IsSelf ? ColorTool.Color_Green_00ff00 : (IsEnemy ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_016bc5)) + KingName + "[-]" + "的" + "[/b]") + (HorseLevelToColorDic[HorseLevel] + "[b]" + "镖马" + "[/b][-]");
            }

            LevelLabel.text = Level.ToString();
            MoneyLabel.text = "+" + (IsSelf ? Money.ToString() : CarriageValueCalctor.GetRealValueOfCarriage(Money, Level, BattleValue, HorseLevel, IsChouRen).ToString());

            ShowCarriageParticle();
        }

        public void ShowCarriageParticle()
        {
            if (m_particleGameObject != null)
            {
                Debug.LogWarning("Cannot show particle cause particle existed.");
                return;
            }

            //Play walking particle only when horse level major to 1.
            if (HorseLevel > 1)
            {
                FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(600216 + HorseLevel), gameObject, OnParticleLoadCallBack);
            }
        }

        private GameObject m_particleGameObject;

        private void OnParticleLoadCallBack(GameObject fx)
        {
            m_particleGameObject = fx;
        }

        public void HideCarriageParticle()
        {
            if (m_particleGameObject != null)
            {
                Destroy(m_particleGameObject);
            }
        }
    }
}

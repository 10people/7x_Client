using UnityEngine;
using System.Collections;

namespace Carriage
{
    public class CarriagePlayerCultureController : CarriageBaseCultureController
    {
        public UISprite TitleSprite;

        public override void SetThis()
        {
            base.SetThis();

            if (string.IsNullOrEmpty(KingName))
            {
                Debug.LogError("King name null");
            }
            else
            {
                NameLabel.text = ("[b]" + (IsSelf ? ColorTool.Color_Green_00ff00 : (IsEnemy ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_016bc5)) + KingName + "[-][/b]");
            }

            //TitleSprite.spriteName = Title.ToString();
            TitleSprite.gameObject.SetActive(false);
            LevelLabel.text = Level.ToString();
        }
    }
}


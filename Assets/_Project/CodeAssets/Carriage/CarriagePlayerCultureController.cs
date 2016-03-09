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

            NameLabel.text = string.IsNullOrEmpty(KingName) ? "" : MyColorData.getColorString(9, "[b]" + KingName + "[/b]");
            TitleSprite.spriteName = Title.ToString();
            LevelLabel.text = Level.ToString();
        }
    }
}


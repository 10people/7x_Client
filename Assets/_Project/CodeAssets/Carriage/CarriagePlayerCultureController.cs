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

            TitleSprite.spriteName = Title.ToString();
            LevelLabel.text = Level.ToString();
        }
    }
}


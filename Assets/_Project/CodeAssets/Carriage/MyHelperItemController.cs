using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

namespace Carriage
{
    public class MyHelperItemController : MonoBehaviour
    {
        public UISprite HeadSprite;
        public UILabel NameLabel;
        public UIProgressBar Bar;

        public XieZhuJunZhu m_StoredXieZhuJunZhu;

        private string playerIconPrefix = "PlayerIcon";

        public void SetThis(XieZhuJunZhu tempInfo)
        {
            m_StoredXieZhuJunZhu = tempInfo;

            SetThis();
        }

        private void SetThis()
        {
            if (m_StoredXieZhuJunZhu != null)
            {
                HeadSprite.spriteName = playerIconPrefix + m_StoredXieZhuJunZhu.roleId;
                NameLabel.text = m_StoredXieZhuJunZhu.name;
                Bar.value = (float)m_StoredXieZhuJunZhu.curHp / m_StoredXieZhuJunZhu.maxHp;
            }
        }
    }
}

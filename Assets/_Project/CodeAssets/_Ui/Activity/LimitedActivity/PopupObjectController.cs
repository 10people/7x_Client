using UnityEngine;
using System.Collections;

namespace LimitActivity
{
    public class PopupObjectController : MonoBehaviour
    {
        public UILabel m_NumLabel;
        public GameObject m_IconRootObject;

        public void SetThis(string text, GameObject iconPrefab)
        {
            m_NumLabel.text = text;

            while (m_IconRootObject.transform.childCount > 0)
            {
                var child = m_IconRootObject.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }

            var icon = Instantiate(iconPrefab) as GameObject;
            UtilityTool.ActiveWithStandardize(m_IconRootObject.transform, icon.transform);
            icon.transform.localScale = Vector3.one * 0.5f;
        }
    }
}

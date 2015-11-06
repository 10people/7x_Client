using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LimitActivity
{
    public class PopupListController : MonoBehaviour
    {
        public GameObject prefab;
        public UIGrid m_Grid;

        public struct PopupItem
        {
            public string labelText;
            public GameObject iconObject;
        }

        public void SetThis(List<PopupItem> icon_List)
        {
            while (m_Grid.transform.childCount > 0)
            {
                var child = m_Grid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }

            foreach (var icon in icon_List)
            {
                var responseObject = Instantiate(prefab) as GameObject;
                var controller = responseObject.GetComponent<PopupObjectController>();
                controller.SetThis(icon.labelText, icon.iconObject);

                UtilityTool.ActiveWithStandardize(m_Grid.transform, responseObject.transform);
            }

            m_Grid.Reposition();
        }
    }
}

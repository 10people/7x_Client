using UnityEngine;
using System.Collections;

namespace Rank
{
    public abstract class DetailController : MonoBehaviour
    {
        public ModuleController m_ModuleController;

        public GameObject FloatButtonsRoot;
        public FloatButtonsController FloatButtonsController;

        public IEnumerator AdjustFloatButton()
        {
            yield return new WaitForEndOfFrame();

            //Cancel adjust cause multi touch may destroy this float buttons gameobject.
            if (FloatButtonsController == null || FloatButtonsController.gameObject == null)
            {
                yield break;
            }

			NGUIHelper.AdaptWidgetInScrollView(m_ModuleController.m_ScrollView, m_ModuleController.m_ScrollBar, FloatButtonsController.m_BGLeft.GetComponent<UIWidget>());
        }

        public void DestroyFloatButtons()
        {
            if (FloatButtonsController != null)
            {
                Destroy(FloatButtonsController.gameObject);
                FloatButtonsController = null;
            }
        }

        /// <summary>
        /// Clear all float buttons when click, duplicate as press cause multi click may recreate float button object after clearing it on press, then redestroy it on click.
        /// [WARNING]Do not remove it, this is not multi added code.
        /// </summary>
        public void OnClick()
        {
            if (m_ModuleController != null)
            {
                m_ModuleController.m_DetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }
        }

        /// <summary>
        /// Clear all float buttons when press.
        /// </summary>
        /// <param name="isPress"></param>
        public void OnPress(bool isPress)
        {
            if (isPress)
            {
                if (m_ModuleController != null)
                {
                    m_ModuleController.m_DetailControllerList.ForEach(item => item.DestroyFloatButtons());
                }
            }
        }

        public abstract void GetInfo();

        public abstract void AddFriend();

        public abstract void Shield();

        public abstract void Rob();
    }
}

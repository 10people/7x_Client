using UnityEngine;
using System.Collections;

public class SpecialPurposeCloseLayer : MonoBehaviour
{
    public ScaleEffectController m_ScaleEffectController;

    public GameObject m_TargetDestroy;
    public bool m_IsDragTouch = false;
	void Start () 
    {
	
	}

    void OnClick()
    {
        if (m_ScaleEffectController != null)
        {
            m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
            m_ScaleEffectController.OnCloseWindowClick();
        }
        else
        {
            MainCityUI.TryRemoveFromObjectList(m_TargetDestroy);
            Destroy(m_TargetDestroy);
           
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (m_IsDragTouch)
        {
            if (m_ScaleEffectController != null)
            {
                m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
                m_ScaleEffectController.OnCloseWindowClick();
            }
            else
            {
                MainCityUI.TryRemoveFromObjectList(m_TargetDestroy);
                Destroy(m_TargetDestroy);

            }
        }
    }
    void DoCloseWindow()
    {
		FriendData.Instance.RequestData ();
        MainCityUI.TryRemoveFromObjectList(m_TargetDestroy);
        Destroy(m_TargetDestroy);
      
    }
}

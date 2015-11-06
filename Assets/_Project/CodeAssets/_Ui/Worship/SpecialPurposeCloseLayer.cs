using UnityEngine;
using System.Collections;

public class SpecialPurposeCloseLayer : MonoBehaviour
{
    public ScaleEffectController m_ScaleEffectController;

    public GameObject m_TargetDestroy;
  
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

    void DoCloseWindow()
    {
		FriendData.Instance.RequestData ();
        MainCityUI.TryRemoveFromObjectList(m_TargetDestroy);
        Destroy(m_TargetDestroy);
      
    }
}

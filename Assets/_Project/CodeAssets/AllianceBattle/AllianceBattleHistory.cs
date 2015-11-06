using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllianceBattleHistory : MonoBehaviour
{
    public List<EventIndexHandle> m_EventIndexHandles = new List<EventIndexHandle>();
    public ScaleEffectController m_ScaleEffectController;
    public TogglesControl m_TogglesControl;

    public GameObject m_NowObject;
    public GameObject m_LastObject;

    public UIScrollView m_NowScrollView;
    public UIScrollBar m_NowScrollBar;

    public UIScrollView m_LastScrollView;
    public UIScrollBar m_LastScrollBar;

    public void SetThis()
    {

    }

    void RefreshNow()
    {

    }

    void RefreshLast()
    {

    }

    void CloseWindow()
    {
        m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
    }

    void DoCloseWindow()
    {
        Destroy(gameObject);
    }

    private void OnEventClick(int index)
    {
        switch (index)
        {
            //Back
            case 0:
                {
                    CloseWindow();
                    break;
                }
            //Close
            case 1:
                {
                    CloseWindow();
                    break;
                }
            //Now
            case 2:
                {
                    m_TogglesControl.OnToggleClick(0);
                    RefreshNow();
                    break;
                }
            //Last
            case 3:
                {
                    m_TogglesControl.OnToggleClick(1);
                    RefreshLast();
                    break;
                }
        }
    }

    void Start()
    {
        OnEventClick(2);
    }

    void Awake()
    {
        m_EventIndexHandles.ForEach(item => item.m_Handle += OnEventClick);
    }

    void OnDestroy()
    {
        m_EventIndexHandles.ForEach(item => item.m_Handle -= OnEventClick);
    }
}

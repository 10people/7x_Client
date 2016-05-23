using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AllianceBattle
{
    public class AllianceBattleRule : MonoBehaviour
    {
        public List<EventIndexHandle> m_EventIndexHandles = new List<EventIndexHandle>();
        public ScaleEffectController m_ScaleEffectController;

        public UIScrollView m_ScrollView;
        public UIScrollBar m_ScrollBar;

        public UILabel m_MainLabel;

        public void SetThis()
        {

        }

        private void CloseWindow()
        {
            m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        private void DoCloseWindow()
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
            }
        }

        private void Awake()
        {
            m_EventIndexHandles.ForEach(item => item.m_Handle += OnEventClick);
        }

        private void OnDestroy()
        {
            m_EventIndexHandles.ForEach(item => item.m_Handle -= OnEventClick);
        }
    }
}
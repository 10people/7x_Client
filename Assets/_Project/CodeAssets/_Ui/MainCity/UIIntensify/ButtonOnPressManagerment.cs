using UnityEngine;
using System.Collections;

public class ButtonOnPressManagerment : MonoBehaviour 
{
    public delegate void PressDownSend(int index,bool down);
    public event PressDownSend m_PressHandle;
    public int m_SendIndex = 0;
    private float _timeInterval = 0.0f;
    private bool _dragged = false;
    private bool _pressDown = false;
    void Start()
    {

    }
         
    private void OnDragStart()
    {
        _dragged = true;
    }

    void Update()
    {
        if (_pressDown)
        {
            _timeInterval += Time.deltaTime;

            if (_timeInterval > 0.06f && !_dragged)
            {
                if (m_PressHandle != null)
                {
                    m_PressHandle(m_SendIndex, _pressDown);
                }
            }
        }
    
    }
    void OnPress(bool isDown)
    {
        _pressDown = isDown;

        if (!isDown)
        {
            if (m_PressHandle != null)
            {
                m_PressHandle(m_SendIndex, isDown);
            }
            _timeInterval = 0.0f;
            _dragged = false;
        }
    }
 
}

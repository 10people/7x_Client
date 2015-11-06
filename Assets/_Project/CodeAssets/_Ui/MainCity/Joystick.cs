using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Joystick : MYNGUIPanel
{
    public Camera m_currentCamera;

    public Transform m_joystickBackGround;

    public Transform m_joystickTransform;

    private UICamera.MouseOrTouch m_MouseOrTouch;

    public Vector3 m_uiOffset;

    public bool m_isLockBack;
    void Awake()
    {

    }

    void Start()
    {

    }

    public void setPos(int x, int y)
    {
        gameObject.transform.localPosition = new Vector3(x, y, 0);
    }

    public const float MaxToggleDistance = 60;

    private bool isWidgetShowed = true;

    void Update()
    {
        if (m_MouseOrTouch != null)
        {
            if (!m_MouseOrTouch.pressStarted)
            {
                m_MouseOrTouch = null;
                m_joystickBackGround.localPosition = Vector3.zero;
                m_joystickTransform.localPosition = Vector3.zero;
                m_uiOffset = Vector3.zero;

                if (MainCityUI.m_MainCityUI != null)
                {
                    if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
                    {
                        //                    StopNavigation();
                    }
                    else
                    {
                        HousePlayerController.s_HousePlayerController.StopPlayerNavigation();
                    }
                }
                return;
            }
            m_joystickTransform.position = m_currentCamera.ScreenToWorldPoint(m_MouseOrTouch.pos);

            float Dis = Vector3.Distance(Vector3.zero, m_joystickTransform.localPosition);
            if (Dis > MaxToggleDistance)
            {
                m_joystickTransform.localPosition = m_joystickTransform.localPosition.normalized * MaxToggleDistance;
            }
            m_uiOffset = new Vector3(m_joystickTransform.localPosition.x, 0, m_joystickTransform.localPosition.y);
        }
        //else if (isWidgetShowed)
        //{
        //    SetChildrenWidgetA(transform, 0.01f);
        //    isWidgetShowed = false;
        //}
    }

    private void SetChildrenWidgetA(Transform parent, float a)
    {
        var childs = parent.GetComponentsInChildren<UIWidget>();
        for (var i = 0; i < childs.Count(); i++)
        {
            childs[i].color = new Color(childs[i].color.r, childs[i].color.g, childs[i].color.b, a);
        }
    }

    public override void MYClick(GameObject ui)
    {

    }

    public override void MYMouseOver(GameObject ui)
    {

    }

    public override void MYMouseOut(GameObject ui)
    {

    }

    public override void MYPress(bool isPress, GameObject ui)
    {
        if (isPress)
        {
            CityGlobalData.m_joystickControl = true;

            m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
            if (!m_isLockBack)
            {
                m_joystickBackGround.position = m_currentCamera.ScreenToWorldPoint(m_MouseOrTouch.pos);
                //Check
                m_joystickBackGround.localPosition = new Vector3(
                    m_joystickBackGround.localPosition.x < 90 ? 90 : m_joystickBackGround.localPosition.x,
                    m_joystickBackGround.localPosition.y < 90 ? 90 : m_joystickBackGround.localPosition.y,
                    0);
            }

            if (MainCityUI.m_MainCityUI != null)
            {
                //Only in main city
                if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
                {
                    //                PlayerModelController.m_playerModelController.StopNavigation();
                }
                else
                {
                    HousePlayerController.s_HousePlayerController.StopPlayerNavigation();
                }
            }
        }
        else
        {
            CityGlobalData.m_joystickControl = false;

            m_MouseOrTouch = null;
            m_joystickBackGround.localPosition = new Vector3(90, 90, 0);
            m_joystickTransform.localPosition = Vector3.zero;
            m_uiOffset = Vector3.zero;
        }
    }

    public override void MYelease(GameObject ui)
    {

    }

    public override void MYondrag(Vector2 delta)
    {

    }

    public override void MYoubleClick(GameObject ui)
    {

    }

    public override void MYonInput(GameObject ui, string c)
    {

    }
}

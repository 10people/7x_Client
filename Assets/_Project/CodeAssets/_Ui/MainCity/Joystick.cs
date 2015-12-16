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

	public BoxCollider m_Box;

    public Vector3 m_uiOffset;

	public UISprite m_spriteBG;
	public UISprite m_spriteButton;
    public bool m_isLockBack;
    void Awake()
    {

    }

    void Start()
    {
		m_Box.center = new Vector3((480 + ClientMain.m_iMoveX) / 2 - 76, (320 + ClientMain.m_iMoveY) / 2 - 76, 0);
		m_Box.size = new Vector3(480 + ClientMain.m_iMoveX, 320 + ClientMain.m_iMoveY, 0);
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
                    m_joystickBackGround.localPosition.x < 76 ? 76 : m_joystickBackGround.localPosition.x,
                    m_joystickBackGround.localPosition.y < 76 ? 76 : m_joystickBackGround.localPosition.y,
                    0);
				m_spriteBG.color = new Color(1f, 1f, 1f, 1f);
				m_spriteButton.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            CityGlobalData.m_joystickControl = false;

            m_MouseOrTouch = null;
            m_joystickBackGround.localPosition = new Vector3(76, 76, 0);
            m_joystickTransform.localPosition = Vector3.zero;
            m_uiOffset = Vector3.zero;
			m_spriteBG.color = new Color(1f, 1f, 1f, 0.3f);
			m_spriteButton.color = new Color(1f, 1f, 1f, 0.3f);
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

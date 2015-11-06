using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QXJoystick : MonoBehaviour //主城摇杆
{
	public Camera m_currentCamera;

	public Vector3 m_uiOffset;

    private float m_handleRange = 60;

    public Vector3 m_u3dOffset;

	private Transform m_joystickBackGround;

	public Transform m_joystickTransform;

	private Vector3 m_startPosition;
	
	private bool m_touchBegain;
	
	private bool m_phone;

    Ray m_ray;



    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_phone = true;
        }
        else
        {
            m_phone = false;
        }
        m_startPosition = Vector3.zero;
        m_uiOffset = Vector3.zero;
        m_touchBegain = false;
    }

    void Start()
    {
		m_joystickBackGround = this.GetComponent<Transform>();
    }


    void Update()
    {
		if(MainCityUI.IsWindowsExist())
		{
			return;
		}

        if (m_phone == true) //手机
        {
			if (Input.touchCount >= 1)
			{
				Vector3 tempTouchPosition = Input.GetTouch(0).position;
				m_ray = m_currentCamera.ScreenPointToRay(tempTouchPosition);// 从屏幕发射线
				RaycastHit hit;
				if (Physics.Raycast(m_ray, out hit)) return;//碰到主界面UI按钮
				
				if (m_touchBegain == false)
				{
					if (CityGlobalData.m_touchRect.Contains(tempTouchPosition)) //限定触摸区域
					{
						Vector3 tempPosition = m_currentCamera.ScreenToWorldPoint(tempTouchPosition);
						
						m_startPosition = tempTouchPosition;
						
						m_touchBegain = true;
						
						CityGlobalData.m_joystickControl = true;

						m_joystickBackGround.position = new Vector3(tempPosition.x, tempPosition.y, 0);//position;
					}
				}
				if (m_touchBegain == true)
				{
                    Vector3 tempOffset = Input.mousePosition - m_startPosition;

                    if (tempOffset.x > m_handleRange) 
                    {
                        tempOffset.x = m_handleRange;
                    }
                    if (tempOffset.y > m_handleRange)
                    {
                        tempOffset.y = m_handleRange;
                    }

                    m_uiOffset = tempOffset;

                    m_u3dOffset = new Vector3(m_uiOffset.x, 0, m_uiOffset.y);

                    //PlayerRunController.m_CarriagePlayerController.StopNavigation();
				}
			}
			else
			{
				CityGlobalData.m_joystickControl = false;
				
				m_touchBegain = false;
				
				m_joystickBackGround.localPosition = Vector3.zero;

                m_uiOffset = Vector3.zero;

                m_u3dOffset = Vector3.zero;
			}
        }
        else
        {
            if (Input.GetMouseButton(0)) //pc
            {

				Vector3 tempMousePosition = Input.mousePosition;

				m_ray = m_currentCamera.ScreenPointToRay(tempMousePosition);// 从屏幕发射线
                RaycastHit hit;
                if (Physics.Raycast(m_ray, out hit))
                {
//                    Debug.Log("碰到物体");
                    return;//碰到主界面UI按钮
                } 

				if(MainCityUI.IsWindowsExist()) return; //有UI 界面弹出

                if (m_touchBegain == false)
                {
                    if (CityGlobalData.m_touchRect.Contains(tempMousePosition))
                    {
						Vector3 tempPosition = m_currentCamera.ScreenToWorldPoint(tempMousePosition);

                        m_startPosition = tempMousePosition;

                        m_touchBegain = true;

						CityGlobalData.m_joystickControl = true;

						m_joystickBackGround.position = new Vector3(tempPosition.x, tempPosition.y, 0);//position;
                    }
                }
                if (m_touchBegain == true)
                {
                    Vector3 tempOffset = Input.mousePosition - m_startPosition;

//					Debug.Log(tempOffset.x);
                    
					if (tempOffset.x > m_handleRange)
                    {
                        tempOffset.x = m_handleRange;
                    }
					else if(tempOffset.x < -m_handleRange)
					{
						tempOffset.x = -m_handleRange;
					}
                    if (tempOffset.y > m_handleRange)
                    {
                        tempOffset.y = m_handleRange;
                    }
					else if(tempOffset.y < -m_handleRange)
					{
						tempOffset.y = -m_handleRange;
					}

                    m_uiOffset = tempOffset;

                    m_u3dOffset = new Vector3(m_uiOffset.x, 0, m_uiOffset.y);
					Debug.Log("===================3");
					PlayerModelController.m_playerModelController.StopPlayerNavigation();
                }
            }
            else
            {
				CityGlobalData.m_joystickControl = false;

                m_touchBegain = false;

				m_joystickBackGround.localPosition = Vector3.zero;

                m_uiOffset = Vector2.zero;

                m_u3dOffset = Vector3.zero;
            }
        }
        m_joystickTransform.localPosition = new Vector3(m_uiOffset.x, m_uiOffset.y, 0);
    }

//    Rect tempRect = new Rect(CityGlobalData.m_ScreenWidth * 0.02f, CityGlobalData.m_ScreenHeight * 0.68f, CityGlobalData.m_ScreenWidth * 0.2f, CityGlobalData.m_ScreenHeight * 0.3f);

    void OnGUI()
    {
		//GUI.Window(0,tempRect, tuiCallBack, "TouchRect");
    }

    void tuiCallBack(int windowId)
    {
        
    }
}

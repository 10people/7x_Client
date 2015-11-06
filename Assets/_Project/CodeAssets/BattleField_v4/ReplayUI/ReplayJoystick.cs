using UnityEngine;
using System.Collections;

public class ReplayJoystick : MonoBehaviour{
	
	Ray m_ray;
	
	RaycastHit m_rayCastHit;
	
	private UITexture m_joystick;
	
	private Transform m_transform;
	
	private Vector3 m_startPosition;
	
	private Camera m_currentCamera;
	
	public Vector3 m_offset;
	
	private bool m_phone;

	private Vector3 startPosition;

	private bool stickOn;

	private int touchId;


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
	}
	
	void Start()
	{
		m_joystick = this.GetComponent<UITexture>();
		
		m_transform = this.transform;
		
		m_currentCamera = NGUITools.FindCameraForLayer(m_transform.gameObject.layer);
	
		startPosition = m_joystick.transform.localPosition;

		touchId = -100;
	}

	public void OnPress(bool pressed)
	{
		if(pressed)
		{
			touchId = UICamera.currentTouchID;
		}
		else if(touchId == UICamera.currentTouchID)
		{
			touchId = -100;
			
			stickOn = false;
			
			m_offset = Vector3.zero;
			
			m_joystick.transform.localPosition = startPosition;

			ReplayUIControllor.Instance().moveCamera(m_offset);
		}
	}

	void Update()
	{
		if(touchId == -100) return;

		if (m_phone == true)
		{
			//moveStickMobile(new Vector3(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, 0));

			moveStickMobile(UICamera.GetTouch(touchId).pos);
		}
		else
		{
			moveStickPc(Input.mousePosition);
		}

		ReplayUIControllor.Instance().moveCamera(m_offset);
	}

	private void moveStickMobile(Vector3 position)
	{
		Vector2 tempTouchPosition = position;
		
		Vector3 tempPosition = m_currentCamera.ScreenToWorldPoint(new Vector3(tempTouchPosition.x, tempTouchPosition.y, 0));
		
		if (stickOn == false)
		{
			//if(tempTouchPosition.x > Screen.width / 2) return;
			
			m_startPosition = tempPosition;
			
			stickOn = true;
			
			m_transform.position = new Vector3(tempPosition.x, tempPosition.y, 0);//position;
		}
		else
		{
			Vector3 tempVec3 = (tempPosition - m_startPosition).normalized;
			
			m_offset = new Vector3(tempVec3.x, 0, tempVec3.y);
		}
	}

	private void moveStickPc(Vector3 position)
	{
		Vector3 tempMousePosition = position;
		
		Vector3 tempPosition = m_currentCamera.ScreenToWorldPoint(tempMousePosition);
		
		if (stickOn == false)
		{
			if(tempMousePosition.x > Screen.width / 2) return;
			
			m_startPosition = tempMousePosition;
			
			stickOn = true;
			
			m_transform.position = new Vector3(tempPosition.x, tempPosition.y, 0);//position;
		}
		else
		{
			Vector3 tempVec3 = (position - m_startPosition).normalized;
			
			m_offset = new Vector3(tempVec3.x, 0, tempVec3.y);
		}
	}

}

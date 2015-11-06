using UnityEngine;
using System.Collections;

public class BattleJoystick : MonoBehaviour
{
	public GameObject board;


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

	public void release()
	{
		touchId = -100;
		
		stickOn = false;
		
		m_offset = Vector3.zero;
		
		m_joystick.transform.localPosition = startPosition;
		
		board.transform.localPosition = startPosition;

		#if UNITY_EDITOR || UNITY_STANDALONE
		#else
			BattleUIControlor.Instance().moveKing(m_offset);
		#endif
	}

	public void OnPress(bool pressed)
	{
		if(pressed)
		{
			touchId = UICamera.currentTouchID;
		}
		else if(touchId == UICamera.currentTouchID)
		{
			release();
		}
	}

	void Update()
	{
		if(touchId != -100)
		{
			if (m_phone == true)
			{
				moveStick(UICamera.GetTouch(touchId).pos);
			}
			else
			{
				moveStick(Input.mousePosition);
			}
		}
		#if UNITY_EDITOR || UNITY_STANDALONE
		#else
			BattleUIControlor.Instance().moveKing(m_offset);
		#endif
	}

	private void moveStick(Vector3 position)
	{
		if (stickOn == false)
		{
			Vector3 tempMousePosition = position;

			float minWidth = 80;

			float minHeight = 80;

			tempMousePosition.x = tempMousePosition.x < minWidth ? minWidth : tempMousePosition.x;

			tempMousePosition.y = tempMousePosition.y < minHeight ? minHeight : tempMousePosition.y;

			Vector3 tempPosition = m_currentCamera.ScreenToWorldPoint(tempMousePosition);

			if(tempMousePosition.x > Screen.width / 2) return;

			m_startPosition = tempMousePosition;

			stickOn = true;

			m_transform.position = new Vector3(tempPosition.x, tempPosition.y, 0);//position;

			board.transform.position = new Vector3(tempPosition.x, tempPosition.y, 0);
		}
		else
		{
			Vector3 tempVec3 = (position - m_startPosition).normalized;
			
			m_offset = new Vector3(tempVec3.x, 0, tempVec3.y);
		}

		Vector3 tv = position - m_startPosition;

		if (Vector3.Distance (tv, Vector3.zero) < 5) m_offset = Vector3.zero;

		if (Vector3.Distance (tv, Vector3.zero) > 50) tv = (position - m_startPosition).normalized * 50f;

		m_transform.localPosition = board.transform.localPosition + new Vector3(tv.x, tv.y, 0);
	}

}

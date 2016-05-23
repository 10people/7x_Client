using UnityEngine;
using System.Collections;

public class DragRoleRotate : MonoBehaviour {
	
	void OnDrag (Vector2 delta)
	{
//		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		
		//旋转角色
		QXSelectRolePage.m_instance.DragRole (delta);
	}
}

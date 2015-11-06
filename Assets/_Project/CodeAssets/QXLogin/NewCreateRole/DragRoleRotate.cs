using UnityEngine;
using System.Collections;

public class DragRoleRotate : MonoBehaviour {

	void Start ()
	{
		this.GetComponent<UISprite> ().alpha = 0.1f;
	}

	void OnDrag (Vector2 delta)
	{
//		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		
		//旋转角色
		CreateRoleManager.roleManager.DragRotateRole (delta);
	}
}

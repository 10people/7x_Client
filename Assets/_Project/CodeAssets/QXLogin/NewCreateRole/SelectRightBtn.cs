using UnityEngine;
using System.Collections;

public class SelectRightBtn : MonoBehaviour {

	private bool press = false;

	void OnPress (bool isPress)
	{
		press = isPress;
	}
	
	void Update ()
	{
		if (press)
		{
			CreateRoleManager.roleManager.ClickRotate (RoleRotate.Direction.RIGHT);
		}
	}
}

using UnityEngine;
using System.Collections;

public class ReplayJoyStickCollider : MonoBehaviour {

	public ReplayJoystick stick;

	void OnPress(bool pressed)
	{
		stick.OnPress(pressed);
	}

}

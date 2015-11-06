using UnityEngine;
using System.Collections;

public class JoyStickCollider : MonoBehaviour
{
	public BattleJoystick stick;


	void OnPress(bool pressed)
	{
		stick.OnPress(pressed);
	}

	public void release()
	{
		stick.release ();
	}

}

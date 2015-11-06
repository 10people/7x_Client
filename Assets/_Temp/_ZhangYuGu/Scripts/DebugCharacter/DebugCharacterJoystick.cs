using UnityEngine;
using System.Collections;

public class DebugCharacterJoystick : MonoBehaviour
{
	public DebugCharacterPlayer player;


	private bool inMoveing;


	void Start()
	{
		inMoveing = false;
	}

	void FixedUpdate()
	{
		if(inMoveing == true)
		{
			Vector2 temp = new Vector2(UICamera.lastTouchPosition.x - 480 - transform.localPosition.x, UICamera.lastTouchPosition.y - 320 - transform.localPosition.y);

			Vector2 touch = temp.normalized;

			player.move(new Vector3(touch.x, 0, touch.y));
		}
	}

	public void OnPress(bool pressed)
	{
		inMoveing = pressed;
	}

}

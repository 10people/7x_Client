using UnityEngine;
using System.Collections;

public class AttackJoystick : MonoBehaviour
{
	private int touchId = -100;

	private float startTime;

	private float attackDelay = .3f;

	private bool first;


	void Start()
	{
		//touchId = -100;

		first = true;
	}

	public void reset()
	{
		//startTime = Time.realtimeSinceStartup - attackDelay;

		OnPress (false);
	}

	void OnPress(bool pressed)
	{
		if(pressed)
		{
			touchId = UICamera.currentTouchID;

			startTime = Time.realtimeSinceStartup - attackDelay;
		}
		else
		{
			touchId = -100;
		}

		first = true;
	}

	void Update()
	{
		if(touchId == -100) return;

		float nowTime = Time.realtimeSinceStartup;
		
		if(nowTime - startTime > attackDelay)
		{
			if(first == true)
			{
				startTime = nowTime;
			}
			else 
			{
				startTime = nowTime - attackDelay + .1f;
			}

			BattleUIControlor.Instance().kingAttack();

			first = false;
		}
	}

}

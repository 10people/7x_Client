using UnityEngine;
using System.Collections;

public class BattleCenterLabelControllor : MonoBehaviour 
{
	public UISprite spriteWeaponHint;


	private float tempTime;

	private float showTime = 2f;

	private float hidingTime = .5f;


	void Start () 
	{
		hideAllLabel ();
	}

	public void changeWeapon(KingControllor.WeaponType weapon)
	{
		hideAllLabel ();

		tempTime = 0;

		spriteWeaponHint.gameObject.SetActive (true);

		if(weapon == KingControllor.WeaponType.W_Heavy)
		{
			spriteWeaponHint.spriteName = "battleWeapon_heavy";
		}
		else if(weapon == KingControllor.WeaponType.W_Light)
		{
			spriteWeaponHint.spriteName = "battleWeapon_light";
		}
		else if(weapon == KingControllor.WeaponType.W_Ranged)
		{
			spriteWeaponHint.spriteName = "battleWeapon_range";
		}
	}

	void Update ()
	{
		if(tempTime >= 0)
		{
			tempTime += Time.deltaTime;

			if(tempTime > showTime + hidingTime)
			{
				hideAllLabel();
			}
			else if(tempTime > showTime)//hiding
			{
				float T = tempTime - showTime;

				spriteWeaponHint.alpha = (hidingTime - T) / hidingTime;
			}
		}
	}

	private void hideAllLabel()
	{
		tempTime = -1;

		spriteWeaponHint.gameObject.SetActive (false);

		spriteWeaponHint.alpha = 1f;
	}

}

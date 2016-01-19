using UnityEngine;
using System.Collections;

public class BattleWeaponSkillDolt : MonoBehaviour 
{

	private UISprite sprite;

	private bool coolDown;

	private bool skillUnlock;

	private bool weaponUnlock;


	void Start () 
	{
		sprite = GetComponent<UISprite>();

		coolDown = true;

		skillUnlock = false;

		weaponUnlock = true;
	}

	public void setCooldown(bool _coolDown)
	{
		coolDown = _coolDown;

		//if(coolDown == false) skillUnlock = true;

		updataSprite ();
	}

	public void setSkillLock(bool _skillUnlock)
	{
		skillUnlock = _skillUnlock;

		//if (skillUnlock == true) coolDown = true;

		updataSprite ();
	}

	public void setWeaponUnlock(bool _weaponUnLock)
	{
		weaponUnlock = _weaponUnLock;

		updataSprite ();
	}

	private void updataSprite()
	{
		if(coolDown == true && skillUnlock == true && weaponUnlock == true)
		{
			sprite.spriteName = "battle_dolt_green";
		}
		else
		{
			sprite.spriteName = "battle_dolt_red";
		}
	}

}

using UnityEngine;
using System.Collections;

public class LockControllor : MonoBehaviour 
{
	public enum LOCK_TYPE
	{
		Attack,
		WeaponHeavy,
		WeaponLight,
		WeaponRange,
		HeavySkill_1,
		HeavySkill_2,
		LightSkill_1,
		LightSkill_2,
		RangeSkill_1,
		RangeSkill_2,
		MiBaoSkill,
		Dodge,
		AutoFight,
		Pause,
	}

	public GameObject lockAttack;

	public GameObject lockWeaponHeavy;

	public GameObject lockWeaponLight;

	public GameObject lockWeaponRange;

	public GameObject lockHeavySkill_1;

	public GameObject lockHeavySkill_2;

	public GameObject lockLightSkill_1;

	public GameObject lockLightSkill_2;
	
	public GameObject lockRangeSkill_1;
	
	public GameObject lockRangeSkill_2;

	public GameObject lockMiBaoSkill;

	public GameObject lockDodge;

	public GameObject lockAutoFight;

	public GameObject lockPause;


	private BattleUIControlor controllor;

	private static LockControllor _instance;


	public static LockControllor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }


	public void refreshLock (LOCK_TYPE lockType, bool targetActive) 
	{
		if (controllor == null) controllor = BattleUIControlor.Instance ();

		if(lockType == LOCK_TYPE.Attack) refreshLock(lockAttack, controllor.m_gc_attack, targetActive);

		else if(lockType == LOCK_TYPE.WeaponHeavy) refreshLock(lockWeaponHeavy, controllor.m_changeWeapon.btnHeavy, targetActive);
		
		else if(lockType == LOCK_TYPE.WeaponLight) refreshLock(lockWeaponLight, controllor.m_changeWeapon.btnLight, targetActive);
		
		else if(lockType == LOCK_TYPE.WeaponRange) refreshLock(lockWeaponRange, controllor.m_changeWeapon.btnRange, targetActive);
		
		else if(lockType == LOCK_TYPE.HeavySkill_1) refreshLock(lockHeavySkill_1, controllor.m_gc_skill_1[0], targetActive);
		
		else if(lockType == LOCK_TYPE.HeavySkill_2) refreshLock(lockHeavySkill_2, controllor.m_gc_skill_2[0], targetActive);
		
		else if(lockType == LOCK_TYPE.LightSkill_1) refreshLock(lockLightSkill_1, controllor.m_gc_skill_1[2], targetActive);
		
		else if(lockType == LOCK_TYPE.LightSkill_2) refreshLock(lockLightSkill_2, controllor.m_gc_skill_2[2], targetActive);
		
		else if(lockType == LOCK_TYPE.RangeSkill_1) refreshLock(lockRangeSkill_1, controllor.m_gc_skill_1[1], targetActive);
		
		else if(lockType == LOCK_TYPE.RangeSkill_2) refreshLock(lockRangeSkill_2, controllor.m_gc_skill_2[1], targetActive);
		
		else if(lockType == LOCK_TYPE.MiBaoSkill) refreshLock(lockMiBaoSkill, controllor.btnMibaoSkill, targetActive);
		
		else if(lockType == LOCK_TYPE.Dodge) refreshLock(lockDodge, controllor.m_gc_dodge, targetActive);
		
		else if(lockType == LOCK_TYPE.AutoFight) refreshLock(lockAutoFight, controllor.m_gc_autoFight, targetActive);
		
		else if(lockType == LOCK_TYPE.Pause) refreshLock(lockPause, controllor.m_gc_pause, targetActive);
	}

	private void refreshLock(GameObject gc_lock, GameObject icon, bool targetActive)
	{
		gc_lock.SetActive (targetActive == false && icon.activeSelf == true);

		UISprite[] sprites = icon.GetComponentsInChildren<UISprite>();

		float rgb = targetActive == false ? 0f : 1f;

		foreach(UISprite sprite in sprites)
		{
			if(sprite.gameObject.name.Equals("SpriteWEAPON")) continue;

			sprite.color = new Color(rgb, rgb, rgb, sprite.color.a);
		}
	}

	public void lightOff(GameObject lockObject, bool forced)
	{
		if(forced == true)
		{
			turnLight(lockObject, 0, forced);
		}
		else
		{
			turnLight(lockObject, .1f, forced);
		}
	}

	public void lightOn(GameObject lockObject, bool forced)
	{
		turnLight(lockObject, 1, forced);
	}

	private void turnLight(GameObject lockObject, float alpha, bool forced)
	{
		UISprite sprite = lockObject.GetComponent<UISprite> ();

		if (forced == true)
		{
			if(alpha == 1) lockObject.SetActive (true);

			else if(alpha == 0) lockObject.SetActive (false);
		}

		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
	}

}

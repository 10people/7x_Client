using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public GameObject lockMiBaoSkill;

	public GameObject lockAutoFight;

	public GameObject lockPause;

	public RuntimeAnimatorController unlockAnimation;

	public List<Vector3> iconPositions = new List<Vector3> ();

	public GameObject colliderObject;


	private BattleUIControlor controllor;

	private static LockControllor _instance;


	public static LockControllor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }

	void OnDestroy(){
		_instance = null;
	}

	public void refreshLock (LOCK_TYPE lockType, bool targetActive, bool showLabel = true) 
	{
		BattleWeapon bw = BattleUIControlor.Instance().m_changeWeapon;

		if (controllor == null) controllor = BattleUIControlor.Instance();

		if(lockType == LOCK_TYPE.Attack)
		{
			refreshLock(lockAttack, controllor.m_gc_attack, targetActive);
		}
		else if(lockType == LOCK_TYPE.WeaponHeavy)
		{
			refreshLock(lockWeaponHeavy, controllor.m_changeWeapon.btnHeavy, targetActive);

			bw.spriteHeavySkill_1.setWeaponUnlock(targetActive);

			bw.spriteHeavySkill_2.setWeaponUnlock(targetActive);

			bw.labelHeavy.SetActive(targetActive);
		}
		else if(lockType == LOCK_TYPE.WeaponLight) 
		{
			refreshLock(lockWeaponLight, controllor.m_changeWeapon.btnLight, targetActive);

			bw.spriteLightSkill_1.setWeaponUnlock(targetActive);

			bw.spriteLightSkill_2.setWeaponUnlock(targetActive);

			bw.labelLight.SetActive(targetActive);
		}
		else if(lockType == LOCK_TYPE.WeaponRange) 
		{
			refreshLock(lockWeaponRange, controllor.m_changeWeapon.btnRange, targetActive);

			bw.spriteRangeSkill_1.setWeaponUnlock(targetActive);

			bw.spriteRangeSkill_2.setWeaponUnlock(targetActive);

			if(targetActive == false)
			{
				UILabel label = lockWeaponRange.GetComponentInChildren<UILabel>();
				
				if(label != null) label.gameObject.SetActive(showLabel);
			}

			bw.labelRange.SetActive(targetActive);
		}
		else if(lockType == LOCK_TYPE.HeavySkill_1)
		{
			refreshLock(null, controllor.m_gc_skill_1[0], targetActive, CityGlobalData.skillLevelId.bahuanglieri, KingControllor.WeaponType.W_Heavy);

			bw.spriteHeavySkill_1.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.HeavySkill_2)
		{
			refreshLock(null, controllor.m_gc_skill_2[0], targetActive, CityGlobalData.skillLevelId.qiankundouzhuan, KingControllor.WeaponType.W_Heavy);

			bw.spriteHeavySkill_2.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.LightSkill_1)
		{
			refreshLock(null, controllor.m_gc_skill_1[2], targetActive, CityGlobalData.skillLevelId.jueyingxingguangzhan, KingControllor.WeaponType.W_Light);

			bw.spriteLightSkill_1.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.LightSkill_2)
		{
			refreshLock(null, controllor.m_gc_skill_2[2], targetActive, CityGlobalData.skillLevelId.xuejilaoyin, KingControllor.WeaponType.W_Light);

			bw.spriteLightSkill_2.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.RangeSkill_1)
		{
			refreshLock(null, controllor.m_gc_skill_1[1], targetActive, CityGlobalData.skillLevelId.zhuixingjian, KingControllor.WeaponType.W_Ranged);

			bw.spriteRangeSkill_1.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.RangeSkill_2)
		{
			refreshLock(null, controllor.m_gc_skill_2[1], targetActive, CityGlobalData.skillLevelId.hanbingjian, KingControllor.WeaponType.W_Ranged);

			bw.spriteRangeSkill_2.setSkillLock(targetActive);
		}
		else if(lockType == LOCK_TYPE.MiBaoSkill)
		{
			refreshLock(lockMiBaoSkill, controllor.btnMibaoSkill, targetActive);

			if(targetActive == true && BattleControlor.Instance().getKing() != null)
			{
				BattleControlor.Instance().getKing().addNuqi(0);
			}

			if(targetActive == false)
			{
				UILabel label = lockMiBaoSkill.GetComponentInChildren<UILabel>();

				if(label != null) label.gameObject.SetActive(showLabel);
			}

		}
		else if(lockType == LOCK_TYPE.Dodge)
		{
			refreshLock(null, controllor.m_gc_dodge, targetActive, CityGlobalData.skillLevelId.fangun, BattleControlor.Instance().getKing().weaponType);
		}
		else if(lockType == LOCK_TYPE.AutoFight)
		{
			refreshLock(lockAutoFight, controllor.m_gc_autoFight, targetActive);
		}
		else if(lockType == LOCK_TYPE.Pause)
		{
			refreshLock(lockPause, controllor.m_gc_pause, targetActive);
		}
	}

	private void refreshLock(GameObject gc_lock, GameObject icon, bool targetActive, CityGlobalData.skillLevelId skillLevelId = CityGlobalData.skillLevelId.zhongji, KingControllor.WeaponType weaponType = KingControllor.WeaponType.W_Heavy)
	{
		if(gc_lock != null)
		{
			UISprite lockSprite = gc_lock.GetComponent<UISprite>();

			lockSprite.alpha = ((targetActive == false && icon.activeSelf == true) ? 1f : .1f);

			iTween.StopByName("refreshLock" + gc_lock.name);

			iTween.ValueTo(gameObject, iTween.Hash(
				"name", "refreshLock" + gc_lock.name,
				"from", 0f,
				"to", 1f,
				"time", 2f,
				"onupdate", "unlockUpdate",
				"oncomplete", "unlockComplete",
				"oncompleteparams", gc_lock,
				"ignoretimescale", true
				));

//			gc_lock.SetActive (targetActive == false && icon.activeSelf == true);

			UISprite[] sprites = icon.GetComponentsInChildren<UISprite>();

			float rgb = targetActive == false ? 0f : 1f;

			foreach(UISprite sprite in sprites)
			{
				if(sprite.gameObject.name.Equals("SpriteWEAPON")) continue;

				if(sprite.gameObject.name.Contains("Skill_1")) continue;

				if(sprite.gameObject.name.Contains("Skill_2")) continue;

				sprite.color = new Color(rgb, rgb, rgb, sprite.color.a);
			}
		}
		else if(targetActive == false)
		{
			if(BattleControlor.Instance().getKing().weaponType == weaponType)
			{
				icon.SetActive(targetActive);
			}
		}
		else
		{
			bool showEffectFlag = false;

			if(BattleControlor.Instance().getKing().playUnlockEffList.Contains((int)skillLevelId) == true)
			{
				showEffectFlag = true;
			}
			else
			{

			}

			if(showEffectFlag == true && BattleControlor.Instance().getKing().weaponType == weaponType)
			{
				BattleControlor.Instance().getKing().playUnlockEffList.Remove((int)skillLevelId);
				
				Vector3 tempPosition = iconPositions[(int)skillLevelId];

				if(skillLevelId == CityGlobalData.skillLevelId.fangun)
				{
					icon.transform.localPosition = -BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
				}
				else
				{
					icon.transform.localPosition = -BattleUIControlor.Instance().anchorBottomRight.transform.localPosition - icon.transform.parent.localPosition;
				}

				icon.SetActive(true);

				GameObject sprite = icon.GetComponentInChildren<UISprite>().gameObject;

				UI3DEffectTool.ShowTopLayerEffect( UI3DEffectTool.UIType.MainUI_0, icon, EffectIdTemplate.getEffectTemplateByEffectId(100009).path);

				Animator anim = sprite.GetComponent<Animator>();

				if(anim == null) anim = sprite.AddComponent<Animator>();

				anim.runtimeAnimatorController = unlockAnimation;

				colliderObject.SetActive(true);

				iTween.MoveTo(icon, iTween.Hash(
					"name", "unlockMove_" + icon.name,
					"delay", 1.5f,
					"position", tempPosition,
					"time", .8f,
					"islocal", true,
					"onstarttarget", gameObject,
					"onstart","startMove",
					"onstartparams", icon,
					"oncompletetarget", gameObject,
					"oncomplete", "unlockEffDone",
					"oncompleteparams", sprite,
					"ignoretimescale", true
					));
			}
		}
	}

	private void unlockUpdate(float t)
	{
	
	}

	private void unlockComplete(GameObject lockObject)
	{
		UISprite sprite = lockObject.GetComponent<UISprite>();

		lockObject.SetActive (sprite.alpha > .5f);
	}

	private void startMove(GameObject icon)
	{
		UI3DEffectTool.ClearUIFx (icon);

		//UI3DEffectTool.ShowTopLayerEffect( UI3DEffectTool.UIType.MainUI_0, icon, EffectIdTemplate.getEffectTemplateByEffectId(620223).path);
	}

	private void unlockEffDone(GameObject icon)
	{
		//UI3DEffectTool.ClearUIFx (icon);

		colliderObject.SetActive(false);

		UI3DEffectTool.ShowTopLayerEffect( UI3DEffectTool.UIType.MainUI_0, icon, EffectIdTemplate.getEffectTemplateByEffectId(620222).path);

		StartCoroutine (unlockEffOver(icon));
	}

	IEnumerator unlockEffOver(GameObject icon)
	{
		yield return new WaitForSeconds (1.2f);

		UI3DEffectTool.ClearUIFx (icon);

		Destroy (icon.GetComponent<Animator> ());
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

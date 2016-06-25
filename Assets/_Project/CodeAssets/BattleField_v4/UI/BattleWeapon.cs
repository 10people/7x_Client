using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWeapon : MonoBehaviour 
{
	public GameObject btnHeavy;

	public GameObject btnLight;

	public GameObject btnRange;

	public UISprite focusHeavy;

	public UISprite focusLight;

	public UISprite focusRange;

	public GameObject labelHeavy;

	public GameObject labelLight;

	public GameObject labelRange;

	public BattleWeaponSkillDolt spriteHeavySkill_1;

	public BattleWeaponSkillDolt spriteHeavySkill_2;

	public BattleWeaponSkillDolt spriteLightSkill_1;

	public BattleWeaponSkillDolt spriteLightSkill_2;

	public BattleWeaponSkillDolt spriteRangeSkill_1;

	public BattleWeaponSkillDolt spriteRangeSkill_2;

	public GameObject layerMibaoAnim;


	public void changeWeaponToHeavy()
	{
		DramaControllor.Instance().closeYindao (5);

		bool flag = precheck ();

		if (flag == false) return;

		flag = BattleUIControlor.Instance().changeWeaponTo (KingControllor.WeaponType.W_Heavy);

		if (flag == false) return;

		lightOff ();

		focusHeavy.gameObject.SetActive (true);

		BattleControlor.Instance().getKing ().setAutoWeapon (false);

		//spriteHeavySkill_1.gameObject.SetActive (false);
		
		//spriteHeavySkill_2.gameObject.SetActive (false);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	public void changeWeaponToLight()
	{
		DramaControllor.Instance().closeYindao (6);

		bool flag = precheck ();
		
		if (flag == false) return;

		flag = BattleUIControlor.Instance().changeWeaponTo (KingControllor.WeaponType.W_Light);

		if (flag == false) return;

		lightOff ();

		focusLight.gameObject.SetActive (true);

		BattleControlor.Instance().getKing ().setAutoWeapon (false);

		//spriteLightSkill_1.gameObject.SetActive (false);
		
		//spriteLightSkill_2.gameObject.SetActive (false);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	public void changeWeaponToRange()
	{
		DramaControllor.Instance().closeYindao(7);

		bool flag = precheck ();

		if (flag == false) return;
		
		flag = BattleUIControlor.Instance().changeWeaponTo (KingControllor.WeaponType.W_Ranged);

		if (flag == false) return;

		lightOff ();

		focusRange.gameObject.SetActive (true);

		BattleControlor.Instance().getKing ().setAutoWeapon (false);

		//spriteRangeSkill_1.gameObject.SetActive (false);
		
		//spriteRangeSkill_2.gameObject.SetActive (false);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	private void lightOff()
	{
		focusHeavy.gameObject.SetActive (false);

		focusLight.gameObject.SetActive (false);

		focusRange.gameObject.SetActive (false);

//		spriteHeavySkill_1.gameObject.SetActive (true);
//
//		spriteHeavySkill_2.gameObject.SetActive (true);
//
//		spriteLightSkill_1.gameObject.SetActive (true);
//
//		spriteLightSkill_2.gameObject.SetActive (true);
//
//		spriteRangeSkill_1.gameObject.SetActive (true);
//
//		spriteRangeSkill_2.gameObject.SetActive (true);
	}

	public void refreshIcons()
	{
		lightOff ();

//		btnHeavy.SetActive (btnHeavy.activeSelf && BattleControlor.Instance().getKing().weaponDateHeavy != null);
//
//		btnLight.SetActive (btnLight.activeSelf && BattleControlor.Instance().getKing().weaponDateLight != null);
//
//		btnRange.SetActive (btnRange.activeSelf && BattleControlor.Instance().getKing().weaponDateRanged != null);

		if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
		{
			focusHeavy.gameObject.SetActive (true);

			//spriteHeavySkill_1.gameObject.SetActive (false);
			
			//spriteHeavySkill_2.gameObject.SetActive (false);
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
		{
			focusLight.gameObject.SetActive (true);

			//spriteLightSkill_1.gameObject.SetActive (false);
			
			//spriteLightSkill_2.gameObject.SetActive (false);
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
		{
			focusRange.gameObject.SetActive (true);

			//spriteRangeSkill_1.gameObject.SetActive (false);
			
			//spriteRangeSkill_2.gameObject.SetActive (false);
		}
	}

	private bool precheck()
	{
		//if (BattleControlor.Instance().getKing ().isPlayingAttack ()) return false;

		string playing = BattleControlor.Instance().getKing ().IsPlaying ();

		if (BattleControlor.Instance().getKing ().isPlayingSwing ()) return true;

		if (BattleControlor.Instance().getKing ().isPlayingSkill ()) return false;

		if (playing.Equals ("XuanFengZhan") == true) return false;

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName (BaseAI.AniType.ANI_DODGE)) == true) return false;

		if (playing.Equals (BattleControlor.Instance().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCDown)) == true) return false;

		if (playing.Equals (BattleControlor.Instance().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCUp)) == true) return false;

		return true;
	}

	public int getUnlockCount()
	{
		int count = 0;

		if (labelHeavy.gameObject.activeSelf == true) count ++;

		if (labelLight.gameObject.activeSelf == true) count ++;

		if (labelRange.gameObject.activeSelf == true) count ++;

		return count;
	}

}

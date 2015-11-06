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


	public void changeWeaponToHeavy()
	{
		bool flag = precheck ();

		if (flag == false) return;

		flag = BattleUIControlor.Instance ().changeWeaponTo (KingControllor.WeaponType.W_Heavy);

		if (flag == false) return;

		lightOff ();

		focusHeavy.gameObject.SetActive (true);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	public void changeWeaponToLight()
	{
		DramaControllor.Instance ().closeYindao (20103);

		bool flag = precheck ();
		
		if (flag == false) return;

		flag = BattleUIControlor.Instance ().changeWeaponTo (KingControllor.WeaponType.W_Light);

		if (flag == false) return;

		lightOff ();

		focusLight.gameObject.SetActive (true);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	public void changeWeaponToRange()
	{
		DramaControllor.Instance().closeYindao(20105);

		bool flag = precheck ();

		if (flag == false) return;
		
		flag = BattleUIControlor.Instance ().changeWeaponTo (KingControllor.WeaponType.W_Ranged);

		if (flag == false) return;

		lightOff ();

		focusRange.gameObject.SetActive (true);

		//UIYindao.m_UIYindao.CloseUI ();
	}

	private void lightOff()
	{
		focusHeavy.gameObject.SetActive (false);

		focusLight.gameObject.SetActive (false);

		focusRange.gameObject.SetActive (false);
	}

	public void refreshIcons()
	{
		lightOff ();

		btnHeavy.SetActive (btnHeavy.activeSelf && BattleControlor.Instance().getKing().weaponDateHeavy != null);

		btnLight.SetActive (btnLight.activeSelf && BattleControlor.Instance().getKing().weaponDateLight != null);

		btnRange.SetActive (btnRange.activeSelf && BattleControlor.Instance().getKing().weaponDateRanged != null);

		if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
		{
			focusHeavy.gameObject.SetActive (true);
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
		{
			focusLight.gameObject.SetActive (true);
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
		{
			focusRange.gameObject.SetActive (true);
		}
	}

	private bool precheck()
	{
		//if (BattleControlor.Instance ().getKing ().isPlayingAttack ()) return false;

		string playing = BattleControlor.Instance ().getKing ().IsPlaying ();

		if (BattleControlor.Instance ().getKing ().isPlayingSwing ()) return true;

		if (BattleControlor.Instance ().getKing ().isPlayingSkill ()) return false;

		if (playing.Equals ("XuanFengZhan") == true) return false;

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCDown)) == true) return false;

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCUp)) == true) return false;

		return true;
	}

}

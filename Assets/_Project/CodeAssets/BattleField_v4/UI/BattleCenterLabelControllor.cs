#define HIDE_MIBAO_LABEL

using UnityEngine;
using System.Collections;

public class BattleCenterLabelControllor : MonoBehaviour 
{
	public UILabel labelWeapon;

	public UILabel labelSkill_1;

	public UILabel labelSkill_2;


	public float tempTime;


	void Start () 
	{
		hideAllLabel ();

		#if HIDE_MIBAO_LABEL
		Debug.Log( "1 TODO here, Temporary Hide Label For Art, " );

		labelWeapon.gameObject.SetActive( false );

		labelSkill_1.gameObject.SetActive( false );

		labelSkill_2.gameObject.SetActive( false );
		#endif
	}

	public void changeWeapon(KingControllor.WeaponType weapon)
	{
		hideAllLabel ();

		tempTime = 0;

		if(weapon == KingControllor.WeaponType.W_Heavy)
		{
			labelWeapon.text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_WEAPON_HEAVY);
		}
		else if(weapon == KingControllor.WeaponType.W_Light)
		{
			labelWeapon.text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_WEAPON_LIGHT);
		}
		else if(weapon == KingControllor.WeaponType.W_Ranged)
		{
			labelWeapon.text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_WEAPON_RANGE);
		}
	}

	public void useSkill(string skillNameId)
	{
		string skillName = NameIdTemplate.GetName_By_NameId (int.Parse(skillNameId));

		StartCoroutine (showSkillLabel(skillName));
	}

	IEnumerator showSkillLabel(string skillName)
	{
		hideAllLabel ();

		string text1 = skillName.Substring (0, 2);
		
		string text2 = skillName.Substring (2);
		
		tempTime = 0;
		
		labelSkill_1.text = text1;

		labelSkill_1.transform.localScale = new Vector3 (5f, 5f, 5f);

		iTween.ScaleTo (labelSkill_1.gameObject, iTween.Hash(
			"scale", new Vector3(1f, 1f, 1f),
			"time", .18f,
			"easeType", iTween.EaseType.linear
			));

		yield return new WaitForSeconds (.3f);

		labelSkill_2.text = text2;

		labelSkill_2.transform.localScale = new Vector3 (5f, 5f, 5f);

		iTween.ScaleTo (labelSkill_2.gameObject, iTween.Hash(
			"scale", new Vector3(1f, 1f, 1f),
			"time", .18f,
			"easeType", iTween.EaseType.linear
			));
	}

	void Update ()
	{
		if(tempTime >= 0)
		{
			tempTime += Time.deltaTime;

			if(tempTime > 2)
			{
				hideAllLabel();
			}
		}

	}

	private void hideAllLabel()
	{
		tempTime = -1;
		
		labelWeapon.text = "";
		
		labelSkill_1.text = "";
		
		labelSkill_2.text = "";
	}

}

using UnityEngine;
using System.Collections;

public class BattleUIButton : MonoBehaviour
{
	public enum BtnType
	{
		BTN_ATTACK,
		BTN_WEAPON,
		BTN_CHANGE_SCENE,
		BTN_GUWU,
		BTN_YUANJUN,
		BTN_DAO_SKILL_1,
		BTN_DAO_SKILL_2,
		BTN_QIANG_SKILL_1,
		BTN_QIANG_SKILL_2,
		BTN_GONG_SKILL_1,
		BTN_GONG_SKILL_2,
		BTN_AUTO_FIGHT,
		BTN_MIBAO,
	}

	public BtnType btnType;


//	private bool sendAttackOrder;

	void Start()
	{
//		sendAttackOrder = false;
	}

	void OnClick()
	{
		if(btnType == BtnType.BTN_WEAPON)
		{
			BattleUIControlor.Instance().kingChangeWeapon();
		}
		else if(btnType == BtnType.BTN_CHANGE_SCENE)
		{
			BattleUIControlor.Instance().enterPause();
		}
		else if(btnType == BtnType.BTN_DAO_SKILL_1)
		{
			BattleUIControlor.Instance().useDaoSkill_1();
		}
		else if(btnType == BtnType.BTN_DAO_SKILL_2)
		{
			BattleUIControlor.Instance().useDaoSkill_2();
		}
		else if(btnType == BtnType.BTN_QIANG_SKILL_1)
		{
			BattleUIControlor.Instance().useQiangSkill_1();
		}
		else if(btnType == BtnType.BTN_QIANG_SKILL_2)
		{
			BattleUIControlor.Instance().useQiangSkill_2();
		}
		else if(btnType == BtnType.BTN_GONG_SKILL_1)
		{
			BattleUIControlor.Instance().useGongSkill_1();
		}
		else if(btnType == BtnType.BTN_GONG_SKILL_2)
		{
			BattleUIControlor.Instance().useGongSkill_2();
		}
		else if(btnType == BtnType.BTN_GUWU)
		{
			//BattleUIControlor.Instance().heroInspire();
		}
		else if(btnType == BtnType.BTN_AUTO_FIGHT)
		{
			BattleUIControlor.Instance().changeAutoFight();
		}
		else if(btnType == BtnType.BTN_MIBAO)
		{
			BattleUIControlor.Instance().useMiBaoSkill();
		}
	}

	void OnPress(bool pressed)
	{
		if(btnType == BtnType.BTN_ATTACK)
		{
//			sendAttackOrder = pressed;
		}
	}

}

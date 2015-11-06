using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TipsData : MonoBehaviour {

	public UISprite m_icon;

	public UILabel m_dec;


	public void ShowData(int tempType) //君主属性tips
	{
		List<string> m_stringList = new List<string>(){"增加君主的攻击力","增加君主的防御力","增加君主的总生命", "增加君主的生命百分比" ,"增加君主的物理伤害和物理减免","增加君主的技能伤害和技能减免"};

		List<string> m_iconList = new List<string>(){"att_attack","att_defense","att_hp","att_command","att_power","att_intelligence"};

		m_icon.spriteName = m_iconList[tempType];

		m_dec.text = m_stringList[tempType];
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JianDunDataManager 
{
	public List<string> descs = new List<string> ();

	private List<List<int>> m_listData = new List<List<int>>(){new List<int>(), new List<int>(), new List<int>()};
	private List<bool> m_listBool = new List<bool>(){false, false, false};

	public JianDunDataManager(List<int> dataID)
	{
		for(int i = 0; i < dataID.Count; i ++)
		{
			PveStarTemplate temp = PveStarTemplate.getPveStarTemplateByStarId(dataID[i]);

//			Debug.Log("JianDunDataManager ID " + dataID[i]);

			descs.Add(DescIdTemplate.GetDescriptionById(temp.desc));

			string[] tempData = temp.condition.Split(',');

			for(int q = 0; q < tempData.Length; q ++)
			{
				m_listData[i].Add(int.Parse(tempData[q]));
			}
		}
	}

	//击杀怪物
	public void KillMonster(int id)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 2)
			{
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(Mathf.Abs(m_listData[i][2 + q] % 10000) == id)
					{
						m_listData[i][2 + q] = 0;

						break;
					}
				}
			}
		}
	}

	//使用技能
	public void UseSkill(int skillId, int monsterId)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 3)
			{
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] == skillId)
					{
						m_listData[i][2 + q] = 0;

						break;
					}
				}
			}
			if(m_listData[i][0] == 6)
			{
				if(m_listData[i][1] == monsterId)
				{
					for(int q = 0; q < m_listData[i][2]; q ++)
					{
						if(m_listData[i][4 + (q * 2)] == skillId)
						{
							m_listData[i][4 + (q * 2) - 1] --;

							break;
						}
					}
				}
			}
		}
	}

	//切换武器
	public void ReplaceWap(int type)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 4)
			{
				bool tempbool = false;
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] == type)
					{
						tempbool = true;
						break;
					}
				}
				if(!tempbool)
				{
					for(int q = 0; q < m_listData[i][1]; q ++)
					{
						m_listData[i][2 + q] = -1;
					}
				}
			}
		}
	}

	//造成伤害
	public void AttackNum(int num)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 5)
			{
				m_listData[i][1] -= num;
			}
		}
	}

	//打断
	public void Interrupt(int monterId)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 7)
			{
				if(m_listData[i][1] == -1 || m_listData[i][1] == monterId)
				{
					m_listData[i][2] --;
				}
			}
		}
	}

	//承受技能
	public void BySkill(int skillId)
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 8)
			{
				if(m_listData[i][2] == -1 || m_listData[i][2] == skillId)
				{
					m_listData[i][1] --;
				}
			}
		}
	}

	//	12击退,最大击退次数
	//	13击倒,最大击倒次数
	//	14击晕,最大击晕次数

	//击退
	public void ByJiTui()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 12)
			{
				m_listData[i][1] --;
			}
		}
	}

	//击倒
	public void ByJiDao()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 13)
			{
				m_listData[i][1] --;
			}
		}
	}

	//击晕
	public void ByJiYun()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 14)
			{
				m_listData[i][1] --;
			}
		}
	}

	//结束时调用
	public List<bool> EndBattle()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			bool temp = true;
			switch(m_listData[i][0])
			{
			case 1:
				if(BattleControlor.Instance().battleTime > m_listData[i][1])
				{
					temp = false;
				}
				break;
			case 2:
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] != 0)
					{
						temp = false;
						break;
					}
				}
				break;
			case 3:
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] == 0)
					{
						temp = false;
						break;
					}
				}
				break;
			case 4:
				if(m_listData[i][2] == -1)
				{
					temp = false;
				}
				break;
			case 5:
				if(m_listData[i][1] > 0)
				{
					temp = false;
				}
				break;
			case 6:
				for(int q = 0; q < m_listData[i][2]; q ++)
				{
					if(m_listData[i][4 + (q * 2) - 1] > 0)
					{
						temp = false;
						break;
					}
				}
				break;
			case 7:
				if(m_listData[i][2] > 0)
				{
					temp = false;
				}
				break;
			case 8:
				if(m_listData[i][1] > 0)
				{
					temp = false;
				}
				break;
			case 9:
				List<Buff> Buffs = BattleControlor.Instance().getKing().getBuffs();
				for(int q = 0; q < Buffs.Count; q ++)
				{
					for(int p = 0; p < m_listData[i][1]; p ++)
					{
						if(m_listData[i][2 + p] == (int)Buffs[q].buffType)
						{
							m_listData[i][2 + p] = 0;
						}
					}
				}
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] == 0)
					{
						temp = false;
						break;
					}
				}
				break;
			case 10:
				List<int> mibaos = BattleControlor.Instance().mibaoIds;
				for(int q = 0; q < mibaos.Count; q ++)
				{
					bool istemp = false;
					for(int p = 0; p < m_listData[i][1]; p ++)
					{
						if(m_listData[i][2 + p] == mibaos[q])
						{
							m_listData[i][2 + p] = 0;
							istemp = true;
							break;
						}
					}
					if(!istemp)
					{
						for(int p = 0; p < m_listData[i][1]; p ++)
						{
							if(m_listData[i][2 + p] == -1)
							{
								m_listData[i][2 + p] = 0;
								break;
							}
						}
					}
				}
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] != 0)
					{
						temp = false;
						break;
					}
				}
				break;
			case 11:
				float maxHP = BattleControlor.Instance().getKing().nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpMax);
				float curHP = BattleControlor.Instance().getKing().nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp);
				if(((curHP / maxHP) * 100) < m_listData[i][1])
				{
					temp = false;
				}
				break;
			case 12:
			case 13:
			case 14:
				if(m_listData[i][1] < 0)
				{
					temp = false;
				}
				break;
			}
			m_listBool[i] = temp;
		}
		return m_listBool;
	}

//	1时间类型,X时间
//	2打怪类型,X打怪数量,怪物ID(与数量匹配)
//	3不使用技能,X数量,技能ID(与数量匹配)
//	4只使用武器类型,X数量,武器类型(与数量匹配)
//	5造成伤害类型,X伤害总数
//	6技能使用类型,X怪物的ID,X技能的数量,(X使用次数,技能ID)
//	7打断类型,怪物ID(-1为任意怪),打断次数
//	8承受技能类型,技能数量,技能ID
//	9结算BUFF,buff数量,buff类型(与数量匹配)
//	10秘宝,秘宝数量,秘宝ID(与数量匹配)
//	11HP
//	12击退,最大击退次数
//	13击倒,最大击倒次数
//	14击晕,最大击晕次数
}

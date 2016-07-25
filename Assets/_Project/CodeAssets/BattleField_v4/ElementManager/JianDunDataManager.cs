using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JianDunDataManager 
{
	public List<string> descs = new List<string> ();


	private List<List<int>> m_listData = new List<List<int>>(){new List<int>(), new List<int>(), new List<int>()};


	//bool改为int型，0-未达成，1-已达成，-1-无法达成
	private List<int> m_listBool = new List<int>(){0, 0, 0};


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

	//掉血
	public void lostHp()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			if(m_listData[i][0] == 11)
			{
				float maxHP = BattleControlor.Instance().getKing().nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpMaxReal);
				float curHP = BattleControlor.Instance().getKing().nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp);
				if(((curHP / maxHP) * 100) < m_listData[i][1])
				{
					if(m_listBool[i] != -1)
					{
						m_listBool[i] = -1;
						BattleControlor.Instance().achivementCallback(i, -1);
					}
				}
			}
		}

	}
	//	11HP
	//	12击退,最大击退次数
	//	13击倒,最大击倒次数
	//	14击晕,最大击晕次数

	//击杀怪物
	public void KillMonster(int id)
	{
		int temp = 1;
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
					if(m_listData[i][2 + q] != 0)
					{
						temp = 0;
					}
				}
				if(temp == 1 && m_listBool[i] != 1)
				{
					m_listBool[i] = 1;
					BattleControlor.Instance().achivementCallback(i, temp);
				}
			}
		}
	}

	//使用技能
	public void UseSkill(int skillId, int monsterId)
	{
		int temp = 1;
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
					if(m_listData[i][2 + q] == 0)
					{
						temp = -1;
					}
				}
				if(temp == -1 && m_listBool[i] != -1)
				{
					m_listBool[i] = -1;
					BattleControlor.Instance().achivementCallback(i, temp);
				}
			}
			if(m_listData[i][0] == 6)
			{
				temp = 1;
				if(m_listData[i][1] == monsterId)
				{
					for(int q = 0; q < m_listData[i][2]; q ++)
					{
						if(m_listData[i][4 + (q * 2)] == skillId)
						{
							m_listData[i][4 + (q * 2) - 1] --;

							break;
						}
						if(m_listData[i][4 + (q * 2) - 1] != 0)
						{
							temp = 0;
						}
					}
					if(temp == 1 && m_listBool[i] != 1)
					{
						m_listBool[i] = 1;
						BattleControlor.Instance().achivementCallback(i, temp);
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
					if(m_listBool[i] != -1)
					{
						m_listBool[i] = -1;
						BattleControlor.Instance().achivementCallback(i, -1);
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
				if(m_listData[i][1] <= 0)
				{
					if(m_listBool[i] != 1)
					{
						m_listBool[i] = 1;
						BattleControlor.Instance().achivementCallback(i, 1);
					}
				}
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
					if(m_listData[i][2] <= 0)
					{
						if(m_listBool[i] != 1)
						{
							m_listBool[i] = 1;
							BattleControlor.Instance().achivementCallback(i, 1);
						}
					}
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
					if(m_listData[i][1] <= 0)
					{
						if(m_listBool[i] != 1)
						{
							m_listBool[i] = 1;
							BattleControlor.Instance().achivementCallback(i, 1);
						}
					}
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
				if(m_listData[i][1] <= 0)
				{
					if(m_listBool[i] != -1)
					{
						m_listBool[i] = -1;
						BattleControlor.Instance().achivementCallback(i, 1);
					}
				}
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
				if(m_listData[i][1] <= 0)
				{
					if(m_listBool[i] != -1)
					{
						m_listBool[i] = -1;
						BattleControlor.Instance().achivementCallback(i, 1);
					}
				}
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
				if(m_listData[i][1] <= 0)
				{
					if(m_listBool[i] != -1)
					{
						m_listBool[i] = -1;
						BattleControlor.Instance().achivementCallback(i, 1);
					}
				}
			}
		}
	}

	//结束时调用
	public List<int> EndBattle()
	{
		for(int i = 0; i < m_listData.Count; i ++)
		{
			int temp = 1;
			switch(m_listData[i][0])
			{
			case 1:
				if(BattleControlor.Instance().battleTime > m_listData[i][1])
				{
					temp = 0;
				}
				break;
			case 2:
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] != 0)
					{
						temp = 0;
						break;
					}
				}
				break;
			case 3:
				for(int q = 0; q < m_listData[i][1]; q ++)
				{
					if(m_listData[i][2 + q] == 0)
					{
						temp = 0;
						break;
					}
				}
				break;
			case 4:
				if(m_listData[i][2] == -1)
				{
					temp = 0;
				}
				break;
			case 5:
				if(m_listData[i][1] > 0)
				{
					temp = 0;
				}
				break;
			case 6:
				for(int q = 0; q < m_listData[i][2]; q ++)
				{
					if(m_listData[i][4 + (q * 2) - 1] > 0)
					{
						temp = 0;
						break;
					}
				}
				break;
			case 7:
				if(m_listData[i][2] > 0)
				{
					temp = 0;
				}
				break;
			case 8:
				if(m_listData[i][1] > 0)
				{
					temp = 0;
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
						temp = 0;
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
						temp = 0;
						break;
					}
				}
				break;
			case 11:
				BaseAI tempBaseAI = BattleControlor.Instance().getNodebyId(m_listData[i][2]);
				if(tempBaseAI == null)
				{
					temp = 0;
				}
				else
				{
					float maxHP = tempBaseAI.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpMaxReal);
					float curHP = tempBaseAI.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp);
					if(((curHP / maxHP) * 100) < m_listData[i][1])
					{
						temp = 0;
					}
					else
					{
						temp = 1;
					}
				}
				break;
			case 12:
			case 13:
			case 14:
				if(m_listData[i][1] < 0)
				{
					temp = 0;
				}
				else
				{
					temp = 1;
				}
				break;
			}
			if(m_listBool[i] != -1)
			{
				m_listBool[i] = temp;
				if(temp == 1)
				{
					BattleControlor.Instance().achivementCallback(i, 1);
				}
				else
				{
					BattleControlor.Instance().achivementCallback(i, 0);
				}
			}
		}
		return m_listBool;
	}

	public List<int> getListBool()
	{
		return m_listBool;
	}

	public void setListBool(int achi_1, int achi_2, int achi_3)
	{
		m_listBool [0] = achi_1;

		m_listBool [1] = achi_2;

		m_listBool [2] = achi_3;
	}
}

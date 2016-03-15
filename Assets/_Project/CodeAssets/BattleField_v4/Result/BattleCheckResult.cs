using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleCheckResult
{
	[HideInInspector] public int bossKilled;
	
	[HideInInspector] public int gearKilled;
	
	[HideInInspector] public int soldierKilled;
	
	[HideInInspector] public int heroKilled;


	private int proNum_lose;

	private int proNum_win;


	public BattleCheckResult()
	{
		bossKilled = 0;
		
		gearKilled = 0;
		
		soldierKilled = 0;
		
		heroKilled = 0;

		proNum_lose = 0;

		proNum_win = 0;
	}

	public BattleControlor.BattleResult checkResult(BaseAI _node)
	{
		BattleControlor.BattleResult result = checkResultWin (_node);

		if(result == BattleControlor.BattleResult.RESULT_WIN)
		{
			return BattleControlor.BattleResult.RESULT_WIN;
		}

		result = checkResultLose (_node);

		if(result == BattleControlor.BattleResult.RESULT_LOSE)
		{
			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan && CityGlobalData.battleTemplateId == -1)//玩家第一次进入百战必胜
			{
				return BattleControlor.BattleResult.RESULT_WIN;
			}

			return BattleControlor.BattleResult.RESULT_LOSE;
		}

		return BattleControlor.BattleResult.RESULT_BATTLING;
	}

	private BattleControlor.BattleResult checkResultWin(BaseAI _node)
	{
		bool fWin = false;
		
		BattleWinTemplate winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_All, true);
		
		if(winTemplate != null)
		{
			fWin = true;
			
			foreach(BaseAI enemyNode in BattleControlor.Instance().enemyNodes)
			{
				if(_node != null && enemyNode.nodeId == _node.nodeId)
				{
					continue;
				}
				
				if(enemyNode.flag.accountable)
				{
					fWin = false;
				}
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_Boss, true);
		
		if(winTemplate != null)
		{
			int killNum = winTemplate.killNum;
			
			if(_node!= null && _node.nodeData.nodeType == NodeType.BOSS && _node.nodeId > 0)
			{
				killNum --;
			}
			
			if(bossKilled >= killNum)
			{
				fWin = true;
				
				bossKilled = winTemplate.killNum;
				
				BattleUIControlor.Instance().refreshWinDesc();
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_Gear, true);
		
		if(winTemplate != null)
		{
			int killNum = winTemplate.killNum;
			
			if(_node!= null && _node.nodeData.nodeType == NodeType.GEAR && _node.nodeId > 0)
			{
				killNum --;
			}
			
			if(gearKilled >= killNum)
			{
				fWin = true;
				
				gearKilled = winTemplate.killNum;
				
				BattleUIControlor.Instance().refreshWinDesc();
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_Hero, true);
		
		if(winTemplate != null)
		{
			int killNum = winTemplate.killNum;
			
			if(_node!= null && _node.nodeData.nodeType == NodeType.HERO && _node.nodeId > 0)
			{
				killNum --;
			}
			
			if(heroKilled >= killNum)
			{
				fWin = true;
				
				heroKilled = winTemplate.killNum;
				
				BattleUIControlor.Instance().refreshWinDesc();
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_Soldier, true);
		
		if(winTemplate != null)
		{
			int killNum = winTemplate.killNum;
			
			if(_node!= null && _node.nodeData.nodeType == NodeType.SOLDIER && _node.nodeId > 0)
			{
				killNum --;
			}
			
			if(soldierKilled >= killNum)
			{
				fWin = true;
				
				soldierKilled = winTemplate.killNum;
				
				BattleUIControlor.Instance().refreshWinDesc();
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Reach_Destination, true);
		
		if(winTemplate != null)
		{
			int num = 0;
			
			foreach(int nodeId in winTemplate.activeList)
			{
				BaseAI node = BattleControlor.Instance().getNodebyId(nodeId);
				
				if(node != null)
				{
					float length = Vector3.Distance(node.transform.position, winTemplate.destination);
					
					if(length < winTemplate.destinationRadius)
					{
						num ++;
					}
				}
			}
			
			if(num >= winTemplate.activeNum)
			{
				fWin = true;
			}

			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}
		
		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.PROTECT, true);

		if(winTemplate != null)
		{
			if(_node != null)
			{
				foreach(int nodeId in winTemplate.protectList)
				{
					if(nodeId == _node.nodeId)
					{
						proNum_win ++;
					}
				}
			}
			
			if(proNum_win >= winTemplate.protectNum)
			{
				fWin = true;
			}
			
			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}

		winTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Reach_Time, true);

		if(winTemplate != null)
		{
			if(BattleControlor.Instance().timeLast <= 0 && BattleControlor.Instance().timeLast != -100)
			{
				fWin = true;
			}

			if(fWin == true) fWin = BattleWinTemplate.reachTypeWin(winTemplate.winId);
		}

		if (fWin == true) return BattleControlor.BattleResult.RESULT_WIN;

		return BattleControlor.BattleResult.RESULT_BATTLING;
	}

	private BattleControlor.BattleResult checkResultLose(BaseAI _node)
	{
		bool fLose = false;
		
		BattleWinTemplate loseTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Kill_All, false);
		
		if(loseTemplate != null)
		{
			fLose = true;
			
			foreach(BaseAI enemyNode in BattleControlor.Instance().selfNodes)
			{
				if(_node != null && enemyNode.nodeId == _node.nodeId)
				{
					continue;
				}
				
				if(enemyNode.flag.accountable)
				{
					fLose = false;
				}
			}
			
			if(fLose == true) fLose = BattleWinTemplate.reachTypeLose(loseTemplate.winId);
		}

		loseTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Reach_Destination, false);
		
		if(loseTemplate != null)
		{
			int num = 0;

			foreach(int nodeId in loseTemplate.activeList)
			{
				BaseAI node = BattleControlor.Instance().getNodebyId(nodeId);

				if(node != null)
				{
					float length = Vector3.Distance(node.transform.position, loseTemplate.destination);

					if(length < loseTemplate.destinationRadius)
					{
						num ++;
					}
				}
			}

			if(num >= loseTemplate.activeNum)
			{
				fLose = true;
			}
			
			if(fLose == true) fLose = BattleWinTemplate.reachTypeLose(loseTemplate.winId);
		}

		loseTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.PROTECT, false);
		
		if(loseTemplate != null)
		{
			if(_node != null)
			{
				foreach(int nodeId in loseTemplate.protectList)
				{
					if(nodeId == _node.nodeId)
					{
						proNum_lose ++;
					}
				}
			}

			if(proNum_lose >= loseTemplate.protectNum)
			{
				fLose = true;
			}
			
			if(fLose == true) fLose = BattleWinTemplate.reachTypeLose(loseTemplate.winId);
		}

		loseTemplate = BattleWinTemplate.getWinTemplateContainsType (BattleWinFlag.EndType.Reach_Time, false);

		if(loseTemplate != null)
		{
			if(BattleControlor.Instance().timeLast <= 0 && BattleControlor.Instance().timeLast != -100)
			{
				fLose = true;
			}
			
			if(fLose == true) fLose = BattleWinTemplate.reachTypeLose(loseTemplate.winId);
		}

		if (fLose == true) return BattleControlor.BattleResult.RESULT_LOSE;

		return BattleControlor.BattleResult.RESULT_BATTLING;
	}
}

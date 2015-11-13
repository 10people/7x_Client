using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralRules : MonoBehaviour {

	public UILabel rulesLabel;

	public BoxCollider touchCol;

	private GeneralControl.RuleType ruleType;

	public ScaleEffectController m_ScaleEffectController;

	/// <summary>
	/// 规则
	/// </summary>
	/// <param name="type">挑战规则类型（需要自己添加，特殊处理需求）</param>
	/// <param name="textList">规则list</param>
	/// <param name="rootObjName">根prefab名字</param>
	public void ShowRules (GeneralControl.RuleType type,List<string> textList)
	{
		ruleType = type;

		for (int i = 0;i < textList.Count;i ++)
		{
			if (i < textList.Count - 1)
			{
				rulesLabel.text += textList[i] + "\n\n";
			}
			else
			{
				rulesLabel.text += textList[i];
			}
		}

		if (rulesLabel.height <= 476)
		{
			touchCol.enabled = false;
		}
		else
		{
			touchCol.enabled = true;
		}
	}

	/// <summary>
	/// 返回按钮，关闭当前页面
	/// </summary>
	public void BackBtn ()
	{
		switch (ruleType)
		{
		case GeneralControl.RuleType.PVP:
		{
			PvpPage.pvpPage.PvpActiveState (true);
//			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
//			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
			break;
		}
		case GeneralControl.RuleType.LUE_DUO:
		{
			LueDuoData.Instance.IsStop = false;
			LueDuoManager.ldManager.ShowChangeSkillEffect (true);
			break;
		}
		default:
			break;
		}
		Destroy (this.gameObject);
	}

	/// <summary>
	/// 关闭按钮，关闭所有页面
	/// </summary>
	public void CloseBtn ()
	{
		switch (ruleType)
		{
		case GeneralControl.RuleType.PVP:

//			BaiZhanData.Instance ().CloseBaiZhan ();
			PvpPage.pvpPage.DisActiveObj ();
		
			break;

		case GeneralControl.RuleType.ALLIANCE_FIGHT:

			_MyAllianceManager.Instance().Closed();
			AllianceFightMainPage.fightMainPage.CloseBtn ();

			break;

		case GeneralControl.RuleType.HUANGYE:

			HY_UIManager.Instance().CloseUI();

			break;

		case GeneralControl.RuleType.LUE_DUO:

			LueDuoManager.ldManager.DestroyRoot ();

			break;
		default:
			break;
		}

		m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
		m_ScaleEffectController.OnCloseWindowClick();
	}

	void OnCloseWindow()
	{
		Destroy (this.gameObject);
	}
}

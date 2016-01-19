using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralControl : Singleton<GeneralControl>
{
	#region GeneralRule

	private string ruleText;
	private GeneralRules.OnRulesBtnClick rulesClick;

	/// <summary>
	/// Loads the rules prefab.
	/// </summary>
	/// <param name="tempText">Temp text.</param>
	/// <param name="tempClick">Temp click.</param>
	public void LoadRulesPrefab (string tempText,GeneralRules.OnRulesBtnClick tempClick = null)
	{
		ruleText = tempText;
		rulesClick = tempClick;

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
		                        RulesLoadBack );
	}

	/// <summary>
	/// Loads the rules prefab.
	/// </summary>
	/// <param name="textList">Text list.</param>
	/// <param name="tempClick">Temp click.</param>
	public void LoadRulesPrefab (List<string> textList,GeneralRules.OnRulesBtnClick tempClick = null)
	{
		string text = "";
		foreach (string s in textList)
		{
			text += (s + "\n");
		}
		ruleText = text;
		rulesClick = tempClick;

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
		                        RulesLoadBack );
	}
	void RulesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject rulesObj = GameObject.Instantiate( p_object ) as GameObject;
		
		GeneralRules rules = rulesObj.GetComponent<GeneralRules> ();
		rules.ShowRules (ruleText,rulesClick);
	}

	#endregion

	#region GeneralTiaoZhan

	public enum ChallengeType
	{
		PVP,//百战
		PLUNDER,//掠夺
	}
	private ChallengeType challengeType;

	//百战
	private ChallengeResp pvpResp;

	//掠夺
	private LveGoLveDuoResp plunderResp;

	private GameObject tiaoZhanObj;

	/// <summary>
	/// Opens the pvp challenge page.
	/// </summary>
	public void OpenPvpChallengePage (ChallengeResp tempResp)
	{
		challengeType = ChallengeType.PVP;
		pvpResp = tempResp;
		LoadChallengePage ();
	}

	/// <summary>
	/// Opens the plunder challenge page.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void OpenPlunderChallengePage (LveGoLveDuoResp tempResp)
	{
		challengeType = ChallengeType.PLUNDER;
		plunderResp = tempResp;
		LoadChallengePage ();
	}

	void LoadChallengePage ()
	{
		if (tiaoZhanObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_CHALLENGE_PAGE ),
			                        ChallengeLoadCallback );
		}
		else
		{
			tiaoZhanObj.SetActive (true);
			InItChallengePage ();
		}
	}
	void ChallengeLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		tiaoZhanObj = Instantiate (p_object) as GameObject;
		InItChallengePage ();
	}
	void InItChallengePage ()
	{
		MainCityUI.TryAddToObjectList (tiaoZhanObj);
		switch (challengeType)
		{
		case ChallengeType.PVP:
			GeneralChallengePage.gcPage.InItPvpChallengePage (challengeType,pvpResp);
			break;
		case ChallengeType.PLUNDER:
			GeneralChallengePage.gcPage.InItPlunderChallengePage (challengeType,plunderResp);
			break;
		default:
			break;
		}
	}
	#endregion

	void OnDestroy(){
		base.OnDestroy();
	}
}

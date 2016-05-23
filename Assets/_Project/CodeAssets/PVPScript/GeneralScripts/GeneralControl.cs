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
	#region GeneralRules

	private GameObject m_rulesObj;

	private string ruleText;

	private GeneralRules.RulesDelegate m_rulesDelegate;

	/// <summary>
	/// Loads the rules prefab.
	/// </summary>
	/// <param name="tempText">Temp text.</param>
	/// <param name="tempClick">Temp click.</param>
	public void LoadRulesPrefab (string tempText,GeneralRules.RulesDelegate tempClick = null)
	{
		ruleText = tempText;
		m_rulesDelegate = tempClick;

		if (m_rulesObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
			                        RulesLoadBack );
		}
		else
		{
			m_rulesObj.SetActive (true);
		}
	}

	/// <summary>
	/// Loads the rules prefab.
	/// </summary>
	/// <param name="textList">Text list.</param>
	/// <param name="tempClick">Temp click.</param>
	public void LoadRulesPrefab (List<string> textList,GeneralRules.RulesDelegate tempClick = null)
	{
		string text = "";
		foreach (string s in textList)
		{
			text += (s + "\n");
		}
		ruleText = text;
		m_rulesDelegate = tempClick;

		if (m_rulesObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
			                        RulesLoadBack );
		}
		else
		{
			m_rulesObj.SetActive (true);
			InItRulesPage ();
		}
	}

	void RulesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		m_rulesObj = GameObject.Instantiate( p_object ) as GameObject;

		InItRulesPage ();
	}

	void InItRulesPage ()
	{
//		MainCityUI.TryAddToObjectList (m_rulesObj);
		GeneralRules.m_instance.M_RulesDelegate = RulesDelegateCallBack;
		GeneralRules.m_instance.ShowRules (ruleText);
	}

	void RulesDelegateCallBack ()
	{
		if (m_rulesDelegate != null)
		{
			m_rulesDelegate ();
		}
//		MainCityUI.TryRemoveFromObjectList (m_rulesObj);
		m_rulesObj.SetActive (false);
	}

	#endregion

	#region GeneralTiaoZhan

	private GeneralChallengePage.ChallengeType m_challengeType;

	private object m_GeneralResp;

	private GameObject tiaoZhanObj;

	/// <summary>
	/// Opens the pvp challenge page.
	/// </summary>
	public void OpenChallengePage (GeneralChallengePage.ChallengeType tempType,object tempResp)
	{
		m_challengeType = tempType;
		m_GeneralResp = tempResp;

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
		GeneralChallengePage.m_instance.InItChallengePage (m_challengeType,m_GeneralResp);
		GeneralChallengePage.m_instance.M_ChallengeDelegate = ChallengeDelegateCallBack;
	}
	void ChallengeDelegateCallBack ()
	{
		tiaoZhanObj.SetActive (false);
		MainCityUI.TryRemoveFromObjectList (tiaoZhanObj);
	}
	#endregion

	#region GeneralRecord

	private GameObject m_recordObj;

	private GeneralRecord.RecordType m_recordType;

	private object m_objectResp;

	private GeneralRecord.RecordDelegate m_recordDelegate;

	public void LoadRecordPage (GeneralRecord.RecordType tempType,object tempResp,GeneralRecord.RecordDelegate tempDelegate = null)
	{
		m_recordType = tempType;
		m_objectResp = tempResp;
		m_recordDelegate = tempDelegate;

		if (m_recordObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.TIAOZHAN_RECORD ),
			                        RecordLoadCallback );
		}
		else
		{
			m_recordObj.SetActive (true);
			InItRecordPage ();
		}
	}

	void RecordLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		m_recordObj = Instantiate (p_object) as GameObject;
		InItRecordPage ();
	}

	void InItRecordPage ()
	{
//		MainCityUI.TryAddToObjectList (m_recordObj);
		GeneralRecord.m_instance.M_RecordDelegate = RecordDelegateCallBack;
		GeneralRecord.m_instance.InItRecordPage (m_recordType,m_objectResp);
	}

	void RecordDelegateCallBack ()
	{
		if (m_recordDelegate != null)
		{
			m_recordDelegate ();
		}
//		MainCityUI.TryRemoveFromObjectList (m_recordObj);
		m_recordObj.SetActive (false);
	}

	#endregion

	void OnDestroy(){
		base.OnDestroy();
	}
}
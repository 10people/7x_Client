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
	//规则类型
	public enum RuleType
	{
		PVP,
		LUE_DUO,
		HUANGYE,
		ALLIANCE_FIGHT,
		MIBAO,
	}
	private RuleType ruleType = RuleType.PVP;
	
	private List<string> ruleList;
	
	/// <summary>
	/// 规则
	/// </summary>
	/// <param name="type">挑战规则类型（需要自己添加，特殊处理需求）</param>
	/// <param name="textList">规则list</param>
	/// <param name="rootObjName">根prefab名字</param>
	public void LoadRulesPrefab (RuleType type,List<string> textList)
	{
		ruleType = type;
		ruleList = textList;
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
		                        RulesLoadBack );
	}
	
	void RulesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject rulesObj = GameObject.Instantiate( p_object ) as GameObject;
		
		GeneralRules rules = rulesObj.GetComponent<GeneralRules> ();
		rules.ShowRules (ruleType,ruleList);
	}
	#endregion
}

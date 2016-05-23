using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralRules : GeneralInstance<GeneralRules> {

	public delegate void RulesDelegate ();

	public RulesDelegate M_RulesDelegate;

	public UILabel m_rules;

	public UIDragScrollView m_dragArea;

	public UIScrollView m_ruleSc;
	public UIScrollBar m_ruleSb;

	new void Awake ()
	{
		base.Awake ();
	}

	/// <summary>
	/// Shows the rules.
	/// </summary>
	/// <param name="textList">Text list.</param>
	/// <param name="tempClick">Temp click.</param>
	public void ShowRules (string ruleText)
	{
		m_rules.text = ruleText;
		m_ruleSc.UpdateScrollbars (true);

		m_dragArea.enabled = m_rules.height > 180 ? true : false;

		m_ruleSb.gameObject.SetActive (m_rules.height > 180 ? true : false);
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			M_RulesDelegate ();

			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}

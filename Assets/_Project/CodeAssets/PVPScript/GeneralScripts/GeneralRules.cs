using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralRules : MonoBehaviour {

	public delegate void OnRulesBtnClick ();

	private OnRulesBtnClick onRulesBtnClick;

	public UILabel rulesLabel;

	public UIDragScrollView dragArea;

	public UIScrollView ruleSc;
	public UIScrollBar ruleSb;

	public List<EventHandler> closeHandlerList = new List<EventHandler>();

	public ScaleEffectController m_ScaleEffectController;

	/// <summary>
	/// Shows the rules.
	/// </summary>
	/// <param name="textList">Text list.</param>
	/// <param name="tempClick">Temp click.</param>
	public void ShowRules (string ruleText,OnRulesBtnClick tempClick)
	{
		m_ScaleEffectController.OnOpenWindowClick ();

//		rulesLabel.text = "[ffffff]" + ruleText + "[-]";
		rulesLabel.text = ruleText;
		ruleSc.UpdateScrollbars (true);

		dragArea.enabled = rulesLabel.height > 326 ? true : false;

		ruleSb.gameObject.SetActive (rulesLabel.height > 326 ? true : false);

		onRulesBtnClick = tempClick;
		foreach (EventHandler handler in closeHandlerList)
		{
			handler.m_handler += CloseHandlerClickBack;
		}
	}

	void CloseHandlerClickBack (GameObject obj)
	{
		if (onRulesBtnClick != null)
		{
			onRulesBtnClick ();
		}

		Destroy (gameObject);
	}
}

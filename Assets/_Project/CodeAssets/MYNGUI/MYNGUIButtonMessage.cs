//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>
public class MYNGUIButtonMessage : MonoBehaviour
{
	public MYNGUIPanel panel;
	bool mStarted = false;
	bool mHighlighted = false;

	
	void Start () 
	{
		mStarted = true; 
	}
	
	void OnEnable ()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(gameObject));
		}
	}
	
	void OnHover (bool isOver)
	{
		if (enabled)
		{
			mHighlighted = isOver;
		}
		else
		{
			
		}
	}
	
	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			panel.MYPress(isPressed, gameObject);
		}
	}
	
	void OnClick ()
	{
		float tempTime = Time.time;
		if(tempTime - panel.m_TimeP > 0.2f)
		{
			panel.m_TimeP = tempTime;
			panel.MYClick(gameObject);
		}
	}
	
	void OnDoubleClick ()
	{
		if (enabled )
		{
			panel.MYoubleClick(gameObject);
		}
	}
	
	void OnDrag(Vector2 delta)
	{
		panel.MYondrag(delta);
	}
	
	public void onInput(string text)
	{
		panel.MYonInput(gameObject, text);
	}
}
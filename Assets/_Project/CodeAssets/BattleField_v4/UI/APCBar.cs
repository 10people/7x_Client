using UnityEngine;
using System.Collections;

public class APCBar : MonoBehaviour
{
	public UIProgressBar barPlayer;

	public UILabel labelName;

	public UISprite spriteIcon;

	public UILabel labelBubble;


	private BaseAI focusNode = null;

	private float targetValue;
	
	private float curValue;

	private float tempTime;


	public void setFocusNode(BaseAI _node)
	{
		if (focusNode != null) return;

		gameObject.SetActive (true);

		focusNode = _node;

		targetValue = 1;

		curValue = 1;

		tempTime = 0;

		labelName.text = focusNode.nodeData.nodeName + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_PROTECT_NAME);

		ModelTemplate modelTemp = ModelTemplate.getModelTemplateByModelId (focusNode.modelId, true);

		spriteIcon.spriteName = modelTemp.icon + "";

		labelBubble.gameObject.SetActive (false);
	}

	public BaseAI getFocusNode()
	{
		return focusNode;
	}

	void FixedUpdate()
	{
		if (focusNode == null) return;
		
		targetValue = focusNode.nodeData.GetAttribute ((int)AIdata.AttributeType.ATTRTYPE_hp) / focusNode.nodeData.GetAttribute ((int)AIdata.AttributeType.ATTRTYPE_hpMaxReal);
	
		UpdateBarPlayer ();

		updateTime ();
	}

	void UpdateBarPlayer()
	{
		float step = .05f;
		
		float value = barPlayer.value;
		
		if(Mathf.Abs(value - targetValue) < step * 1.5f)
		{
			value = targetValue;
		}
		else if(value > targetValue)
		{
			value -= step;
		}
		else if(value < targetValue)
		{
			value += step;
		}
		
		barPlayer.value = value;
	}

	void updateTime()
	{
		if (tempTime == 0) return;
		
		float now = Time.realtimeSinceStartup;
		
		if (now - tempTime > 1.5f)
		{
			closeBubble();
		}
	}

	public void setBubbleText(string text)
	{
		tempTime = Time.realtimeSinceStartup;

		labelBubble.gameObject.SetActive (true);

		labelBubble.text = text;
	}

	public void closeBubble()
	{
		labelBubble.gameObject.SetActive (false);
		
		tempTime = 0;
	}

}

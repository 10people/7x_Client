using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubblePopNode : MonoBehaviour 
{
	[HideInInspector] public BaseAI node;

	[HideInInspector] public Dictionary<int, BubblePopTemplate> dict = null;

	[HideInInspector] public GameObject gcTemple;


	private GameObject gc;

	private UILabel label;

	private float tempTime;

	private BubblePopTemplate distanceOutTemplate;

	private BubblePopTemplate distanceInTemplate;


	public void init()
	{
		tempTime = 0;

		distanceOutTemplate = null;

		distanceInTemplate = null;

		dict.TryGetValue (3, out distanceOutTemplate);

		dict.TryGetValue (4, out distanceInTemplate);

		foreach(BubblePopTemplate temp in dict.Values)
		{
			temp.triggerCurNum = 0;
		}
	}

	public void triggerFunc(BubblePopTemplate template)
	{
		if(gc == null)
		{
			gc = Instantiate(gcTemple);

			gc.transform.parent = node.transform;

			gc.transform.localScale = new Vector3(1 / node.transform.localScale.x, 1 / node.transform.localScale.y, 1 / node.transform.localScale.z );

			gc.transform.localPosition = new Vector3(0, node.appearanceTemplate.height * 2 + .2f, 0);

			label = gc.GetComponentInChildren<UILabel>();

			gc.SetActive(false);
		}

		int rand = ((int)(Random.value * 100)) % template.listTextId.Count;

		int textId = template.listTextId [rand];

		if (textId == 0) return;

		if (template.triggerCurNum >= template.triggerNum) return;

		template.triggerCurNum ++;

		if(BattleUIControlor.Instance().barAPC.gameObject.activeSelf == true
		   && BattleUIControlor.Instance().barAPC.getFocusNode().nodeId == node.nodeId)
		{
			Vector3 position2D = Camera.main.WorldToViewportPoint(node.transform.position);

			if(position2D.x < 0 || position2D.x > 1 || position2D.y < 0 || position2D.y > 1)
			{
				BattleUIControlor.Instance().barAPC.setBubbleText(BubbleTextTemplate.getBubbleTextById (textId));

				return;
			}
		}

		label.text = BubbleTextTemplate.getBubbleTextById (textId);

		tempTime = Time.realtimeSinceStartup;

		gc.SetActive (true);

		if(template.soundID != 0)
		{
			ClientMain.m_ClientMain.m_SoundPlayEff.PlaySound(template.soundID + "");
		}
	}

	void Update ()
	{
		updateOutDistance ();
		
		updateInDistance ();

		updateForward ();

		updateTime ();
	}

	private void updateForward()
	{
		if (Camera.main == null) return;
		
		if (gc == null) return;
		
		gc.transform.forward = Camera.main.transform.forward;
	}

	private void updateOutDistance()
	{
		if(distanceOutTemplate == null) return;

		BaseAI t_node = BattleControlor.Instance().getNodebyId ((int)distanceOutTemplate.tp1);

		if (t_node == null || t_node.gameObject.activeSelf == false || t_node.isAlive == false || t_node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) <= 0) 
		{
			return;
		}

		float distance = Vector3.Distance (t_node.transform.position, node.transform.position);

		if(distance > distanceOutTemplate.tp2)
		{
			triggerFunc(distanceOutTemplate);
		}
	}

	private void updateInDistance()
	{
		if(distanceInTemplate == null) return;
		
		BaseAI t_node = BattleControlor.Instance().getNodebyId ((int)distanceInTemplate.tp1);
		
		if (t_node == null || t_node.gameObject.activeSelf == false || t_node.isAlive == false || t_node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) <= 0) 
		{
			return;
		}
		
		float distance = Vector3.Distance (t_node.transform.position, node.transform.position);
		
		if(distance < distanceInTemplate.tp2)
		{
			triggerFunc(distanceInTemplate);
		}
	}

	private void updateTime()
	{
		if (tempTime == 0) return;

		float now = Time.realtimeSinceStartup;

		if (now - tempTime > 1.5f)
		{
			close ();
		}
	}

	public void close()
	{
		if (gc == null) return;

		gc.SetActive(false);
		
		tempTime = 0;
	}

}

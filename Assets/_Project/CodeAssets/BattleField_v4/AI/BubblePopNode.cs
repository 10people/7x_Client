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

	private UISprite spriteJian;

	private float tempTime;

	private BubblePopTemplate distanceOutTemplate;

	private BubblePopTemplate distanceInTemplate;

	private float cdTime;

	private Vector3 offset;


	public void init()
	{
		tempTime = 0;

		distanceOutTemplate = null;

		distanceInTemplate = null;

		cdTime = 0;

		offset = Vector3.zero;

		dict.TryGetValue (3, out distanceOutTemplate);

		dict.TryGetValue (4, out distanceInTemplate);

		foreach(BubblePopTemplate temp in dict.Values)
		{
			temp.triggerCurNum = 0;
		}
	}

	public void triggerFunc(BubblePopTemplate template)
	{
		if(cdTime < template.cdTime) return;

		if(gc == null)
		{
			gc = Instantiate(gcTemple);

			gc.transform.parent = node.transform;

			gc.transform.localScale = new Vector3(1 / node.transform.localScale.x, 1 / node.transform.localScale.y, 1 / node.transform.localScale.z );

			gc.transform.localPosition = new Vector3(0, node.getHeight() + .2f, 0);

			label = gc.GetComponentInChildren<UILabel>();

			UISprite[] sprites = gc.GetComponentsInChildren<UISprite>();

			foreach(UISprite sprite in sprites)
			{
				if(sprite.spriteName.IndexOf("Jian") != -1)
				{
					spriteJian = sprite;

					break;
				}
			}

			gc.SetActive(false);
		}

		int rand = ((int)(Random.value * 100)) % template.listTextId.Count;

		int textId = template.listTextId [rand];

		if (textId == 0) return;

		if (template.triggerCurNum >= template.triggerNum) return;

		template.triggerCurNum ++;

		cdTime = 0;

		if(BattleUIControlor.Instance().barAPC.gameObject.activeSelf == true
		   && BattleUIControlor.Instance().barAPC.getFocusNode().nodeId == node.nodeId)
		{
			Vector3 position2D = Camera.main.WorldToViewportPoint(node.transform.position);

			spriteJian.transform.localPosition = new Vector3(0, 0.1f, 0);
			
			spriteJian.transform.localEulerAngles = new Vector3(0, 0, 0);
			
			spriteJian.transform.localScale = new Vector3(.02f, .02f, .02f);

			offset = new Vector3(0, node.getHeight() + .2f, 0);

			if(position2D.x < 0 || position2D.x > 1 || position2D.y < 0 || position2D.y > 1)
			{
				BattleUIControlor.Instance().barAPC.setBubbleText(BubbleTextTemplate.getBubbleTextById (textId));

				return;
			}
		}
		else
		{
			offset = new Vector3(0, node.getHeight() + .2f, 0);

			spriteJian.transform.localPosition = new Vector3(0, 0.1f, 0);
			
			spriteJian.transform.localEulerAngles = new Vector3(0, 0, 0);
			
			spriteJian.transform.localScale = new Vector3(.02f, .02f, .02f);

			Vector3 position2D = Camera.main.WorldToViewportPoint(node.transform.position + offset + new Vector3(0, .8f, 0));//加上锚点偏移
			
			bool inScreen = !(position2D.x < 0 || position2D.x > 1 || position2D.y < 0 || position2D.y > 1);

			if(!inScreen)
			{
				UIAnchor.Side leftOrRight = UIAnchor.Side.Left;

				offset = new Vector3(2.8f, node.getHeight() / 2, 0);

//				spriteJian.transform.localPosition = new Vector3(-2.27f, 0.5f, 0);
//
//				spriteJian.transform.localEulerAngles = new Vector3(0, 0, 270f);
//				
//				spriteJian.transform.localScale = new Vector3(.02f, .02f, .02f);

				float angle = Vector3.Angle(node.transform.forward + new Vector3(0, -node.transform.forward.y, 0), Camera.main.transform.forward + new Vector3(0, -Camera.main.transform.forward.y, 0));

				if(Mathf.Abs(angle) > 90)//怪与镜头反方向
				{
					leftOrRight = UIAnchor.Side.Right;
				}
				else
				{
					leftOrRight = UIAnchor.Side.Left;
				}

				position2D = Camera.main.WorldToViewportPoint(node.transform.position + offset + new Vector3(0, .8f, 0));//加上锚点偏移

				inScreen = !(position2D.x < 0 || position2D.x > 1 || position2D.y < 0 || position2D.y > 1);

				if(!inScreen)
				{
					offset = new Vector3(-2.8f, node.getHeight() / 2, 0);

//					spriteJian.transform.localPosition = new Vector3(2.24f, 0.5f, 0);
//					
//					spriteJian.transform.localEulerAngles = new Vector3(0, 0, 270f);
//
//					spriteJian.transform.localScale = new Vector3(.02f, -.02f, .02f);

					if(Mathf.Abs(angle) > 90)//怪与镜头反方向
					{
						leftOrRight = UIAnchor.Side.Left;
					}
					else
					{
						leftOrRight = UIAnchor.Side.Right;
					}
				}

				if(leftOrRight == UIAnchor.Side.Left)
				{
					spriteJian.transform.localPosition = new Vector3(-2.27f, 0.5f, 0);
	
					spriteJian.transform.localEulerAngles = new Vector3(0, 0, 270f);
					
					spriteJian.transform.localScale = new Vector3(.02f, .02f, .02f);
				}
				else
				{
					spriteJian.transform.localPosition = new Vector3(2.24f, 0.5f, 0);

					spriteJian.transform.localEulerAngles = new Vector3(0, 0, 270f);

					spriteJian.transform.localScale = new Vector3(.02f, -.02f, .02f);
				}
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
		updateCDTime ();

		updateOutDistance ();
		
		updateInDistance ();

		updateForward ();

		updateTime ();
	}

	private void updateForward()
	{
		if (Camera.main == null) return;
		
		if (gc == null) return;

		Vector3 tempNodeForward = node.transform.forward;

		gc.transform.localPosition = Vector3.zero;

		node.transform.forward = (Camera.main.transform.position - node.transform.position);

		gc.transform.localPosition += offset;

		node.transform.forward = tempNodeForward;

		gc.transform.forward = Camera.main.transform.forward;
	}

	private void updateOutDistance()
	{
		if(distanceOutTemplate == null) return;

		BaseAI t_node = BattleControlor.Instance().getNodebyId ((int)distanceOutTemplate.tp1);

		if (t_node == null || t_node.gameObject.activeSelf == false || t_node.isAlive == false || t_node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) 
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
		
		if (t_node == null || t_node.gameObject.activeSelf == false || t_node.isAlive == false || t_node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) 
		{
			return;
		}
		
		float distance = Vector3.Distance (t_node.transform.position, node.transform.position);
		
		if(distance < distanceInTemplate.tp2)
		{
			triggerFunc(distanceInTemplate);
		}
	}

	private void updateCDTime()
	{
		cdTime += Time.deltaTime;
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

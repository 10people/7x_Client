using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class SignShadow : MonoBehaviour
{
	public GameObject signRing_B;

	public GameObject signRing_R;

	public GameObject signRing_B_R;

	public GameObject signRing_R_B;

	public List<GameObject> signArrows;


	private KingControllor node;

	private BaseAI nextNode;

	private GameObject unrealObject;

	private List<Renderer> arrowRender = new List<Renderer>();

	private bool enterFight;


	public static SignShadow create()
	{
		GameObject gc = (GameObject) Instantiate (BattleEffectControllor.Instance().getEffect(1030));

		gc.transform.parent = BattleControlor.Instance ().getKing ().transform;

		gc.transform.localPosition = Vector3.zero;

		gc.transform.localScale = new Vector3 (1, 1, 1);

		gc.transform.localEulerAngles = Vector3.zero;

		SignShadow ss = gc.GetComponent<SignShadow> ();

		ss.init ();

		return ss;
	}

	private void init()
	{
		node = BattleControlor.Instance ().getKing ();

		node.signShadow = this;

		unrealObject = new GameObject ();

		unrealObject.transform.parent = transform;

		unrealObject.name = "unrealObject";

		unrealObject.transform.localPosition = Vector3.zero;

		unrealObject.transform.localScale = new Vector3 (1, 1, 1);

		arrowRender.Clear ();

		foreach(GameObject arrowObject in signArrows)
		{
			arrowRender.Add(arrowObject.GetComponentInChildren<Renderer>());
		}

		enterFight = false;
		
		signRing_B.SetActive (true);

		signRing_R.SetActive (false);

		signRing_B_R.SetActive (false);

		signRing_R_B.SetActive (false);

		LateUpdate ();
	}

	private void refreshnextNode(int nodeType)
	{
		float length = 999;

		foreach(BaseAI t_node in BattleControlor.Instance().enemyNodes)
		{
			bool nFlag = t_node == null 
				|| t_node.gameObject.activeSelf == false 
				|| t_node.isAlive == false 
				|| t_node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0 
				|| t_node.targetNode == null
				|| t_node.nodeData.nodeType == NodeType.NPC
				|| t_node.nodeData.nodeType == NodeType.GOD;

			if(nFlag == true) continue;

			if(nodeType != 0 && (int)t_node.nodeData.nodeType != nodeType) continue;

			float t_length = Vector3.Distance(BattleControlor.Instance().getKing().transform.position, t_node.transform.position);

			if(t_length < length)
			{
				length = t_length;

				nextNode = t_node;
			}
		}
	}

	private void refreshnextNodeById(int nodeType)
	{
		int nodeId = 999;
		
		foreach(BaseAI t_node in BattleControlor.Instance().enemyNodes)
		{
			bool nFlag = t_node == null 
				|| t_node.isAlive == false 
				|| t_node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0
				|| t_node.nodeData.nodeType == NodeType.NPC
				|| t_node.nodeData.nodeType == NodeType.GOD;

			if(nFlag == true) continue;

			if(nodeType != 0 && (int)t_node.nodeData.nodeType != nodeType) continue;

			int t_id = t_node.nodeId;

			if(t_id < nodeId && t_node != null)
			{
				nodeId = t_id;

				nextNode = t_node;
			}
		}
	}

	public void LateUpdate ()
	{
		UpdateArrow ();

		UpdateRing ();
	}

	Vector3 UpdateArrow_Node (int nodeType)
	{
		bool nFlag = nextNode == null || nextNode.gameObject.activeSelf == false || nextNode.isAlive == false || nextNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0;

		if(nFlag == true)
		{
			refreshnextNode(nodeType);
		}

		nFlag = nextNode == null || nextNode.gameObject.activeSelf == false || nextNode.isAlive == false || nextNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0;

		if (nFlag == true)
		{
			refreshnextNodeById(nodeType);
		}

		nFlag = nextNode == null || nextNode.isAlive == false || nextNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0;

		if (nFlag == true) 
		{
			foreach(GameObject arrowObject in signArrows)
			{
				arrowObject.SetActive(false);
			}

			return Vector3.zero;
		}

		foreach(GameObject arrowObject in signArrows)
		{
			arrowObject.SetActive(true);
		}

		return nextNode.transform.position;
	}

	void UpdateArrow()
	{
		Vector3 targetPo = Vector3.zero;

		targetPo = UpdateArrow_Node (0);

		if(Vector3.Distance(targetPo, Vector3.zero) < .2f)
		{
			BattleWinTemplate winTemplate = BattleWinTemplate.getWinTemplateContainsType(BattleWinFlag.EndType.PROTECT, true);

			if(winTemplate != null)
			{
				foreach(int nodeId in winTemplate.protectList)
				{
					if(nodeId != 1)
					{
						targetPo = BattleControlor.Instance().getNodebyId(nodeId).transform.position;

						break;
					}
				}
			}
		}

		if(Vector3.Distance(targetPo, Vector3.zero) < .2f)
		{
			BattleWinTemplate winTemplate = BattleWinTemplate.getWinTemplateContainsType(BattleWinFlag.EndType.Reach_Destination, true);

			if(winTemplate != null)
			{
				targetPo = winTemplate.destination;
			}
		}

		if(Vector3.Distance(targetPo, Vector3.zero) < .2f)
		{
			return;
		}

		if(Vector3.Distance(targetPo - node.transform.position, Vector3.zero) > .2f)
		{
			unrealObject.transform.forward = targetPo - node.transform.position;
		}

		foreach(GameObject arrowObject in signArrows)
		{
			arrowObject.transform.localEulerAngles = new Vector3 (0, unrealObject.transform.localEulerAngles.y, 0);
		}

		float length = Vector3.Distance (targetPo, node.transform.position);

		length = length > 10f ? 10f : length;

		float alpha = 0f;

		if(length > 5f)
		{
			alpha = .5f * (length - 5f) / 5f;
		}

		foreach(Renderer renderer in arrowRender)
		{
			renderer.material.SetColor ("_TintColor", new Color(.5f, .5f, .5f, alpha));
		}
	}

	void UpdateRing()
	{
		//if (enterFight == false) return;

		bool inFight = false;

		foreach(BaseAI t_node in BattleControlor.Instance().enemyNodes)
		{
			bool nFlag = t_node == null 
				|| t_node.isAlive == false 
				|| t_node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0
				|| t_node.nodeData.nodeType == NodeType.NPC
				|| t_node.nodeData.nodeType == NodeType.GOD;
			
			if(nFlag == true) continue;

			if(t_node.targetNode != null)
			{
				inFight = true;

				break;
			}
		}

		if(inFight == true && enterFight == false)
		{
			setEnterBattle();
		}

		if(inFight == false && enterFight == true)
		{
			setExitBattle();
		}
	}

	private void setEnterBattle()
	{
		enterFight = true;

		StartCoroutine (enterBattleAction());
	}

	IEnumerator enterBattleAction()
	{
		signRing_B.SetActive (false);
		
		signRing_R.SetActive (false);
		
		signRing_B_R.SetActive (true);
		
		signRing_R_B.SetActive (false);

		yield return new WaitForSeconds(.5f);

		if(enterFight == true)
		{
			signRing_B.SetActive (false);
			
			signRing_R.SetActive (true);
			
			signRing_B_R.SetActive (false);
			
			signRing_R_B.SetActive (false);
		}
	}

	private void setExitBattle()
	{
		enterFight = false;

		StartCoroutine (exitBattleAction());
	}

	IEnumerator exitBattleAction()
	{
		signRing_B.SetActive (false);
		
		signRing_R.SetActive (false);
		
		signRing_B_R.SetActive (false);
		
		signRing_R_B.SetActive (true);
		
		yield return new WaitForSeconds(.5f);

		if(enterFight == false)
		{
			signRing_B.SetActive (true);
			
			signRing_R.SetActive (false);
			
			signRing_B_R.SetActive (false);
			
			signRing_R_B.SetActive (false);
		}
	}

}

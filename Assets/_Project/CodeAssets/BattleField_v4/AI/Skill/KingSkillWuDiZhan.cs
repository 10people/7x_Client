using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  无敌斩
*/
public class KingSkillWuDiZhan : MonoBehaviour 
{

	[HideInInspector] public BaseAI curNode;

	[HideInInspector] public float radius = 5;


	private KingControllor king;

	private List<BaseAI> nodeList = new List<BaseAI> ();

	private List<BaseAI> deadList = new List<BaseAI> ();

	private Vector3 tempPos;


	private void init()
	{
		king = gameObject.GetComponent<KingControllor> ();

		tempPos = gameObject.transform.position;

		SkillTemplate template = SkillTemplate.getSkillTemplateById (200021);

		radius = template.value1;
	}

	public void chooseTarget_skill_1(int index)
	{
		if(index == 0)
		{
			init();

			if(king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.dark();
		}
		else
		{
			if(CityGlobalData.skillLevel[(int)CityGlobalData.skillLevelId.jueyingxingguangzhan] == 0)
			{
				if((index + 1) % 3 == 0) return;
			}
		}

		Vector3 vcZero = curNode == null ? king.transform.position : curNode.transform.position;

		List<BaseAI> ns = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;

		nodeList.Clear ();

		deadList.Clear ();

		foreach(BaseAI n in ns)
		{
			if(n.gameObject.activeSelf == true
			   && Vector3.Distance(n.transform.position, vcZero) < radius 
			   && n.isAlive
			   && n.nodeData.nodeType != NodeType.GOD
			   && n.nodeData.nodeType != NodeType.NPC)
			{
				if( n.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) > 0)
				{
					nodeList.Add(n);
				}
				else
				{
					deadList.Add(n);
				}
			}
		}

		curNode = null;

		if(nodeList.Count > 0) curNode = getCurNode(nodeList);

		else if(deadList.Count > 0) curNode = getCurNode(deadList);

		if (curNode == null) return;

		transform.forward = curNode.transform.position - transform.position;

		float l = Vector3.Distance(transform.position, curNode.transform.position);

		iTween.MoveTo(gameObject, iTween.Hash(
			"name", "WuDiZhan",
			"position", transform.position + transform.forward * (l - 1.2f),
			"time", .05f,
			"easeType", iTween.EaseType.easeOutQuint
			));
	}

	private BaseAI getCurNode(List<BaseAI> list)
	{
		if(list.Count == 0) return null;

		BaseAI t_node = list[0];

		foreach(BaseAI node in list)
		{
			if(node.nodeData.nodeType > t_node.nodeData.nodeType)
			{
				t_node = node;
			}
		}

		return t_node;
	}

	public void skill_1_resetPosition()
	{
		iTween.MoveTo(gameObject, iTween.Hash(
			"name", "WuDiZhan",
			"position", tempPos,
			"time", .05f,
			"easeType", iTween.EaseType.easeOutQuint
			));
	}

	public void WudizhanDone()
	{
		List<BaseAI> ns = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;

		Vector3 tempFow = transform.forward;

		SkillTemplate template = SkillTemplate.getSkillTemplateById (200021);

		foreach(BaseAI n in ns)
		{
			float l = Vector3.Distance(transform.position, n.transform.position);
			
			if(l < template.value6 && n.isAlive)
			{
				transform.forward = n.transform.position - transform.position;

				Vector3 targetP = transform.position + transform.forward * (l + 2f);

				StartCoroutine(n.attackedMovement(0.05f, targetP, iTween.EaseType.easeOutExpo, 0.2f));
			
				if(n.nodeData.nodeType != qxmobile.protobuf.NodeType.BOSS)
				{
					//n.mAnim.SetTrigger(n.getAnimationName(BaseAI.AniType.ANI_BATCDown));

					n.beatDown(101);
				}
			}
		}

		transform.forward = tempFow;
	}

}

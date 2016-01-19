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

	private Vector3 tempPos;

	private GameObject copyObject;

	private Animator copyAnim;

	private string colorName = "Main Color";

	private Vector3 curNodePosition;

	private Vector3[] offsetPos = new Vector3[]{
		new Vector3(0f, 0f, 1f),
		new Vector3(-1.01f, 0f, -0.36f),
		new Vector3(0.98f, 0f, -0.49f),
		new Vector3(-1.07f, 0f, 0.67f),
		new Vector3(1.04f, 0f, 0.49f),
		new Vector3(0f, 0f, -1f),
	};


	void Start()
	{
		king = gameObject.GetComponent<KingControllor> ();
		
		if(copyObject == null) copyObject = Instantiate (gameObject);

		copyObject.name = "WuDiZhanCopy";

		copyObject.SetActive (false);

		copyObject.transform.parent = transform.parent;

		copyObject.transform.localScale = transform.localScale;

		copyObject.transform.position = new Vector3(0, -1000, 0);

		KingControllor copyKing = copyObject.GetComponent<KingControllor> ();

		DestroyObject (copyKing.m_weapon_Heavy);

		DestroyObject (copyKing.m_weapon_Ranged);

		Destroy (copyKing);

		Destroy (copyObject.GetComponent<KingSkillWuDiZhan>());

		Destroy (copyObject.GetComponent<KingSkillXuanFengZhan>());

		Destroy (copyObject.GetComponent<CharacterController>());

		Destroy (copyObject.GetComponent<NavMeshAgent>());

		Destroy (copyObject.GetComponent<SphereCollider>());

		copyObject.AddComponent<DramaStorySimulation>();

		foreach(HeroSkill skill in copyObject.GetComponents<HeroSkill>())
		{
			Destroy(skill);
		}

		Global.ResourcesDotLoad("_3D/Models/BattleField/DramaControllor/DramaControllor_" + king.modelId,
		                        loadControllorCallback );
	}

	
	
	void OnDestroy()
	{
		curNode = null;
		
		king = null;

		copyObject = null;

		if( copyAnim != null ){
			copyAnim.runtimeAnimatorController = null;
		}

		copyAnim = null;
	}

	void loadControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		copyAnim = copyObject.GetComponent<Animator>();

		copyAnim.runtimeAnimatorController = (RuntimeAnimatorController)p_object;
	}

	private void init()
	{
		tempPos = gameObject.transform.position;

		SkillTemplate template = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);

		radius = template.value1;

		curNodePosition = transform.position + (transform.forward * 3);

		curNode = getCurNode ();

		if(curNode != null) curNodePosition = curNode.transform.position;

		king.signShadow.gameObject.SetActive (false);

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 1f,
			"to", 0f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));

		if(curNode != null) king.gameCamera.targetChang (curNode.gameObject);
	}

	public void chooseTarget_skill_1(int index)
	{
		if(index == 0) init();

		if(SkillTemplate.getSkillLevelBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king) == 0)
		{
			if((index + 1) % 3 == 0) return;
		}

		if(curNode == null || curNode.isAlive == false || curNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) <= 0)
		{
			curNode = getCurNode();

			if(curNode != null) curNodePosition = curNode.transform.position;
		}

		float length = 5.5f;

		copyObject.SetActive (true);

		copyObject.transform.position = (curNode != null ? curNode.transform.position : curNodePosition) + (offsetPos [index % offsetPos.Length].normalized * length);

		copyObject.transform.forward = (curNode != null ? curNode.transform.position : curNodePosition) - copyObject.transform.position;

		copyAnim.Play ("attack_" + ((index % 4) + 1));

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", .15f,
			"onupdate", "setAlphaCopy",
			"easetype", iTween.EaseType.easeInExpo
			));

		iTween.MoveTo (copyObject, iTween.Hash(
			"position", copyObject.transform.position + copyObject.transform.forward * (length - 1),
			"time", 0.18f,
			"easetype", iTween.EaseType.easeInOutQuart
			));

		BattleEffectControllor.Instance ().PlayEffect (600211, copyObject.transform.position, copyObject.transform.forward);
	}

	private BaseAI getCurNode()
	{
		List<BaseAI> list = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;

		if(list.Count == 0) return null;

		BaseAI t_node = null;

		foreach(BaseAI node in list)
		{
			if(node.gameObject.activeSelf == true
			   && Vector3.Distance(node.transform.position, transform.position) < radius 
			   && node.isAlive
			   && node.nodeData.nodeType != NodeType.GOD
			   && node.nodeData.nodeType != NodeType.NPC
			   && node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) > 0)
			{
				if(t_node == null || node.nodeData.nodeType > t_node.nodeData.nodeType)
				{
					t_node = node;
				}
			}
		}

		return t_node;
	}

	void setWeaponHurt(int _aid)
	{
		Vector3 effectPo = curNodePosition;

		if (curNode != null) effectPo = curNode.transform.position;

		if(curNode != null)
		{
			BattleEffectControllor.Instance ().PlayEffect (600210, curNode.gameObject);
		}

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);
		
		KingSkillWuDiZhan wudizhan = (KingSkillWuDiZhan)gameObject.GetComponent("KingSkillWuDiZhan");
		
		int count = 0;
		
		if(wudizhan.curNode != null)
		{
			foreach(Buff buff in wudizhan.curNode.buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_Focus)
				{
					count++;
				}
			}
			
			count = count > (int)skillTemplate.value5 ? (int)skillTemplate.value5 : count;
			
			FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(
				king, 
				wudizhan.curNode, 
				skillTemplate.value2 + count * skillTemplate.value4, 
				0
				);
			
			foreach(Buff buff in wudizhan.curNode.buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
				{
					king.attackHp(king, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_REFLEX);
					
					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

					wudizhan.curNode.showText(LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

					break;
				}
			}
			
			king.attackHp(wudizhan.curNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK);
			
			Buff.createBuff(wudizhan.curNode, AIdata.AttributeType.ATTRTYPE_Focus, 0, 5f);
			
			king.Shake(KingCamera.ShakeType.Cri);
		}
	}

	public void skill_1_resetPosition()
	{
		king.signShadow.gameObject.SetActive (true);

		copyObject.SetActive (false);
		
		copyObject.transform.position = new Vector3(0, -1000, 0);

		king.gameCamera.targetChang (king.gameObject);

		iTween.MoveTo(gameObject, iTween.Hash(
			"name", "WuDiZhan",
			"position", tempPos,
			"time", .05f,
			"easeType", iTween.EaseType.easeOutQuint
			));

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));
	}

	public void WudizhanDone()
	{
		List<BaseAI> ns = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;

		Vector3 tempFow = transform.forward;

		SkillTemplate template = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);

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

	void setAlphaSelf(float alpha)
	{
		Renderer[] rds = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(m.shader.name.Equals("Custom/Characters/Main Texture High Light"))
				{
					m.color = new Color( 0.537f, 0.537f, 0.537f, alpha);
				}
			}
		}
	}

	void setAlphaCopy(float alpha)
	{
		Renderer[] rds = copyObject.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(m.shader.name.Equals("Custom/Characters/Main Texture High Light"))
				{
					m.color = new Color( 0.537f, 0.537f, 0.537f, alpha);
				}
			}
		}
	}

}

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

	private float curNavSpeed;

	private int curCount;

	private int countMax;

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

		curCount = 0;
	}
	
	void OnDestroy()
	{
		curNode = null;
		
		king = null;

		copyObject = null;

		if( copyAnim != null )
		{
			copyAnim.runtimeAnimatorController = null;
		}

		copyAnim = null;
	}

	void loadControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		copyAnim = copyObject.GetComponent<Animator>();

		copyAnim.runtimeAnimatorController = (RuntimeAnimatorController)p_object;
	}

	private void initSkill_1()
	{
		king.actionId = 145;

		tempPos = gameObject.transform.position;

		SkillTemplate template = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);

		radius = template.value1;

		countMax = (int)template.value3;

		curNodePosition = transform.position + (transform.forward * 3);

		curNode = getCurNode ();

		if(curNode != null) curNodePosition = curNode.transform.position;

		if(king.signShadow != null) king.signShadow.gameObject.SetActive (false);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (false);

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 1f,
			"to", 0f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));

		if(curNode != null && king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (curNode.gameObject);
	}

	private void initSkill_2()
	{
		king.actionId = 150;
		
		tempPos = gameObject.transform.position;
		
		SkillTemplate template = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.xuejilaoyin, king);
		
		radius = template.value1;
		
		curNodePosition = transform.position + (transform.forward * 3);
		
		curNode = getCurNode ();
		
		if(curNode != null) curNodePosition = curNode.transform.position + (curNode.transform.forward * -(curNode.radius + king.radius));
		
		if(king.signShadow != null) king.signShadow.gameObject.SetActive (false);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (false);

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 1f,
			"to", 0f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));
		
		if(curNode != null && king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (curNode.gameObject);

		copyObject.SetActive (true);

		copyObject.transform.position = curNodePosition;

		if(curNode != null) copyObject.transform.forward = curNode.transform.position - copyObject.transform.position;

		else copyObject.transform.forward = king.transform.forward;

		curNavSpeed = king.getNavMeshSpeedReal ();

		king.setNavMeshSpeedReal (1000);

		king.setNavMeshDestinationReal (copyObject.transform.position);
	}

	public void chooseTarget_skill_1()
	{
		if(curCount == 0) initSkill_1();

//		if(SkillTemplate.getSkillLevelBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king) == 0)
//		{
//			if((curCount + 1) % 3 == 0) return;
//		}

		if(curNode == null || curNode.isAlive == false || curNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0)
		{
			curNode = getCurNode();

			if(curNode != null) curNodePosition = curNode.transform.position;
		}

		float length = 5.5f;

		copyObject.SetActive (true);

		copyObject.transform.position = (curNode != null ? curNode.transform.position : curNodePosition) + (offsetPos [curCount % offsetPos.Length].normalized * length);

		copyObject.transform.forward = (curNode != null ? curNode.transform.position : curNodePosition) - copyObject.transform.position;

		copyAnim.Play ("JYXGZ_" + ((curCount % 4) + 1));

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

		BattleEffectControllor.Instance().PlayEffect (600211, copyObject.transform.position, copyObject.transform.forward);

		curCount ++;

		if(curCount >= countMax)
		{
			king.mAnim.SetBool("WuDiOver", true);
		}
	}

	public void chooseTarget_skill_2(int index)
	{
		if(index == 0) initSkill_2();
		
//		if(curNode == null || curNode.isAlive == false || curNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0)
//		{
//			curNode = getCurNode();
//			
//			if(curNode != null) curNodePosition = curNode.transform.position;
//		}
		
		copyAnim.Play ("XJLY_" + index);

//		iTween.ValueTo (gameObject, iTween.Hash(
//			"from", 0f,
//			"to", 1f,
//			"time", .15f,
//			"onupdate", "setAlphaCopy",
//			"easetype", iTween.EaseType.easeInExpo
//			));
//
//		iTween.MoveTo (copyObject, iTween.Hash(
//			"position", copyObject.transform.position + copyObject.transform.forward * (length - 1),
//			"time", 0.18f,
//			"easetype", iTween.EaseType.easeInOutQuart
//			));

		if(index == 0)
		{
			BattleEffectControllor.Instance().PlayEffect (600227, copyObject.transform.position, copyObject.transform.forward);
		}
		else if(index  == 1)
		{
			BattleEffectControllor.Instance().PlayEffect (600228, copyObject.transform.position, copyObject.transform.forward);
		}
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
			   && node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) >= 0)
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
//		1140-1151

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);
		
		int index = _aid % 10;

		if(index >= skillTemplate.value3)
		{
			return;
		}

		Vector3 effectPo = curNodePosition;

		if (curNode != null) effectPo = curNode.transform.position;

		if(curNode != null)
		{
			BattleEffectControllor.Instance().PlayEffect (600210, curNode.gameObject);
		}
		
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
					king.attackHp(king, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_REFLEX, BattleControlor.NuqiAddType.NULL);
					
					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

					wudizhan.curNode.showText(LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

					break;
				}
			}
			
			king.attackHp(wudizhan.curNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.LIGHT_SKILL_1);
			
			Buff.createBuff(wudizhan.curNode, AIdata.AttributeType.ATTRTYPE_Focus, 0, 5f);
			
			if(king.stance == BaseAI.Stance.STANCE_SELF) king.Shake(KingCamera.ShakeType.Cri);
		}
	}

	void setWeaponHurtSkill_2(int index)
	{
//		1600

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.xuejilaoyin, king);
		
		Vector3 effectPo = curNodePosition;
		
		if (curNode != null) effectPo = curNode.transform.position;
		
		if(curNode != null)
		{
			BattleEffectControllor.Instance().PlayEffect (600210, curNode.gameObject);
		}
		
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
				index == 0 ? skillTemplate.value2 : skillTemplate.value3, 
				0
				);
			
			foreach(Buff buff in wudizhan.curNode.buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
				{
					king.attackHp(king, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_REFLEX, BattleControlor.NuqiAddType.NULL);
					
					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;
					
					wudizhan.curNode.showText(LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);
					
					break;
				}
			}
			
			king.attackHp(wudizhan.curNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.LIGHT_SKILL_1);
			
			if(index == 1) BattleEffectControllor.Instance().PlayEffect (600229, wudizhan.curNode.transform.position, copyObject.transform.forward);
			
			if(king.stance == BaseAI.Stance.STANCE_SELF) king.Shake(KingCamera.ShakeType.Cri);
		}
	}

	public void skill_1_resetPosition()
	{
		curCount = 0;

		king.mAnim.SetBool("WuDiOver", false);

		if(king.signShadow != null) king.signShadow.gameObject.SetActive (true);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (true);

		copyObject.SetActive (false);
		
		copyObject.transform.position = new Vector3(0, -1000, 0);

		if(king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (king.gameObject);

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

	public void skill_2_resetPosition()
	{
		king.setNavMeshStop ();

		king.transform.forward = copyObject.transform.forward;

		if(king.signShadow != null) king.signShadow.gameObject.SetActive (true);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (true);

		BattleEffectControllor.Instance().PlayEffect (600230, copyObject.transform.position, copyObject.transform.forward);

		copyObject.SetActive (false);
		
		copyObject.transform.position = new Vector3(0, -1000, 0);
		
		if(king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (king.gameObject);

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));

		king.setNavMeshSpeedReal (curNavSpeed);
	}

	public void WudizhanDone()
	{
		List<BaseAI> ns = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;

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

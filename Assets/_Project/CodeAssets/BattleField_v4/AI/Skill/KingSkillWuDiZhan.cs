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

	private string colorName = "Main Color";

	private Vector3 curNodePosition;

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

	private bool inSkill;


	void Start()
	{
		king = gameObject.GetComponent<KingControllor> ();
		
		if(king.copyObject == null) king.copyObject = Instantiate (gameObject);

		king.copyObject.name = "WuDiZhanCopy";

		king.copyObject.SetActive (false);

		king.copyObject.transform.parent = transform.parent;

		king.copyObject.transform.localScale = transform.localScale;

		king.copyObject.transform.position = new Vector3(0, -1000, 0);

		KingControllor copyKing = king.copyObject.GetComponent<KingControllor> ();

		DestroyObject (copyKing.m_weapon_Light_left);

		DestroyObject (copyKing.m_weapon_Light_right);

		DestroyObject (copyKing.m_weapon_Heavy);

		DestroyObject (copyKing.m_weapon_Ranged);

		Destroy (copyKing);

		Destroy (king.copyObject.GetComponent<KingSkillWuDiZhan>());

		Destroy (king.copyObject.GetComponent<KingSkillXuanFengZhan>());

		Destroy (king.copyObject.GetComponent<CharacterController>());

		Destroy (king.copyObject.GetComponent<NavMeshAgent>());

		Destroy (king.copyObject.GetComponent<SphereCollider>());

		Destroy (king.copyObject.GetComponent<SoundPlayEff>());

		king.copyObject.AddComponent<DramaStorySimulation_2>();

		foreach(HeroSkill skill in king.copyObject.GetComponents<HeroSkill>())
		{
			Destroy(skill);
		}

		SphereCollider sc = king.copyObject.AddComponent<SphereCollider>();

		sc.radius = 0;

		sc.center = Vector3.zero;

		king.copyAnim = king.copyObject.GetComponent<Animator>();

		Global.ResourcesDotLoad("_3D/Models/BattleField/DramaControllor/DramaControllor_" + (510 + ((king.modelId - 1001) * 10) + 1),
		                        loadHeavyControllorCallback );

		Global.ResourcesDotLoad("_3D/Models/BattleField/DramaControllor/DramaControllor_" + (510 + ((king.modelId - 1001) * 10) + 2),
		                        loadLightControllorCallback );

		Global.ResourcesDotLoad("_3D/Models/BattleField/DramaControllor/DramaControllor_" + (510 + ((king.modelId - 1001) * 10) + 3),
		                        loadRangeControllorCallback );

		curCount = 0;

		inSkill = false;
	}
	
	void OnDestroy()
	{
		curNode = null;

		if (king == null) return;

		king.copyObject = null;

		if( king.copyAnim != null )
		{
			king.copyAnim.runtimeAnimatorController = null;
		}

		king.copyAnim = null;

		king = null;
	}

	void loadHeavyControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		king.copyRuntimeControllorHeavy = (RuntimeAnimatorController)p_object;
	}

	void loadLightControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		king.copyRuntimeControllorLight = (RuntimeAnimatorController)p_object;
	}

	void loadRangeControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		king.copyRuntimeControllorRange = (RuntimeAnimatorController)p_object;
	}

	private void initSkill_1()
	{
		inSkill = true;

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
		inSkill = true;

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

		king.copyObject.SetActive (true);

		king.copyObject.transform.position = curNodePosition;

		if(curNode != null) king.copyObject.transform.forward = curNode.transform.position - king.copyObject.transform.position;

		else king.copyObject.transform.forward = king.transform.forward;

		king.moveAction (curNodePosition, iTween.EaseType.linear, .1f, 1);
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

		king.copyObject.SetActive (true);

		king.copyObject.transform.position = (curNode != null ? curNode.transform.position : curNodePosition) + (offsetPos [curCount % offsetPos.Length].normalized * length);

		king.copyObject.transform.forward = (curNode != null ? curNode.transform.position : curNodePosition) - king.copyObject.transform.position;

		if(king.weaponType == KingControllor.WeaponType.W_Heavy)
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorHeavy;
		}
		else if(king.weaponType == KingControllor.WeaponType.W_Light)
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorLight;
		}
		else
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorRange;
		}

		king.copyAnim.Play ("JYXGZ_" + ((curCount % 4) + 1));

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", .15f,
			"onupdate", "setAlphaCopy",
			"easetype", iTween.EaseType.easeInExpo
			));

		iTween.MoveTo (king.copyObject, iTween.Hash(
			"position", king.copyObject.transform.position + king.copyObject.transform.forward * (length - 1),
			"time", 0.18f,
			"easetype", iTween.EaseType.easeInOutQuart
			));

		if(king.modelId == 1005)
		{
			BattleEffectControllor.Instance().PlayEffect (600267, king.copyObject.transform.position, king.copyObject.transform.forward);
		}
		else if(king.modelId == 1002)
		{
			BattleEffectControllor.Instance().PlayEffect (600281, king.copyObject.transform.position, king.copyObject.transform.forward);
		}
		else if(king.modelId == 1003)
		{
			BattleEffectControllor.Instance().PlayEffect (600310, king.copyObject.transform.position, king.copyObject.transform.forward);
		}
		else
		{
			BattleEffectControllor.Instance().PlayEffect (600211, king.copyObject.transform.position, king.copyObject.transform.forward);
		}

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

		if(king.weaponType == KingControllor.WeaponType.W_Heavy)
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorHeavy;
		}
		else if(king.weaponType == KingControllor.WeaponType.W_Light)
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorLight;
		}
		else
		{
			king.copyAnim.runtimeAnimatorController = king.copyRuntimeControllorRange;
		}

		king.copyAnim.Play ("XJLY_" + index);

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
			if(king.modelId == 1005)
			{
				BattleEffectControllor.Instance().PlayEffect (600265, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
			else if(king.modelId == 1002)
			{
				BattleEffectControllor.Instance().PlayEffect (600278, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
			else if(king.modelId == 1003)
			{
				BattleEffectControllor.Instance().PlayEffect (600311, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
			else
			{
				BattleEffectControllor.Instance().PlayEffect (600227, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
		}
		else if(index  == 1)
		{
			if(king.modelId == 1002)
			{
				BattleEffectControllor.Instance().PlayEffect (600279, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
			else if(king.modelId == 1003)
			{
				BattleEffectControllor.Instance().PlayEffect (600312, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
			else
			{
				BattleEffectControllor.Instance().PlayEffect (600228, king.copyObject.transform.position, king.copyObject.transform.forward);
			}
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
			if(CityGlobalData.t_resp.selfTroop.nodes[0].modleId == 4)
			{
				BattleEffectControllor.Instance().PlayEffect (600266, curNode.gameObject);
			}
			else if(CityGlobalData.t_resp.selfTroop.nodes[0].modleId == 1)
			{
				BattleEffectControllor.Instance().PlayEffect (600280, curNode.gameObject);
			}
			else if(CityGlobalData.t_resp.selfTroop.nodes[0].modleId == 2)
			{
				BattleEffectControllor.Instance().PlayEffect (600309, curNode.gameObject);
			}
			else
			{
				BattleEffectControllor.Instance().PlayEffect (600210, curNode.gameObject);
			}
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

			curNode.beatDown(100, king);

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
			if(CityGlobalData.t_resp.selfTroop.nodes[0].modleId == 4)
			{
				BattleEffectControllor.Instance().PlayEffect (600264, curNode.gameObject);
			}
			else
			{
				BattleEffectControllor.Instance().PlayEffect (600210, curNode.gameObject);
			}
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

			curNode.beatDown(100, king);

			if(index == 1)
			{
				BattleEffectControllor.Instance().PlayEffect (600229, wudizhan.curNode.transform.position, king.copyObject.transform.forward);
			
				if(skillTemplate.value4 != 0)
				{
					Buff.createBuff(wudizhan.curNode, AIdata.AttributeType.ATTRTYPE_skillReduction_Light, skillTemplate.value4, skillTemplate.value5);
				}
			}

			if(king.stance == BaseAI.Stance.STANCE_SELF) king.Shake(KingCamera.ShakeType.Cri);
		}
	}

	public void skill_1_resetPosition()
	{
		curCount = 0;

		king.mAnim.SetBool("WuDiOver", false);

		if(king.signShadow != null) king.signShadow.gameObject.SetActive (true);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (true);

		king.copyObject.SetActive (false);
		
		king.copyObject.transform.position = new Vector3(0, -1000, 0);

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
		inSkill = false;

		king.setNavMeshStop ();

		king.transform.forward = king.copyObject.transform.forward;

		if(king.signShadow != null) king.signShadow.gameObject.SetActive (true);

		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (true);

		BattleEffectControllor.Instance().PlayEffect (600230, king.copyObject.transform.position, king.copyObject.transform.forward);

		king.copyObject.SetActive (false);
		
		king.copyObject.transform.position = new Vector3(0, -1000, 0);
		
		if(king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (king.gameObject);

		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", .2f,
			"onupdate", "setAlphaSelf"
			));
	}

	public void WudizhanDone()
	{
		inSkill = false;

//		List<BaseAI> ns = king.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;
//
//		Vector3 tempFow = transform.forward;
//
//		SkillTemplate template = SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.jueyingxingguangzhan, king);
//
//		foreach(BaseAI n in ns)
//		{
//			float l = Vector3.Distance(transform.position, n.transform.position);
//			
//			if(l < template.value6 && n.isAlive)
//			{
//				transform.forward = n.transform.position - transform.position;
//
//				Vector3 targetP = transform.position + transform.forward * (l + 2f);
//
//				StartCoroutine(n.attackedMovement(0.05f, targetP, iTween.EaseType.easeOutExpo, 0.2f));
//			
//				if(n.nodeData.nodeType != qxmobile.protobuf.NodeType.BOSS)
//				{
//					//n.mAnim.SetTrigger(n.getAnimationName(BaseAI.AniType.ANI_BATCDown));
//
//					n.beatDown(101, king);
//				}
//			}
//		}
//
//		transform.forward = tempFow;
	}

	void setAlphaSelf(float alpha)
	{
		Renderer[] rds = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(m.shader.name.Equals("Custom/Characters/Main Texture Hight Light Rim"))
				{
					m.SetColor("_MainColor", new Color( 0.537f, 0.537f, 0.537f, alpha));
				}
				else if(m.shader.name.Equals("Custom/Characters/Main Texture High Light"))
				{
					m.color = new Color( 0.537f, 0.537f, 0.537f, alpha);
				}
				else if(m.shader.name.Equals("Custom/Characters/Main Texture Diffuse Rim"))
				{
					Color c = m.GetColor("_MainColor");
					
					m.SetColor("_MainColor", new Color( c.r, c.g, c.b, alpha));
				}
			}
		}
	}

	void setAlphaCopy(float alpha)
	{
		Renderer[] rds = king.copyObject.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(m.shader.name.Equals("Custom/Characters/Main Texture Hight Light Rim"))
				{
					m.SetColor("_MainColor", new Color( 0.537f, 0.537f, 0.537f, alpha));
				}
				else if(m.shader.name.Equals("Custom/Characters/Main Texture High Light"))
				{
					m.color = new Color( 0.537f, 0.537f, 0.537f, alpha);
				}
				else if(m.shader.name.Equals("Custom/Characters/Main Texture Diffuse Rim"))
				{
					Color c = m.GetColor("_MainColor");

					m.SetColor("_MainColor", new Color( c.r, c.g, c.b, alpha));
				}
			}
		}
	}

	public void cut()
	{
		if(inSkill) StartCoroutine (cutAction());
	}

	IEnumerator cutAction()
	{
		iTween.Stop (king.gameObject);

		yield return new WaitForSeconds (.1f);

		curCount = 0;

		king.mAnim.SetBool("WuDiOver", false);

		setAlphaCopy(1);
		
		setAlphaSelf (1);

		king.setNavMeshStop ();
		
		king.transform.forward = king.copyObject.transform.forward;
		
		if(king.signShadow != null) king.signShadow.gameObject.SetActive (true);
		
		if(king.shadowObject_2 != null) king.shadowObject_2.SetActive (true);
		
		king.copyObject.SetActive (false);
		
		king.copyObject.transform.position = new Vector3(0, -1000, 0);
		
		if(king.stance == BaseAI.Stance.STANCE_SELF) king.gameCamera.targetChang (king.gameObject);

		transform.position = tempPos;

		inSkill = false;
	}
}

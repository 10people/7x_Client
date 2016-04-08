//#define REMOVE_MIBAO_CD

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class BaseAI : MonoBehaviour
{
	public enum Stance
	{
		STANCE_SELF,
		STANCE_ENEMY,
		STANCE_MID,
	}

	public enum AniType
	{
		ANI_Stand0 = 0,
		ANI_Stand1 = 1,
		ANI_Walk = 2,
		ANI_Run = 3,
		ANI_BATC = 4,
		ANI_BATCDown = 5,
		ANI_BATCFly = 6,
		ANI_Dead = 7,
		ANI_Dialog = 8,
		ANI_OnTheStage = 9,
		ANI_Attack_0 = 10,
		ANI_Attack_1 = 11,
		ANI_Attack_2 = 12,
		ANI_Skill_0 = 13,
		ANI_Skill_1 = 14,
		ANI_Skill_2 = 15,
		ANI_Skill_3 = 16,
		ANI_Skill_4 = 17,
		ANI_Skill_5 = 18,
		ANI_Skill_6 = 19,
		ANI_Skill_7 = 20,
		ANI_Skill_8 = 21,
		ANI_BATCUp = 22,
		ANI_DODGE = 23,
	}

    private string[] m_strAnimationName = new string[]{
		"Stand0",         //0
		"Stand1",         //1
		"Walk",           //2
		"Run",            //3
		"BATC",           //4
		"BATCDown",       //5
		"BATCFly",        //6
		"Dead",           //7
		"Dialog",         //8
		"OnTheStage",     //9
		"Attack_0",       //10
		"Attack_1",       //11
		"Attack_2",       //12
		"Skill_0",        //13
		"Skill_1",        //14
		"Skill_2",        //15
		"Skill_3",        //16
		"Skill_4",        //17
		"Skill_5",        //18
		"Skill_6",        //19
		"Skill_7",        //20
		"Skill_8",        //21
		"BATCUp",         //22
		"DODGE",          //23
	};

	public GameObject body;


	[HideInInspector] public GameObject m_Gameobj;

	[HideInInspector] public BattleFlag flag;

	[HideInInspector] public BattleBuffFlag buffFlag;

	[HideInInspector] public KingWeapon weapon;

	[HideInInspector] public AIdata nodeData;

	[HideInInspector] public AIdata nodeDataCopy;

	[HideInInspector] public Stance stance;

	[HideInInspector] public float curAttackSpeed;//当前攻击速度

	[HideInInspector] public BaseAI targetNode;

	[HideInInspector] public GameObject shadowTemple;

	[HideInInspector] public GameObject shadowObject;

	[HideInInspector] public GameObject shadowTemple_2;
	
	[HideInInspector] public GameObject shadowObject_2;

	[HideInInspector] public GameObject bloodTemple;

	[HideInInspector] public BloodBar bloodbar;

	[HideInInspector] public int nodeId;

	[HideInInspector] public Animator mAnim;

	[HideInInspector] public List<GameObject> trails = new List<GameObject>();

	[HideInInspector] public List<Buff> buffs = new List<Buff>();

	[HideInInspector] public List<BaseAI> enemysInRange = new List<BaseAI>();

	[HideInInspector] public bool isAlive = true;

	[HideInInspector] public bool aiable = true;

	[HideInInspector] public bool runable = true;

	[HideInInspector] public Color shadowColor;

	[HideInInspector] public bool isBlind;

	[HideInInspector] public bool isIdle;

	[HideInInspector] public bool slowdownable;

	[HideInInspector] public bool moveable;

	[HideInInspector] public bool hurtable;

	[HideInInspector] public bool sleep;

	[HideInInspector] public KingCrashTemplate curCrashData;

	[HideInInspector] public CharacterController character;

	[HideInInspector] public bool biteme;//被嘲讽

	[HideInInspector] public float reductionRate;//抵挡伤害

	[HideInInspector] public int reductionCount;//抵挡次数

	[HideInInspector] public List<HeroSkill> skills;

	[HideInInspector] public int m_iUseSkillIndex = -1;

	[HideInInspector] public int alarmState;//0-未开始警报，1-报警中，2报警完成

	[HideInInspector] public List<BaseAI> m_listAtk = new List<BaseAI>();//攻击列表

	[HideInInspector] public List<BaseAI> m_listByAtk = new List<BaseAI>();//被攻击列表

	[HideInInspector] public List<BaseAI> m_listSkill = new List<BaseAI>();//技能攻击列表

	[HideInInspector] public List<BaseAI> m_listBySkill = new List<BaseAI>();//被技能攻击列表

	[HideInInspector] public List<BaseAI> m_listCopy = new List<BaseAI>();//复制出的分身列表

	[HideInInspector] public float radius;

	[HideInInspector] public bool m_isStopTime = false;

	[HideInInspector] public float m_fStopTime = 0.0f;

	[HideInInspector] public float m_fStopBTime = 0.0f;

	[HideInInspector] public int modelId;

	[HideInInspector] public Dictionary<int, Threat> threatDict = new Dictionary<int, Threat> ();

	[HideInInspector] public BubblePopNode bubblePopNode;

	[HideInInspector] public BattleAppearanceTemplate appearanceTemplate;

	[HideInInspector] public string m_sPlaySkillAnimation = "";
	

	protected CharacterController chongfengControllor;
	
	protected System.DateTime attackTempTime;

	protected bool inTurning;

	protected int avoidancePriority;

	protected bool inHover;

	protected bool inStrongMove;


	private AnimationState aniWalk;

	private AnimationState aniRun;

	private NavMeshAgent nav;

	private Vector3 tempPos;

	private int moveZeroCount;

	private bool hoverd;

	private bool runawayTurn_1;

	private bool runawayTurn_2;

	private System.DateTime runawayTime;

	private System.DateTime attackedTime;

	private bool inRun;

	private Vector3 tarPosition;

	private int criCount;

	private int criNumCount;

	private int criSkillCount;
	
	private int criSkillNumCount;

	private int hoverIndex;

	private bool m_isStart = false;

	private bool m_isZhuge = false;
	
	private bool perpareForSkill;

	private int droppenIndex;

	private string m_sPAnimationName = "";

	private int tempSkillId;

	private bool inPath;

	private Vector3 pathPos;


	#if UNITY_EDITOR && REMOVE_MIBAO_CD
	private static bool m_log_tips = true;
	#endif

	public virtual void Start()
	{
		#if UNITY_EDITOR && REMOVE_MIBAO_CD
		if( m_log_tips ){
			m_log_tips = false;

			Debug.Log( "Temporary Remove CD(For CaoKai)." );
		}
		#endif

		DisableOcclusion ();

		m_isStart = true;

		if(m_isZhuge)
		{
			RestoreOcclusion();
		}
	}

	public virtual void OnDestroy(){
		body = null;
		
		m_Gameobj = null;
		
		flag = null;

		if( mAnim != null ){
			mAnim.runtimeAnimatorController = null;
		}

		mAnim = null;
	}

	public virtual void initStart()
	{
		isBlind = false;

		isIdle = false;

		slowdownable = true;

		moveable = true;

		hurtable = true;

		sleep = false;

		biteme = false;

		inTurning = false;

		alarmState = 0;

		perpareForSkill = false;

		droppenIndex = 0;

		tempSkillId = 0;

		weapon = (KingWeapon)gameObject.GetComponentInChildren (typeof(KingWeapon));

		if(weapon != null)
		{
			weapon.initWeapon (this, 0);

			setWeaponTriggerFalse(weapon.hand);
		}

		nav = gameObject.GetComponent<NavMeshAgent>();

		mAnim = GetComponentInChildren<Animator>();

		character = this.GetComponent<CharacterController>();

		setNavMeshRadius (appearanceTemplate.colliderRadius);
		
		character.radius = appearanceTemplate.colliderRadius;

		setNavMeshPriority (appearanceTemplate.navPriority);

		ObjectHelper.AddObjectTrace( mAnim.runtimeAnimatorController );

		trails.Clear();

		updataAttackRange();

		tempPos = new Vector3();

		attackTempTime = System.DateTime.Now;

		hoverd = false;

		moveZeroCount = 0;

		curCrashData = null;

		inHover = false;

		inStrongMove = false;

		inPath = false;

		pathPos = Vector3.zero;

		runawayTurn_1 = false;

		runawayTurn_2 = false;

		runawayTime = System.DateTime.Now.AddSeconds (-100);

		attackedTime = System.DateTime.Now.AddSeconds (-100);

		inRun = true;

		tarPosition = Vector3.zero;

		criCount = 0;

		criNumCount = 0;

		criSkillCount = 0;

		criSkillNumCount = 0;

		hoverIndex = -1;

		avoidancePriority = nav.avoidancePriority;

		threatDict.Clear ();
	}

	public void init(int _modelId, Node _nodeData)
	{
		if(nodeData.nodeType == NodeType.PLAYER)
		{
			shadowColor = new Color(.9647f, .5529f, .1765f, .3922f);

			//shadowColor = Color.yellow;
		}
		else
		{
			shadowColor = stance == Stance.STANCE_SELF ? Color.blue : Color.red;

			shadowColor.a = .3922f;
		}

//		if(shadowObject == null) shadowObject = (GameObject)Instantiate(shadowTemple);
//
//		shadowObject.SetActive(true);
//
//		shadowObject.transform.parent = gameObject.transform;
//
//		shadowObject.transform.localPosition = Vector3.zero;
//
//		shadowObject.transform.localScale = shadowTemple.transform.localScale;
//
//		shadowObject.renderer.material.SetColor("_TintColor", shadowColor);

		if( Quality_Shadow.BattleField_ShowSimpleShadow() == true
		   && nodeData.nodeType != NodeType.GOD)
		{
			if(shadowObject_2 == null) shadowObject_2 = (GameObject)Instantiate(shadowTemple_2);
			
			shadowObject_2.SetActive(true);
			
			shadowObject_2.transform.parent = gameObject.transform;
			
			shadowObject_2.transform.localPosition = new Vector3(0, 0, -0.01f);
			
			shadowObject_2.transform.localScale = shadowTemple_2.transform.localScale;
			
			shadowObject_2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color (0.1f, 0.1f, 0.1f, 0.5f));
		}

		float bloodRate = 1f;

		float bloodY = getHeight() + .2f;

		if(nodeData.nodeType == NodeType.SOLDIER)
		{
			bloodRate = 2.5f;
		}
		else if(nodeData.nodeType == NodeType.HERO)
		{
			bloodRate = 3.5f;
		}
		else if(nodeData.nodeType == NodeType.PLAYER || nodeData.nodeType == NodeType.BOSS)
		{
			bloodRate = 5f;
		}

		GameObject bloodObject = (GameObject)Instantiate (bloodTemple);

		bloodObject.transform.parent = gameObject.transform;

		bloodObject.transform.localPosition = new Vector3 (0, bloodY, 0);

		bloodObject.transform.localScale = new Vector3(bloodTemple.transform.localScale.x * bloodRate, bloodTemple.transform.localScale.y, bloodTemple.transform.localScale.z);

		bloodbar = bloodObject.GetComponent<BloodBar> ();

		bloodbar.setValue(1);

		bloodObject.SetActive (false);

		updataBloodBar();

		if(nodeData.nodeType == NodeType.BOSS)
		{
			if(body != null) {
				EffectTool.SetBossEffect( body, appearanceTemplate.bossFxColor, appearanceTemplate.bossFxWidth );
			}
			else {
				EffectTool.SetBossEffect( gameObject, appearanceTemplate.bossFxColor, appearanceTemplate.bossFxWidth );
			}
		}

		enemysInRange.Clear();

		if(stance == Stance.STANCE_ENEMY)
		{
			transform.forward = Vector3.back;
		}

		if( ClientMain.m_ClientMainObj != null)
		{ 
			gameObject.AddComponent<SoundPlayEff> ();
		}

		else gameObject.AddComponent<PlaySoundWithoutNet> ();

		initStart();
	}

	public void initDate(Node _nodeData, GameObject gameobj, int _modelId)
	{
		m_Gameobj = gameobj;

		modelId = _modelId;

		appearanceTemplate = BattleAppearanceTemplate.getBattleAppearanceTemplateById (_nodeData.appearanceId);

		nodeData = new AIdata ();

		nodeData.initValue (this, _nodeData);

		nodeDataCopy = new AIdata ();

		nodeDataCopy.initValue (this, _nodeData);

		if(nodeData.droppenItems != null)
		{
			foreach(DroppenItem dropItem in nodeData.droppenItems)
			{
				int dropId = dropItem.id;

				if(BattleControlor.Instance().droppenDict.ContainsKey(dropId) == false)
				{
					BattleControlor.Instance().droppenDict.Add(dropId, 0);
				}
			}
		}

		getCurAttackSpeed ();

		setNavMeshSpeed(nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ));

		SphereCollider sc = (SphereCollider)GetComponent("SphereCollider");

		if(sc != null)
		{
			sc.radius = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange );

			if(sc.radius < radius)
			{
				//sc.radius = radius;
			}
		}

		gameObject.name = "Node_" + nodeId;

		if(flag != null) flag.node = this;

		if (buffFlag != null) buffFlag.setNode (this);

		if(nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor k = (KingControllor)this;
			
			k.weaponDateHeavy = nodeData.weaponHeavy;
			
			k.weaponDateLight = nodeData.weaponLight;
			
			k.weaponDateRanged = nodeData.weaponRanged;

			initWeaponModel(k);
		}
		else
		{
			KingWeapon kw = GetComponentInChildren<KingWeapon>();

			if(kw != null)
			{
				float length = nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_attackRange) + 1;

				kw.boxSize = new Vector3(length * .75f, 2, length);

				kw.colliderCenter += new Vector3(0, 0, kw.boxSize.z / 2 - kw.colliderCenter.z);

//				BoxCollider boxCol = kw.gameObject.GetComponent<BoxCollider>();
//
//				if(boxCol != null)
//				{
//					float length = nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_attackRange) + 1;
//
//					boxCol.size = new Vector3(length * .75f, 2, length);
//
//					boxCol.center += new Vector3(0, 0, boxCol.size.z / 2 - boxCol.center.z);
//				}
			}
		}

		init(_modelId, _nodeData);

		initSkill();
	}

	private void initSkill()
	{
		skills = new List<HeroSkill> ();

		if (nodeId == 1)
		{
			BattleControlor.Instance().setMiBaoSkill(null, this);
		}

		if (nodeData.skills != null)
		{
			foreach( NodeSkill skill in nodeData.skills )
			{
				if(skill == null) continue;

				HeroSkill heroSkill = gameObject.AddComponent<HeroSkill>();

				skills.Add(heroSkill);

				skills[skills.Count - 1].init( skill , skills.Count - 1);

				if(nodeData.nodeType == NodeType.PLAYER && skill.zhudong == true)
				{
					if(skill.id == 200012 || skill.id == 201301 || skill.id == 201302)//旧机制乾坤斗转技能从服务器获取，改为前台读表
					{
						//BattleControlor.Instance().setHeavySkill_2(heroSkill, this);
					}
					else
					{
						BattleControlor.Instance().setMiBaoSkill(heroSkill, this);
					}
				}
			}
		}

		if(nodeData.nodeType == NodeType.PLAYER)//手动添加乾坤斗转技能
		{
			if((this as KingControllor).skillLevel[(int)CityGlobalData.skillLevelId.qiankundouzhuan] > 0)
			{
				HeroSkill heroSkill = gameObject.AddComponent<HeroSkill>();
				
				skills.Add(heroSkill);

				skills[skills.Count - 1].init( SkillTemplate.getSkillTemplateBySkillLevelIndex(CityGlobalData.skillLevelId.qiankundouzhuan, this as KingControllor) , skills.Count - 1);

				BattleControlor.Instance().setHeavySkill_2(heroSkill, this);
			}
		}

		for(int i = 0; i < skills.Count; i ++)
		{
			skills[i].getSkillAssociated();
		}
	}

	private void initWeaponModel(KingControllor king)
	{
		if(king.weaponDateHeavy != null)
		{
			Object heavyTemple = null;

			BattleNet.Instance().modelList.TryGetValue(ModelTemplate.getModelTemplateByModelId((int)king.weaponDateHeavy.weaponId).path, out heavyTemple);
		
			if(heavyTemple == null) Debug.LogError("CAN NOT FIND WEAPON WITH MODEL ID " + king.weaponDateHeavy.weaponId);

			else
			{
				GameObject gcHeavy = (GameObject)GameObject.Instantiate(heavyTemple);

				gcHeavy.transform.parent = king.m_weapon_Heavy.transform;

				gcHeavy.transform.localPosition = Vector3.zero;

				gcHeavy.transform.localEulerAngles = Vector3.zero;

				gcHeavy.transform.localScale = new Vector3(1, 1, 1);
			}
		}

		if(king.weaponDateLight != null)
		{
			Object lightTemple = null;
			
			BattleNet.Instance().modelList.TryGetValue(ModelTemplate.getModelTemplateByModelId((int)king.weaponDateLight.weaponId).path, out lightTemple);
			
			if(lightTemple == null) Debug.LogError("CAN NOT FIND WEAPON WITH MODEL ID " + king.weaponDateLight.weaponId);
			
			else
			{
				GameObject gcLightLeft = (GameObject)GameObject.Instantiate(lightTemple);
				
				gcLightLeft.transform.parent = king.m_weapon_Light_left.transform;
				
				gcLightLeft.transform.localPosition = Vector3.zero;
				
				gcLightLeft.transform.localEulerAngles = Vector3.zero;
				
				gcLightLeft.transform.localScale = new Vector3(1, 1, 1);

				GameObject gcLightRight = (GameObject)GameObject.Instantiate(lightTemple);
				
				gcLightRight.transform.parent = king.m_weapon_Light_right.transform;
				
				gcLightRight.transform.localPosition = Vector3.zero;
				
				gcLightRight.transform.localEulerAngles = Vector3.zero;
				
				gcLightRight.transform.localScale = new Vector3(1, 1, 1);
			}
		}

		if(king.weaponDateRanged != null)
		{
			Object rangeTemple = null;

			BattleNet.Instance().modelList.TryGetValue(ModelTemplate.getModelTemplateByModelId((int)king.weaponDateRanged.weaponId).path, out rangeTemple);

			if(rangeTemple == null) Debug.LogError("CAN NOT FIND WEAPON WITH MODEL ID " + king.weaponDateRanged.weaponId);

			else
			{
				GameObject gcRange = (GameObject)GameObject.Instantiate(rangeTemple);
				
				gcRange.transform.parent = king.m_weapon_Ranged.transform;
				
				gcRange.transform.localPosition = Vector3.zero;
				
				gcRange.transform.localEulerAngles = Vector3.zero;
				
				gcRange.transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}

	public virtual void refreshWeaponDate()
	{

	}

	public void activeSkillStart(int state)
	{
		//if (BattleControlor.Instance().inDrama == true) return;

		if(nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor king = (KingControllor)this;

			king.comboable = true;
		}

		if(m_iUseSkillIndex != -1)
		{
			skills[m_iUseSkillIndex].activeSkill(state);

//			Debug.Log (skills[m_iUseSkillIndex].m_otherSkill.Count);

			for(int i = 0; i < skills[m_iUseSkillIndex].m_otherSkill.Count; i ++)
			{
				skills[m_iUseSkillIndex].m_otherSkill[i].activeSkill(0);
			}

			BubblePopControllor.Instance().triggerFuncSkill(nodeId, skills[m_iUseSkillIndex].template.id);

			StrEffectItem.OpenEffect(gameObject, SkillTemplate.getSkillTemplateById(skills[m_iUseSkillIndex].template.id).Fx3D, modelId);
		}
	}

	public void skillEnd()
	{
		StrEffectItem.CloseEffect(gameObject);
	}

	public void openShow()
	{
		if (BattleControlor.Instance().inDrama == true) return;
		skills[m_iUseSkillIndex].setShowFanRand();
//		Debug.Log (skills[m_iUseSkillIndex].m_otherSkill.Count);
		for(int i = 0; i < skills[m_iUseSkillIndex].m_otherSkill.Count; i ++)
		{
			skills[m_iUseSkillIndex].m_otherSkill[i].setShowFanRand();
		}
	}

	public void playPre()
	{
		if (BattleControlor.Instance().inDrama == true) return;

		skills[m_iUseSkillIndex].isPlayPer();
		for(int i = 0; i < skills[m_iUseSkillIndex].m_otherSkill.Count; i ++)
		{
			skills[m_iUseSkillIndex].m_otherSkill[i].isPlayPer();
		}
	}

	public void setStand()
	{
		if (BattleControlor.Instance().inDrama == true) return;

		if(gameObject.activeSelf == true && nav != null && nav.enabled == true)
		{
			nav.avoidancePriority = avoidancePriority;
		}
	}

	public void setStone()
	{
		if(nav != null && nav.enabled == true) 
		{
			nav.Stop();

			nav.avoidancePriority = 1;
		}
	}

	public void checkSkillDrama(int skillId)
	{
		if (CityGlobalData.m_battleType != EnterBattleField.BattleType.Type_GuoGuan) return;

		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		bool f = GuideTemplate.HaveId_type (level, 6, skillId);

		if (f == false) return;

		GuideTemplate template = GuideTemplate.getTemplateByLevelAndType (level, 6, skillId);

		bool flag = BattleControlor.Instance().havePlayedGuide (template);

		if (flag == true) return;

		if(template.flagId.Count == 0)
		{
			BattleUIControlor.Instance().showDaramControllor (level, template.id);
		}
		else
		{
			foreach(int flagId in template.flagId)
			{
				BattleFlag bf = null;
				
				BattleControlor.Instance().flags.TryGetValue(flagId, out bf);
				
				if(bf != null) bf.trigger();
			}
		}
	}

	protected void getCurAttackSpeed()
	{
		float ran = Random.value;

		if (nodeData.nodeType == NodeType.SOLDIER)
		{
			ran = ran * 2 - 1;
		}
		else
		{
			ran = 0;
		}

		curAttackSpeed = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackSpeed ) + ran;
	}

	protected void updateAnimationSpeed()
	{
		//if (targetNode == null && hoverIndex == -1) return;

		float sp = Vector3.Distance(tempPos, transform.position);
		
		tempPos = transform.position;
		
		sp = sp / Time.deltaTime;

		if(sp == 0)
		{
			moveZeroCount++;
		}
		else if(nodeData.nodeType != NodeType.PLAYER && nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ) == 0)
		{
			moveZeroCount++;
		}
		
		if(moveZeroCount > 10)
		{
			if(nodeData.nodeType != NodeType.NPC)
			{
				mAnim.SetFloat("move_speed", 0);
			}
		}
		else if(sp == 0)
		{
			return;
		}
		else if(nodeData.nodeType != NodeType.PLAYER && nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ) == 0)
		{
			return;
		}
		else
		{
			mAnim.SetFloat("move_speed", sp);
		}
		
		moveZeroCount = 0;
	}

	public bool IsPlaying( string p_action_name )
	{
		if (mAnim == null) return false;

		AnimatorClipInfo[] t_states = mAnim.GetCurrentAnimatorClipInfo( 0 );
		
		if( t_states.Length > 1 ){
			//return false;
		}
		else if( t_states.Length == 0 ){
			//return false;
		}
		
		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];

			if( t_item.clip.name.Equals(p_action_name) )
			{
				return true;
			}
		}

		return false;
	}

	public string IsPlaying()
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetCurrentAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}

	public string nextPlaying()
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetNextAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}

	protected void refreshCRI(int _criX, int _criY, int _criSkillX, int _criSkillY)
	{
		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criX, _criX );

		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criY, _criY );

		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillX, _criSkillX );

		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillY, _criSkillY );

		//criCount = 0;

		//criNumCount = 0;

		//criSkillCount = 0;

		//criSkillNumCount = 0;
	}

	public bool countCri()//return: cri or not
	{
		criNumCount ++;

		if(criNumCount >= nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criX ))
		{
			criNumCount = 0;

			criCount = 0;
		}

		float r = Random.value;

		float radio = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criY ) / nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criX );

		if(r < radio && criCount < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criY ))
		{
			criCount ++;

			return true;
		}

		if(criNumCount > nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criX ) - nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criY )
		   && criCount < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criY ))
		{
			criCount ++;

			return true;
		}

		return false;
	}

	public bool countCriSkill()//return: cri or not
	{
		criSkillNumCount ++;

		if(criSkillNumCount >= nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillX ))
		{
			criSkillNumCount = 0;
			
			criSkillCount = 0;
		}

		float r = Random.value;
		
		float radio = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillY ) / nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillX );
		
		if(r > radio && criSkillCount < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillY ))
		{
			criSkillCount ++;
			
			return true;
		}
		
		if(criSkillNumCount > nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillX ) - nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillY )
		   && criSkillCount < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_criSkillY ))
		{
			criSkillCount ++;
			
			return true;
		}
		
		return false;
	}

	// use Update() to make it faster.
//	void FixedUpdate ()
	void Update()
	{
		updateThreats ();

		if(BattleUIControlor.Instance().resultControllor != null && BattleUIControlor.Instance().resultControllor.gameObject.activeSelf == true)
		//if(BattleControlor.Instance().result != BattleControlor.BattleResult.RESULT_BATTLING)
		{
			setNavMeshStop();

			return;
		}

		if(BattleControlor.Instance().inDrama)
		{
			return;
		}

		if (BattleControlor.Instance().completed == false)
		{
			return;
		}

		if(isIdle)
		{
			return;
		}

		if(m_sPlaySkillAnimation != "")
		{
			if(m_sPlaySkillAnimation == IsPlaying())
			{
//				Debug.Log("清空技能播放名称=" + m_sPlaySkillAnimation);

				m_sPlaySkillAnimation = "";
			}
		}

		for(int i = 0; i < skills.Count; i ++)
		{
			skills[i].upData();
		}

		if(m_sPlaySkillAnimation != "" && m_sPAnimationName != IsPlaying())
		{
			if(IsPlaying() == "Run" || IsPlaying() == "Walk" || IsPlaying() == "Stand0" || IsPlaying() == "Stand1")
			{
				mAnim.SetTrigger (m_sPlaySkillAnimation);
			}
		}

		m_sPAnimationName = IsPlaying();

		m_listAtk.Clear ();

		m_listByAtk.Clear ();

		m_listSkill.Clear ();

		m_listBySkill.Clear ();

		updateDistanceDrama ();

		BaseUpdate ();
	}

	private void updateThreats()
	{
		foreach(BaseAI node in enemysInRange)
		{
			if(node == null) continue;

			if(node.gameObject.activeSelf == false) continue;

			if(node.isAlive == false) continue;

			if(node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) <= 0) continue;

			Threat threat = null;

			threatDict.TryGetValue(node.nodeId, out threat);

			if(threat == null)
			{
				threat = new Threat();

				threatDict.Add(node.nodeId, threat);
			}

			float lengthThreat = 1000 - Vector3.Distance(node.transform.position, transform.position) * 10;
			
			threat.lengthThreat = lengthThreat;
		}
	}

	public virtual void BaseUpdate ()
	{
		if (!isAlive || !aiable) return;

		if (nodeData.nodeType == NodeType.NPC) return;

		//if(BattleControlor.Instance().result != BattleControlor.BattleResult.RESULT_BATTLING) return;

		if(BattleControlor.Instance().inCameraEffect_update == true) return;

		bool aiFlag = false;

		if(nodeData.nodeType != NodeType.PLAYER)//非玩家单位每5帧执行一次AI
		{
			int frameCount = Time.frameCount % 5;

			int idIndex = Mathf.Abs(nodeId % 5);

			if(frameCount == idIndex)
			{
				aiFlag = true;
			}
		}
		else
		{
			aiFlag = true;
		}

		if(aiFlag == true)
		{
			isStopTimeOver();
			
			alarmUpdate ();

			if (alarmState != 1)
			{
				uptateTarget();
				
				updateEnemysList();

				moveOrAttack();
			}
		}

		if(perpareForSkill == false)
		{
			updateAnimationSpeed ();
		}
	}

	protected void isStopTimeOver()
	{
		if(m_isStopTime)
		{
			if(Time.time - m_fStopBTime > m_fStopTime)
			{
				mAnim.speed = 1;
			}
		}
	}

	protected void updateDistanceDrama()
	{
		if (flag == null || flag.triggerFlagDistance.Count == 0) return;

		List<BaseAI> aliveNodes = new List<BaseAI> ();

		List<int> list = new List<int> ();

		foreach(BaseAI node in BattleControlor.Instance().selfNodes)
		{
			if(node.isAlive == true && node.gameObject.activeSelf == true)
			{
				aliveNodes.Add(node);
			}
		}

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.isAlive == true && node.gameObject.activeSelf == true)
			{
				aliveNodes.Add(node);
			}
		}

		List<DistanceFlag> des = new List<DistanceFlag> ();

		foreach(DistanceFlag df in flag.triggerFlagDistance)
		{
			List<Vector2> vecs = df.triggerDistance;

			int count = 0;

			foreach(Vector2 vec in vecs)
			{
				float distance = vec.x;

				int id = (int)vec.y;

				BaseAI node = getNodeById(aliveNodes, id);

				if(node == null) continue;

				if(Vector3.Distance(transform.position, node.transform.position) <= distance)
				{
					count ++;
				}
			}

			if(count >= df.count)
			{
				BattleFlag bf = null;

				BattleControlor.Instance().flags.TryGetValue(df.triggerFlag, out bf);

				if(bf != null)
				{
					bf.trigger();
				
					des.Add(df);
				}
			}
		}

		foreach(DistanceFlag df in des)
		{
			flag.triggerFlagDistance.Remove(df);
		}
	}

	private BaseAI getNodeById(List<BaseAI> nodeList, int id)
	{
		foreach(BaseAI node in nodeList)
		{
			if(node.nodeId == id) return node;
		}

		return null;
	}

	public void uptateTarget()
	{
//		if(targetNode != null
//		   && targetNode.isAlive
//		   && targetNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) >= 0
//		   && Vector3.Distance(targetNode.transform.position, transform.position) < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange ) )
//			return;

//      if (biteme == true && targetNode != null && targetNode.isAlive == true && targetNode.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) >= 0) return;

		targetNode = null;

		float tempThreast = 0;

		float tempLength = 999;

		//List<BaseAI> tempList = stance == Stance.STANCE_ENEMY ? BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes;

		foreach(BaseAI node in enemysInRange)
		{
			if(node == null || !node.isAlive || node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0) continue;

			if(node.nodeData.nodeType == NodeType.GOD || node.nodeData.nodeType == NodeType.NPC) continue;

			if(node.gameObject.activeSelf == false) continue;

			float leng = Vector3.Distance(transform.position, node.transform.position);

			Threat threat = null;

			threatDict.TryGetValue(node.nodeId, out threat);

			float threatNum = 0;

			if(threat != null)
			{
				threatNum = threat.totalThreat;
			}

			if(threatNum > tempThreast)
			{
				targetNode = node;

				tempThreast = threatNum;

				tempLength = leng;
			}
			else if(threatNum == tempThreast)
			{
				if(leng < tempLength)
				{
					targetNode = node;
					
					tempThreast = threatNum;

					tempLength = leng;
				}
			}
		}

		if(nodeData.nodeType == NodeType.PLAYER && targetNode == null)
		{
			if(stance == Stance.STANCE_SELF)
			{
				foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
				{
					if(node == null || node.isAlive == false || node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) <= 0)
					{
						continue;
					}

					if(node.nodeData.nodeType == NodeType.GOD || node.nodeData.nodeType == NodeType.NPC) continue;

					targetNode = node;

					return;
				}
			}
			else if(stance == Stance.STANCE_ENEMY)
			{
				foreach(BaseAI node in BattleControlor.Instance().selfNodes)
				{
					if(node == null || node.isAlive == false || node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) <= 0)
					{
						continue;
					}

					if(node.nodeData.nodeType == NodeType.GOD || node.nodeData.nodeType == NodeType.NPC) continue;

					targetNode = node;
					
					return;
				}
			}
		}
	}

	public void setTargetNull()
	{
		targetNode = null;
		
		enemysInRange.Clear();

		threatDict.Clear ();

		List<BaseAI> templist = null;

		if(stance == Stance.STANCE_ENEMY)
		{
			templist = BattleControlor.Instance().selfNodes;
		}
		else if(stance == Stance.STANCE_SELF)
		{
			templist = BattleControlor.Instance().enemyNodes;
		}
		else
		{
			templist = new List<BaseAI>();

			foreach(BaseAI n in BattleControlor.Instance().selfNodes)
			{
				templist.Add(n);
			}

			foreach(BaseAI n in BattleControlor.Instance().enemyNodes)
			{
				templist.Add(n);
			}
		}

		foreach(BaseAI node in templist)
		{
			if(node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0
			   || node.isAlive == false
			   || node.nodeId == nodeId
			   )
			{
				continue;
			}

			enemysInRange.Add(node);
		}
	}

	private void moveOrAttack()
	{
		if(isIdle || sleep)
		{
			setNavMeshStop();

			return;
		}
		else if(isBlind)
		{
			setNavMeshDestination(transform.position + new Vector3(Random.value * 4 - 2, 0, Random.value * 4 - 2));

			return;
		}

		if(nodeData.nodeType != NodeType.PLAYER)
		{
			string isPlaying = IsPlaying();

			if (isPlayingAttack() == true 
			    || isPlaying.Equals(getAnimationName(AniType.ANI_Dead)) == true
			    || isPlaying.Equals(getAnimationName(AniType.ANI_BATCDown)) == true
			    || isPlaying.Equals(getAnimationName(AniType.ANI_BATCUp)) == true
			    || isPlaying.Equals(getAnimationName(AniType.ANI_BATC)) == true
			    || isPlaying.Equals(getAnimationName(AniType.ANI_BATCFly)) == true
			    )
			{
				setNavMeshStop();
				
				return;
			}
		}

		if (moveable == false) return;

		if (targetNode == null && stance == Stance.STANCE_SELF && nodeData.nodeType != NodeType.PLAYER && flag.hoverPath.Count == 0)
		{
			Vector3 po = BattleControlor.Instance().getKing().transform.position;

			if(Vector3.Distance(transform.position, po) < 7)
			{
				setNavMeshStop();
			}
			else
			{
				setNavMeshDestination(po);
			}

			return;
		}
		else if(targetNode == null || nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_eyeRange) < 0) 
		{
			if(nodeId == 1)
			{
				BattleWinTemplate winTemp = BattleWinTemplate.getWinTemplateContainsType(BattleWinFlag.EndType.Reach_Destination, true);

				if(winTemp != null)
				{
					setNavMeshDestination(winTemp.destination);
				}
			}
			else
			{
				hover();
			}

			return;
		}

		if(inPath == true)
		{
			inPath = false;

			pathPos = transform.position;
		}

		List<BaseAI> nodeList = stance == Stance.STANCE_ENEMY ? BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes;

		if(nodeList.Count == 0)
		{
			setNavMeshStop();

			return;
		}

		float tempL = Vector3.Distance(targetNode.transform.position, transform.position);

		int runOrAttack = 0;//0:run 1:attack 2:tarPosition

		float range_1 = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange ) - 1f;

		float range_2 = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange ) * .8f;

		float range = range_1 > range_2 ? range_1 : range_2;

		if(Vector3.Distance(tarPosition, Vector3.zero) > 0.02f)
		{
			runOrAttack = 2;

			inRun = true;

			if(Vector3.Distance(tarPosition, transform.position) < 0.02f)
			{
				setTargetPosition(Vector3.zero);
			}
		}
		else if(tempL < range && targetNode.gameObject.activeSelf == true)
		{
			runOrAttack = 1;

			inRun = false;
		}
		else if(tempL > range)
		{
			runOrAttack = 0;

			inRun = true;
		}
		else
		{
			if(inRun == true) runOrAttack = 0;

			else if(inRun == false) runOrAttack = 1;
		}

		if(runOrAttack == 1)
		{
			attack(targetNode);

			setNavMeshStop();
		}
		else if(runOrAttack == 0)
		{
			setNavMeshDestination(targetNode.transform.position);
		}
		else if(runOrAttack == 2)
		{
			setNavMeshDestination(tarPosition);
		}
	}

	private void hover()
	{
		if (flag == null || flag.hoverPath.Count == 0) return;

		if(Vector3.Distance(pathPos, Vector3.zero) > .1f)
		{
			float lenth = Vector3.Distance(pathPos, transform.position);

			if(lenth > 1)
			{
				setNavMeshDestination(pathPos);

				return;
			}
			else
			{
				pathPos = Vector3.zero;
			}
		}

		bool enf = hoverIndex == -1;

		if(enf == false)
		{
			Vector3 targetPos = flag.hoverPath [hoverIndex];
			
			float length = Vector3.Distance (targetPos, transform.position);

			enf = length < 1;

			if(enf == false && Vector3.Distance(nav.destination, targetPos) > 1f)
			{
				setNavMeshDestination(targetPos);
			}
		}

		if(enf == true && hoverIndex < flag.hoverPath.Count - 1)
		{
			hoverIndex ++;

			hoverIndex = hoverIndex >= flag.hoverPath.Count ? 0 : hoverIndex;

			Vector3 targetPos = flag.hoverPath [hoverIndex];

			inPath = true;

			setNavMeshDestination(targetPos);
		}
	}

	public void alarm(Vector3 targetPosition)
	{
		if (alarmState != 0) return;

		alarmState = 1;

		setNavMeshDestination (targetPosition);
	}

	private void alarmUpdate()
	{
		if (alarmState != 1) return;

		float length = Vector3.Distance (transform.position, flag.alarmPosition);

		if(length < 1)
		{
			alarmState = 2;

			//OnTriggerEnter(BattleControlor.Instance().getKing().GetComponent<Collider>());
		}
	}

	protected virtual void runaway(float time)
	{
		float ra = Random.value * 90 - 45;

		transform.localEulerAngles += new Vector3 (0, 180 + ra, 0);

		inHover = true;

		setNavMeshStop ();
		
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", Vector3.zero,
			"to", new Vector3(100, 0, 0),
			"delay", 0,
			"time", time,
			"easetype", iTween.EaseType.linear,
			"onupdate", "onRunawayUpdate",
			"oncomplete", "onRunawayComplete"
			));
	}

	public void onRunawayUpdate(Vector3 p_offset)
	{
		if (isAlive == false) return;

		if (IsPlaying (getAnimationName (AniType.ANI_BATCDown)) == true)
		{
			onRunawayComplete();

			return;
		}

		if (IsPlaying (getAnimationName (AniType.ANI_BATCUp)) == true)
		{
			onRunawayComplete();
			
			return;
		}

		inHover = true;

		float x = p_offset.x;

		bool tu = false;

		if (x > 40 && runawayTurn_1 == false)
		{
			tu = true;

			runawayTurn_1 = true;
		}

		if(x > 70 && runawayTurn_2 == false)
		{
			tu = true;

			runawayTurn_2 = true;
		}

		if(tu == true)
		{
			float ra = Random.value * 180 + 90;

			transform.localEulerAngles += new Vector3 (0, ra, 0);
		}

		character.Move(transform.forward * getNavMeshSpeed() * Time.deltaTime);
	}

	public void onRunawayComplete()
	{
		runawayTurn_1 = false;
		
		runawayTurn_2 = false;

		inHover = false;
	}

	private void waitAttack()
	{
		int waitAction = 0;//0:stand 1:hover

		if(waitAction == 0)
		{
			float r = Random.value * 100;

			if(r < 70)
			{
				waitAction = 1;
			}
		}
		if(waitAction == 0)
		{

		}
		else if(waitAction == 1)
		{
			//if(nodeType == NodeType.SOLDIER_QIANG) hover();
		}
	}

	public bool attack(BaseAI defender)
	{
		if(nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor king = (KingControllor)this;

			king.transform.forward = defender.transform.position - king.transform.position;

			king.attack();

			return true;
		}

		int flagTime = attackInCold();

		if(flagTime != 0)
		{
			//hover(flagTime);

			if (isPlayingAttack () == true) return false;
			
			if (IsPlaying (getAnimationName (AniType.ANI_BATC)) == true) return false;
			
			if (IsPlaying (getAnimationName(AniType.ANI_BATCDown)) == true) return false;

			if (IsPlaying (getAnimationName(AniType.ANI_BATCUp)) == true) return false;

			if (inHover == true) return false;

			if(hoverd == false && flagTime > curAttackSpeed * 450 && flagTime < curAttackSpeed * 550)
			{
				waitAttack();

				hoverd = true;
			}

			return false;
		}

		bool inAttacked = IsPlaying (getAnimationName(AniType.ANI_BATC));

		if(nodeData.nodeType == NodeType.SOLDIER)
		{
			
		}
		else
		{
			inAttacked = false;
		}

		if (inAttacked == true) return false;

		if (IsPlaying (getAnimationName(AniType.ANI_BATCDown)) == true) return false;

		if (IsPlaying (getAnimationName(AniType.ANI_BATCUp)) == true) return false;

		mAnim.SetTrigger(getAnimationName(AniType.ANI_Attack_0 + (int)((Random.value * 100) % ModelTemplate.getModelTemplateByModelId(modelId).attackCount)));

		transform.forward = defender.transform.position - transform.position;

		//StartCoroutine(minusTargetHp(defender));

		attackTempTime = System.DateTime.Now;

		getCurAttackSpeed ();

		hoverd = false;

		return true;
	}

	protected int attackInCold()
	{
		if (nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackSpeed ) == 0 ) return 1;

		if (curAttackSpeed == 0) return 1;

		if (IsPlaying(getAnimationName(AniType.ANI_Attack_0))) return 1;
		
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - attackTempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		if(mill < curAttackSpeed * 1000)
		{
			return (int)(curAttackSpeed * 1000 - mill);
		}

		return 0;
	}

	public void attackDone()//已废弃
	{

	}

	private void updataBloodBar()
	{
		if(shadowObject != null)
		{
			if (nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) <= 0) shadowObject.SetActive(false);
		}

		float rate = nodeData.GetAttribute ((int)AIdata.AttributeType.ATTRTYPE_hp) / nodeData.GetAttribute ((int)AIdata.AttributeType.ATTRTYPE_hpMaxReal);

		if (bloodbar != null)
		{
			ModelTemplate mt = ModelTemplate.getModelTemplateByModelId(modelId);

			if(mt.height != 0)
			{
				if(stance == Stance.STANCE_SELF && nodeData.nodeType == NodeType.PLAYER) 
				{
					bloodbar.gameObject.SetActive(false);

					return;
				}

				if(nodeData.nodeType != NodeType.GOD && nodeData.nodeType != NodeType.NPC) bloodbar.setValue(rate);
			}
		}

		if(flag != null && flag.triggerFlagBloodInteger.Count > 0 && nodeId > 0)
		{
			foreach(Vector2 vec in flag.triggerFlagBloodInteger)
			{
				if(rate < vec.x)
				{
					int flagId = (int)vec.y;

					BattleFlag bf = null;

					BattleControlor.Instance().flags.TryGetValue(flagId, out bf);

					if(bf != null) bf.trigger();
				}
			}
		}
	}

	public void updataAttackRange()
	{
		if(nav != null)
		{
			nav.speed = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed );
		}
	}

	public void updateEnemysList()
	{
		if(nodeData.nodeType == NodeType.PLAYER)
		{
			enemysInRange.Clear();

			List<BaseAI> temp = stance == Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;

			foreach(BaseAI node in temp)
			{
				enemysInRange.Add(node);
			}
		}

		foreach(BaseAI node in enemysInRange)
		{
			if(node == null || !node.isAlive)
			{
				enemysInRange.Remove(node);

				return;
			}
		}
	}

	private Vector3 tempMovePostion = Vector3.zero;

	public void MovePositonTween( Vector3 p_offset )
	{
		if (isAlive == false) return;

		Vector3 step = p_offset - tempMovePostion;

		tempMovePostion = p_offset;

		character.Move(step);
	}

	public void moveAction(Vector3 targetPosition, iTween.EaseType easeType, float time, int colliderLevel = 0)
	{
//		if(colliderLevel == 0)
//		{
//			tempMovePostion = Vector3.zero;
//
//			iTween.ValueTo(gameObject, iTween.Hash(
//				"from", Vector3.zero,
//				"to", targetPosition - transform.position,
//				"time", time,
//				"easeType", easeType,
//				"onupdate", "MovePositonTween"
//				));
//
//			return;
//		}

		if(chongfengControllor == null)
		{
			GameObject chongfengObject = new GameObject ();
			
			chongfengObject.transform.parent = transform.parent;
			
			chongfengObject.transform.position = transform.position;
			
			chongfengObject.transform.eulerAngles = transform.eulerAngles;
			
			chongfengObject.transform.localScale = new Vector3(1, 1, 1);
			
			chongfengControllor = chongfengObject.AddComponent<CharacterController>();

			chongfengControllor.radius = character.radius;

			chongfengControllor.center = new Vector3(0, 1.5f, 0);

			chongfengControllor.height = 3f;
		}
		
		chongfengControllor.transform.position = transform.position;

		if(colliderLevel > 0)
		{
			foreach(BaseAI a in BattleControlor.Instance().enemyNodes)
			{
				a.setNavMeshRadiusTemp(0);

				a.character.radius = 0;

				if(a.nodeId == nodeId) continue;

				a.transform.position += new Vector3(0, 5000, 0);

				//a.character.enabled = false;
			}

			foreach(BaseAI a in BattleControlor.Instance().selfNodes)
			{
				a.setNavMeshRadiusTemp(0);

				a.character.radius = 0;

				if(a.nodeId == nodeId) continue;

				a.transform.position += new Vector3(0, 5000, 0);

				//a.character.enabled = false;
			}
		}

		float length = Vector3.Distance (targetPosition, transform.position);

		chongfengControllor.transform.forward = targetPosition - transform.position;

		chongfengControllor.transform.localEulerAngles += new Vector3 (0, 3, 0);

		chongfengControllor.Move (chongfengControllor.transform.forward * length);

		if(colliderLevel > 0)
		{
			foreach(BaseAI a in BattleControlor.Instance().enemyNodes)
			{
				a.setNavMeshRadiusTemp(a.radius);
				
				a.character.radius = a.radius;
	
				if(a.nodeId == nodeId) continue;
	
				a.transform.position += new Vector3(0, -5000, 0);
	
				//a.character.enabled = true;
			}
			
			foreach(BaseAI a in BattleControlor.Instance().selfNodes)
			{
				a.setNavMeshRadiusTemp(a.radius);
				
				a.character.radius = a.radius;
	
				if(a.nodeId == nodeId) continue;
	
				a.transform.position += new Vector3(0, -5000, 0);
	
				//a.character.enabled = true;
			}
		}

		if(colliderLevel < 2)
		{
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", transform.position,
				"to", chongfengControllor.transform.position,
				"time", time,
				"easeType", easeType,
				"onupdate", "PositonTweenWithoutCharactor",
				"oncomplete", "PositonTweenWithoutCharactorComplete"
				));
		}
		else
		{
			StrongMove();
		}
	}

	protected virtual void StrongMove()
	{
	
	}

	protected void PositonTweenWithoutCharactor(Vector3 p_offset)
	{
		if (inStrongMove) return;

		setTransformPosition (p_offset);
	}

	protected void PositonTweenWithoutCharactorComplete()
	{
		chongfengControllor.transform.position = new Vector3 (0, -500, 0);

//		foreach(BaseAI a in BattleControlor.Instance().enemyNodes)
//		{
//			a.setNavMeshRadiusTemp(a.radius);
//			
//			a.character.radius = a.radius;
//			
//			//a.character.enabled = true;
//		}
//		
//		foreach(BaseAI a in BattleControlor.Instance().selfNodes)
//		{
//			a.setNavMeshRadiusTemp(a.radius);
//			
//			a.character.radius = a.radius;
//			
//			//a.character.enabled = true;
//		}
	}

	protected void dropItem()
	{
		if (nodeData.droppenItems == null) return;

		BattleControlor.Instance().droppenList.Add (nodeId);

		if(nodeData.droppenType == 0)
		{
			foreach(DroppenItem di in nodeData.droppenItems)
			{
				dropItem(di);
			}
		}
		else if(nodeData.droppenType == 1)
		{
			droppenIndex = droppenIndex > nodeData.droppenItems.Count - 1 ? 0 : droppenIndex;

			DroppenItem di = nodeData.droppenItems[droppenIndex];

			droppenIndex ++;

			dropItem(di);
		}
	}

	private void dropItem(DroppenItem di)
	{
		BattleControlor.Instance().droppenDict[di.id] ++;

		Vector3 foward = new Vector3(Random.value, 0, Random.value).normalized;
		
		Vector3 targetPos = transform.position + foward * 2f;
		
		CommonItemTemplate commonItemTemplate = CommonItemTemplate.getCommonItemTemplateById(di.commonItemId);
		
		Object temple = null;
		
		BattleNet.Instance().modelList.TryGetValue(ModelTemplate.getModelTemplateByModelId(commonItemTemplate.dropModel).path, out temple);
		
		if(temple == null) 
		{
			return;
		}
		
		GameObject droppenObject = (GameObject)Instantiate(temple as GameObject);
		
		droppenObject.transform.parent = transform.parent;
		
		droppenObject.transform.position = transform.position;
		
		droppenObject.transform.localScale = new Vector3(1, 1, 1);
		
		DroppenAI da = droppenObject.GetComponent<DroppenAI>();
		
		da.refreshdata(targetPos, di);
	}

	public virtual void die(bool slowDown)
	{
		if (isAlive == false) return;

		//if (IsPlaying (getAnimationName(AniType.ANI_BATCDown)) == true) return;

		//dieActionDone ();

		//dropItem ();

		if(flag.dieable == true)
		{
			mAnim.SetTrigger (getAnimationName(AniType.ANI_Dead));
			
			if(slowDown == true)
			{
				dieActionDone ();
			}
		}
		else
		{
			dieActionDone ();
		}

		//if(flag.dieable == true) mAnim.Play (getAnimationName(AniType.ANI_Dead));
	}

	public void dieActionDone()
	{
		_dieActionDone ();
	}

	public void _dieActionDone(bool force = false)
	{
		if (BattleControlor.Instance().inDrama == true && force == false) return;

		if (isAlive == false) return;

		BubblePopControllor.Instance().triggerFuncDie (nodeId);

		StartCoroutine(dieAction());
	}

	public virtual IEnumerator dieAction()
	{
		for(;aiable == false;)
		{
			yield return new WaitForEndOfFrame();
		}

		foreach(BattleFlag f in flag.triggerFlagKill)
		{
			if(nodeId < 0) break;

			if(f == null || isAlive == false) continue;

			f.trigger();
		}

		List<Buff> list = new List<Buff> ();

		foreach(Buff buff in buffs)
		{
			list.Add(buff);
		}

		foreach(Buff buff in list)
		{
			buff.end();
		}

		isAlive = false;

		dropItem ();

		if(nodeId > 0)
		{
			if (BattleControlor.Instance().achivement != null && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING) BattleControlor.Instance().achivement.KillMonster (nodeId);

			if (nodeData.nodeType == NodeType.BOSS && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING) BattleControlor.Instance().battleCheck.bossKilled ++;

			if (nodeData.nodeType == NodeType.GEAR && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING) BattleControlor.Instance().battleCheck.gearKilled ++;

			if (nodeData.nodeType == NodeType.HERO && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING) BattleControlor.Instance().battleCheck.heroKilled ++;

			if (nodeData.nodeType == NodeType.SOLDIER && BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING) BattleControlor.Instance().battleCheck.soldierKilled ++;
		}

		if(stance == Stance.STANCE_SELF)
		{
			BattleControlor.Instance().selfNodes.Remove(this);
		}
		else if(stance == Stance.STANCE_ENEMY)
		{
			BattleControlor.Instance().enemyNodes.Remove(this);
		}
		else if(stance == Stance.STANCE_MID)
		{
			BattleControlor.Instance().midNodes.Remove(this);
		}

		if(flag.dieable == true) 
		{
			CharacterController cc = gameObject.GetComponent<CharacterController>();

			Destroy(cc);

			Destroy(nav);

			if(flag.willRelive == false)
			{
				Destroy(shadowObject_2);

				BattleControlor.Instance().deadNodes.Add(nodeId);
			}
			else
			{
				BattleControlor.Instance().reliveNodes.Add(this);
			}

			DisableOcclusion();

			yield return new WaitForSeconds(3.0f);

			if(isAlive == false)
			{
				Vector3 temppos = transform.position;

				if(nodeData.nodeType != NodeType.PLAYER)
				{
					if(body != null) {
						EffectTool.SetDeadEffect(body);
					}

					else {
						EffectTool.SetDeadEffect(gameObject);
					}

					yield return new WaitForSeconds(2.0f);
				}

				if(isAlive == false)
				{
					if(nodeData.nodeType == NodeType.PLAYER && stance == Stance.STANCE_SELF)
					{
						//body.SetActive(false);

						//transform.position = temppos;
					}
					else if(flag.willRelive == false)
					{
						Destroy(gameObject);
					}
					else
					{
						gameObject.SetActive(false);

						setTransformPosition(temppos);
					}
				}
			}
		}
	}

	public void relive()
	{
		if (isAlive == true) return;

		gameObject.SetActive (true);

		character = gameObject.AddComponent<CharacterController>();

		nav = gameObject.AddComponent<NavMeshAgent>();

		setNavMeshRadius(radius);

		isAlive = true;

		//transform.position += new Vector3(0, 2.5f, 0);

		RestoreOcclusion();

		mAnim.Play (getAnimationName( AniType.ANI_Stand0));

		mAnim.ResetTrigger (getAnimationName(AniType.ANI_Dead));

		if(stance == Stance.STANCE_ENEMY)
		{
			BattleControlor.Instance().enemyNodes.Add(this);
		}
		else if(stance == Stance.STANCE_SELF)
		{
			BattleControlor.Instance().selfNodes.Add(this);
		}
		else
		{
			BattleControlor.Instance().midNodes.Add(this);
		}

		BattleControlor.Instance().reliveNodes.Remove(this);

		initDate (nodeDataCopy.getNodeData(), m_Gameobj, modelId);

		if(nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_eyeRange) > 0)
		{
			nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange, 10000 );
		}
	}

	private void RestoreOcclusion()
	{
		if (nodeData.nodeType == NodeType.GEAR) return;

		if(!m_isStart)
		{
			m_isZhuge = true;

			return;
		}

		if (nodeData.nodeType == NodeType.PLAYER) return;

		if(body == null) 
		{
			EffectTool.RestoreBattleOcclusion (gameObject);

			EffectTool.DisableDeadEffect(gameObject);
		}
		else
		{
			EffectTool.RestoreBattleOcclusion (body);

			EffectTool.DisableDeadEffect(body);
		}

	}

	private void DisableOcclusion()
	{
		if (nodeData.nodeType == NodeType.GEAR) return;

		//if (nodeData.nodeType == NodeType.PLAYER) return;

		if(body == null){
			EffectTool.DisableBattleOcclusion (gameObject);
		}
		
		else {
			EffectTool.DisableBattleOcclusion (body);
		}
	}

	public void checkDieInKnockdown()
	{
		if (nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) > 0) return;

		mAnim.speed = 0;

		StartCoroutine(dieAction());
	}

	public void fadeIn()
	{
		gameObject.SetActive(true);

//		foreach(HeroSkill skill in skills)
//		{
//			skill.wake();
//		}
	}

	public void addNuqi(float addNuqi)
	{
		float nuqi = nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_NUQI);

		nuqi += addNuqi;

		float nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX);

		#if UNITY_EDITOR && REMOVE_MIBAO_CD
		nuqi = nuqiMax;
		#endif
			
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 0 && CityGlobalData.m_tempLevel == 1)
		{
			nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX_0);
		}

		nuqi = nuqi > nuqiMax ? nuqiMax : nuqi;

		nuqi = nuqi < 0 ? 0 : nuqi;

		float tempNuqi = nuqi - nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_NUQI);

		nodeData.SetAttribute (AIdata.AttributeType.ATTRTYPE_NUQI, nuqi);

		if(nodeId == 1 && BattleControlor.Instance().getKing().kingSkillMibao != null && BattleControlor.Instance().getKing().kingSkillMibao.Count > 0)
		{
			BattleUIControlor.Instance().spriteBarMibao.fillAmount = nuqi / nuqiMax;

			float unityMax = nuqiMax / 3;

			int count = (int)(nuqi / unityMax);

			BattleUIControlor.Instance().spriteMibaoFrame.SetActive(count == 0);

			bool have = UI3DEffectTool.HaveAnyFx(BattleUIControlor.Instance().btnMibaoSkill);

			if(count == 0)
			{
				BattleUIControlor.Instance().labelBarMibao.text = "";

//				setHeartOn(false);
			}
			else if(count == 1)
			{
				BattleUIControlor.Instance().labelBarMibao.text = "x1";

				if(BattleUIControlor.Instance().b_skill_miBao == true)
				{
					if(have == false) UI3DEffectTool.ShowTopLayerEffect(
						UI3DEffectTool.UIType.FunctionUI_1, 
						BattleUIControlor.Instance().btnMibaoSkill,
						EffectIdTemplate.GetPathByeffectId(100189) );

//					setHeartOn(true);
				}
			}
			else if(count == 2)
			{
				BattleUIControlor.Instance().labelBarMibao.text = "x2";

				if(BattleUIControlor.Instance().b_skill_miBao == true)
				{
					if(have == false) UI3DEffectTool.ShowTopLayerEffect(
						UI3DEffectTool.UIType.FunctionUI_1, 
						BattleUIControlor.Instance().btnMibaoSkill,
						EffectIdTemplate.GetPathByeffectId(100189) );

//					setHeartOn(true);
				}
			}
			else if(count == 3)
			{
				BattleUIControlor.Instance().labelBarMibao.text = "MAX";

				if(BattleUIControlor.Instance().b_skill_miBao == true)
				{
					if(have == false) UI3DEffectTool.ShowTopLayerEffect(
						UI3DEffectTool.UIType.FunctionUI_1, 
						BattleUIControlor.Instance().btnMibaoSkill,
						EffectIdTemplate.GetPathByeffectId(100189) );

//					setHeartOn(true);
				}
			}

			if(count > 0 
			   && have == false 
			   && CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan 
			   && CityGlobalData.m_configId == 100204
			   ) ClientMain.Instance().m_SoundPlayEff.PlaySound("811800");
		}
	}

	public void setHeartOn(bool on)
	{
		BattleUIControlor.Instance().btnMibaoSkillIconAnim.enabled = on;
		
		BattleUIControlor.Instance().btnMibaoSkillIconAnim_1.gameObject.SetActive(on);
		
		BattleUIControlor.Instance().btnMibaoSkillIconAnim_2.gameObject.SetActive(on);
	}

	protected void clearNuQi()
	{
		float nuqi = nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_NUQI);

		float nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 0 && CityGlobalData.m_tempLevel == 1)
		{
			nuqiMax = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.NUQI_MAX_0);
		}

		float unityMax = nuqiMax / 3;

		int count = (int)(nuqi / unityMax);

		addNuqi ( -unityMax * count );
	}

	private IEnumerator minusTargetHp(BaseAI defender)
	{
		yield return new WaitForSeconds(0.7f);

		if(nodeData.nodeProfession == NodeProfession.GONG || nodeData.nodeProfession == NodeProfession.GONG)
		{
			transform.forward = defender.transform.position - transform.position;

			KingArrow.createArrow(this, BattleControlor.AttackType.BASE_ATTACK);
		}
		else
		{
			FloatBoolParam fbp = BattleControlor.Instance().getAttackValue(this, defender);

			foreach(Buff buff in buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
				{
					attackHp(this, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.BASE_REFLEX, BattleControlor.NuqiAddType.NULL);

					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

					defender.showText(LanguageTemplate.GetText( LanguageTemplate.Text.BATTLE_BASE_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

					break;
				}
			}

			BattleControlor.NuqiAddType nuqiType = BattleControlor.NuqiAddType.NULL;

			if(nodeData.nodeType == NodeType.PLAYER)
			{
				KingControllor king = (KingControllor)this;

				if(king.weaponType == KingControllor.WeaponType.W_Heavy) nuqiType = BattleControlor.NuqiAddType.HEAVY_BASE;

				else if(king.weaponType == KingControllor.WeaponType.W_Light) nuqiType = BattleControlor.NuqiAddType.LIGHT_BASE;

				else if(king.weaponType == KingControllor.WeaponType.W_Ranged) nuqiType = BattleControlor.NuqiAddType.RANGE_BASE;
			}

			attackHp(defender, fbp.Float, fbp.Bool, BattleControlor.AttackType.BASE_ATTACK, nuqiType);
		}
	}

	public virtual bool isPlayingAttack()
	{
		if (isIdle == true) return true;

		string actionName = IsPlaying ();
		
		if (actionName.IndexOf(getAnimationName(AniType.ANI_Attack_0)) != -1) return true;

		if (actionName.IndexOf("Attack_1") != -1) return true;

		if (actionName.IndexOf("Attack_2") != -1) return true;

		if (actionName.IndexOf("Skill_1") != -1) return true;

		if (actionName.IndexOf("Skill_2") != -1) return true;

		if (actionName.IndexOf("Skill_1_1") != -1) return true;

		if (actionName.IndexOf("Skill_1_2") != -1) return true;

		if (actionName.IndexOf ("Skill_") != -1) return true;

		if (actionName.IndexOf (getAnimationName(AniType.ANI_BATC)) != -1) return true;

		if (actionName.IndexOf (getAnimationName(AniType.ANI_BATCDown)) != -1) return true;

		if (actionName.IndexOf (getAnimationName(AniType.ANI_Dead)) != -1) return true;

		return false;
	}

	public virtual bool isPlayingSkill()
	{
		if (m_sPlaySkillAnimation.Length > 0) return true;

		string playing = IsPlaying ();

		if (playing.IndexOf ("Skill") != -1) return true;

		string nextPlying = nextPlaying ();

		if (nextPlying.IndexOf ("Skill") != -1) return true;

		return false;
	}

	public virtual bool isPlayingSwing()
	{
		return false;
	}

	public void attacked(BaseAI attacker, float hpValue, bool cri, BattleControlor.AttackType attackedType, BattleControlor.NuqiAddType nuqiType)
	{
		//BattleReplayorWrite.Instance().addReplayNodeAttacked(nodeId, hpValue);

		if(nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0 && attacker.nodeData.nodeType != NodeType.PLAYER)
		{
			updataBloodBar();

			return;
		}

		BubblePopControllor.Instance().triggerFuncBATC (nodeId);

		if(attackedType == BattleControlor.AttackType.BASE_ATTACK) m_listByAtk.Add (attacker);

		else m_listBySkill.Add(attacker);

		//if(attackedType == BattleControlor.AttackType.BASE_ATTACK)
		{
			addNuqi ((float)CanshuTemplate.GetValueByKey(CanshuTemplate.NUQI_SHENGMING_REDUCE) * hpValue / nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpMaxReal));
		}

		if(attacker.nodeData.nodeType != NodeType.GEAR && nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_eyeRange) > .1f) 
		{
			OnTriggerEnterNode ((Collider)attacker.GetComponent(typeof(Collider)));
		}

		bool flag = nodeData.nodeType == NodeType.PLAYER;

		inHover = false;

		sleep = false;

		attackedTime = System.DateTime.Now;

		if(nodeData.nodeType != NodeType.PLAYER && hpValue > 0 && nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_eyeRange) >= 0)
		{
			setNavMeshStop();
		}

		if(flag == true)
		{
			flag = !IsPlaying(getAnimationName(AniType.ANI_Stand0));
		}

		if(flag == false && hpValue > 0)
		{
			bool f = false;

			if(attacker.nodeData.nodeType == NodeType.PLAYER)
			{
				KingControllor king = (KingControllor)attacker;

				f = king.attackBaseAI(this, hpValue, cri, nuqiType);
			}

			if(f == false && nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) > 0)
			{
				string playing = IsPlaying();

				string nextP = nextPlaying();

				if(nodeData.nodeType == NodeType.PLAYER || isPlayingAttack() || isPlayingSkill() || m_iUseSkillIndex != -1 || nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_eyeRange) < 0)
				{

				}
				else if(playing.Equals(getAnimationName( AniType.ANI_Stand0))
				   || playing.Equals(getAnimationName( AniType.ANI_Stand1))
				   || playing.Equals(getAnimationName( AniType.ANI_BATC))
				   )
				{
					mAnim.SetTrigger(getAnimationName(AniType.ANI_BATC));

					setNavMeshStop();
				}
			}

//			if(nodeData.nodeType != NodeType.PLAYER)
//			{
//				Buff.createBuff(this, 
//					AIdata.AttrubuteType.ATTRTYPE_moveSpeed,
//				    -nodeData.GetAttribute( (int)AIdata.AttrubuteType.ATTRTYPE_moveSpeed ),
//				    .5f);
//			}
		}

		if(attacker.stance == Stance.STANCE_SELF)
		{
			//HeroAI hero = (HeroAI)attacker;
			if(BattleControlor.Instance().hurts.ContainsKey(attacker.nodeId))
			{
				BattleControlor.Instance().hurts[attacker.nodeId] += (int)hpValue;
			}
		}

		{
			GameObject t_gb = BattleEffectControllor.Instance().PlayEffect(67, transform.position + new Vector3(0, 1.2f, 0), transform.forward);

			if( !QualityTool.GetBool( QualityTool.CONST_CHARACTER_HITTED_FX ) ){
				ComponentHelper.DisableAllVisibleObject( t_gb );
			}
		}

		if(nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) > -hpValue
		   || hpValue > 0)
		{
			if(gameObject.activeSelf == true) 
			{
				BloodLabelControllor.Instance().showBloodEx(this, (int)hpValue, cri, attackedType);
			}
		}

		updataBloodBar();
	}

	public void showText(string text, int skillId)
	{
		if (skillId == tempSkillId) return;

		tempSkillId = skillId;

		BloodLabelControllor.Instance().showText(this, text, showTextCallback);
	}

	public void showTextCallback()
	{
		tempSkillId = 0;
	}

	public bool beatDown(int beatdownTemplateId)
	{
		string playing = IsPlaying ();

		if (playing.IndexOf (getAnimationName (BaseAI.AniType.ANI_DODGE)) != -1) return false;

		if (playing.IndexOf (getAnimationName (BaseAI.AniType.ANI_BATCDown)) != -1) return false;

		if (playing.IndexOf (getAnimationName (BaseAI.AniType.ANI_BATCUp)) != -1) return false;

		bool f = ControlOrderLvTemplate.haveBeatDownTemplateById (beatdownTemplateId);

		if (f == false) return false;

		bool able = ControlOrderLvTemplate.getCantrolableById (beatdownTemplateId, AIdata.AttributeType.ATTRTYPE_ReductionBTACDown, this);

		if (able == false) return false;

		mAnim.SetTrigger(getAnimationName(AniType.ANI_BATCDown));

		if(nodeData.nodeType == NodeType.PLAYER)
		{
			KingSkillWuDiZhan wudizhan = gameObject.GetComponent<KingSkillWuDiZhan>();

			wudizhan.cut();
		}

		setNavMeshStop();

		targetNode = null;

		if(BattleControlor.Instance().achivement != null && nodeId == 1)
		{
			BattleControlor.Instance().achivement.ByJiDao();
		}

		for(int i = 0; i < skills.Count; i ++)
		{
			if(skills[i].m_isUseThisSkill)
			{
				skills[i].ForcedTermination();
			}
		}

		return true;
	}

	public void BATC()
	{
		if (isAlive == false || nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) <= 0) return;

		string playing = IsPlaying ();

		if(playing.Equals(getAnimationName(AniType.ANI_Stand0)) == false 
		   && playing.Equals(getAnimationName(AniType.ANI_Stand1)) == false)
		{
			return;
		}

		if (isPlayingSkill () == true || isPlayingAttack () == true) return;

		mAnim.SetTrigger(getAnimationName(AniType.ANI_BATC));
		
		setNavMeshStop();
		
		for(int i = 0; i < skills.Count; i ++)
		{
			if(skills[i].m_isUseThisSkill)
			{
				skills[i].ForcedTermination();
			}
		}
	}

	public void rupt()//打断，强制播放受击动作
	{
		if (nodeData.nodeType == NodeType.PLAYER && isPlayingSkill () == true) return;

		if (nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return;

		if (IsPlaying ().IndexOf (getAnimationName (BaseAI.AniType.ANI_DODGE)) != -1) return;

		mAnim.SetBool(getAnimationName(AniType.ANI_BATC), true);

		setNavMeshStop();
		
		for(int i = 0; i < skills.Count; i ++)
		{
			if(skills[i].m_isUseThisSkill)
			{
				skills[i].ForcedTermination();
			}
		}

		if (BattleControlor.Instance().achivement != null)
			BattleControlor.Instance().achivement.Interrupt (nodeId);
	}

	public void refreshRupt()//已废弃
	{
		if (BattleControlor.Instance().inDrama == true) return;
	}

	protected virtual void playAttackEffect(int attackId)
	{
		if (BattleControlor.Instance().inDrama == true) return;

		if(attackId == 90)
		{
			//被击

//			BattleEffectControllor.Instance().PlayEffect(
//				67,
//				gameObject);
		}
		else
		{
			BattleEffectControllor.Instance().PlayEffect(
				attackId,
				gameObject);
		}
	}

	private void attackedActionStart()
	{
		if (BattleControlor.Instance().inDrama == true) return;

		if(curCrashData == null) return;

		mAnim.speed = curCrashData.actionSpeed;
	}

	private void attackedActionEnd()
	{
		if (true) return;

		if (BattleControlor.Instance().inDrama == true) return;

		if (aiable == false) return;

		curCrashData = null;

		mAnim.speed = 1.0f;

		if(nodeData.nodeType == NodeType.SOLDIER)
		{
			System.DateTime now = System.DateTime.Now;

			System.TimeSpan span = now - runawayTime;

			if(span.TotalSeconds < 10)
			{
				return;
			}
		
			float v = Random.value * 100;

			if(v < 20) runaway(1.0f);
		}
		else if(nodeData.nodeType == NodeType.PLAYER)
		{
			if(BattleControlor.Instance().autoFight == false && stance == Stance.STANCE_SELF) return;

			runaway(1.0f);
		}
	}

	public void movement(float delay, Vector3 targetPosition, iTween.EaseType easeType, float time)
	{
		StartCoroutine (attackedMovement(delay, targetPosition, easeType, time));
	}

	public IEnumerator attackedMovement(float delay, Vector3 targetPosition, iTween.EaseType easeType, float time)
	{
		if(nodeData.nodeType != NodeType.GEAR)
		{
			yield return new WaitForSeconds (delay);

			if (isAlive == true)
			{
				iTween.ValueTo(gameObject, iTween.Hash(
					"name", "action_" + nodeId,
					"from", transform.position,
					"to", targetPosition,
					"time", time,
					"easeType", easeType,//iTween.EaseType.easeOutExpo,
					"onupdate", "PositonTween"
					));
			}
		}
	}

	public void PositonTween( Vector3 p_offset )
	{
		if (isAlive == false) return;

		character.Move(p_offset - transform.position);
	}

	public void clearTrails()
	{
		if (BattleControlor.Instance().inDrama == true) return;

		foreach(GameObject trailObject in trails)
		{
			Destroy(trailObject);
		}

		trails.Clear();
	}

	public List<Buff> getBuffs()
	{
		return buffs;
	}

	public void setNavMeshDestinationReal(Vector3 targetPosition)
	{
		nav.SetDestination(targetPosition);

		nav.Resume ();
	}

	public void setNavMeshDestination(Vector3 targetPosition)
	{
		if (isPlayingAttack () == true) 
		{
			setNavMeshStop();

			return;
		}

		if(isPlayingSkill() == true)
		{
			string strPlaying = IsPlaying();

			if (strPlaying.IndexOf ("XuanFengZhan") == -1)
			{
				setNavMeshStop();

				return;
			}
		}

		string playing = IsPlaying ();

		if (playing.Equals (getAnimationName (AniType.ANI_BATC)) == true)
		{
			setNavMeshStop();

			return;
		}

		if (playing.Equals (getAnimationName(AniType.ANI_BATCDown)) == true)
		{
			setNavMeshStop();

			return;
		}

		if (playing.Equals (getAnimationName(AniType.ANI_BATCUp)) == true)
		{
			setNavMeshStop();

			return;
		}

		string nextPlay = nextPlaying ();

		if (nextPlay.Equals (getAnimationName (AniType.ANI_BATC)) == true)
		{
			setNavMeshStop();
			
			return;
		}
		
		if (nextPlay.Equals (getAnimationName(AniType.ANI_BATCDown)) == true)
		{
			setNavMeshStop();
			
			return;
		}
		
		if (nextPlay.Equals (getAnimationName(AniType.ANI_BATCUp)) == true)
		{
			setNavMeshStop();
			
			return;
		}

		if (nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_moveSpeed) == 0) return;

		if (inHover == true) return;

		float length = Vector3.Distance (targetPosition, nav.destination);

		if (length < .5f && nodeData.nodeType != NodeType.PLAYER) return;

		if(inTurning == false) StartCoroutine (turnToDestination(targetPosition));
	}
	
	IEnumerator turnToDestination(Vector3 targetPosition)
	{
		inTurning = true;

		Vector3 nextPosition = targetPosition;

		NavMeshPath tempPath = null;

		for(; nav.enabled == true;)
		{
//			setNavDestination(targetPosition);

			if(tempPath != null && tempPath.Equals(nav.path) == false)
			{
				if(nav.path.corners.Length > 1 
				   && Vector3.Distance(nav.path.corners[1], Vector3.zero) > .1f)
				{
					nextPosition = nav.path.corners[1];
				}

				break;
			}

			if( nav != null ){
				tempPath = nav.path;
			}

			yield return new WaitForEndOfFrame();
		}

		for(int i = 0; inTurning == true; i++)
		{
			Vector3 oldangle = transform.eulerAngles;

			transform.forward = nextPosition - transform.position;

			float tar = transform.eulerAngles.y; 

			//float sp = 120 * Time.deltaTime; 

			float sp = 1080 * Time.deltaTime; 

			float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);

			transform.eulerAngles = new Vector3(0, angle, 0);

			if(Mathf.Abs(tar - oldangle.y) < 20)
			{
				break;
			}

			yield return new WaitForEndOfFrame();
		}

		if(isAlive == true && nav.enabled == true && gameObject.activeSelf == true && inTurning == true) 
		{
			setNavDestination(targetPosition);
		}

		yield return new WaitForSeconds (.5f);

		inTurning = false;
	}

	private void setNavDestination(Vector3 destination)
	{
		nav.Resume ();

		if(Vector3.Distance(destination, nav.destination) < 1f)
		{
//			return;
		}

		nav.SetDestination(destination);
	}

	public void addNavMeshSpeed(float tempSpeed)
	{
		float t_attr = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed );

		nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed, t_attr + tempSpeed );

		updataAttackRange();
	}

	public void setNavMeshSpeed(float _speed)
	{
		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed, _speed );
	}

	public void setNavMeshPriority(int priority)
	{
		nav.avoidancePriority = priority;
	}

	public void stopAllForSkill()
	{
		//targetNode = null;

		StartCoroutine (stopAllForSkillAction());
	}

	IEnumerator stopAllForSkillAction()
	{
		perpareForSkill = true;

		setNavMeshStop ();
		
		yield return new WaitForSeconds (1f);

		perpareForSkill = false;
	}

	public void setNavMeshStop()
	{
		if(gameObject.activeSelf == true && nav != null && nav.enabled == true)
		{
			nav.destination = transform.position;
			
			nav.Stop();
		}

		inTurning = false;

		if( mAnim.isActiveAndEnabled )
		{
			if( AnimatorHelper.HaveParameter( mAnim, "move_speed" ) )
			{
				mAnim.SetFloat("move_speed", 0);	
			}
		}

		moveZeroCount = 0;
	}

	public float getNavMeshSpeed()
	{
		return nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed );
	}

	public void setNavMeshRadius(float _radius)
	{
		radius = _radius;

		nav.radius = radius;

		character.radius = radius;
	}

	public void setNavMeshRadiusTemp(float _radius)
	{
		nav.radius = _radius;
		
		character.radius = _radius;
	}

	public float getNavMeshRadius()
	{
		return nav.radius;
	}

	public float getNavMeshRemainingDistance()
	{
		return nav.remainingDistance;
	}

	public void setNavEnabled(bool enable)
	{
		nav.enabled = enable;
	}

	public void setTargetPosition(Vector3 _targetPosition)
	{
		tarPosition = _targetPosition;
	}

	public void setTransformPosition(Vector3 _position)
	{
		transform.position = _position;
	}

	public virtual void setWeaponTriggerTrue(int _aid)
	{
		if (BattleControlor.Instance().inDrama == true) return;

		weapon.gameObject.SetActive(false);
		
		weapon.gameObject.SetActive(true);
		
		weapon.setTriggerable(true);
	}

	public virtual void setWeaponTriggerFalse(int hand)
	{
		if (BattleControlor.Instance().inDrama == true) return;

		weapon.gameObject.SetActive(false);

		if (BattleControlor.Instance().completed == false) return;

		weapon.setTriggerable(false);
	}

	public virtual void createArrow(int _actionId)
	{
		KingArrow.createArrow(this, BattleControlor.AttackType.BASE_ATTACK);
	}

	public virtual void arrowAttackCallback(int _aid, BaseAI defenderNode)
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		OnTriggerEnterNode (other);
	}

	private bool triggerable = true;

	public void OnTriggerEnterNode(Collider other)
	{
		if (triggerable == false) return;

		BaseAI node = (BaseAI)other.gameObject.GetComponent("BaseAI");

		if(node == null || !node.isAlive) return;

		if (node.stance == stance)
		{
			if(node.alarmState == 1)
			{
				node = BattleControlor.Instance().selfNodes[0];
			}
			else
			{
				return;
			}
		}

		BattleUIControlor.Instance().barEnemy.setFocusNode (this);

		if(targetNode == null && stance == Stance.STANCE_ENEMY)
		{
			RestoreOcclusion ();
		}

		if(flag != null && Vector3.Distance(flag.alarmPosition, Vector3.zero) > 1)
		{
			alarm (flag.alarmPosition);
		}

		if(nodeData.nodeType != NodeType.PLAYER && nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_eyeRange) >= 0)
		//if( stance == Stance.STANCE_ENEMY || CityGlobalData.m_enterPvp == true )
		{
			nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange, 0 );

			SphereCollider sc = (SphereCollider)GetComponent("SphereCollider");

			if(sc != null) sc.radius = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange );
		}

		bool b = checkEnemysInRange( node );

		if( b == true )
		{
			if( nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange ) <= 0 )
			{
				enemysInRange.Clear();

				List<BaseAI> temp = stance == Stance.STANCE_ENEMY ? BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes;

				foreach( BaseAI n in temp )
				{
					enemysInRange.Add(n);
				}
			}
			else
			{
				enemysInRange.Add(node);
			}

			if(flag != null)
			{
				List<BattleFlag> list = new List<BattleFlag>();

				foreach(BattleFlag f in flag.triggerFlagEnter)
				{
					list.Add(f);
				}

				flag.triggerFlagEnter.Clear();

				foreach(BattleFlag f in list)
				{
					f.trigger(node);
				}

				foreach(BattleDoorFlag door in flag.doorFlags)
				{
					door.trigger();
				}

				List<BattleFlag> listEye = new List<BattleFlag>();
				
				foreach(BattleFlag f in flag.triggerFlagEye2eye)
				{
					listEye.Add(f);
				}
				
				flag.triggerFlagEye2eye.Clear();

				foreach(BattleFlag f in listEye)
				{
					f.OnTriggerEyeToEye(node);
				}
			}
		}

		BubblePopControllor.Instance().triggerFuncOpenEye (nodeId);

		triggerable = false;
	}

	public void OnTriggerExit(Collider other)
	{
		if (nodeData == null) return;

		if (nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange ) == 0) return;

		BaseAI node = (BaseAI)other.gameObject.GetComponent("BaseAI");

		enemysInRange.Remove(node);
	}

	private bool checkEnemysInRange(BaseAI node)
	{
		foreach(BaseAI ai in enemysInRange)
		{
			if(ai.nodeId == node.nodeId)
			{
				return false;
			}
		}

		return true;
	}

	public void setShadowColor(Color _shadowColor)
	{
		shadowColor = _shadowColor;

		shadowObject.GetComponent<Renderer>().material.color = shadowColor;
	}

	public void addHp(float hpValue)
	{
		float t_attr = nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );

		nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp, t_attr + hpValue );

		nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_hp,
				nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp )
				> nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMaxReal )
		        ? nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMaxReal )
				: nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) );

		BloodLabelControllor.Instance().showBloodEx(this, (int)hpValue, false, BattleControlor.AttackType.ADD_HP);

		updataBloodBar ();
	}

	public void attackHp(BaseAI defender, float hpValue, bool cri, BattleControlor.AttackType attackedType, BattleControlor.NuqiAddType nuqiType)
	{
		if ( defender == null ) return;

		float t_attr = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );

		defender.nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp, t_attr - hpValue );

		Buff.createBuffThreat (defender, (float)CanshuTemplate.m_TaskInfoDic[CanshuTemplate.GONGJICHOUHEN_ADD], (float)CanshuTemplate.m_TaskInfoDic[CanshuTemplate.GONGJICHOUHEN_TIME], nodeId, 0);

		if (attackedType == BattleControlor.AttackType.BASE_ATTACK) m_listAtk.Add (defender);

		else m_listSkill.Add (defender);

		defender.attacked(this, hpValue, cri, attackedType, nuqiType);

		if( defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0 )
		{
			BattleControlor.BattleResult fRes = BattleControlor.Instance().battleCheck.checkResult (defender);

			bool slowdown = fRes != BattleControlor.BattleResult.RESULT_BATTLING;

			if(slowdown)
			{
				BattleControlor.Instance().ResultSlowDown();
			}

			defender.die(slowdown);
		}
	}

	public void firstAtt()
	{
		BattleControlor tempBattleControlor = BattleControlor.Instance();
		for(int i = 0; i < tempBattleControlor.selfNodes.Count; i ++)
		{
			if(tempBattleControlor.selfNodes[i].isAlive && tempBattleControlor.selfNodes[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0)
			{
				tempBattleControlor.selfNodes[i].dieActionDone();
			}
		}
		for(int i = 0; i < tempBattleControlor.enemyNodes.Count; i ++)
		{
			if(tempBattleControlor.enemyNodes[i].isAlive && tempBattleControlor.enemyNodes[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0)
			{
				tempBattleControlor.enemyNodes[i].dieActionDone();
			}
		}
		for(int i = 0; i < tempBattleControlor.reliveNodes.Count; i ++)
		{
			if(tempBattleControlor.reliveNodes[i].isAlive && tempBattleControlor.reliveNodes[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0)
			{
				tempBattleControlor.reliveNodes[i].dieActionDone();
			}
		}
	}

	IEnumerator lastCameraEffect(BaseAI attacker, BaseAI defender)
	{
		Time.timeScale = 0.2f;

//		if(defender.nodeId == BattleControlor.Instance().getKing().nodeId && false)
//		{
//			TweenPosition.Begin(Camera.main.gameObject, 0.2f, defender.transform.position + new Vector3(0, 1.7f, -3.4f));
//			
//			TweenRotation.Begin(Camera.main.gameObject, 0.2f, new Quaternion(0, 0, 0, 0));
//		}
//		else
		{
			Transform t_trs = Camera.main.transform.parent;
			
			Vector3 t_po = Camera.main.transform.position;
			
			Quaternion t_ro = Camera.main.transform.rotation;
			
			
			Camera.main.transform.transform.parent = attacker.transform;
			
			Camera.main.transform.localScale = new Vector3(1f, 1f, 1f);
			
			Camera.main.transform.localEulerAngles = new Vector3(29.3f, 0, 0);
			
			Camera.main.transform.localPosition = new Vector3(0, 5.4f, -7.5f);
			
			
			Vector3 m_po = Camera.main.transform.position;
			
			Quaternion m_ro = Camera.main.transform.rotation;
			
			
			Camera.main.transform.parent = t_trs;
			
			Camera.main.transform.localScale = new Vector3(1f, 1f, 1f);
			
			Camera.main.transform.rotation = t_ro;
			
			Camera.main.transform.position = t_po;
			
			
			TweenPosition.Begin(Camera.main.gameObject, 0.2f, m_po);
			
			TweenRotation.Begin(Camera.main.gameObject, 0.2f, m_ro);
		}
		
		BattleControlor.Instance().lastCameraEffect = true;
		
		yield return new WaitForSeconds(2.0f);
		
		Time.timeScale = 1f;
		
		//yield return new WaitForSeconds(0.2f);
		
		BattleControlor.Instance().lastCameraEffect = false;
	}

	public virtual string getAnimationName(AniType aniType)
	{
		int index = (int)aniType;

		return m_strAnimationName[index];
	}

	public void stopTime(string data)
	{
		if(BattleControlor.Instance().inDrama == true)
		{
			return;
		}

		string[] tempdata = data.Split(',');
		m_isStopTime = true;
		m_fStopTime = float.Parse(tempdata[0]);
		m_fStopBTime = Time.time;
		mAnim.speed = float.Parse(tempdata[1]);
	}

	public HeroSkill getSkillById(int skillId)
	{
		foreach(HeroSkill skill in skills)
		{
			if(skill.template.id == skillId) return skill;
		}

		return null;
	}

	public void deleteBuff(SkillDeleteBuff skillDeleteBuff)
	{
		if (skillDeleteBuff == null) return;

		for(int q = 0; q < buffs.Count; q ++)
		{
			Buff buff = buffs[q];

			if(buff != null && buff.supplement != null && skillDeleteBuff.m_iBuffType == buff.supplement.m_iBuffType)
			{
				//Debug.Log("DDDDDDDDDDDDDDD   " + nodeId + ", " + buffs[q].buffType);

				buffs[q].end();
				q --;
				continue;
			}
		}
	}

	public void setLayer(int layer)
	{
		Renderer[] rens = GetComponentsInChildren<Renderer>();
		
		foreach(Renderer ren in rens)
		{
			GameObjectHelper.SetGameObjectLayer( ren.gameObject, layer );
		}
	}

	public float getHeight()
	{
		return appearanceTemplate.height * ModelTemplate.getModelTemplateByModelId(modelId).height;
	}

}

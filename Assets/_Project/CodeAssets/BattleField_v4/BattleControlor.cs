using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleControlor : MonoBehaviour
{
	public enum BattleResult
	{
		RESULT_BATTLING,
		RESULT_WIN,
		RESULT_LOSE,
	}

	public enum AttackType
	{
		BASE_ATTACK,
		SKILL_ATTACK,
		MIBAO_ATTACK,
		ADD_HP,
		DEFAULT,
	}

	public GameObject selfTeam;

	public GameObject enemyTeam;

	public GameObject flagRoot;

	public List<GameObject> cameraFlags;

	public GameObject cameraEnd;


	[HideInInspector] public int battleId;

	[HideInInspector] public bool autoFight;

	[HideInInspector] public List<BaseAI> selfNodes = new List<BaseAI>();
	
	[HideInInspector] public List<BaseAI> enemyNodes = new List<BaseAI>();

	[HideInInspector] public List<BaseAI> midNodes = new List<BaseAI> ();

	[HideInInspector] public List<BaseAI> reliveNodes = new List<BaseAI> ();

	[HideInInspector] public List<int> deadNodes = new List<int> ();
	
	[HideInInspector] public BattleResult result;

	[HideInInspector] public bool inCameraEffect_camera;

	[HideInInspector] public bool inCameraEffect_update;

	[HideInInspector] public bool lastCameraEffect;

	[HideInInspector] public float battleTime;

	[HideInInspector] public float totalBloodSelf;
	
	[HideInInspector] public float totalBloodEnemey;

	[HideInInspector] public Dictionary<int, int> hurts = new Dictionary<int, int>();

	[HideInInspector] public Dictionary<int, BattleFlag> flags = new Dictionary<int, BattleFlag>();

	[HideInInspector] public Dictionary<int, BattleBuffFlag> buffFlags = new Dictionary<int, BattleBuffFlag> ();

	[HideInInspector] public bool inDrama;

	[HideInInspector] public int timeLast;

	[HideInInspector] public List<GameObject> GuangQiangList = new List<GameObject>();

	[HideInInspector] public GameObject GuangQiang_Forever;

	[HideInInspector] public List<GuideTemplate> guidePlayed = new List<GuideTemplate> ();

	[HideInInspector] public bool completed;

	[HideInInspector] public List<int> mibaoIds = new List<int>();

	[HideInInspector] public JianDunDataManager achivement;

	[HideInInspector] public Dictionary<int, int> droppenDict = new Dictionary<int, int>();

	[HideInInspector] public List<int> droppenList = new List<int> ();

	[HideInInspector] public float HYK;//荒野战斗中计算伤害的系数K

	[HideInInspector] public BattleCheckResult battleCheck;


	private KingControllor king;

	private static BattleControlor _instance;

	private GameObject shadowTemple;
	
	private GameObject shadowTemple_2;
	
	private GameObject labelTemple_base;

	private GameObject labelTemple_enemy;

	private GameObject labelTemple_addHp;

	private GameObject labelTemple_skill;

	private GameObject labelTemple_mibao;

	private GameObject labelTemple_default;

	private GameObject bloodbarTemple_red;
	
	private GameObject bloodbarTemple_green;

	private bool showHint;

	private int preLoadEffCount;


	void Awake()
	{
		_instance = this; 
	}

	public static BattleControlor Instance() { return _instance; }


	#region Prepare Battle Field

	private static List<int> m_pre_load_sound_list = new List<int>();


	void Start()
	{
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)//过关斩将中有可能要屏蔽全服广播
		{
			int chapterId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
			
			PveFunctionOpen template = PveFunctionOpen.getPveFunctionOpenByIdWithoutRefresh(chapterId);
			
			if(template.broadcastable == false)
			{
				BroadCast.IsOpenBroadCast = false;

				HighestUI.Instance.m_BroadCast.Clear();
			}
		}
	}

	/// Reset Params.
	/// Load Effect.
	/// Change BGM.
	/// Load sfx.
	public void BattleStart()
	{
		//transform.localScale = new Vector3(1 / transform.parent.parent.localScale.x, 1 / transform.parent.parent.localScale.y, 1 / transform.parent.parent.localScale.z); 

		inDrama = false;

		lastCameraEffect = false;

		inCameraEffect_camera = false;

		inCameraEffect_update = false;

		completed = false;

		showHint = false;

		timeLast = 180;

		result = BattleResult.RESULT_BATTLING;

		totalBloodSelf = 0;

		totalBloodEnemey = 0;

		{
			foreach(BaseAI node in selfNodes) { Destroy(node.gameObject); }
			
			foreach(BaseAI node in enemyNodes) { Destroy(node.gameObject); }

			foreach(BaseAI node in reliveNodes) {Destroy(node.gameObject); }

			selfNodes.Clear();
			
			enemyNodes.Clear();

			reliveNodes.Clear();

			deadNodes.Clear();
		}

		{
			m_pre_load_sound_list.Clear();
		}

		guidePlayed.Clear ();

		//loadFlags();
		
		if(ClientMain.m_ClientMainObj != null) 
		{
			if(CityGlobalData.m_tempSection == 0 || CityGlobalData.m_tempLevel == 0)
			{
				PveTempTemplate pt = PveTempTemplate.GetPVETemplate(1, 1);
				
				if(pt != null) ClientMain.m_sound_manager.chagneBGSound (pt.soundId);
			}
			else
			{
				PveTempTemplate pt = PveTempTemplate.GetPVETemplate(CityGlobalData.m_tempSection, CityGlobalData.m_tempLevel);
				
				if(pt != null) ClientMain.m_sound_manager.chagneBGSound (pt.soundId);
			}
		}
	}

	public bool havePlayedGuide(GuideTemplate template)
	{
		if (template == null || template.levelType == 1 || CityGlobalData.m_debugPve == true || CityGlobalData.autoFightDebug == true) return true;

		foreach(GuideTemplate t in guidePlayed)
		{
			if(t.id == template.id && t.dungeonId == template.dungeonId && template.retriggerableCurBattle == false)
			{
				return true;
			}
		}

		if(template.retriggerable == false)
		{
			if(template.id <= CityGlobalData.m_pve_max_level)
			{
				return true;
			}
		}

		//玩家死亡之后，除了失败播放的剧情之外，其他都不触发
		if(result == BattleResult.RESULT_LOSE || king.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) <= 0)
		{
			if(template.triggerType != 3)
			{
				return true;
			}
		}

		return false;
	}

	public void PreLoadEffect(){
//		Debug.Log( "PreLoadEffect()" );

		preLoadEffCount = 0;

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 51 ).path, 
		                        ResourceLoadCallbackShadowTemple );

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 52 ).path, 
		                        ResourceLoadCallbackShadowTemple_2 );

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 53 ).path,
		                        ResourceLoadCallbackLabelTemple_base );
		
		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId( 56 ).path, 
		                        ResourceLoadCallbackLabelTemple_addHp );
		
		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (57).path, 
		                        ResourceLoadCallbackLabelTemple_enemy );
		
		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (58).path,
		                        ResourceLoadCallbackLabelTemple_mibao );
		
		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (59).path, 
		                        ResourceLoadCallbackLabelTemple_skill );

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (55).path, 
		                        ResourceLoadCallbackLabelTemple_default );

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (73).path, 
		                        ResourceLoadCallbackBloodBar_red );

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId (74).path, 
		                        ResourceLoadCallbackBloodBar_green );
	}

	public void ResourceLoadCallbackShadowTemple( ref WWW p_www, string p_path, Object p_object ){
//		Debug.Log( "Load shadowTemple Done: " + p_object );

		shadowTemple = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackShadowTemple_2( ref WWW p_www, string p_path, Object p_object ){
//		Debug.Log( "Load shadowTemple_2 Done: " + p_object );

		shadowTemple_2 = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_base( ref WWW p_www, string p_path, Object p_object ){

		labelTemple_base = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_addHp( ref WWW p_www, string p_path, Object p_object ){
		labelTemple_addHp = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_enemy( ref WWW p_www, string p_path, Object p_object ){
		labelTemple_enemy = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_mibao( ref WWW p_www, string p_path, Object p_object ){
		labelTemple_mibao = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_skill( ref WWW p_www, string p_path, Object p_object ){
		labelTemple_skill = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackLabelTemple_default( ref WWW p_www, string p_path, Object p_object ){
		labelTemple_default = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackBloodBar_red( ref WWW p_www, string p_path, Object p_object ){
		bloodbarTemple_red = (GameObject)p_object;
		
		preLoadEffCountPlus ();
	}

	public void ResourceLoadCallbackBloodBar_green( ref WWW p_www, string p_path, Object p_object ){
		bloodbarTemple_green = (GameObject)p_object;

		preLoadEffCountPlus ();
	}

	private void preLoadEffCountPlus(){
		LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections, 
		                         PrepareForBattleField.CONST_BATTLE_LOADING_FX, "preLoadEffCountPlus" );

		preLoadEffCount ++;

		if(preLoadEffCount >= 10)
		{
			BattleNet.Instance().initData();
		}
	}

	private void PreFindSoundFx( int p_model_id )
	{
		ModelTemplate t_model_data = ModelTemplate.getModelTemplateByModelId( p_model_id );

		foreach(int t_sound_id in t_model_data.sounds ){
			if( !m_pre_load_sound_list.Contains( t_sound_id ) ){
				m_pre_load_sound_list.Add( t_sound_id );
			}
		}
	}

	public void PreLoadSoundFx(){
		PreCountSoundFx();

		m_sound_loaded_count = 0;

		foreach(int t_sound_id in m_pre_load_sound_list ){
			ClientMain.m_sound_manager.getIdClip( t_sound_id,
			                                    ClientMain.m_sound_manager.getIdClipResLoad,
			                                     UtilityTool.GetEventDelegateList( LoadSoundCallback ) );
		}

		if( m_pre_load_sound_list.Count == 0 ){
			m_sound_loaded_count = -1;

			LoadSoundCallback();
		}

//		EnterNextScene.LogTimeSinceLoading( "PreLoadSoundFx.Done" );
	}

	private int m_sound_loaded_count = 0;

	public void LoadSoundCallback(){
		m_sound_loaded_count++;

		if( m_sound_loaded_count == m_pre_load_sound_list.Count ){
			PrepareForBattleField.Instance().BattleLoadDone();
		}
	}

	private void PreCountSoundFx(){
		LoadingSection t_loading = LoadingHelper.GetSection( StaticLoading.m_loading_sections,
		                                                                  PrepareForBattleField.CONST_BATTLE_LOADING_SOUND );
		
		if( t_loading != null ){
			t_loading.SetTotalCount( m_pre_load_sound_list.Count );
		}
		else{
			Debug.LogError( "Error In Finding." );
		}
	}

	#endregion

	public void loadBuffFlags()
	{
		BattleBuffFlag[] bfs = flagRoot.GetComponentsInChildren<BattleBuffFlag>();

		foreach(BattleBuffFlag buffFlag in bfs)
		{
			bool f = buffFlags.ContainsKey(buffFlag.flagId);

			if(f == false) 
			{
				buffFlags.Add(buffFlag.flagId, buffFlag);
			}
			else
			{
				Debug.LogError( "Battle Buff Flag Already Contained Key: " + buffFlag.flagId );
			}
		}
	}

	public void loadFlags()
	{
		Component[] t_battle_flags = flagRoot.GetComponentsInChildren( typeof( BattleFlag ) );

		foreach( Component fl in t_battle_flags ){
			BattleFlag bf = (BattleFlag) fl;

			bool f = flags.ContainsKey(bf.flagId);

			if(f == false){
				flags.Add( bf.flagId, bf );
			}
			else
			{
				Debug.LogError( "Battle Flag Already Contained Key: " + bf.flagId );
			}
		}
	}

	private void LogFlags()
	{
		foreach( KeyValuePair< int,BattleFlag > t_pair in flags )
		{
			Debug.Log( t_pair.Key + " - " + t_pair.Value.flagId );
		}
	}

	public BaseAI CreateNode( Node nodeData, BaseAI.Stance stance, int flag, GameObject nodeTemple, int copyIndex = 0 )
	{
//		Debug.Log("Create Node " + stance + ", " + flag + " with NodeData: "
//		           + " nodeType: " + nodeData.nodeType + ", nodeProfession: " + nodeData.nodeProfession
//		           + ", modleId: " + nodeData.modleId + ", skill.Count: " + (nodeData.skills == null ? "0" : (nodeData.skills.Count + ""))
//		           + ", nodeName: " + nodeData.nodeName + ", moveSpeed: " + nodeData.moveSpeed
//		           + ", attackSpeed: " + nodeData.attackSpeed + ", attackRange: " + nodeData.attackRange
//		           + ", eyeRange: " + nodeData.eyeRange + ", attackValue: " + nodeData.attackValue
//		           + ", defenceValue: " + nodeData.defenceValue 
//		           + ", hp: " + nodeData.hp + ", hpMax: " + nodeData.hpMax
//		           + ", attackAmplify: " + nodeData.attackAmplify + ", attackReduction: " + nodeData.attackReduction
//		           + ", attackAmplify_cri: " + nodeData.attackAmplify_cri + ", attackReduction_cri: " + nodeData.attackReduction_cri
//		           + ", skillAmplify: " + nodeData.skillAmplify + ", skillReduction: " + nodeData.skillReduction
//		           + ", skillAmplify_cri: " + nodeData.skillAmplify_cri + ", skillReduction_cri: " + nodeData.skillReduction_cri
//		           + ", criX: " + nodeData.criX + ", criY: " + nodeData.criY
//		           + ", criSkillX: " + nodeData.criSkillX + ", criSkillY: " + nodeData.criSkillY
//  		           + ", droppenItem: " + (nodeData.droppenItems == null ? 0 : nodeData.droppenItems.Count)
//		          );

		float t_cur_time = Time.realtimeSinceStartup;

		if(nodeData.nodeType == NodeType.PLAYER)
		{
//			if(nodeData.weaponHeavy != null)
//			{
//				Debug.Log("Player Weapon Heavy     flagId: " + flag 
//				           + ", weaponId: " + nodeData.weaponHeavy.weaponId
//				           + ", moveSpeed: " + nodeData.weaponHeavy.moveSpeed
//				           + ", attackSpeed: " + nodeData.weaponHeavy.attackSpeed
//				           + ", attackRange: " + nodeData.weaponHeavy.attackRange
//				           + ", weaponRatio_1: " + nodeData.weaponHeavy.weaponRatio[0]
//				           + ", weaponRatio_2: " + nodeData.weaponHeavy.weaponRatio[1]
//				           + ", weaponRatio_3: " + nodeData.weaponHeavy.weaponRatio[2]
//				           + ", weaponRatio_4: " + nodeData.weaponHeavy.weaponRatio[3]
//				           + ", criX: " + nodeData.weaponHeavy.criX
//				           + ", criY: " + nodeData.weaponHeavy.criY
//				           + ", criSkillX: " + nodeData.weaponHeavy.criSkillX
//				           + ", criSkillY: " + nodeData.weaponHeavy.criSkillY
//				          );
//			}
//
//			if(nodeData.weaponLight != null)
//			{
//				Debug.Log("Player Weapon Light     flagId: " + flag 
//				           + ", weaponId: " + nodeData.weaponLight.weaponId
//				           + ", moveSpeed: " + nodeData.weaponLight.moveSpeed
//				           + ", attackSpeed: " + nodeData.weaponLight.attackSpeed
//				           + ", attackRange: " + nodeData.weaponLight.attackRange
//				           + ", weaponRatio_1: " + nodeData.weaponLight.weaponRatio[0]
//				           + ", weaponRatio_2: " + nodeData.weaponLight.weaponRatio[1]
//				           + ", weaponRatio_3: " + nodeData.weaponLight.weaponRatio[2]
//				           + ", weaponRatio_4: " + nodeData.weaponLight.weaponRatio[3]
//				           + ", criX: " + nodeData.weaponLight.criX
//				           + ", criY: " + nodeData.weaponLight.criY
//				           + ", criSkillX: " + nodeData.weaponLight.criSkillX
//				           + ", criSkillY: " + nodeData.weaponLight.criSkillY
//				          );
//			}
//
//			if(nodeData.weaponRanged != null)
//			{
//				Debug.Log("Player Weapon Ranged     flagId: " + flag 
//				           + ", weaponId: " + nodeData.weaponRanged.weaponId
//				           + ", moveSpeed: " + nodeData.weaponRanged.moveSpeed
//				           + ", attackSpeed: " + nodeData.weaponRanged.attackSpeed
//				           + ", attackRange: " + nodeData.weaponRanged.attackRange
//				           + ", weaponRatio_1: " + nodeData.weaponRanged.weaponRatio[0]
//				           + ", weaponRatio_2: " + nodeData.weaponRanged.weaponRatio[1]
//				           + ", weaponRatio_3: " + nodeData.weaponRanged.weaponRatio[2]
//				           + ", weaponRatio_4: " + nodeData.weaponRanged.weaponRatio[3]
//				           + ", criX: " + nodeData.weaponRanged.criX
//				           + ", criY: " + nodeData.weaponRanged.criY
//				           + ", criSkillX: " + nodeData.weaponRanged.criSkillX
//				           + ", criSkillY: " + nodeData.weaponRanged.criSkillY
//				          );
//			}

			if(nodeData.weaponHeavy == null && nodeData.weaponLight == null && nodeData.weaponRanged == null)
			{
				Debug.LogError("THERE IS NO WEAPON !!!  THERE IS NO WEAPON !!!  THERE IS NO WEAPON !!!");

				showHint = true;
			}
		}

		int model = 0;

		if(nodeData.nodeType == NodeType.PLAYER)
		{
			model = nodeData.modleId + 1001;

//			if(CityGlobalData.getDramable() == false)
//			{
//				nodeData.weaponHeavy = nodeData.weaponLight;
//				
//				nodeData.weaponRanged = nodeData.weaponLight;
//				
//				nodeData.weaponRanged.attackRange = 15f;
//			}
		}
		else
		{
			model = nodeData.modleId;
		}

		t_cur_time = Time.realtimeSinceStartup;
		
		PreFindSoundFx( model );
		
		BattleFlag bf = null;

		BattleBuffFlag buffbf = null;

		flags.TryGetValue( flag, out bf) ;

		buffFlags.TryGetValue (flag, out buffbf);

		if( bf == null && buffbf == null)
		{
			Debug.LogError( "NO FLAG " + flag + " IN SCENE ! " + flags.Count + ", " + buffFlags.Count );
			
			return null;
		}
		
		GameObject parent = stance == BaseAI.Stance.STANCE_ENEMY ? enemyTeam : selfTeam;
		
		List<BaseAI> list = stance == BaseAI.Stance.STANCE_ENEMY ? enemyNodes : selfNodes;
		
		if( nodeTemple == null ){
			Debug.LogError( "Error nodeTemple: " + nodeTemple );
		}

		Vector3 startPosition = bf != null ? bf.transform.position : buffbf.transform.position;

		GameObject nodeObjcet = ( GameObject )Instantiate( 
	          nodeTemple, 
		      startPosition, 
	          nodeTemple.transform.rotation );
		
		nodeObjcet.SetActive( true );
		
		nodeObjcet.transform.parent = parent.transform;
		
		nodeObjcet.transform.localScale = new Vector3( 1, 1, 1 );

		BaseAI t_ai = (BaseAI)nodeObjcet.GetComponent( "BaseAI" );

		if(t_ai == null) t_ai = (BaseAI)nodeObjcet.AddComponent<BaseAI>( );

		if(nodeData.nodeType == NodeType.NPC)
		{
			NavMeshAgent t_nav = nodeObjcet.GetComponent<NavMeshAgent>();

			if(t_nav == null) t_nav = nodeObjcet.AddComponent<NavMeshAgent>();

			t_nav.radius = 0;

			t_nav.enabled = false;

			CharacterController cc = nodeObjcet.GetComponent<CharacterController>();

			if(cc == null) cc = nodeObjcet.AddComponent<CharacterController>();

			cc.height = 0;

			cc.enabled = false;

			CapsuleCollider ccc = nodeObjcet.GetComponent<CapsuleCollider>();

			if(ccc != null) ccc.radius = 0;

			UISprite t_sprite = nodeObjcet.GetComponentInChildren<UISprite>();

			if(t_sprite != null) Destroy(t_sprite.gameObject);

			UILabel t_label = nodeObjcet.GetComponentInChildren<UILabel>();

			if(t_label != null) Destroy(t_label.gameObject);
		}

		t_ai.flag = bf;

		t_ai.buffFlag = buffbf;

		if(t_ai.flag != null) t_ai.flag.node = t_ai;
	
		if(copyIndex == 0)
		{
			t_ai.nodeId = flag;
		}
		else
		{
			t_ai.nodeId = -1 * flag - 10000 * copyIndex;
		}
		//		Debug.Log( "Assign AI.Shadow Temple. t_ai: " + t_ai );
		
		//		Debug.Log( "Assign AI.Shadow Temple shadowTemple: " + shadowTemple );
		
		t_ai.shadowTemple = shadowTemple;
		
		t_ai.shadowTemple_2 = shadowTemple_2;
		
		t_ai.stance = stance;
		
		t_ai.bloodTemple = stance == BaseAI.Stance.STANCE_SELF ? bloodbarTemple_green : bloodbarTemple_red;

		t_ai.initDate( nodeData, nodeTemple, model );

		if (t_ai.buffFlag != null)
						t_ai.buffFlag.setNode (t_ai);

		if(nodeData.nodeType == NodeType.BOSS)
		{
			if(t_ai.body != null)
			{
				float sc = 1.5f;

				t_ai.body.transform.localScale = new Vector3(sc, sc, sc);
			}
		}


		if(bf != null)
		{
			if( bf.triggerFunc == BattleFlag.TriggerFunc.FadeIn )
			{
				t_ai.gameObject.SetActive(false);
			}
			else if(bf.triggerFunc == BattleFlag.TriggerFunc.NodeSkillOff)
			{
				foreach(int skillId in bf.nodeSkillAble)
				{
					HeroSkill skill = t_ai.getSkillById(skillId);

					if(skill != null) skill.template.zhudong = false;
				}
			}
			else if(bf.triggerFunc == BattleFlag.TriggerFunc.NodeSkillOn)
			{
				foreach(int skillId in bf.nodeSkillAble)
				{
					HeroSkill skill = t_ai.getSkillById(skillId);

					if(skill != null) skill.template.zhudong = true;
				}
			}
			
			if(bf.triggerFunc == BattleFlag.TriggerFunc.Hurtable)
			{
				t_ai.hurtable = false;
			}

			if(bf.hideWithDramable == true)
			{
				int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

				if(level <= CityGlobalData.m_pve_max_level)
				{
					t_ai.gameObject.SetActive(false);
				}
			}
		}

		list.Add( t_ai );
		
		ModelTemplate modelTemplate = ModelTemplate.getModelTemplateByModelId (model);
		
		t_ai.setNavMeshRadius( modelTemplate.radius );
		
		int outValue = 0;

		bool f = hurts.TryGetValue (t_ai.nodeId, out outValue);
		
		if(f == false && stance == BaseAI.Stance.STANCE_SELF)
		{
			hurts.Add( t_ai.nodeId, 0 );
		}
		
		if( t_ai.nodeData.nodeType == NodeType.PLAYER && stance == BaseAI.Stance.STANCE_SELF )
		{
			//LogFlags();
			
			king = (KingControllor)t_ai;
			
			BattleUIControlor.Instance().labelName.text = t_ai.nodeData.nodeName;

			BattleUIControlor.Instance ().spriteAvatar.spriteName = "PlayerIcon" + nodeData.modleId;
		}
		
		{
			TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_CREATE_NODE, 
			                           Time.realtimeSinceStartup - t_cur_time );
		}

		return t_ai;
	}

	public void exitBattle( int i )
	{
		GameObject root3d = GameObject.Find ("BattleField_V4_3D");
		
		GameObject root2d = GameObject.Find ("BattleField_V4_2D");
		
		Destroy (root3d);
		
		Destroy (root2d);

        //		CityGlobalData.m_nextSceneName = ConstInGame.CONST_SCENE_NAME_MAIN_CITY;
        //		
        //		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );

        //	SceneManager.EnterMainCity();


        //if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        //{
           JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;//new add
            SceneManager.EnterMainCity();
        //}
        //else 
        //{
        //    SceneManager.EnterAllianceCity();
        //}

		if (!string.IsNullOrEmpty(PlayerPrefs.GetString ("JunZhu"))) 
		{
			CityGlobalData.m_JunZhuCreate = true;	
		}
	}

	public void createWall(BattleFlag bf)
	{
		int navCount = (int)(bf.transform.localScale.x / 1f);

		for(int i = 0; i < navCount; i++)
		{
			GameObject gc = new GameObject();

			gc.name = "NavObs";

			gc.transform.parent = bf.transform;

			gc.transform.localScale = new Vector3(1, 1, 1);

			gc.transform.localEulerAngles = Vector3.zero;

			gc.transform.localPosition = new Vector3(1f * (i - navCount / 2), -1.5f, 0);

			NavMeshObstacle navObs = gc.AddComponent<NavMeshObstacle>();

			navObs.radius = 1f;

			navObs.height = 4;

			navObs.carving = false;

			CapsuleCollider capCol = gc.AddComponent<CapsuleCollider>();

			capCol.radius = 1f;

			capCol.height = 4;

			capCol.center = new Vector3(0, 2, 0);
		}

		if(bf.triggerCount < 0)
		{
			createWallForever(bf);
		}
		else 
		{
			createWallNormal(bf);
		}
	}

	private void createWallForever(BattleFlag bf)
	{
		float width = 4f;
		
		int count = (int)(bf.transform.localScale.x / width);
		
		count = count < 1 ? 1 : count;
		
		GameObject GQTemple = GuangQiang_Forever;
		
		float ii = bf.triggerCount < 0 ? 2f : 4f;
		
		for(int i = 0; i < count; i++)
		{
			float x = - bf.transform.localScale.x / 2 + width * (i + .5f);
			
			GameObject guangObject = (GameObject)Instantiate (GQTemple);
			
			guangObject.SetActive (true);
			
			guangObject.transform.parent = bf.transform;
			
			guangObject.transform.localScale = new Vector3(1, 1, 1);
			
			guangObject.transform.position = bf.transform.position + new Vector3(0, -2 /*bf.transform.localScale.y / ii */, 0);
			
			if(count != 1) guangObject.transform.localPosition += new Vector3(x, 0, 0);
			
			guangObject.transform.rotation = bf.transform.rotation;
		}
		
		BoxCollider box = bf.gameObject.AddComponent<BoxCollider>();
		
		box.size = new Vector3 (bf.transform.localScale.x, bf.transform.localScale.y, bf.transform.localScale.z);
		
		bf.transform.localScale = new Vector3 (1, 1, 1);
	}

	private void createWallNormal(BattleFlag bf)
	{
		float totalWidth = bf.transform.localScale.x;

		List<int> countList = new List<int> ();

		for(int i = 0; i < 5; i++)
		{
			countList.Add(0);
		}

		for(int i = 0; totalWidth > 2; i++)
		{
			if(totalWidth > 50)
			{
				countList[4] ++;

				totalWidth -= 50;
			}
			else if(totalWidth > 20)
			{
				countList[3] ++;
				
				totalWidth -= 20;
			}
			else if(totalWidth > 10)
			{
				countList[2] ++;
				
				totalWidth -= 10;
			}
			else if(totalWidth > 5)
			{
				countList[1] ++;
				
				totalWidth -= 5;
			}
			else
			{
				countList[0] ++;
				
				totalWidth -= 2;
			}
		}

		float startX = - bf.transform.localScale.x / 2;

		for(int i = 0; i < countList.Count; i++)
		{
			float width = 0;

			if(i == 0) width = 2f;

			else if(i == 1) width = 5f;

			else if(i == 2) width = 10f;

			else if(i == 3) width = 20f;

			else if(i == 4) width = 50f;

			int count = countList[i];

			if(count == 0) continue;

			GameObject GQTemple = GuangQiangList[i];

			for(int j = 0; j < count; j++)
			{
				float x = startX + width * (j + .5f);
				
				GameObject guangObject = (GameObject)Instantiate (GQTemple);
				
				guangObject.SetActive (true);
				
				guangObject.transform.parent = bf.transform;
				
				guangObject.transform.localScale = new Vector3(1, 1, 1);
				
				guangObject.transform.position = bf.transform.position + new Vector3(0, -2, 0);
				
				guangObject.transform.localPosition += new Vector3(x, 0, 0);
				
				guangObject.transform.rotation = bf.transform.rotation;
			}

			startX += width * count;
		}

		BoxCollider box = bf.gameObject.AddComponent<BoxCollider>();
		
		box.size = new Vector3 (bf.transform.localScale.x, bf.transform.localScale.y, bf.transform.localScale.z);
		
		bf.transform.localScale = new Vector3 (1, 1, 1);

		if(bf.triggerFunc == BattleFlag.TriggerFunc.FadeIn)
		{
			bf.gameObject.SetActive(false);
		}
	}

	public void nodeCreateComplete()
	{
		EnterBattleFieldNet.sending = false;

		if(showHint == true)
		{
			StartCoroutine(showHintBox());

			return;
		}

		battleCheck = new BattleCheckResult ();

		UIYindao.m_UIYindao.CloseUI ();

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			int chapterId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
			
			PveFunctionOpen template = PveFunctionOpen.getPveFunctionOpenByIdWithoutRefresh(chapterId);
			
			if(template.broadcastable == false)
			{
				BroadCast.IsOpenBroadCast = false;

				HighestUI.Instance.m_BroadCast.Clear();
			}
		}

		BattleUIControlor.Instance().checkDrama (true);

		king.initWeaponData ();

		foreach(BaseAI node in selfNodes)
		{
			totalBloodSelf += node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );

			node.transform.forward = enemyNodes[0].transform.position - node.transform.position;
		}

		foreach(BaseAI node in enemyNodes)
		{
			KingControllor _k = node as KingControllor;

			if(_k != null)
			{
				_k.initWeaponDataEnemy();
			}

			totalBloodEnemey += node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );

			if(node.flag.forwardFlagId == 1)
			{
				node.transform.forward = selfNodes[0].transform.position - node.transform.position;
			}
			else
			{
				Vector3 forward = flags[node.flag.forwardFlagId].transform.position - node.transform.position;

				node.transform.forward = new Vector3(forward.x, node.transform.forward.y, forward.z);
			}
		}

		foreach(BattleFlag flag in flags.Values)
		{
			if(flag.node != null) continue;

			if(flag.flagId > 999) continue;

			foreach(BattleFlag f in flag.triggerFlagKill)
			{
				if(f == null) continue;

				f.trigger();
			}
		}

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)//荒野pve中玩家的补充站位
		{
			int firstFlag = 101;

			int firstWave = 1;
			
			foreach(BattleFlag flag in flags.Values)
			{
				if(flag.flagId > 100 && flag.flagId < 999 && flag.node != null)
				{
					firstFlag = flag.flagId;
					
					break;
				}
			}

			firstWave = (firstFlag - 1 - 100) / 100;//0:101-199,1:201-299

			if(firstWave != 0)
			{
				BattleFlag bf;//0:null, 1:51, 2,52

				for(int i = firstWave; i > 0; i--)
				{
					flags.TryGetValue(i + 50, out bf);

					if(bf != null) 
					{
						king.setNavEnabled(false);

						king.transform.position = bf.transform.position;

						king.setNavEnabled(true);

						break;
					}
				}
			}
		}

		BloodLabelControllor.Instance ().initStart ();

		SignShadow.create ();

		SceneGuideManager.Instance ().OnSceneTipsHide ();

		completed = true;

		if(CityGlobalData.m_debugPve == true)
		{
			string key = "TestOver_" + CityGlobalData.m_tempSection + "_" + CityGlobalData.m_tempLevel + "_" + CityGlobalData.m_levelType;

			TimeHelper.Instance.AddOneDelegateToTimeCalc(key, 5f, closeToTest);

			//closeToTest();
		}
		else if(CityGlobalData.autoFightDebug == true) 
		{
			if(autoFight == false) BattleUIControlor.Instance().changeAutoFight();
		}
		else
		{
			bool fd = CityGlobalData.getDramable () && CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan;

			if(fd == true)
			{
				checkStartDrama ();
			}
			else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
			{
				showLockEff();

				showSceneName ();
			}
			else
			{
				showSceneName ();
			}
		}
	}

	private void showLockEff()
	{
		//StartCoroutine (showLockEffAction());

		showSceneName ();

		PveFunctionOpen template = PveFunctionOpen.getPveFunctionOpenById (CityGlobalData.m_configId);

		if(template.joystick_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockAttack, BattleUIControlor.Instance().attackJoystick.gameObject));
		}
		else if(template.joystick_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockAttack, BattleUIControlor.Instance().attackJoystick.gameObject));
		}

		if(template.attack_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockAttack, BattleUIControlor.Instance().attackJoystick.gameObject));
		}
		else if(template.attack_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockAttack, BattleUIControlor.Instance().attackJoystick.gameObject));
		}

		if(template.skill_light_1_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockLightSkill_1, BattleUIControlor.Instance().m_gc_skill_1[2].gameObject));
		}
		else if(template.skill_light_1_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockLightSkill_1, BattleUIControlor.Instance().m_gc_skill_1[2].gameObject));
		}

		if(template.skill_light_2_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockLightSkill_2, BattleUIControlor.Instance().m_gc_skill_2[2].gameObject));
		}
		else if(template.skill_light_2_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockLightSkill_2, BattleUIControlor.Instance().m_gc_skill_2[2].gameObject));
		}

		if(template.skill_heavy_1_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockHeavySkill_1, BattleUIControlor.Instance().m_gc_skill_1[0].gameObject));
		}
		else if(template.skill_heavy_1_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockHeavySkill_1, BattleUIControlor.Instance().m_gc_skill_1[0].gameObject));
		}

		if(template.skill_heavy_2_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockHeavySkill_2, BattleUIControlor.Instance().m_gc_skill_2[0].gameObject));
		}
		else if(template.skill_heavy_2_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockHeavySkill_2, BattleUIControlor.Instance().m_gc_skill_2[0].gameObject));
		}

		if(template.skill_ranged_1_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockRangeSkill_1, BattleUIControlor.Instance().m_gc_skill_1[1].gameObject));
		}
		else if(template.skill_ranged_1_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockRangeSkill_1, BattleUIControlor.Instance().m_gc_skill_1[1].gameObject));
		}

		if(template.skill_ranged_2_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockRangeSkill_2, BattleUIControlor.Instance().m_gc_skill_2[1].gameObject));
		}
		else if(template.skill_ranged_2_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockRangeSkill_2, BattleUIControlor.Instance().m_gc_skill_2[1].gameObject));
		}

		if(template.weaponHeavy_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockWeaponHeavy, BattleUIControlor.Instance().m_changeWeapon.btnHeavy));
		}
		else if(template.weaponHeavy_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockWeaponHeavy, BattleUIControlor.Instance().m_changeWeapon.btnHeavy));
		}

		if(template.weaponLight_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockWeaponLight, BattleUIControlor.Instance().m_changeWeapon.btnLight));
		}
		else if(template.weaponLight_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockWeaponLight, BattleUIControlor.Instance().m_changeWeapon.btnLight));
		}

		if(template.weaponRange_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockWeaponRange, BattleUIControlor.Instance().m_changeWeapon.btnRange));
		}
		else if(template.weaponRange_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockWeaponRange, BattleUIControlor.Instance().m_changeWeapon.btnRange));
		}

		if(template.skill_miBao_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockMiBaoSkill, BattleUIControlor.Instance().btnMibaoSkillIcon.gameObject));
		}
		else if(template.skill_miBao_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockMiBaoSkill, BattleUIControlor.Instance().btnMibaoSkillIcon.gameObject));
		}

		if(template.autoFight_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockAutoFight, BattleUIControlor.Instance().m_gc_autoFight));
		}
		else if(template.autoFight_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockAutoFight, BattleUIControlor.Instance().m_gc_autoFight));
		}

		if(template.pause_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockPause, BattleUIControlor.Instance().m_gc_pause));
		}
		else if(template.pause_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockPause, BattleUIControlor.Instance().m_gc_pause));
		}

		if(template.dodge_eff == -1)
		{
			StartCoroutine(playLockEff(LockControllor.Instance().lockDodge, BattleUIControlor.Instance().m_gc_dodge));
		}
		else if(template.dodge_eff == 1)
		{
			StartCoroutine(playUnlockEff(LockControllor.Instance().lockDodge, BattleUIControlor.Instance().m_gc_dodge));
		}
	}

	private IEnumerator playLockEff(GameObject lockObject, GameObject effectRoot)
	{
		yield return new WaitForSeconds (.2f);

		LockControllor.Instance().lightOff(lockObject, false);
		
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, 
		                                             effectRoot, 
		                                             EffectIdTemplate.GetPathByeffectId(100169) );
		yield return new WaitForSeconds (.3f);

		LockControllor.Instance ().lightOn (lockObject, false);
	}

	private IEnumerator playUnlockEff(GameObject lockObject, GameObject effectRoot)
	{
		yield return new WaitForSeconds (.2f);

		LockControllor.Instance ().lightOn (lockObject, true);

		UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, effectRoot, EffectIdTemplate.GetPathByeffectId(100170));
		
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, effectRoot, EffectIdTemplate.GetPathByeffectId(100009));

		yield return new WaitForSeconds (.3f);

		LockControllor.Instance ().lightOff (lockObject, true);
	}

	private void ChatWindowLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		var chatUI = Instantiate(p_object) as GameObject;

		chatUI.SetActive(false);
	}

	public void closeToTest()
	{
		Debug.Log ("CHECK PVE OVER: " + CityGlobalData.m_tempSection + "-" + CityGlobalData.m_tempLevel + ", " + CityGlobalData.m_levelType + " !");

		EnterBattleField.setCheckingOver ();
		
		AudioListener al = king.gameCamera.target.GetComponent<AudioListener>();
		
		Destroy ( al );
		
		ClientMain.m_ClientMainObj.AddComponent<AudioListener> ();

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
		}
		else //if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			SceneManager.EnterMainCity();
		}
		//else
		//{
		//	SceneManager.EnterAllianceCity();
		//}
		
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
		{
			CityGlobalData.m_JunZhuCreate = true;
		}

		//EnterBattleField.EnterBattlePveDebug();
	}

	private void showSceneName()
	{
		if(BattleUIControlor.Instance().b_autoFight == true && autoFight == false)
		{
			BattleUIControlor.Instance().changeAutoFight();
		}

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			PveTempTemplate pve = PveTempTemplate.GetPVETemplate (CityGlobalData.m_tempSection, CityGlobalData.m_tempLevel);

			string pveName = NameIdTemplate.GetName_By_NameId(pve.smaName);

			string pn = pveName.Split(' ')[1];

			SceneGuideManager.Instance ().ShowSceneGuide (1070001, pn);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan)
		{
			SceneGuideManager.Instance ().ShowSceneGuide (1050001);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			HuangYePveTemplate template = HuangYePveTemplate.getHuangYePveTemplatee_byid(CityGlobalData.battleTemplateId);

			SceneGuideManager.Instance ().ShowSceneGuide (1060001, NameIdTemplate.GetName_By_NameId(template.nameId));
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp)
		{
			HuangyeTemplate template = HuangyeTemplate.getHuangyeTemplate_byid(CityGlobalData.battleTemplateId);

			SceneGuideManager.Instance ().ShowSceneGuide (1060001, NameIdTemplate.GetName_By_NameId(template.nameId));
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
			SceneGuideManager.Instance ().ShowSceneGuide (1090001);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			if(CityGlobalData.m_tempSection == 1)
			{
				SceneGuideManager.Instance ().ShowSceneGuide (1100001);
			}
			else if(CityGlobalData.m_tempSection == 2)
			{
				SceneGuideManager.Instance ().ShowSceneGuide (1110001);
			}
			else
			{
				SceneGuideManager.Instance ().ShowSceneGuide (1120001);
			}
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
		{

		}
	}

	private void checkStartDrama()
	{
		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		bool f = GuideTemplate.HaveId_type (level, 1);

		if (f == false) 
		{
			showLockEff();

			return;
		}

		GuideTemplate template = GuideTemplate.getTemplateByLevelAndType (level, 1);

		BattleUIControlor.Instance ().showDaramControllor (level, template.id, showLockEff);
	}

	IEnumerator showHintBox()
	{
		yield return new WaitForSeconds (.5f);

		Global.CreateBox( "Battle Init Error",
		                 "Wear At Least One EQUIP",
		                 "",
		                 null,
		                 "Go And Wear Something",
		                 null,
		                 exitBattle );
	}

	IEnumerator battleTimeClock()
	{
		int totalTime = timeLast;

		if(totalTime == -100)
		{
			BattleUIControlor.Instance().labelTime.gameObject.SetActive(false);
		}

		double lastTime;

		CanshuTemplate.m_TaskInfoDic.TryGetValue (CanshuTemplate.LASTTIME_PVE, out lastTime);

		float tempRealTime = Time.realtimeSinceStartup;

		for(;result == BattleResult.RESULT_BATTLING && totalTime != -100;)
		{
			yield return new WaitForEndOfFrame();

//			if(completed)
//			{
//				yield return new WaitForSeconds(1.0f);
//			}
//			else
//			{
//				yield return new WaitForEndOfFrame();
//			}

			if(timeLast > 0)
			{
				float curRealTime = Time.realtimeSinceStartup;

				if(Time.timeScale > .1f && inDrama == false && BattleUIControlor.Instance().pauseControllor.gameObject.activeSelf == false)
				{
					battleTime += curRealTime - tempRealTime;
				}

				tempRealTime = curRealTime;
				
				timeLast = totalTime - (int)battleTime;

				int miao = timeLast % 60;

				int fen = timeLast / 60;

				string strFen = fen < 10 ? ("0" + fen) : (fen + "");

				string strMiao = miao < 10 ? ("0" + miao) : (miao + "");

				BattleUIControlor.Instance().labelTime.text = strFen + ":" + strMiao;

				if(timeLast <= lastTime && miao % 2 == 0)
				{
					BattleUIControlor.Instance().labelTime.color = Color.red;

//					BattleUIControlor.Instance().labelTimeLast.transform.localScale = new Vector3(1, 1, 1);
//
//					BattleUIControlor.Instance().labelTimeLast.gameObject.SetActive(true);
//
//					BattleUIControlor.Instance().labelTimeLast.text = "" + miao;
//
//					iTween.ScaleTo(BattleUIControlor.Instance().labelTimeLast.gameObject, iTween.Hash(
//						"scale", new Vector3(1, 0, 1),
//						"delay", 0.6f,
//						"time", 0.3f,
//						"easetype", iTween.EaseType.easeOutExpo
//						));
				}
				else
				{
					BattleUIControlor.Instance().labelTime.color = Color.yellow;
				}
			}
		}
	}

	public void setMiBaoId(List<int> _mibaoIds)
	{
		mibaoIds.Clear();

		for(int i = 0; _mibaoIds != null && i < _mibaoIds.Count; i++)
		{
			mibaoIds.Add(_mibaoIds[i]);
		}
	}

	public void setMiBaoSkill(HeroSkill skillMiBao, BaseAI node)
	{
		BattleUIControlor.Instance ().setMiBaoSkillIcon (skillMiBao, node.nodeId != 1);

		if (skillMiBao == null) return;

		KingControllor _king = node as KingControllor;

		_king.kingSkillMibao = skillMiBao;
	}

	public void setHeavySkill_2(HeroSkill skillHeavy_2, BaseAI node)
	{
		KingControllor _king = node as KingControllor;
		
		_king.kingSkillHeavy_2 = skillHeavy_2;
	}

	public void setReactiveSkill(List<NodeSkill> skillList)
	{
		king.setSkill(skillList);
	}

	public void setStarTemp(List<int> startTemp)
	{
		if (startTemp == null) return;
	}

	public void setTimeLimit(int timeLimit)
	{
		timeLast = timeLimit;

		BattleUIControlor.Instance ().labelLeave.gameObject.SetActive (false);

		StartCoroutine(battleTimeClock());
	}

	public void setAchivmentId(List<int> list)
	{
		if (list == null) return;

		if (list [0] == 0 || list [1] == 0) return;

		achivement = new JianDunDataManager (list);
	}

	public int getLabelType(BaseAI defender, AttackType _type)
	{
		if(_type == AttackType.ADD_HP)
		{
			return (int)AttackType.ADD_HP;
		}
		else if(_type == AttackType.DEFAULT)
		{
			return (int)AttackType.DEFAULT;
		}
		
		if(defender.stance == BaseAI.Stance.STANCE_SELF)
		{
			return 5;
		}
		
		if(_type == AttackType.BASE_ATTACK)
		{
			return (int)AttackType.BASE_ATTACK;
		}
		else if(_type == AttackType.MIBAO_ATTACK)
		{
			return (int)AttackType.MIBAO_ATTACK;
		}
		else if(_type == AttackType.SKILL_ATTACK)
		{
			return (int)AttackType.SKILL_ATTACK;
		}
		
		return (int)AttackType.BASE_ATTACK;
	}

	public GameObject getLabelTemplate(BaseAI defender, AttackType _type)
	{
		if(_type == AttackType.ADD_HP)
		{
			return labelTemple_addHp;
		}
		else if(_type == AttackType.DEFAULT)
		{
			return labelTemple_default;
		}

		if(defender.stance == BaseAI.Stance.STANCE_SELF)
		{
			return labelTemple_enemy;
		}

		if(_type == AttackType.BASE_ATTACK)
		{
			return labelTemple_base;
		}
		else if(_type == AttackType.MIBAO_ATTACK)
		{
			return labelTemple_mibao;
		}
		else if(_type == AttackType.SKILL_ATTACK)
		{
			return labelTemple_skill;
		}

		return labelTemple_base;
	}

	public GameObject getLabelTemplate(int labelType)
	{
		if(labelType == (int)AttackType.ADD_HP)
		{
			return labelTemple_addHp;
		}
		else if(labelType == (int)AttackType.DEFAULT)
		{
			return labelTemple_default;
		}
		
		if(labelType == 5)
		{
			return labelTemple_enemy;
		}
		
		if(labelType == (int)AttackType.BASE_ATTACK)
		{
			return labelTemple_base;
		}
		else if(labelType == (int)AttackType.MIBAO_ATTACK)
		{
			return labelTemple_mibao;
		}
		else if(labelType == (int)AttackType.SKILL_ATTACK)
		{
			return labelTemple_skill;
		}
		
		return labelTemple_base;
	}

	void FixedUpdate ()
	{
		BattleUIControlor.Instance ().addDebugText ("completed " + completed + ", result " + result);

		if(completed == false) return;

		if(result != BattleResult.RESULT_BATTLING) return;

		checkResult();

		if(result != BattleResult.RESULT_BATTLING)
		{
			foreach(BaseAI ba in selfNodes)
			{
				ba.setNavMeshStop();
			}

			foreach(BaseAI ba in enemyNodes)
			{
				ba.setNavMeshStop();
			}

			int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

			bool fWin = GuideTemplate.HaveId_type(level, 2);

			bool fLose = GuideTemplate.HaveId_type(level, 3);

			bool f = CityGlobalData.getDramable() && CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan;

			if(fWin == true && result == BattleResult.RESULT_WIN && f == true)
			{
				GuideTemplate gt_win = GuideTemplate.getTemplateByLevelAndType(level, 2);
				
				StartCoroutine(showDramaControllor(level, gt_win.dungeonId, showResult));
			}
			else if(fLose == true && result == BattleResult.RESULT_LOSE && f == true)
			{
				GuideTemplate gt_lose = GuideTemplate.getTemplateByLevelAndType(level, 3);
				
				StartCoroutine(showDramaControllor(level, gt_lose.dungeonId, showResult));
			}
			else
			{
				showResult();
			}
		}
	}

	private IEnumerator showDramaControllor(int level, int eventid, DramaControllor.Callback callback)
	{
		yield return new WaitForSeconds (2f);

		BattleUIControlor.Instance().showDaramControllor(level, eventid, callback);
	}

	public void showCreateRole()
	{
		StartCoroutine(showCreateRoleAction());
	}

	IEnumerator showCreateRoleAction()
	{
		BattleUIControlor.Instance().textureBlack.gameObject.SetActive(true);

		TweenAlpha.Begin (BattleUIControlor.Instance().textureBlack.gameObject, 0, 0f);

		TweenAlpha.Begin (BattleUIControlor.Instance().textureBlack.gameObject, 0.5f, 1f);

		yield return new WaitForSeconds (1f);

		GameObject mainCam = Camera.main.gameObject;

		Component[] cams = mainCam.GetComponents (typeof(Camera));

		foreach(Camera cam in cams)
		{
			cam.enabled = false;
		}

		GameObject createRole3D = GameObject.Find ("CreateRole_camra_3D");

		BattleActive ba3D = (BattleActive)createRole3D.GetComponent ("BattleActive");

		ba3D.setActive (true);

		GameObject createRole2D = GameObject.Find ("CreateRole_camra_2D");
		
		BattleActive ba2D = (BattleActive)createRole2D.GetComponent ("BattleActive");
		
		ba2D.setActive (true);

		yield return new WaitForSeconds (1f);

		TweenAlpha.Begin (BattleUIControlor.Instance().textureBlack.gameObject, 0.5f, 0);
	}

	public void showResult()
	{
		Debug.Log ("showResult showResult");

		BattleUIControlor.Instance ().addDebugText ("showResult()");

		UtilityTool.Instance.DelayedUnloadUnusedAssets();

		ClientMain.m_sound_manager.RemoveAllSound();

		BattleEffectControllor.Instance ().removeAllEffect ();

		UIYindao.m_UIYindao.CloseUI ();

		GameObject btnWin = GameObject.Find ("BtnWin");

		if(btnWin != null) btnWin.SetActive (false);

		GameObject btnLose = GameObject.Find ("BtnLose");

		if(btnLose != null) btnLose.SetActive (false);

//		if(result == BattleResult.RESULT_WIN)
//		{
//			foreach(BaseAI node in enemyNodes)
//			{
//				node.setLayer(0);
//			}
//		}

		StartCoroutine (showResultAction());

		//BattleReplayorWrite.Instance().sendReplay();
	}

	IEnumerator showResultAction()
	{
		Debug.Log ("showResultAction showResultAction");

		UtilityTool.UnloadUnusedAssets ();

		BattleUIControlor.Instance ().LoadResultRes ();

		double waitTime = 2f;

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
		{
			if(result == BattleResult.RESULT_WIN)
			{
				CanshuTemplate.m_TaskInfoDic.TryGetValue(CanshuTemplate.ENDTIME_BAIZHAN_WIN, out waitTime);
			}
			else
			{
				CanshuTemplate.m_TaskInfoDic.TryGetValue(CanshuTemplate.ENDTIME_BAIZHAN_LOSE, out waitTime);
			}
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			CanshuTemplate.m_TaskInfoDic.TryGetValue(CanshuTemplate.ENDTIME_YOUXIA, out waitTime);
		}
		else
		{
			if(result == BattleResult.RESULT_WIN)
			{
				CanshuTemplate.m_TaskInfoDic.TryGetValue(CanshuTemplate.ENDTIME_PVE_WIN, out waitTime);
			}
			else
			{
				CanshuTemplate.m_TaskInfoDic.TryGetValue(CanshuTemplate.ENDTIME_PVE_LOSE, out waitTime);
			}
		}

		if(waitTime > 1)
		{
			BattleUIControlor.Instance ().labelTime.gameObject.SetActive (false);

			BattleUIControlor.Instance ().labelLeave.gameObject.SetActive (true);
		}

		//BattleUIControlor.Instance ().layerFight.SetActive (false);

		BattleUIControlor.Instance ().labelLeave.text = (int)waitTime + LanguageTemplate.GetText((LanguageTemplate.Text)425);

		for(;;)
		{
			yield return new WaitForSeconds (1f);

			waitTime --;

			BattleUIControlor.Instance ().labelLeave.text = (int)waitTime + LanguageTemplate.GetText((LanguageTemplate.Text)425);

			if(waitTime <= 0) break;
		}

		for(;;)
		{
			if(BattleUIControlor.Instance().resultControllor != null) break;

			yield return new WaitForEndOfFrame();
		}

		BattleUIControlor.Instance().showResult(result);

		foreach(BaseAI node in selfNodes)
		{
			node.isAlive = false;
		}

		foreach(BaseAI node in enemyNodes)
		{
			node.isAlive = false;
		}
	}

	private void checkResult()
	{
		if(lastCameraEffect == true) return;

		result = battleCheck.checkResult (null);

//		BattleResult br = battleCheck.checkResult (null);
//
//		bool fWin = br == BattleResult.RESULT_WIN;
//
//		bool fLose = br == BattleResult.RESULT_LOSE;
//
//		if(fWin == true) result = BattleResult.RESULT_WIN;
//
//		if(king.isAlive == false) 
//		{
//			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve
//			   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
//			{
//				result = BattleResult.RESULT_WIN;
//			}
//			else
//			{
//				result = BattleResult.RESULT_LOSE;
//			}
//		}
//
//		if(timeLast <= 0 && timeLast != -100)
//		{
//			BattleWinTemplate template = BattleWinTemplate.getWinTemplateContainsType(BattleWinFlag.EndType.Reach_Time);
//
//			bool winFlag = false;
//
//			if(template != null)
//			{
//				winFlag = BattleWinTemplate.reachTypeWin(template.winId);
//			}
//
//			if(winFlag == true
//			   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve
//			   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
//			{
//				result = BattleResult.RESULT_WIN;
//			}
//			else
//			{
//				result = BattleResult.RESULT_LOSE;
//			}
//		}
//
//		if(result != BattleResult.RESULT_BATTLING)
//		{
//			Debug.Log("checkResult checkResult " + result);
//		}
	}
 
	public void ResultSlowDown()
	{
		StartCoroutine (slowDownClock());
	}

	IEnumerator slowDownClock()
	{
		float scale = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.BATTLE_SLOWDOWN_VALUE);

		float keepTime = (float)CanshuTemplate.GetValueByKey (CanshuTemplate.BATTLE_SLOWDOWN_TIME);

		float startTime = Time.realtimeSinceStartup;

		Time.timeScale = scale;

		for(;;)
		{
			yield return new WaitForEndOfFrame();

			float curTime = Time.realtimeSinceStartup;

			if(curTime - startTime > keepTime)
			{
				break;
			}
		}

		Time.timeScale = 1f;
	}

	private float getBaseAttackValue(BaseAI attacker, BaseAI defender)
	{
		/*
		鍩虹?浼ゅ?JC = (a*A鏀诲嚮*(A鏀诲嚮+k)/(A鏀诲嚮+B闃插尽+k)*D*H
          
		  鍏朵腑锛櫳		  闃插尽绯绘暟D=((A鐢熷懡/c+k锛堥 锛圔鐢熷懡/c+k锛夛級^0.5 /锛圔闃插尽+k锛埳        娴?偣鍨嬭嚦灏戜繚鐣橊浣嶅皬鏁般伾        
        浣撳姏绯绘暟H=If(A鐢熷懡>B鐢熷懡锛宎rctan锛圓鐢熷懡/ B鐢熷懡锛堥1.083+0.15锛嬸)
        娴?偣鍨嬭嚦灏戜繚鐣橊浣嶅皬鏁般伾        
        甯告暟a=1; c=20; k=10.
		*/
		
		float a = 1f;
		
		float c = 20f;
		
		float k = 10f;

		float D = Mathf.Pow((attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) / c + k) * (defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) / c + k), 0.5f) / (defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_defenceValue ) + k);
		
		float H = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) <= defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) ? 1f : Mathf.Atan (attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) / defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax )) * 1.083f + 0.15f;;
		
		float JC = (a * attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackValue ) * (attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackValue ) + k)) / (attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackValue ) + defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_defenceValue ) + k) * D * H;

		return JC;
	}

	public FloatBoolParam getAttackValue(BaseAI attacker, BaseAI defender, float weaponRatio = 1)
	{
		FloatBoolParam fbp = _getAttackValue (attacker, defender, weaponRatio);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			if(fbp.Float >= HYK && attacker.stance == BaseAI.Stance.STANCE_SELF)
			{
				fbp.Float = (fbp.Float / (fbp.Float + HYK)) * 2 * HYK;
			}

			return fbp;
		}
		else 
		{
			return fbp;
		}
	}

	public FloatBoolParam getAttackValueSkill(BaseAI attacker, BaseAI defender, float shanghaixishu, float gudingshanghai, bool criable = true)
	{
		FloatBoolParam fbp = _getAttackValueSkill (attacker, defender, shanghaixishu, gudingshanghai, criable);
		
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			if(fbp.Float >= HYK && attacker.stance == BaseAI.Stance.STANCE_SELF)
			{
				fbp.Float = (fbp.Float / (fbp.Float + HYK)) * 2 * HYK;
			}

			return fbp;
		}
		else 
		{
			return fbp;
		}
	}

	private FloatBoolParam _getAttackValue(BaseAI attacker, BaseAI defender, float weaponRatio = 1)
	{
		/*
		姝﹀櫒鏅?氫激瀹颤INT(JC*X*WM*WE)
		鑻ュ彇鏁村悗浼ゅ?涓癸锛屽垯浼ゅ?=1

		X涓烘墍浣跨敤姝﹀櫒鐨勪激瀹崇郴鏁帮紝瑙乑huangBei閰嶇疆琛▁ishu瀛楁?

		WM = (L+A姝﹀櫒浼ゅ?鍔犳繁)/( L +姝﹀櫒浼ゅ?鍑忓厤)

		WE =锛囸 + 棰濆?姝﹀櫒浼ゅ?鍔犳繁锛堥锛囸 - 棰濆?姝﹀櫒浼ゅ?鍑忓厤锛埳		璇ュ睘鎬ф潵鑷?妧鑳芥晥鏋浬
		姝﹀櫒鏆村嚮鐜嗊= 鍩虹?姝﹀櫒鏆村嚮鐜嗊+ 棰濆?姝﹀櫒鏆村嚮鐜喩		鍩虹?姝﹀櫒鏆村嚮鐜嗊= 0.2 
		棰濆?姝﹀櫒鏆村嚮鐜囨牴鎹?妧鑳藉姞鎴愯幏寰柹
		姝﹀櫒鏆村嚮浼ゅ? = 姝﹀櫒鏅?氫激瀹查(1+ (L+A姝﹀櫒鏆村嚮鍔犳繁)/(L+姝﹀櫒鏆村嚮鍑忓厤))

		L=100
		*/

		FloatBoolParam fbp = new FloatBoolParam ();

		if (defender == null || attacker == null || defender.hurtable == false) 
		{
			fbp.Float = 0;
			
			fbp.Bool = false;
			
			return fbp;
		}

		float L = 500f;

		float M = 5;

		float weaponAmplify = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_weaponAmplify_Total );

		float weaponReduction = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_weaponReduction_Total );

		if(weaponReduction >= 1)
		{
			BloodLabelControllor.Instance().showText(defender, LanguageTemplate.GetText( (LanguageTemplate.Text) 1243));
		}

		if(attacker.nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor kingAttacker = (KingControllor)attacker;

			if(kingAttacker.weaponType == KingControllor.WeaponType.W_Heavy)
			{
				weaponAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponAmplify_Heavy);
			
				weaponReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponReduction_Heavy);
			}
			else if(kingAttacker.weaponType == KingControllor.WeaponType.W_Light)
			{
				weaponAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponAmplify_Light);

				weaponReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponReduction_Light);
			}
			else if(kingAttacker.weaponType == KingControllor.WeaponType.W_Ranged)
			{
				weaponAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponAmplify_Range);

				weaponReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_weaponReduction_Range);
			}
		}

		float JC = getBaseAttackValue (attacker, defender);

		float attackAmplify = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackAmplify );

		float attackReduction = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackReduction );

		float WM = (L + attackAmplify) / (L + attackReduction);

		float WE = (1 + weaponAmplify) * (1 - weaponReduction);

		float SJ = Random.value * 0.2f + 0.9f;

		int pt = (int)(JC * weaponRatio * WM * WE * SJ);

		pt = pt < 1 ? 1 : pt;

		bool criFlag = attacker.countCri ();

		if(criFlag == false)
		{
			if(defender.reductionCount > 0)
			{
				pt = (int)(1.0f * pt * defender.reductionRate);

				defender.reductionCount --;
			}

			pt = pt < 1 ? 1 : pt;

			fbp.Float = pt;

			fbp.Bool = false;

			return fbp;
		}

		float attackAmplify_cri = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackAmplify_cri );
		
		float attackReduction_cri = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackReduction_cri );

		float WB = (L / M + attackAmplify_cri) / (L / M + attackReduction_cri);

		int bj = pt + (int)(JC * weaponRatio * WE * WB * SJ);

		bj = bj < 1 ? 1 : bj;

		if(defender.reductionCount > 0)
		{
			bj = (int)(1.0f * bj * defender.reductionRate);
			
			defender.reductionCount --;
		}

		bj = bj < 1 ? 1 : bj;

		fbp.Float = bj;
		
		fbp.Bool = true;
		
		return fbp;
	}

	private FloatBoolParam _getAttackValueSkill(BaseAI attacker, BaseAI defender, float shanghaixishu, float gudingshanghai, bool criable = true)
	{
		/*
		鎶鑳芥櫘閫氫激瀹颤INT(JC*K*JM)
		鑻ュ彇鏁村悗浼ゅ?涓癸锛屽垯浼ゅ?=1
			
		K涓鸿?鎶鑳藉?搴旂殑浼ゅ?绯绘暟锛屾牴鎹??搴旀妧鑳借?SkillTemplate琛?		
		JM = (1000+A鎶鑳戒激瀹冲姞娣拌/(1000+鎶鑳戒激瀹冲噺鍏岃
		
		鎶鑳芥毚鍑荤巼 = 鍩虹?鎶鑳芥毚鍑荤巼 + 棰濆?鎶鑳芥毚鍑荤巼
		鍩虹?鎶鑳芥毚鍑荤巼 = 0.2
		棰濆?鎶鑳芥毚鍑荤巼鏍规嵁鎶鑳藉姞鎴愯幏寰柹		
		鎶鑳芥毚鍑讳激瀹策= 鎶鑳芥櫘閫氫激瀹查(1+ (1000+A鎶鑳芥毚鍑诲姞娣拌/(1000+鎶鑳芥毚鍑诲噺鍏岃)
		*/

		FloatBoolParam fbp = new FloatBoolParam ();

		if (defender == null || attacker == null) 
		{
			fbp.Float = 0;

			fbp.Bool = false;

			return fbp;
		}

		float L = 500f;

		float M = 5;

		float skillAmplify = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillAmplify_Total );
		
		float skillReduction = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillReduction_Total );

		if(skillReduction >= 1)
		{
			BloodLabelControllor.Instance().showText(defender, LanguageTemplate.GetText( (LanguageTemplate.Text) 1242));
		}

		if(attacker.nodeData.nodeType == NodeType.PLAYER)
		{
			KingControllor kingAttacker = (KingControllor)attacker;
			
			if(kingAttacker.weaponType == KingControllor.WeaponType.W_Heavy)
			{
				skillAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillAmplify_Heavy);
				
				skillReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillReduction_Heavy);
			}
			else if(kingAttacker.weaponType == KingControllor.WeaponType.W_Light)
			{
				skillAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillAmplify_Light);
				
				skillReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillReduction_Light);
			}
			else if(kingAttacker.weaponType == KingControllor.WeaponType.W_Ranged)
			{
				skillAmplify += attacker.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillAmplify_Range);
				
				skillReduction += defender.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_skillReduction_Range);
			}
		}

		float JC = getBaseAttackValue (attacker, defender);

		float attackAmplify = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillAmplify );
		
		float attackReduction = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillReduction );
		
		float WM = (L + attackAmplify) / (L + attackReduction);

		float WE = (1 + skillAmplify) * (1 - skillReduction);

		float SJ = Random.value * 0.2f + 0.9f;

		int pt = (int)((JC * shanghaixishu + gudingshanghai) * WM * WE * SJ);

		pt = pt < 1 ? 1 : pt;

		bool criFlag = false;

		if(criable == true) criFlag = attacker.countCriSkill ();

		if(criFlag == false)
		{
			if(defender.reductionCount > 0)
			{
				pt = (int)(1.0f * pt * defender.reductionRate);

				defender.reductionCount --;
			}

			pt = pt < 1 ? 1 : pt;

			fbp.Float = pt;
			
			fbp.Bool = false;

			return fbp;
		}
		
		float attackAmplify_cri = attacker.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillAmplify_cri );
		
		float attackReduction_cri = defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_skillReduction_cri );

		//int bj = (int)(pt * (1 + (L + attackAmplify_cri) / (L + attackReduction_cri)));

		float JB = (L / M + attackAmplify_cri) / (L / M + attackReduction_cri);
		
		int bj = pt + (int)((JC * shanghaixishu + gudingshanghai) * WE * JB * SJ);

		if (bj < 1) bj = 1;

		if(defender.reductionCount > 0)
		{
			bj = (int)(1.0f * bj * defender.reductionRate);

			defender.reductionCount --;
		}

		bj = bj < 1 ? 1 : bj;

		fbp.Float = bj;
		
		fbp.Bool = true;

		return fbp;
	}

	public KingControllor getKing()
	{
		return king;
	}

	public BaseAI getNodebyId(int id)
	{
		foreach(BaseAI node in enemyNodes)
		{
			if(node.nodeId == id)
			{
				return node;
			}
		}

		foreach(BaseAI node in selfNodes)
		{
			if(node.nodeId == id)
			{
				return node;
			}
		}

		foreach(BaseAI node in midNodes)
		{
			if(node.nodeId == id)
			{
				return node;
			}
		}

		foreach(BaseAI node in reliveNodes)
		{
			if(node.nodeId == id)
			{
				return node;
			}
		}

//		Debug.Log ("RETURN NULL WITH DATA : " + id);

		return null;
	}

	public BaseAI getNodebyModelId(int modelId)
	{
		foreach(BaseAI node in enemyNodes)
		{
			if(node.nodeData.modleId == modelId)
			{
				return node;
			}
		}
		
		foreach(BaseAI node in selfNodes)
		{
			if(node.nodeData.modleId == modelId)
			{
				return node;
			}
		}
		
		foreach(BaseAI node in midNodes)
		{
			if(node.nodeData.modleId == modelId)
			{
				return node;
			}
		}
		
		foreach(BaseAI node in reliveNodes)
		{
			if(node.nodeData.modleId == modelId)
			{
				return node;
			}
		}
		
		return null;
	}

	public delegate void ClockCallback ();

	private ClockCallback mClockCallback;

	public void clockWithCallback(float time, ClockCallback _callback)
	{
		StartCoroutine (_clockWithCallback(time, _callback));
	}

	private IEnumerator _clockWithCallback(float time, ClockCallback _callback)
	{
		yield return new WaitForSeconds(time);

		_callback ();
	}

}
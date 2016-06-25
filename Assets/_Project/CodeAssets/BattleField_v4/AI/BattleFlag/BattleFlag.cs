using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlag : MonoBehaviour
{
	public enum TriggerMode
	{
		Enter,
		Attack,
		Kill,
	}

	public enum TriggerFunc
	{
		Null,				//无操作
		Crash,				//死亡
		FadeIn,				//出现
		Hurtable,			//取消无敌状态
		EyeToEye,			//视野共享
		Guide,				//guide表
		HintLabel,			//触发提示文字
		NodeSkillOn,		//触发后开启技能
		NodeSkillOff,		//触发后关闭技能
	}

	public int flagId;

	public int forwardFlagId = 1;

	public TriggerMode triggerMode;

	public int triggerCount;

	public bool willRelive = false;

	public bool dieable = true;

	public bool accountable = true;

	public bool hideInDrama = false;//小电影中是否隐藏

	public bool hideWithDramable = false;//根据是否出现剧情来决定是否显示

	public int guideId;

	public int hintLabelId;

	public bool showOnUI;

	public List<int> nodeSkillAble = new List<int> ();

	public List<GameObject> hoverPathGc = new List<GameObject>();

	public GameObject alarmGc;

	public TriggerFunc triggerFunc;

	public int triggerFuncEffect;

	public float triggerDelay;

	public GameObject triggerFlagEye2eyeRoot;

	public GameObject triggerFlagEnterRoot;

	public GameObject triggerFlagAttackRoot;

	public GameObject triggerFlagKillRoot;

	public List<BattleFlag> triggerFlagEye2eye = new List<BattleFlag> ();

	public List<BattleFlag> triggerFlagEnter = new List<BattleFlag>();

	public List<BattleFlag> triggerFlagAttack = new List<BattleFlag>();

	public List<BattleFlag> triggerFlagKill = new List<BattleFlag>();

	public List<Vector2> triggerFlagBlood = new List<Vector2>();


	[HideInInspector] public BaseAI node;

	[HideInInspector] public List<Vector3> hoverPath = new List<Vector3>();

	[HideInInspector] public Vector3 alarmPosition;

	[HideInInspector] public List<int> triggerFlagEye2eyeInteger = new List<int> ();

	[HideInInspector] public List<int> triggerFlagEnterInteger = new List<int>();

	[HideInInspector] public List<int> triggerFlagAttackInteger = new List<int>();

	[HideInInspector] public List<int> triggerFlagKillInteger = new List<int>();

	[HideInInspector] public List<Vector2> triggerFlagBloodInteger = new List<Vector2>();

	[HideInInspector] public List<DistanceFlag> triggerFlagDistance = new List<DistanceFlag>();

	[HideInInspector] public BattleFlagGroup flagGroup;

	[HideInInspector] public List<BattleDoorFlag> doorFlags = new List<BattleDoorFlag>();

	[HideInInspector] public GameObject wallObject;


	private bool triggerSuc;


	void Start()
	{
		triggerSuc = false;
	}

	void OnDestroy()
	{
		triggerFlagEye2eye.Clear();
		
		triggerFlagEnter.Clear();
		
		triggerFlagAttack.Clear();
		
		triggerFlagKill.Clear();
		
		triggerFlagBlood.Clear();

		doorFlags.Clear();

		node = null;
		
		flagGroup = null;
	}

	public void refreshEye2eyeFlags()
	{
		if (triggerFlagEye2eyeRoot == null) return;

		Component[] coms = triggerFlagEye2eyeRoot.GetComponentsInChildren(typeof(BattleFlag));

		foreach(Component co in coms)
		{
			BattleFlag bf = (BattleFlag)co;
			
			bool b = true;
			
			foreach(BattleFlag temp in triggerFlagEye2eye)
			{
				if(temp.flagId == bf.flagId) 
				{
					b = false;

					break;
				}
			}
			
			if(b == true) triggerFlagEye2eye.Add(bf);
		}
	}

	public void refreshEnterFlags()
	{
		if(triggerFlagEnterRoot == null) return;

		Component[] coms = triggerFlagEnterRoot.GetComponentsInChildren(typeof(BattleFlag));

		foreach(Component co in coms)
		{
			BattleFlag bf = (BattleFlag)co;

			bool b = true;

			foreach(BattleFlag temp in triggerFlagEnter)
			{
				if(temp.flagId == bf.flagId) 
				{
					b = false;

					break;
				}
			}

			if(b == true) triggerFlagEnter.Add(bf);
		}
	}

	public void refreshAttackFlags()
	{
		if(triggerFlagAttackRoot == null) return;
		
		Component[] coms = triggerFlagAttackRoot.GetComponentsInChildren(typeof(BattleFlag));
		
		foreach(Component co in coms)
		{
			BattleFlag bf = (BattleFlag)co;
			
			bool b = true;
			
			foreach(BattleFlag temp in triggerFlagAttack)
			{
				if(temp.flagId == bf.flagId) 
				{
					b = false;
					
					break;
				}
			}
			
			if(b == true) triggerFlagAttack.Add(bf);
		}
	}

	public void refreshKillFlags()
	{
		if(triggerFlagKillRoot == null) return;
		
		Component[] coms = triggerFlagKillRoot.GetComponentsInChildren(typeof(BattleFlag));
		
		foreach(Component co in coms)
		{
			BattleFlag bf = (BattleFlag)co;
			
			bool b = true;
			
			foreach(BattleFlag temp in triggerFlagKill)
			{
				if(temp.flagId == bf.flagId) 
				{
					b = false;

					break;
				}
			}
			
			if(b == true) triggerFlagKill.Add(bf);
		}
	}

	public void refreshBloodFlags()
	{

	}

	public void refreshGroup()
	{
		flagGroup = transform.parent.gameObject.GetComponent<BattleFlagGroup>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (triggerMode != TriggerMode.Enter) return;

		BaseAI t_node = other.gameObject.GetComponent<BaseAI>();

		if(t_node == null || !t_node.isAlive)
		{
			return;
		}

		foreach(BattleFlag f in triggerFlagEnter)
		{
			f.trigger(t_node);
		}

		foreach(BattleFlag f in triggerFlagEye2eye)
		{
			f.OnTriggerEyeToEye(t_node);
		}
	}

	public void trigger(BaseAI t_node = null)
	{
		if(!triggerSuc) StartCoroutine (_trigger(t_node));
	}

	IEnumerator _trigger(BaseAI t_node)
	{
		if(triggerDelay > 0)
		{
			yield return new WaitForSeconds(triggerDelay);
		}

		bool suc = false;

		if(triggerFunc == BattleFlag.TriggerFunc.Crash)
		{
			suc = OnTriggerCrash();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.FadeIn)
		{
			suc = OnTriggerFadeIn();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.Hurtable)
		{
			suc = OnTriggerHurtable();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.EyeToEye)
		{
			suc = OnTriggerEyeToEye(t_node);
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.Guide)
		{
			suc = OnTriggerGuide();
		}
		else if(triggerFunc == TriggerFunc.HintLabel)
		{
			suc = OnTriggerHintLabel();
		}
		else if(triggerFunc == TriggerFunc.NodeSkillOn)
		{
			suc = OnTriggerSkillOn();
		}
		else if(triggerFunc == TriggerFunc.NodeSkillOff)
		{
			suc = OnTriggerSkillOff();
		}

		if(suc)
		{
			if(!triggerSuc && triggerFuncEffect != 0)
			{
				if(node != null) BattleEffectControllor.Instance ().PlayEffect (triggerFuncEffect, node.transform.position, node.transform.forward);
				
				else BattleEffectControllor.Instance ().PlayEffect (triggerFuncEffect, transform.position, transform.forward);
			}

			triggerSuc = true;
		}
	}

	private bool OnTriggerCrash()
	{
		if(triggerCount > 0)
		{
			triggerCount --;

			if(triggerCount > 0) return false;
		}

		if(flagId >= 1000 && flagId < 2000) 
		{
			Destroy(gameObject);

			/*
			BoxCollider bc = (BoxCollider)gameObject.GetComponent(typeof(BoxCollider));

			if(bc != null) Destroy(bc);

			NavMeshObstacle nmo = (NavMeshObstacle)gameObject.GetComponent (typeof(NavMeshObstacle));

			if(nmo != null) Destroy(nmo);
			*/
		}
		else
		{
			if(node != null) node.die(false);
		}

		return true;
	}

	public bool OnTriggerEyeToEye(BaseAI nodeEnter)
	{
		if(node == null) return false;

		if (nodeEnter == null) return false;

		SphereCollider collider = (SphereCollider)nodeEnter.GetComponent("SphereCollider");

		node.OnTriggerEnterNode(nodeEnter);

		return true;
	}

	private bool OnTriggerFadeIn()
	{
		if(triggerCount > 0)
		{
			triggerCount --;

			if(triggerCount > 0) return false;
		}

		if(flagId >= 1000 && flagId < 2000) 
		{
			wallObject.SetActive(true);
		}
		else if(flagGroup != null)
		{
			flagGroup.FadeIn(this);
		}
		else
		{
			if(node != null) node.fadeIn();
		}

		return true;
	}

	private bool OnTriggerHurtable()
	{
		if(node == null) return false;

		if(triggerCount > 0)
		{
			triggerCount --;
			
			if(triggerCount > 0) return false;
		}

		node.hurtable = true;

		return true;
	}

	private bool OnTriggerGuide()
	{
		if(triggerCount > 0)
		{
			triggerCount --;
			
			if(triggerCount > 0) return false;
		}
		
		GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (CityGlobalData.m_configId, guideId);
		
		bool flag = BattleControlor.Instance().havePlayedGuide (template);
		
		if (flag == true) return false;
		
		BattleUIControlor.Instance().showDaramControllor (CityGlobalData.m_configId, template.id);

		return true;
	}

	private bool OnTriggerHintLabel()
	{
		if(CityGlobalData.m_levelType != qxmobile.protobuf.LevelType.LEVEL_TALE)
		{
			SceneGuideManager.Instance().ShowSceneGuide (hintLabelId);
		}

		return true;
	}

	private bool OnTriggerSkillOn()
	{
		if (node == null) return false;

		foreach(int skillId in nodeSkillAble)
		{
			HeroSkill skill = node.getSkillById(skillId);
			
			if(skill != null) skill.template.zhudong = false;
		}

		return true;
	}

	private bool OnTriggerSkillOff()
	{
		if (node == null) return false;

		foreach(int skillId in nodeSkillAble)
		{
			HeroSkill skill = node.getSkillById(skillId);
			
			if(skill != null) skill.template.zhudong = true;
		}

		return true;
	}

}

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
		Null,              //无操作
		Crash,             //死亡
		FadeIn,            //出现
		Hurtable,          //取消无敌状态
		EyeToEye,          //视野共享
		Guide,             //guide表
		HintLabel,         //触发提示文字
		NodeSkillOn,       //触发后开启技能
		NodeSkillOff,      //触发后关闭技能
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

	public List<int> nodeSkillAble = new List<int> ();

	public List<GameObject> hoverPathGc = new List<GameObject>();

	public GameObject alarmGc;

	public TriggerFunc triggerFunc;

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
		if(triggerFunc == BattleFlag.TriggerFunc.Crash)
		{
			OnTriggerCrash();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.FadeIn)
		{
			OnTriggerFadeIn();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.Hurtable)
		{
			OnTriggerHurtable();
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.EyeToEye)
		{
			OnTriggerEyeToEye(t_node);
		}
		else if(triggerFunc == BattleFlag.TriggerFunc.Guide)
		{
			OnTriggerGuide();
		}
		else if(triggerFunc == TriggerFunc.HintLabel)
		{
			OnTriggerHintLabel();
		}
		else if(triggerFunc == TriggerFunc.NodeSkillOn)
		{
			OnTriggerSkillOn();
		}
		else if(triggerFunc == TriggerFunc.NodeSkillOff)
		{
			OnTriggerSkillOff();
		}
	}

	private void OnTriggerCrash()
	{
		if(triggerCount > 0)
		{
			triggerCount --;

			if(triggerCount > 0) return;
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
			node.die();
		}
	}

	public void OnTriggerEyeToEye(BaseAI nodeEnter)
	{
		if(node == null) return;

		if (nodeEnter == null) return;

		SphereCollider collider = (SphereCollider)nodeEnter.GetComponent("SphereCollider");

		node.OnTriggerEnter(collider);
	}

	private void OnTriggerFadeIn()
	{
		if(triggerCount > 0)
		{
			triggerCount --;

			if(triggerCount > 0) return;
		}

		if(flagGroup != null)
		{
			flagGroup.FadeIn(this);
		}
		else
		{
			if(node != null) node.fadeIn();
		}
	}

	private void OnTriggerHurtable()
	{
		if(node == null) return;

		if(triggerCount > 0)
		{
			triggerCount --;
			
			if(triggerCount > 0) return;
		}

		node.hurtable = true;
	}

	private void OnTriggerGuide()
	{
		if(triggerCount > 0)
		{
			triggerCount --;
			
			if(triggerCount > 0) return;
		}
		
		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
		
		GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (level, guideId);
		
		bool flag = BattleControlor.Instance ().havePlayedGuide (template);
		
		if (flag == true) return;
		
		BattleUIControlor.Instance ().showDaramControllor (level, template.id);
	}

	private void OnTriggerHintLabel()
	{
		if(CityGlobalData.m_levelType != qxmobile.protobuf.LevelType.LEVEL_TALE)
		{
			SceneGuideManager.Instance ().ShowSceneGuide (hintLabelId);
		}
	}

	private void OnTriggerSkillOn()
	{
		if (node == null) return;

		foreach(int skillId in nodeSkillAble)
		{
			HeroSkill skill = node.getSkillById(skillId);
			
			if(skill != null) skill.template.zhudong = false;
		}
	}

	private void OnTriggerSkillOff()
	{
		if (node == null) return;

		foreach(int skillId in nodeSkillAble)
		{
			HeroSkill skill = node.getSkillById(skillId);
			
			if(skill != null) skill.template.zhudong = true;
		}
	}

}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class HeroSkill : MonoBehaviour
{
	public static int m_Nummmmm = 0;

	public static int m_NumStop = 0;
	public bool m_isGuanlian = false;

	private bool m_isNewSkillType = true;

	public bool m_isUseThisSkill = false;

	public NodeSkill template;
	
	public BaseAI node;
	
	private float cooldown;

	private bool m_vectorMove = false;

	[HideInInspector] public int m_isDeadOverSkill = 0; //死亡后0不播放1继续播放

	public List<HeroSkill> m_otherSkill = new List<HeroSkill> ();

	private List<SkillDataBead> m_SkillDataBead = new List<SkillDataBead>();
	//随机天雷
	private string temptemptempString = "50,0,10,1,1,0,0,66,0,1,14,0,2,1,2,1.5,0,999,0.5,1,0,1,0.5,30,1.5,1,8,0,-50,30,1.5,1,14,0,-0.5,30,1.5,1,11,1,0.5,30,1.5,1,12,1,0.5,30,1.5,1,100,3,1.5,1,101,1,0.01,10,1,1.5,1,102,3,4.5,2,10,132,133,134,135,136,137,138,139,140,141,0.5,3,3,2,0,1,0.5,7,0,-50,4,6,125,126,127,128,129,130,1.5,5,5,1.5,6,1,0.01,1.5";

	public string m_sDec;

	private USETYPE m_iUseType = 0;//技能释放触发类型

	//技能释放触发类型
	public enum USETYPE{
		DISTANCE = 0,
		ATK,
		BYATK,
		SKILLATK,
		SKILLBYATK,
		HP,
		POSDIS,
		DISNUM,
		DISROUND,
	}

	private List<int> m_ListProfession = new List<int>();

	private List<int> m_ListType = new List<int>();

	private float m_iUseValue0 = 0;

	private float m_iUseValue1 = 0;

	private float m_iUseValue2 = 0;

	private List<int> m_listUseNodeID = new List<int>();

	private List<int> m_listCollNodeID = new List<int>();

	public enum COLLSTATETYPE//碰撞类型
	{
		ROUND = 0,
		ANGLE,
		VECTOR
	}

	private COLLSTATETYPE m_CollStateType = COLLSTATETYPE.ROUND;//碰撞类型

	private float m_iCollValue0;

	private float m_iCollValue1;

	private float m_iCollValue2;

	private float m_iCollValue3;

	private float m_iCollValue4;

	private string m_sAnimName;

	private bool m_isUseSelf = false;//施法敌方我方0敌方1我方

	private bool m_isRoleSelf = false;//作用敌方我方0敌方1我方

	public enum ATTTYPE//单体群体
	{
		ONE = 0,//单体
		SOME,//群体
		RANDOM//随机
	}

	private ATTTYPE m_ATTTYPE = ATTTYPE.RANDOM;//单体群体

	private int m_iTargetValue0;

	private int m_iTargetValue1;

	private int m_iEffID = 0;//特效ID

	private int m_iEffLockID = 0;//绑定ID

	private int m_iShow = 0;//0显示技能攻击范围  1不显示

	private int m_iPlayByAtk = 0;//0播放被击  1不播放

	private int m_iPlayPreID = 0;//首次击中播放技能ID
	
	private float m_iPlayPreTime = 0.0f;//首次击中特效生命时间

	private int m_iPlayFirstID = 0;//首次击中播放技能ID

	private float m_iPlayFirstTime = 0.0f;//首次击中特效生命时间

	private int m_iUseNum = 0;

	private bool m_isEffLock = false;

	private bool m_isEffPlaySelf = false;

	private List<GameObject> m_listMyEffElement = new List<GameObject>();

	private List<float> m_listMyEffCutTime = new List<float>();

	private List<GameObject> m_listShowRange = new List<GameObject>();

	public List<BaseAI> m_listATTTarget = new List<BaseAI>();

	private GameObject m_PreElement;

	private float m_PreTime;

	private List<GameObject> m_listFirstElement = new List<GameObject>();

	private List<BaseAI> m_listFirstAI = new List<BaseAI>();

	private List<float> m_listFirstTime = new List<float>();

	public int m_iIndex;

	public enum SKILLELEMENTTYPE//技能元素类型
	{
		LOSTHP = 0,//伤害
		BUFF,//BUFF
		SUMMON,//召唤
		COPY,//分身
		RESURRECTION,//复活
		BACK,//后退
		ADDHP,//加血
		FALL,//击倒
		BYATK,//让对方播放被击 强制打断
		DELETEBUFF,//清除BUFF
	}

	public List<SKILLELEMENTTYPE> m_look = new List<SKILLELEMENTTYPE>();
	//此技能打到的记录
	public List<List<BaseAI>> m_AttBaseAI = new List<List<BaseAI>>();

	public Vector3 m_VForWard;

	public float ceshi_endTime;

	public string dis;

	public void init(NodeSkill _template, int index)
	{
		if( _template == null ){
			Debug.LogError( "HeroSkill.init _template == null." );
		}

		template = _template;

		m_iIndex = index;

		node = (BaseAI) gameObject.GetComponent ("BaseAI");

		init();
		ceshi_endTime = template.endtime;
	}

	public virtual void init()
	{
		int tempValue;
		temptemptempString = template.value7;

		if(temptemptempString == "" || temptemptempString == null || temptemptempString == "0")
		{
			m_isNewSkillType = false;
			return;
		}

		m_sDec = Global.NextCutting(ref temptemptempString);
		if(m_sDec == "0")
		{
			m_sDec = "";
		}

		int otherSkillNum = int.Parse(Global.NextCutting(ref temptemptempString));
//		Debug.Log("otherSkillNum="+otherSkillNum);
//		for(int q = 0; q < node.skills.Count; q ++)
//		{
//			Debug.Log("node.skills[q].template.id="+node.skills[q].template.id);
//		}
		for (int i = 0; i < otherSkillNum; i ++)
		{
			int tempSkillID = int.Parse(Global.NextCutting(ref temptemptempString));
//			Debug.Log("tempSkillID="+tempSkillID);
			for(int q = 0; q < node.skills.Count; q ++)
			{
				if(node.skills[q].template.id == tempSkillID)
				{
					node.skills[q].m_isGuanlian = true;
					m_otherSkill.Add(node.skills[q]);
				}
			}
		}

		m_isDeadOverSkill = int.Parse(Global.NextCutting(ref temptemptempString));

		m_iUseType = (USETYPE)int.Parse(Global.NextCutting(ref temptemptempString));//技能触发类型
		dis = "";
		switch(m_iUseType)
		{
		case USETYPE.DISTANCE:
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
//			m_iUseValue0 = 1.1f;
			dis += "施法为距离" + m_iUseValue0 + ",";
			break;
		case USETYPE.ATK:
			dis += "攻击时施法,";
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			break;
		case USETYPE.BYATK:
			dis += "被攻击时施法,";
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			break;
		case USETYPE.SKILLATK:
			dis += "施放技能时施法,";
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			break;
		case USETYPE.HP:
			dis += "HP小于施放,";
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			m_iUseValue1 = float.Parse(Global.NextCutting(ref temptemptempString));//血量
			int tempNum = int.Parse(Global.NextCutting(ref temptemptempString));

			if(tempNum > 0)
			{
				dis += "目标ID分别为";
				for(int i = 0; i < tempNum; i ++)
				{
					m_listUseNodeID.Add(int.Parse(Global.NextCutting(ref temptemptempString)));
					dis += (m_listUseNodeID + ",");
				}
				dis += "，";
			}
			else if(tempNum == -1)
			{
//				Debug.Log("Null");
				m_listUseNodeID = null;
			}
			break;
		case USETYPE.POSDIS:
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			dis += "技能CD到在面前距离为" + m_iUseValue0 +"的地方释放技能";
			break;
		case USETYPE.DISNUM:
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			m_iUseValue1 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			m_iUseValue2 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			dis += "范围" + m_iUseValue0 +"内";
			if(m_iUseValue1 == 0)
			{
				dis += "大于" + m_iUseValue2 + "施放，";
			}
			else
			{
				dis += "小于" + m_iUseValue2 + "施放，";
			}
			break;
		case USETYPE.DISROUND:
			m_iUseValue0 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			m_iUseValue1 = float.Parse(Global.NextCutting(ref temptemptempString));//施法距离(圆形)
			dis += "环行内圈" + m_iUseValue0 + "外圈" + m_iUseValue1 + "施放，";
			break;
		}

		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));
		for(int i = 0; i < tempValue; i ++)
		{
			m_ListProfession.Add(int.Parse(Global.NextCutting(ref temptemptempString)));
		}

		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));
		for(int i = 0; i < tempValue; i ++)
		{
			m_ListType.Add(int.Parse(Global.NextCutting(ref temptemptempString)));
		}

		m_CollStateType = (COLLSTATETYPE)int.Parse(Global.NextCutting(ref temptemptempString));//作用范围类型

		//作用范围
		switch(m_CollStateType)
		{
		case COLLSTATETYPE.ROUND:
			m_iCollValue0 = float.Parse(Global.NextCutting(ref temptemptempString));
			dis += "圆形作用范围为" + m_iCollValue0 + "，";
			break;
		case COLLSTATETYPE.ANGLE:
			m_iCollValue0 = float.Parse(Global.NextCutting(ref temptemptempString));
			m_iCollValue1 = float.Parse(Global.NextCutting(ref temptemptempString));
			dis += "扇形作用范围半径为" + m_iCollValue0 + "角度为" + m_iCollValue1 + "，";
			break;
		case COLLSTATETYPE.VECTOR:
			m_iCollValue0 = float.Parse(Global.NextCutting(ref temptemptempString));
			m_iCollValue1 = float.Parse(Global.NextCutting(ref temptemptempString));
			m_iCollValue2 = float.Parse(Global.NextCutting(ref temptemptempString));
			m_iCollValue3 = float.Parse(Global.NextCutting(ref temptemptempString));
			m_iCollValue4 = int.Parse(Global.NextCutting(ref temptemptempString));
			dis += "射线，";
			break;
		}
		m_sAnimName = Global.NextCutting(ref temptemptempString);//动作名字

//		Debug.Log("m_sAnimName="+m_sAnimName);
		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));//施法目标敌方我方
		if(tempValue == 0)
		{
			m_isUseSelf = true;
			dis += "施法目标我方，";
//			Debug.Log("m_isUseSelf="+m_isUseSelf);
		}
		else
		{
			m_isUseSelf = false;
			dis += "施法目标敌方，";
		}

		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));//作用目标敌方我方
		if(tempValue == 0)
		{
			m_isRoleSelf = true;
			dis += "作用目标我方，";
		}
		else
		{
			m_isRoleSelf = false;
			dis += "作用目标敌方，";
		}
//		Debug.Log("m_isRoleSelf="+m_isRoleSelf);

		m_ATTTYPE = (ATTTYPE)int.Parse(Global.NextCutting(ref temptemptempString));//技能播放目标单体群体
		//目标
		switch(m_ATTTYPE)
		{
		case ATTTYPE.ONE:
			m_iTargetValue0 = int.Parse(Global.NextCutting(ref temptemptempString));
			break;
		case ATTTYPE.SOME:
			m_iTargetValue0 = int.Parse(Global.NextCutting(ref temptemptempString));
//			Debug.Log("m_iTargetValue0="+m_iTargetValue0);
			break;
		case ATTTYPE.RANDOM:
			m_iTargetValue1 = int.Parse(Global.NextCutting(ref temptemptempString));
			m_iTargetValue0 = int.Parse(Global.NextCutting(ref temptemptempString));
			break;
		}
		m_iEffID = int.Parse(Global.NextCutting(ref temptemptempString));//特效ID
		m_iEffLockID = int.Parse(Global.NextCutting(ref temptemptempString));//绑定特效ID
		if(m_iEffLockID != 0)
		{
			GameObject tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffLockID);
			tempEffobj.SetActive(true);
			tempEffobj.transform.position = node.gameObject.transform.position;
			tempEffobj.transform.rotation = node.gameObject.transform.rotation;
			tempEffobj.transform.parent = node.gameObject.transform;
		}
		m_iShow = int.Parse(Global.NextCutting(ref temptemptempString));
		m_iPlayByAtk = int.Parse(Global.NextCutting(ref temptemptempString));
		m_iPlayPreID = int.Parse(Global.NextCutting(ref temptemptempString));
		m_iPlayPreTime = float.Parse(Global.NextCutting(ref temptemptempString));
		m_iPlayFirstID = int.Parse(Global.NextCutting(ref temptemptempString));
		m_iPlayFirstTime = float.Parse(Global.NextCutting(ref temptemptempString));
//		m_iUseNum = 1;
		m_iUseNum = int.Parse(Global.NextCutting(ref temptemptempString));
		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));//技能播放目标
		if(tempValue == 0)
		{
			m_isEffLock = true;
		}
		else
		{
			m_isEffLock = false;
		}
		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));//技能播放单体群体
		if(tempValue == 0)
		{
			m_isEffPlaySelf = true;
		}
		else
		{
			m_isEffPlaySelf = false;
		}
//		m_fEffEndTime = float.Parse(Global.NextCutting(ref temptemptempString));
//		Debug.Log("temptemptempString="+temptemptempString);
		tempValue = int.Parse(Global.NextCutting(ref temptemptempString));
//		Debug.Log(tempValue);
		for(int i = 0; i < tempValue; i ++)
		{
//			Debug.Log(temptemptempString);
			SKILLELEMENTTYPE tempSKILLELEMENTTYPE = (SKILLELEMENTTYPE)int.Parse(Global.NextCutting(ref temptemptempString));
//			Debug.Log(tempSKILLELEMENTTYPE);
			m_look.Add(tempSKILLELEMENTTYPE);
			SkillDataBead tempSkillDataBead;
			switch(tempSKILLELEMENTTYPE)
			{
			case SKILLELEMENTTYPE.LOSTHP:
				tempSkillDataBead = new SkillShanghai(this);
				break;
			case SKILLELEMENTTYPE.BUFF:
				tempSkillDataBead = new SkillBuff(this);
				break;
			case SKILLELEMENTTYPE.SUMMON:
				tempSkillDataBead = new SkillSummon(this);
				break;
			case SKILLELEMENTTYPE.COPY:
				tempSkillDataBead = new SkillCopy(this);
				break;
			case SKILLELEMENTTYPE.RESURRECTION:
				tempSkillDataBead = new SkillResurrection(this);
				break;
			case SKILLELEMENTTYPE.BACK:
				tempSkillDataBead = new SkillBack(this);
				break;
			case SKILLELEMENTTYPE.ADDHP:
				tempSkillDataBead = new SkillAddHP(this);
				break;
			case SKILLELEMENTTYPE.FALL:
				tempSkillDataBead = new SkillFall(this);
				break;
			case SKILLELEMENTTYPE.BYATK:
				tempSkillDataBead = new SkillByAtk(this);
				break;
			case SKILLELEMENTTYPE.DELETEBUFF:
				tempSkillDataBead = new SkillDeleteBuff(this);
				break;
			default:
				Debug.Log("tempSKILLELEMENTTYPE="+tempSKILLELEMENTTYPE);
				tempSkillDataBead = new SkillSummon(this);
				break;
			}
			tempSkillDataBead.setData(ref temptemptempString);
			m_SkillDataBead.Add(tempSkillDataBead);
//			if(tempSkillDataBead != null)
//			{
//				tempSkillDataBead.setData(ref temptemptempString);
//				m_SkillDataBead.Add(tempSkillDataBead);
//			}
		}
		if(template.zhudong)
		{
			cooldown = Time.time - template.timePeriod;
		}
	}
	int m_Num = 0;
	public virtual bool castSkill()
	{
		m_Num ++;
		if(m_isNewSkillType)
		{
			List<BaseAI> tempTarget = new List<BaseAI>();
			List<BaseAI> tempTarget2 = new List<BaseAI>();
			List<BaseAI> tempTarget3 = new List<BaseAI>();
			switch(m_iUseType)
			{
			case USETYPE.DISTANCE:
				for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().midNodes[i]);
				}
				tempTarget = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0, tempTarget, m_isUseSelf);
				break;
			case USETYPE.ATK:
				tempTarget = node.m_listAtk;
				m_listATTTarget = node.m_listAtk;
				break;
			case USETYPE.BYATK:
				tempTarget = node.m_listByAtk;
				m_listATTTarget = node.m_listByAtk;
				break;
			case USETYPE.SKILLATK:
				tempTarget = node.m_listSkill;
				m_listATTTarget = node.m_listSkill;
				break;
			case USETYPE.SKILLBYATK:
				tempTarget = node.m_listBySkill;
				m_listATTTarget = node.m_listBySkill;
				break;
			case USETYPE.HP:
				for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().midNodes[i]);
				}
				tempTarget = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0, tempTarget, m_isUseSelf);

				if(m_listUseNodeID == null)
				{
					tempTarget.Remove(node);
					for(int i = 0; i < tempTarget.Count; i ++)
					{
						if(tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) > m_iUseValue1 )
						{
							tempTarget.RemoveAt(i);
							i --;
						}
					}
				}
				else if(m_listUseNodeID.Count != 0)
				{
					tempTarget.Remove(node);
					for(int i = 0; i < tempTarget.Count; i ++)
					{
						for(int q = 0; q < m_listUseNodeID.Count; q ++)
						{
							if(m_listUseNodeID[q] == tempTarget[i].nodeId)
							{
								if(tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) <= m_iUseValue1 )
								{
									return true;
								}
							}
						}
					}
					return false;
				}
				else
				{
					tempTarget = new List<BaseAI>();
					tempTarget.Add(node);
					for(int i = 0; i < tempTarget.Count; i ++)
					{
						if(tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / tempTarget[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) > m_iUseValue1 )
						{
							tempTarget.RemoveAt(i);
							i --;
						}
					}
				}
//				else
//				{
//					tempTarget.Remove(node);
//
//					return false;
//				}
				break;
			case USETYPE.POSDIS:
				tempTarget.Add(node);
				break;
			case USETYPE.DISNUM:
//				if(m_iIndex == 2)
//				{
//					Debug.Log("Nodes");
//					Debug.Log(BattleControlor.Instance().selfNodes.Count);
//					Debug.Log(BattleControlor.Instance().reliveNodes.Count);
//					Debug.Log(BattleControlor.Instance().enemyNodes.Count);
//					Debug.Log(BattleControlor.Instance().midNodes.Count);
//				}
				for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().midNodes[i]);
				}

				tempTarget2 = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0, tempTarget, m_isUseSelf);
				if(m_iIndex == 2)
				{
					//Debug.Log(tempTarget2.Count);
				}
				if(m_iUseValue1 == 0)
				{
					if(tempTarget2.Count >= m_iUseValue2)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					if(tempTarget2.Count <= m_iUseValue2)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				break;
			case USETYPE.DISROUND:
				for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().midNodes[i]);
				}

				tempTarget2 = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0, tempTarget, m_isUseSelf);
				tempTarget = new List<BaseAI>();
				for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				}
				for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
				{
					tempTarget.Add(BattleControlor.Instance().midNodes[i]);
				}
				tempTarget3 = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue1, tempTarget, m_isUseSelf);
				int tempNum = tempTarget3.Count - tempTarget2.Count;
				if(tempNum == 0)
				{
					return false;
				}
				else
				{
					return true;
				}
				break;
			}

			if(tempTarget.Count != 0)
			{
				m_isUseThisSkill = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}
	List<GameObject> m_tempCube = new List<GameObject>();
	public void setShowFanRand()
	{
		for(int q = 0; q < m_SkillDataBead.Count; q ++)
		{
			m_SkillDataBead[q].m_iCurrentIndex = 0;
		}
		GameObject tempEffobj;
		if(!m_isUseThisSkill)
		{
			return;
		}
		List<BaseAI> tempTarget = new List<BaseAI>();
		if(m_iUseType != USETYPE.DISTANCE && m_iUseValue0 == 1 && !m_isGuanlian)
		{
			tempTarget = m_listATTTarget;
		}
		else if(m_iUseType == USETYPE.POSDIS)
		{
			tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
			tempEffobj.SetActive(false);
			Vector3 tempPos = node.gameObject.transform.forward;
			tempPos.y = 0;
			tempEffobj.transform.position = node.gameObject.transform.position;
			tempEffobj.transform.position += (tempPos.normalized * m_iUseValue0);
			tempEffobj.name = "skill" + m_iIndex;
			m_listMyEffElement.Add(tempEffobj);
			m_AttBaseAI.Add(new List<BaseAI>());
			setShow();
			return;
		}
		else
		{
			for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
			{
				tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
			}
			for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
			{
				tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
			}
			for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
			{
				tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
				//			Debug.Log(BattleControlor.Instance().enemyNodes[i].gameObject.name);
			}
			for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
			{
				tempTarget.Add(BattleControlor.Instance().midNodes[i]);
			}
			int tempAddDis = 0;
			List<BaseAI> tempBaseAI = new List<BaseAI>();
			for(int i = 0; i < 10; i ++)
			{
				tempBaseAI = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0 + tempAddDis, tempTarget, m_isUseSelf);
				tempAddDis += 2;
				if(tempBaseAI != null && tempBaseAI.Count > 0)
				{
					break;
				}
				else
				{
					tempTarget = new List<BaseAI>();
					for(int q = 0; q < BattleControlor.Instance().selfNodes.Count; q ++)
					{
						tempTarget.Add(BattleControlor.Instance().selfNodes[q]);
					}
					for(int q = 0; q < BattleControlor.Instance().reliveNodes.Count; q ++)
					{
						tempTarget.Add(BattleControlor.Instance().reliveNodes[q]);
					}
					for(int q = 0; q < BattleControlor.Instance().enemyNodes.Count; q ++)
					{
						tempTarget.Add(BattleControlor.Instance().enemyNodes[q]);
						//			Debug.Log(BattleControlor.Instance().enemyNodes[i].gameObject.name);
					}
					for(int q = 0; q < BattleControlor.Instance().midNodes.Count; q ++)
					{
						tempTarget.Add(BattleControlor.Instance().midNodes[q]);
					}
				}
			}
//			while(true)
//			{
//				tempBaseAI = getListTarget(COLLSTATETYPE.ROUND, node.gameObject, m_iUseValue0 + tempAddDis, tempTarget, m_isUseSelf);
//				tempAddDis ++;
//				if(tempBaseAI != null && tempBaseAI.Count > 0)
//				{
//					break;
//				}
//			}
			tempTarget = tempBaseAI;
		}
		if(tempTarget == null || tempTarget.Count == 0)
		{
			return;
		}
		BaseAI tempTargetOne;
		tempTargetOne = getLatestTarget(tempTarget);//获取目标中最近的一个
		node.gameObject.transform.LookAt(tempTargetOne.gameObject.transform.position);
		if((m_CollStateType == COLLSTATETYPE.VECTOR) && m_sAnimName != "null")
		{
			m_VForWard = node.gameObject.transform.forward;
			m_VForWard.y = 0;
			m_VForWard.Normalize();
		}

		switch(m_ATTTYPE)
		{
		case ATTTYPE.ONE:
			if(m_isEffPlaySelf)
			{
				tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
				tempEffobj.SetActive(false);
				tempEffobj.transform.position = node.gameObject.transform.position;
				tempEffobj.transform.rotation = node.gameObject.transform.rotation;
				if(m_CollStateType == COLLSTATETYPE.VECTOR)
				{
					tempEffobj.transform.LookAt(tempTargetOne.gameObject.transform);
					tempEffobj.transform.position = new Vector3(tempEffobj.transform.position.x, tempEffobj.transform.position.y, tempEffobj.transform.position.z);
				}
				if(m_isEffLock)
				{
					tempEffobj.transform.parent = node.gameObject.transform;
					tempEffobj.transform.localPosition = new Vector3(tempEffobj.transform.localPosition.x, (tempEffobj.transform.localPosition.y + (node.getHeight() * EffectIdTemplate.GetHeight(m_iEffID))), tempEffobj.transform.localPosition.z);
				}
				tempEffobj.name = "skill" + m_iIndex;
			}
			else
			{
				tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
				tempEffobj.SetActive(false);
				tempEffobj.transform.position = tempTargetOne.gameObject.transform.position;
				if(m_isEffLock)
				{
					tempEffobj.transform.parent = tempTargetOne.gameObject.transform;
					tempEffobj.transform.localPosition = new Vector3(tempEffobj.transform.localPosition.x, (tempEffobj.transform.localPosition.y + (node.getHeight() * EffectIdTemplate.GetHeight(m_iEffID))), tempEffobj.transform.localPosition.z);
				}
				tempEffobj.name = "skill" + m_iIndex;
			}
			m_listMyEffElement.Add(tempEffobj);
			m_AttBaseAI.Add(new List<BaseAI>());
			break;
		case ATTTYPE.SOME:
			if(m_isEffPlaySelf)
			{
				tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
				tempEffobj.SetActive(false);
				tempEffobj.transform.position = node.gameObject.transform.position;
				if(m_isEffLock)
				{
					tempEffobj.transform.parent = node.gameObject.transform;
					tempEffobj.transform.localPosition = new Vector3(tempEffobj.transform.localPosition.x, (tempEffobj.transform.localPosition.y + (node.getHeight() * EffectIdTemplate.GetHeight(m_iEffID))), tempEffobj.transform.localPosition.z);
				}
				m_listMyEffElement.Add(tempEffobj);
				m_AttBaseAI.Add(new List<BaseAI>());
			}
			else
			{
				for(int i = 0; i < tempTarget.Count; i ++)
				{
					tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
					tempEffobj.SetActive(false);
					tempEffobj.transform.position = tempTarget[i].gameObject.transform.position;
					if(m_isEffLock)
					{
						tempEffobj.transform.parent = tempTarget[i].gameObject.transform;
						tempEffobj.transform.localPosition = new Vector3(tempEffobj.transform.localPosition.x, (tempEffobj.transform.localPosition.y + (node.getHeight() * EffectIdTemplate.GetHeight(m_iEffID))), tempEffobj.transform.localPosition.z);
					}
					m_listMyEffElement.Add(tempEffobj);
					m_AttBaseAI.Add(new List<BaseAI>());
				}
			}
			break;
		case ATTTYPE.RANDOM:
			for(int i = 0; i < m_iTargetValue0; i ++)
			{
				while(true)
				{
					int tempX = Global.getRandom(m_iTargetValue1 + 1) - (m_iTargetValue1 / 2);
					int tempZ = Global.getRandom(m_iTargetValue1 + 1) - (m_iTargetValue1 / 2);
					Vector3 tempSkillPos = new Vector3(tempTargetOne.transform.position.x + tempX, tempTargetOne.transform.position.y, tempTargetOne.transform.position.z + tempZ);
					if(Vector3.Distance(tempSkillPos, tempTargetOne.transform.position) < m_iTargetValue1)
					{
						tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iEffID);
						tempEffobj.SetActive(false);
						tempEffobj.transform.position = tempSkillPos;
						m_listMyEffElement.Add(tempEffobj);
						m_AttBaseAI.Add(new List<BaseAI>());
						break;
					}
				}
			}
			break;
		}
		//Debug.Log("m_listMyEffElement.Count="+m_listMyEffElement.Count);
		setShow();
	}

	public void setShow()
	{
		if(m_iShow == 0)
		{
			Debug.Log(node.gameObject.name + "+m_CollStateType="+m_CollStateType);
			GameObject tempobjColl;
			switch(m_CollStateType)
			{
			case COLLSTATETYPE.VECTOR:
				for(int i = 0; i < m_listMyEffElement.Count; i ++)
				{
					float tempValue1 = m_iCollValue1;
					if(m_iCollValue0 != 0 || m_isEffLock)
					{
						m_iCollValue1 = 20;
						if(m_isEffLock)
						{
							m_iCollValue1 = m_iCollValue0 * template.endtime;
						}
						else
						{
							m_iCollValue1 = m_iCollValue0 * 33 * template.endtime;
						}

					}

					tempobjColl = Global.getEffAtkWaring(ClientMain.m_rect);
					tempobjColl.transform.position = node.transform.position;
					tempobjColl.transform.position = new Vector3(tempobjColl.transform.position.x, tempobjColl.transform.position.y + 0.01f, tempobjColl.transform.position.z);
					Vector3 moveat = node.transform.forward;
					moveat.y = 0;
					tempobjColl.transform.position += (moveat.normalized * (m_iCollValue1 / 2));
					Vector3 tempRotation = node.transform.localRotation.eulerAngles;

					tempobjColl.transform.localRotation = Quaternion.Euler(90,tempRotation.y + 90,0);

					tempobjColl.transform.localScale = new Vector3(m_iCollValue1, m_iCollValue2, 1);

					m_listShowRange.Add(tempobjColl);
					m_iCollValue1 = tempValue1;
				}
				break;
			case COLLSTATETYPE.ROUND:
				for(int i = 0; i < m_listMyEffElement.Count; i ++)
				{
					tempobjColl = Global.getEffAtkWaring(ClientMain.m_akkack);
					tempobjColl.transform.position = m_listMyEffElement[i].transform.position;
					tempobjColl.transform.position = new Vector3(tempobjColl.transform.position.x, tempobjColl.transform.position.y + 0.01f, tempobjColl.transform.position.z);
					tempobjColl.transform.parent = m_listMyEffElement[i].transform.parent;
					tempobjColl.transform.localScale = new Vector3(m_iCollValue0 * 2, 1, m_iCollValue0 * 2);
					m_listShowRange.Add(tempobjColl);
//					m_listFanRange.Add(tempobjColl.GetComponent<FanRange>());
//					m_listFanRange[m_listFanRange.Count - 1].SetScale(m_iCollValue0);
//					m_listFanRange[m_listFanRange.Count - 1].SetAngle(360);
					//				m_listMyEffColl.Add();
				}
				break;
			case COLLSTATETYPE.ANGLE:
				for(int i = 0; i < m_listMyEffElement.Count; i ++)
				{
					m_iCollValue1 = 30;
					tempobjColl = Global.getEffAtkWaring(ClientMain.m_akkack);
					tempobjColl.transform.position = node.transform.position;
					tempobjColl.transform.position = new Vector3(tempobjColl.transform.position.x, tempobjColl.transform.position.y + 0.01f, tempobjColl.transform.position.z);
					tempobjColl.transform.localRotation = node.transform.localRotation;
					tempobjColl.transform.parent = node.transform.parent;
					tempobjColl.transform.localScale = new Vector3(m_iCollValue0 * 2, 1, m_iCollValue0 * 2);
					m_listShowRange.Add(tempobjColl);
//					m_listFanRange.Add(tempobjColl.GetComponent<FanRange>());
//					m_listFanRange[m_listFanRange.Count - 1].SetScale(m_iCollValue0);
//					m_listFanRange[m_listFanRange.Count - 1].SetAngle((int)m_iCollValue1);
				}
				break;
			}
		}
	}

	public virtual void activeSkill(int state)
	{
		if(node.gameObject.name == "Node_1" && template.id == 250101)
		{
			Debug.Log(template.id);
			Debug.Log(template.name);
			Debug.Log("技能可关闭");
		}
//		Debug.Log(node.gameObject.name + "activeSkill");
		if(m_listShowRange != null)
		{
			for(int i = 0; i < m_listShowRange.Count; i ++)
			{
				switch(m_CollStateType)
				{
				case COLLSTATETYPE.VECTOR:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
					break;
				case COLLSTATETYPE.ROUND:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
					
//					Destroy(m_listFanRange[i].gameObject);
//					m_listFanRange.RemoveAt(i);
					break;
				case COLLSTATETYPE.ANGLE:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
//					
//					Destroy(m_listFanRange[i].gameObject);
//					m_listFanRange.RemoveAt(i);
					break;
				}
				i --;
			}
		}

//		Debug.Log("m_listMyEffElement.Count="+m_listMyEffElement.Count);
	
		// debug use
		if(!Console_SetBattleFieldFx.IsEnableSkillFx() ){
			return;
		}

		for(int i = 0; i < m_listMyEffElement.Count; i ++)
		{
			if(m_listMyEffElement[i] != null && !m_listMyEffElement[i].activeSelf)
			{
				m_listMyEffCutTime.Add(Time.time);
				m_listMyEffElement[i].SetActive(true);

				AudioSource[] tempAudio = m_listMyEffElement[i].GetComponents<AudioSource>();
				
				for(int q = 0; q < tempAudio.Length; q ++)
				{
					tempAudio[q].Play();
				}
			}
		}
	}
	private int num = 0;
	private Vector3 m_VectorPos;
	public virtual void upData()
	{
		float tempTime = 0;
		num ++;
		List<int> mTimeOver = new List<int> ();
		if(m_isNewSkillType)
		{
			if(node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) >= 0 && node.targetNode != null || m_isDeadOverSkill == 1)
			{
				tempTime = Time.time;
			}
			else
			{
				deleteAllTheEff();
			}
			if(m_isUseThisSkill)
			{
				if((tempTime - cooldown) >= template.timePeriod)
				{
					if(node.gameObject.name == "Node_1" && template.id == 250101 && m_NumStop == 0)
					{
						Debug.Log(template.id);
						Debug.Log(template.name);
						Debug.Log(m_Nummmmm);
						m_NumStop = 1;
						BattleUIControlor.Instance().enterPause();
					}
				}
			}
			if((tempTime - cooldown) >= template.timePeriod 
			   && !template.zhudong
			   && m_iUseNum > 0
			   && !m_isUseThisSkill
			   && node.m_sPlaySkillAnimation == ""
			   && ((
				(m_sAnimName == "null" || 
			 node.IsPlaying() == "Run" || 
			 node.IsPlaying() == "Walk" ||
			 node.IsPlaying() == "Stand0" ||
			 node.IsPlaying() == "Stand1" || 
			 node.isPlayingSwing()) ||
				m_isDeadOverSkill == 1))
			   )
			{
				bool flag = false;
				if(castSkill())
				{
					int tempIndex = node.m_iUseSkillIndex;
					m_isUseThisSkill = true;
					m_vectorMove = false;
					m_AttBaseAI = new List<List<BaseAI>>();
					m_iUseNum --;
					node.m_iUseSkillIndex = m_iIndex;
					node.checkSkillDrama(node.skills[m_iIndex].template.id);
					for(int i = 0; i < m_otherSkill.Count; i ++)
					{
						m_otherSkill[i].m_isUseThisSkill = true;
					}
					//							BaseAI.checkSkillDrama();
//					Debug.Log("设定技能播放名称="+m_sAnimName);
					if(node.gameObject.name == "Node_1" && template.id == 250101)
					{
						m_Nummmmm ++;
					}

					if(m_sAnimName == "null")
					{
						if(template.id / 1000 == 250)//开头是250的技能是主动秘宝技能
						{
							if(node.nodeId == 1)
							{
								BattleUIControlor.Instance().cooldownMibaoSkill.refreshCDTime();
							}
							else if(node.nodeData.nodeType == NodeType.PLAYER && node.stance == BaseAI.Stance.STANCE_ENEMY)
							{
								BattleUIControlor.Instance().cooldownMibaoSkill_enemy.refreshCDTime();
							}
						}
						node.openShow();
						node.activeSkillStart(0);
//						setShowFanRand();
//						activeSkill(0);
						for(int i = 0; i < m_otherSkill.Count; i ++)
						{
							m_otherSkill[i].setShowFanRand();
							m_otherSkill[i].activeSkill(0);
						}
						node.m_iUseSkillIndex = tempIndex;
					}
					else
					{
						node.m_iUseSkillIndex = m_iIndex;
						if(node.IsPlaying().Equals("Stand0") && node.nodeData.nodeType != NodeType.PLAYER)
						{
							node.mAnim.Play("Stand");
						}

						node.stopAllForSkill();
						node.mAnim.SetTrigger (m_sAnimName);
						node.m_sPlaySkillAnimation = m_sAnimName;

//						Debug.Log("设定技能播放名称="+m_sAnimName);
					}
					cooldown = tempTime;
				}
			}
			
			for(int i = 0; i < m_listFirstAI.Count; i ++)//判定每个技能特效里 元素组的第几个攻击时间
			{
				if(m_listFirstAI[i] == null || m_listFirstAI[i].gameObject == null || !m_listFirstAI[i].gameObject.activeSelf)
				{
					Destroy(m_listFirstElement[i]);
					m_listFirstElement.RemoveAt(i);
					m_listFirstTime.RemoveAt(i);
					m_listFirstAI.RemoveAt(i);
					i --;
					continue;
				}
				if((tempTime - m_listFirstTime[i]) > m_iPlayFirstTime)
				{
					Destroy(m_listFirstElement[i]);
					m_listFirstElement.RemoveAt(i);
					m_listFirstTime.RemoveAt(i);
					m_listFirstAI.RemoveAt(i);
					i --;
					continue;
				}
			}
			if(m_PreElement != null && (tempTime - m_PreTime) > m_iPlayPreTime)
			{
				Destroy(m_PreElement);
				m_PreElement = null;
			}
			if(m_isUseThisSkill)
			{
				//Debug.Log(m_listMyEffCutTime.Count);
				for(int i = 0; i < m_listMyEffCutTime.Count; i ++)//判定每个技能特效里 元素组的第几个攻击时间
				{
					if(m_listMyEffElement[i] == null)
					{
//						Debug.Log("null");
						Destroy(m_listMyEffElement[i]);
						m_listMyEffElement.RemoveAt(i);
						m_listMyEffCutTime.RemoveAt(i);
						m_AttBaseAI.RemoveAt(i);
						i --;
						if(m_listMyEffCutTime.Count == 0)
						{
							m_isUseThisSkill = false;
							if(node.gameObject.name == "Node_1" && template.id == 250101)
							{
								Debug.Log("技能结束");
							}
							return;
						}
						continue;
					}
					else if(!m_listMyEffElement[i].activeSelf)
					{
						//Debug.Log(m_listMyEffElement[i].activeSelf);
						if((tempTime - m_listMyEffCutTime[i]) > template.endtime)//结束消失特效
						{
//							Debug.Log("shanchu");
							Destroy(m_listMyEffElement[i]);
							m_listMyEffElement.RemoveAt(i);
							m_listMyEffCutTime.RemoveAt(i);
							m_AttBaseAI.RemoveAt(i);
							i --;
							if(m_listMyEffCutTime.Count == 0)
							{
								m_isUseThisSkill = false;
								if(node.gameObject.name == "Node_1" && template.id == 250101)
								{
									Debug.Log("技能结束");
								}
							}
						}
						continue;
					}

					if(m_CollStateType == COLLSTATETYPE.VECTOR && m_iCollValue0 != 0)
					{
						if(m_isEffLock)
						{
							if(!m_vectorMove)
							{
								m_vectorMove = true;
								m_VectorPos = node.gameObject.transform.position;
								node.moveAction(node.gameObject.transform.position + (m_VForWard * (m_iCollValue0 * template.endtime)), iTween.EaseType.linear, template.endtime, (m_iCollValue4 == 1));
							}
							else
							{
								m_VectorPos = node.gameObject.transform.position;
							}
						}
						else
						{
							m_listMyEffElement[i].gameObject.transform.position += (m_VForWard * m_iCollValue0);
						}
					}
					m_listATTTarget = null;

					if(m_CollStateType == COLLSTATETYPE.VECTOR && m_iCollValue0 != 0)
					{
//						m_listMyEffElement[i].gameObject.transform.position += (m_VForWard * m_iCollValue0);
						m_listATTTarget = getAttListTarget(m_listMyEffElement[i]);
						m_listATTTarget = getNotByAttackNode(m_listATTTarget, i);
						if(m_listATTTarget != null && m_listATTTarget.Count != 0)
						{
							while((m_AttBaseAI[i].Count + m_listATTTarget.Count) > m_iCollValue3)
							{
								m_listATTTarget.RemoveAt(m_listATTTarget.Count - 1);
							}
							for(int q = 0; q < m_SkillDataBead.Count; q ++)
							{
								m_SkillDataBead[q].activeSkill(0);
								addAttackNode(m_listATTTarget, i);
							}
							if(m_iCollValue3 <= m_AttBaseAI[i].Count)
							{
								m_listMyEffCutTime[i] = tempTime - (template.endtime + 1);
							}
						}
					}
					else
					{
						for(int q = 0; q < m_SkillDataBead.Count; q ++)
						{
							if(m_SkillDataBead[q].m_iCurrentIndex == m_SkillDataBead[q].m_fTime.Count)
							{
								continue;
							}
							if((tempTime - m_listMyEffCutTime[i]) > m_SkillDataBead[q].m_fTime[m_SkillDataBead[q].m_iCurrentIndex])
							{
								if(m_listATTTarget == null)
								{
									m_listATTTarget = getAttListTarget(m_listMyEffElement[i]);
									if(m_listATTTarget != null && m_listATTTarget.Count != 0)
									{
										m_SkillDataBead[q].activeSkill(0);
										addAttackNode(m_listATTTarget, i);
										m_listATTTarget = null;
									}
								}
								m_SkillDataBead[q].isNextIndex = true;
							}
						}
					}
					if((tempTime - m_listMyEffCutTime[i]) > template.endtime)//结束消失特效
					{
//						Debug.Log("shanchu");
						Destroy(m_listMyEffElement[i]);
						m_listMyEffElement.RemoveAt(i);
						m_listMyEffCutTime.RemoveAt(i);
						m_AttBaseAI.RemoveAt(i);
						i --;
						if(m_listMyEffCutTime.Count == 0)
						{
							m_isUseThisSkill = false;
							if(node.gameObject.name == "Node_1" && template.id == 250101)
							{
								Debug.Log("技能结束");
							}
						}
						continue;
					}
				}
				for(int i = 0; i < m_SkillDataBead.Count; i ++)
				{
					if(m_SkillDataBead[i].isNextIndex)
					{
						m_SkillDataBead[i].m_iCurrentIndex ++;
//						Debug.Log(m_SkillDataBead[i].m_iCurrentIndex);
						m_SkillDataBead[i].isNextIndex = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// 获得目标人物集.
	/// </summary>
	/// <returns>返回目标人物集</returns>
	/// <param name="distanceType">碰撞类型</param>
	/// <param name="bPos">初始位置</param>
	/// <param name="distance">范围</param>
	/// <param name="listBaseAI">全部人物集 用来筛选</param>
	public List<BaseAI> getListTarget(COLLSTATETYPE distanceType, GameObject effElement,float distance, List<BaseAI> listBaseAI, bool isSelf)
	{
		Vector3 pos1 = effElement.transform.position;
		pos1.y = 0;
		Vector3 pos2;
		for(int i = 0; i < listBaseAI.Count; i ++)
		{
			if(listBaseAI[i]==null)
			{
				listBaseAI.RemoveAt(i);
				i --;
				continue;
			}
			if(listBaseAI[i].gameObject.activeSelf == false && listBaseAI[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) >= 0 )
			{
				listBaseAI.RemoveAt(i);
				i --;
				continue;
			}
			bool tempRemove = false;
			for(int q = 0; q < m_ListProfession.Count; q ++)//职业塞选
			{
				if(m_ListProfession[q] == (int)listBaseAI[i].nodeData.nodeProfession)
				{
					tempRemove = true;
					break;
				}
			}

			for(int q = 0; q < m_ListType.Count; q ++)
			{
				if(m_ListType[q] == (int)listBaseAI[i].nodeData.nodeType)
				{
					tempRemove = true;
					break;
				}
			}
			if(tempRemove)
			{
				listBaseAI.RemoveAt(i);
				i --;
				continue;
			}

			pos2 = listBaseAI[i].gameObject.transform.position;
			pos2.y = 0;
			switch(distanceType)
			{
			case COLLSTATETYPE.ROUND:
				if(Vector3.Distance(pos1, pos2) > distance)//没在距离内删除此对象
				{
					listBaseAI.RemoveAt(i);
					i --;
					continue;
				}
				break;
			case COLLSTATETYPE.ANGLE:
				if(Vector3.Distance(pos1, pos2) > distance)
				{
					listBaseAI.RemoveAt(i);
					i --;
					continue;
				}
				float alpha = Vector3.Angle(listBaseAI[i].gameObject.transform.position - node.transform.position, node.transform.forward);
				if(alpha > m_iCollValue1 / 2)
				{
					listBaseAI.RemoveAt(i);
					i --;
					continue;
				}
				break;
			case COLLSTATETYPE.VECTOR:
				{
					Vector3 p4 = effElement.gameObject.transform.position;
					p4.y = 0;
					Vector3 p0 = p4 - ((m_VForWard * m_iCollValue1) / 2);
					Vector3 p1 = p4 + ((m_VForWard * m_iCollValue1) / 2);

					Vector3 p2 = p4 + (new Vector3(-m_VForWard.z, 0f, m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));
					Vector3 p3 = p4 + (new Vector3(m_VForWard.z, 0f, -m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));

					pos2.y = 0;
					if(!Global.getCollRect(pos2, p0, p1, p2, p3))
					{
						listBaseAI.RemoveAt(i);
						i --;
						continue;
					}
				}
				break;
			}
//			Debug.Log("jinru");
			if((listBaseAI[i].stance == node.stance) == isSelf)//满足敌方或者我方条件
			{
				switch(m_ATTTYPE)
				{
				case ATTTYPE.ONE:
					if(m_iTargetValue0 == 1)
					{//单体如果仅对自己释放 但是此范围内对象不是自己那么删除
						if(listBaseAI[i] != node)
						{
							listBaseAI.RemoveAt(i);
							i --;
							continue;
						}
					}
					else if(m_iTargetValue0 == 2)
					{//单体如果对他人释放 但是此范围内对象是自己那么删除
						if(listBaseAI[i] == node)
						{
							listBaseAI.RemoveAt(i);
							i --;
							continue;
						}
					}
					break;
				case ATTTYPE.SOME:
					if(m_iTargetValue0 == 1)
					{//群体释放目标不包括自己 但是此范围内对象是自己那么删除
						if(listBaseAI[i] == node)
						{
							listBaseAI.RemoveAt(i);
							i --;
							continue;
						}
					}
					break;
				case ATTTYPE.RANDOM:
					break;
				}
			}
			else
			{
				listBaseAI.RemoveAt(i);
				i --;
				continue;
			}

			//是否是死的角色
			if( listBaseAI[i].nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) < 0 ){
				int indexResurrection = -1;

				for(int l = 0; l < m_look.Count; l ++)//遍历技能元素组是否有复活
				{
					if(m_look[l] == SKILLELEMENTTYPE.RESURRECTION)
					{
						indexResurrection = l;
						break;
					}
				}
				if(indexResurrection == -1)//如果没有删除
				{
					listBaseAI.RemoveAt(i);
					i --;
				}
				else
				{
					bool tempRure = false;//如果有遍历ID是否和要复活的ID相等
					SkillResurrection tempSkillRes = (SkillResurrection)m_SkillDataBead[indexResurrection];
					for(int l = 0; l < tempSkillRes.m_iID.Count; l ++)
					{
						if(tempSkillRes.m_iID[l] == listBaseAI[i].nodeId)
						{
							tempRure = true;
							break;
						}
					}
					if(!tempRure)
					{
						listBaseAI.RemoveAt(i);
						i --;
					}
				}
				continue;
			}
			else
			{
				if(m_look.Count == 1 && m_look[0] == SKILLELEMENTTYPE.RESURRECTION)
				{
					listBaseAI.RemoveAt(i);
					i --;
					continue;
				}
			}
		}
		return listBaseAI;
	}
	/// <summary>
	/// 获得最近的一个目标
	/// </summary>
	/// <returns>The latest target.</returns>
	/// <param name="listBaseAI">已经确定的目标集</param>
	public BaseAI getLatestTarget(List<BaseAI> listBaseAI)
	{
		return listBaseAI[Global.getRandom(listBaseAI.Count)];
//		float tempDistace = 0;
//		BaseAI tempReturn = null;
//		for(int i = 0; i < listBaseAI.Count; i ++)
//		{
//			if(Vector3.Distance(listBaseAI[i].gameObject.transform.position, node.gameObject.transform.position) > tempDistace || (listBaseAI[i] == node))
//			{
//				tempReturn = listBaseAI[i];
//			}
//		}
//		return tempReturn;
	}
	/// <summary>
	/// 获得可攻击到的目标
	/// </summary>
	/// <returns>The att list target.</returns>
	public List<BaseAI> getAttListTarget(GameObject effElement)
	{
		List<BaseAI> tempTarget = new List<BaseAI>();
		for(int i = 0; i < BattleControlor.Instance().selfNodes.Count; i ++)
		{
			tempTarget.Add(BattleControlor.Instance().selfNodes[i]);
		}
		for(int i = 0; i < BattleControlor.Instance().reliveNodes.Count; i ++)
		{
			tempTarget.Add(BattleControlor.Instance().reliveNodes[i]);
		}
		for(int i = 0; i < BattleControlor.Instance().enemyNodes.Count; i ++)
		{
			tempTarget.Add(BattleControlor.Instance().enemyNodes[i]);
		}
		for(int i = 0; i < BattleControlor.Instance().midNodes.Count; i ++)
		{
			tempTarget.Add(BattleControlor.Instance().midNodes[i]);
		}
		return getListTarget(m_CollStateType, effElement, m_iCollValue0, tempTarget, m_isRoleSelf);
//		for(int i = 0; i < m_listMyEffElement.Count; i ++)
//		{
//			for(int q = 0; q < tempAddTarget.Count; q ++)
//			{
//				tempReturnTarget.Add(tempAddTarget[q]);
//			}
//		}
	}

	public void addAttackNode(List<BaseAI> baseAI, int index)
	{
		for(int i = 0; i < baseAI.Count; i ++)
		{
			m_AttBaseAI[index].Add(baseAI[i]);
		}
	}

	public List<BaseAI> getNotByAttackNode(List<BaseAI> baseAI, int index)
	{
		for(int i = 0; i < baseAI.Count; i ++)
		{
			for(int q = 0; q < m_AttBaseAI[index].Count; q ++)
			{
				if(baseAI[i] == m_AttBaseAI[index][q])
				{
					baseAI.RemoveAt(i);
					i--;
					break;
				}
			}
		}
		return baseAI;
	}

	public void isFirse(BaseAI ai)
	{
		if(m_iPlayFirstID != 0)
		{
			for(int i = 0; i < m_listFirstAI.Count; i ++)
			{
				if(m_listFirstAI[i] == ai)
				{
					return;
				}
			}
			GameObject tempEffobj = BattleEffectControllor.Instance().getInstantiateEffect(m_iPlayFirstID);
			if( Console_SetBattleFieldFx.IsEnableSkillFx() )
			{
				tempEffobj.SetActive(true);
				tempEffobj.transform.position = ai.gameObject.transform.position;
				tempEffobj.transform.rotation = ai.gameObject.transform.rotation;
				tempEffobj.transform.parent = ai.gameObject.transform;
			}
			m_listFirstElement.Add(tempEffobj);
			m_listFirstTime.Add(Time.time);
			m_listFirstAI.Add(ai);
		}
	}

	public void isPlayPer()
	{
		if(m_iPlayPreID != 0)
		{
			if( Console_SetBattleFieldFx.IsEnableSkillFx() )
			{
				m_PreElement = BattleEffectControllor.Instance().getInstantiateEffect(m_iPlayPreID);
				m_PreElement.SetActive(true);
				m_PreElement.transform.position = node.gameObject.transform.position;
				m_PreElement.transform.rotation = node.gameObject.transform.rotation;
				m_PreElement.transform.parent = node.gameObject.transform;
			}
			m_PreTime = Time.time;
		}
	}

	public void ForcedTermination()//强行终止技能
	{
		if(node.gameObject.name == "Node_1" && template.id == 250101)
		{
			Debug.Log(template.id);
			Debug.Log(template.name);
			Debug.Log("强制终止清空技能播放名称");
		}

		node.m_isStopTime = false;
		node.mAnim.speed = 1;

		m_isUseThisSkill = false;
		if(node.gameObject.name == "Node_1" && template.id == 250101)
		{
			Debug.Log("技能结束");
		}
		node.m_sPlaySkillAnimation = "";
		deleteAllTheEff();
	}

	public void deleteAllTheEff()
	{
		for(int i = 0; i < m_listFirstAI.Count; i ++)//判定每个技能特效里 元素组的第几个攻击时间
		{
			Destroy(m_listFirstElement[i]);
			m_listFirstElement.RemoveAt(i);
			m_listFirstTime.RemoveAt(i);
			m_listFirstAI.RemoveAt(i);
			i --;
		}
		
		if(m_PreElement != null)
		{
			Destroy(m_PreElement);
			m_PreElement = null;
		}

		if(m_listShowRange != null)
		{
			for(int i = 0; i < m_listShowRange.Count; i ++)
			{
				switch(m_CollStateType)
				{
				case COLLSTATETYPE.VECTOR:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
					break;
				case COLLSTATETYPE.ROUND:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
					
//					Destroy(m_listFanRange[i].gameObject);
//					m_listFanRange.RemoveAt(i);
					break;
				case COLLSTATETYPE.ANGLE:
					Destroy(m_listShowRange[i].gameObject);
					m_listShowRange.RemoveAt(i);
					
//					Destroy(m_listFanRange[i].gameObject);
//					m_listFanRange.RemoveAt(i);
					break;
				}
				i --;
			}
		}
		for(int i = 0; i < m_listMyEffElement.Count; i ++)//判定每个技能特效里 元素组的第几个攻击时间
		{
			Destroy(m_listMyEffElement[i]);
			m_listMyEffElement.RemoveAt(i);
			if(m_listMyEffCutTime.Count > i)
			{
				m_listMyEffCutTime.RemoveAt(i);
			}
			i --;
			if(m_listMyEffElement.Count == 0)
			{
				m_isUseThisSkill = false;
				if(node.gameObject.name == "Node_1" && template.id == 250101)
				{
					Debug.Log("技能结束");
				}
			}
		}
	}
}

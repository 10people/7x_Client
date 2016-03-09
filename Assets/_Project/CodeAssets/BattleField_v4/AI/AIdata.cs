//#define ENABLE_ENCRYPT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class AIdata
{
	public enum AttributeType
	{
		ATTRTYPE_moveSpeed = 0,				//移动速度
		ATTRTYPE_attackSpeed = 1,			//攻击速度
		ATTRTYPE_attackRange = 2,			//攻击范围
		ATTRTYPE_eyeRange = 3,				//视野范围
		ATTRTYPE_attackValue = 4,			//攻击力
		ATTRTYPE_defenceValue = 5,			//防御力
		ATTRTYPE_hpMax = 6,					//当前血条血量上限
		ATTRTYPE_hp = 7,					//当前血量
		ATTRTYPE_attackAmplify = 8,			//伤害加深_物理不暴击
		ATTRTYPE_attackReduction = 9,		//伤害减免_物理不暴击
		ATTRTYPE_attackAmplify_cri = 10,	//伤害加深_物理暴击
		ATTRTYPE_attackReduction_cri = 11,	//伤害减免_物理暴击
		ATTRTYPE_skillAmplify = 12,			//伤害加深_技能不暴击
		ATTRTYPE_skillReduction = 13,		//伤害减免_技能不暴击
		ATTRTYPE_skillAmplify_cri = 14,		//伤害加深_技能暴击
		ATTRTYPE_skillReduction_cri = 15,	//伤害减免_技能暴击
		ATTRTYPE_weaponAmplify_Total = 16,	//伤害加深_武器物理通用
		ATTRTYPE_weaponAmplify_Heavy = 17,	//伤害加深_武器物理重武器
		ATTRTYPE_weaponAmplify_Light = 18,	//伤害加深_武器物理轻武器
		ATTRTYPE_weaponAmplify_Range = 19,	//伤害加深_武器物理弓武器
		ATTRTYPE_weaponReduction_Total = 20,//伤害减免_武器物理通用
		ATTRTYPE_weaponReduction_Heavy = 21,//伤害减免_武器物理重武器
		ATTRTYPE_weaponReduction_Light = 22,//伤害减免_武器物理轻武器
		ATTRTYPE_weaponReduction_Range = 23,//伤害减免_武器物理弓武器
		ATTRTYPE_skillAmplify_Total = 24,	//伤害加深_武器技能通用
		ATTRTYPE_skillAmplify_Heavy = 25,	//伤害加深_武器技能重武器
		ATTRTYPE_skillAmplify_Light = 26,	//伤害加深_武器技能轻武器
		ATTRTYPE_skillAmplify_Range = 27,	//伤害加深_武器技能弓武器
		ATTRTYPE_skillReduction_Total = 28,	//伤害减免_武器技能通用
		ATTRTYPE_skillReduction_Heavy = 29,	//伤害减免_武器技能重武器
		ATTRTYPE_skillReduction_Light = 30,	//伤害减免_武器技能轻武器
		ATTRTYPE_skillReduction_Range = 31,	//伤害减免_武器技能弓武器
		ATTRTYPE_criX = 32,					//暴击x
		ATTRTYPE_criY = 33,					//暴击y
		ATTRTYPE_criSkillX = 34,			//技能暴击x
		ATTRTYPE_criSkillY = 35,			//技能暴击y

		ATTRTYPE_Heavy_moveSpeed = 36,		//重武器移动速度
		ATTRTYPE_Heavy_attackSpeed = 37,	//重武器攻击速度
		ATTRTYPE_Heavy_attackRange = 38,	//重武器攻击范围
		ATTRTYPE_Heavy_weaponRatio_1 = 39,	//重武器武器系数_第一击
		ATTRTYPE_Heavy_weaponRatio_2 = 40,	//重武器武器系数_第二击
		ATTRTYPE_Heavy_weaponRatio_3 = 41,	//重武器武器系数_第三击
		ATTRTYPE_Heavy_weaponRatio_4 = 42,	//重武器武器系数_第四击
		ATTRTYPE_Heavy_criX = 43,			//重武器暴击x
		ATTRTYPE_Heavy_criY = 44,			//重武器暴击y
		ATTRTYPE_Heavy_criSkillX = 45,		//重武器技能暴击x
		ATTRTYPE_Heavy_criSkillY = 46,		//重武器技能暴击y

		ATTRTYPE_Light_moveSpeed = 47,		//轻武器移动速度
		ATTRTYPE_Light_attackSpeed = 48,	//轻武器攻击速度
		ATTRTYPE_Light_attackRange = 49,	//轻武器攻击范围
		ATTRTYPE_Light_weaponRatio_1 = 50,	//轻武器武器系数_第一击
		ATTRTYPE_Light_weaponRatio_2 = 51,	//轻武器武器系数_第二击
		ATTRTYPE_Light_weaponRatio_3 = 52,	//轻武器武器系数_第三击
		ATTRTYPE_Light_weaponRatio_4 = 53,	//轻武器武器系数_第四击
		ATTRTYPE_Light_criX = 54,			//轻武器暴击x
		ATTRTYPE_Light_criY = 55,			//轻武器暴击y
		ATTRTYPE_Light_criSkillX = 56,		//轻武器技能暴击x
		ATTRTYPE_Light_criSkillY = 57,		//轻武器技能暴击y

		ATTRTYPE_Range_moveSpeed = 58,		//弓武器移动速度
		ATTRTYPE_Range_attackSpeed = 59,	//弓武器攻击速度
		ATTRTYPE_Range_attackRange = 60,	//弓武器攻击范围
		ATTRTYPE_Range_weaponRatio_1 = 61,	//弓武器武器系数_第一击
		ATTRTYPE_Range_weaponRatio_2 = 62,	//弓武器武器系数_第二击
		ATTRTYPE_Range_weaponRatio_3 = 63,	//弓武器武器系数_第三击
		ATTRTYPE_Range_weaponRatio_4 = 64,	//弓武器武器系数_第四击
		ATTRTYPE_Range_criX = 65,			//弓武器暴击x
		ATTRTYPE_Range_criY = 66,			//弓武器暴击y
		ATTRTYPE_Range_criSkillX = 67,		//弓武器技能暴击x
		ATTRTYPE_Range_criSkillY = 68,		//弓武器技能暴击y

		ATTRTYPE_ReductionBTACDown = 69,	//击倒抗性
		ATTRTYPE_ReductionBTACMove = 70,	//击退抗性
		ATTRTYPE_ReductionBTAC = 71,		//被击抗性
		ATTRTYPE_ReductionDisable = 72,		//击晕抗性

		ATTRTYPE_hpNum = 73,				//血条数量
		ATTRTYPE_hpMaxReal = 74,			//总血量上限
		
		ATTRTYPE_Threat = 75,				//增加的仇恨值，用于BUFF计算

		//////////////////////////////////

		ATTRTYPE_Ice = 76,              	//冰箭射中后的短暂免疫
		ATTRTYPE_Focus = 77,				//轻武器技能2的标记
		ATTRTYPE_NUQI = 78,					//怒气
		ATTRTYPE_isIdle = 100,				//眩晕
		ATTRTYPE_hpDelay = 101,				//减少生命
		ATTRTYPE_Blind = 102,				//致盲，恐惧
		ATTRTYPE_DeleteBuff = 103,			//抵消BUFF
		ATTRTYPE_Sleep = 105,				//沉睡
		ATTRTYPE_Betray = 106,				//魅惑
		ATTRTYPE_ECHO_WEAPON = 108,			//武器反射伤害
		ATTRTYPE_ECHO_SKILL = 109,			//技能反射伤害
	}

	public string nodeName{get{ return _nodeName; } set{ _nodeName = value; }}
	
	public List<int> flagIds{get{ return nodeData.flagIds; } set{ nodeData.flagIds = value; }}
	
	public int modleId{get{ return nodeData.modleId; } set{ nodeData.modleId = value; }}
	
	public NodeType nodeType{get{ return nodeData.nodeType; } set{ nodeData.nodeType = value; }}

	public NodeProfession nodeProfession{get{ return nodeData.nodeProfession; } set{ nodeData.nodeProfession = value; }}

	public List<NodeSkill> skills{get{ return nodeData.skills; } set{ nodeData.skills = value; }}

	public PlayerWeapon weaponHeavy{get{ return nodeData.weaponHeavy; } set{ nodeData.weaponHeavy = value; }}
	
	public PlayerWeapon weaponLight{get{ return nodeData.weaponLight; } set{ nodeData.weaponLight = value; }}
	
	public PlayerWeapon weaponRanged{get{ return nodeData.weaponRanged; } set{ nodeData.weaponRanged = value; }}

	public List<DroppenItem> droppenItems{get{return nodeData.droppenItems; }}

	public int droppenType{get{return nodeData.droppenType; }}


	public Node nodeData;


	private BaseAI node;

	private string _nodeName;


	#region Init

	public void initValue(BaseAI _node, Node _nodeData)
	{
		nodeData = _nodeData;

		node = _node;

		attributes.Clear();

		AddAttribute (nodeData.moveSpeed);//AttrubuteType.ATTRTYPE_moveSpeed
		AddAttribute (nodeData.attackSpeed);//AttrubuteType.ATTRTYPE_attackSpeed
		AddAttribute (nodeData.attackRange);//AttrubuteType.ATTRTYPE_attackRange
		AddAttribute ((CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan 
		               || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao 
		               || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp
		               || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo) ? 1000 : nodeData.eyeRange);//AttrubuteType.ATTRTYPE_eyeRange
		AddAttribute (nodeData.attackValue);//AttrubuteType.ATTRTYPE_attackValue
		AddAttribute (nodeData.defenceValue);//AttrubuteType.ATTRTYPE_defenceValue
		AddAttribute (nodeData.hpMax);//AttrubuteType.ATTRTYPE_hpMax
		AddAttribute (nodeData.hp);//AttrubuteType.ATTRTYPE_hp
		AddAttribute (nodeData.attackAmplify);//AttrubuteType.ATTRTYPE_attackAmplify
		AddAttribute (nodeData.attackReduction);//AttrubuteType.ATTRTYPE_attackReduction
		AddAttribute (nodeData.attackAmplify_cri);//AttrubuteType.ATTRTYPE_attackAmplify_cri
		AddAttribute (nodeData.attackReduction_cri);//AttrubuteType.ATTRTYPE_attackReduction_cri
		AddAttribute (nodeData.skillAmplify);//AttrubuteType.ATTRTYPE_skillAmplify
		AddAttribute (nodeData.skillReduction);//AttrubuteType.ATTRTYPE_skillReduction
		AddAttribute (nodeData.skillAmplify_cri);//AttrubuteType.ATTRTYPE_skillAmplify_cri
		AddAttribute (nodeData.skillReduction_cri);//AttrubuteType.ATTRTYPE_skillReduction_cri
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponAmplify_Total
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponAmplify_Soldier
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponAmplify_Hero
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponAmplify_Boss
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponReduction_Total
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponReduction_Soldier
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponReduction_Hero
		AddAttribute (0);//AttrubuteType.ATTRTYPE_weaponReduction_Boss
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillAmplify_Total
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillAmplify_Soldier
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillAmplify_Hero
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillAmplify_Boss
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillReduction_Total
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillReduction_Soldier
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillReduction_Hero
		AddAttribute (0);//AttrubuteType.ATTRTYPE_skillReduction_Boss
		AddAttribute (nodeData.criX);//AttrubuteType.ATTRTYPE_criX
		AddAttribute (nodeData.criY);//AttrubuteType.ATTRTYPE_criY
		AddAttribute (nodeData.criSkillX);//AttrubuteType.ATTRTYPE_criSkillX
		AddAttribute (nodeData.criSkillY);//AttrubuteType.ATTRTYPE_criSkillY

		if(nodeData.weaponHeavy != null)
		{
			AddAttribute (nodeData.weaponHeavy.moveSpeed);//ATTRTYPE_Heavy_moveSpeed = 36,
			AddAttribute (nodeData.weaponHeavy.attackSpeed);//ATTRTYPE_Heavy_attackSpeed = 37,
			AddAttribute (nodeData.weaponHeavy.attackRange);//ATTRTYPE_Heavy_attackRange = 38,
			AddAttribute (nodeData.weaponHeavy.weaponRatio[0]);//ATTRTYPE_Heavy_weaponRatio = 39,
			AddAttribute (nodeData.weaponHeavy.weaponRatio[1]);//ATTRTYPE_Heavy_weaponRatio = 39,
			AddAttribute (nodeData.weaponHeavy.weaponRatio[2]);//ATTRTYPE_Heavy_weaponRatio = 39,
			AddAttribute (nodeData.weaponHeavy.weaponRatio[3]);//ATTRTYPE_Heavy_weaponRatio = 39,
			AddAttribute (nodeData.weaponHeavy.criX);//ATTRTYPE_Heavy_criX = 40,
			AddAttribute (nodeData.weaponHeavy.criY);//ATTRTYPE_Heavy_criY = 41,
			AddAttribute (nodeData.weaponHeavy.criSkillX);//ATTRTYPE_Heavy_criSkillX = 42,
			AddAttribute (nodeData.weaponHeavy.criSkillY);//ATTRTYPE_Heavy_criSkillY = 43,
		}
		else
		{
			for(int i= 0; i < 11; i++)
			{
				AddAttribute (0);
			}
		}

		if(nodeData.weaponLight != null)
		{
			AddAttribute (nodeData.weaponLight.moveSpeed);//ATTRTYPE_Light_moveSpeed = 44,
			AddAttribute (nodeData.weaponLight.attackSpeed);//ATTRTYPE_Light_attackSpeed = 45,
			AddAttribute (nodeData.weaponLight.attackRange);//ATTRTYPE_Light_attackRange = 46,
			AddAttribute (nodeData.weaponLight.weaponRatio[0]);//ATTRTYPE_Light_weaponRatio = 47,
			AddAttribute (nodeData.weaponLight.weaponRatio[1]);//ATTRTYPE_Light_weaponRatio = 47,
			AddAttribute (nodeData.weaponLight.weaponRatio[2]);//ATTRTYPE_Light_weaponRatio = 47,
			AddAttribute (nodeData.weaponLight.weaponRatio[3]);//ATTRTYPE_Light_weaponRatio = 47,
			AddAttribute (nodeData.weaponLight.criX);//ATTRTYPE_Light_criX = 48,
			AddAttribute (nodeData.weaponLight.criY);//ATTRTYPE_Light_criY = 49,
			AddAttribute (nodeData.weaponLight.criSkillX);//ATTRTYPE_Light_criSkillX = 50,
			AddAttribute (nodeData.weaponLight.criSkillY);//ATTRTYPE_Light_criSkillY = 51,
		}
		else
		{
			for(int i= 0; i < 11; i++)
			{
				AddAttribute (0);
			}
		}

		if(nodeData.weaponRanged != null)
		{
			AddAttribute (nodeData.weaponRanged.moveSpeed);//ATTRTYPE_Range_moveSpeed = 52,
			AddAttribute (nodeData.weaponRanged.attackSpeed);//ATTRTYPE_Range_attackSpeed = 53,
			AddAttribute (nodeData.weaponRanged.attackRange);//ATTRTYPE_Range_attackRange = 54,
			AddAttribute (nodeData.weaponRanged.weaponRatio[0]);//ATTRTYPE_Range_weaponRatio = 55,
			AddAttribute (nodeData.weaponRanged.weaponRatio[1]);//ATTRTYPE_Range_weaponRatio = 55,
			AddAttribute (nodeData.weaponRanged.weaponRatio[2]);//ATTRTYPE_Range_weaponRatio = 55,
			AddAttribute (nodeData.weaponRanged.weaponRatio[3]);//ATTRTYPE_Range_weaponRatio = 55,
			AddAttribute (nodeData.weaponRanged.criX);//ATTRTYPE_Range_criX = 56,
			AddAttribute (nodeData.weaponRanged.criY);//ATTRTYPE_Range_criY = 57,
			AddAttribute (nodeData.weaponRanged.criSkillX);//ATTRTYPE_Range_criSkillX = 58,
			AddAttribute (nodeData.weaponRanged.criSkillY);//ATTRTYPE_Range_criSkillY = 59,
		}
		else
		{
			for(int i= 0; i < 11; i++)
			{
				AddAttribute (0);
			}
		}

		AddAttribute (0);//ATTRTYPE_ReductionBTACDown
		AddAttribute (0);//ATTRTYPE_ReductionBTACMove
		AddAttribute (0);//ATTRTYPE_ReductionBTAC
		AddAttribute (0);//ATTRTYPE_ReductionDisable

		AddAttribute (nodeData.hpNum);//ATTRTYPE_hpNum = 73,血条数量
		AddAttribute (nodeData.hpMax * nodeData.hpNum);//ATTRTYPE_hpMaxReal = 74,总血量上限

		AddAttribute (0);//ATTRTYPE_Ice = 60,
		AddAttribute (0);//ATTRTYPE_Focus = 61,

		AddAttribute (0);

		AddAttribute (nodeData.nuQiZhi);//78,怒气

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan 
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YuanZhu)
		{
			_nodeName = nodeData.nodeName;
		}
		else if(node.nodeId == 1)
		{
			_nodeName = nodeData.nodeName;
		}
		else
		{
			bool have = NameIdTemplate.haveNameIdTemplateByNameId(int.Parse(nodeData.nodeName));

			if(have == true) _nodeName = NameIdTemplate.GetName_By_NameId(int.Parse(nodeData.nodeName));
		}
	}

	public void resetValue()
	{
		initValue (node, nodeData);
	}

	public Node getNodeData()
	{
		return nodeData;
	}


	#endregion



	#region attributes
		
	private List<float> attributes = new List<float>();

	public float GetAttribute( AttributeType p_type ){
		return GetAttribute( (int)p_type );
	}

	public float GetAttribute( int p_index ){
		if( p_index < 0 || p_index >= attributes.Count ){
			Debug.LogError( "Attribute Out of Range: " + p_index + ", " + attributes.Count);

			return 0.0f;
		}

		#if ENABLE_ENCRYPT
		return EncryptTool.Decrypt( attributes[ p_index ] );
		#else
		return attributes[ p_index ];
		#endif
	}

	public void SetAttribute( AttributeType p_type, float p_float ){
		SetAttribute( (int)p_type, p_float );
	}

	public void SetAttribute( int p_index, float p_float ){
		if( p_index < 0 || p_index >= attributes.Count )
		{
			Debug.LogError( "Attribute Out of Range: " + p_index + " - " + p_float );
			
			return;
		}

		#if ENABLE_ENCRYPT
		attributes[ p_index ] = EncryptTool.Encrypt( p_float );
		#else
		attributes[ p_index ] = p_float;
		#endif
		
		if((AttributeType)p_index == AttributeType.ATTRTYPE_eyeRange)
		{
			SphereCollider sc = (SphereCollider)node.GetComponent("SphereCollider");
			
			if(sc != null) sc.radius = GetAttribute(AttributeType.ATTRTYPE_eyeRange);
		}
	}

	public void AddAttribute( float p_float ){
		#if ENABLE_ENCRYPT
		attributes.Add( EncryptTool.Encrypt( p_float ) );
		#else
		attributes.Add( p_float );
		#endif

	}

	public int GetAttributeCount(){
		return attributes.Count;
	}
	
	#endregion
}

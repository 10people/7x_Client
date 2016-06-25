using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff : MonoBehaviour
{
	public AIdata.AttributeType buffType;


	private BaseAI attacker;

	private BaseAI ai;

	private float buffValue;

	private float time;

	private float startTime;

	public float timeFlag;

	public float timePluse;

	public SkillBuff supplement;

	private GameObject m_ObjEff;

	private int dictId;

	private int skillId;


	public static Buff createBuff(BaseAI _ai, AIdata.AttributeType _buffType, float _buffValue, float _time, SkillBuff _supplement = null)
	{
		List<Buff> m_typeListBuff = _ai.getBuffs();//鑾峰彇AI鐨凚UFFS

		for(int i = 0; i < m_typeListBuff.Count; i ++)//妫鏌ヨ韩涓婃槸鍚︽湁鎶垫秷BUFF 骞朵笖姝ｅソ鍙?互鎶垫秷姝?UFF
		{
			if(m_typeListBuff[i].buffType == AIdata.AttributeType.ATTRTYPE_DeleteBuff)
			{
				if(_supplement.m_iBuffType == m_typeListBuff[i].supplement.m_iBuffType)
				{
					m_typeListBuff[i].time = 0.1f;

					return null;
				}
			}
		}

		bool rule = buffsInRule(_ai, _buffType, _buffValue, _time, _supplement);

		if(rule == false) return null;

		Buff buff = (Buff)_ai.gameObject.AddComponent<Buff>();
		
		buff.ai = _ai;
		
		buff.buffType = _buffType;
		
		buff.buffValue = _buffValue;

		buff.time = _time;

		buff.supplement = _supplement;

		buff.startTime = Time.time;

		buff.attacker = null;

		buff.initValue();

		_ai.getBuffs().Add (buff);

		_ai.refreshWeaponDate ();

		if(buff.supplement != null && buff.supplement.m_iEffID != 0)
		{
			if(buff.supplement.m_iEffID != 0)
			{
				buff.m_ObjEff = BattleEffectControllor.Instance().getEffect(buff.supplement.m_iEffID);

				buff.m_ObjEff = GameObject.Instantiate(buff.m_ObjEff) as GameObject;

				buff.m_ObjEff.SetActive(true);

				Vector3 tempPos = _ai.gameObject.transform.position;

				tempPos.y += (_ai.getHeight() * EffectIdTemplate.GetHeight(buff.supplement.m_iEffID));

				buff.m_ObjEff.transform.position = tempPos;

				buff.m_ObjEff.transform.rotation = _ai.gameObject.transform.rotation;

				buff.m_ObjEff.transform.parent = _ai.gameObject.transform;

				BattleEffect be = buff.m_ObjEff.GetComponent<BattleEffect>();

				if(be == null) be = buff.m_ObjEff.AddComponent<BattleEffect>();

				be.updataDodgeOn();

				EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (buff.supplement.m_iEffID);

				if(et.sound.Equals("-1") == false)
				{
					SoundPlayEff spe = buff.m_ObjEff.GetComponent<SoundPlayEff>();
		
					if(spe == null) spe = buff.m_ObjEff.AddComponent<SoundPlayEff>();
		
					spe.PlaySound(et.sound);
				}
			}
		}

		return buff;
	}

	public static Buff createBuffThreat(BaseAI _ai, float _buffValue, float _time, int _dictId, int _skillId)
	{
		Threat threat = null;

		_ai.threatDict.TryGetValue (_dictId, out threat);

		if (threat == null) 
		{
			threat = new Threat();

			_ai.threatDict.Add(_dictId, threat);
		}
		else
		{
			foreach(Buff _buff in _ai.buffs)
			{
				if(_buff.dictId == _dictId && _buff.skillId == _skillId)
				{
					_buff.startTime = Time.time;

					return _buff;
				}
			}
		}

		Buff buff = (Buff)_ai.gameObject.AddComponent<Buff>();
		
		buff.ai = _ai;
		
		buff.buffType = AIdata.AttributeType.ATTRTYPE_Threat;

		buff.buffValue = _buffValue;
		
		buff.time = _time;

		buff.dictId = _dictId;

		buff.skillId = _skillId;

		buff.supplement = null;
		
		buff.startTime = Time.time;
		
		_ai.threatDict [_dictId].actionThreat += _buffValue;

		_ai.getBuffs().Add (buff);

		return buff;
	}

	public static void delBuff(BaseAI _ai, int _type, SkillBuff _supplement = null )
	{
		List<Buff> m_typeListBuff = _ai.getBuffs();//鑾峰彇AI鐨凚UFFS
		
		for(int i = 0; i < m_typeListBuff.Count; i ++)//妫鏌ヨ韩涓婃槸鍚︽湁鎶垫秷BUFF 骞朵笖姝ｅソ鍙?互鎶垫秷姝?UFF
		{
			Buff buff = m_typeListBuff[i];

			if(buff.supplement != null && buff.supplement.m_iBuffType == _type)
			{
				buff.time = 0.1f;
			}
		}
	}

	//return: true-鍙?互鍙犲姞, false-涓嶅彲鍙犲姞锛屽彧鍒锋柊鏃堕棿
	private static bool buffsInRule(BaseAI _ai, AIdata.AttributeType _buffType, float _buffValue, float _time, SkillBuff _supplement)
	{
		if(!_ai.isAlive)
		{
			return false;
		}

		if (_supplement == null) return true;

		foreach(Buff buff in _ai.getBuffs())
		{
			if(sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_Blind)
			   || sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_isIdle)
			   || sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_Sleep)
			   || sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_Betray)
			   ||sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
			   ||sameAttrType(buff.buffType, _buffType, AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
			   )
			{
				buff.startTime = Time.time;

				if(buff.time < _time)
				{
					buff.time = _time;
				}

				return false;
			}

			if(buff.supplement == null) continue;

			if(buff.supplement.getHeroSkill().template.skillType == _supplement.getHeroSkill().template.skillType
			   && _buffType == buff.buffType)
			{
				buff.startTime = Time.time;

				if(buff.time < _time)
				{
					buff.time = _time;
				}

				return false;
			}
		}

		if(_ai.IsPlaying().IndexOf(_ai.getAnimationName(BaseAI.AniType.ANI_DODGE)) != -1)
		{
			if( _buffType == AIdata.AttributeType.ATTRTYPE_Blind
			   || _buffType == AIdata.AttributeType.ATTRTYPE_isIdle
			   || _buffType == AIdata.AttributeType.ATTRTYPE_Sleep
			   || _buffType == AIdata.AttributeType.ATTRTYPE_Betray )
			{
				return false;
			}
		}

		return true;
	}

	//return: true-same, false-not same
	private static bool sameAttrType(AIdata.AttributeType type_1, AIdata.AttributeType type_2, AIdata.AttributeType type)
	{
		if (type_1 == type_2 && type_1 == type) return true;

		return false;
	}

	private void initValue()
	{
		timePluse = 1f;

		if((int)buffType < ai.nodeData.GetAttributeCount() && ai.nodeData.GetAttribute( buffType ) + buffValue < 0 && buffValue < 0)
		{
			if(buffType == AIdata.AttributeType. ATTRTYPE_attackAmplify
			   || buffType == AIdata.AttributeType.ATTRTYPE_attackReduction
			   || buffType == AIdata.AttributeType.ATTRTYPE_attackReduction_cri
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify_cri
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction_cri
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponAmplify_Total
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponAmplify_Heavy
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponAmplify_Light
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponAmplify_Range
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponReduction_Total
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponReduction_Heavy
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponReduction_Light
			   || buffType == AIdata.AttributeType.ATTRTYPE_weaponReduction_Range
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify_Total
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify_Heavy
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify_Light
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillAmplify_Range
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction_Total
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction_Heavy
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction_Light
			   || buffType == AIdata.AttributeType.ATTRTYPE_skillReduction_Range
			   )
			{

			}
			else
			{
				buffValue = -ai.nodeData.GetAttribute( buffType );
			}
		}

		if(buffType == AIdata.AttributeType.ATTRTYPE_moveSpeed)
		{
			if(ai.slowdownable)
			{
				ai.addNavMeshSpeed(buffValue);
			}
			else if(buffValue > 0)
			{
				ai.addNavMeshSpeed(buffValue);
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Blind)
		{
			ai.isBlind = true;

			if(ai.nodeData.nodeType == qxmobile.protobuf.NodeType.PLAYER)
			{
				for(int i = 0; i < 4; i++)
				{
					if(ai.containsParameter("attack_" + i)) ai.mAnim.SetBool("attack_" + i, false);
				}

				(ai as KingControllor).hitCount = 0;
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_hpDelay)
		{
			if(supplement != null)
			{
				timePluse = supplement.m_fValue1;
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_isIdle)
		{
			ai.isIdle = true;

			if(ai.nodeData.nodeType == qxmobile.protobuf.NodeType.PLAYER)
			{
				for(int i = 0; i < 4; i++)
				{
					if(ai.containsParameter("attack_" + i)) ai.mAnim.SetBool("attack_" + i, false);
				}
				
				(ai as KingControllor).hitCount = 0;
			}

			if(BattleControlor.Instance().achivement != null && ai.nodeId == 1)
			{
				BattleControlor.Instance().achivement.ByJiYun();
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Sleep)
		{
			ai.sleep = true;

			if(ai.nodeData.nodeType == qxmobile.protobuf.NodeType.PLAYER)
			{
				for(int i = 0; i < 4; i++)
				{
					if(ai.containsParameter("attack_" + i)) ai.mAnim.SetBool("attack_" + i, false);
				}
				
				(ai as KingControllor).hitCount = 0;
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Betray)
		{
			buffValue = (int)ai.stance;
			
			if(ai.stance == BaseAI.Stance.STANCE_ENEMY)
			{
				BattleControlor.Instance().midNodes.Add(ai);

				BattleControlor.Instance().enemyNodes.Remove(ai);
			}
			else if(ai.stance == BaseAI.Stance.STANCE_SELF)
			{
				BattleControlor.Instance().midNodes.Add(ai);

				BattleControlor.Instance().selfNodes.Remove(ai);
			}

			ai.stance = BaseAI.Stance.STANCE_MID;

			foreach(BaseAI node in BattleControlor.Instance().selfNodes)
			{
				node.setTargetNull();
			}

			foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
			{
				node.setTargetNull();
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_DeleteBuff)
		{

		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
		{
			
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
		{

		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_hpShield)
		{
			float t_att = ai.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hpMaxReal);
			
			ai.nodeData.SetAttribute( buffType, t_att * buffValue );
		}
		else
		{
			float t_att = ai.nodeData.GetAttribute( buffType );

			ai.nodeData.SetAttribute( buffType, t_att + buffValue );
		}
	}

	public static void setAttacker(BaseAI _attacker, Buff buff)
	{
		if (buff != null) buff.setAttacker (_attacker);
	}

	public void setAttacker(BaseAI _attacker)
	{
		attacker = _attacker;
	}

	public void setEffect(GameObject effectObject)
	{
		m_ObjEff = effectObject;
	}

	public float getBuffValue()
	{
		return buffValue;
	}

	void FixedUpdate()
	{
		updateBuff();

		float now = Time.time;

		if(time != 0 && now >= startTime + time)
		{
			BuffEnd();
		}
	}

	private void updateBuff()
	{
		if(ai == null || ai.isAlive == false) return;

		if(buffType == AIdata.AttributeType.ATTRTYPE_hpDelay)
		{
			timeFlag += Time.deltaTime;

			if(timeFlag < timePluse) return;

			timeFlag = 0;

			if(buffValue > 0)
			{
				if(attacker != null && attacker.isAlive && attacker.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) >= 0)
				{
					attacker.attackHp(ai, buffValue, false, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);
				}
				else
				{
					ai.attackHp(ai, buffValue, false, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);
				}
			}
			else 
			{
				ai.addHp(-buffValue);
			}
		}
	}

	private void BuffEnd()
	{
		if(ai == null || ai.isAlive == false) return;

		if(buffType == AIdata.AttributeType.ATTRTYPE_moveSpeed)
		{
			if(ai.slowdownable)
			{
				ai.addNavMeshSpeed(-buffValue);
			}
			else if(buffValue > 0)
			{
				ai.addNavMeshSpeed(-buffValue);
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Blind)
		{
			ai.isBlind = false;
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_hpDelay)
		{

		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_isIdle)
		{
			ai.isIdle = false;
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Sleep)
		{
			ai.sleep = false;
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Betray)
		{
			ai.stance = (BaseAI.Stance)buffValue;

			if(ai.stance == BaseAI.Stance.STANCE_ENEMY)
			{
				BattleControlor.Instance().enemyNodes.Add(ai);

				BattleControlor.Instance().midNodes.Remove(ai);
			}
			else if(ai.stance == BaseAI.Stance.STANCE_SELF)
			{
				BattleControlor.Instance().selfNodes.Add(ai);

				BattleControlor.Instance().midNodes.Remove(ai);
			}

			foreach(BaseAI node in BattleControlor.Instance().selfNodes)
			{
				node.setTargetNull();
			}
			
			foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
			{
				node.setTargetNull();
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_DeleteBuff)
		{

		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_Threat)
		{
			if(ai.threatDict [dictId] != null)
			{
				ai.threatDict [dictId].actionThreat -= buffValue;
			}
		}
		else if(buffType == AIdata.AttributeType.ATTRTYPE_hpShield)
		{
			ai.nodeData.SetAttribute( buffType, 0);
		}
		else if((int)buffType < 100)
		{
			float t_att = ai.nodeData.GetAttribute( buffType );
			
			ai.nodeData.SetAttribute( buffType, t_att - buffValue );
		}

		ai.getBuffs().Remove(this);

		attacker = null;

		Destroy(m_ObjEff);

		Destroy(this);
	}

	public void end()
	{
		BuffEnd ();
	}

}

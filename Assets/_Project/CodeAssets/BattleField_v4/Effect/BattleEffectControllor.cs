using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleEffectControllor : MonoBehaviour
{
//	public enum EffectType
//	{
//		EFFECT_SHANPING = 2,
//
//		EFFECT_ATTACKED_DAO,
//		EFFECT_ATTACKED_QIANG,
//		EFFECT_ATTACKED_GONG,
//
//		EFFECT_KING_ATTACKED_DAO,
//		EFFECT_KING_ATTACKED_QIANG,
//		EFFECT_KING_ATTACKED_GONG,
//
//		EFFECT_KING_ATTACK_DAO_0,
//		EFFECT_KING_ATTACK_DAO_1,
//		EFFECT_KING_ATTACK_DAO_2,
//		EFFECT_KING_ATTACK_DAO_3,
//
//		EFFECT_KING_SKILL_DAO_1,
//		EFFECT_KING_SKILL_DAO_2,
//
//		EFFECT_KING_ATTACK_LIGHT_0,
//		EFFECT_KING_ATTACK_LIGHT_1,
//		EFFECT_KING_ATTACK_LIGHT_2,
//		EFFECT_KING_ATTACK_LIGHT_3,
//		EFFECT_KING_ATTACK_LIGHT_4,
//		EFFECT_KING_ATTACK_LIGHT_5,
//		EFFECT_KING_ATTACK_LIGHT_6,
//		EFFECT_KING_ATTACK_LIGHT_7,
//		EFFECT_KING_ATTACK_LIGHT_8,//23
//		EFFECT_KING_ATTACK_LIGHT_9,
//
//		EFFECT_KING_CESHI_LIGHT_1,//25
//		EFFECT_KING_CESHI_LIGHT_2,//26
//		EFFECT_KING_CESHI_LIGHT_3,
//		EFFECT_KING_CESHI_LIGHT_4,//28
//		EFFECT_KING_CESHI_LIGHT_5,
//		EFFECT_KING_CESHI_LIGHT_6,
//		EFFECT_KING_CESHI_LIGHT_SHIZI,//31
//		EFFECT_KING_CESHI_LIGHT_TUCI,
//		EFFECT_KING_CESHI_LIGHT_XUANFENGZHAN,//33
//		EFFECT_KING_CESHI_LIGHT_SHANGTIAO_ZUO,//34
//		EFFECT_KING_CESHI_LIGHT_SHANGTIAO_YOU,
//		EFFECT_KING_CESHI_LIGHT_QIXUAN,
//		EFFECT_KING_CESHI_LIGHT_LUANWU_1,//37
//		EFFECT_KING_CESHI_LIGHT_LUANWU_2,
//		EFFECT_KING_CESHI_LIGHT_LUANWU_3,
//		EFFECT_KING_CESHI_JUQI,
//		EFFECT_KING_CESHI_HEIPING,
//		EFFECT_KING_CESHI_SKILL_HUAHEN,//42
//		EFFECT_KING_CESHI_SKILL_CHONGJIBO,
//		EFFECT_KING_CESHI_SKILL_QIXUAN,//44
//		EFFECT_KING_CESHI_SKILL_HONGCHA,
//		EFFECT_KING_CESHI_SKILL_BAOQI,//46
//		EFFECT_KING_CESHI_SKILL_WUQICHANRAO_LEFT,
//		EFFECT_KING_CESHI_SKILL_WUQICHANRAO_RIGHT,
//
//		EFFECT_SKILL_JIANYU,//49
//
//		BATTLE_FIELD_TO_REPLACE_0,
//		BATTLE_FIELD_TO_REPLACE_1,
//		BATTLE_FIELD_TO_REPLACE_2,
//		BATTLE_FIELD_TO_REPLACE_3,
//		BATTLE_FIELD_TO_REPLACE_4,
//		BATTLE_FIELD_TO_REPLACE_5,
//		BATTLE_FIELD_TO_REPLACE_6,
//		BATTLE_FIELD_TO_REPLACE_7,
//		BATTLE_FIELD_TO_REPLACE_8,
//		BATTLE_FIELD_TO_REPLACE_9,
//
//		EFFECT_KING_YI_HUANG_QING_LONG_JUE = 60,
//		EFFECT_KING_QIE_HUAN_WU_QI,//61
//		EFFECT_KING_QIAN_LONG_CHU_HAI,
//		EFFECT_KING_JI_SHE,//63
//		EFFECT_KING_BA_HUANG_LIE_RI,
//		EFFECT_WU_QI,
//
//		EFFECT_TIANLEI,//66
//
//		EFFECT_BIAO_XUE,//67
//		EFFECT_BING_JIAN,
//		EFFECT_FANGYU,//69
//		EFFECT_GONGJI,//70
//		EFFECT_JIA_XUE,
//		EFFECT_CHI_YOU_QING_TONG_YIN,
//
//		BATTLE_FIELD_TO_REPLACE_10 = 73,
//		BATTLE_FIELD_TO_REPLACE_11,
//
//		EFFECT_KING_CESHI_HEIPING_2 = 75,
//
//		BATTLE_FIELD_TO_REPLACE_12 = 76,
//	}

	private Dictionary<string, GameObject> dict_effectObj = new Dictionary<string, GameObject>();

	private List<BattleEffect> effects = new List<BattleEffect> ();

	private Dictionary<int, List<BattleEffect>> effectGroup = new Dictionary<int, List<BattleEffect>> ();


	private static BattleEffectControllor m_instance;

	public static BattleEffectControllor Instance() { return m_instance; }

	#region Mono

	void Awake() {
		m_instance = this;
	}
	
	void OnDestroy(){
		m_instance = null;
	}

	#endregion

	public void LoadEffectByModelId( int modelId, EventDelegate.Callback p_callback )
	{
		ModelTemplate mt = ModelTemplate.getModelTemplateByModelId( modelId );

		foreach( int effectId in mt.effects )
		{
			LoadEffectByEffectId( effectId, p_callback );
		}
//		List<int> = 
//		foreach( int effectId in mt.effects ){
//			LoadEffectByEffectId( effectId, p_callback );
//		}
	}

	public List<int> getEffectIdByModelId( int modelId )
	{
		ModelTemplate mt = ModelTemplate.getModelTemplateByModelId( modelId );

		List<int> tempList = new List<int> ();

		foreach( int effectId in mt.effects )
		{
			tempList.Add(effectId);
		}

		return tempList;
	}

	public string getSoundIdByModelId( int modelId )
	{
		ModelTemplate mt = ModelTemplate.getModelTemplateByModelId( modelId );

		return mt.sound;
	}

	public static int GetEffectCount( int p_model_id )
	{
		int t_count = 0;

		ModelTemplate mt = ModelTemplate.getModelTemplateByModelId( p_model_id );

		foreach( int effectId in mt.effects )
		{
			if( effectId == 0 ){
				continue;
			}
			
			t_count++;
		}

		return t_count;
	}

	public Dictionary<string, GameObject> GetEffectDict()
	{
		return dict_effectObj;
	}

	public void LoadEffectByEffectId( int effectId, EventDelegate.Callback p_callback )
	{
		if( effectId == 0 ) return;
 
		{
			TimeHelper.ResetTaggedTime( TimeHelper.CONST_TIME_INFO_CREATE_EFFECT );
		}

		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId ( effectId ).path, 
		                        LoadCallback,
		                        UtilityTool.GetEventDelegateList( p_callback ) );

		{
			TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_CREATE_EFFECT );
		}
	}

	public void LoadCallback(ref WWW p_www, string p_path, Object p_object){
		LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_FX, p_path );

		if( dict_effectObj.ContainsKey( p_path ) ){
			return;
		}

		{
//			float t_delta = UtilityTool.GetTimeInfoDeltaTime( UtilityTool.CONST_TIME_INFO_CREATE_EFFECT );
//
//			Debug.Log( UtilityTool.FloatPrecision( t_delta, 5 ) + " Effect Loaded: " + p_path );

			TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_CREATE_EFFECT );
		}

		dict_effectObj.Add (p_path, p_object as GameObject );
	}

	public GameObject PlayEffect(int effectId, GameObject host )
	{
		return PlayEffect( effectId, host, GetDefaultEffectTime(), Vector3.zero, Vector3.zero );
	}

	public GameObject PlayEffect(int effectId, GameObject host, float _time)
	{
		return PlayEffect (effectId, host, _time, Vector3.zero, Vector3.zero);
	}

	public GameObject PlayEffect(int effectId, Vector3 position, Vector3 forward)
	{
		return PlayEffect (effectId, null, GetDefaultEffectTime(), position, forward);
	}

	public GameObject PlayEffect(int effectId, Vector3 position, Vector3 forward, float _time)
	{
		return PlayEffect (effectId, null, _time, position, forward);
	}

	public static float GetDefaultEffectTime()
	{
		return 5.0f;
	}

	public GameObject PlayEffect(int effectId, GameObject host, float _time, Vector3 position, Vector3 forward)
	{
		EffectIdGroup group = EffectIdGroup.getGroudById (effectId);

		bool canCreate = true;

		int count = 0;

		if(group != null)
		{
			if(effectGroup.ContainsKey(group.id))
			{
				List<BattleEffect> list = null;

				effectGroup.TryGetValue(group.id, out list);

				if(list != null) 
				{
					for(int i = list.Count - 1; i >= 0; i--)
					{
						BattleEffect e = list[i];

						if(e == null)
						{
							list.Remove(e);
						}
					}

					count = list.Count;
				}
			}

			if(count >= group.count)
			{
				canCreate = false;
			}
		}

		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (effectId);

		GameObject temple;

		dict_effectObj.TryGetValue (et.path, out temple);

		if(temple == null)
		{
			Debug.LogError("Never Load Effect With effectId " + effectId);

			return null;
		}

		GameObject effectObject = null;

		if(canCreate == true)
		{
			effectObject = FxHelper.ObtainFreeFxGameObject( et.path, temple );
		}
		else
		{
			return null;
		}

		effectObject.SetActive(true);

		if(host != null)
		{
			effectObject.transform.parent = host.transform;
		}
		else
		{
			effectObject.transform.parent = gameObject.transform;
		}

		effectObject.transform.localScale = temple.transform.localScale;

		BattleEffect effect = (BattleEffect)ComponentHelper.AddIfNotExist( effectObject, typeof(BattleEffect) );

		effect.refreshDate(group, host, _time, position, forward, et.ratio);

		effect.realTime = Time.realtimeSinceStartup;

		Renderer[] rens = effect.gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer ren in rens)
		{
			GameObjectHelper.SetGameObjectLayer( ren.gameObject, 8 );
		}

		effects.Add(effect);

		if(et.sound.Equals("-1") == false)
		{
			SoundPlayEff spe = (SoundPlayEff)ComponentHelper.AddIfNotExist( effectObject, typeof(SoundPlayEff) );

			spe.PlaySound(et.sound);
		}

		if(group != null)
		{
			if(effectGroup.ContainsKey(group.id) == false)
			{
				effectGroup.Add(group.id, new List<BattleEffect>());
			}

			effectGroup[group.id].Add(effect);
		}

		return effect.gameObject;
	}

	void Update ()
	{
		for(int i = effects.Count - 1; i >= 0; i--)
		{
			BattleEffect effect = effects[i];

			if(effect != null && effect.des == false) continue;

			effects.RemoveAt (i);

			if(effect != null)
			{
				if(effect.group != null)
				{
					if(effectGroup.ContainsKey(effect.group.id))
					{
						effectGroup[effect.group.id].Remove(effect);

						effect.group = null;
					}
				}

				effect.destoryEffect();
			}
		}

		float now = Time.realtimeSinceStartup;

		foreach(BattleEffect effect in effects)
		{
			effect.effectUpdate();

			if(effect.group != null)
			{
				float tempTime = now - effect.realTime;

				if(tempTime > effect.group.time)
				{
					if(effectGroup.ContainsKey(effect.group.id))
					{
						effectGroup[effect.group.id].Remove(effect);

						effect.group = null;
					}
				}
			}
		}
	}

	public GameObject getEffect(int id, bool sound = true)
	{
		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (id);

		GameObject temple;

		dict_effectObj.TryGetValue (et.path, out temple);

//		if(sound ==true && et.sound.Equals("-1") == false)
//		{
//			SoundPlayEff spe = temple.GetComponent<SoundPlayEff>();
//
//			if(spe == null) spe = temple.AddComponent<SoundPlayEff>();
//
//			spe.PlaySound(et.sound);
//		}

		return temple;
	}

	public GameObject getInstantiateEffect( int id, bool sound = true )
	{
		if( id == 0 )
		{
			return new GameObject();
		}

		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (id);

		GameObject temple;
		
		dict_effectObj.TryGetValue (et.path, out temple);

		if(temple == null)
		{
			Debug.LogError("------------------    " + id + ", " + et.path);

			return null;
		}

//		GameObject tempObj = GameObject.Instantiate( temple ) as GameObject;

		GameObject tempObj = FxHelper.ObtainFreeFxGameObject( et.path, temple );

//		Debug.Log( "getInstantiateEffect: " + id + " - " + temple.name );

		if( sound == true && et.sound.Equals("-1") == false )
		{
			SoundPlayEff spe = tempObj.GetComponent<SoundPlayEff>();
			
			if(spe == null) spe = tempObj.AddComponent<SoundPlayEff>();

			spe.PlaySound(et.sound);
		}
		
		return tempObj;
	}

	public void removeAllEffect()
	{
		foreach(BattleEffect effect in effects)
		{
			if(effect != null) effect.destoryEffect();
		}

		effects.Clear ();
	}

}

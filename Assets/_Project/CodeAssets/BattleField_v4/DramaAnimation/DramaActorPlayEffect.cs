using UnityEngine;
using System.Collections;

public class DramaActorPlayEffect : DramaActor
{
	public int effectid;

	public Vector3 position;

	public Vector3 foward;

	public float playTime;

	public bool follow;


	void Start()
	{
		actorType = ACTOR_TYPE.EFFECT;
	}

	protected override float func ()
	{
//		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId ( effectid ).path, 
//		                        LoadCallback );

		//if (BattleEffectControllor.Instance () == null)
		{
			LoadEffectDebug();

			return playTime;
		}

//		if(follow == true)
//		{
//			BattleEffectControllor.Instance ().PlayEffect (
//				effectid,
//				gameObject,
//				playTime);
//		}
//		else
//		{
//			BattleEffectControllor.Instance().PlayEffect(
//				effectid,
//				position,
//				foward,
//				playTime);
//		}
//
//		return playTime;
	}

	public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		BattleEffectControllor.Instance().PlayEffect(
			effectid,
			position,
			foward,
			playTime);
	}

	private void LoadEffectDebug()
	{
		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (effectid);

		Global.ResourcesDotLoad (et.path, PlayEffectDebug);
	}

	private void PlayEffectDebug(ref WWW p_www, string p_path, Object p_object)
	{
		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (effectid);

		GameObject temple = (GameObject)p_object;
		
		if(temple == null)
		{
			Debug.LogError("CAN NOT Load Effect With ID " + effectid + " And Path " + et.path);
			
			return;
		}
		
		GameObject effectObject = (GameObject)Instantiate(temple);
		
		effectObject.SetActive(true);
		
		if(follow == true)
		{
			effectObject.transform.parent = gameObject.transform;
		}
		else
		{
			effectObject.transform.parent = gameObject.transform.parent;
		}
		
		effectObject.transform.localScale = temple.transform.localScale;
		
		BattleEffect effect = (BattleEffect)effectObject.AddComponent<BattleEffect>();
		
		effect.refreshDate(null, follow ? gameObject : null, playTime, position, foward);
		
		effect.realTime = Time.realtimeSinceStartup;
		
		if(et.sound.Equals("-1") == false)
		{
			SoundPlayEff spe = effectObject.AddComponent<SoundPlayEff>();
			
			spe.PlaySound(et.sound);
		}
	}

}

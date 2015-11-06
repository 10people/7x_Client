using UnityEngine;
using System.Collections;

public class DramaActorPlaySound : DramaActor
{
	public int soundId;


	void Start()
	{
		actorType = ACTOR_TYPE.SOUND;
	}
	
	protected override float func ()
	{
		SoundPlayEff spe = gameObject.AddComponent<SoundPlayEff>();

		spe.PlaySound(soundId + "");

		return .1f;
	}

}

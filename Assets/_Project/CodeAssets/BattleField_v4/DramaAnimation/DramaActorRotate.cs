using UnityEngine;
using System.Collections;

public class DramaActorRotate : DramaActor
{
	public Vector3 targetRotation;
	
	public float rotateTime;
	
	
	void Start()
	{
		actorType = ACTOR_TYPE.ROTATE;
	}
	
	protected override float func ()
	{
		base.func ();
		
		iTween.RotateTo (gameObject, iTween.Hash(
			"name", "func",
			"rotation", targetRotation,
			"time", rotateTime,
			"easeType", iTween.EaseType.linear
			));
		
		return rotateTime;
	}
	
	protected override bool funcDone ()
	{
		float l = Vector3.Distance (targetRotation, transform.eulerAngles);
		
		if (l < 0.05f) return true;
		
		return true;
	}
}

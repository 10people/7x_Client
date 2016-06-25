using UnityEngine;
using System.Collections;

public class DramaActorMove : DramaActor
{
	public Vector3 targetPosition;

	public float movingTime;


	void Start()
	{
		actorType = ACTOR_TYPE.MOVE;
	}

	protected override float func ()
	{
		base.func ();

		//transform.forward = targetPosition - transform.position;

		iTween.MoveTo (gameObject, iTween.Hash(
			"name", "func",
			"position", targetPosition,
			"time", movingTime,
			"easeType", iTween.EaseType.linear
			));

		return movingTime;
	}

	protected override bool funcDone ()
	{
		float l = Vector3.Distance (targetPosition, transform.position);

		if (l < 0.05f) return true;

		return false;
	}

	protected override void funcForcedEnd()
	{
		iTween.Stop (gameObject);

		DramaActorMove[] coms = gameObject.GetComponents<DramaActorMove>();

		foreach(DramaActorMove dam in coms)
		{
			if(dam.waittingTime > waittingTime)
			{
				return;
			}
		}

		transform.position = targetPosition;
	}

}

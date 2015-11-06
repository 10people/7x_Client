using UnityEngine;
using System.Collections;

public class DramaActorCameraRotate : DramaActor
{
	public Camera targetCamera;

	public Vector3 targetRotation;

	public float rotateTime;


	private bool inMoving;


	void Start()
	{
		inMoving = false;

		actorType = ACTOR_TYPE.CAMERA_ROTATE;
	}

	protected override float func ()
	{
		base.func ();

		inMoving = true;

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
		inMoving = false;

		return true;

		/*
		float l = Vector3.Distance (targetRotation, transform.eulerAngles);
		
		if (l < 0.05f)
		{
			inMoving = false;

			return true;
		}
		
		return false;
		*/
	}

	void Update()
	{
		if (inMoving == false) return;

		targetCamera.transform.eulerAngles = gameObject.transform.eulerAngles;
	}

}

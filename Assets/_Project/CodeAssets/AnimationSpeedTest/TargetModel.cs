using UnityEngine;
using System.Collections;

public class TargetModel : MonoBehaviour
{
	public GameObject modelTemple;

	public string animName;

	public GameObject startFlag;

	public GameObject endFlag;

	public UILabel labelSpeed;


	private float speed;

	private NavMeshAgent nav;

	private AnimationState mAniWalk;


	void Start()
	{
		nav = (NavMeshAgent)gameObject.GetComponent("NavMeshAgent");

		gameObject.transform.position = startFlag.transform.position;

		nav.Resume();

		nav.SetDestination(endFlag.transform.position);

		GameObject gc = (GameObject)Instantiate(modelTemple);

		gc.SetActive(true);

		gc.transform.parent = gameObject.transform;

		gc.transform.localScale = modelTemple.transform.localScale;

		gc.transform.localPosition = Vector3.zero;

		Animation anim = (Animation)gc.GetComponent("Animation");

		anim.Play(animName);

		foreach(AnimationState ani in anim)
		{
			if(ani.name.Equals(animName))
			{
				mAniWalk = ani;

				break;
			}
		}

		if(mAniWalk == null)
		{
			Debug.LogError("ERROR: Can't get AnimationState with animName \"" + animName + "\"");
		}

		speed = 1.0f;

		refreshSpeed();
	}

	public void setSpeedUpBig()
	{
		speed += 0.5f;

		refreshSpeed();
	}

	public void setSpeedUpSmall()
	{
		speed += 0.1f;
		
		refreshSpeed();
	}

	public void setSpeedDownBig()
	{
		speed -= 0.5f;

		refreshSpeed();
	}

	public void setSpeedDownSmall()
	{
		speed -= 0.1f;
		
		refreshSpeed();
	}

	private void refreshSpeed()
	{
		if(speed != 0)
		{
			//mAniWalk.speed = 1.0f / speed;
		
			nav.speed = speed;

			Time.timeScale = 1.0f / speed;
		}

		labelSpeed.text = speed + "";
	}

	void FixedUpdate()
	{
		if(nav.remainingDistance < 1)
		{
			gameObject.transform.position = startFlag.transform.position;

			nav.Resume();

			nav.SetDestination(endFlag.transform.position);
		}
	}

	void LateUpdate()
	{
		updateCamera();
	}

	private void updateCamera()
	{
		Camera.main.transform.position = transform.position + new Vector3(-4, 1.75f, 0);
	}

}

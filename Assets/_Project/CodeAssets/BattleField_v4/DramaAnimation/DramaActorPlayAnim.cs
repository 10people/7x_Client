using UnityEngine;
using System.Collections;

public class DramaActorPlayAnim : DramaActor
{
	public string animName;

	public float playTime;

	public bool m_isStand;


	private Animator anim;

	private float tempTime;

//	private bool act;


	void Start()
	{
		actorType = ACTOR_TYPE.ANIM;
	}

	protected override void funcAfterWait ()
	{
		anim = (Animator)gameObject.GetComponent ("Animator");

		tempTime = 0;

//		act = true;

//		StartCoroutine (clock());
	}

//	IEnumerator clock()
//	{
//		for(;act == true;)
//		{
//			tempTime += Time.deltaTime;
//
//			if(tempTime >= playTime)
//			{
//				if(m_isStand) anim.SetTrigger ("Stand0");
//
//				act = false;
//			}
//
//			yield return new WaitForEndOfFrame ();
//		}
//	}

	protected override float func ()
	{
//		Debug.Log("DramaActorPlayAnim:func " + animName);

//		if(animName.Equals("Run"))
//		{
//			anim.SetFloat("move_speed", 10);
//		}
//		else if(animName.Equals("Stand1"))
//		{
//			anim.Play ("Stand");
//		}
//		else
//		{
			anim.Play (animName);
//		}

		return playTime;
	}

	protected override bool funcDone ()
	{
		if(m_isStand) anim.Play ("Stand0");

		if(animName.Equals("Run"))
		{
			anim.SetFloat("move_speed", 0);
		}

		return true;

//		if (playTime == 0 ||act == false)
//		{
//			return true;
//		}
//
//		return false;
	}

}

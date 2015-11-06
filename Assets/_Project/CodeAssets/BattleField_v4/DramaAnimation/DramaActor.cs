using UnityEngine;
using System.Collections;

public class DramaActor : MonoBehaviour
{
	public enum ACTOR_TYPE
	{
		APPEAR,
		CAMERA_ANIMATION,
		CAMERA_ROTATE,
		MOVE,
		ANIM,
		ALPHABG,
		EFFECT,
		SOUND,
	}

	public float waittingTime;


	[HideInInspector] public ACTOR_TYPE actorType;

	[HideInInspector] public bool actionDone;


	public void action()
	{
		StartCoroutine (_Acton());
	}

	private IEnumerator _Acton()
	{
		actionDone = false;

		funcStart ();

		DramaActorControllor controllor = (DramaActorControllor)gameObject.GetComponent ("DramaActorControllor");

		yield return new WaitForSeconds(waittingTime);

		funcAfterWait ();

		float t = func ();
		
		t = t < .02f ? .02f : t;
		
		yield return new WaitForSeconds(t);

		bool done = false;

		for(;done == false;)
		{
			done = funcDone ();

			if(done == false) yield return new WaitForEndOfFrame();
		}

		actionDone = true;
	}

	protected virtual void funcStart()
	{

	}

	protected virtual void funcAfterWait()
	{

	}

	protected virtual float func ()
	{
		return 0;
	}

	protected virtual bool funcDone ()
	{
		return true;
	}

}

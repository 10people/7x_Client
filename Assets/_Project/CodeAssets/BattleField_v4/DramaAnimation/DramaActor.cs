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
		ROTATE,
		UISprite,
		UILabel,
	}

	public float waittingTime;


	[HideInInspector] public ACTOR_TYPE actorType;

	[HideInInspector] public bool actionDone;


	private bool end;

	private float curTime;


	protected virtual void OnDestroy()
	{

	}

	public void action()
	{
		StartCoroutine (_Acton());
	}

	private IEnumerator _Acton()
	{
		actionDone = false;

		end = false;

		funcStart ();

		DramaActorControllor controllor = gameObject.GetComponent<DramaActorControllor>();

		yield return new WaitForSeconds(waittingTime);

		funcAfterWait ();

		float t = func ();
		
		t = t < .02f ? .02f : t;

		curTime = Time.realtimeSinceStartup;

		for(int i = 0; i < 1;)
		{
			float now = Time.realtimeSinceStartup;

			if(now - curTime < t && end == false)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				i++;
			}
		}

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

	protected virtual void funcForcedEnd ()
	{

	}

	public void _forcedEnd()
	{
		end = true;
	}

	public virtual void log()
	{

	}

}

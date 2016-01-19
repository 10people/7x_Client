using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DramaActorControllor : MonoBehaviour
{
	public int actorFlagId;

	public int actorModelId;

	public string actorName;

	public Vector3 startPosition;

	public Vector3 startRotation;

	public float startScale = 1;

	public bool m_isJuqing;


	private RuntimeAnimatorController tempControllor;

	private List<DramaActor> actors = new List<DramaActor>();

	private DramaStoryBoard storyBoard;

	private Vector3 tempScale;

	void OnDestroy(){
		tempControllor = null;

		storyBoard = null;

		actors.Clear();
	}


	public void init(DramaStoryBoard _storyBoard)
	{
		storyBoard = _storyBoard;

		if(gameObject.GetComponent<SoundPlayEff>() == null)
		{
			gameObject.AddComponent <SoundPlayEff>();
		}
	}

	public void action()
	{
		Component[] coms = gameObject.GetComponents (typeof(DramaActor));

		actors.Clear ();

		foreach(Component com in coms)
		{
			actors.Add((DramaActor)com);
		}

		if(Vector3.Distance(Vector3.zero, startPosition) > .1f)
		{
			transform.position = startPosition;

			transform.eulerAngles = startRotation;
		}

		tempScale = transform.localScale;

		if(startScale > .1f && m_isJuqing == true)
		{
			transform.localScale = new Vector3 (startScale, startScale, startScale);
		}

		startActions ();
	}

	private void startActions()
	{
		foreach(DramaActor act in actors)
		{
			act.action();
		}
	}

	public bool getActionDone()
	{
		foreach(DramaActor da in actors)
		{
			if(da != null && da.actionDone == false) 
			{
				return false;
			}
		}

		return true;
	}

	public DramaStoryBoard getStoryBoard()
	{
		return storyBoard;
	}

	public void actionDone()
	{
		Animator anim = GetComponent<Animator>();
		
		if(anim != null) anim.runtimeAnimatorController = tempControllor;

		transform.localScale = tempScale;

		/* By YuGu:
		 * 
		 * Remove Actor.Destroy, to make DramaDirector able to write storyboard.
		 */
		if ( !DramaDirector.IsDramaPreviewing () ) 
		{
			foreach(DramaActor da in actors)
			{
				Destroy(da);
			}
		}

		actors.Clear ();

		NavMeshAgent nav = gameObject.GetComponent<NavMeshAgent>();
		
		if(nav != null)
		{
			nav.enabled = true;
		}
	}

	public void resetControllor(int modleId)
	{
		Animator anim = GetComponent<Animator>();

		if (anim == null || modleId <= 0) return;

		tempControllor = anim.runtimeAnimatorController;

		anim.runtimeAnimatorController = null;

		Global.ResourcesDotLoad("_3D/Models/BattleField/DramaControllor/DramaControllor_" + modleId,
		                        loadControllorCallback );
	}

	public void loadControllorCallback(ref WWW p_www, string p_path, Object p_object)
	{
		Animator anim = GetComponent<Animator>();
		
		if(p_object == null)
		{
			anim.runtimeAnimatorController = tempControllor;

			return;
		}

		RuntimeAnimatorController con = (RuntimeAnimatorController)p_object;

		anim.runtimeAnimatorController = con;
	}

}

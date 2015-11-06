using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodLabel : UILabel
{

	[HideInInspector] public int state;

	[HideInInspector] public bool strong;


	private List<BloodLabel> list;

	private EventDelegate.Callback m_callback;


	public void Update ()
	{
		//base.Update();

		if (Camera.main == null) return;

		if (Camera.main.gameObject.activeSelf == false) return;

		transform.forward = Camera.main.transform.forward;
	}

	public void showBloodEx(float time, float _ty, List<BloodLabel> _list, EventDelegate.Callback p_callback = null)
	{
		list = _list;

		strong = false;

		m_callback = p_callback;

		StartCoroutine (_Clock(time));

		StartCoroutine (_showBloodEx(time, _ty));
	}

	IEnumerator _Clock(float time)
	{
		state = 0;

		//float timeC = strong ? time * 1.2f : (time + 1.2f);

		float timeC = time + .2f;

		yield return new WaitForSeconds (timeC);
		
		state = 1;

		yield return new WaitForSeconds (time / 2);
		
		state = 2;
	}

	IEnumerator _showBloodEx(float time, float _ty)
	{
		TweenAlpha.Begin (gameObject, 0, 1);

		iTween.MoveTo (gameObject, iTween.Hash(
			"name", "Blood",
			"position", transform.position + new Vector3(Random.value, _ty, Random.value),
			"time", time,
			"easeType", iTween.EaseType.easeOutQuint
			));

		for(;state == 0;)
		{
			yield return new WaitForEndOfFrame ();
		}

		iTween.MoveTo (gameObject, iTween.Hash(
			"name", "Blood",
			"position", transform.position + new Vector3(Random.value, _ty + .5f, Random.value),
			"time", time / 2,
			"easeType", iTween.EaseType.linear
			));

		TweenAlpha.Begin (gameObject, time / 2, 0);

		for(;state == 1;)
		{
			yield return new WaitForEndOfFrame ();
		}

		destroy ();
	}

	public void destroy()
	{
		list.Remove (this);

		gameObject.SetActive (false);

		if (m_callback != null) m_callback ();
	}

}

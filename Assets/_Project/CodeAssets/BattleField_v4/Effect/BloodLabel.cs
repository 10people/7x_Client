//#define USE_ITWEEN



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class BloodLabel : UILabel
{

	[HideInInspector] public int state;

	[HideInInspector] public bool strong;


	private List<BloodLabel> list;

	private EventDelegate.Callback m_callback;

	#region Mono

	private Camera m_cached_camera_main = null;

	private int m_cached_reset_count = -1;

	public void Update ()
	{
		//base.Update();

		m_cached_camera_main = Camera.main;

		if ( m_cached_camera_main == null ){
			return;
		}

		if ( m_cached_camera_main.gameObject.activeSelf == false ){ 
			return;
		}

		transform.forward = m_cached_camera_main.transform.forward;
	}

	#endregion

	public void showBloodEx(float time, float _ty, List<BloodLabel> _list, EventDelegate.Callback p_callback = null)
	{
		list = _list;

		strong = false;

		m_callback = p_callback;

		Update ();

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

		int random = (int)(Random.value * 1000) % BloodLabelControllor.Instance().randomList.Count;

		Vector3 randomVector3 = BloodLabelControllor.Instance().randomList [random];

		randomVector3.y = _ty;

		{
			#if USE_ITWEEN
			iTween.MoveTo (gameObject, iTween.Hash(
				"name", "Blood",
				"position", transform.position + randomVector3,
				"time", time,
				"easeType", iTween.EaseType.easeOutQuint
			));
			#else
			LeanTween.move( gameObject, transform.position + randomVector3, time ).setEase( LeanTweenType.easeOutQuint );
			#endif
		}

		for(;state == 0;)
		{
			yield return new WaitForEndOfFrame ();
		}

		randomVector3.y = _ty + .5f;

		{
			#if USE_ITWEEN
			iTween.MoveTo (gameObject, iTween.Hash(
				"name", "Blood",
				"position", transform.position + randomVector3,
				"time", time / 2,
				"easeType", iTween.EaseType.linear
			));
			#else
			LeanTween.move( gameObject, transform.position + randomVector3, time / 2 ).setEase( LeanTweenType.linear );
			#endif
		}

		TweenAlpha.Begin (gameObject, time / 2, 0);

		for(;state == 1;)
		{
			yield return new WaitForEndOfFrame ();
		}

		destroy ();
	}

	public void destroy()
	{
		if(list != null) list.Remove (this);

		gameObject.SetActive (false);

		if (m_callback != null) m_callback ();
	}

}

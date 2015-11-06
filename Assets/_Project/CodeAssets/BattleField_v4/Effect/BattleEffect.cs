using UnityEngine;
using System.Collections;

public class BattleEffect : MonoBehaviour
{
	public GameObject host;


	[HideInInspector] public bool des;

	[HideInInspector] public EffectIdGroup group;

	[HideInInspector] public float realTime;


	private Vector3 position;

	private Vector3 forward;

	private float time;


	public void refreshDate(EffectIdGroup _group, GameObject _host, float _time, Vector3 _position, Vector3 _forward)
	{
		group = _group;

		host = _host;

		position = _position;

		forward = _forward;

		time = _time;

		des = false;

		effectUpdate();
	}

	public void effectUpdate ()
	{
		if(host != null)
		{
			transform.position = host.transform.position;

			transform.forward = host.transform.forward;
		}
		else
		{
			transform.position = position;

			if(Vector3.Distance(Vector3.zero, forward) > .1f) transform.forward = forward;
		}

		time -= Time.deltaTime;

		if(time < 0)
		{
			des = true;
		}
	}

	public void destoryEffect()
	{
		Destroy(gameObject);
	}

}

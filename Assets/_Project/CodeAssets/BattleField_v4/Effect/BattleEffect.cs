using UnityEngine;
using System.Collections;

public class BattleEffect : MonoBehaviour
{
	public GameObject host;


	[HideInInspector] public bool des;

	[HideInInspector] public EffectIdGroup group;

	[HideInInspector] public float realTime;

	[HideInInspector] public Vector3 offset;


	private Vector3 position;

	private float ratio;

	private Vector3 forward;

	private float time;


	public void refreshDate(EffectIdGroup _group, GameObject _host, float _time, Vector3 _position, Vector3 _forward, float _ratio)
	{
		group = _group;

		host = _host;

		position = _position;

		forward = _forward;

		time = _time;

		ratio = _ratio;

		des = false;

		offset = Vector3.zero;

		effectUpdate();
	}

	public void effectUpdate ()
	{
		if(host != null)
		{
			transform.position = host.transform.position;

			if(ratio != 0)
			{
				BaseAI node = host.GetComponentInChildren<BaseAI>();

				if(node != null)
				{
					transform.position += new Vector3(0, node.getHeight() * ratio, 0);
				}
			}

			transform.forward = host.transform.forward;
		}
		else
		{
			transform.position = position + offset;

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

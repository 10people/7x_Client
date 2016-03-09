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

	private bool selfUpdatePosition = false;

	private bool selfUpdateTime = false;

	private FxCacheItem m_fx_cache = null;


	void OnEnable(){
		m_fx_cache = GetComponent<FxCacheItem>();

		host = null;

		des = false;

		group = null;

		realTime = 0.0f;

		offset = Vector3.zero;

		position = Vector3.zero;

		ratio = 0.0f;

		forward = Vector3.zero;

		time = 0.0f;

		selfUpdatePosition = false;

		selfUpdateTime = false;
	}

	public void refreshDate(EffectIdGroup _group, GameObject _host, float _time, Vector3 _position, Vector3 _forward, float _ratio)
	{
		selfUpdatePosition = false;

		selfUpdateTime = false;

		group = _group;

		host = _host;

		position = _position;

		forward = _forward;

		time = _time;

		ratio = _ratio;

		des = false;

		offset = Vector3.zero;

		updataPosition ();
	}

	public void updateOn()
	{
		selfUpdatePosition = true;
		
		selfUpdateTime = true;
	}

	public void updateOff()
	{
		selfUpdatePosition = false;
		
		//selfUpdateTime = false;
	}

	void Update ()
	{
		//if (selfUpdate == false) return;

		if(selfUpdatePosition) updataPosition ();
		
		if(selfUpdateTime) updataTime ();
	}

	public void effectUpdate ()
	{
		updataPosition ();

		updataTime ();
	}

	private void updataPosition()
	{
		if(host != null)
		{
			transform.position = host.transform.position + offset;
			
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
	}

	private void updataTime()
	{
		time -= Time.deltaTime;
		
		if(time < 0)
		{
			des = true;
			
			destoryEffect();
		}
	}

	public void destoryEffect()
	{
		if( m_fx_cache != null ){
			m_fx_cache.FreeFx();
		}
		else{
//			Debug.LogError( "destoryEffect() ERROR, Should not manual destroy here: " + gameObject );

			Destroy(gameObject);	
		}

//		gameObject.SetActive( false );
	}

	public void OnDestroy(){
//		Debug.LogError( "OnDestroy() Error, Should not manual destroy here: " + gameObject );
	}
}

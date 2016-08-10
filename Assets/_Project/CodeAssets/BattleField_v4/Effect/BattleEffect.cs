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

	private bool selfUpdataDodge = false;

	private FxCacheItem m_fx_cache = null;

	private float m_fTransformY = 0;


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

		selfUpdataDodge = false;
	}

	public void refreshDate(EffectIdGroup _group, GameObject _host, float _time, Vector3 _position, Vector3 _forward, float _ratio)
	{
		selfUpdatePosition = false;

		selfUpdateTime = false;

		selfUpdataDodge = false;

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

	public void updataDodgeOn()
	{
		selfUpdataDodge = true;
	}

	public void setTanstrformY(float y)
	{
		m_fTransformY = y;
	}

	void Update ()
	{
		//if (selfUpdate == false) return;

		if (selfUpdatePosition) updataPosition ();

		if(selfUpdateTime) updataTime ();
	}

	void LateUpdate()
	{
		updataPositionDodge ();
	}

	public void effectUpdate ()
	{
		updataPosition ();

		updataTime ();
	}

	public void effectLateUpdate ()
	{
		//updataPositionDodge ();
	}

	private void updataPosition()
	{
		if(host != null)
		{
			transform.position = host.transform.position + offset;
			
//			if(ratio != 0)
//			{
//				BaseAI node = host.GetComponentInChildren<BaseAI>();
//				
//				if(node != null)
//				{
//					transform.position += new Vector3(0, node.getHeight() * ratio, 0);
//				}
//			}
			
			transform.forward = host.transform.forward;
		}
		else
		{
			transform.position = position + offset;
			
			if(Vector3.Distance(Vector3.zero, forward) > .1f) transform.forward = forward;
		}
		transform.position = new Vector3(transform.position.x, transform.position.y + m_fTransformY, transform.position.z);
	}

	public void updataPositionDodge()
	{
		if (transform.parent == null) return;

		KingControllor kc = transform.parent.gameObject.GetComponent<KingControllor>();

		if(kc != null)
		{
			if(kc.copyObject.activeSelf == true)
			{
				transform.position = kc.copyObject.transform.position;
			}
			else
			{
				transform.position = kc.transform.position;
			}
		}
		transform.position = new Vector3(transform.position.x, transform.position.y + m_fTransformY, transform.position.z);
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
		selfUpdataDodge = false;

		selfUpdatePosition = false;

		selfUpdateTime = false;

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

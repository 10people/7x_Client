using UnityEngine;
using System.Collections;

public class DramaActorAppear : DramaActor
{
	public bool appear;


	private float timeCount;

	private float curAlpha;


	void Start()
	{
		actorType = ACTOR_TYPE.APPEAR;
	}

	protected override void funcStart ()
	{
		if(appear == true)
		{
			setAlpha(0);
		}
		else
		{
			setAlpha(1);
		}
	}

	protected override float func ()
	{
		if(appear == true)
		{
			StartCoroutine(powerUpAction());
		}
		else
		{
			StartCoroutine(powerOffAction());
		}
		
		return 0.35f;	
	}

	IEnumerator powerUpAction()
	{
		timeCount = 0;
		
		float _time = 0.3f;
		
		float defaultAlpha = 1f;
		
		for(;true;)
		{
			float dt = Time.deltaTime;
			
			float alpha = defaultAlpha * timeCount / _time;

			setAlpha(alpha > defaultAlpha ? defaultAlpha : alpha);
			
			timeCount += dt;
			
			if(alpha > defaultAlpha) break;
			
			yield return new WaitForEndOfFrame();
		}
	}
	
	IEnumerator powerOffAction()
	{
		timeCount = 0;
		
		float _time = 0.3f;
		
		float defaultAlpha = 1f;
		
		for(;!end;)
		{
			float dt = Time.deltaTime;
			
			float alpha = defaultAlpha * timeCount / _time;
			
			alpha = defaultAlpha - alpha;
			
			setAlpha(alpha < 0 ? 0 : alpha);
			
			timeCount += dt;
			
			if(alpha < 0) break;
			
			yield return new WaitForEndOfFrame();
		}
	}

	private void setAlpha(float alpha)
	{
		curAlpha = alpha;

		Renderer[] rds = transform.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(m.shader.name.Equals("Mobile/Transparent/Main Texture With Z"))
				{
					m.color = new Color (m.color.r, m.color.g, m.color.b, alpha);
				}
				else if(m.shader.name.Equals("Particles/Alpha Blended"))
				{
					Color mc = m.GetColor("_TintColor");
					
					m.SetColor("_TintColor", new Color (mc.r, mc.g, mc.b, alpha));
				}
				else if(m.shader.name.Equals("Transparent/Diffuse"))
				{
					m.color = new Color (m.color.r, m.color.g, m.color.b, alpha);
				}
				else if(m.shader.name.Equals("Self-Illumin/Diffuse"))
				{
					r.gameObject.SetActive(alpha > 0);
				}
				else if(m.shader.name.Equals("Unlit/Transparent"))
				{
					if(alpha > 0)
					{
						//r.gameObject.layer = 8;
					}
					else
					{
						//r.gameObject.layer = 1;
					}
				}
				else if(m.shader.name.Equals("Custom/Characters/Main Texture High Light"))
				{
					//m.color = new Color (m.color.r, m.color.g, m.color.b, alpha);
				}
			}
		}
	}

	protected override bool funcDone ()
	{
		return true;
	}

	protected override void funcForcedEnd()
	{
		if(appear == true)
		{
			setAlpha(1);
		}
		else
		{
			setAlpha(0);
		}
	}

}

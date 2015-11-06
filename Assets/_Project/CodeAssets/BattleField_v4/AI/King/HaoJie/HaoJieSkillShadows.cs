using UnityEngine;
using System.Collections;

public class HaoJieSkillShadows : MonoBehaviour
{
	private float timeCount;

	private bool inChangeOn;

	private bool inChangeOff;

	private bool inPower;


	public void init(Material t_material)
	{
		inPower = (t_material == null);

		inChangeOn = false;

		inChangeOff = false;

		timeCount = 0;

		if (t_material == null) return;

		Renderer[] rds = transform.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer r in rds)
		{
			r.material = t_material;

			//Material m = r.materials[0];
		}
	}

	public void setAlpha(float alpha)
	{
		Renderer[] rds = transform.GetComponentsInChildren<Renderer>();

		foreach (Renderer r in rds)
		{
			foreach (Material m in r.materials)
			{
				if(r.gameObject.name.Equals("ShadowTemple(Clone)") == true) alpha = alpha * .3922f;

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
			}
		}
	}

	public void powerUp(bool des = false)
	{
		StartCoroutine (powerUpAction(des));
	}

	public void powerOff(bool des = true)
	{
		if (inPower == false) return;

		StartCoroutine (powerOffAction(des));
	}

	IEnumerator powerUpAction(bool des)
	{
		inChangeOn = true;

		inChangeOff = false;

		timeCount = 0;

		float _time = 0.15f;

		float defaultAlpha = 0.8f;

		if (des == true) defaultAlpha = 1f;

		for(;inChangeOn == true;)
		{
			float dt = Time.deltaTime;
			
			float alpha = defaultAlpha * timeCount / _time;
			
			setAlpha(alpha > defaultAlpha ? defaultAlpha : alpha);
			
			timeCount += dt;

			if(alpha > defaultAlpha) break;
			
			yield return new WaitForEndOfFrame();
		}

		inChangeOn = false;

		inPower = true;

		if (des == true) Destroy (this);
	}

	IEnumerator powerOffAction(bool des)
	{
		inChangeOff = true;

		inChangeOn = false;

		timeCount = 0;

		float _time = 0.3f;

		float defaultAlpha = 0.8f;
		
		for(;inChangeOff == true;)
		{
			float dt = Time.deltaTime;
			
			float alpha = defaultAlpha * timeCount / _time;

			alpha = defaultAlpha - alpha;

			setAlpha(alpha < 0 ? 0 : alpha);

			timeCount += dt;

			if(alpha < 0) break;

			yield return new WaitForEndOfFrame();
		}

		inChangeOff = false;

		inPower = false;

		if (des == true)
		{
			gameObject.SetActive(false);
		}
	}

}

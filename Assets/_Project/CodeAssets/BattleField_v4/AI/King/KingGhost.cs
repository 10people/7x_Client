using UnityEngine;
using System.Collections;

public class KingGhost : MonoBehaviour 
{
	private int ghostId;

	private float timeCount;


	public void init(int _ghostId, float time)
	{
		ghostId = _ghostId;

		timeCount = 0;

		StartCoroutine (clock(time));
	}

	IEnumerator clock(float _time)
	{
		bool flag = true;

		float defaultAlpha = ghostId == 0 ? 0.7f : 1;

		for(;flag == true;)
		{
			float dt = Time.deltaTime;

			float alpha = defaultAlpha * timeCount / _time;

			Renderer[] rds = transform.GetComponentsInChildren<Renderer>();
			
			foreach (Renderer r in rds)
			{
				foreach (Material m in r.materials)
				{
					if(m.shader.name.Equals("Mobile/Transparent/Main Texture With Z"))
					{
						m.color = new Color (m.color.r, m.color.g, m.color.b, defaultAlpha - alpha);

						if(m.color.a <= 0)
						{
							flag = false;
						}
					}
				}
			}

			timeCount += dt;

			yield return new WaitForEndOfFrame();
		}

		Destroy (gameObject);
	}

}

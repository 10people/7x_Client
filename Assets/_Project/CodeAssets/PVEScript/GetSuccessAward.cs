using UnityEngine;
using System.Collections;

public class GetSuccessAward : MonoBehaviour {

	public UISprite M_Sprite;

	public UILabel Lb;

	float m_color = 1;

	void Start () {
	 
		Destroy (this.gameObject,1.0f);

	}
	void Update()
	{
		m_color -= Time.deltaTime;
		
		if(m_color <= 0 )
		{
			m_color = 0;
		}
		M_Sprite.alpha = Mathf.Abs (m_color);

		Lb.alpha = Mathf.Abs (m_color);
	}
}

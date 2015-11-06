using UnityEngine;
using System.Collections;

public class BtnShanShuo : MonoBehaviour {
	public UISprite M_Sprite;
	float m_color;
	void Start () {
	
		m_color = M_Sprite.alpha;

	}
	
	// Update is called once per frame
	void Update () {
	
			m_color -= Time.deltaTime*1.5f;
	
	    if(m_color <= -1 )
		{
			m_color = 1;
		}
		M_Sprite.alpha = Mathf.Abs (m_color);
	}
}

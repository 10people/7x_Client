using UnityEngine;
using System.Collections;

public class JianTouMove : MonoBehaviour {

	public bool IsStarMove = false;

	public UISprite M_Sprite;
	public float mTime;
	float m_color = 0;

	void Start () {
		M_Sprite.alpha = 0;
		m_color = M_Sprite.alpha;
		StartCoroutine (m_JiantouMove());
	}
	IEnumerator m_JiantouMove()
	{
		yield return new WaitForSeconds (mTime);

		IsStarMove = true;

	}
	// Update is called once per frame
	void Update () {
	
		if(IsStarMove)
		{
			m_color -= Time.deltaTime*1.5f;
			
			if(m_color <= -1 )
			{
				m_color = 1;
			}
			M_Sprite.alpha = Mathf.Abs (m_color);
		}
	}
}

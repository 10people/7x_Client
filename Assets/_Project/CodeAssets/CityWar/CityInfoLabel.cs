using UnityEngine;
using System.Collections;

public class CityInfoLabel : MonoBehaviour {

	private float m_time = 0.15f;

	private bool m_refresh = false;

	public void MoveLabel (Vector3 pos,bool refresh)
	{
		m_refresh = refresh;

		Hashtable move = new Hashtable();
		move.Add ("time",m_time);
		move.Add ("position",pos);
		move.Add ("islocal",true);
		move.Add ("easetype",iTween.EaseType.linear);
		iTween.MoveTo (gameObject,move);
	}

	void Update ()
	{
		if (m_refresh)
		{
			UILabel label = GetComponent<UILabel> ();

			if (label.alpha > 0)
			{
				label.alpha -= 0.05f;
			}
			else
			{
//				Debug.Log ("Destroy");
				Destroy (gameObject);
			}
		}
	}
}

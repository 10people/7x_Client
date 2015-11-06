using UnityEngine;
using System.Collections;

public class TongBiMoveEffect : MonoBehaviour {

	public void Init()
	{


		Vector3 scale = new Vector3 (2f,2f,2);

		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale,"time",0.2));
		StartCoroutine (Move());
	}
	IEnumerator Move()
	{
		yield return new WaitForSeconds (0.2f);
		Vector3 endposition = new Vector3 (0,100,0);
		iTween.MoveTo(this.gameObject, iTween.Hash("position", endposition, "time",1.50f,"islocal",true));
		Destroy (this.gameObject,1.0f);
	}
}

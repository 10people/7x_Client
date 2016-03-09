using UnityEngine;
using System.Collections;

public class BigMiBaoCardTemp : MonoBehaviour {

	public float time;
	void Start () {
		time = 1.0f;
		CardAnim ();
	}
	

	void CardAnim ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("scale",new Vector3(1.14f,1.14f,1.0f));
		scale.Add ("easetype",iTween.EaseType.easeOutBack);
		scale.Add ("oncomplete","TurnToPiece");
		iTween.ScaleTo(this.gameObject,scale);
	}
}

using UnityEngine;
using System.Collections;

public class RoleAnimation : MonoBehaviour {

	private float time;
	private float size;

	private Vector3 thisPos;

	void Awake(){
		time = 0.4f;
		size = 1.2f;
		
		thisPos = this.transform.localPosition;
	}

	void Start ()
	{

	}

	public void RoleAnimate (bool isAnimate)
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("time",time);
		scale.Add ("islocal",true);
		if (isAnimate)
		{
			scale.Add ("scale",new Vector3(size,size,size));
		}
		else
		{
			scale.Add ("scale",Vector3.one);
		}
		iTween.ScaleTo (this.gameObject,scale);

		Hashtable move = new Hashtable ();
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("time",time);
		move.Add ("islocal",true);
		if (isAnimate)
		{
			move.Add ("position",thisPos - new Vector3(0,20,0));
		}
		else
		{
			move.Add ("position",thisPos);
		}
		iTween.MoveTo (this.gameObject,move);
	}
}

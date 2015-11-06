using UnityEngine;
using System.Collections;

public class DonationFlyNum : MonoBehaviour {
	
	[HideInInspector]public Vector3 position;

	public UILabel disHuFu;

	public UILabel addGongXian;

	public UILabel addContribution;

	void Start ()
	{
		Fly ();
	}
	
	void Fly ()
	{
		Hashtable fly = new Hashtable ();

		fly.Add ("time",1.5f);
		fly.Add ("position",position);
		fly.Add ("islocal",true);
		fly.Add ("easetype",iTween.EaseType.easeOutQuart);
		fly.Add ("oncomplete","FlyEnd");

		iTween.MoveTo (this.gameObject,fly);
	}

	void FlyEnd ()
	{
		Destroy (this.gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class MoveClouds : MonoBehaviour {

	public GameObject t1;
	public GameObject t2;
	public GameObject t3;
	public GameObject t4;
	float m_time = 0.3f;
	Vector3 UpandRight = new Vector3(700,360,0);
	Vector3 UpandLeft = new Vector3(-700,360,0);
	Vector3 DownandRight = new Vector3(700,-360,0);
	Vector3 DownandLeft = new Vector3(-700,-360,0);
	Vector3 Centerpoint = new Vector3(0,0,0);
	void Start () {
		MoveCloud ();
		Destroy (this.gameObject,1.4f);
	}

	void MoveCloud()
	{

		TweenPosition.Begin(t1, m_time,Centerpoint );
		TweenPosition.Begin(t2, m_time,Centerpoint );
		TweenPosition.Begin(t3, m_time,Centerpoint );
		TweenPosition.Begin(t4, m_time,Centerpoint );
		StartCoroutine (CouldMovetoleave());
	}
	IEnumerator CouldMovetoleave()
	{
		yield return new WaitForSeconds (m_time+0.5f);
		TweenPosition.Begin(t1, m_time,UpandLeft );
		TweenPosition.Begin(t2, m_time,UpandRight );
		TweenPosition.Begin(t3, m_time,DownandLeft );
		TweenPosition.Begin(t4, m_time,DownandRight );
	}
}

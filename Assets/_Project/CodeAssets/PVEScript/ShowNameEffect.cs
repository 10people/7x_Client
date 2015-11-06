using UnityEngine;
using System.Collections;

public class ShowNameEffect : MonoBehaviour {

	PveEnemyIfon menemyifo;
	void Start () {

		menemyifo = NGUITools.FindInParents<PveEnemyIfon>(gameObject);
	}

	public void init()
	{

	}

	void OnPress()
	{

		//Debug.Log ("你点击了这个敌人");
		menemyifo.PoPEnemyInfo ();

	}

}

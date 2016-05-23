using UnityEngine;
using System.Collections;

public class LuoLiTestCamera : MonoBehaviour 
{
	public LuoLiTestKing king;


	private Vector3 tempPos;


	void Start () 
	{
		tempPos = transform.position - king.transform.position;
	}
	
	void LateUpdate () 
	{
		Vector3 targetPositon = king.transform.position + tempPos;

		transform.position += (targetPositon - transform.position) / 2f;
	}
}

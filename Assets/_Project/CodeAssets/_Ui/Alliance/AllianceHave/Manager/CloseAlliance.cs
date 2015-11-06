using UnityEngine;
using System.Collections;

public class CloseAlliance : MonoBehaviour {

	private GameObject allianceObj;

	public GameObject thisWindow;

	void Start ()
	{
		allianceObj = GameObject.Find ("My_Union(Clone)");
	}

	//返回
	public void Back ()
	{
		Destroy (thisWindow);
	}

	//关闭
	public void CloseAll ()
	{
		Destroy (allianceObj);
	}
}

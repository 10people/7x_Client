using UnityEngine;
using System.Collections;

public class ShowDropinfo : MonoBehaviour {

	public UILabel Rew_Name;
	public UILabel Rew_Info;

	[HideInInspector]public string mName;
	[HideInInspector]public string mInfo;
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void init()
	{
		Rew_Name.text = mName;
		Rew_Info.text = mInfo;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Temp : MonoBehaviour 
{
	Texture2D a;
	Texture2D b;
	// Use this for initialization
	void Start () 
	{
		a = Resources.Load("a") as Texture2D;
//		b = Resources.Load("b") as Texture2D;
//		mm.SetTrigger("aa");
//		Global.inDrama = true;
//		m_Animation = m_obj.GetComponentInChildren<Animation>();
//		Debug.Log(m_Animation.GetClipCount());
//		m_Animation.wrapMode = WrapMode.Loop;
//		m_Animation.Play("Run");
	}
		
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,1024,1024), a);
//		GUI.DrawTexture(new Rect(0,0,1024,1024), b);
	}
}
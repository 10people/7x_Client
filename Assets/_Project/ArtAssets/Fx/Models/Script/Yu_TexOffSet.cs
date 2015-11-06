using UnityEngine;
using System.Collections;



public class Yu_TexOffSet : MonoBehaviour {
	
	
	
	public Vector2 OffSpeed;
	
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex",new Vector2(OffSpeed.x*Time.time,OffSpeed.y*Time.time));
		GetComponent<Renderer>().material.SetTextureOffset("Normalmap",new Vector2(OffSpeed.x*Time.time,OffSpeed.y*Time.time));
	}
}

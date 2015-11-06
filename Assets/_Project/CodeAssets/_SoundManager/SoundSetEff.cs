using UnityEngine;
using System.Collections;

public class SoundSetEff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SoundManager.getAudioSource(GameObject.Find("3D Layer"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class AutoplaySplashFoam : MonoBehaviour {
	ParticleSystem ps;
	float offsetTime;
	// Use this for initialization
	void Start () {
		offsetTime = Random.Range(0f,5f);
		ps = GetComponent<ParticleSystem>();
		InvokeRepeating("Play",offsetTime,5f);
	}
	
	// Update is called once per frame
	void Play () {
		ps.Play();
	}
}

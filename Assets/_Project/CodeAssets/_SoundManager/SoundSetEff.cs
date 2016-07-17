using UnityEngine;
using System.Collections;

public class SoundSetEff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SoundManager.getAudioSource(gameObject);
	}

}

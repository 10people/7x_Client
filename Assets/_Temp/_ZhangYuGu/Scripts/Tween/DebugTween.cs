using UnityEngine;
using System.Collections;

public class DebugTween : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.RotateBy(gameObject, iTween.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));

		iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
	}
}

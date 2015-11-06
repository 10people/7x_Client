using UnityEngine;
using System.Collections;

public class _Empty : MonoBehaviour {

	public Camera m_camera;

	// Use this for initialization
	void Start () {
		if (m_camera == null) {
			return;
		}

		Debug.Log ( "Pixel Rect: " + m_camera.pixelRect );

		Debug.Log ( "w, h: " + m_camera.pixelWidth + ", " + m_camera.pixelHeight );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class CameraCullingTest : MonoBehaviour 
{

	void Start () 
	{
		Camera[] cl = gameObject.GetComponentsInChildren<Camera>();
		
		foreach(Camera camera in cl)
		{
			Debug.Log("CameraName : " + camera.gameObject.name + ", " + camera.cullingMask);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class CameraCenter : MonoBehaviour {

	public Transform target;

	public float CameraOffset;

	public float CameraLength;

	public Vector3 CameraRotation;

	private float Distance;

	void Start ()
	{
		
	}
	
    void Update() 
	{
		transform.localRotation = Quaternion.Euler( CameraRotation + target.transform.eulerAngles );

		transform.position = target.position + new Vector3(0, CameraOffset, 0);

		transform.Translate( new Vector3( 0, 0, -CameraLength ), Space.Self );
    }
}

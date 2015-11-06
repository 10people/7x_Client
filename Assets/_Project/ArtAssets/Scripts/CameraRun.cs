using UnityEngine;
using System.Collections;

public class CameraRun : MonoBehaviour {

	public Transform target;
	public Vector3 CameraOffset;
    public float smoothTime = 0.3F;
    private float yVelocity = 0.0F;
	private Vector3 TargetPosition;
	private Vector3 CameraPosition;
	private float Distance;
	
	
	
	void Start () {
//	     CameraPosition=transform.position;
//		 TargetPosition=target.position;
//		 Distance=Mathf.Abs(CameraPosition.y-TargetPosition.y);
		 //CameraOffset=new Vector3(2.0f,0,-2.0f);
	}

	public void setCameraPos()
	{
//		CameraPosition=transform.position;
//		TargetPosition=target.position;
//		Distance=Mathf.Abs(CameraPosition.y-TargetPosition.y);
	}
	
    void LateUpdate() {
//        float newPosition = Mathf.SmoothDamp(transform.position.x, target.position.x, ref yVelocity, smoothTime);
//		TargetPosition=target.position;

		if( target != null ){
			transform.position = target.position + CameraOffset;
		}
    }
}

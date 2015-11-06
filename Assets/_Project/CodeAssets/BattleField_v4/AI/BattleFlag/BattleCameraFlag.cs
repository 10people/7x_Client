using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCameraFlag : MonoBehaviour
{
	public int flagId;

	public float radius;

	public Vector3 cameraPosition = new Vector3(0, 6.6f, -10.0f);
	
	public Vector3 cameraRotation = new Vector3( 26.1f, 0, 0 );

	public Vector4 camera4Param = new Vector4 (0, 0, 0, 0);

	public int killMin = 0;

	public int killMax = 1000;

}

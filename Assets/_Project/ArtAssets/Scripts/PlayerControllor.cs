using UnityEngine;
using System.Collections;

public class PlayerControllor : MonoBehaviour {

	public GameObject targetCamera;

	public float moveSpeed;

	public float angleSpeed;


	private CharacterController controllor;     
	     
	private bool keyW;

	private bool keyA;

	private bool keyD;

//	private bool keyS;


	void Start () 
	{
		controllor = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.W)) keyW = true;

		if(Input.GetKeyDown(KeyCode.A)) keyA = true;

		if(Input.GetKeyDown(KeyCode.D)) keyD = true;

//		if(Input.GetKeyDown(KeyCode.S)) keyS = true;


		if(Input.GetKeyUp(KeyCode.W)) keyW = false;
		
		if(Input.GetKeyUp(KeyCode.A)) keyA = false;
		
		if(Input.GetKeyUp(KeyCode.D)) keyD = false;
		
//		if(Input.GetKeyUp(KeyCode.S)) keyS = false;


		float x = 0;

		float z = 0;


		if(keyA == true) x = -1;

		if(keyD == true) x = 1;

		if(keyW == true) z = 1;

		//if(keyS == true) z = -1;


		Vector3 v = new Vector3(x, 0, z);

		movePlayer(v);
	}

	public void movePlayer(Vector3 offset)
	{
		if(Vector3.Distance(offset, Vector3.zero) < 0.02f)
		{
			GetComponent<Animation>().Play("idle");

			return;
		}

		if(GetComponent<Animation>().IsPlaying("pao") == false) GetComponent<Animation>().Play("pao");

		if(targetCamera.transform.localEulerAngles.y != 0)
		{
			GameObject offsetObject =  new GameObject();

			offsetObject.transform.localPosition = offset;

			offsetObject.transform.localEulerAngles = new Vector3(0, 0, 0);

			offsetObject.transform.localScale = new Vector3(1, 1, 1);

			GameObject camObject = new GameObject();

			camObject.transform.localPosition = new Vector3(0, 0, 0);

			camObject.transform.localEulerAngles = Vector3.zero;

			camObject.transform.localScale = new Vector3(1, 1, 1);

			offsetObject.transform.parent = camObject.transform;

			camObject.transform.localEulerAngles = new Vector3(0, targetCamera.transform.localEulerAngles.y, 0);

			offset = offsetObject.transform.position;

			Destroy(camObject);

			Destroy(offsetObject);
		}

		{
			GameObject angleObject = new GameObject();

			angleObject.transform.localScale = new Vector3(1, 1, 1);

			angleObject.transform.localPosition = Vector3.zero;

			angleObject.transform.eulerAngles = gameObject.transform.eulerAngles;

			Vector3 oldAngle = angleObject.transform.eulerAngles;

			angleObject.transform.forward = offset;

			float tar = angleObject.transform.eulerAngles.y;

			float sp = angleSpeed * Time.deltaTime;

			float angle = Mathf.MoveTowardsAngle(oldAngle.y, tar, sp);

			angleObject.transform.eulerAngles = new Vector3(0, angle, 0);

			offset = angleObject.transform.forward;

			Destroy(angleObject);
		}

		float speed = moveSpeed * Time.deltaTime;

		controllor.Move(new Vector3(offset.x * speed, 0, offset.z * speed));
		
		transform.forward = offset;

	}

}

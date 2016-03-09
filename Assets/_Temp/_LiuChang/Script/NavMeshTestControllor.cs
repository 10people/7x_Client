using UnityEngine;
using System.Collections;

public class NavMeshTestControllor : MonoBehaviour 
{
	public GameObject testObject;

	public GameObject targetObject;


	private NavMeshAgent nav;

	private CharacterController cont;

	private bool keyDownW;
	
	private bool keyDownS;
	
	private bool keyDownA;
	
	private bool keyDownD;

	private GameObject offObject;

	private GameObject camObject;


	public void Start()
	{
		nav = testObject.GetComponent<NavMeshAgent>();

		cont = testObject.GetComponent<CharacterController>();
	}

	public void StartTest()
	{
		nav.SetDestination (targetObject.transform.position);
	}

	void Update()
	{
		keyboardListen();
	}

	private void keyboardListen()
	{
		//if(dramaControllor.gameObject.activeSelf == true) return;

		if(Input.GetKey(KeyCode.J))
		{
			StartTest();
		}

		if(Input.GetKey(KeyCode.Space))
		{
			keyDownW = false;
			
			keyDownS = false;
			
			keyDownA = false;
			
			keyDownD = false;
		}
		
		{
			if(Input.GetKeyDown(KeyCode.W)) keyDownW = true;
			
			if(Input.GetKeyDown(KeyCode.S)) keyDownS = true;
			
			if(Input.GetKeyDown(KeyCode.A)) keyDownA = true;
			
			if(Input.GetKeyDown(KeyCode.D)) keyDownD = true;
		}
		
		{
			if(Input.GetKeyUp(KeyCode.W)) keyDownW = false;
			
			if(Input.GetKeyUp(KeyCode.S)) keyDownS = false;
			
			if(Input.GetKeyUp(KeyCode.A)) keyDownA = false;
			
			if(Input.GetKeyUp(KeyCode.D)) keyDownD = false;
		}
		
		{
			Vector3 offset = Vector3.zero;
			
			if(keyDownW) offset += new Vector3(0, 0, 1);
			
			else if(keyDownS) offset += new Vector3(0, 0, -1);
			
			if(keyDownA) offset += new Vector3(-1, 0, 0);
			
			else if(keyDownD) offset += new Vector3(1, 0, 0);
			
			//if(Vector3.Distance(offset, Vector3.zero) > .2f) 
			
			moveCont(offset);
		}
	}

	private void moveCont(Vector3 offset)
	{
		float offsetSize = Vector3.Distance (offset, Vector3.zero);

		if( offsetSize != 0
		   && Camera.main.transform.localEulerAngles.y != 0) //摄像机角度不同时的修正
		{
			if(offObject == null)
			{
				offObject = new GameObject();
			}
			
			offObject.transform.localPosition = offset;
			
			offObject.transform.localEulerAngles = new Vector3(0, 0, 0);
			
			offObject.transform.localScale = new Vector3(1, 1, 1);
			
			if(camObject == null)
			{
				camObject = new GameObject();
			}
			
			camObject.transform.localPosition = new Vector3(0, 0, 0);
			
			camObject.transform.localEulerAngles = Vector3.zero;
			
			camObject.transform.localScale = new Vector3(1, 1, 1);
			
			offObject.transform.parent = camObject.transform;
			
			camObject.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);
			
			offset = offObject.transform.position;
		}

		cont.Move(offset * nav.speed * 2 * Time.deltaTime);
	}

}

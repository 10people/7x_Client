using UnityEngine;
using System.Collections;

public class LuoLiTestControllor : MonoBehaviour 
{
	public LuoLiTestKing king;


	private bool keyDownW;
	
	private bool keyDownS;
	
	private bool keyDownA;
	
	private bool keyDownD;

	private LuoLiTestCamera m_camera;

	private GameObject offObject;

	private GameObject camObject;

	private GameObject angleObject;


	void Start()
	{
		keyDownW = false;
		
		keyDownS = false;
		
		keyDownA = false;
		
		keyDownD = false;

		m_camera = Camera.main.gameObject.GetComponent<LuoLiTestCamera>();
	}

	void Update()
	{
		keyboardListen();
	}

	private void keyboardListen()
	{
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
			
			moveKing(offset.normalized);
		}
	}

	public void moveKing(Vector3 offset)
	{
		float offsetSize = Vector3.Distance (offset, Vector3.zero);
		
		{
			if( offsetSize != 0
			   && m_camera != null 
			   && m_camera.transform.localEulerAngles.y != 0) //摄像机角度不同时的修正
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
				
				camObject.transform.localEulerAngles = new Vector3(0, m_camera.transform.localEulerAngles.y, 0);
				
				offset = offObject.transform.position;
			}
			
			bool playingAttack = king.isPlayingAttack() == true;
			
			if(offsetSize != 0 && playingAttack == false) //转向时的角速度
			{
				if(angleObject == null)
				{
					angleObject = new GameObject();
				}
				
				angleObject.transform.localScale = new Vector3(1, 1, 1);
				
				angleObject.transform.localPosition = Vector3.zero;
				
				angleObject.transform.eulerAngles = king.transform.eulerAngles;
				
				Vector3 oldangle = angleObject.transform.eulerAngles;
				
				angleObject.transform.forward = offset;
				
				float tar = angleObject.transform.eulerAngles.y; 
				
				float sp = 1080 * Time.deltaTime;
				
				float angle = Mathf.MoveTowardsAngle (oldangle.y, tar, sp);
				
				angleObject.transform.eulerAngles = new Vector3(0, angle, 0);
				
				offset = angleObject.transform.forward;
			}
			
			if(king.isPlayingAttack() == true)
			{
				king.move(Vector3.zero);
				
				return;
			}
			
			if(offset.x != 0) offset += new Vector3(offset.x * 0.1f, 0, 0);
			
			king.move(offset);
		}
	}

	public void resetUI()
	{
		LuoLiTestKing.instance ().animEnmey.transform.position = new Vector3 (0f, -.6f, 23f);
	}

}

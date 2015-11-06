using UnityEngine;
using System.Collections;

public class m_Test : MonoBehaviour {

	public float minSIZE = 0.9f;  
	public float maxSIZE = 1.1f; 
	public float sensitivity = 10f;
	
	public GameObject target;  
	public float SIZE = 1.0f; 
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;  
	Vector3 tmp;  
	public float yMinLimit = -20;  
	public float yMaxLimit = 80; 
	public float x = 0.0f;  
	public float y = 0.0f;
	private Vector2 oldPosition1;  
	private Vector2 oldPosition2; 
	
	void Start (){  
		
		Vector2 angles= transform.eulerAngles; 
		x = angles.y; 
		y = angles.x; 
		
		if (GetComponent<Rigidbody>())  
			
			GetComponent<Rigidbody>().freezeRotation = true; 
		
	}        
	void Update (){ 


		SIZE = this.GetComponent<Camera>().orthographicSize;   
		//fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;    
		//fov = Mathf.Clamp(fov, minFov, maxFov);    

		//Debug.Log("Input.touchCount =  " +Input.touchCount);
		if(Input.touchCount == 1)         
		{     
			Debug.Log("y =  " +y);
			if(Input.GetTouch(0).phase==TouchPhase.Moved)  
			{              
				x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;           
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;         
				y = ClampAngle(y,yMinLimit,yMaxLimit);   
				
			}  
			
		}   
		if(Input.touchCount >1 )   
		{   
			if(Input.GetTouch(0).phase==TouchPhase.Moved||Input.GetTouch(1).phase==TouchPhase.Moved)
			{ 
				Vector3 tempPosition1= Input.GetTouch(0).position;   
				Vector3 tempPosition2= Input.GetTouch(1).position;   
				if(isEnlarge(oldPosition1,oldPosition2,tempPosition1,tempPosition2))   
				{                         
					if(SIZE > maxSIZE)   
					{                           
						SIZE -= 0.01f;      
					}              
				}else  {   
					if(SIZE < minSIZE)   
					{   
						SIZE += 0.01f;   
					}   
				}   
				SIZE = Mathf.Clamp(SIZE, minSIZE, maxSIZE);  
				this.GetComponent<Camera>().orthographicSize = SIZE;
				oldPosition1=tempPosition1;   
				oldPosition2=tempPosition2;   
			}   
		}   
	}   
	
	bool isEnlarge ( Vector2 oP1 ,  Vector2 oP2 ,  Vector2 nP1 ,  Vector2 nP2  )   
	{   
		float leng1=Mathf.Sqrt((oP1.x-oP2.x)*(oP1.x-oP2.x)+(oP1.y-oP2.y)*(oP1.y-oP2.y));   
		float leng2=Mathf.Sqrt((nP1.x-nP2.x)*(nP1.x-nP2.x)+(nP1.y-nP2.y)*(nP1.y-nP2.y));   
		if(leng1<leng2)   
		{   
			return true;    
		}else   
		{   
			return false;    
		}   
	}  //Unity3D教程手册：www.unitymanual.com 084         
	public void LateUpdate (){   
//		if (target) {     
//			ClampAngle(y, yMinLimit, yMaxLimit);   
//			Quaternion rotation= Quaternion.Euler(y, x, 0);  
//			tmp.Set(0.0f, 0.0f, (-1)*distance);    
//			Vector3 position= rotation * tmp + target.transform.position;  
//			transform.rotation = rotation;   
//			transform.position = position;   
//		}   
	}   
	static float ClampAngle ( float angle ,   float min ,   float max  ){   
		if (angle < -360)   
			angle += 360;   
		if (angle > 360)   
			angle -= 360;   
		return Mathf.Clamp (angle, min, max);   
	}   
}

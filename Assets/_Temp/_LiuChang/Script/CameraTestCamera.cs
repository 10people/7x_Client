using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraTestCamera : MonoBehaviour
{
	public int section;//章

	public int level;//节

	public GameObject cameraFlagRoot;

	public GameObject cameraTarget;


	private List<BattleCameraFlag> flags = new List<BattleCameraFlag>();

	private Vector3 m_cam_position;
	
	private Vector3 m_cam_rotation;
	
	private Vector3 cur_cam_position;
	
	private Vector3 cur_cam_rotation;
	
	private int curCameraId;

	private int elasticCount;


	void Start ()
	{
		m_cam_position = new Vector3 ( 0, 5.2f, -9f );
		
		cur_cam_position = new Vector3 ( 0, 5.2f, -9f );

		m_cam_rotation = new Vector3( 18f, 0, 0 );
		
		cur_cam_rotation = new Vector3( 18f, 0, 0 );

		LoadCameraFlag ();
	}

	private void LoadCameraFlag()
	{
		BattleCameraFlagTemplate.SetLoadDoneCallback( LoadCameraFlagDone );
		
		BattleCameraFlagTemplate.LoadTemplates(100000 + section * 100 + level);
	}

	private void LoadCameraFlagDone()
	{
		foreach(BattleCameraFlagTemplate template in BattleCameraFlagTemplate.templates)
		{
			GameObject gc = new GameObject();
			
			gc.transform.parent = cameraFlagRoot.transform;
			
			gc.name = "CameraFlag_" + template.flagId;
			
			gc.transform.localScale = new Vector3(1, 1, 1);
			
			gc.transform.position = new Vector3(template.x, template.y, template.z);
			
			BattleCameraFlag bf =  (BattleCameraFlag)gc.AddComponent<BattleCameraFlag>();
			
			bf.flagId = template.flagId;
			
			bf.radius = template.radius;
			
			//bf.priority = template.priority;
			
			bf.cameraPosition = new Vector3(template.px, template.py, template.pz);
			
			bf.cameraRotation = new Vector3(template.rx, template.ry, template.rz);

			bf.killMin = template.killMin;

			bf.killMax = template.killMax;

			flags.Add(bf);
		}
	}

	void LateUpdate()
	{
		updateCameraFlags ();
		
		updateCamera();
	}

	public void updateCameraFlags()
	{
		if (flags.Count == 0) return;
		
		int camCount = 0;
		
		BattleCameraFlag tempCamera = null;

		for(int i = 0; i < flags.Count; i++)
		{
			BattleCameraFlag flag = flags[i];
			
			float radius = flag.radius;
			
			if(Vector3.Distance(cameraTarget.transform.position, flag.transform.position) <= radius)
			{
				if(tempCamera == null) tempCamera = flag;
				
//				if(tempCamera.priority < flag.priority) tempCamera = flag;
//				
//				else if(tempCamera.priority == flag.priority) camCount ++;
			}
		}
		
		if(camCount == 1 && tempCamera.flagId != curCameraId)
		{
			cur_cam_position = tempCamera.cameraPosition;
			
			cur_cam_rotation = tempCamera.cameraRotation;
			
			CameraChange (tempCamera.cameraPosition, tempCamera.cameraRotation);
			
			curCameraId = tempCamera.flagId;
			
			elasticCount = 20;
		}
	}

	private void CameraChange(Vector3 _positon, Vector3 _rotation)
	{
		m_cam_position = _positon;
		
		m_cam_rotation = _rotation;
	}

	private void updateCamera()
	{
		Camera cam = gameObject.GetComponent<Camera> ();

		Vector3 targetPos = cameraTarget.transform.position + m_cam_position;
		
		Vector3 targetRotation = m_cam_rotation;
		
		float l = Vector3.Distance(targetPos, transform.position);
		
		if(l > 50)
		{
			transform.eulerAngles = targetRotation;
			
			transform.position = targetPos;
		}
		else
		{
			float t = targetRotation.y - transform.eulerAngles.y;
			
			if(Mathf.Abs(targetRotation.y - 360f - transform.eulerAngles.y) < Mathf.Abs(t)) targetRotation += new Vector3(0, -360f , 0);
			
			if(Mathf.Abs(targetRotation.y + 360f - transform.eulerAngles.y) < Mathf.Abs(t)) targetRotation += new Vector3(0, 360f , 0);
			
			transform.eulerAngles += (targetRotation - transform.eulerAngles) / elasticCount;
			
			transform.position += (targetPos - transform.position) / elasticCount;
		}
		
		//transform.eulerAngles = targetRotation;
		
		elasticCount --;
		
		elasticCount = elasticCount < 5 ? 5 : elasticCount;
	}

}

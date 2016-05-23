using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingCamera : MonoBehaviour 
{
	public enum ShakeType
	{
		Vertical = 0,//竖着震的，比如砸地那一下
		Forward = 1,//前后震动，比如双刀的普攻
		Cri = 2,//横向小幅震动
	}


	public GameObject cameraFlagRoot;

	public GameObject uiCamera;

	public RuntimeAnimatorController shakeControllor;

	public GameObject childCamera;


	[HideInInspector] public GameObject target;


	private List<BattleCameraFlag> flags = new List<BattleCameraFlag>();

	private Vector3 shake_offset;

	private bool m_shake;

	private bool cameraLock;

	private bool m_initialized = false;

	private Vector3 m_cam_position;

	private Vector3 m_cam_rotation;

	private Vector3 cur_cam_position;
	
	private Vector3 cur_cam_rotation;

	private Vector4 cur_cam_parm;

	private int curCameraId;

	private float elasticCount;

	private List<BattleCameraFlag> listCurCam = new List<BattleCameraFlag> ();
	
	private List<float> listCurLength = new List<float> ();
	
	private float totalLength = 0;

	void OnDestroy(){
		shakeControllor = null;
	}

	public void init ()
	{
		cameraLock = false;

		m_cam_position = new Vector3 ( 0, 5.2f, -9f );

		cur_cam_position = new Vector3 ( 0, 5.2f, -9f );

		//m_cam_rotation = new Vector3( 23f, 0, 0 );

		m_cam_rotation = new Vector3( 18f, 0, 0 );

		cur_cam_rotation = new Vector3( 18f, 0, 0 );

		cur_cam_parm = Vector4.zero;

		curCameraId = 0;

		elasticCount = 5f;

		loadFlags ();

		//StartCoroutine (elasticAction());
	}

	public static GameObject getChildCamera()
	{
		return Camera.main.GetComponent<KingCamera>().childCamera;
	}

	public void loadFlags()
	{
		flags.Clear ();

		Component[] fs = cameraFlagRoot.GetComponentsInChildren(typeof(BattleCameraFlag));
		
		foreach(Component fl in fs)
		{
			BattleCameraFlag bf = (BattleCameraFlag) fl;
			
			flags.Add(bf);
		}
	}

	void LateUpdate()
	{
		if (DramaControllor.Instance() != null && DramaControllor.Instance().getInDrama () == true) return;

		updateCameraFlags ();

		updateCamera();

		{
			ConsoleTool.Instance().ManualLateUpdate();
		}
	}

	public void targetChang(GameObject _target)
	{
		if (_target == null) _target = BattleControlor.Instance().getKing ().gameObject;

		if (_target.Equals (target)) return;

		GameObject tempTarget = target;

		target = _target;

		if(tempTarget != null)
		{
			AudioListener al = tempTarget.GetComponent<AudioListener>();

			Destroy(al);
		}
		else
		{
			AudioListener al = ClientMain.m_ClientMainObj.GetComponent<AudioListener>();

			Destroy(al);
		}

		target.AddComponent<AudioListener>();
	}

	public void setDebugCameraValue()
	{
		float x = cur_cam_parm.x;

		float y = cur_cam_parm.y;

		float length = cur_cam_parm.z;
		
		float offset = cur_cam_parm.w;

//		GameObject gc = new GameObject ();
//
//		gc.transform.parent = target.transform;
//
//		gc.transform.position = transform.position;
//
//		gc.transform.eulerAngles = transform.eulerAngles;
//
//		x = gc.transform.localEulerAngles.x;
//
//		y = gc.transform.localEulerAngles.y;
//
//		for(int i = 0; ; i++)
//		{
//			Vector3 temp = gc.transform.forward * 0.1f;
//
//			gc.transform.position += temp;
//
//			length += Vector3.Distance(temp, Vector3.zero);
//
//			if(Mathf.Abs(gc.transform.position.x - target.transform.position.x) < 0.1f)
//			{
//				break;
//			}
//		}
//
//		offset = gc.transform.position.x - target.transform.position.x;

		DebugCameraAdjust.Instance().SetValue( x, y, length, offset );

//		Destroy (gc);
	}

	public void CameraChange(Vector4 parm)
	{
		Component kc = null;

		if(target != null) kc = target.GetComponent ("KingControllor");

		if (kc == null) return;

		float x = parm.x;

		float y = parm.y;

		float length = parm.z;

		float offset = parm.w;

		GameObject gc = new GameObject ();

		gc.transform.parent = transform.parent;

		gc.transform.position = target.transform.position;

		gc.transform.forward = target.transform.forward;

		gc.transform.localEulerAngles += new Vector3 (-x, -y, 0);

		gc.transform.position += gc.transform.forward * length;

		gc.transform.forward = target.transform.forward;
		
		gc.transform.localEulerAngles += new Vector3 (x, y, 0);

		gc.transform.forward = target.transform.position - gc.transform.position;

		gc.transform.position += new Vector3(0, offset, 0);

		gc.transform.parent = target.transform;

		m_cam_position = gc.transform.localPosition;

		m_cam_rotation = gc.transform.localEulerAngles;

		cur_cam_parm = parm;

		Destroy (gc);
	}

	public void CameraChange(Vector3 _positon, Vector3 _rotation)
	{
		m_cam_position = _positon;
		
		m_cam_rotation = _rotation;

//		{
//			if( target != null ){
//				Debug.Log( target.gameObject.name + " Target.Pos: " + target.transform.position );
//			}
//
//			Debug.Log( "Cam.Pos: " + m_cam_position );
//
//			Debug.Log( "Cam.Rotation: " + m_cam_rotation );
//		}
	}

	public void resetCamera()
	{
		//Camera cam = (Camera)gameObject.GetComponent ("Camera");
		
		//cam.cullingMask = 1280;

		targetChang (BattleControlor.Instance().getKing ().gameObject);

		CameraChange (cur_cam_position, cur_cam_rotation);
	}

	public void writeXml()
	{
		foreach(BattleCameraFlag flag in flags)
		{
			if(flag.flagId == curCameraId)
			{
				updateCameraFlag(flag);

				DebugCameraAdjust.Instance().saveCameraSucc();

				return;
			}
		}
	}

	private void updateCameraFlag(BattleCameraFlag flag)
	{
		flag.cameraPosition = m_cam_position;

		flag.cameraRotation = m_cam_rotation;

		flag.camera4Param = DebugCameraAdjust.Instance().get4Param();

		BattleFlagWritor writor = (BattleFlagWritor)flag.transform.parent.gameObject.AddComponent <BattleFlagWritor>();

		writor.rewriteCameraFlag ();

		Destroy (writor);
	}

	public Vector3 getCamPosition()
	{
		return m_cam_position;
	}

	public Vector3 getCamRotation()
	{
		return m_cam_rotation;
	}

	public void updateCamera()
	{
		if (BattleControlor.Instance().getKing () == null) return;

//		if(BattleControlor.Instance().lastCameraEffect == true || cameraLock == true) 
//		{
//			cameraLock = true;
//
//			return;
//		}

		Camera cam = gameObject.GetComponent<Camera> ();

		if(BattleControlor.Instance().inCameraEffect_camera == false)
		{
			if(target == null) 
			{
				if(BattleControlor.Instance().inDrama == true)
				{
					return;
				}
				else
				{
					targetChang(BattleControlor.Instance().getKing ().gameObject);
				}
			}

			//if(target == null) return;

			BaseAI node = target.GetComponent<BaseAI>();

			if(node != null && node.isAlive == false)
			{
				return;
			}

//			if(!CityGlobalData.dramable && !m_initialized && DebugCameraAdjust.Instance() != null)
//			{
//				DebugCameraAdjust.Instance().m_target = gameObject;
//
//				setDebugCameraValue();
//
//				m_initialized = true;
//			}

			Vector3 targetPos = target.transform.position + m_cam_position;

			if(BattleControlor.Instance().lastCameraEffect == true)
			{
				targetPos = targetPos + (transform.forward * Vector3.Distance(targetPos, target.transform.position) * .25f);
			}

			Vector3 targetRotation = m_cam_rotation;

			float l = Vector3.Distance(targetPos, transform.position);

			if(l > 50 || BattleControlor.Instance().inDrama == true)
			{
				transform.eulerAngles = targetRotation;

				transform.position = targetPos;
			}
			else
			{
				float t = targetRotation.y - transform.eulerAngles.y;

				if(Mathf.Abs(targetRotation.y - 360f - transform.eulerAngles.y) < Mathf.Abs(t)) targetRotation += new Vector3(0, -360f , 0);

				if(Mathf.Abs(targetRotation.y + 360f - transform.eulerAngles.y) < Mathf.Abs(t)) targetRotation += new Vector3(0, 360f , 0);

				Vector3 tempAngle = (targetRotation - transform.eulerAngles) / elasticCount;

				Vector3 tempPostion = (targetPos - transform.position) / elasticCount;

				transform.eulerAngles += tempAngle;

				transform.position += tempPostion;
			}

			//transform.eulerAngles = targetRotation;

			//elasticCount -= Time.deltaTime;

			//elasticCount = elasticCount < 5 ? 5 : elasticCount;

			if(m_shake == true) 
			{
				transform.position = transform.position + shake_offset;
			}
		}
	}

	public void Shake(ShakeType shakeType)
	{
		string trigger = "";

		if(shakeType == ShakeType.Vertical)
		{
			trigger = "shake_Vertical";
		}
		else if(shakeType == ShakeType.Cri)
		{
			trigger = "shake_CRI";
		}
		else if(shakeType == ShakeType.Forward)
		{
			trigger = "shake_Forward";
		}

		Animator anim = gameObject.GetComponent<Animator>();

		if(anim == null) anim = gameObject.AddComponent<Animator>();

		if(anim != null)
		{
			anim.runtimeAnimatorController = shakeControllor;

			anim.applyRootMotion = true;

			anim.enabled = true;
		
			anim.SetTrigger(trigger);

			StopCoroutine ("shakeClock");

			StartCoroutine ("shakeClock");
		}

//		if (m_shake == true) return;
//		
//		StartCoroutine (ShakeAction(actionId));
	}

	IEnumerator shakeClock()
	{
		yield return new WaitForSeconds (.3f);

		Animator[] anims = gameObject.GetComponents<Animator>();

		foreach(Animator anim in anims)
		{
			if(anim != null)
			{
				Destroy(anim);
			}
		}
	}

	IEnumerator ShakeAction(int actionId)
	{
		m_shake = true;
		
		float value = actionId == 1 ? .15f : .10f;

		float timeTotal = .03f;

		int count = 1;

		iTween.EaseType et = iTween.EaseType.easeOutQuart;

		for(int i = 0; i < count; i++)
		{
			Vector3 fromP = Vector3.zero;

			Vector3 toP = Vector3.zero;
			
			string strComp = "";

			float time = timeTotal / count;

			if(i == count - 1) strComp = "OnShakeComplete";

			if(actionId == 1)
			{
				toP = new Vector3(0, value, 0);
			}
			else
			{
				toP = new Vector3(value, 0, 0);
			}

			iTween.ValueTo(gameObject, iTween.Hash(
				"id", "shake" + i,
				"from", fromP,
				"to", toP,
				"time", time,
				"easetype", et,
				"onupdate", "OnShakeUpdate",
				"oncomplete", strComp
				));

			yield return new WaitForSeconds(time);
		}
	}

	public void OnShakeUpdate( Vector3 p_offset )
	{
		shake_offset = p_offset;
	}
	
	public void OnShakeComplete()
	{
		m_shake = false;
	}

	public void shine()
	{
		if (uiCamera == null) uiCamera = GameObject.Find ("BattleUICamera");

		BattleEffectControllor.Instance().PlayEffect(2, uiCamera.transform.position + uiCamera.transform.forward * 0.003f, uiCamera.transform.forward);
	}

	public void dark()
	{
//
//		BattleEffectControllor.Instance().PlayEffect(
//			BattleEffectControllor.EffectType.EFFECT_KING_CESHI_HEIPING, 
//			gameObject);
	}

	public void dark_2()
	{
//		BattleEffectControllor.Instance().PlayEffect(
//			BattleEffectControllor.EffectType.EFFECT_KING_CESHI_HEIPING_2, 
//			gameObject);
	}

	public void updateCameraFlags()
	{
		if (flags.Count == 0) return;

		if (BattleControlor.Instance().inDrama == true) return;

		listCurCam.Clear ();

		listCurLength.Clear ();

		totalLength = 0;

		for(int i = 0; i < flags.Count; i++)
		{
			BattleCameraFlag flag = flags[i];

			float radius = flag.radius;

			if(BattleControlor.Instance() != null)
			{
				KingControllor king = BattleControlor.Instance().getKing();

				float dis = Vector3.Distance(king.transform.position, flag.transform.position);

				bool killed = true;

				if(flag.killMin != 0 || flag.killMax != 1000)
				{
					int k = 0;

					foreach(BattleFlag bf in BattleControlor.Instance().flags.Values)
					{
						if(bf.flagId < 100) continue;

						if(bf.flagId > 1000) continue;

						if(bf.node != null && bf.node.isAlive == true) break;

						k++;
					}

					killed = k >= flag.killMin && k < flag.killMax;
				}

				if(king != null && dis <= radius && killed)
				{
					float length = radius - dis;//君主坐标距离圆形范围边界的距离

					totalLength += length;

					listCurCam.Add(flag);

					listCurLength.Add(length);
				}
			}
		}

		if(listCurCam.Count < 1)
		{
			return;
		}
		else if(listCurCam.Count == 1)
		{
			if(curCameraId != listCurCam[0].flagId)
			{
				cur_cam_position = listCurCam[0].cameraPosition;

				cur_cam_rotation = listCurCam[0].cameraRotation;

				cur_cam_parm = listCurCam[0].camera4Param;

				CameraChange (listCurCam[0].cameraPosition, listCurCam[0].cameraRotation);

				curCameraId = listCurCam[0].flagId;
			}
		}
		else
		{
			Vector3 tempPos = Vector3.zero;

			Vector3 tempRot = Vector3.zero;

			Vector3 flagRot = Vector3.zero;

			for(int i = 0; i < listCurCam.Count; i++)
			{
				if(i == 0) flagRot = listCurCam[i].cameraRotation;

//				if(tempRot.y - listCurCam[i].cameraRotation.y > 180 || flagRot.y - listCurCam[i].cameraRotation.y > 180)
//				{
//					listCurCam[i].cameraRotation += new Vector3(0, 360, 0);
//				}
//				else if(listCurCam[i].cameraRotation.y - tempRot.y > 180 || listCurCam[i].cameraRotation.y - flagRot.y > 180)
//				{
//					listCurCam[i].cameraRotation += new Vector3(0, -360, 0);
//				}

				if( flagRot.y - listCurCam[i].cameraRotation.y > 180)
				{
					listCurCam[i].cameraRotation += new Vector3(0, 360, 0);
				}
				else if( listCurCam[i].cameraRotation.y - flagRot.y > 180)
				{
					listCurCam[i].cameraRotation += new Vector3(0, -360, 0);
				}

				tempPos += listCurCam[i].cameraPosition * (listCurLength[i] / totalLength);

				tempRot += listCurCam[i].cameraRotation * (listCurLength[i] / totalLength);
			}

			cur_cam_position = tempPos;
			
			cur_cam_rotation = tempRot;
			
			CameraChange (tempPos, tempRot);
		}

//		if(camCount == 1 && tempCamera.flagId != curCameraId)
//		{
//			cur_cam_position = tempCamera.cameraPosition;
//
//			cur_cam_rotation = tempCamera.cameraRotation;
//
//			cur_cam_parm = tempCamera.camera4Param;
//
//			CameraChange (tempCamera.cameraPosition, tempCamera.cameraRotation);
//
//			//elasticCount = 5f;
//		}
	}

	private IEnumerator elasticAction()
	{
		for(;;)
		{
			elasticCount -= Time.deltaTime * 30f;

			elasticCount = elasticCount < 5 ? 5 : elasticCount;

			yield return new WaitForEndOfFrame();
		}
	}

}

using UnityEngine;
using System.Collections;

public class DramaActorCameraMove : DramaActor
{
	public Camera targetCamera;

	public int parentNodeId = 0;

	public string m_AnimationPath = null;

	public float movingTime;


	private Animator m_Animator;
	
	private RuntimeAnimatorController m_RunTimeAnimatorController;


	void Start()
	{
		actorType = ACTOR_TYPE.CAMERA_ANIMATION;

		if(m_AnimationPath != null)
		{
			Global.ResourcesDotLoad(m_AnimationPath,
			                        LoadCallback );
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		targetCamera = null;

		if( m_Animator != null ){
			m_Animator.runtimeAnimatorController = null;
		}

		m_Animator = null;

		m_RunTimeAnimatorController = null;
	}

	public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		m_RunTimeAnimatorController = p_object as RuntimeAnimatorController;
	}
	
	protected override float func ()
	{
		base.func ();

		if(parentNodeId == 0 || BattleControlor.Instance() == null)
		{
			targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		}
		else
		{
			BaseAI node = BattleControlor.Instance().getNodebyId(parentNodeId);
			
			if(node == null) node = BattleControlor.Instance().getNodebyModelId(parentNodeId);
			
			if(node != null)
			{
				GameObject mainCamera = GameObject.Find("Main Camera");

				Quality_SceneCameraFx tempSceneCamera = mainCamera.GetComponentInChildren<Quality_SceneCameraFx>();

				GameObject tempCamera = tempSceneCamera == null ? null : tempSceneCamera.gameObject;

				if(tempCamera != null) tempCamera.SetActive(false);

				GameObject targetCameraObject = (GameObject)Instantiate(mainCamera);

				if(tempCamera != null) tempCamera.SetActive(true);

				targetCameraObject.name = "STORY_CAMERA";

				{
					EffectTool.StoryVignet( targetCameraObject );
				}
				
				targetCameraObject.transform.parent = node.transform;
				
				targetCameraObject.transform.localPosition = Vector3.zero;
				
				targetCameraObject.transform.localEulerAngles = Vector3.zero;
				
				Destroy(targetCameraObject.GetComponent<KingCamera>());
				
				targetCamera = targetCameraObject.GetComponent<Camera>();

				targetCamera.cullingMask = LayerMask.GetMask(new string[]{"3D Layer", "Grounded", "3D Layer Without Light"});

				targetCameraObject.SetActive(false);
			}
			else
			{
				targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
				
				Debug.Log("NO Node With Id " + parentNodeId);
			}
		}

		if(parentNodeId == 0 || BattleControlor.Instance() == null)
		{

		}
		else
		{
			GameObject.Find("Main Camera").SetActive(false);

			targetCamera.gameObject.SetActive(true);
		}

		m_Animator = targetCamera.GetComponent<Animator>();

		/* By YuGu:
		 * 
		 * Remove animator if already exist.
		 * 
		 * REMOVED, avoid multi-load.
		 */
		if ( DramaDirector.IsDramaPreviewing ( )) 
		{
			if( m_Animator != null )
			{
				ComponentHelper.DestroyImmediate( m_Animator );
				
				m_Animator = null;
			}
		}

		if(m_Animator == null) m_Animator = targetCamera.gameObject.AddComponent<Animator>();

		if(m_Animator != null) 
		{
			m_Animator.runtimeAnimatorController = m_RunTimeAnimatorController;
		
			m_Animator.applyRootMotion = false;
		}

		return movingTime;
	}

	protected override void funcForcedEnd ()
	{
		funcDone ();
	}

	protected override bool funcDone ()
	{
		if(parentNodeId == 0 || BattleControlor.Instance() == null)
		{
			GameObject.Destroy(m_Animator);
		}
		else
		{
			if(targetCamera.gameObject.name.Equals("STORY_CAMERA") == true)
			{
				targetCamera.gameObject.SetActive(false);

				BattleControlor.Instance().getKing().gameCamera.gameObject.SetActive(true);
			}
		}

		return true;
	}

	public override void log()
	{
		Debug.Log ("DramaActorCameraMove data :");
		
		Debug.Log ("parentNodeId: " + parentNodeId);
		
		Debug.Log ("m_AnimationPath: " + m_AnimationPath);
		
		Debug.Log ("movingTime: " + movingTime);
	}

}

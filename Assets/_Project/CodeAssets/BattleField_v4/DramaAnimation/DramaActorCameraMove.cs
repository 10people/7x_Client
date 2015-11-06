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
				
				GameObject targetCameraObject = (GameObject)Instantiate(mainCamera);
				
				targetCameraObject.name = "STORY_CAMERA";
				
				targetCameraObject.transform.parent = node.transform;
				
				targetCameraObject.transform.localPosition = Vector3.zero;
				
				targetCameraObject.transform.localEulerAngles = Vector3.zero;
				
				Destroy(targetCameraObject.GetComponent<KingCamera>());
				
				targetCamera = targetCameraObject.GetComponent<Camera>();
				
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
		if ( DramaDirector.IsDramaPreviewing ( )) {
			if( m_Animator != null ) {
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

}

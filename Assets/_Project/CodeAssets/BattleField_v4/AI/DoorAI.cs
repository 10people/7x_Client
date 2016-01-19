using UnityEngine;
using System.Collections;

public class DoorAI : MonoBehaviour 
{
	public enum AniType
	{
		ANI_Stand0 = 0,		//开着门
		ANI_Stand1 = 1,		//关着门
		ANI_Open = 2,		//开门
		ANI_Close = 3,		//关门
	}
	
	private string[] m_strAnimationName = new string[]{
		"Stand0",         //0
		"Stand1",         //1
		"Open",           //2
		"Close",          //3
	};

	private Animator mAnim;

	private NavMeshObstacle nav;

	private BoxCollider m_box_collider;

	void Awake(){
//		ObjectHelper.AddGameObjectTrace( gameObject );
//
//		ObjectHelper.AddComponentTrace( this );
	}

	void OnDestroy(){
		mAnim = null;

		nav = null;

		m_box_collider = null;  
	}

	public void init()
	{
		mAnim = GetComponent<Animator>();

		nav = GetComponent<NavMeshObstacle>();

		m_box_collider = GetComponent<BoxCollider>();

		OpenDoor ();

//		ObjectHelper.AddObjectTrace( mAnim );
//
//		ObjectHelper.AddObjectTrace( mAnim.runtimeAnimatorController );
	}

	public void OpenDoor()
	{
		mAnim.Play (m_strAnimationName[(int)AniType.ANI_Open]);

		nav.enabled = false;

		m_box_collider.enabled = false;
	}

	public void CloseDoor()
	{
		mAnim.Play (m_strAnimationName[(int)AniType.ANI_Close]);
		
		nav.enabled = true;
		
		m_box_collider.enabled = true;
	}
}
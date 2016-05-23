using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LuoLiTestKing : MonoBehaviour 
{
	public List<GameObject> fxList = new List<GameObject>();

	public Animator animEnmey;

	public Transform fxParent;


	[HideInInspector] public Animator mAnim;

	[HideInInspector] public CharacterController character;


	private static LuoLiTestKing _instance;

	private int hitCount;

	private Dictionary<int, float> movementList;


	void Awake()
	{
		_instance = this;
	}

	public static LuoLiTestKing instance()
	{
		return _instance;
	}

	void Start () 
	{
		mAnim = GetComponentInChildren<Animator>();

		character = this.GetComponent<CharacterController>();

		hitCount = 0;

		movementList = new Dictionary<int, float> ();

		movementList.Add (11, 1f);

		movementList.Add (21, 1f);

		movementList.Add (22, .2f);

		movementList.Add (31, 1f);

		movementList.Add (41, .5f);
	}
	
	void Update () 
	{

	}

	public void move(Vector3 offset)
	{
		if (isPlayingAttack ()) return;

		string isplaying = IsPlaying ();

		if (isplaying.IndexOf ("_done") != -1) return;

		float length = Vector3.Distance (Vector3.zero, offset);

		if(!isplaying.Equals("Run") && length > 0)
		{
			mAnim.Play("Run");
		}
		else if(isplaying.Equals("Run") && length == 0)
		{
			mAnim.Play("Stand");
		}

		if(length > 0)
		{
			transform.forward = offset;
		}

		if(character.enabled == true)
		{
			character.Move(offset * 7 * Time.deltaTime);
		}
	}
	
	public void attack()
	{
		hitCount = 0;

		if (isPlayingAttack ()) 
		{
			string playing = IsPlaying ();
			
			string[] strs = playing.Split ('_');
			
			int index = int.Parse (strs [1]);
			
			hitCount = index;
		}

		mAnim.SetBool ("attack_" + (hitCount + 1), true);
	}

	public void attackAnimationStart(int index)
	{
		resetHit (index);

		playAttackFx (index);
	}

	private void resetHit(int index)
	{
		mAnim.SetBool ("attack_" + index, false);
	}

	private void playAttackFx(int index)
	{
		GameObject gc = GameObject.Instantiate(fxList[index]);

		gc.transform.parent = fxParent;

		gc.transform.position = transform.position;

		gc.transform.forward = transform.forward;
	}

	public bool isPlayingAttack()
	{
		string playing = IsPlaying ();
		
		if (playing.IndexOf ("attack_") != -1 && playing.IndexOf ("_done") == -1) return true;
		
		string nextPlay = nextPlaying ();
		
		if (nextPlay.IndexOf ("attack_") != -1&& playing.IndexOf ("_done") == -1) return true;
		
		return false;
	}

	public string IsPlaying()
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetCurrentAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}

	public string nextPlaying()
	{
		if (mAnim == null) return "";
		
		AnimatorClipInfo[] t_states = mAnim.GetNextAnimatorClipInfo( 0 );
		
		for( int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[ i ];
			
			return t_item.clip.name;
		}
		
		return "";
	}

	public void hit(int index)
	{
		if(Vector3.Distance(transform.position, animEnmey.transform.position) > 5)
		{
			return;
		}

		float length = 0;

		movementList.TryGetValue (index, out length);

		Vector3 targetPos = animEnmey.transform.position + transform.forward * length;

		iTween.MoveTo (animEnmey.gameObject, iTween.Hash(
			"position", targetPos,
			"time", .2f,
			"easetype", iTween.EaseType.easeInCubic
			));

		animEnmey.SetTrigger ("BATC");
	}

	public void movement(int index)
	{

	}

}

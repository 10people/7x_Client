using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TCityPlayerMove : MonoBehaviour {

	private EnterScene enterScene;

	private NavMeshAgent playerAgent;

	private Animator pAnimator;

	private enum AnimationType
	{
		IDLE,
		RELAX1,
		RELAX2
	}
	private AnimationType playerType = new AnimationType();
	private AnimationType playerMidType = new AnimationType();

	private bool randomBegin = false;
	private int randomTime = 0;
	private float animatorTime = 0.0f;

	private bool _isTimeGo = false;

	private enum MoveType
	{
		NONE,
		MOVE_TYPE_RUN,
		MOVE_TYPE_IDLE
	}
	private MoveType moveType = new MoveType();

	public void InItTCityPlayer (EnterScene tempEnterScene)
	{
		enterScene = tempEnterScene;
		playerAgent = GetComponent<NavMeshAgent> ();
		pAnimator = GetComponentInChildren<Animator> ();
		{
			randomBegin = true;
			animatorTime = 0;
			randomTime = Random.Range(5, 15);
			moveType = MoveType.NONE;
			playerType = AnimationType.IDLE;
		}
	}

	public EnterScene PlayerEnterScene ()
	{
		return enterScene;
	}

	public void PlayerRun (Vector3 targetPos)
	{
		if (randomBegin)
		{
			randomBegin = false;
			AnimationPlay(1);
		}
		
		playerAgent.speed = TreasureCityPlayer.m_instance.m_speed;
		
		playerAgent.Resume();
		playerAgent.SetDestination (targetPos);
		Vector3 curPos = transform.position;
		Debug.Log ("curPos:" + curPos);
		Debug.Log ("Vector3.Distance (targetPos,curPos):" + Vector3.Distance (targetPos,curPos));
//		float time = Vector3.Distance (targetPos,curPos) / playerAgent.speed;
//		Debug.Log ("time:" + time);
//		transform.position = Vector3.Lerp (curPos,targetPos,Time.deltaTime * 2);
	}

	void Update()
	{
		if (playerAgent != null )
		{
			if (Mathf.Abs(playerAgent.remainingDistance) < 0.01f)
			{
				PlayerStop();
			}
		}

		if (randomBegin && !_isTimeGo)
		{
			animatorTime += Time.deltaTime;

			if (animatorTime >= randomTime)
			{
				randomTime = Random.RandomRange(5, 15);
				_isTimeGo = true;

				switch (playerType)
				{
				case AnimationType.IDLE:
				{
					playerMidType = playerType;

					AnimationPlay (2);

					playerType = AnimationType.RELAX1;

					break;
				}
				case AnimationType.RELAX2:
				{
					playerMidType = playerType;

					AnimationPlay(3);

					playerType = AnimationType.RELAX1;

					break;
				}
				default:
					break;
				}
				
				animatorTime = 0;
			}
		}
		else if (animatorTime > 0)
		{
			animatorTime = 0;
		}

		if ((IsPlayComplete("inRelax_1") || IsPlayComplete("inRelax_2")) && playerType == AnimationType.RELAX1)
		{
			_isTimeGo = false;
			moveType = MoveType.MOVE_TYPE_IDLE;

			PlayerStop();

			if (playerMidType == AnimationType.IDLE)
			{
				playerType = AnimationType.RELAX1;
			}
			else
			{
				playerType = AnimationType.RELAX2;
			}
		}
	}
	
	private void PlayerStop()
	{
//		playerAgent.Stop();
		if (moveType == MoveType.NONE)
		{
			randomBegin = true;
		}
		
		if (moveType == MoveType.MOVE_TYPE_IDLE)
		{
			moveType = MoveType.MOVE_TYPE_RUN;

			AnimationPlay(0);
			randomBegin = true;
		}
	}
	
	public void AnimationPlay (int index)
	{
		switch (index)
		{
		case 0:
		{
			randomBegin = true;
			pAnimator.Play ("zhuchengidle");

			break;
		}
		case 1:
		{
			_isTimeGo = false;
			randomBegin = false;
			playerType = AnimationType.IDLE;
			moveType = MoveType.MOVE_TYPE_IDLE;
			pAnimator.Play ("zhuchengrun");

			break;
		}
		case 2:
		{
			pAnimator.Play ("zhuchengrelax_1");
			break;
		}
		case 3:
		{
			pAnimator.Play ("zhuchengrelax_2");
			break;
		}
		default:
			break;
		}
	}

	private bool IsPlayComplete (string tempName)
	{
		if (pAnimator == null) return false;
		
		AnimatorClipInfo[] t_states = pAnimator.GetCurrentAnimatorClipInfo(0);
		AnimatorStateInfo info = pAnimator.GetCurrentAnimatorStateInfo(0);
		float playing = Mathf.Clamp01(info.normalizedTime);
		
		for (int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[i];
			
			if (t_item.clip.name.Equals(tempName) && playing >= 1.0f)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		return false;
	}
}

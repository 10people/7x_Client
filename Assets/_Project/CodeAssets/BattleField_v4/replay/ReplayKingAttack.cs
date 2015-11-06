using UnityEngine;
using System.Collections;

public class ReplayKingAttack : BaseReplay
{
	public float delayTime;

	public Vector3 offset;


	public static ReplayKingAttack createReplayKingAttack(float delayTime, Vector3 offset)
	{
		ReplayKingAttack ra = new ReplayKingAttack();

		ra.replayorType = ReplayorNodeType.KING_ATTACK;

		ra.delayTime = delayTime;

		ra.offset = offset;

		return ra;
	}

}

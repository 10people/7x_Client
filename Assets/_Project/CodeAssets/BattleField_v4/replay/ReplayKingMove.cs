using UnityEngine;
using System.Collections;

public class ReplayKingMove : BaseReplay
{
	public float delayTime;

	public Vector3 offset;


	public static ReplayKingMove createReplayKingMove(float delayTime, Vector3 offset)
	{
		ReplayKingMove ra = new ReplayKingMove();

		ra.replayorType = ReplayorNodeType.KING_MOVE;

		ra.delayTime = delayTime;

		ra.offset = offset;

		return ra;
	}

}

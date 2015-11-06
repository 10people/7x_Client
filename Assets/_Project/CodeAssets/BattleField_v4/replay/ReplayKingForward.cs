using UnityEngine;
using System.Collections;

public class ReplayKingForward : BaseReplay
{
	public float delayTime;

	public Vector3 forward;


	public static ReplayKingForward createReplayKingForward(float delayTime, Vector3 forward)
	{
		ReplayKingForward ra = new ReplayKingForward();

		ra.replayorType = ReplayorNodeType.KING_FORWARD;

		ra.delayTime = delayTime;

		ra.forward = forward;

		return ra;
	}

}

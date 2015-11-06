using UnityEngine;
using System.Collections;

public class ReplayKingRevise : BaseReplay
{
	public float delayTime;

	public Vector3 position;


	public static ReplayKingRevise createReplayKingRevise(float delayTime, Vector3 position)
	{
		ReplayKingRevise ra = new ReplayKingRevise();

		ra.replayorType = ReplayorNodeType.KING_REVISE;

		ra.delayTime = delayTime;

		ra.position = position;

		return ra;
	}

}

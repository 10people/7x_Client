using UnityEngine;
using System.Collections;

public class ReplayKingInspire : BaseReplay
{
	public float delayTime;


	public static ReplayKingInspire createReplayKingInspire(float delayTime)
	{
		ReplayKingInspire ra = new ReplayKingInspire();

		ra.replayorType = ReplayorNodeType.KING_INSPIRE;

		ra.delayTime = delayTime;

		return ra;
	}

}

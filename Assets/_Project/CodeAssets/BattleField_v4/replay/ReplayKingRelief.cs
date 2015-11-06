using UnityEngine;
using System.Collections;

public class ReplayKingRelief : BaseReplay
{
	public float delayTime;


	public static ReplayKingRelief createReplayKingRelief(float delayTime)
	{
		ReplayKingRelief ra = new ReplayKingRelief();

		ra.replayorType = ReplayorNodeType.KING_RELIF;

		ra.delayTime = delayTime;

		return ra;
	}

}

using UnityEngine;
using System.Collections;

public class ReplayAutoFight : BaseReplay
{
	public float delayTime;


	public static ReplayAutoFight createReplayAutoFight(float delayTime)
	{
		ReplayAutoFight ra = new ReplayAutoFight();

		ra.replayorType = ReplayorNodeType.AUTO_FIGHT;

		ra.delayTime = delayTime;

		return ra;
	}

}

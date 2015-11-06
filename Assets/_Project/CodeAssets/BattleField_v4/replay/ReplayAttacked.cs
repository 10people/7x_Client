using UnityEngine;
using System.Collections;

public class ReplayAttacked : BaseReplay
{
	public float delayTime;

	public float hpValue;


	public static ReplayAttacked createReplayAttack(float delayTime, float hpValue)
	{
		ReplayAttacked ra = new ReplayAttacked();

		ra.replayorType = ReplayorNodeType.ATTACK;

		ra.delayTime = delayTime;

		ra.hpValue = hpValue;

		return ra;
	}

}

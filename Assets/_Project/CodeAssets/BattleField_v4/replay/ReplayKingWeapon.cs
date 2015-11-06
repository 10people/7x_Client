using UnityEngine;
using System.Collections;

public class ReplayKingWeapon : BaseReplay
{
	public float delayTime;

	public int weapon;//0:dao  1:gong  2:qiang


	public static ReplayKingWeapon createReplayKingWeapon(float delayTime, int weapon)
	{
		ReplayKingWeapon ra = new ReplayKingWeapon();

		ra.replayorType = ReplayorNodeType.KING_WEAPON;

		ra.delayTime = delayTime;

		ra.weapon = weapon;

		return ra;
	}

}

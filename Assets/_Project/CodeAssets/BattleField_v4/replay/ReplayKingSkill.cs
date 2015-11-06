using UnityEngine;
using System.Collections;

public class ReplayKingSkill : BaseReplay
{
	public float delayTime;

	public int skillType;//1:dao_1  2:dao_2  3:gong_1  4:gong_2  5:qiang_1_prepare  6:qiang_1_cacel  7:qiang_1_use  8:qiang_2


	public static ReplayKingSkill createReplayKingSkill(float delayTime, int skillType)
	{
		ReplayKingSkill ra = new ReplayKingSkill();

		ra.replayorType = ReplayorNodeType.KING_SKILL;

		ra.delayTime = delayTime;

		ra.skillType = skillType;

		return ra;
	}

}

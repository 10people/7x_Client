using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseReplay
{
	public enum ReplayorNodeType
	{
		ATTACK,
		KING_MOVE,
		DELAY,
		KING_INSPIRE,
		KING_RELIF,
		KING_SKILL,
		KING_WEAPON,
		KING_ATTACK,
		KING_REVISE,
		KING_FORWARD,
		AUTO_FIGHT,
	}

	public ReplayorNodeType replayorType;

}

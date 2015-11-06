using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class EnterBattleFieldWithoutNetworking : MonoBehaviour
{
	public int iconId;

	void OnClick()
	{
		CityGlobalData.m_king_icon = iconId;

		EnterBattleField.EnterBattlePve(-100, 0, LevelType.LEVEL_NORMAL);
	}

}

using UnityEngine;
using System.Collections;

public class AchivementHintItem : MonoBehaviour 
{
	public UISprite sprite;

	public UISprite sprite_2;

	public UILabel label;


	private int index;

	private int state;


	public void init(int _index, int _state)
	{
		index = _index;

		state = _state;

		label.text = BattleControlor.Instance().achivement.descs [index];

		setState (_state);
	}
	
	public void setState (int _state) 
	{
		if(_state == state || CityGlobalData.mPveStar[index] == 1)
		{
			if(CityGlobalData.mPveStar[index] == 1)
			{
				if(sprite.spriteName != "battle_star")
				{
					sprite.spriteName = "battle_star";
					sprite_2.spriteName = sprite.spriteName;
				}
			}
			return;
		}

		if(_state == 0)
		{
			sprite.spriteName = "battle_star_gray";
		}
		else if(_state == 1)
		{
			sprite.spriteName = "battle_star";
		}
		else if(_state == -1)
		{
			sprite.spriteName = "battle_cross";
		}

		sprite_2.spriteName = sprite.spriteName;

		state = _state;
	}

}

using UnityEngine;
using System.Collections;

public class AchivementHintItem : MonoBehaviour 
{
	public UISprite sprite;

	public UILabel label;


	private int index;

	private int state;


	public void init(int _index, int _state)
	{
		index = _index;

		state = 0;

		label.text = BattleControlor.Instance().achivement.descs [index];

		setState (_state);
	}
	
	public void setState (int _state) 
	{
		if(_state == state)
		{
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

		state = _state;
	}

}

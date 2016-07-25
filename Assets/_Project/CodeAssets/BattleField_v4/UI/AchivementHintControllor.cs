using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchivementHintControllor : MonoBehaviour
{
	public List<AchivementHintItem> itemList = new List<AchivementHintItem>();

	public UISprite spriteArrow;

	public float upY;

	public float downY;


	private int updown = -2;//1-上升中，2-下降中，-1-上升之后，-2-下降之后


	public void init()
	{
		if(BattleControlor.Instance().achivement == null) 
		{
			gameObject.SetActive(false);

			return;
		}

		int achive_1 = CityGlobalData.mPveStar[0];

		int achive_2 = CityGlobalData.mPveStar[1];

		int achive_3 = CityGlobalData.mPveStar[2];

		//BattleControlor.Instance().achivement.setListBool (achive_1, achive_2, achive_3);

		int[] list = new int[]{achive_1, achive_2, achive_3};

		for(int i = 0; i < itemList.Count; i++)
		{
			AchivementHintItem item = itemList[i];

			item.init(i, list[i]);
		}
	}
	
	public void UpDown()
	{
		if(updown == -2)
		{
			updown = 1;

			spriteArrow.spriteName = "battle_down";

			iTween.MoveTo (gameObject, iTween.Hash(
				"name", "func",
				"position", transform.localPosition + new Vector3(0, -transform.localPosition.y + upY, 0),
				"time", .12f,
				"easeType", iTween.EaseType.linear,
				"islocal",true,
				"oncomplete", "upDone"
				));

			foreach(AchivementHintItem item in itemList)
			{
				item.sprite_2.gameObject.SetActive(false);
			}
		}
		else if(updown == -1)
		{
			updown = 2;

			spriteArrow.spriteName = "battle_up";

			iTween.MoveTo (gameObject, iTween.Hash(
				"name", "func",
				"position", transform.localPosition + new Vector3(0, -transform.localPosition.y + downY, 0),
				"time", .4f,
				"easeType", iTween.EaseType.easeOutElastic,
				"islocal",true,
				"oncomplete", "downDone"
				));

			foreach(AchivementHintItem item in itemList)
			{
				item.sprite_2.gameObject.SetActive(true);
			}
		}
	}

	private void upDone()
	{
		updown = -1;
	}

	private void downDone()
	{
		updown = -2;
	}

	public void achivementCallback(int index, int state)
	{
		itemList [index].setState (state);
	}

}

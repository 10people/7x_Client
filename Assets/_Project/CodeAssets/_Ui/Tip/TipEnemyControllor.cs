using UnityEngine;
using System.Collections;

public class TipEnemyControllor : MonoBehaviour
{
	public UISprite spriteIcon;

	public UILabel labelName;

	public UILabel labelDesc;


	public void refreshData(string iconName, string enemyName, string enemyDesc)
	{
		if(UICamera.lastTouchPosition.x < Screen.width / 2)
		{
			gameObject.transform.localPosition = new Vector3(240, 0, 0);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(-240, 0, 0);
		}

		spriteIcon.spriteName = iconName;

		labelName.text = enemyName;

		labelDesc.text = enemyDesc;
	}

}

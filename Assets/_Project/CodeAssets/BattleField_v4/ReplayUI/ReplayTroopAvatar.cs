using UnityEngine;
using System.Collections;

public class ReplayTroopAvatar : MonoBehaviour {

	public UIRoot uiRoot;

	public GameObject avatarObject;

	public UISprite bloodBar;

	public UISprite spriteArrow;

	public UILabel debugFont;


	private TroopReplay troop;

	private float valueMax;


	public void init(TroopReplay _troop)
	{
		troop = _troop;

		valueMax = troop.hero.hpMax;

		foreach(ReplayNode soldier in troop.soldiers)
		{
			valueMax += soldier.hpMax;
		}

		debugFont.text = "";
	}

	void FixedUpdate()
	{
		if(troop == null || troop.hero == null || !troop.hero.isAlive)
		{
			Destroy(gameObject);

			return;
		}

		updatePosition();

		updateBarValue();
	}

	private void updatePosition()
	{
		Vector3 po = Camera.main.WorldToViewportPoint(troop.hero.transform.position);

		Vector3 targetPosition = new Vector3(po.x * 960 - 480, po.y * 640 - 320, 0);//屏幕中心是(0, 0, 0)

		if(troop.position.z < Camera.main.transform.position.z)
		{
			po = Camera.main.WorldToViewportPoint(troop.hero.transform.position + new Vector3(0, 0, (Camera.main.transform.position.z - troop.position.z) * 2));

			targetPosition = new Vector3(po.x * 960 - 480, po.y * 640 - 320, 0);

			targetPosition += new Vector3(0, - targetPosition.y - 640, 0);
		}

		Vector3 position = targetPosition;

		float l = Vector3.Distance(Camera.main.transform.position, troop.position) - 24.0f;

		l = l <= 0 ? 0.1f : l;

		float scale = (200 / l) - 1.0f;

		scale = scale < 0.6f ? 0.6f : scale;

		scale = scale > 1.0f ? 1.0f : scale;

		transform.localScale = new Vector3(scale, scale, 1);

		float w = -480.0f + (70.0f * scale);// w = -410

		float h = -320.0f + (70.0f * scale);// h = -250

		if(position.x < w || position.x > -w || position.y < h || position.y > -h)
		{
			if(position.x < w)
			{
				position += new Vector3(-position.x + w, 0, 0);
			}

			if(position.x > -w)
			{
				position += new Vector3(-position.x - w, 0, 0);
			}

			if(position.y < h)
			{
				position += new Vector3(0, -position.y + h, 0);
			}

			if(position.y > -h)
			{
				position += new Vector3(0, -position.y - h, 0);
			}

			avatarObject.SetActive(true);

			transform.localPosition = position;

			uodateArrow(position, targetPosition);
		}
		else
		{
			avatarObject.SetActive(false);
		}
	}

	private void uodateArrow(Vector3 avatarPosition, Vector3 targetPosition)
	{
		Vector3 tempPosition = targetPosition - avatarPosition;

		float alpha = Mathf.Atan(tempPosition.x / tempPosition.y) * Mathf.Rad2Deg;

		if(tempPosition.y < 0)
		{
			alpha += 180;
		}

		spriteArrow.transform.localEulerAngles = new Vector3(0, 0, -alpha - 90);

		float tl = Vector3.Distance(avatarPosition, targetPosition);

		float tx = tempPosition.x;

		float ty = tempPosition.y;

		float l = 55.0f;

		float x = l * tx / tl;

		float y = l * ty / tl;

		spriteArrow.transform.localPosition = new Vector3(x, y, 0);
	}

	private void updateBarValue()
	{
		float blood = troop.hero.hp;
		
		foreach(ReplayNode soldier in troop.soldiers)
		{
			blood += soldier.hp;
		}

		bloodBar.fillAmount = blood / valueMax;
	}

}

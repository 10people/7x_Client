using UnityEngine;
using System.Collections;

public class TroopAvatar : MonoBehaviour {

	public UIRoot uiRoot;

	public GameObject avatarObject;

	public UISprite spriteArrow;


	private BaseAI node;

//	private float valueMax;


	public void init(BaseAI _node)
	{
		node = _node;

//		valueMax = node.hpMax;
	}

	void FixedUpdate()
	{
		if(node == null || !node.isAlive)
		{
			Destroy(gameObject);

			return;
		}

		updatePosition();
	}

	private void updatePosition()
	{
		Vector3 po = Camera.main.WorldToViewportPoint(node.transform.position);

		Vector3 targetPosition = new Vector3(po.x * 960 - 480, po.y * 640 - 320, 0);//屏幕中心是(0, 0, 0)

		if(node.transform.position.z < Camera.main.transform.position.z)
		{
			po = Camera.main.WorldToViewportPoint(node.transform.position + new Vector3(0, 0, (Camera.main.transform.position.z - node.transform.position.z) * 2));

			targetPosition = new Vector3(po.x * 960 - 480, po.y * 640 - 320, 0);

			targetPosition += new Vector3(0, - targetPosition.y - 640, 0);
		}

		Vector3 position = targetPosition;

		float l = Vector3.Distance(Camera.main.transform.position, node.transform.position) - 24.0f;

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

}

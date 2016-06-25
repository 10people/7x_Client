using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleComboControllor : MonoBehaviour 
{
	public UISprite sprite1;

	public UISprite sprite10;

	public UISprite sprite100;

	public GameObject layerSprites;

	public List<int> widthList = new List<int>();

	public List<int> heighList = new List<int> ();

	public List<int> drawWidthList = new List<int> ();


	private int comboNum;

	private List<UISprite> spriteList = new List<UISprite>();

	private float tempTime;

	private float curTime;

	private bool inshow;

	private float waittingTime = 5;

	private float scaleTime = .25f;

	private float scaleValue = .75f;


	void Start ()
	{
		waittingTime = 5f;

		comboNum = 0;

		UISprite[] sprites = gameObject.GetComponentsInChildren<UISprite>();

		spriteList.Clear ();

		foreach(UISprite sprite in sprites)
		{
			spriteList.Add(sprite);

			sprite.gameObject.SetActive(false);
		}

		tempTime = 0;

		curTime = 0;

		inshow = false;
	}

	public void addComboNum()
	{
		setComboNum (comboNum + 1);
	}

	public void setComboNum(int _comboNum)
	{
		comboNum = _comboNum;

		comboNum = comboNum > 999 ? 999 : comboNum;

		tempTime = Time.realtimeSinceStartup;

		int num1 = comboNum % 10;

		int num10 = (comboNum % 100) / 10;

		int num100 = (comboNum % 1000) / 100;

		inshow = true;

		foreach(UISprite sprite in spriteList)
		{
			sprite.gameObject.SetActive(true);

			sprite.alpha = 1;
		}

		sprite100.gameObject.SetActive(comboNum >= 100);
		
		sprite10.gameObject.SetActive (comboNum >= 10);
		
		sprite1.gameObject.SetActive (comboNum >= 1);

		sprite1.spriteName = "battle_combo_" + num1;

		sprite1.SetDimensions (widthList[num1], heighList[num1]);

		sprite10.spriteName = "battle_combo_" + num10;

		sprite10.SetDimensions (widthList[num10], heighList[num10]);

		sprite100.spriteName = "battle_combo_" + num100;

		sprite100.SetDimensions (widthList[num100], heighList[num100]);

		int x10 = drawWidthList [num1];

		int x100 = drawWidthList [num1] + drawWidthList [num10];

		sprite10.transform.localPosition += new Vector3 (sprite1.transform.localPosition.x - x10 - sprite10.transform.localPosition.x, 0, 0);

		sprite100.transform.localPosition += new Vector3 (sprite1.transform.localPosition.x - x100 - sprite100.transform.localPosition.x, 0, 0);
	}

	void Update () 
	{
		if (!inshow) return;

		curTime = Time.realtimeSinceStartup;

		updateDuang ();

		updateHide ();
	}

	private void updateDuang()
	{
		float timeCount = curTime - tempTime;

		if(timeCount < scaleTime)
		{
			float scale = scaleValue - (timeCount * scaleValue / scaleTime) + 1f;

			layerSprites.transform.localScale = new Vector3(scale, scale, 1);
		}
	}

	private void updateHide()
	{
		float timeCount = curTime - tempTime;
		
		//允许间歇时间20秒，渐隐时间2秒。
		if(timeCount > waittingTime)
		{
			timeCount -= waittingTime;
			
			foreach(UISprite sprite in spriteList)
			{
				sprite.alpha = 1 - (timeCount * .5f);
				
				if(sprite.alpha < .1f)
				{
					sprite.alpha = .1f;
					
					sprite.gameObject.SetActive(false);
					
					inshow = false;
					
					comboNum = 0;
				}
			}
		}
	}

}

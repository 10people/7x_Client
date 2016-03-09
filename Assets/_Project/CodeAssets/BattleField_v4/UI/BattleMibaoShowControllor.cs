using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMibaoShowControllor : MonoBehaviour
{
	public GameObject layerMibaoNum;

	public GameObject layerPowerup;

	public List<UISprite> spriteNums; 

	public UILabel labelPowerup;


	private int offsetY = 23;

	private int mibaoNum = 21;

	private bool inShowNum;

	private float showMibaoNumTime = 30f;

	private float showNumOffsetY = 31f;

	private int mibaoTen;

	private int mibaoOne;

	private int curTen;

	private int curOne;

	private bool tenDone;

	private bool oneDone;

	private int powerUp = 12345;


	void Start ()
	{
		inShowNum = true;

		mibaoTen = mibaoNum / 10;

		mibaoOne = mibaoNum % 10;

		curTen = 0;

		curOne = 0;

		tenDone = false;

		oneDone = false;
	}
	
	void Update () 
	{
		if (inShowNum) updateMibaoNum ();
	}

	private void updateMibaoNum()
	{
		float tenTime = showMibaoNumTime / (mibaoTen + 1);

		float oneTime = showMibaoNumTime / (mibaoNum + 1);

		if(tenDone == false)
		{
			spriteNums [0].transform.localPosition += new Vector3 (0, showNumOffsetY / tenTime, 0);

			if(spriteNums [0].transform.localPosition.y > offsetY)
			{
				spriteNums [0].transform.localPosition += new Vector3(0, -spriteNums [0].transform.localPosition.y - 39f , 0);

				if(curTen < mibaoTen)
				{
					curTen ++;

					spriteNums[0].spriteName = "battle_" + curTen;
				}
				else
				{
					tenDone = true;

					spriteNums [0].transform.localPosition += new Vector3(0, -spriteNums [0].transform.localPosition.y - 8f , 0);
				}
			}
		}

		if(tenDone == false)
		{
			spriteNums [1].transform.localPosition += new Vector3 (0, showNumOffsetY / tenTime, 0);
			
			if(spriteNums [1].transform.localPosition.y > offsetY)
			{
				spriteNums [1].transform.localPosition += new Vector3(0, -spriteNums [1].transform.localPosition.y - 39f , 0);

				if(curTen < mibaoTen)
				{
					curTen ++;
					
					spriteNums[1].spriteName = "battle_" + curTen;
				}
				else
				{
					tenDone = true;

					spriteNums [1].transform.localPosition += new Vector3(0, -spriteNums [1].transform.localPosition.y - 8f , 0);
				}
			}
		}

		if(oneDone == false)
		{
			spriteNums [2].transform.localPosition += new Vector3 (0, showNumOffsetY / oneTime, 0);

			if(spriteNums [2].transform.localPosition.y > offsetY)
			{
				spriteNums [2].transform.localPosition += new Vector3(0, -spriteNums [2].transform.localPosition.y - 39f , 0);
				
				if(curOne < mibaoNum)
				{
					curOne ++;
					
					spriteNums[2].spriteName = "battle_" + (curOne % 10);
				}
				else
				{
					oneDone = true;

					spriteNums [2].transform.localPosition += new Vector3(0, -spriteNums [2].transform.localPosition.y - 8f , 0);
				}
			}
		}

		if(oneDone == false)
		{
			spriteNums [3].transform.localPosition += new Vector3 (0, showNumOffsetY / oneTime, 0);

			if(spriteNums [3].transform.localPosition.y > offsetY)
			{
				spriteNums [3].transform.localPosition += new Vector3(0, -spriteNums [3].transform.localPosition.y - 39f , 0);
				
				if(curOne < mibaoNum)
				{
					curOne ++;
					
					spriteNums[3].spriteName = "battle_" + (curOne % 10);
				}
				else
				{
					oneDone = true;

					spriteNums [3].transform.localPosition += new Vector3(0, -spriteNums [3].transform.localPosition.y - 8f , 0);
				}
			}
		}

		if (tenDone == true && oneDone == true)
		{
			inShowNum = false;

			spriteNums[0].spriteName = "battle_" + (mibaoNum / 10);

			spriteNums[1].spriteName = "battle_" + (mibaoNum / 10);

			spriteNums[2].spriteName = "battle_" + (mibaoNum % 10);

			spriteNums[3].spriteName = "battle_" + (mibaoNum % 10);

			spriteNums[0].transform.localPosition += new Vector3(0, -spriteNums[0].transform.localPosition.y - 8f, 0);

			spriteNums[1].transform.localPosition += new Vector3(0, -spriteNums[1].transform.localPosition.y - 50f, 0);

			spriteNums[2].transform.localPosition += new Vector3(0, -spriteNums[2].transform.localPosition.y - 8f, 0);

			spriteNums[3].transform.localPosition += new Vector3(0, -spriteNums[3].transform.localPosition.y - 50f, 0);
		
			StartCoroutine(hideMiBaoNum());
		}
	}

	IEnumerator hideMiBaoNum()
	{
		yield return new WaitForSeconds(.5f);

		layerMibaoNum.SetActive (false);

		layerPowerup.SetActive (true);

		labelPowerup.text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SHOW_MIBAO) + powerUp;

		yield return new WaitForSeconds(1f);

		layerPowerup.SetActive (false);

		yield return new WaitForSeconds(1f);

		gameObject.SetActive (false);
	}

}

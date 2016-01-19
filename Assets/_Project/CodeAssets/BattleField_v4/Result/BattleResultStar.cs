using UnityEngine;
using System.Collections;

public class BattleResultStar : MonoBehaviour 
{
	public UISprite spriteStar;

	public UILabel labelDesc;

	public AudioClip audioClip;


	private bool m_enable;

	public void refreshData(bool enable, string desc)
	{
		//gameObject.SetActive (true);

		m_enable = enable;

		if(desc != null) labelDesc.text = desc;

		if (enable == true) 
		{
			TweenScale.Begin(spriteStar.gameObject, 0f, new Vector3(10, 10, 10));
		} 
		else
		{
			spriteStar.color = new Color(0, 0, 0);

			labelDesc.color = new Color(.6f, .6f, .6f);

			TweenAlpha.Begin(spriteStar.gameObject, 0f, 0);
		}

		labelDesc.SetDimensions(1, 36);
	}

	public void refreshData_2(bool enable, string desc)
	{
		//gameObject.SetActive (true);
		
		m_enable = enable;
		
		if(desc != null) labelDesc.text = desc;
		
		if (enable == true) 
		{
			TweenScale.Begin(spriteStar.gameObject, 0f, new Vector3(10, 10, 10));
		} 
		else
		{
			spriteStar.color = new Color(0, 0, 0);
			
			labelDesc.color = new Color(.6f, .6f, .6f);
			
			TweenAlpha.Begin(spriteStar.gameObject, 0f, 0);
		}
		
		TweenAlpha.Begin (labelDesc.gameObject, 0, 0);
	}

	public void onShow()
	{
		gameObject.SetActive(true);

		if(m_enable == true)
		{
			iTween.ScaleTo(spriteStar.gameObject, iTween.Hash(
				"scale", new Vector3(1, 1, 1),
				"time", .3f,
				"easeType", iTween.EaseType.linear,
				"oncomplete", "playSound",
				"oncompletetarget", gameObject
				));
		}
		else
		{
			TweenAlpha.Begin(spriteStar.gameObject, 1f, 1);
		}
		
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1,
			"to", 400,
			"time", 2f,
			"easeType", iTween.EaseType.linear,
			"onupdate", "OnItweenUpdataWidth"
			));
	}

	public void onShow_2()
	{
		gameObject.SetActive(true);
		
		if(m_enable == true)
		{
			iTween.ScaleTo(spriteStar.gameObject, iTween.Hash(
				"scale", new Vector3(1, 1, 1),
				"time", .3f,
				"easeType", iTween.EaseType.linear,
				"oncomplete", "playSound",
				"oncompletetarget", gameObject
				));
		}
		else
		{
			TweenAlpha.Begin(spriteStar.gameObject, 1f, 1);
		}

		TweenAlpha.Begin (labelDesc.gameObject, 1f, 1);
	}

	public void OnItweenUpdataWidth(float width)
	{
		labelDesc.SetDimensions ((int)width, 36);
	}

	void playSound()
	{
		NGUITools.PlaySound(audioClip, 1, 1);
	}

}

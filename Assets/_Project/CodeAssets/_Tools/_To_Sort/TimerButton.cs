using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UIButton))]

public class TimerButton : MonoBehaviour
{
	public UILabel labelTime;

	public UISprite gray;

	public int seconds;


	private UIButton btn;

	private bool flag;

	private int curSeconds;


	void Start()
	{
		Restart();
	}

	public void Restart()
	{
		curSeconds = seconds;
		
		flag = true;
		
		btn = (UIButton)gameObject.GetComponent("UIButton");
		
		refreshDate();
	}

	void refreshDate()
	{
		if(curSeconds > 0)
		{
			labelTime.text = curSeconds + "";
		}

		labelTime.gameObject.SetActive(curSeconds > 0);
		
		gray.gameObject.SetActive(curSeconds > 0);

		btn.isEnabled = curSeconds <= 0;

		btn.tweenTarget.SetActive(btn.isEnabled);
	}

	void FixedUpdate ()
	{
		if(flag == true)
		{
			StartCoroutine(StartAction());

			flag = false;
		}
	}

	IEnumerator StartAction()
	{
		for(;;)
		{
			yield return new WaitForSeconds(1);

			curSeconds --;

			refreshDate();

			if(curSeconds <= 0) break;
		}
	}

}

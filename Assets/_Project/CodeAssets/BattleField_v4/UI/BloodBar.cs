using UnityEngine;
using System.Collections;

public class BloodBar : MonoBehaviour 
{
	public UISprite frontSprite;


	[HideInInspector] public UIProgressBar bar;
	

	private float targetValue;

	private float speed;


	void Update ()
	{
		UpdateForward ();

		updateValue ();
	}

	void UpdateForward ()
	{
		if (Camera.main == null) return;

		if (Camera.main.gameObject.activeSelf == false) return;

		transform.forward = Camera.main.transform.forward;
	}

	void updateValue()
	{
		float step = speed * Time.deltaTime;

		if(bar == null) bar = GetComponent<UIProgressBar>();

		if(Mathf.Abs(bar.value - targetValue) < step * .55f)
		{
			bar.value = targetValue;
		}
		else if(bar.value > targetValue)
		{
			bar.value -= step;
		}
		else if(bar.value < targetValue)
		{
			bar.value += step;
		}

		if (bar.value == 1) gameObject.SetActive (false);

		if (bar.value <= 0) gameObject.SetActive (false);
	}

	public void setValue(float _value)
	{
		if(bar == null) bar = GetComponent<UIProgressBar>();

		gameObject.SetActive (true);

		if (_value == 1) gameObject.SetActive (false);
		
		else if (_value <= 0) _value = 0;

		targetValue = _value;

		frontSprite.fillAmount = targetValue;

		speed =  Mathf.Abs(targetValue - bar.value) / .5f;
	}

}

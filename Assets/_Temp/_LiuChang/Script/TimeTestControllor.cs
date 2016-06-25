using UnityEngine;
using System.Collections;

public class TimeTestControllor : MonoBehaviour 
{

	private bool paused;

	private System.DateTime appPauseStartTime;


	void Start()
	{
		paused = false;
	}

	public void click()
	{
		paused = ! paused;

		if (paused) 
		{
			appPauseStartTime = System.DateTime.Now;

			Time.timeScale = 0;

			Debug.Log("OOOOOOOOOOOOOOOOOOOOOO    " + appPauseStartTime.ToString() + ", " + appPauseStartTime.Minute + ", " + appPauseStartTime.Hour + ", " + appPauseStartTime.Day);
		}
		else
		{
			System.DateTime timeCust = System.DateTime.Now;

			System.TimeSpan temp = timeCust - appPauseStartTime;

			Debug.Log("TTTTTTTTTTTTTTTTT    " + timeCust.ToString() + ", " + temp.TotalSeconds);

//			if(BattleUIControlor.Instance().pauseControllor.gameObject.activeSelf == true)
//			{
//				if(timeCust.Minute >= 3 || timeCust.Hour > 1 || timeCust.Day > 1)
//				{
//					BattleUIControlor.Instance().pauseControllor.Lose();
//				}
//			}
		}
	}


}

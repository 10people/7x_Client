using UnityEngine;
using System.Collections;

public class DebugTimeScale : MonoBehaviour
{
	public UILabel label;
	
	void Update ()
	{
		label.text = "TimeScale: " + Time.timeScale;
	}
}

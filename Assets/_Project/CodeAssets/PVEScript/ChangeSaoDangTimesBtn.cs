using UnityEngine;
using System.Collections;

public class ChangeSaoDangTimesBtn : MonoBehaviour {

	public bool AddTimes;
	public static int myadd;
	public static bool startadd;
	void Start () {
		myadd = 0;
	}

	void OnClick()
	{

		if(AddTimes)
		{
			myadd = 1;
			startadd = true;
		}
		else{

			myadd = -1;
			startadd = true;
		}

	}
}

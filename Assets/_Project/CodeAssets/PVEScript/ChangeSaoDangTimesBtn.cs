using UnityEngine;
using System.Collections;

public class ChangeSaoDangTimesBtn : MonoBehaviour {

	public bool AddTimes;
	public static int myadd;
	public static bool startadd;
	void Start () {
		myadd = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
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

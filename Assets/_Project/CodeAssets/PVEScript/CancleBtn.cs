using UnityEngine;
using System.Collections;

public class CancleBtn : MonoBehaviour {

	public GameObject supObj;

	void OnClick()
	{

		PveLevelUImaneger mPveLevelUImaneger = supObj.GetComponent<PveLevelUImaneger>();
		mPveLevelUImaneger.Create_No_Disdroy = false;
	}
}

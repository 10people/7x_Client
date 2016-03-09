using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodLabelTest : MonoBehaviour 
{

	public List<GameObject> listGc = new List<GameObject>();

	public List<GameObject> listLabelTemp = new List<GameObject> ();


	private bool testing;


	public void initTest()
	{
		BloodLabelControllor.Instance().initStartGenarl (listLabelTemp);
	}

	public void startTest()
	{
		testing = true;

		StartCoroutine (testAction());
	}

	public void endTest()
	{
		testing = false;
	}

	IEnumerator testAction()
	{
		for(int i = 0; testing; i++)
		{
			GameObject gc = listGc[i % listGc.Count];

			BloodLabelControllor.Instance().createBloodLabelGenarl (gc, "test_" + i, 2.2f, i % 3 == 0, i % listLabelTemp.Count);

			yield return new WaitForSeconds(.2f);
		}
	}

}

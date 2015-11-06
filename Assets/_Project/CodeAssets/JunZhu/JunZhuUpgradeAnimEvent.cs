using UnityEngine;
using System.Collections;

public class JunZhuUpgradeAnimEvent : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}

    public void Hidden()
    {
        StartCoroutine(WaitForUpgaade()); ;
    }

    IEnumerator WaitForUpgaade()
    {
        yield return new WaitForSeconds(1.4f);
        gameObject.SetActive(false);

    }
}

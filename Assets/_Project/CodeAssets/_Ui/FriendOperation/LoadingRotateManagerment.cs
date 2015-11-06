using UnityEngine;
using System.Collections;

public class LoadingRotateManagerment : MonoBehaviour
{

	// Use this for initialization
	void Start () 
    {
        
	}
    void OnEnable()
    {
        StartCoroutine(WaitFor());
    }
	
	// Update is called once per frame
	void Update ()
    {
	 
	}
    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(0.04f);
        transform.Rotate(0, 0, -15);
        StartCoroutine(WaitFor());
    }
}

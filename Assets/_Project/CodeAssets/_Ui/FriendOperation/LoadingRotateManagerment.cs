using UnityEngine;
using System.Collections;

public class LoadingRotateManagerment : MonoBehaviour
{
	
    void OnEnable()
    {
        StartCoroutine(WaitFor());
    }

    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(0.04f);
        transform.Rotate(0, 0, -15);
        StartCoroutine(WaitFor());
    }
}

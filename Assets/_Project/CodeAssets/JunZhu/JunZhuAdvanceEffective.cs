using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunZhuAdvanceEffective : MonoBehaviour
{
    [HideInInspector]
    public int index = 0;
    private List<Vector3> listVect = new List<Vector3>();
	void Start ()
    {
        Vector3 [] vec = {new Vector3(136,121,0),new Vector3(-151,121,0)};
        listVect.AddRange(vec);
        Move();
	}
    
    void Move()
    {
        transform.GetComponent<TweenPosition>().from = transform.localPosition;
        transform.GetComponent<TweenPosition>().to = listVect[index];
        transform.GetComponent<TweenPosition>().duration =1f;
        transform.GetComponent<TweenPosition>().enabled = true;
        EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, SelfDestroy);
    
    }

    void SelfDestroy()
    {

        Destroy(this.gameObject);
     
    }
	 

}

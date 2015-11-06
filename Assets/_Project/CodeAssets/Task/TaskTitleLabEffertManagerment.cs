using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TaskTitleLabEffertManagerment : MonoBehaviour 
{
    private List<Vector3> listVect = new List<Vector3>();
    void Start()
    {
        Vector3[] vec = { new Vector3(-80, 70, 0) };
        listVect.AddRange(vec);
        Move();
       
    }

    void Move()
    {
        transform.GetComponent<TweenAlpha>().from = 1.0f;
        transform.GetComponent<TweenAlpha>().to = 0.2f;
        transform.GetComponent<TweenAlpha>().duration = 0.3f;
        transform.GetComponent<TweenAlpha>().enabled = true;
       
        transform.GetComponent<TweenPosition>().from = transform.localPosition;
        transform.GetComponent<TweenPosition>().to = listVect[0];
        transform.GetComponent<TweenPosition>().duration = 1.0f;
        transform.GetComponent<TweenPosition>().enabled = true;
        EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, SelfDestroy);

    }

    void SelfDestroy()
    {
        Destroy(this.gameObject);

    }
  
   
}

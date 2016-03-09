using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorshipMoveItemManagerment : MonoBehaviour 
{
    public UILabel m_LabCount;
    public Vector3 m_Target = new Vector3(-390, 190, 0);

    void Start () 
    {
	
	}
    
    public void ShowInfo(Vector3 from)
    {
        m_LabCount.text = "";
        if (transform.GetComponent<TweenPosition>() == null)
        {
            gameObject.AddComponent<TweenPosition>();
            transform.GetComponent<TweenPosition>().enabled = false;
        }
        transform.GetComponent<TweenPosition>().from = from;
      
   
        {
            transform.GetComponent<TweenPosition>().to = m_Target;
        }
        transform.GetComponent<TweenPosition>().duration = 0.6f;
        transform.GetComponent<TweenPosition>().enabled = true;
        EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, DestroySelf);
    }

    void DestroySelf()
    {
        EventDelegate.Remove(transform.GetComponent<TweenPosition>().onFinished, DestroySelf);
        Destroy(gameObject);
    }
	
 
}

using UnityEngine;
using System.Collections;

public class TaskButtonEffectManagerment : MonoBehaviour {
	void Start ()
    {
	
	}
    public void ButtonState( bool isTouch)
    {
        if (isTouch)
        {
            if (gameObject.GetComponent<TweenScale>() == null)
            {
                gameObject.AddComponent<TweenScale>();
                gameObject.GetComponent<TweenScale>().enabled = false;
            }
            gameObject.GetComponent<TweenScale>().from = new Vector3(0.9f, 0.9f, 1);
            gameObject.GetComponent<TweenScale>().to = new Vector3(1f, 1f, 1f);
            gameObject.GetComponent<TweenScale>().duration = 0.1f;
            gameObject.GetComponent<TweenScale>().enabled = true;
            EventDelegate.Add(gameObject.GetComponent<TweenScale>().onFinished, ScaleDestroy);
        }
        else
        {
            if (gameObject.GetComponent<TweenScale>() == null)
            {
                gameObject.AddComponent<TweenScale>();
                gameObject.GetComponent<TweenScale>().enabled = false;
            }
            gameObject.GetComponent<TweenScale>().from = new Vector3(1f, 1f, 1);
            gameObject.GetComponent<TweenScale>().to = new Vector3(0.9f, 0.9f, 1);
            gameObject.GetComponent<TweenScale>().duration = 0.1f;
            gameObject.GetComponent<TweenScale>().enabled = true;

            EventDelegate.Add(gameObject.GetComponent<TweenScale>().onFinished, ScaleDestroy);
        }
    }
    void ScaleDestroy()
    {
        EventDelegate.Remove(gameObject.GetComponent<TweenScale>().onFinished, ScaleDestroy);

        Destroy(gameObject.GetComponent<TweenScale>());
    }
}

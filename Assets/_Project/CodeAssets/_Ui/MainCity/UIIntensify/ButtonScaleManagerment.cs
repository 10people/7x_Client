using UnityEngine;
using System.Collections;

public class ButtonScaleManagerment : MonoBehaviour
{
	void Start ()
    {
	
	}
    public void ButtonsControl(bool colliderEnable,float scale = 1.2f)
    {
        if (gameObject.GetComponent<TweenScale>() == null)
        {
            gameObject.gameObject.AddComponent<TweenScale>();
            gameObject.gameObject.AddComponent<TweenScale>().enabled = false;
        }
        if (colliderEnable)
        {
            gameObject.GetComponent<TweenScale>().from = new Vector3(1.0f, 1.0f, 1.0f);
            gameObject.GetComponent<TweenScale>().to = new Vector3(scale, scale, 1.0f);
        }
        else
        {
            gameObject.GetComponent<TweenScale>().from = new Vector3(scale, scale, 1.0f);
            gameObject.GetComponent<TweenScale>().to = new Vector3(1.0f, 1.0f, 1.0f);
        }

        gameObject.GetComponent<TweenScale>().duration = 0.08f;
        gameObject.GetComponent<TweenScale>().enabled = true;
        EventDelegate.Add(gameObject.GetComponent<TweenScale>().onFinished, TweenColorDestroy);
    }

    void TweenColorDestroy()
    {
        if (gameObject.GetComponent<TweenScale>() != null)
        {
            EventDelegate.Remove(gameObject.GetComponent<TweenScale>().onFinished, TweenColorDestroy);
            Destroy(gameObject.GetComponent<TweenScale>());
        }
    }
}

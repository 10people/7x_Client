using UnityEngine;
using System.Collections;

public class FadeInOrOutManagerment : MonoBehaviour
{
   public delegate void OnFinish(int index);
    OnFinish OnCallBack;
    private bool _IsFadein;
    public void FadeEffect(OnFinish calback = null, bool is_out = false)
    {
        _IsFadein = is_out;
        if (calback != null)
        {
            OnCallBack = calback;
        }
        if (gameObject.GetComponent<TweenAlpha>() == null)
        {
            gameObject.gameObject.AddComponent<TweenAlpha>();
            gameObject.gameObject.AddComponent<TweenAlpha>().enabled = false;
        }
        if (is_out)
        {
            gameObject.GetComponent<TweenAlpha>().from = 0.1f;
            gameObject.GetComponent<TweenAlpha>().to = 1f;
            gameObject.GetComponent<TweenAlpha>().duration = 0.2f;
        }
        else
        {
            gameObject.GetComponent<TweenAlpha>().from = 1.0f;
            gameObject.GetComponent<TweenAlpha>().to = 0.1f;
            gameObject.GetComponent<TweenAlpha>().duration = 0.5f;
        }
 
        gameObject.GetComponent<TweenAlpha>().enabled = true;
 
        EventDelegate.Add(gameObject.GetComponent<TweenAlpha>().onFinished, TweenColorDestroy);
    }
    void TweenColorDestroy()
    {
        if (gameObject.GetComponent<TweenAlpha>() != null)
        {
            EventDelegate.Remove(gameObject.GetComponent<TweenAlpha>().onFinished, TweenColorDestroy);
            Destroy(gameObject.GetComponent<TweenAlpha>());
        }

        if (OnCallBack != null)
        {
            if (_IsFadein)
            {
                OnCallBack(0);
            }
            else
            {
                OnCallBack(1);
            }
        }
    }
}

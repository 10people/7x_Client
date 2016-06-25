using UnityEngine;
using System.Collections;

public class UIlabelBreathingEffectManagerment : MonoBehaviour {
 
	void Start ()
    {
        BreathingEffect();
    }

    void BreathingEffect()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0.5f, "time",0.5f
            , "easetype", "easeInOutSine", "onupdate", "OnUpdateCycleValue", "oncomplete", "OnEndCycleValue"));
    }

    void TweenColorDestroy()
    {
        //Destroy(transform.GetComponent<TweenColor>());
        transform.GetComponent<TweenAlpha>().from = 100 / 255.0f;
        transform.GetComponent<TweenAlpha>().to = 1.0f ;
        transform.GetComponent<TweenAlpha>().duration = 0.5f;
        transform.GetComponent<TweenAlpha>().delay = 0.5f;
        transform.GetComponent<TweenAlpha>().enabled = true;
        EventDelegate.Add(transform.GetComponent<TweenAlpha>().onFinished, BreathingEffect);
    }
}

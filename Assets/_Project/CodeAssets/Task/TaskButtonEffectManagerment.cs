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
          transform.localScale = new Vector3(1f, 1f, 1);
        }
        else
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 1);
        }
    }
    
}

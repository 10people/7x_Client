using UnityEngine;
using System.Collections;

public class WorshipEffertMoveManagerment : MonoBehaviour
{
    public UILabel m_LabTiLi;
    public UILabel m_LabGongXian;
    public void ShowInfo(string countTiLi, string countGongXian, Vector3 from, Vector3 topos)
    {
        m_LabTiLi.text = MyColorData.getColorString(4,"+" +countTiLi);
        m_LabGongXian.text = MyColorData.getColorString(4,"+" + countGongXian);
        if (transform.GetComponent<TweenPosition>() == null)
        {
            gameObject.AddComponent<TweenPosition>();
            transform.GetComponent<TweenPosition>().enabled = false;
        }
        transform.GetComponent<TweenPosition>().from = from;

        transform.GetComponent<TweenPosition>().to = topos;
        
        transform.GetComponent<TweenPosition>().duration = 1.2f;
        transform.GetComponent<TweenPosition>().enabled = true;
        EventDelegate.Add(transform.GetComponent<TweenPosition>().onFinished, DestroySelf);

    }

    void DestroySelf()
    {
        EventDelegate.Remove(transform.GetComponent<TweenPosition>().onFinished, DestroySelf);
        Destroy(gameObject);
    }
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MoveCloneManangement : MonoBehaviour 
{
    public UILabel m_LabComtent;
    public GameObject m_MainParent;
    private GameObject clone;
    public  void CreateClone(GameObject move, int content)
    {
        if (move != null)
        {
            clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
            clone.transform.localPosition = move.transform.localPosition;
            clone.transform.localRotation = move.transform.localRotation;
            clone.transform.localScale = move.transform.localScale;
        }
        else
        {
            clone = m_LabComtent.gameObject;
        }
        clone.GetComponent<UILabel>().text = "";
        if (content < 0)
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(5, content.ToString());
        }
        else
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + content.ToString());
        }

        clone.AddComponent(typeof(TweenPosition));
        clone.AddComponent(typeof(TweenAlpha));
        if (move != null)
        {
            clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
            clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        }
        else
        {
            clone.GetComponent<TweenPosition>().from = clone.transform.localPosition;
            clone.GetComponent<TweenPosition>().to = clone.transform.localPosition + Vector3.up * 40;
        }
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiForee(m_MainParent));
    }

    IEnumerator WatiForee(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }


    
    public void CreateMove(GameObject move, string content)
    {
        if (move != null)
        {
            clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
            clone.transform.localPosition = move.transform.localPosition;
            clone.transform.localRotation = move.transform.localRotation;
            clone.transform.localScale = move.transform.localScale;
        }
        else
        {
            clone = m_LabComtent.gameObject;
        }
        clone.GetComponent<UILabel>().text = "";

        clone.GetComponent<UILabel>().text = MyColorData.getColorString(1, content);


        clone.AddComponent(typeof(TweenPosition));
        clone.AddComponent(typeof(TweenAlpha));
        if (move != null)
        {
            clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
            clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        }
        else
        {
            clone.GetComponent<TweenPosition>().from = clone.transform.localPosition;
            clone.GetComponent<TweenPosition>().to = clone.transform.localPosition + Vector3.up * 40;
        }
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiFor(m_MainParent));
    }
    IEnumerator WatiFor(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
        CityGlobalData.m_isCreatedMoveLab = false;
    }
    
}

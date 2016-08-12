using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Toggles controller, call OnToggleClick manully.
/// </summary>
public class TogglesControl : MonoBehaviour
{
    /// <summary>
    /// Listeners binded to toggle objects, you can only PLUS new delegate here!
    /// </summary>
    public List<EventIndexHandle> TogglesEvents;

    /// <summary>
    /// Widget depth move value.
    /// </summary>
    public int MoveDepth;

    /// <summary>
    /// Widget a change value.
    /// </summary>
    public float ChangeA;

    private GameObject TriggeredObject;

	public bool IsLeft;

	public GameObject mBtnBg;
    /// <summary>
    /// Call this method manully.
    /// </summary>
    /// <param name="index"></param>
    public void OnToggleClick(int index)
    {
		if(IsLeft)
		{
			for(int i = 0 ; i < TogglesEvents.Count ;i++)
			{
				TogglesEvents[i].gameObject.GetComponent<UISprite>().spriteName = "sprite"+(i+1).ToString();
			}
			
			if (MoveDepth != 0)
			{
				SetObjectsDepth(TogglesEvents[index].gameObject, MoveDepth, true);
			}
		}
		else
		{
			TogglesEvents.ForEach(item => SetColorA(item.gameObject, 0.5f));
		}


	
		SetColorA(TogglesEvents[index].gameObject, index);

        TriggeredObject = TogglesEvents[index].gameObject;
    }


    private EventIndexHandle GetToggleEventByIndex(int index)
    {
        return TogglesEvents.Where(item => item.m_SendIndex == index).FirstOrDefault();
    }

    /// <summary>
    /// Set object and object's children widget depth forward or backward.
    /// </summary>
    /// <param name="go">the object</param>
    /// <param name="translateValue">changed depth</param>
    /// <param name="isForward">is depth forward</param>
    private void SetObjectsDepth(GameObject go, int translateValue, bool isForward)
    {
        var widgets = go.GetComponentsInChildren<UIWidget>(true);
        foreach (var widget in widgets)
        {
            if (isForward)
            {
                widget.depth += translateValue;
            }
            else
            {
                widget.depth -= translateValue;
            }
        }
    }

    /// <summary>
    /// Set object widget color a.
    /// </summary>
    /// <param name="go">the object</param>
    /// <param name="a">the value</param>
    private void SetColorA(GameObject go, float a)
    {
		if (IsLeft) {
			if (a == 0) {
				return;
			}
			string mstr = "";
			switch ((int)a) {
			case 6:
				TogglesEvents [0].gameObject.GetComponent<UISprite> ().spriteName = "Bigsprite5";
				go = TogglesEvents [0].gameObject;
				break;
			case 2:
				TogglesEvents [1].gameObject.GetComponent<UISprite> ().spriteName = "Bigsprite2";
				go = TogglesEvents [1].gameObject;
				break;
			case 3:
				TogglesEvents [2].gameObject.GetComponent<UISprite> ().spriteName = "Bigsprite3";
				go = TogglesEvents [2].gameObject;
				break;
			case 1:
				TogglesEvents [3].gameObject.GetComponent<UISprite> ().spriteName = "Bigsprite4";
				go = TogglesEvents [3].gameObject;
				break;
			}
			//		TogglesEvents[a].gameObject.GetComponent<UISprite>().spriteName = "Bigsprite"+(a+1).ToString();
			Hashtable move = new Hashtable ();
			move.Add ("time", 0.2f);
			move.Add ("position", go.transform.localPosition);
			move.Add ("islocal", true);
			move.Add ("easetype", iTween.EaseType.linear);
			iTween.MoveTo (mBtnBg, move);
		}
		else 
		{
	        var widget = go.GetComponent<UIWidget>();
	        widget.color = new Color(a, a, a, 1f);
		}


    }
}

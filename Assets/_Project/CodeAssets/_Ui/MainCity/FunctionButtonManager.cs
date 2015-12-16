using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FunctionButtonManager : MonoBehaviour, IComparable<FunctionButtonManager>
{
	[HideInInspector]public int m_iWantToX;
	[HideInInspector]public int m_iWantToY;
	[HideInInspector]public float m_iWantMoveX;
	[HideInInspector]public float m_iWantMoveY;
	[HideInInspector]public int m_iMoveIndex = 0;
	[HideInInspector]public int m_iMoveNum = 10;
	public int m_index;
	public MYNGUIButtonMessage m_MYNGUIButtonMessage;
	private FunctionOpenTemp m_FuncTemplate;
	private int m_RankIndex;//排序
	private int m_Type;

    public void SetData(FunctionOpenTemp template)
    {
        m_FuncTemplate = template;

        m_index = template.m_iID;
        if (m_index < 0)
        {
            m_ButtonSprite.spriteName = "NULL";
        }
        else
        {
            m_ButtonSprite.spriteName = "Function_" + m_index;
        }

        m_RankIndex = template.rank;
        m_Type = template.type;
    }

    /// <summary>
    /// Locked item cannot be enabled.
    /// </summary>
    public static List<int> s_LockedList = new List<int>();

    public UISprite m_ButtonSprite;
    public GameObject RedAlertObject;

    /// <summary>
    /// Is red alert object showed or particle effect showed.
    /// </summary>
    [HideInInspector]
    public bool IsAlertShowed;

    /// <summary>
    /// Show red alert object.
    /// </summary>
    public void ShowRedAlert()
    {
        //Cancel show sprite when null button.
        if (m_index < 0) return;

        //cancel show locked button
        if (s_LockedList.Contains(m_index)) return;

        IsAlertShowed = true;
        RedAlertObject.SetActive(true);
    }

    public void HideRedAlert()
    {
        IsAlertShowed = false;
        RedAlertObject.SetActive(false);
    }

	public void GoToPos()
	{
		gameObject.transform.localPosition = new Vector3(m_iWantToX, m_iWantToY, 0);
	}

	public bool Move()
	{
		if(m_iMoveIndex != m_iMoveNum)
		{
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + m_iWantMoveX, gameObject.transform.localPosition.y + m_iWantMoveY, 0);
			m_iMoveIndex ++;
			return false;
		}
		else
		{
			GoToPos();
			return true;
		}
	}

	public void setMoveDis()
	{
		m_iWantMoveX = (m_iWantToX - gameObject.transform.localPosition.x) / m_iMoveNum;
		
		m_iWantMoveY = (m_iWantToY - gameObject.transform.localPosition.y) / m_iMoveNum;

		m_iMoveIndex = 0;
	}

    /// <summary>
    /// for buttons compare, not used in this version.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(FunctionButtonManager other)
    {
        if (other == null)
        {
            return 1;
        }
        else
        {
            return m_RankIndex.CompareTo(other.m_RankIndex);
        }
    }
}

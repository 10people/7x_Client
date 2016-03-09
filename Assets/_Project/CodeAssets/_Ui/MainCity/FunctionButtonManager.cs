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
	public UISprite m_ButtonSprite;
	public GameObject m_RedAlertObject;
	public UILabel m_LabelButtonName;
	public UILabel m_LabelTime;
	public int m_index;
	public MYNGUIButtonMessage m_MYNGUIButtonMessage;
	public BoxCollider m_Coll;
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
		m_LabelButtonName.text = template.Des;
		m_ButtonSprite.SetDimensions(template.m_iImageW, template.m_iImageH);
		m_Coll.size = new Vector3(template.m_iImageW, template.m_iImageH, 0);
		if(template.m_iShowDesc == 1)
		{
			m_LabelButtonName.gameObject.SetActive(true);
		}
		else
		{
			m_LabelButtonName.gameObject.SetActive(false);
		}
		if(m_FuncTemplate.m_iRedType != 0)
		{
			GameObject.Destroy(m_RedAlertObject.GetComponent<UISprite>());
		}
    }

	public void Teshu()
	{
		if(m_FuncTemplate.m_iID == 17)
		{
			var temp = FunctionUnlock.templates.Where(item => !FunctionOpenTemp.m_EnableFuncIDList.Contains(item.id));
			m_ButtonSprite.spriteName = "Function_" + temp.First().id;
		}
		else if(m_FuncTemplate.m_iID == 15)
		{
			MainCityUI.m_MainCityUI.m_MainCityUIRB.TimeCalcRoot.SetActive(true);
			MainCityUI.m_MainCityUI.m_MainCityUIRB.TimeCalcRoot.transform.position = m_ButtonSprite.transform.position;
			MainCityUI.m_MainCityUI.m_MainCityUIRB.TimeCalcRoot.transform.parent = m_ButtonSprite.transform.parent;
		}
		else if(m_FuncTemplate.m_iID == 311)
		{
			if( !UI3DEffectTool.HaveAnyFx( m_RedAlertObject ) )
			{
				MainCityUI.SetRedAlert(311, true);
			}
		}
		else if(m_FuncTemplate.m_iID == 139)
		{
//			if(!m_LabelTime.gameObject.activeSelf)
//			{
//				if()
//				int time = 100000;
//				for(int i = 0; i < MainCityUI.m_MainCityUI.m_FuLiHuoDongResp.xianshi.Count; i ++)
//				{
//					if(MainCityUI.m_MainCityUI.m_FuLiHuoDongResp.xianshi[i].isCanGet)
//					{
//						TimeLabelHelper.Instance.setTimeLabel(m_LabelTime, "领取", -1);
//						return;
//					}
//					else
//					{
//						if(MainCityUI.m_MainCityUI.m_FuLiHuoDongResp.xianshi[i].remainTime < time)
//						{
//							time = MainCityUI.m_MainCityUI.m_FuLiHuoDongResp.xianshi[i].remainTime;
//						}
//					}
//				}
//				TimeLabelHelper.Instance.setTimeLabel(m_LabelTime, "领取", time);
//			}
		}
	}

    /// <summary>
    /// Locked item cannot be enabled.
    /// </summary>
    public static List<int> s_LockedList = new List<int>();

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

		m_RedAlertObject.SetActive(true);
		if(m_FuncTemplate.m_iRedType != 0)
		{
			UI3DEffectTool.ClearUIFx(m_RedAlertObject);
			m_RedAlertObject.transform.localPosition = Vector3.zero;
			if( !UI3DEffectTool.HaveAnyFx( m_RedAlertObject ) ){
				UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_RedAlertObject, EffectTemplate.getEffectTemplateByEffectId(100185).path);
			}
		}
		else
		{
			m_RedAlertObject.transform.localPosition = new Vector3(-22, 30, 0);
		}

        IsAlertShowed = true;
        
    }

    public void HideRedAlert()
    {
        IsAlertShowed = false;
		m_RedAlertObject.SetActive(false);
		if(m_FuncTemplate.m_iRedType != 0)
		{
			UI3DEffectTool.ClearUIFx(m_RedAlertObject);
//			m_RedAlertObject
		}
    }

	public void GoToPos()
	{
		gameObject.transform.localPosition = new Vector3(m_iWantToX, m_iWantToY, 0);
		if(!m_RedAlertObject.activeSelf && m_FuncTemplate.m_show_red_alert)
		{
			MainCityUI.SetRedAlert(m_index, m_FuncTemplate.m_show_red_alert);
		}
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

	public void setRed(bool show)
	{
		if(show)
		{
			ShowRedAlert();
		}
		else
		{
			HideRedAlert();
		}
	}
}

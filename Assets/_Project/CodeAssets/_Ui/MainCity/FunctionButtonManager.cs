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
	public bool m_isSuperAlert = false;
	public UISprite m_ButtonSprite;
	public BoxChangeScale m_BoxChangeScale;
	public GameObject m_RedAlertObject;
	public UILabel m_LabelButtonName;
	public UILabel m_LabelTime;
	public UILabel m_LabelNum;
	public int m_index;
	public MYNGUIButtonMessage m_MYNGUIButtonMessage;
	public BoxCollider m_Coll;
	private FunctionOpenTemp m_FuncTemplate;
	private int m_RankIndex;//排序
	private int m_Type;
	private bool m_isTeshu = false;

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
		if(template.m_iNum > 0)
		{
			m_LabelNum.text = template.m_iNum.ToString();
			m_LabelNum.gameObject.SetActive(true);
		}
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
		if(m_FuncTemplate.m_iRedType == 1)
		{
			GameObject.Destroy(m_RedAlertObject.GetComponent<UISprite>());
		}
    }

	public void Teshu()
	{
		m_isTeshu = true;
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
		else if(m_FuncTemplate.m_iID == 142)
		{
			if( !UI3DEffectTool.HaveAnyFx( m_RedAlertObject ) )
			{
				MainCityUI.SetRedAlert(142, true);
			}
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
			Global.ScendNull(ProtoIndexes.C_FULIINFO_REQ);
		}
		else if(m_FuncTemplate.m_iID == 141)
		{
			setSuperAlert(true);
		}
		else if(m_FuncTemplate.m_iID == 104)
		{
			if(JunZhuData.Instance().m_junzhuInfo.lianMengId == 0)
			{
				setSuperAlert(true);
			}
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
		if(m_isSuperAlert)
		{
			return;
		}
        //Cancel show sprite when null button.
        if (m_index < 0) return;

        //cancel show locked button
        if (s_LockedList.Contains(m_index)) return;

		switch(m_FuncTemplate.m_iRedType)
		{
		case 0:
			m_RedAlertObject.SetActive(true);
			m_RedAlertObject.transform.localPosition = new Vector3(-22, 30, 0);
			break;
		case 1:
			m_RedAlertObject.SetActive(true);
			UI3DEffectTool.ClearUIFx(m_RedAlertObject);
			m_RedAlertObject.transform.localPosition = Vector3.zero;
			if( !UI3DEffectTool.HaveAnyFx( m_RedAlertObject ) ){
				UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_RedAlertObject, EffectTemplate.getEffectTemplateByEffectId(100185).path);
			}
			break;
		case 2:
			m_BoxChangeScale.enabled = true;
			break;
		case 3:
			EffectTool.OpenMultiUIEffect_ById(m_ButtonSprite.gameObject, 223, 224, 225);
			break;
		case 4:
			m_RedAlertObject.SetActive(true);
			m_RedAlertObject.transform.localPosition = new Vector3(-15, 15, 0);
			break;
		case 5:
			m_RedAlertObject.SetActive(true);
			UISprite sprite = m_RedAlertObject.GetComponent<UISprite>();
			sprite.spriteName = "RedTanhao";
			break;
		}

        IsAlertShowed = true;
        
    }

    public void HideRedAlert()
    {
        IsAlertShowed = false;
		m_RedAlertObject.SetActive(false);
		switch(m_FuncTemplate.m_iRedType)
		{
		case 0:
			m_RedAlertObject.SetActive(false);
			break;
		case 1:
			m_RedAlertObject.SetActive(false);
			UI3DEffectTool.ClearUIFx(m_RedAlertObject);
			break;
		case 2:
			m_BoxChangeScale.enabled = false;
			break;
		case 3:
			EffectTool.CloseMultiUIEffect_ById(m_ButtonSprite.gameObject, 223, 224, 225);
			break;
		case 4:
			m_RedAlertObject.SetActive(false);
			break;
		case 5:
			m_RedAlertObject.SetActive(false);
			break;
		}
		if(m_FuncTemplate.m_iRedType != 0)
		{

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
			if(!m_isTeshu)
			{
				Teshu();
				m_isTeshu = true;
			}
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

	public void setNum(int num)
	{
		if(num > 0)
		{
			m_LabelNum.text = num.ToString();
			m_LabelNum.gameObject.SetActive(true);
		}
		else
		{
			m_LabelNum.gameObject.SetActive(false);
		}
	}

	public void setSuperAlert(bool isShow)
	{
		if(m_FuncTemplate.m_iID == 610)
		{
			Debug.Log(isShow);
		}
		if(isShow)
		{
			EffectTool.OpenMultiUIEffect_ById(m_ButtonSprite.gameObject, 223, 224, 225);
			HideRedAlert();
			m_isSuperAlert = true;
		}
		else
		{
			EffectTool.CloseMultiUIEffect_ById(m_ButtonSprite.gameObject, 223, 224, 225);
			setRed(m_FuncTemplate.m_show_red_alert);
			setNum(m_FuncTemplate.m_iNum);
			m_isSuperAlert = false;
			m_ButtonSprite.depth = 2;
		}
	}
}

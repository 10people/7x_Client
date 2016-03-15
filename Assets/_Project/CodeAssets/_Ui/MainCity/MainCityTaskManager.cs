using UnityEngine;
using System.Collections;

public class MainCityTaskManager : MonoBehaviour
{
	[HideInInspector]public int m_iWantToX;
	[HideInInspector]public int m_iWantToY;
	[HideInInspector]public float m_iWantMoveX;
	[HideInInspector]public float m_iWantMoveY;
	[HideInInspector]public int m_iMoveIndex = 0;
	[HideInInspector]public int m_iMoveNum = 10;

	private int m_iNum;
	public int m_iThisPanelState;
	public UISprite m_UISpriteBG;
	public UILabel m_UILabelCur;
	public UILabel m_UILabelState;
	public UILabel m_UILabelClick;

	public UILabel m_UILabelChange;
	public GameObject m_objChangeEff;
	public GameObject m_objFinishEff;
	public GameObject m_objRefreshEff;
	public GameObject m_objAnimationEnd;
	public GameObject m_objAlret;
	public GameObject m_objUpEff;
	public GameObject m_objDownEff;
	private int m_iRefreshNum = 0;
	private int m_iShowID = -1;
	private int m_iState;

	public enum TaskAnimationSatatae
	{
		Def 	= 1,
		Eff 	= 2,
		Label	= 3,
		Finish	= 4,
		Close	= 5,
	}

	private TaskAnimationSatatae m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
	// Use this for initialization
	void Start ()
	{
		if(m_iThisPanelState == 0)
		{
			UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_objUpEff, EffectTemplate.getEffectTemplateByEffectId( 620212 ).path);
			UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_objDownEff, EffectTemplate.getEffectTemplateByEffectId( 620213 ).path);
		}
	}

	// Update is called once per frame
	void Update () {
//		switch(m_TaskAnimationSatatae)
//		{
//		case TaskAnimationSatatae.Eff:
//			m_iNum ++;
//			if(m_iNum == 10)
//			{
//				m_iNum = 0;
//				m_TaskAnimationSatatae = TaskAnimationSatatae.Label;
//			}
//			break;
//		case TaskAnimationSatatae.Label:
//			float move = (291f - m_UILabelCur.transform.localPosition.x) / 4.8f;
//			if(move <= 1)
//			{
//				m_UILabelChange.gameObject.SetActive(false);
//				m_UILabelCur.transform.localPosition = new Vector3(0,0,0);
//				if(TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress < 0)
//				{
//					m_UILabelCur.text = "          任务完成";
//				}
//				else
//				{
//					m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
//					m_objAnimationEnd.SetActive(false);
//					ClientMain.closePopUp();
//				}
//				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
//			}
//			else
//			{
//				m_UILabelCur.transform.localPosition = new Vector3(m_UILabelCur.transform.localPosition.x + move,0,0);
//				m_UILabelChange.transform.localPosition = new Vector3(m_UILabelChange.transform.localPosition.x + move,0,0);
//			}
//			break;
//		case TaskAnimationSatatae.Finish:
//			m_iNum ++;
//			if(m_iNum <= 10)
//			{
//				float tempScal = 2.0f - m_iNum * 0.1f;
//				m_UISpriteBG.gameObject.transform.localScale = new Vector3(tempScal,tempScal,tempScal);
//				if(m_iNum == 10)
//				{
//					m_UISpriteBG.gameObject.transform.localScale = new Vector3(1,1,1);
//					UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objFinishEff, EffectTemplate.getEffectTemplateByEffectId( 100175 ).path);
//				}
//			}
//			else if(m_iNum == 60)
//			{
//				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
//				ClientMain.closePopUp();
//			}
//			break;
//		case TaskAnimationSatatae.Close:
//			m_iNum ++;
//			if(m_iNum == 3)
//			{
//				ClientMain.closePopUp();
//				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
//			}
//			break;
//		}
//		if(m_iState >= 0)
//		{
//			m_iRefreshNum ++;
//			if(m_iRefreshNum == 500)
//			{
//				m_iRefreshNum = 0;
//				UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objRefreshEff, EffectTemplate.getEffectTemplateByEffectId( 100184 ).path);
//			}
//		}
	}

	public bool setChange(string data)
	{
		m_iNum = 0;
		m_iRefreshNum = 0;
		if((m_iState == TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress && m_iShowID == TaskData.Instance.ShowId) || m_iShowID == -1 || TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress < 0)
		{
			m_TaskAnimationSatatae = TaskAnimationSatatae.Close;
			return true;
		}
		else
		{
			m_iState = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress;
			m_iShowID = TaskData.Instance.ShowId;
		}

		if(TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress < 0)
		{
			m_UILabelCur.text = "          任务完成";
			m_UISpriteBG.gameObject.SetActive(true);
			m_UISpriteBG.gameObject.transform.localScale = new Vector3(2,2,2);
			m_TaskAnimationSatatae = TaskAnimationSatatae.Finish;
			UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objChangeEff, EffectTemplate.getEffectTemplateByEffectId( 100177 ).path);
		}
		else
		{
			UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objChangeEff, EffectTemplate.getEffectTemplateByEffectId( 100177 ).path);
			m_UILabelChange.gameObject.SetActive(true);
			m_UILabelChange.transform.localPosition = new Vector3(-291, 0 , 0);
			m_UILabelChange.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
			m_TaskAnimationSatatae = TaskAnimationSatatae.Eff;
			m_UISpriteBG.gameObject.SetActive(false);
		}
		m_objAnimationEnd.SetActive(true);
		return true;
	}

	public void setData(int taskID)
	{
		m_objAlret.SetActive(false);
		m_iShowID = taskID;
		if(m_iShowID == -1)
		{
			closeShow();
			return;
		}
		m_iState = TaskData.Instance.m_TaskInfoDic[m_iShowID].progress;
		m_UILabelState.text = TaskData.Instance.m_TaskInfoDic[m_iShowID].desc;
		if(m_iThisPanelState == 0)
		{
			m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[m_iShowID].title;
		}
		else
		{
			m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[m_iShowID].title;
		}
		if(m_iState < 0)
		{
			m_UILabelClick.text = "点击领奖";
			if(m_iThisPanelState != 0)
			{
				m_objAlret.SetActive(true);
			}
		}
		else
		{
			m_UILabelClick.text = "点击前往";
		}
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
	}

	public void closeShow()
	{
		gameObject.SetActive(false);
	}
}

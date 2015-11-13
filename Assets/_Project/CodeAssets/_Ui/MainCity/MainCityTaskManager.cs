using UnityEngine;
using System.Collections;

public class MainCityTaskManager : MYNGUIPanel 
{
	private int m_iNum;
	public UISprite m_UISpriteBG;
	public UILabel m_UILabelCur;
	public UILabel m_UILabelChange;
	public GameObject m_objChangeEff;
	public GameObject m_objFinishEff;
	public GameObject m_objRefreshEff;
	public GameObject m_objAnimationEnd;
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
		m_iState = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress;
		m_iShowID = TaskData.Instance.ShowId;
		if(MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.m_UILabelCur.text == "")
		{
			if(TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress < 0)
			{
				MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.m_UILabelCur.text = "          任务完成";
				MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.m_UISpriteBG.gameObject.SetActive(true);
			}
			else
			{
				MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(m_TaskAnimationSatatae)
		{
		case TaskAnimationSatatae.Eff:
			m_iNum ++;
			if(m_iNum == 10)
			{
				m_iNum = 0;
				m_TaskAnimationSatatae = TaskAnimationSatatae.Label;
			}
			break;
		case TaskAnimationSatatae.Label:
			float move = (291f - m_UILabelCur.transform.localPosition.x) / 4.8f;
			if(move <= 1)
			{
				m_UILabelChange.gameObject.SetActive(false);
				m_UILabelCur.transform.localPosition = new Vector3(0,0,0);
				if(TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].progress < 0)
				{
					m_UILabelCur.text = "          任务完成";
				}
				else
				{
					m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
					m_objAnimationEnd.SetActive(false);
					ClientMain.closePopUp();
				}
				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
			}
			else
			{
				m_UILabelCur.transform.localPosition = new Vector3(m_UILabelCur.transform.localPosition.x + move,0,0);
				m_UILabelChange.transform.localPosition = new Vector3(m_UILabelChange.transform.localPosition.x + move,0,0);
			}
			break;
		case TaskAnimationSatatae.Finish:
			m_iNum ++;
			if(m_iNum <= 10)
			{
				float tempScal = 2.0f - m_iNum * 0.1f;
				m_UISpriteBG.gameObject.transform.localScale = new Vector3(tempScal,tempScal,tempScal);
				if(m_iNum == 10)
				{
					m_UISpriteBG.gameObject.transform.localScale = new Vector3(1,1,1);
					UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objFinishEff, EffectTemplate.getEffectTemplateByEffectId( 100175 ).path);
				}
			}
			else if(m_iNum == 60)
			{
				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
				ClientMain.closePopUp();
			}
			break;
		case TaskAnimationSatatae.Close:
			m_iNum ++;
			if(m_iNum == 3)
			{
				ClientMain.closePopUp();
				m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
			}
			break;
		}
//		if(m_iState >= 0)
//		{
//			m_iRefreshNum ++;
//			if(m_iRefreshNum == 500)
//			{
//				m_iRefreshNum = 0;
//				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objRefreshEff, EffectTemplate.getEffectTemplateByEffectId( 100184 ).path);
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
			UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objChangeEff, EffectTemplate.getEffectTemplateByEffectId( 100177 ).path);
		}
		else
		{
			UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objChangeEff, EffectTemplate.getEffectTemplateByEffectId( 100177 ).path);
			m_UILabelChange.gameObject.SetActive(true);
			m_UILabelChange.transform.localPosition = new Vector3(-291, 0 , 0);
			m_UILabelChange.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
			m_TaskAnimationSatatae = TaskAnimationSatatae.Eff;
			m_UISpriteBG.gameObject.SetActive(false);
		}
		m_objAnimationEnd.SetActive(true);
		return true;
	}

	public void setData(string data)
	{
		m_UILabelCur.text = data;
	}

	public override void MYClick(GameObject ui)
	{
		m_objChangeEff.SetActive(false);
		m_UILabelChange.gameObject.SetActive(false);
		m_UILabelCur.transform.localPosition = new Vector3(0,0,0);
		m_UILabelCur.text = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId].title;
		m_objAnimationEnd.SetActive(false);
		ClientMain.closePopUp();
		m_TaskAnimationSatatae = TaskAnimationSatatae.Def;
	}
	
	public override void MYMouseOver(GameObject ui)
	{
	}
	
	public override void MYMouseOut(GameObject ui)
	{
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
	}
	
	public override void MYelease(GameObject ui)
	{
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//iTween.ValueTo(gameObject, iTween.Hash("from", -ClientMain.m_TotalWidthInCoordinate, "to", 0, "time", LabelMoveDuration, "easetype", "easeOutBack", "onupdate", "SetAlertEffectInfoPos", "oncomplete", "EnableAlertEffectClick"));

public class MainCityWantOpenManager : MYNGUIPanel 
{
	public UISprite m_bgH;
	public UISprite m_spriteIcon;
	public UILabel m_labelDes;
	public UILabel m_labelUnLock;
	public UILabel m_labelRenyi;
	public GameObject m_objMovePanel;
	public enum AnimationStatae
	{
		Bg		= 0,
		MoveC 	= 1,
		Click	= 2,
		MoveR   = 3,
	}

	public AnimationStatae m_AnimationState = AnimationStatae.Bg;
	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < FunctionUnlock.templates.Count; i ++)
		{
			if(!FunctionOpenTemp.IsHaveID(FunctionUnlock.templates[i].id))
			{
//				Debug.Log(FunctionUnlock.templates[i].id);
				m_spriteIcon.spriteName = "Function_" + FunctionUnlock.templates[i].id;
				m_labelDes.text = FunctionUnlock.templates[i].des1;
				m_labelUnLock.text = "解锁条件:" + FunctionUnlock.templates[i].des2;
				break;
			}
		}
		MainCityUI.TryAddToObjectList(gameObject);
		CityGlobalData.m_isRightGuide = true;
		setAnimation();
	}

	public void scaleOver()
	{

	}

	public override void MYClick(GameObject ui)
	{
		switch(m_AnimationState)
		{
		case AnimationStatae.Bg:
			break;
		case AnimationStatae.MoveC:
			break;
		case AnimationStatae.Click:
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);

//			m_AnimationState = AnimationStatae.MoveR;
//			setAnimation();
			break;
		case AnimationStatae.MoveR:
			break;
		}
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

	public void setAnimation()
	{
		switch(m_AnimationState)
		{
		case AnimationStatae.Bg:
			iTween.ValueTo(gameObject, iTween.Hash("from", 0, 
			                                       "to", 1, 
			                                       "time", 0.3f, 
			                                       "easetype",  "easeOutBack", 
			                                       "onupdate",  "upValue", 
			                                       "oncomplete", "End"));
			break;
		case AnimationStatae.MoveC:
			iTween.ValueTo(gameObject, iTween.Hash("from", -950, 
			                                       "to", 0, 
			                                       "time", 0.5f, 
			                                       "easetype",  "easeOutBack", 
			                                       "onupdate",  "upValue", 
			                                       "oncomplete", "End"));
			break;
		case AnimationStatae.Click:
			CycleTween.StartCycleTween(m_labelRenyi.gameObject, 1, 0.4f, 0.5f, OnUpdateAlertInfoLabelA);
			break;
		case AnimationStatae.MoveR:
			iTween.ValueTo(gameObject, iTween.Hash("from", 0, 
			                                       "to", 950, 
			                                       "time", 0.5f, 
			                                       "easetype",  "easeOutBack", 
			                                       "onupdate",  "upValue", 
			                                       "oncomplete", "End"));
			break;
		}
	}

	public void upValue(float value)
	{
		switch(m_AnimationState)
		{
		case AnimationStatae.Bg:
			m_bgH.gameObject.transform.localScale = new Vector3(1f, value, 1f);
			break;
		case AnimationStatae.MoveC:
			m_objMovePanel.transform.localPosition = new Vector3(value, 0f, 0f);
			break;
		case AnimationStatae.Click:
			break;
		case AnimationStatae.MoveR:
			m_objMovePanel.transform.localPosition = new Vector3(value, 0f, 0f);
			break;
		}
	}

	public void End()
	{
		switch(m_AnimationState)
		{
		case AnimationStatae.Bg:
			m_AnimationState = AnimationStatae.MoveC;
			setAnimation();
			break;
		case AnimationStatae.MoveC:
			m_AnimationState = AnimationStatae.Click;
			setAnimation();
			break;
		case AnimationStatae.Click:
			break;
		case AnimationStatae.MoveR:
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);
			break;
		}
	}
	
	private void OnUpdateAlertInfoLabelA(float value)
	{
		m_labelRenyi.color = new Color(m_labelRenyi.color.r, m_labelRenyi.color.g, m_labelRenyi.color.b, value);
	}
}

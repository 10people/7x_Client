using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAddZhanliManager
{
	private static GameObject m_ObjText;
	private UIAddZhanliData m_UIAddZhanliData;
	private int m_iCurNum;
	private int m_iNum;
	
	public void createText(int data)
	{
		m_iNum = 0;
		m_iCurNum = data;
		if(m_ObjText == null)
		{
			Global.ResourcesDotLoad("New/ZhanliUP", CachedTextCallback);
		}
		else
		{
			initText();
		}
	}
	
	private void CachedTextCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		m_ObjText = p_object as GameObject;
		initText();
	}
	
	private void initText()
	{
		if(m_UIAddZhanliData != null)
		{
			GameObject.Destroy(m_UIAddZhanliData.gameObject);
		}
		m_UIAddZhanliData = GameObject.Instantiate(m_ObjText).GetComponent<UIAddZhanliData>();
		m_UIAddZhanliData.m_UILabel.text = "战力提升：" + m_iCurNum;
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_UIAddZhanliData.m_Eff, EffectTemplate.getEffectTemplateByEffectId( 100179 ).path);

	}
	
	// Update is called once per frame
	public void Update ()
	{
		if(m_UIAddZhanliData != null)
		{
			m_iNum++;
			if(m_iNum == 100)
			{
				GameObject.Destroy(m_UIAddZhanliData.gameObject);
				m_UIAddZhanliData = null;
			}
		}
	}
}

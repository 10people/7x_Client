//#define DEBUG_RED_ALERT

//#define DEBUG_USE_PUSH_DATA



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class ReportTemplate : XmlLoadManager
{
	public int m_iID;
	public int m_iEvent;
	public int m_iSendCondition;
	public int m_iReceiveObject;
	public string m_sRecveiveScence;
	public string m_sPopupable;
	public int m_iLanguageID;
	public string m_sButton;
	public string m_sClickAward;
	public string m_ShowType;
	public string m_sReportTitle;
	public string m_sButtonfeedback;
	
	public static List<ReportTemplate> templates = new List<ReportTemplate>();
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
//		Debug.Log("=============2");
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ReportTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
//		Debug.Log("===========1");
		{
			templates.Clear();
		}
		XmlReader t_reader = null;
		
		if (obj != null)
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create(new StringReader(www.text));
		}
		
		bool t_has_items = true;
		
		do
		{
			t_has_items = t_reader.ReadToFollowing("ReportTemp");
			
			if (!t_has_items)
			{
				break;
			}
			
			ReportTemplate t_template = new ReportTemplate();
			
			{
				t_template.m_iID = ReadNextInt(t_reader);
				t_template.m_iEvent = ReadNextInt(t_reader);
				t_template.m_iSendCondition = ReadNextInt(t_reader);
				t_template.m_iReceiveObject = ReadNextInt(t_reader);
				t_template.m_sRecveiveScence = ReadNextString(t_reader);
				t_template.m_sPopupable = ReadNextString(t_reader);
				t_template.m_iLanguageID = ReadNextInt(t_reader);
				t_template.m_sButton = ReadNextString(t_reader);
				t_template.m_sClickAward = ReadNextString(t_reader);
				t_template.m_ShowType = ReadNextString(t_reader);
				t_template.m_sReportTitle = ReadNextString(t_reader);
				t_template.m_sButtonfeedback = ReadNextString(t_reader);
			}
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	
	
	public static ReportTemplate GetHeroSkillUpByID(int id)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].m_iID == id)
			{
				return templates[i];
			}
		}
		return null;
	}
}

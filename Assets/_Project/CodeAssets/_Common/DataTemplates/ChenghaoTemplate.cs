using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ChenghaoTemplate : XmlLoadManager {
	
	public int m_iID;

	public string m_sName;

	public int m_iDisID;

	public int m_iType;
	
	public static List<ChenghaoTemplate> templates = new List<ChenghaoTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Chenghao.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing("Chenghao");
			
			if (!t_has_items)
			{
				break;
			}
			
			ChenghaoTemplate t_template = new ChenghaoTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.m_iID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.m_sName = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.m_iDisID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.m_iType = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static string GetChenghaoName(int id)
	{
		foreach (ChenghaoTemplate template in templates)
		{
			if (template.m_iID == id)
			{
				return template.m_sName;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get ChenghaoTemplate with chenghaoID " + id);
		
		return null;
	}

	public static string GetChenghaoDis(int id)
	{
		foreach (ChenghaoTemplate template in templates)
		{
			if (template.m_iID == id)
			{
				return DescIdTemplate.GetDescriptionById(template.m_iDisID);
			}
		}
		
		Debug.LogError("XML ERROR: Can't get ChenghaoTemplate with chenghaoID " + id);
		
		return null;
	}
}

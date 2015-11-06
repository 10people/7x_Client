using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenJiaChengTemplate : XmlLoadManager {

	public int ID;
	
	public int levelMin;//最低符文等级
	
	public int addition;//加成效果
	
	public static List<FuWenJiaChengTemplate> templates = new List<FuWenJiaChengTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FuwenJiacheng.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing("FuwenJiacheng");
			
			if (!t_has_items)
			{
				break;
			}
			
			FuWenJiaChengTemplate t_template = new FuWenJiaChengTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.ID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.levelMin = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.addition = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	
	public static FuWenJiaChengTemplate GetFuWenJiaChengTemplateById (int id)
	{
		foreach (FuWenJiaChengTemplate template in templates)
		{
			if (template.ID == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get FuWenJiaChengTemplate with ID " + id);
		
		return null;
	}
}

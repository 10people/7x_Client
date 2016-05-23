using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenDuiHuanTemplate : XmlLoadManager {

	public int id;
	
	public int Num;
	
	public int itemID;
	
	public int cost;
	
	public static List<FuWenDuiHuanTemplate> templates = new List<FuWenDuiHuanTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FuwenDuihuan.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing("FuwenDuihuan");
			
			if (!t_has_items)
			{
				break;
			}
			
			FuWenDuiHuanTemplate t_template = new FuWenDuiHuanTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.Num = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.itemID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.cost = int.Parse(t_reader.Value);
					
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	public static FuWenDuiHuanTemplate GetFuWenDuiHuanTemplate_By_Id (int id)
	{
		foreach (FuWenDuiHuanTemplate template in templates)
		{
			if (template.id == id)
			{
				return template;
			}
		}
		return null;
	}
	public static List<int> GetFuWenDuiHuanTemplate_IDs ()
	{
		List<int > ids = new List<int> ();
		foreach (FuWenDuiHuanTemplate template in templates)
		{
			ids.Add(template.id);
		}
		return ids;
	}
}

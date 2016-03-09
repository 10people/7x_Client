using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LMTargetTemplate : XmlLoadManager{

	
	public int id;
	
	public string Name;
	
	public string condition;

	public static List<LMTargetTemplate> templates = new List<LMTargetTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LMTarget.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			templates.Clear();
		}
		
		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "LMTarget" );
			
			if( !t_has_items ){
				break;
			}
			
			LMTargetTemplate t_template = new LMTargetTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Name =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.condition = t_reader.Value;

			}
	
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	public static LMTargetTemplate getLMTargetTemplate_by_Id(int id)
	{
		foreach(LMTargetTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get LMTargetTemplate with starId " + id);
		
		return null;
	}
	public static int  getLMTargetTemplateCOunt()
	{
		return templates.Count;
	}
}

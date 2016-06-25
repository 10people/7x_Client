using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TalentArrTemplate : XmlLoadManager
{
	public int id;
	
	public int lv;

	public string des;
	
	public static List<TalentArrTemplate> templates= new List<TalentArrTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "TalentAttribute.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
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
			t_has_items = t_reader.ReadToFollowing( "TalentAttribute" );
			
			if( !t_has_items ){
				break;
			}
			
			TalentArrTemplate t_template = new TalentArrTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();

				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();
				t_reader.MoveToNextAttribute();

				t_reader.MoveToNextAttribute();
				t_template.des = t_reader.Value;
			}
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static TalentArrTemplate getByIDLV(int id, int lv)
	{
		foreach(TalentArrTemplate template in templates)
		{
			if(template.id == id && template.lv == lv)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get SkillTemplate with id " + id);
		
		return null;
	}
}

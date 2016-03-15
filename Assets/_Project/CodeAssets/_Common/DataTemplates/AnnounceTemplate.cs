using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class AnnounceTemplate : XmlLoadManager 
{
	public int id;

	public int type;

	public string condition;

	public string announcement;

	public static List<AnnounceTemplate> templates = new List<AnnounceTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "AnnounceTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
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
			t_has_items = t_reader.ReadToFollowing( "AnnounceTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			AnnounceTemplate t_template = new AnnounceTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.condition = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.announcement = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}	

	public static AnnounceTemplate GetAnnounceTempById (int id)
	{
		foreach(AnnounceTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get AnnounceTemplate with id " + id);
		
		return null;
	}

	public static bool IsSendAnnounce (int type,int condition)
	{
		foreach(AnnounceTemplate template in templates)
		{
			if(template.type == type && int.Parse (template.condition) == condition)
			{
				return true;
			}
		}

		return false;
	}
}

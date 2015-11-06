using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DiaoLuoTemplate : XmlLoadManager 
{

	public int itemId;
 
	public string PveIds;

	public static List<DiaoLuoTemplate> templates = new List<DiaoLuoTemplate>();

	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( PathManager.GetUrl(m_LoadPath + "DiaoLuo.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW p_www, string path, Object p_object ){
		{
			templates.Clear();
		}

		XmlReader t_reader = null;
		
		if( p_object != null ){
			TextAsset t_text_asset = p_object as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( p_www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "DiaoLuo" );
			
			if( !t_has_items ){
				break;
			}
			
			DiaoLuoTemplate t_template = new DiaoLuoTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.itemId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.PveIds = t_reader.Value;
			}
			
//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
}


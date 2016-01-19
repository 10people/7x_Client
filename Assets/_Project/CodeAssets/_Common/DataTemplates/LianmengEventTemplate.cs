using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianmengEventTemplate : XmlLoadManager {

	public int ID;
	
	public string Desc;

	public static List<LianmengEventTemplate> templates = new List<LianmengEventTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianmengEvent.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianmengEvent" );
			
			if( !t_has_items ){
				break;
			}
			
			LianmengEventTemplate t_template = new LianmengEventTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.ID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Desc = t_reader.Value ;

				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianmengEventTemplate GetLianmengEvent_by_Id(int id)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].ID == id)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

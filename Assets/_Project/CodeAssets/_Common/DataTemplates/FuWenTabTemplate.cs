using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenTabTemplate : XmlLoadManager{
	public int id;
	public int level;

	
	public static List<FuWenTabTemplate> templates = new List<FuWenTabTemplate>();
	
	
	public void Log(){
		Debug.Log( "FuWenTabTemplate.Log( id: " +
		          " awardId : " );
	}
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FuwenTab.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "FuwenTab" );
			
			if( !t_has_items ){
				break;
			}
			
			FuWenTabTemplate t_template = new FuWenTabTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );

			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static FuWenTabTemplate getFuWenTabTemplateBytab(int tab){
		foreach( FuWenTabTemplate template in templates ){
			if( template.id == tab ){
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoFuWenTabTemplate with id " + tab);
		
		return null;
	}
}

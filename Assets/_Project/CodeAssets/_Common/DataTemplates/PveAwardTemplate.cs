using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class PveAwardTemplate : XmlLoadManager {
	public int id;
	
	public string awardId;
	public static List<PveAwardTemplate> templates = new List<PveAwardTemplate>();
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PveBigAward.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "PveBigAward" );
			
			if( !t_has_items ){
				break;
			}
			
			PveAwardTemplate t_template = new PveAwardTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.awardId =  t_reader.Value ;
				

			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	public static PveAwardTemplate getAwardTemp_By_AwardId( int ZhangJieId )
	{
		foreach( PveAwardTemplate template in templates )
		{
			if( template.id == ZhangJieId )
			{
				return template;
			}
		}
		
		Debug.LogError( "PveAwardTemplate not found with award id: " + ZhangJieId );
		
		return null;
	}
}

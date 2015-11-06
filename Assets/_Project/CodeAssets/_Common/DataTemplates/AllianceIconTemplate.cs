using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;
public class AllianceIconTemplate : XmlLoadManager 
{
    public int icon;


	public static List<AllianceIconTemplate> templates = new List<AllianceIconTemplate>();


    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianmengIcon.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianmengIcon" );
			
			if( !t_has_items ){
				break;
			}
			
			AllianceIconTemplate t_template = new AllianceIconTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
    }
}

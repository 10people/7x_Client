using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TextIdTemplate : XmlLoadManager
{
	//<TextId textId="1" Text="白" />

	public int textId;

	public string Text;


	public static List<TextIdTemplate> templates = new List<TextIdTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "TextId.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "TextId" );
			
			if( !t_has_items ){
				break;
			}
			
			TextIdTemplate t_template = new TextIdTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.textId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Text = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static TextIdTemplate getTextIdTemplateById(int textId){
		foreach(TextIdTemplate template in templates){
			if(template.textId == textId){
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get TextIdTemplate with id " + textId);
		
		return null;
	}

}

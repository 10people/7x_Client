using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HeroTypeTemplate : XmlLoadManager
{
	//<HeroType typeId="1" typeName="11000" typeDesc="11000" atkType="1" />

	public int typeId;

	public int typeName;

	public int typeDesc;

	public int atkType;


	public static List<HeroTypeTemplate> templates = new List<HeroTypeTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "heroType.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HeroType" );
			
			if( !t_has_items ){
				break;
			}
			
			HeroTypeTemplate t_template = new HeroTypeTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.typeId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.typeName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.typeDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.atkType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				// kuId
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static HeroTypeTemplate getTemplateBytypeId(int typeId)
	{
		foreach(HeroTypeTemplate template in templates)
		{
			if(template.typeId == typeId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HeroTypeTemplate with typeId " + typeId);
		
		return null;
	}

}

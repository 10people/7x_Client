using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HeroStarTemplate : XmlLoadManager
{
	//<HeroStar id="1000" quality="1" star="0" nextStar="1" exp="5" jingmaidian="1" />

	public int id;

	public int quality;

	public int star;

	public int nextStar;

	public int exp;

	public int jingmaidian;


	public static List<HeroStarTemplate> templates = new List<HeroStarTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HeroStar.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HeroStar" );
			
			if( !t_has_items ){
				break;
			}
			
			HeroStarTemplate t_template = new HeroStarTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.quality = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.star = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nextStar = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.exp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jingmaidian = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static HeroStarTemplate getTemplateById(int id)
	{
		foreach(HeroStarTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogWarning("XML ERROR: Can't get HeroStarTemplate with id " + id);
		
		return null;
	}

}

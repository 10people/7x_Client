using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;

public class BubbleTextTemplate : XmlLoadManager
{
	//<BubbleText id="1" text="我" />

	public int id;

	public string text;


	public static List<BubbleTextTemplate> templates;


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<BubbleTextTemplate>();
		
		else templates.Clear();
		
		UnLoadManager.DownLoad( PathManager.GetUrl(m_LoadPath + "BubbleText.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "BubbleText" );
			
			if( t_has_items == false ) break;
			
			BubbleTextTemplate t_template = new BubbleTextTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.text = t_reader.Value;
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static BubbleTextTemplate getBubbleTextTemplateById(int id)
	{
		foreach(BubbleTextTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get BubbleTextTemplate with id " + id);

		return null;
	}

	public static string getBubbleTextById(int id)
	{
		foreach(BubbleTextTemplate template in templates)
		{
			if(template.id == id)
			{
				return template.text;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get BubbleTextTemplate with id " + id);
		
		return "" + id;
	}

}

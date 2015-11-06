using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PveStarTemplate : XmlLoadManager
{
	//<PveStar starId="2011" desc="502011" condition="1" award="0:900002:20" />

	public int starId;

	public int desc;

	public string condition;

	public string award;


	public static List<PveStarTemplate> templates = new List<PveStarTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PveStar.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "PveStar" );
			
			if( !t_has_items ){
				break;
			}
			
			PveStarTemplate t_template = new PveStarTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.starId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.condition = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static PveStarTemplate getPveStarTemplateByStarId(int starId)
	{
		foreach(PveStarTemplate template in templates)
		{
			if(template.starId == starId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get PveStarTemplate with starId " + starId);
		
		return null;
	}

	public static string[] GetAwardInfo(int starId)
	{
		foreach (PveStarTemplate tempItem in templates)
		{
			if (tempItem.starId == starId)
			{
				string[] awareInfo = tempItem.award.Split(':');

				return awareInfo;
			}
		}

		Debug.LogError("XML ERROR: Can't get PveStarTemplate with starId " + starId);

		return null;
	}
}

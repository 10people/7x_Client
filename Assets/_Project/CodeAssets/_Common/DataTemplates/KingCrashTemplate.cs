using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class KingCrashTemplate : XmlLoadManager
{
	//<KingCrash crashId="100" delay="0.03" time="0.2" length="1.5" actionSpeed="1.25" />
	
	public int crashId;
	
	public float delay;
	
	public float time;

	public float length;

	public float actionSpeed;
	
	
	public static List<KingCrashTemplate> templates = new List<KingCrashTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "KingCrash.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		}
		
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
			t_has_items = t_reader.ReadToFollowing( "KingCrash" );
			
			if( !t_has_items ) break;
			
			KingCrashTemplate t_template = new KingCrashTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.crashId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.delay = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.time = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.length = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.actionSpeed = float.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static KingCrashTemplate getKingCrashById(int id)
	{
		foreach(KingCrashTemplate template in templates)
		{
			if(template.crashId == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get KingCrashTemplate with id " + id);
		
		return null;
	}

	public static bool haveKingCrashById(int id)
	{
		foreach(KingCrashTemplate template in templates)
		{
			if(template.crashId == id)
			{
				return true;
			}
		}
		
		return false;
	}

}

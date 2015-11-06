using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class KingMovementTemplate : XmlLoadManager
{
	//<KingMovement movementId="101" time="0.1" length="0.314" />

	public int movementId;

	public float time;

	public float length;


	public static List<KingMovementTemplate> templates = new List<KingMovementTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "KingMovement.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "KingMovement" );
			
			if( !t_has_items ) break;
			
			KingMovementTemplate t_template = new KingMovementTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.movementId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.time = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.length = float.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static KingMovementTemplate getKingMovementById(int id)
	{
		foreach(KingMovementTemplate template in templates)
		{
			if(template.movementId == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get KingMovementTemplate with id " + id);
		
		return null;
	}
	
}

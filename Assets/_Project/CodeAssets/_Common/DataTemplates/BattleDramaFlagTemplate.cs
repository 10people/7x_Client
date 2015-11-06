using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleDramaFlagTemplate : XmlLoadManager
{

	public int flagId;
	
	public int eventId;
	
	public float x;
	
	public float y;
	
	public float z;
	
	public float cx;
	
	public float cy;
	
	public float cz;
	
	public float rx;
	
	public float ry;
	
	public float rz;

	
	public static List<BattleDramaFlagTemplate> templates;

	public static void LoadTemplates( int chapterId, EventDelegate.Callback p_callback = null ){
		if(templates == null){
			templates = new List<BattleDramaFlagTemplate>();
		}
		else{
			templates.Clear();
		}
		
		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/BattleField/BattleFlags/Drama_" + chapterId + ".xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}

	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "BattleDramaFlag" );
			
			if( !t_has_items ){
				break;
			}
			
			BattleDramaFlagTemplate t_template = new BattleDramaFlagTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.flagId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.eventId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.x = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.y = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.z = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cy = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cz = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ry = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.rz = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		if( m_load_callback != null ){
			m_load_callback();
			
			m_load_callback = null;
		}
	}

	private static Global.LoadResourceCallback m_load_callback = null;
	
	public static void SetLoadDoneCallback( Global.LoadResourceCallback p_callback ){
		m_load_callback = p_callback;
	}
}
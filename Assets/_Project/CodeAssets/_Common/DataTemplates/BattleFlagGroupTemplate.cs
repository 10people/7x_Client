using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleFlagGroupTemplate : XmlLoadManager
{

	public int groupId;
	
	public int maxActive;
	
	public float delay;


	public static List<BattleFlagGroupTemplate> templates = new List<BattleFlagGroupTemplate>();


	public static void LoadTemplates( int chapterId, EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<BattleFlagGroupTemplate>();

		else templates.Clear();
		
		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/BattleField/BattleFlags/Group_" + chapterId + ".xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do
		{
			t_has_items = t_reader.ReadToFollowing( "BattleGroup" );
			
			if( !t_has_items ) break;
			
			BattleFlagGroupTemplate t_template = new BattleFlagGroupTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.groupId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.maxActive = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.delay = float.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		if( m_load_callback != null )
		{
			m_load_callback();
			
			m_load_callback = null;
		}
	}

	private static Global.LoadResourceCallback m_load_callback = null;
	
	public static void SetLoadDoneCallback( Global.LoadResourceCallback p_callback ){
		m_load_callback = p_callback;
	}

}

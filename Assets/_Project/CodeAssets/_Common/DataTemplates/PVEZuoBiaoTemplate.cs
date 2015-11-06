using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PVEZuoBiaoTemplate : XmlLoadManager {

	// <PveZuobiao " zhangjie="1" guanka="1" x="43.5" y="52.9" />

	public int section;

	public int level;

	public float x;

	public float y;

	public float z;

	public int Dir;

	public static List<PVEZuoBiaoTemplate> templates = new List<PVEZuoBiaoTemplate>();


	
	public void Log(){
		Debug.Log( "PVEZuoBiao-  section: " + section +
		          " level: " + level + 
		          " x: " + x + 
		          " y: " + y );
	}


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PveZuobiao.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "PveZuobiao" );
			
			if( !t_has_items ){
				break;
			}
			
			PVEZuoBiaoTemplate t_template = new PVEZuoBiaoTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.x = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.y = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.z = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.Dir = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static Vector2 GetCoordinate( int p_section, int p_level ){
		for( int i = 0 ; i < templates.Count; i++ ){
			PVEZuoBiaoTemplate t_item = templates[ i ];

			if( t_item.level == p_level ){
				return new Vector2( t_item.x, t_item.y );
			}
		}

		Debug.LogError( "Level not Exist: " + p_section + ", " + p_level );

		return new Vector2( 50, 50 );
	}
	public static int GetDir_by( int p_level ){

		for( int i = 0 ; i < templates.Count; i++ ){

			PVEZuoBiaoTemplate t_item = templates[ i ];
			
			if( t_item.level == p_level ){
				return t_item.Dir;
			}
		}

		Debug.LogError( "Level not Exist: " + p_level );
		return 0;
	}
}

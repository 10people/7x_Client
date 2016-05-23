//#define DEBUG_GUIDE_INFO



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;



public class GuideInfoTemplate : XmlLoadManager {
	//Guide.Id=Window.Id
	//1050=201

	public int m_guide_id;
	
	public int m_window_id;

	private static List<GuideInfoTemplate> m_templates = new List<GuideInfoTemplate>();

	public void Log(){
		Debug.Log( "GuideInfoTemplate -  Guide.Id: " + m_guide_id +
		          " = Window.Id: " + m_window_id );
	}

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( 
			PathManager.GetUrl( XmlLoadManager.m_LoadPath + "GuideInfo.txt" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if (obj == null) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;

		#if DEBUG_GUIDE_INFO
		{
			ProcessAsset();
		}
		#endif
	}

	private static void ProcessAsset(){
		if( m_templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		StringReader t_reader = new StringReader( m_templates_text.text );

		do{
			string t_line = t_reader.ReadLine();

			if( t_line == null ){
				break;
			}

			string[] t_contents = t_line.Split( '=' );

			if( t_contents.Length <= 1 ){
				continue;
			}

			{
				if( string.IsNullOrEmpty( t_contents[ 0 ] ) ){
//					Debug.Log( "Skip, Guide Id is null or empty." );

					continue;
				}

				if( string.IsNullOrEmpty( t_contents[ 1 ] ) ){
//					Debug.Log( "Skip, Window Id is null or empty." );

					continue;
				}
			}

			{
				GuideInfoTemplate t_template = new GuideInfoTemplate();

				{
					t_template.m_guide_id = int.Parse( t_contents[ 0 ] );

					t_template.m_window_id = int.Parse( t_contents[ 1 ] );

					#if DEBUG_GUIDE_INFO
					t_template.Log();
					#endif
				}

				m_templates.Add( t_template );
			}
		}
		while( true );

		{
			m_templates_text = null;
		}
	}

	public static int GetWindowId_By_GuideId( int p_guide_id ){
		{
			ProcessAsset();
		}

		for( int i = 0; i < m_templates.Count; i++ ){
			GuideInfoTemplate t_item = m_templates[ i ];

			if( t_item.m_guide_id == p_guide_id ){
				return t_item.m_window_id;
			}
		}

//		Debug.Log( "Guide " + p_guide_id + " has no window setted." );

		return UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID;
	}
}

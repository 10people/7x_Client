using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleCameraFlagTemplate : XmlLoadManager
{
	//<BattleCameraFlag flagId="3" radius="40.6" x="17.812" y="3.119692" z="61.51037" px="0" py="10" pz="-18" rx="35.5" ry="0" rz="0" />

	public int flagId;

	public float radius;

	public float x;

	public float y;

	public float z;

	public float px;
	
	public float py;
	
	public float pz;

	public float rx;
	
	public float ry;
	
	public float rz;

	public float ex;

	public float ey;

	public float ez;

	public float ew;

	public int killMin;
	
	public int killMax = 1000;


	public static List<BattleCameraFlagTemplate> templates;


	public static void LoadTemplates( int chapterId, EventDelegate.Callback p_callback = null ){
		if(templates == null){
			templates = new List<BattleCameraFlagTemplate>();
		}
		else{
			templates.Clear();
		}
		
		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/BattleField/BattleFlags/Camera_" + chapterId + ".xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "BattleCameraFlag" );
			
			if( !t_has_items ){
				break;
			}
			
			BattleCameraFlagTemplate t_template = new BattleCameraFlagTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.flagId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.radius = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.x = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.y = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.z = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.px = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.py = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.pz = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.rx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ry = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.rz = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.ex = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.ey = float.Parse( t_reader.Value );
							
				t_reader.MoveToNextAttribute();
				t_template.ez = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.ew = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.killMin = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.killMax = int.Parse( t_reader.Value );

				templates.Add( t_template );
			}
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

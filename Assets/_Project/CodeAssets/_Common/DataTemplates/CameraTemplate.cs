using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class CameraTemplate : XmlLoadManager
{
	public int id;

	public float cameraPx;
	
	public float cameraPy;
	
	public float cameraPz;
	
	public float cameraRx;
	
	public float cameraRy;


	public static List<CameraTemplate> templates = new List<CameraTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "CameraTemplate.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "CameraTemplate" );
			
			if( !t_has_items ){
				break;
			}
			
			CameraTemplate t_template = new CameraTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraPx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraPy = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraPz = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraRx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraRy = float.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static CameraTemplate getCameraTemplateById(int cameraId)
	{
		foreach(CameraTemplate template in templates)
		{
			if(template.id == cameraId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get CameraTemplate with id " + cameraId);
		
		return null;
	}
}

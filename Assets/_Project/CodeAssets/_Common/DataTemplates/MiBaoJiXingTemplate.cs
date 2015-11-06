using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoJiXingTemplate : XmlLoadManager {

	
	public int sum;
	
	public string award;

	private static List<MiBaoJiXingTemplate> templates = new List<MiBaoJiXingTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoJixing.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}
	
	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "MibaoJixing" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoJiXingTemplate t_template = new MiBaoJiXingTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.sum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		{
			m_templates_text = null;
		}
	}
	public static MiBaoJiXingTemplate getMiBaoJiXingTemplateBysumId(int sumid){
		{
			ProcessAsset();
		}
		
		foreach(MiBaoJiXingTemplate template in templates)
		{
			if(template.sum == sumid)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoJiXingTemplate with nameId " + sumid);
		
		return null;
	}
}

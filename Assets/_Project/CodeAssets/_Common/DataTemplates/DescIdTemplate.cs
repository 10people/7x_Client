using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DescIdTemplate : XmlLoadManager
{
	//<DescId descId="1001" description="攻击1级" />
	
	public int descId;
	
	public string description;
	
	
	private static List<DescIdTemplate> templates = new List<DescIdTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( PathManager.GetUrl( XmlLoadManager.m_LoadPath + "DescId.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
			Debug.LogError( "Asset Not Exist: " + path );
			
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
			t_has_items = t_reader.ReadToFollowing( "DescId" );
			
			if( !t_has_items ){
				break;
			}

			DescIdTemplate t_template = new DescIdTemplate();

			{
				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}
	
	public static DescIdTemplate getDescIdTemplateByNameId(int descId){
		{
			ProcessAsset();
		}

		foreach(DescIdTemplate template in templates)
		{
			if(template.descId == descId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get DescIdTemplate with descId " + descId);
		
		return null;
	}
	
	
	public static string GetDescriptionById(int tempId){
		{
			ProcessAsset();
		}

		foreach (DescIdTemplate template in templates)
		{
			if (template.descId == tempId)
			{
				return template.description;
			}
		}
		return null;
	}

	public static List<DescIdTemplate> GetTemplates(){
		{
			ProcessAsset();
		}

		return templates;
	}
}

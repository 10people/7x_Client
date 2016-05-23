using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LieFuTemplate : XmlLoadManager {

	public int id;

	public int cost;

	public string awardID;

	public int promoRate;

	public string Name;

	public int IconId;

	private static List<LieFuTemplate> templates = new List<LieFuTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LieFuTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LieFuTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			LieFuTemplate t_template = new LieFuTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cost = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.awardID = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.promoRate = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.Name = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.IconId = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		{
			m_templates_text = null;
		}
	}
	public static LieFuTemplate getLieFuTemplateBy_Id(int id){
		{
			ProcessAsset();
		}
		
		foreach(LieFuTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get LieFuTemplate with id " + id);
		
		return null;
	}
}

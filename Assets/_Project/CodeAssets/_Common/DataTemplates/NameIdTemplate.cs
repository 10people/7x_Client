using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class NameIdTemplate : XmlLoadManager
{
	//<NameId nameId="1001" Name="攻击1级" />

	public int nameId;

	public string Name;

	private static Dictionary<int, NameIdTemplate> m_dict = new Dictionary<int, NameIdTemplate>();
//	private static List<NameIdTemplate> templates = new List<NameIdTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "NameId.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}

	public static void ProcessAsset(){
//		if( templates.Count > 0 ) {
		if( m_dict.Count > 0 ){
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "NameId" );
			
			if( !t_has_items ){
				break;
			}
			
			NameIdTemplate t_template = new NameIdTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.Name = t_reader.Value;
			}
			
			//			t_template.Log();
			
//			templates.Add( t_template );

			if( !m_dict.ContainsKey( t_template.nameId ) ){
				m_dict.Add( t_template.nameId, t_template );
			}
			else{
				#if UNITY_EDITOR
				Debug.LogError( "Error, Duplicate Id In NameId: " + t_template.nameId );
				#endif
			}
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static NameIdTemplate getNameIdTemplateByNameId(int nameId){
		{
			ProcessAsset();
		}

		if( m_dict[ nameId ] != null ){
			return m_dict[ nameId ];
		}

//		for( int i = 0; i < templates.Count; i++ ){
//			NameIdTemplate t_template = templates[ i ];
//
//			if(t_template.nameId == nameId){
//				return t_template;
//			}
//		}
//		
		Debug.LogError("XML ERROR: Can't get NameIdTemplate with nameId " + nameId);
		
		return null;
	}

	public static bool haveNameIdTemplateByNameId(int nameId){
		{
			ProcessAsset();
		}

		if( m_dict[ nameId ] != null ){
			return true;
		}

//		for( int i = 0; i < templates.Count; i++ ){
//			NameIdTemplate t_template = templates[ i ];
//
//			if(t_template.nameId == nameId){
//				return true;
//			}
//		}

		return false;
	}

	public static string GetName_By_NameId( int p_name_id ){
		{
			ProcessAsset();
		}

		if(m_dict.ContainsKey(p_name_id) && m_dict[p_name_id] != null)
        {
			return m_dict[ p_name_id ].Name;
		}

//		for( int i = 0; i < templates.Count; i++ ){
//			NameIdTemplate t_template = templates[ i ];
//
//			if( t_template.nameId == p_name_id ){
//				return t_template.Name;
//			}
//		}
		
		Debug.LogError("name not found: " + p_name_id );
		
		return "null";
	}

//	public static List<NameIdTemplate> GetTemplates(){
//		{
//			ProcessAsset();
//		}
//
//		return templates;
//	} 
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class MiBaoExtrattributeTemplate : XmlLoadManager {

	
	public int Id;
	
	public int  needlv;

	public string shuxing;

	public string Name;

	public int  Num;

	private static List<MiBaoExtrattributeTemplate> templates = new List<MiBaoExtrattributeTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MiBaoExtraAttribute.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "MiBaoExtraAttribute" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoExtrattributeTemplate t_template = new MiBaoExtrattributeTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.Id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needlv = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.shuxing = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.Name = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.Num = int.Parse( t_reader.Value );

			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		{
			m_templates_text = null;
		}
	}
	public static MiBaoExtrattributeTemplate GetMiBaoExtrattributeTemplate_By_Id_and_level( int id,int leve){
		{
			ProcessAsset();
		}
		
		foreach(MiBaoExtrattributeTemplate template in templates)
		{
			if( template.Id == id &&template.needlv == leve)
			{
				return template;
			}
		}
		
		Debug.LogError("id not found: " + id );
		
		return null;
	}
}

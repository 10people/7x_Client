using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class CreateRoleTemplate : XmlLoadManager {

	public int m_id;
	
	public int m_sex;
	
	public string m_model;
	
	public string m_icon;
	
	public int m_descId;
	
	public static List<CreateRoleTemplate> createRoleTemplates = new List<CreateRoleTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ZhuceRenwu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			createRoleTemplates.Clear();
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
			t_has_items = t_reader.ReadToFollowing( "ZhuceRenwu" );
			
			if( !t_has_items ){
				break;
			}
			
			CreateRoleTemplate t_template = new CreateRoleTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.m_id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_sex = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.m_model = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.m_icon = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.m_descId = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			createRoleTemplates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static CreateRoleTemplate GetCreateRoleId (int id) {

		foreach (CreateRoleTemplate createRenWu in createRoleTemplates) {

			if (createRenWu.m_id == id) {

				return createRenWu;
			}
		}

		Debug.LogError("XML ERROR: Can't get CreateRoleTemplate with id " + id);
		
		return null;
	}
}

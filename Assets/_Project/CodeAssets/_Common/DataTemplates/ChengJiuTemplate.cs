using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ChengJiuTemplate : XmlLoadManager {
	//<Chengjiu id="100101" name="700101" funDesc="700101" type="1" preId="0" nextId="100102" condition="100" yuanbao="30" />

	public int id;

	public int m_name;

	public int funDesc;

	public int type;

	public int PreId;

	public int nextId;

	public int condition;

	public int yuanbao;

	public static List<ChengJiuTemplate> templates = new List<ChengJiuTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Chengjiu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "Chengjiu" );
			
			if( !t_has_items ){
				break;
			}
			
			ChengJiuTemplate t_template = new ChengJiuTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.funDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

//				preId="0" nextId="100102" condition="100" yuanbao="30" />
				t_reader.MoveToNextAttribute();
				t_template.PreId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nextId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.condition = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.yuanbao = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static ChengJiuTemplate GetTemplateById(int tempId)
	{
		foreach(ChengJiuTemplate template in templates)
		{
			if(template.id == tempId)
			{
				return template;
			}
		}
		return null;
	}
}

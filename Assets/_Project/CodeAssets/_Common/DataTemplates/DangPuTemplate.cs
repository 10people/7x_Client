using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DangPuTemplate : XmlLoadManager
{
	//<Dangpu id="1001" itemType="5" itemId="311001" itemNum="2" type="1" needNum="1000" 
	//weight="2500" site="1" flag="1" />

	public int id;

	public int itemType;

	public int itemId;

	public int itemNum;

	public int needType;

	public int needNum;

	public int weight;

	public int site;

	public int flag;

	public int VIP;
	
	public static List<DangPuTemplate> templates = new List<DangPuTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Dangpu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "Dangpu" );
			
			if( !t_has_items ){
				break;
			}
			
			DangPuTemplate t_template = new DangPuTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemNum = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.weight = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.site = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.flag = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.VIP = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static DangPuTemplate getDangpuTemplateById(int id)
	{
		foreach(DangPuTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get DangPuTemplate with id " + id);

		return null;
	}

}

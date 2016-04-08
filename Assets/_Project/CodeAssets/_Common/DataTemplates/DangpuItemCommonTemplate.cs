using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DangpuItemCommonTemplate : XmlLoadManager
{
	//<DangpuCommon id="1001" itemType="0" itemId="910000" itemNum="1" type="1" needNum="50" 
	//site="2" flag="1" />

	public int id;

	public int itemType;

	public int itemId;

	public int itemNum;

	public int needType;

	public int needNum;

	public int site;

	public int flag;

	public int max;

	public int VIP;

	public int ifRecomanded;

	public static List<DangpuItemCommonTemplate> templates;

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		if(templates == null){
			templates = new List<DangpuItemCommonTemplate>();
		}
		else{
			templates.Clear();
		}
		
		UnLoadManager.DownLoad( PathManager.GetUrl( XmlLoadManager.m_LoadPath + "DangpuCommon.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "DangpuCommon" );
			
			if( !t_has_items ){
				break;
			}
			
			DangpuItemCommonTemplate t_template = new DangpuItemCommonTemplate();
			
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
				t_template.site = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.flag = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.max = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.VIP = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.ifRecomanded = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static DangpuItemCommonTemplate getDangpuItemCommonById(int id)
	{
		foreach(DangpuItemCommonTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get DangpuItemCommonTemplate with id " + id);
		
		return null;
	}

	public static List<DangpuItemCommonTemplate> dangpuItemTemplateList ()
	{
		return templates;
	}
}

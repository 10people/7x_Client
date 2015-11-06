using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class CommonItemTemplate : XmlLoadManager
{
	//<CommonItem id="900001" name="900001" funDesc="900001" icon="900001" itemType="0" 
	//color="0" dropModel="0" />

	public int id;

	public int nameId;

	public int descId;

	public int icon;

	public int itemType;

	public int color;

	public int dropModel;

	public int dropDesc;

	public int synItemID;


	public static List<CommonItemTemplate> templates = new List<CommonItemTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "CommonItem.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "CommonItem" );
			
			if( !t_has_items ){
				break;
			}
			
			CommonItemTemplate t_template = new CommonItemTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.color = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dropModel = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dropDesc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.synItemID = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static CommonItemTemplate getCommonItemTemplateById(int id)
	{
		foreach(CommonItemTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get CommonItemTemplate with id " + id);
		
		return null;
	}

	public static bool haveCommonItemTemplateById(int id)
	{
		foreach(CommonItemTemplate template in templates)
		{
			if(template.id == id)
			{
				return true;
			}
		}
		
		return false;
	}
	public static CommonItemTemplate getCommonItemTemplateBy_icon_id(int m_icon)
	{
		foreach(CommonItemTemplate template in templates)
		{
			if(template.icon == m_icon)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get CommonItemTemplate with m_icon " + m_icon);
		return null;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangYeAwardTemplete : XmlLoadManager {

	public int site;

	public int itemType;

	public int itemId;

	public string getLevelIdStr;

	public static List<HuangYeAwardTemplete> templates = new List<HuangYeAwardTemplete>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyeAward.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		}

		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
//						Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "HuangyeAward" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangYeAwardTemplete t_template = new HuangYeAwardTemplete();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.site = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.getLevelIdStr = t_reader.Value;
			}
			
//						t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static HuangYeAwardTemplete getHuangYeAwardTemplateBySiteId(int siteId)
	{
		foreach(HuangYeAwardTemplete template in templates)
		{
			if(template.site == siteId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangYeAwardTemplete with siteId " + siteId);
		
		return null;
	}
}

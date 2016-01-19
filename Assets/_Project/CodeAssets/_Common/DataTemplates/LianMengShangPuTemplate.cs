using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianMengShangPuTemplate : XmlLoadManager {

	// <LianMengShangPu id="104001" shangPuLevel="1" alliance_lv_needed="1" shangPu_lvUp_value="250" item_quantity_max="1" />
	public int id;
	
	public int  shangPuLevel;
	
	public int alliance_lv_needed;
	
	public int  shangPu_lvUp_value;
	
	public int item_quantity_max;
	
	public static List<LianMengShangPuTemplate> templates = new List<LianMengShangPuTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengShangPu.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMengShangPu" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengShangPuTemplate t_template = new LianMengShangPuTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				
				t_reader.MoveToNextAttribute();
				t_template.shangPuLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.alliance_lv_needed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shangPu_lvUp_value = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.item_quantity_max = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengShangPuTemplate GetLianMengShangPuTemplate_by_lv(int lev)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].shangPuLevel == lev)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

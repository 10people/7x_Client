using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianMengShuYuanTemplate : XmlLoadManager {

	// <LianMengShuYuan id="102001" shuYuanLevel="1" alliance_lv_needed="1" shuYuan_lvUp_value="250" keJilLevelMax="1" />
	public int id;
	
	public int  shuYuanLevel;
	
	public int alliance_lv_needed;
	
	public int  shuYuan_lvUp_value;
	
	public int keJilLevelMax;
	
	public static List<LianMengShuYuanTemplate> templates = new List<LianMengShuYuanTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengShuYuan.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMengShuYuan" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengShuYuanTemplate t_template = new LianMengShuYuanTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				
				t_reader.MoveToNextAttribute();
				t_template.shuYuanLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.alliance_lv_needed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shuYuan_lvUp_value = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.keJilLevelMax = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengShuYuanTemplate GetLianMengShuYuanTemplate_by_shuYuanLevel(int lev)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].shuYuanLevel == lev)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

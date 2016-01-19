using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianMengZongMiaoTemplate : XmlLoadManager {

	// <LianMengZongMiao id="105001" zongMiaoLevel="1" alliance_lv_needed="1" zongMiao_lvUp_value="50" jiBaiMaxTimes="1" />
	public int id;
	
	public int  zongMiaoLevel;
	
	public int alliance_lv_needed;
	
	public int  zongMiao_lvUp_value;
	
	public int jiBaiMaxTimes;
	
	public static List<LianMengZongMiaoTemplate> templates = new List<LianMengZongMiaoTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengZongMiao.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMengZongMiao" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengZongMiaoTemplate t_template = new LianMengZongMiaoTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				
				t_reader.MoveToNextAttribute();
				t_template.zongMiaoLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.alliance_lv_needed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.zongMiao_lvUp_value = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.jiBaiMaxTimes = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengZongMiaoTemplate GetLianMengZongMiaoTemplate_by_lev(int lev)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].zongMiaoLevel == lev)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LianMengKeZhanTemplate : XmlLoadManager {
	//<LianMengKeZhan id="101001" keZhanLevel="1" alliance_lv_needed="1" keZhan_lvUp_value="250" renshuMax="15" />
	public int id;

	public int  keZhanLevel;
	
	public int alliance_lv_needed;
	
	public int  keZhan_lvUp_value;
	
	public int renshuMax;

	public static List<LianMengKeZhanTemplate> templates = new List<LianMengKeZhanTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengKeZhan.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMengKeZhan" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengKeZhanTemplate t_template = new LianMengKeZhanTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				
				t_reader.MoveToNextAttribute();
				t_template.keZhanLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.alliance_lv_needed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.keZhan_lvUp_value = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.renshuMax = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengKeZhanTemplate GetLianMengKeZhanTemplate_by_lev(int lev)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].keZhanLevel == lev)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

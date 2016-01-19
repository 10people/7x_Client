using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LianMengTemplate : XmlLoadManager {

	//<LianMeng id="1001" lv="1" exp="5000" renshuMax="15" fumeng="5" statueType="0" mobaiAward="100" buffNum="30" />
	public int id;
	
	public int  lv;
	
	public int exp;
	
	public int  renshuMax;
	
	public int fumeng;

	public int statueType;

	public int mobaiAward;

	public int buffNum;
	
	public static List<LianMengTemplate> templates = new List<LianMengTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMeng.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LianMeng" );
			
			if( !t_has_items ){
				break;
			}
			
			LianMengTemplate t_template = new LianMengTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.exp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.renshuMax = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fumeng = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.statueType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mobaiAward = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.buffNum = int.Parse( t_reader.Value );
				
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static LianMengTemplate GetLianMengTemplate_by_id(int id)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].id == id)
			{
				return templates[i];
			}
		}
		return null;
		
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LueDuoLianmengRankTemplate : XmlLoadManager {

	public int min;
	
	public int max;
	
	public int award;
	
	public static List<LueDuoLianmengRankTemplate> m_templates = new List<LueDuoLianmengRankTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LueduoLianmengRank.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			m_templates.Clear();
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
			t_has_items = t_reader.ReadToFollowing( "LueduoLianmengRank" );
			
			if( !t_has_items ){
				break;
			}
			
			LueDuoLianmengRankTemplate t_template = new LueDuoLianmengRankTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.min = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.max = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.award = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static LueDuoLianmengRankTemplate GetLueDuoLianmengRankTemplateByRank (int rank)
	{
		foreach (LueDuoLianmengRankTemplate template in m_templates)
		{
			if (rank >= template.min && rank <= template.max)
			{
				return template;
			}
		}
		
		Debug.LogError ("Can`t get LueDuoLianmengRankTemplate by Rank:" + rank);
		
		return null;
	}
}

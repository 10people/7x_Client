using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LueDuoUnionRankTemplate : XmlLoadManager {

	public int countryRank;
	
	public int unionRankMin;

	public int unionRankMax;

	public string award;
	
	public string bigAward;
	
	public static List<LueDuoUnionRankTemplate> m_templates = new List<LueDuoUnionRankTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LueduoUnionRank.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LueduoUnionRank" );
			
			if( !t_has_items ){
				break;
			}
			
			LueDuoUnionRankTemplate t_template = new LueDuoUnionRankTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.countryRank = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.unionRankMin = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.unionRankMax = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.bigAward = t_reader.Value;
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static LueDuoUnionRankTemplate GetLueDuoUnionRankTemplateByRankAndCountryRank (int rank,int countryRank)
	{
		foreach (LueDuoUnionRankTemplate template in m_templates)
		{
			if (rank >= template.unionRankMin && rank <= template.unionRankMax && countryRank == template.countryRank)
			{
				return template;
			}
		}
		
		Debug.LogError ("Can`t get LueDuoUnionRankTemplate by Rank:“" + rank + "” and CountryRank:“" + countryRank + "”");
		
		return null;
	}
}

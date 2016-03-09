using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LueDuoPersonRankTemplate : XmlLoadManager {

	public int min;

	public int max;

	public string award;

	public int updateNum;

	public static List<LueDuoPersonRankTemplate> m_templates = new List<LueDuoPersonRankTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LueduoPersonRank.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "LueduoPersonRank" );
			
			if( !t_has_items ){
				break;
			}
			
			LueDuoPersonRankTemplate t_template = new LueDuoPersonRankTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.min = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.max = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.updateNum = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static LueDuoPersonRankTemplate GetLueDuoPersonRankTemplateByRank (int rank)
	{
		foreach (LueDuoPersonRankTemplate template in m_templates)
		{
			if (rank >= template.min && rank <= template.max)
			{
				return template;
			}
		}

		Debug.LogError ("Can`t get LueDuoPersonRankTemplate by rank:" + rank);

		return null;
	}
}

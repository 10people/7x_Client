using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HYRankAwardTemplate : XmlLoadManager {

	public int rank;
	
	public float award;
	
	//private static Dictionary<int, HYRankAwardTemplate> m_dict = new Dictionary<int, HYRankAwardTemplate>();
	private static List<HYRankAwardTemplate> templates = new List<HYRankAwardTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyeRankAward.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HuangyeRankAward" );
			
			if( !t_has_items ){
				break;
			}
			
			HYRankAwardTemplate t_template = new HYRankAwardTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.rank = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.award = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	public static HYRankAwardTemplate getHYRankAwardTemplateTemplateBy_Rank(int rank)
	{
		foreach(HYRankAwardTemplate template in templates)
		{
			if(template.rank == rank)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HYRankAwardTemplate with rank " + rank);
		
		return null;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BaiZhanRankTemplate : XmlLoadManager {

	public int rank;
	
	public int yuanbao;
	
	public int weiwang;
	
	public int money;

	public int weiwangLimit;
	
	public static List<BaiZhanRankTemplate> templates = new List<BaiZhanRankTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaiZhanRank.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
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
			t_has_items = t_reader.ReadToFollowing( "BaiZhanRank" );
			
			if( !t_has_items ){
				break;
			}
			
			BaiZhanRankTemplate t_template = new BaiZhanRankTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.rank = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.yuanbao = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.weiwang = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.money = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.weiwangLimit = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static BaiZhanRankTemplate getBaiZhanRankTemplateByRank (int rank)
	{
		foreach(BaiZhanRankTemplate template in templates)
		{
			if(template.rank == rank)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get BaiZhanRankTemplate with rank " + rank);
		
		return null;
	}
}

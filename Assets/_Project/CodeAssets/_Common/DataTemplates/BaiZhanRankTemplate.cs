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
	
	private static List<BaiZhanRankTemplate> m_templates = new List<BaiZhanRankTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaiZhanRank.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
	}

	private static TextAsset m_m_templates_text = null;

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
			Debug.LogError ("Asset Not Exist: " + path);

			return;
		}

		m_m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if( m_templates.Count > 0 ) {
			return;
		}

		if( m_m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );

			return;
		}

		XmlReader t_reader = XmlReader.Create( new StringReader( m_m_templates_text.text ) );

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

			m_templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_m_templates_text = null;
		}
	}

	public static int GetTemplatesCount(){
		{
			ProcessAsset();
		}

		return m_templates.Count;
	}
	
	public static BaiZhanRankTemplate getBaiZhanRankTemplateByRank (int rank){
		{
			ProcessAsset();
		}

		foreach(BaiZhanRankTemplate template in m_templates)
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

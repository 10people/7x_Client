using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BaiZhanTemplate : XmlLoadManager
{
	//<BaiZhan id="1" name="70001" funDesc="70001" icon="1" jibie="1" xishu="5" dayAward="0:920001:5#0:921001:5#0:900005:0#10:7001:1" 
	//seasonAward="801" win="40" lose="0" need="100" nextId="0" />
	//<BaiZhan id="1" name="70001" funDesc="70001" icon="1" minRank="2001" maxRank="100000" jibie="1" xishu="20" dayAward="10:7001:1" />

	public int id;

	public int templateName;

	public int funDesc;

	public int icon;

	public int minRank;

	public int maxRank;

	public int jibie;

	public int xishu;

	public string dayAward;

	public static List<BaiZhanTemplate> templates = new List<BaiZhanTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BaiZhan.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );

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
			t_has_items = t_reader.ReadToFollowing( "BaiZhan" );
			
			if( !t_has_items ){
				break;
			}
			
			BaiZhanTemplate t_template = new BaiZhanTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.templateName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.funDesc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.minRank = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.maxRank = int.Parse( t_reader.Value );


				t_reader.MoveToNextAttribute();
				t_template.jibie = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.xishu = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.dayAward = t_reader.Value;
				

			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static BaiZhanTemplate getBaiZhanTemplateById(int id)
	{
		foreach(BaiZhanTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get BaiZhanTemplate with id " + id);
		
		return null;
	}

}

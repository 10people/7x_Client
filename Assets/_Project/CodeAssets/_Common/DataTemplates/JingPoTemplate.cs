using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class JingPoTemplate : XmlLoadManager
{
	//<JingPo id="930101" name="930101" funDesc="930101" icon="35" quality="1" maxNum="9999" hechengNum="5" 
	//fenjieNum="5" exp="10" wujiangExp="1" heroId="700101" />

	public int id;

	public int itemName;

	public int funDesc;

	public int icon;

	public int quality;

	public int maxNum;

	public int hechengNum;

	public int fenjieNum;

	public int exp;

	public int wujiangExp;

	public int heroId;


	public static List<JingPoTemplate> templates = new List<JingPoTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "JingPo.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "JingPo" );
			
			if( !t_has_items ){
				break;
			}
			
			JingPoTemplate t_template = new JingPoTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.funDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.quality = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.maxNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.hechengNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fenjieNum = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.exp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wujiangExp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.heroId = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static JingPoTemplate getJingPoTemplateById(int id)
	{
		foreach(JingPoTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get JingPoTemplate with id " + id);
		
		return null;
	}

}

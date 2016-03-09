using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;

public class BattleConfigTemplate : XmlLoadManager
{
	//<BattleConfigTemplate configId="1" autoFight="1" />

	public int configId;

	public int autoFight;

	public int soundID;

	public int preDesc;


	public static List<BattleConfigTemplate> templates;


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		if(templates == null)
			templates = new List<BattleConfigTemplate>();
		else{
			templates.Clear();
		}
		
		UnLoadManager.DownLoad( PathManager.GetUrl( m_LoadPath + "BattleConfigTemplate.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}

	public static void CurLoad( ref WWW www, string path, Object obj )
	{
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "BattleConfigTemplate" );
			
			if( !t_has_items ){
				break;
			}
			
			BattleConfigTemplate t_template = new BattleConfigTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.autoFight = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.soundID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.preDesc = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static BattleConfigTemplate getBattleConfigTemplateByConfigId(int configId)
	{
		foreach(BattleConfigTemplate template in templates)
		{
			if(template.configId == configId)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get BattleConfigTemplate with configId " + configId);

		return null;
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;

public class BattleAppearanceTemplate : XmlLoadManager
{
	public int appearanceId;

	public float height;

	public string bossFxColor;

	public float bossFxWidth;

	public int navPriority;

	public float colliderRadius;


	public static List<BattleAppearanceTemplate> templates = new List<BattleAppearanceTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "BattleAppearance.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
		templates.Clear();
		
		XmlReader t_reader = null;
		
		if( obj != null )
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
		}
		else
		{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "BattleAppearance" );
			
			if( !t_has_items ){
				break;
			}
			
			BattleAppearanceTemplate t_template = new BattleAppearanceTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.appearanceId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.height = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.bossFxColor = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.bossFxWidth = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.navPriority = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.colliderRadius = float.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static BattleAppearanceTemplate getBattleAppearanceTemplateById(int appearanceId)
	{
		foreach(BattleAppearanceTemplate template in templates)
		{
			if(template.appearanceId == appearanceId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get BattleAppearanceTemplate with appearanceId " + appearanceId);
		
		return templates[0];
	}

}

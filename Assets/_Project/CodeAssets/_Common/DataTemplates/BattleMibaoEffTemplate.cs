using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleMibaoEffTemplate : XmlLoadManager
{
	public int nameId;

	public float frameStartTime;

	public float frameActionTime;

	public float backStartTime;

	public float backActionTime;

	public float label1StartTime;

	public float label2StartTime;

	public float shakeTime;

	public float endTime;


	public static List<BattleMibaoEffTemplate> templates;


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		if(templates == null) templates = new List<BattleMibaoEffTemplate>();

		else templates.Clear();
		
		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/Design/BattleMibaoEff.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
	{
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
			t_has_items = t_reader.ReadToFollowing( "BattleMibaoEff" );
			
			if( !t_has_items ) break;
			
			BattleMibaoEffTemplate t_template = new BattleMibaoEffTemplate();
			
			t_reader.MoveToNextAttribute();
			t_template.nameId = int.Parse( t_reader.Value );
			
			t_reader.MoveToNextAttribute();
			t_template.frameStartTime = float.Parse( t_reader.Value );

			t_reader.MoveToNextAttribute();
			t_template.frameActionTime = float.Parse( t_reader.Value );
			
			t_reader.MoveToNextAttribute();
			t_template.backStartTime = float.Parse( t_reader.Value );

			t_reader.MoveToNextAttribute();
			t_template.backActionTime = float.Parse( t_reader.Value );

			t_reader.MoveToNextAttribute();
			t_template.label1StartTime = float.Parse( t_reader.Value );
			
			t_reader.MoveToNextAttribute();
			t_template.label2StartTime = float.Parse( t_reader.Value );

			t_reader.MoveToNextAttribute();
			t_template.shakeTime = float.Parse( t_reader.Value );

			t_reader.MoveToNextAttribute();
			t_template.endTime = float.Parse( t_reader.Value );
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static BattleMibaoEffTemplate getMibaoEffectTemplateByNameId(int nameId)
	{
		foreach(BattleMibaoEffTemplate template in templates)
		{
			if(template.nameId == nameId)
			{
				return template;
			}
		}

		Debug.LogError ("Can't get BattleMibaoEffTemplate with nameId " + nameId);

		return null;
	}

}

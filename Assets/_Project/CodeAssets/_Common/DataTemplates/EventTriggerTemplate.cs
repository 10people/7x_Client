using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EventTriggerTemplate : XmlLoadManager
{
	//<EventTriggerTemp ID="1010001" type="1" EventID="1" DescId="洛阳" DicType="1" />

	public int id;

	public int triggerType;

	public int eventId;

	public int descId;

	public int dictType;


	public static List<EventTriggerTemplate> templates = new List<EventTriggerTemplate>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "EventTriggerTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj )
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
		
		do
		{
			t_has_items = t_reader.ReadToFollowing( "EventTriggerTemp" );
			
			if( !t_has_items ) break;
			
			EventTriggerTemplate t_template = new EventTriggerTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.triggerType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.eventId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dictType = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static EventTriggerTemplate getEventTriggerTemplateById(int id)
	{
		foreach(EventTriggerTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}

		Debug.LogError ("THERE IS NO EventTriggerTemplate With Id " + id);

		return null;
	}

}

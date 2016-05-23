using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class YouXiaOpenTimeTemplate :  XmlLoadManager  {


	public int id;

	public string OpenDay;

	public string OpenTime;

	public int maxTimes;

	public int CD;

	public int NameId;

	public int openLevel;

	public int functionID;
	public static List<YouXiaOpenTimeTemplate> templates = new List<YouXiaOpenTimeTemplate>();
	
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "YouxiaOpenTime.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
		templates.Clear();
		
		XmlReader t_reader = null;
		
		if (obj != null)
		{
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else
		{
			t_reader = XmlReader.Create(new StringReader(www.text));
		}
		
		bool t_has_items = true;
		
		do
		{
			t_has_items = t_reader.ReadToFollowing("YouxiaOpenTime");
			
			if (!t_has_items)
			{
				break;
			}
			
			YouXiaOpenTimeTemplate t_template = new YouXiaOpenTimeTemplate();
			//	<YouxiaOpenTime id="1" OpenDay="1,4,7" OpenTime="4:00" maxTimes="3" CD="600" NameId="300100" openLevel="27" />
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.OpenDay = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.OpenTime = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.maxTimes = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.CD = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.NameId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.openLevel = int.Parse(t_reader.Value);
	
				t_reader.MoveToNextAttribute();
				t_template.functionID = int.Parse(t_reader.Value);

				templates.Add(t_template);
			}
		}
		while (t_has_items);
	}
	public static YouXiaOpenTimeTemplate getYouXiaOpenTimeTemplateby_Id( int id )
	{
		foreach( YouXiaOpenTimeTemplate template in templates ){
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get YouXiaOpenTimeTemplate with bigid " + id);
		
		return null;
	}
}

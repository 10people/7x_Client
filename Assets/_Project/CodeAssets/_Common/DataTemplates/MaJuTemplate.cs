using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using qxmobile;
using qxmobile.protobuf;

public class MaJuTemplate : XmlLoadManager
{
	public int id;

	public int nameId;

	public int descId;

	public int iconId;

	public int colorId;

	public int functionTypeId;

	public int value1;

	public int value2;

	public int value3;

	public int value4;

	public int priceId;

	public static List<MaJuTemplate> templates = new List<MaJuTemplate>();

	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MaJu.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
		{
			templates.Clear();
		}
		
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
			t_has_items = t_reader.ReadToFollowing("MaJu");
			
			if (!t_has_items)
			{
				break;
			}
			
			MaJuTemplate t_template = new MaJuTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.iconId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.colorId = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.functionTypeId = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.value1 = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.value2 = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.value3 = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.value4 = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.priceId = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static List<MaJuTemplate> GetMaJuTemplateList ()
	{
		return templates;
	}

	public static MaJuTemplate GetMaJuTemplateById (int tempId)
	{
		foreach (MaJuTemplate template in templates)
		{
			if (template.id == tempId)
			{
				return template;
			}
		}

		return null;
	}
}

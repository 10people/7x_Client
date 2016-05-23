using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenOpenTemplate : XmlLoadManager {

	//id-index，level-君主开启等级，lanweiID-符文栏位id，lanweiType-符文所在栏位类型

	public int id;

	public int Tab;

	public int level;
	
	public int inlayColor;

	public string background;


	public static List<FuWenOpenTemplate> templates = new List<FuWenOpenTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FuwenOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing("FuwenOpen");
			
			if (!t_has_items)
			{
				break;
			}
			
			FuWenOpenTemplate t_template = new FuWenOpenTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.Tab = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.inlayColor = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.background = t_reader.Value;

			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static List<int > GetFuWenOpenTemplateByLanWeiIdList (int tab)
	{
		List<int > Ids = new List<int> ();
		foreach (FuWenOpenTemplate template in templates)
		{
			if (template.Tab == tab)
			{
				Ids.Add(template.id);
			}
		}
		return Ids;
	}
	public static FuWenOpenTemplate GetFuWenOpenTemplateBy_By_Id (int id)
	{
		foreach (FuWenOpenTemplate template in templates)
		{
			if (template.id == id)
			{
				return template;
			}
		}
		return null;
	}
}

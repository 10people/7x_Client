using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenOpenTemplate : XmlLoadManager {

	//id-index，level-君主开启等级，lanweiID-符文栏位id，lanweiType-符文所在栏位类型

	public int id;

	public int level;

	public int lanweiID;

	public int lanweiType;

	public float positionX;

	public float positionY;

	public float positionZ;

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
				t_template.level = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.lanweiID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.lanweiType = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.positionX = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.positionY = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.positionZ = float.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static FuWenOpenTemplate GetFuWenOpenTemplateByLanWeiId (int tempLanWeiId)
	{
		foreach (FuWenOpenTemplate template in templates)
		{
			if (template.lanweiID == tempLanWeiId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get FuWenOpenTemplate with lanweiID " + tempLanWeiId);
		
		return null;
	}
}

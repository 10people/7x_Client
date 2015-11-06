using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FuWenTemplate : XmlLoadManager {

	public int fuwenID;//符文id
	
	public int name;//名字id
	
	public int desc;//描述id
	
	public int type;//道具类型

	public int icon;//图标

	public int color;//边框颜色

	public int fuwenLevel;//符文等级

	public int shuxing;//属性

	public int shuxingValue;//属性值

	public int fuwenFront;//前一级符文

	public int fuwenNext;//后一级符文

	public int shuXingName;//属性名字
	
	public static List<FuWenTemplate> templates = new List<FuWenTemplate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Fuwen.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing("Fuwen");
			
			if (!t_has_items)
			{
				break;
			}
			
			FuWenTemplate t_template = new FuWenTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.fuwenID = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.name = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.color = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenLevel = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.shuxing = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shuxingValue = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenFront = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.fuwenNext = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.shuXingName = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	
	public static FuWenTemplate GetFuWenTemplateByFuWenId (int fuWenId)
	{
		foreach (FuWenTemplate template in templates)
		{
			if (template.fuwenID == fuWenId)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get FuWenTemplate with fuwenID " + fuWenId);
		
		return null;
	}
}

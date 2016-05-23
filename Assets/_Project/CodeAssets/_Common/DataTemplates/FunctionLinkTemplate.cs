using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FunctionLinkTemplate : XmlLoadManager
{
	public int id;
	public string tile;
	public string des;
	public int[] functionID;
	public string[] functionSprite;
	
	public static List<FunctionLinkTemplate> templates = new List<FunctionLinkTemplate>();
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FuncLink.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
		}
		else
		{
			t_reader = XmlReader.Create(new StringReader(www.text));
		}
		
		bool t_has_items = true;
		
		do
		{
			t_has_items = t_reader.ReadToFollowing("FuncLink");
			
			if (!t_has_items)
			{
				break;
			}
			
			FunctionLinkTemplate t_template = new FunctionLinkTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.tile = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.des = t_reader.Value;


				t_reader.MoveToNextAttribute();
				string tempFunctionid = t_reader.Value;
				string[] tempFunctionData = tempFunctionid.Split(',');
				t_template.functionID = new int[tempFunctionData.Length / 2];
				t_template.functionSprite = new string[tempFunctionData.Length / 2];
				for(int i = 0; i < tempFunctionData.Length; i += 2)
				{
					t_template.functionID[i / 2] = int .Parse(tempFunctionData[i]);
					t_template.functionSprite[i / 2] = tempFunctionData[i + 1];
				}
			}
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static FunctionLinkTemplate getGroudById(int id)
	{
		foreach(FunctionLinkTemplate eig in templates)
		{
			if(eig.id == id)
			{
				return eig;
			}
		}
		return null;
	}
}
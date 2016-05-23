using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class JCZCityTemplate : XmlLoadManager {

	public int id;

	public int name;

	public int desc;

	public int type;

	public int allianceLv;

	public int frontPoint;

	public int cost;

	public float zuobiaoX;

	public float zuobiaoY;

	public int icon;

	public string award;

	public string firstAwardId;

	public int npcId;

	public int sceneId;

	public int soundId;

	public int recZhanli;

	public int smaID;

	public int awardShow;

	public string NPCname;

	public static List<JCZCityTemplate> templates = new List<JCZCityTemplate>();
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "JCZCity.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
			t_has_items = t_reader.ReadToFollowing("JCZCity");
			
			if (!t_has_items)
			{
				break;
			}
			
			JCZCityTemplate t_template = new JCZCityTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.name = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.allianceLv = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.frontPoint = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.cost = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.zuobiaoX = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.zuobiaoY = float.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.firstAwardId = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.soundId = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.recZhanli = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.smaID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.awardShow = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.NPCname = t_reader.Value;
			}
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static JCZCityTemplate GetJCZCityTemplateById (int id)
	{
		foreach (JCZCityTemplate template in templates)
		{
			if (template.id == id)
			{
				return template;
			}
		}

		Debug.LogError ("Can not get JCZCityTemplate by id:" + id);
		return null;
	}
}

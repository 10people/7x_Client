//#define DEBUG_RED_ALERT

//#define DEBUG_USE_PUSH_DATA



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class HeroSkillUpTemplate : XmlLoadManager
{
	public int m_iID;
	public string m_sName;
	public string m_sDesc;
	public string m_sSpriteName;
	public int m_iWuqiType;
	public int m_iJinenType;
	public int m_iQuality;
	public int m_iNextID;
	public int m_iNeedLV;
	public int m_iSkillID;
	public int m_iNeedMoney;


	public static List<HeroSkillUpTemplate> templates = new List<HeroSkillUpTemplate>();

	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FunctionOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
			t_has_items = t_reader.ReadToFollowing("JiNengPeiYang");
			
			if (!t_has_items)
			{
				break;
			}
			
			HeroSkillUpTemplate t_template = new HeroSkillUpTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.m_iID = ReadNextInt(t_reader);
				t_template.m_sName = ReadNextString(t_reader);
				t_template.m_sDesc = ReadNextString(t_reader);
				t_template.m_sSpriteName = ReadNextString(t_reader);
				t_template.m_iWuqiType = ReadNextInt(t_reader);
				t_template.m_iJinenType = ReadNextInt(t_reader);
				t_template.m_iQuality = ReadNextInt(t_reader);
				t_template.m_iNextID = ReadNextInt(t_reader);
				t_template.m_iNeedLV = ReadNextInt(t_reader);
				t_template.m_iSkillID = ReadNextInt(t_reader);
				t_template.m_iNeedMoney = ReadNextInt(t_reader);
			}
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}
	

	public static HeroSkillUpTemplate GetHeroSkillUpByID(int id)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].m_iID == id)
			{
				return templates[i];
			}
		}
		return null;
	}
}

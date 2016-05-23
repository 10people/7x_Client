using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Object = UnityEngine.Object;

public class JCZTemplate : XmlLoadManager {

	public static string declaration_startTime = "declaration_startTime";
	public static string declaration_endTime = "declaration_endTime";
	public static string preparation_startTime = "preparation_startTime";
	public static string preparation_endTime = "preparation_endTime";
	public static string fighting_startTime = "fighting_startTime";
	public static string fighting_endTime = "fighting_endTime";
	public static string add_level1 = "add_level1";
	public static string add_level2 = "add_level2";
	public static string add_level3 = "add_level3";
	public static string resurgence_countDown = "resurgence_countDown";
	public static string resurgence_hp = "resurgence_hp";
	public static string resurgence_freeTimes = "resurgence_freeTimes";
	public static string prepare_duration = "prepare_duration";
	public static string fighting_duration = "fighting_duration";
	public static string ending_duration = "ending_duration";
	public static string campSeized_area_radius = "campSeized_area_radius";
	public static string campSeized_initial_value = "campSeized_initial_value";
	public static string campSeized_critical_value = "campSeized_critical_value";
	public static string Seized_value_addSpeed = "Seized_value_addSpeed";
	public static string report_CD = "report_CD";
	public static string patrol_radius = "patrol_radius";
	public static string offline_preserve_time = "offline_preserve_time";
	public static string redElixir_freeNum = "redElixir_freeNum";

	public string key;
	public string desc;
	public string value;

	private static Dictionary<string, JCZTemplate> m_JCZtempDic = new Dictionary<string, JCZTemplate> ();
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{	
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "JCZTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj)
	{
		{
			m_JCZtempDic.Clear();
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
			t_has_items = t_reader.ReadToFollowing("JCZTemp");
			
			if (!t_has_items)
			{
				break;
			}
			
			JCZTemplate t_template = new JCZTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.key = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.desc = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.value = t_reader.Value;
			}

			if (!m_JCZtempDic.ContainsKey (t_template.key))
			{
				try
				{
					m_JCZtempDic.Add (t_template.key,t_template);
				}
				catch (Exception e)
				{
					Debug.LogError("m_JCZtempDic_ex:" + e.Message + ", \n" + e.StackTrace + ", \nkey:" + t_template.key + ", desc:" + t_template.desc + ", value:" + t_template.value);
				}
			}
		}
		while (t_has_items);
	}

	public static JCZTemplate GetJCZTemplateByKey (string key)
	{
		if (m_JCZtempDic.ContainsKey (key))
		{
			return m_JCZtempDic[key];
		}
		else
		{
			Debug.LogError ("Can not get JCZTemplate by key:" + key);
			return null;
		}
	}
}

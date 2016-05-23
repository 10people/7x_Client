using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;

public class VipGiftTemplate : XmlLoadManager {

	public int vip;

	public string award;

	public static List<VipGiftTemplate> templates = new List<VipGiftTemplate>();
	
	
	public static void LoadTemplates(EventDelegate.Callback p_callback = null)
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "VIPGift.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
			t_has_items = t_reader.ReadToFollowing("VIPGift");
			
			if (!t_has_items)
			{
				break;
			}
			
			VipGiftTemplate t_template = new VipGiftTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.vip = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add(t_template);
		}
		while (t_has_items);
	}

	public static VipGiftTemplate GetVipGiftTemplateByVip(int vip)
	{
		for (int i = 0; i < templates.Count; i++)
		{
			if (templates[i].vip == vip)
			{
				return templates[i];
			}
		}

		Debug.LogError ("Can not get VipGiftTemplate by vip:" + vip);
		return null;
	}
}

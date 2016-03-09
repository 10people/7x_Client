using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LMBubbleTemplate : XmlLoadManager {

	public int Buld_Id;
	
	public string Name;
	
	public string triggerFunc;

	public static List<LMBubbleTemplate> templates = new List<LMBubbleTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LMBubblePop.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			templates.Clear ();
		}
		
		XmlReader t_reader = null;
		
		if (obj != null) {
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create (new StringReader (t_text_asset.text));
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		} else {
			t_reader = XmlReader.Create (new StringReader (www.text));
		}
		
		bool t_has_items = true;
		
		do {
			t_has_items = t_reader.ReadToFollowing ("LMBubblePop");
			
			if (!t_has_items) {
				break;
			}
			
			LMBubbleTemplate t_template = new LMBubbleTemplate ();
			
			{
				t_reader.MoveToNextAttribute ();
				t_template.Buld_Id = int.Parse (t_reader.Value);
			
				t_reader.MoveToNextAttribute ();
				t_template.Name = t_reader.Value;
				
				t_reader.MoveToNextAttribute ();
				t_template.triggerFunc = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add (t_template);
		} while( t_has_items );
	}
	public static LMBubbleTemplate getLMBubbleTemplateBy_id(int id)
	{
		foreach (LMBubbleTemplate tempItem in templates)
		{
			if (tempItem.Buld_Id == id)
			{
				return tempItem;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get LMBubbleTemplate with id " + id);
		
		return null;
	}
}
